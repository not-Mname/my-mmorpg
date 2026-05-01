using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using UnityEngine;
using SkillBridge.Message;

namespace Network
{
    /// <summary>
    /// 网络客户端单例类
    /// 负责管理与服务器的TCP连接、消息发送与接收、重连机制等
    /// </summary>
    class NetClient : MonoSingleton<NetClient>
    {

        const int DEF_POLL_INTERVAL_MILLISECONDS = 100; //默认网络线程轮询间隔（毫秒）
        const int DEF_TRY_CONNECT_TIMES = 3;            //默认服务器重试连接次数
        const int DEF_RECV_BUFFER_SIZE = 64 * 1024;     //默认接收缓冲区初始大小
        const int DEF_PACKAGE_HEADER_LENGTH = 4;        //默认数据包头部长度
        const int DEF_SEND_PING_INTERVAL = 30;          //默认发送Ping包间隔（秒）
        const int NetConnectTimeout = 10000;    //默认连接超时等待时间（毫秒）
        const int DEF_LOAD_WHEEL_MILLISECONDS = 1000;   //默认显示加载转圈前的等待时间（毫秒）
        const int NetReconnectPeriod = 10;              //默认重连周期（秒）

        public const int NET_ERROR_UNKNOW_PROTOCOL = 2;           //协议错误
        public const int NET_ERROR_SEND_EXCEPTION = 1000;       //发送异常
        public const int NET_ERROR_ILLEGAL_PACKAGE = 1001;      //接收到错误数据包
        public const int NET_ERROR_ZERO_BYTE = 1002;            //收发0字节
        public const int NET_ERROR_PACKAGE_TIMEOUT = 1003;      //收包超时
        public const int NET_ERROR_PROXY_TIMEOUT = 1004;        //Proxy超时
        public const int NET_ERROR_FAIL_TO_CONNECT = 1005;      //3次连接失败
        public const int NET_ERROR_PROXY_ERROR = 1006;          //Proxy重启
        public const int NET_ERROR_ON_DESTROY = 1007;           //销毁时关闭网络连接
        public const int NET_ERROR_ON_KICKOUT = 25;           //被踢出服务器

        // 定义连接事件委托
        public delegate void ConnectEventHandler(int result, string reason);
        // 定义期望包超时/恢复事件委托
        public delegate void ExpectPackageEventHandler();

        // 连接成功事件
        public event ConnectEventHandler OnConnect;
        // 断开连接事件
        public event ConnectEventHandler OnDisconnect;
        // 期望包超时事件
        public event ExpectPackageEventHandler OnExpectPackageTimeout;
        // 期望包恢复事件
        public event ExpectPackageEventHandler OnExpectPackageResume;

        //socket实例
        private IPEndPoint address; //服务器地址 endpoint
        private Socket clientSocket; //客户端Socket对象
        private MemoryStream sendBuffer = new MemoryStream(); //发送缓冲区
        private MemoryStream receiveBuffer = new MemoryStream(DEF_RECV_BUFFER_SIZE); //接收缓冲区
        private Queue<NetMessage> sendQueue = new Queue<NetMessage>(); //待发送消息队列

        private bool connecting = false; //是否正在连接中

        private int retryTimes = 0; //当前重试次数
        private int retryTimesTotal = DEF_TRY_CONNECT_TIMES; //最大重试次数
        private float lastSendTime = 0; //上次发送时间
        private int sendOffset = 0; //发送缓冲区偏移量

        // 获取当前是否处于运行状态
        public bool running { get; set; }

        // 数据包处理器
        public PackageHandler packageHandler = new PackageHandler(null);

        /// <summary>
        /// 启动时初始化
        /// </summary>
        protected override void OnAwake()
        {
            running = true;
            // 设置消息分发器抛出异常以便调试
            MessageDistributer.Instance.ThrowException = true;
        }

        /// <summary>
        /// 触发连接成功事件
        /// </summary>
        /// <param name="result">结果代码，0表示成功</param>
        /// <param name="reason">原因描述</param>
        protected virtual void RaiseConnected(int result, string reason)
        {
            ConnectEventHandler handler = OnConnect;
            if (handler != null)
            {
                handler(result, reason);
            }
        }

        /// <summary>
        /// 触发断开连接事件
        /// </summary>
        /// <param name="result">错误代码</param>
        /// <param name="reason">原因描述</param>
        public virtual void RaiseDisonnected(int result, string reason = "")
        {
            ConnectEventHandler handler = OnDisconnect;
            if (handler != null)
            {
                handler(result, reason);
            }
        }

