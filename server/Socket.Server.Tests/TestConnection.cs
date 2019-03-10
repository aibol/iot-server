using System.Net;
using System.Net.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Socket.Server.Tests
{
    [TestClass]
    public class TestConnection
    {
        [TestMethod]
        public void TestConsoleServer()
        {
            var ipe = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8044);
            var clientSocket =
                new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(ipe);

            clientSocket.Close();
        }
    }
}