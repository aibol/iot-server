namespace Socket.Server
{
    /// <summary>
    /// This class is designed for use as the object to be assigned to the SocketAsyncEventArgs.UserToken property. 
    /// </summary>
    public class AsyncUserToken
    {
        public AsyncUserToken() : this(null)
        {
        }

        public AsyncUserToken(System.Net.Sockets.Socket socket)
        {
            Socket = socket;
        }

        public System.Net.Sockets.Socket Socket { get; set; }
    }
}