        /// <summary>
        /// 触发期望包超时事件
        /// </summary>
        protected virtual void RaiseExpectPackageTimeout()
        {
            ExpectPackageEventHandler handler = OnExpectPackageTimeout;
            if (handler != null)
            {
                handler();
            }
        }

        /// <summary>
        /// 触发期望包恢复事件
        /// </summary>
        protected virtual void RaiseExpectPackageResume()
        {
            ExpectPackageEventHandler handler = OnExpectPackageResume;
            if (handler != null)
            {
                handler();
            }
        }

        /// <summary>
        /// 获取当前Socket连接状态
        /// </summary>
        public bool Connected
        {
            get
            {
                return (clientSocket != default(Socket)) ? clientSocket.Connected : false;
            }
        }

        public NetClient()
        {
        }

        /// <summary>
        /// 重置网络客户端状态
        /// 清空消息队列、缓冲区，重置连接标志和计数器，清理事件绑定
        /// </summary>
        public void Reset()
        {
            MessageDistributer.Instance.Clear();
            this.sendQueue.Clear();

            this.sendOffset = 0;

            this.connecting = false;

            this.retryTimes = 0;
            this.lastSendTime = 0;

            // 清理事件订阅，防止内存泄漏或重复触发
            this.OnConnect = null;
            this.OnDisconnect = null;
            this.OnExpectPackageTimeout = null;
            this.OnExpectPackageResume = null;
        }

        /// <summary>
        /// 初始化服务器地址
        /// </summary>
        /// <param name="serverIP">服务器IP地址字符串</param>
        /// <param name="port">服务器端口号</param>
        public void Init(string serverIP, int port)
        {
            this.address = new IPEndPoint(IPAddress.Parse(serverIP), port);
        }

        /// <summary>
        /// 发起异步连接
        /// 请通过OnConnect事件处理连接结果
        /// </summary>
        /// <param name="times">最大重试次数，默认为DEF_TRY_CONNECT_TIMES</param>
        public void Connect(int times = DEF_TRY_CONNECT_TIMES)
        {
            // 如果正在连接中，直接返回，避免重复连接
            if (this.connecting)
            {
                return;
            }

            // 如果已有Socket实例，先关闭它
            if (this.clientSocket != null)
            {
                this.clientSocket.Close();
            }
            
            // 检查是否已初始化地址
            if (this.address == default(IPEndPoint))
            {
                throw new Exception("Please Init first.");
            }
            
            Debug.Log("DoConnect");
            this.connecting = true;
            this.lastSendTime = 0;

            // 执行实际连接逻辑
            this.DoConnect();
        }

        /// <summary>
        /// 销毁时调用，关闭网络连接
        /// </summary>
        public void OnDestroy()
        {
            Debug.Log("OnDestroy NetworkManager.");
            this.CloseConnection(NET_ERROR_ON_DESTROY);
        }

        /// <summary>
        /// 关闭网络连接并处理相应错误逻辑
        /// </summary>
        /// <param name="errCode">错误代码</param>
        public void CloseConnection(int errCode)
        {
            Debug.LogWarning("CloseConnection(), errorCode: " + errCode.ToString());
            this.connecting = false;
            
            // 关闭Socket
            if (this.clientSocket != null)
            {
                this.clientSocket.Close();
            }

            //清空缓冲区及消息队列
            MessageDistributer.Instance.Clear();
            this.sendQueue.Clear();

            // 重置缓冲区位置指针
            this.receiveBuffer.Position = 0;
            this.sendBuffer.Position = sendOffset = 0;

            // 根据错误代码执行不同逻辑
            switch (errCode)
            {
                case NET_ERROR_UNKNOW_PROTOCOL:
                    {
                        //致命错误，停止网络服务
                        this.running = false;
                    }
                    break;
                case NET_ERROR_FAIL_TO_CONNECT:
                case NET_ERROR_PROXY_TIMEOUT:
                case NET_ERROR_PROXY_ERROR:
                    // 这些错误可能需要上层逻辑处理重连或提示，此处暂留空
                    //NetworkManager.Instance.dropCurMessage();
                    //NetworkManager.Instance.Connect();
                    break;
                //离线处理：触发断开连接事件
                case NET_ERROR_ON_KICKOUT:
                case NET_ERROR_ZERO_BYTE:
                case NET_ERROR_ILLEGAL_PACKAGE:
                case NET_ERROR_SEND_EXCEPTION:
                case NET_ERROR_PACKAGE_TIMEOUT:
                default:
                    this.lastSendTime = 0;
                    this.RaiseDisonnected(errCode);
                    break;
            }

        }

