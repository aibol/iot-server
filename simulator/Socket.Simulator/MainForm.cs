using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Socket.Simulator
{
    public partial class MainForm : Form
    {
        private System.Net.Sockets.Socket _socket;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(tbxPort.Text, out var port))
            {
                lblMessage.Text = "error port";
                return;
            }

            var ipe = new IPEndPoint(IPAddress.Parse(tbxIP.Text), port);
            _socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            try
            {
                _socket.Connect(ipe);
                lblMessage.Text = "server connected";
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            var datagram = tbxDatagram.Text;
            if (string.IsNullOrEmpty(datagram))
            {
                MessageBox.Show("please input datagram");
                return;
            }

            var bytes = Encoding.UTF8.GetBytes(datagram);

            _socket.Send(bytes);
            lvDatagram.Items.Add($"发送: {datagram}");

            var bytesReceived = new byte[1024];
            var length = _socket.Receive(bytesReceived);

            var message = Encoding.UTF8.GetString(bytesReceived, 0, length);
            lvDatagram.Items.Add($"服务器返回: {message}");
        }
    }
}