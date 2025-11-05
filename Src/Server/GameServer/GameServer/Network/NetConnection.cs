using GameServer.Network;
using System;
using System.Net;
using System.Net.Sockets;

namespace Network
{
    /// <summary>
    /// 与服务器的连接。
    /// </summary>
    public class NetConnection<T> where T : INetSession
    {
        /// <summary>
        /// 表示一个回调，用于通知监听者 ServerConnection 已接收到数据。
        /// </summary>
        /// <param name="sender">回调的发送者。</param>
        /// <param name="e">包含接收到数据的 DataEventArgs 对象。</param>
        public delegate void DataReceivedCallback(NetConnection<T> sender, DataEventArgs e);
        /// <summary>
        /// 表示一个回调，用于通知监听者 ServerConnection 已断开连接。
        /// </summary>
        /// <param name="sender">回调的发送者。</param>
        /// <param name="e">ServerConnection 使用的 SocketAsyncEventArgs 对象。</param>
        public delegate void DisconnectedCallback(NetConnection<T> sender, SocketAsyncEventArgs e);

        #region Internal Classes
        internal class State
        {
            public DataReceivedCallback dataReceived;
            public DisconnectedCallback disconnectedCallback;
            public Socket socket;
        }
        #endregion

        #region Fields
        private SocketAsyncEventArgs eventArgs;

        public PackageHandler<NetConnection<T>> packageHandler;
        #endregion

        #region Constructor
        /// <summary>
        /// 与服务器的连接，始终以异步方式监听。
        /// </summary>
        /// <param name="socket">连接所使用的 Socket。</param>
        /// <param name="args">用于异步接收的 SocketAsyncEventArgs。</param>
        /// <param name="dataReceived">当接收到数据时触发的回调。</param>
        /// <param name="disconnectedCallback">断开连接时触发的回调。</param>
        public NetConnection(Socket socket, SocketAsyncEventArgs args, DataReceivedCallback dataReceived,
            DisconnectedCallback disconnectedCallback, T session)
        {
            lock (this)
            {
                this.packageHandler = new PackageHandler<NetConnection<T>>(this);
                State state = new State()
                {
                    socket = socket,
                    dataReceived = dataReceived,
                    disconnectedCallback = disconnectedCallback
                };
                eventArgs = new SocketAsyncEventArgs();
                eventArgs.AcceptSocket = socket;
                eventArgs.Completed += ReceivedCompleted;
                eventArgs.UserToken = state;
                eventArgs.SetBuffer(new byte[64 * 1024], 0, 64 * 1024);

                BeginReceive(eventArgs);
                this.session = session;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 断开客户端连接。
        /// </summary>
        public void Disconnect()
        {
            lock (this)
            {
                CloseConnection(eventArgs);
            }
        }

        /// <summary>
        /// 向客户端发送数据。
        /// </summary>
        /// <param name="data">要发送的数据。</param>
        /// <param name="offset">数据中的偏移量。</param>
        /// <param name="count">要发送的数据量。</param>
        public void SendData(Byte[] data, Int32 offset, Int32 count)
        {
            lock (this)
            {
                State state = eventArgs.UserToken as State;
                Socket socket = state.socket;
                if (socket.Connected)
                    //socket.Send(data, offset, count, SocketFlags.None);
                    socket.BeginSend(data, 0, count, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            }
        }

        public void SendResponse()
        {
            byte[] data = session.GetResponse();
            if (data == null) return;
            SendData(data, 0, data.Length);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // 从状态对象中获取 socket。
                Socket client = (Socket)ar.AsyncState;

                // 完成向远程设备发送数据。
                int bytesSent = client.EndSend(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        #endregion


        #region Private Methods
        /// <summary>
        /// 开始异步接收。
        /// </summary>
        /// <param name="args">要使用的 SocketAsyncEventArgs。</param>
        private void BeginReceive(SocketAsyncEventArgs args)
        {
            lock (this)
            {
                Socket socket = (args.UserToken as State).socket;
                if (socket.Connected)
                {
                    args.AcceptSocket.ReceiveAsync(args);
                    /*
                    socket.InvokeAsyncMethod(new SocketAsyncMethod(socket.ReceiveAsync),
                        ReceivedCompleted, args);*/
                }
            }
        }

        /// <summary>
        /// 当异步接收完成时调用。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="args">该操作的 SocketAsyncEventArgs。</param>
        private void ReceivedCompleted(Object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred == 0)
            {
                CloseConnection(args); // 正常断开连接
                return;
            }
            if (args.SocketError != SocketError.Success)
            {
                CloseConnection(args); // 非正常断开连接
                return;
            }

            State state = args.UserToken as State;

            Byte[] data = new Byte[args.BytesTransferred];
            Array.Copy(args.Buffer, args.Offset, data, 0, data.Length);
            OnDataReceived(data, args.RemoteEndPoint as IPEndPoint, state.dataReceived);

            BeginReceive(args);
        }

        /// <summary>
        /// 关闭连接。
        /// </summary>
        /// <param name="args">此连接的 SocketAsyncEventArgs。</param>
        private void CloseConnection(SocketAsyncEventArgs args)
        {
            State state = args.UserToken as State;
            Socket socket = state.socket;
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch { } // 如果客户端进程已关闭则会抛出异常
            socket.Close();
            socket = null;

            args.Completed -= ReceivedCompleted; // 必须记住这一点！
            OnDisconnected(args, state.disconnectedCallback);
        }
        #endregion

        #region Events
        /// <summary>
        /// 触发 DataReceivedCallback。
        /// </summary>
        /// <param name="data">接收到的数据。</param>
        /// <param name="remoteEndPoint">数据来源的地址。</param>
        /// <param name="callback">回调函数。</param>
        private void OnDataReceived(Byte[] data, IPEndPoint remoteEndPoint, DataReceivedCallback callback)
        {
            callback(this, new DataEventArgs() { RemoteEndPoint = remoteEndPoint, Data = data, Offset = 0, Length = data.Length });
        }

        /// <summary>
        /// 触发 DisconnectedCallback。
        /// </summary>
        /// <param name="args">此连接的 SocketAsyncEventArgs。</param>
        /// <param name="callback">回调函数。</param>
        private void OnDisconnected(SocketAsyncEventArgs args, DisconnectedCallback callback)
        {
            callback(this, args);
        }
        #endregion

        #region public Property

        /// <summary>
        /// 获取或设置连接的认证状态
        /// true : 已认证
        /// false : 未认证
        /// </summary>
        public bool Verified { get; set; }

        private T session;
        /// <summary>
        /// 获取或设置一个会话对象
        /// </summary>
        public T Session { get { return session; } }

        #endregion
    }
}