        /// <summary>
        /// 发送Protobuf消息
        /// 将消息加入发送队列，如果未连接则尝试连接
        /// </summary>
        /// <param name="message">要发送的网络消息对象</param>
        public void SendMessage(NetMessage message)
        {
            // 如果网络模块未运行，直接返回
            if (!running)
            {
                return;
            }

            // 如果未连接，重置缓冲区并尝试连接
            if (!this.Connected)
            {
                this.receiveBuffer.Position = 0;
                this.sendBuffer.Position = sendOffset = 0;

                this.Connect();
                Debug.Log("Connect Server before Send Message!");
                return;
            }

            // 将消息入队，等待Update中发送
            sendQueue.Enqueue(message);

            // 记录首次发送时间
            if (this.lastSendTime == 0)
            {
                this.lastSendTime = Time.time;
            }
        }

        /// <summary>
        /// 执行实际连接操作
        /// 使用阻塞式BeginConnect/EndConnect实现带超时的连接
        /// </summary>
        void DoConnect()
        {
            Debug.Log("NetClient.DoConnect on " + this.address.ToString());
            try
            {
                // 确保旧Socket已关闭
                if (this.clientSocket != null)
                {
                    this.clientSocket.Close();
                }

                // 创建新的TCP Socket
                this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // 连接阶段设为阻塞模式，以便等待连接结果
                this.clientSocket.Blocking = true;

                Debug.Log(string.Format("Connect[{0}] to server {1}", this.retryTimes, this.address) + "\n");
                
                // 开始异步连接
                IAsyncResult result = this.clientSocket.BeginConnect(this.address, null, null);
                // 等待连接完成或超时
                bool success = result.AsyncWaitHandle.WaitOne(NetConnectTimeout);
                
                if (success)
                {
                    // 结束异步连接，获取最终状态
                    this.clientSocket.EndConnect(result);
                }
            }
            catch (SocketException ex)
            {
                // 处理连接被拒绝等Socket异常
                if (ex.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    this.CloseConnection(NET_ERROR_FAIL_TO_CONNECT);
                }
                Debug.LogErrorFormat("DoConnect SocketException:[{0},{1},{2}]{3} ", ex.ErrorCode, ex.SocketErrorCode, ex.NativeErrorCode, ex.ToString());
            }
            catch (Exception e)
            {
                // 处理其他异常
                Debug.Log("DoConnect Exception:" + e.ToString() + "\n");
            }

            // 检查连接是否成功建立
            if (this.clientSocket.Connected)
            {
                // 连接成功后，将Socket设为非阻塞模式，用于后续数据收发
                this.clientSocket.Blocking = false;
                this.RaiseConnected(0, "Success");
            }
            else
            {
                // 连接失败，增加重试计数
                this.retryTimes++;
                // 如果达到最大重试次数，通知连接失败
                if (this.retryTimes >= this.retryTimesTotal)
                {
                    this.RaiseConnected(1, "Cannot connect to server");
                }
            }
            // 标记连接过程结束
            this.connecting = false;
        }

        /// <summary>
        /// 保持连接状态
        /// 如果断开且未达到最大重试次数，则尝试重连
        /// </summary>
        /// <returns>如果已连接或正在连接返回true，否则返回false</returns>
        bool KeepConnect()
        {
            // 如果正在连接中，不执行其他操作
            if (this.connecting)
            {
                return false;
            }
            
            // 如果地址未初始化，无法连接
            if (this.address == null)
                return false;

            // 如果已连接，直接返回true
            if (this.Connected)
            {
                return true;
            }

            // 如果未连接且重试次数未满，发起重连
            if (this.retryTimes < this.retryTimesTotal)
            {
                this.Connect();
            }
            return false;
        }

