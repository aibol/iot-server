using System;
using System.Net;
using System.Net.Sockets;
using Aibol.IOT.Share;
using log4net;

namespace Socket.Server
{
    public class DualModeSocketAsyncEventArgs
    {
        private readonly BufferManager _bufferManager;

        private readonly ILog _logger;

        public DualModeSocketAsyncEventArgs(BufferManager bufferManager)
        {
            _bufferManager = bufferManager;

            UserToken = new AsyncUserToken();

            _logger = LogManager.GetLogger("DualModeSocketAsyncEventArgs");

            ReceiveArgs = new SocketAsyncEventArgs { UserToken = UserToken };
            ReceiveArgs.Completed += IO_Completed;

            SendArgs = new SocketAsyncEventArgs { UserToken = UserToken };
            SendArgs.Completed += IO_Completed;
        }

        public AsyncUserToken UserToken { get; set; }

        /// <summary>
        /// socket only for receiving
        /// </summary>
        public SocketAsyncEventArgs ReceiveArgs { get; set; }

        /// <summary>
        /// socket only for sending
        /// </summary>
        public SocketAsyncEventArgs SendArgs { get; set; }

        /// <summary>
        /// check if socket is connected
        /// </summary>
        public bool Connected => UserToken.Socket != null && UserToken.Socket.Connected;

        public event EventHandler<DualModeSocketAsyncEventArgs> Closed;

        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    ProcessAccept();
                    break;
                case SocketAsyncOperation.Receive:
                    ProcessReceive();
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend();
                    break;
            }
        }

        public void Connect(IPEndPoint remotePoint)
        {
            var socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            UserToken.Socket = socket;
            SendArgs.RemoteEndPoint = remotePoint;

            var willRaiseEvent = socket.ConnectAsync(SendArgs);
            if (!willRaiseEvent)
            {
                ProcessAccept();
            }
        }

        public void ProcessAccept()
        {
            // As client connected, post a receive to the connection
            var willRaiseEvent = UserToken.Socket.ReceiveAsync(ReceiveArgs);
            if (!willRaiseEvent)
            {
                lock (UserToken)
                {
                    ProcessReceive();
                }
            }
        }

        private void ProcessReceive()
        {
            if (ReceiveArgs.BytesTransferred > 0 && ReceiveArgs.SocketError == SocketError.Success)
            {
                // todo: ReceiveArgs handler;

                UserToken.Socket.ReceiveAsync(ReceiveArgs);
            }
            else
            {
                Close();
            }
        }

        private void ProcessSend()
        {
            if (SendArgs.SocketError != SocketError.Success)
            {
                Close();
            }
        }

        public void Send(IDeviceCommand command)
        {
            try
            {
                var bytes = command.GetBytes();

                _bufferManager.AdjustSendBuffer(SendArgs, bytes.Length);

                UserToken.Socket.SendAsync(SendArgs);
            }
            catch (Exception e)
            {
                _logger.Error($"Send Error:{e.Message}");
            }
        }

        public void Close()
        {
            try
            {
                UserToken.Socket.Shutdown(SocketShutdown.Both);
                UserToken.Socket.Close();
            }
            catch (Exception e)
            {
                _logger.Error($"Close Error:{e.Message}");
            }

            Closed?.Invoke(this, this);
        }

        /// <summary>
        /// 获取当前客户端的标识
        /// </summary>
        /// <returns></returns>
        public string GetIdentifier()
        {
            throw new NotImplementedException();
        }
    }
}