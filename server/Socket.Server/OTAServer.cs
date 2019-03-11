using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;

namespace Socket.Server
{
    public class OTAServer
    {
        private readonly BufferManager _bufferManager;
        private readonly ILog _logger;
        private readonly int _maxConnectionsAllowed;
        private readonly Semaphore _maxNumberAcceptedClients;
        private readonly SocketAsyncEventArgsPool _saeaPool;
        private int _connectedSocketsCount;
        private System.Net.Sockets.Socket _listenSocket;

        private static byte[] SocketOptionInValue => new byte[]
        {
            1, 0, 0, 0, // 是否启用Keep-Alive
            136, 19, 0, 0, // 多长时间后开始第一次探测（单位：毫秒）,该值是5000毫秒
            136, 19, 0, 0
            //96, 234, 0, 0 // 探测时间间隔（单位：毫秒）,该值是60,000毫秒, BitConverter.GetBytes((uint)60000);
        };

        public OTAServer(int maxConnectionsAllowed, int bufferSize)
        {
            _connectedSocketsCount = 0;

            _maxConnectionsAllowed = maxConnectionsAllowed;

            _bufferManager = new BufferManager(maxConnectionsAllowed, bufferSize);

            _saeaPool = new SocketAsyncEventArgsPool(maxConnectionsAllowed);

            _maxNumberAcceptedClients = new Semaphore(maxConnectionsAllowed, maxConnectionsAllowed);

            _logger = LogManager.GetLogger("OTAServer");
        }

        /// <summary>
        /// 初始化OTA服务器
        /// </summary>
        public void Initialize()
        {
            _bufferManager.InitBuffer();

            for (var i = 0; i < _maxConnectionsAllowed; i++)
            {
                var dmsaea = new DualModeSocketAsyncEventArgs(_bufferManager);
                dmsaea.Closed += ClientClosed;

                // set buffer first for dualmode socket first
                _bufferManager.SetBuffer(dmsaea);

                // push dmsaea into pool, waiting for further using
                _saeaPool.Push(dmsaea);
            }
        }

        /// <summary>
        /// 开启OTA服务器的监听
        /// </summary>
        /// <param name="endPoint">监听配置</param>
        public void Start(IPEndPoint endPoint)
        {
            _listenSocket = new System.Net.Sockets.Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.Bind(endPoint);
            _listenSocket.Listen(100);

            StartAccept(null);

            // todo: start message queue here if need
        }

        /// <summary>
        /// 关闭OTA服务器
        /// </summary>
        public void Stop()
        {
            var clients = _saeaPool.GetAll();

            foreach (var client in clients)
                client?.Close();
        }

        /// <summary>
        /// 开始监听客户端的连接
        /// </summary>
        /// <param name="receiveEventArgs"></param>
        private void StartAccept(SocketAsyncEventArgs receiveEventArgs)
        {
            if (receiveEventArgs == null)
            {
                receiveEventArgs = new SocketAsyncEventArgs();
                receiveEventArgs.Completed += Accept_Completed;
            }
            else
            {
                receiveEventArgs.AcceptSocket = null;
            }

            _maxNumberAcceptedClients.WaitOne();

            var willRaiseEvent = _listenSocket.AcceptAsync(receiveEventArgs);
            if (!willRaiseEvent)
            {
                ProcessAccept(receiveEventArgs);
            }
        }

        /// <summary>
        /// 有客户端新连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    ProcessAccept(e);
                    break;
            }
        }

        /// <summary>
        /// 处理连接事件
        /// </summary>
        /// <param name="receiveEventArgs">收到的客户端Socket</param>
        private void ProcessAccept(SocketAsyncEventArgs receiveEventArgs)
        {
            Interlocked.Increment(ref _connectedSocketsCount);

            _logger.Info($"Connection accepted. {_connectedSocketsCount} clients in total");

            // Get the socket for the accepted client connection and put it into the  
            // ReadEventArg object user token
            var eventArgs = _saeaPool.Pop();

            // As soon as the client is connected, send the client socket into a DualMode saea
            var socketAccepted = receiveEventArgs.AcceptSocket;
            //socketAccepted.DualMode = true;
            socketAccepted.IOControl(IOControlCode.KeepAliveValues, SocketOptionInValue, null);
            eventArgs.UserToken.Socket = socketAccepted;

            eventArgs.ProcessAccept();

            // Accept the next connection request
            StartAccept(receiveEventArgs);
        }

        /// <summary>
        /// 客户端中断连接的事件处理
        /// </summary>
        private void ClientClosed(object sender, DualModeSocketAsyncEventArgs e)
        {
            Interlocked.Decrement(ref _connectedSocketsCount);

            _maxNumberAcceptedClients.Release();

            _logger.Info($"Client {e.GetIdentifier()} closed, {_connectedSocketsCount} clients left");

            _saeaPool.Push(e);
        }
    }
}