        /// <summary>
        /// 处理接收数据
        /// 检查Socket状态，读取数据并交给PackageHandler处理
        /// </summary>
        /// <returns>如果处理成功返回true，发生错误返回false</returns>
        bool ProcessRecv()
        {
            bool ret = false;
            try
            {
                // 调试检查：确保Socket处于非阻塞模式
                if (this.clientSocket.Blocking)
                {
                    Debug.Log("this.clientSocket.Blocking = true\n");
                }
                
                // 检查Socket是否有错误
                bool error = this.clientSocket.Poll(0, SelectMode.SelectError);
                if (error)
                {
                    Debug.Log("ProcessRecv Poll SelectError\n");
                    this.CloseConnection(NET_ERROR_SEND_EXCEPTION);
                    return false;
                }

                // 检查Socket是否可读
                ret = this.clientSocket.Poll(0, SelectMode.SelectRead);
                if (ret)
                {
                    // 接收数据到缓冲区
                    int n = this.clientSocket.Receive(this.receiveBuffer.GetBuffer(), 0, this.receiveBuffer.Capacity, SocketFlags.None);
                    
                    // 如果接收字节数<=0，表示连接关闭或出错
                    if (n <= 0)
                    {
                        this.CloseConnection(NET_ERROR_ZERO_BYTE);
                        return false;
                    }

                    // 将接收到的数据交给包处理器解析
                    this.packageHandler.ReceiveData(this.receiveBuffer.GetBuffer(), 0, n);

                }
            }
            catch (Exception e)
            {
                // 捕获接收过程中的异常
                Debug.Log("ProcessReceive exception:" + e.ToString() + "\n");
                this.CloseConnection(NET_ERROR_ILLEGAL_PACKAGE);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 处理发送数据
        /// 检查Socket状态，从发送队列取包或直接发送缓冲区剩余数据
        /// </summary>
        /// <returns>如果处理成功返回true，发生错误返回false</returns>
        bool ProcessSend()
        {
            bool ret = false;
            try
            {
                // 调试检查：确保Socket处于非阻塞模式
                if (this.clientSocket.Blocking)
                {
                    Debug.Log("this.clientSocket.Blocking = true\n");
                }
                
                // 检查Socket是否有错误
                bool error = this.clientSocket.Poll(0, SelectMode.SelectError);
                if (error)
                {
                    Debug.Log("ProcessSend Poll SelectError\n");
                    this.CloseConnection(NET_ERROR_SEND_EXCEPTION);
                    return false;
                }
                
                // 检查Socket是否可写
                ret = this.clientSocket.Poll(0, SelectMode.SelectWrite);
                if (ret)
                {
                    // 如果发送缓冲区中有未发送完的数据
                    if (this.sendBuffer.Position > this.sendOffset)
                    {
                        // 计算剩余待发送数据大小
                        int bufsize = (int)(this.sendBuffer.Position - this.sendOffset);
                        // 发送数据
                        int n = this.clientSocket.Send(this.sendBuffer.GetBuffer(), this.sendOffset, bufsize, SocketFlags.None);
                        
                        // 如果发送字节数<=0，表示连接关闭或出错
                        if (n <= 0)
                        {
                            this.CloseConnection(NET_ERROR_ZERO_BYTE);
                            return false;
                        }
                        
                        // 更新发送偏移量
                        this.sendOffset += n;
                        
                        // 如果所有数据都已发送完毕
                        if (this.sendOffset >= this.sendBuffer.Position)
                        {
                            // 重置缓冲区和偏移量
                            this.sendOffset = 0;
                            this.sendBuffer.Position = 0;
                            // 从队列中移除已发送的消息
                            this.sendQueue.Dequeue();
                        }
                    }
                    else
                    {
                        // 如果缓冲区为空，从发送队列中获取下一个消息
                        if (this.sendQueue.Count > 0)
                        {
                            NetMessage message = this.sendQueue.Peek();
                            // 打包消息
                            byte[] package = PackageHandler.PackMessage(message);
                            // 写入发送缓冲区
                            this.sendBuffer.Write(package, 0, package.Length);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // 捕获发送过程中的异常
                Debug.LogError("ProcessSend exception:" + e.ToString() + "\n");
                this.CloseConnection(NET_ERROR_SEND_EXCEPTION);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 处理接收到的消息分发
        /// 调用消息分发器处理已解析的消息
        /// </summary>
        void ProceeMessage()
        {
            MessageDistributer.Instance.Distribute();
        }

        /// <summary>
        /// Unity Update方法，每帧调用
        /// 负责维持连接、接收数据、发送数据和消息分发
        /// </summary>
        public void Update()
        {
            // 如果网络模块未运行，直接返回
            if (!running)
            {
                return;
            }

            // 尝试保持连接状态
            if (this.KeepConnect())
            {
                // 处理接收数据
                if (this.ProcessRecv())
                {
                    // 如果连接正常，继续处理发送和消息分发
                    if (this.Connected)
                    {
                        this.ProcessSend();
                        this.ProceeMessage();
                    }
                }
            }
        }
    }
}