using System;
using System.Net;
using System.Net.Sockets;

namespace Network
{
    /// <summary>
    /// 在指定的地址和端口上监听套接字连接。
    /// </summary>
    public class TcpSocketListener : IDisposable
    {
        #region Fields
        private Int32 connectionBacklog;
        private IPEndPoint endPoint;

        private Socket listenerSocket;
        private SocketAsyncEventArgs args;
        #endregion

        #region Properties
        /// <summary>
        /// 连接等待队列的长度。
        /// </summary>
        public Int32 ConnectionBacklog
        {
            get { return connectionBacklog; }
            set
            {
                lock (this)
                {
                    if (IsRunning)
                        throw new InvalidOperationException("服务器运行时无法更改此属性。");
                    else
                        connectionBacklog = value;
                }
            }
        }
        /// <summary>
        /// 要绑定监听套接字的 IPEndPoint。
        /// </summary>
        public IPEndPoint EndPoint
        {
            get { return endPoint; }
            set
            {
                lock (this)
                {
                    if (IsRunning)
                        throw new InvalidOperationException("服务器运行时无法更改此属性。");
                    else
                        endPoint = value;
                }
            }
        }
        /// <summary>
        /// 该类当前是否正在监听。
        /// </summary>
        public Boolean IsRunning
        {
            get { return listenerSocket != null; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// 在指定的地址和端口上监听套接字连接。
        /// </summary>
        /// <param name="address">要监听的地址。</param>
        /// <param name="port">要监听的端口。</param>
        /// <param name="connectionBacklog">连接等待队列的最大长度（即允许挂起的连接请求数量）。</param>
        public TcpSocketListener(String address, Int32 port, Int32 connectionBacklog)
            : this(IPAddress.Parse(address), port, connectionBacklog)
        { }

        /// <summary>
        /// 在指定的地址和端口上监听套接字连接。
        /// </summary>
        /// <param name="address">要监听的地址（以 IPAddress 形式表示）。</param>
        /// <param name="port">要监听的端口。</param>
        /// <param name="connectionBacklog">连接等待队列的最大长度。</param>
        public TcpSocketListener(IPAddress address, Int32 port, Int32 connectionBacklog)
            : this(new IPEndPoint(address, port), connectionBacklog)
        { }

        /// <summary>
        /// 在指定的终结点上监听套接字连接。
        /// </summary>
        /// <param name="endPoint">要监听的终结点（包含 IP 地址和端口号）。</param>
        /// <param name="connectionBacklog">连接等待队列的最大长度。</param>
        public TcpSocketListener(IPEndPoint endPoint, Int32 connectionBacklog)
        {
            this.endPoint = endPoint;
            args = new SocketAsyncEventArgs();
            args.Completed += OnSocketAccepted;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 开始监听套接字连接。
        /// </summary>
        public void Start()
        {
            lock (this)
            {
                if (!IsRunning)
                {
                    listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    listenerSocket.Bind(endPoint);
                    listenerSocket.Listen(connectionBacklog);
                    BeginAccept(args);
                }
                else
                    throw new InvalidOperationException("服务器已经在运行。");
            }

        }

        /// <summary>
        /// 停止监听套接字连接。
        /// </summary>
        public void Stop()
        {
            lock (this)
            {
                if (listenerSocket == null)
                    return;
                listenerSocket.Close();
                listenerSocket = null;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 异步监听新连接。
        /// </summary>
        /// <param name="args"></param>
        private void BeginAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;
            listenerSocket.AcceptAsync(args);
            /*listenerSocket.InvokeAsyncMethod(new SocketAsyncMethod(listenerSocket.AcceptAsync)
                , OnSocketAccepted, args);*/
        }

        /// <summary>
        /// 当异步接受操作完成时调用。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">该操作的 SocketAsyncEventArgs。</param>
        private void OnSocketAccepted(object sender, SocketAsyncEventArgs e)
        {
            SocketError error = e.SocketError;
            if (e.SocketError == SocketError.OperationAborted)
                return; // 服务器已停止

            if (e.SocketError == SocketError.Success)
            {
                Socket handler = e.AcceptSocket;
                OnSocketConnected(handler);
            }

            lock (this)
            {
                BeginAccept(e);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// 当收到新连接时触发。
        /// </summary>
        public event EventHandler<Socket> SocketConnected;

        /// <summary>
        /// 触发 SocketConnected 事件。
        /// </summary>
        /// <param name="client">新的客户端套接字。</param>
        private void OnSocketConnected(Socket client)
        {
            if (SocketConnected != null)
                SocketConnected(this, client);
        }
        #endregion

        #region IDisposable Members
        private Boolean disposed = false;

        ~TcpSocketListener()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Stop();
                    if (args != null)
                        args.Dispose();
                }

                disposed = true;
            }
        }
        #endregion
    }
}