using System.Configuration;
using System.Net;
using System.ServiceProcess;
using log4net;

namespace Socket.Server
{
    internal partial class SocketServerService : ServiceBase
    {
        private readonly ILog _logger;

        public SocketServerService()
        {
            InitializeComponent();
            _logger = LogManager.GetLogger("SocketServerService");
        }

        protected override void OnStart(string[] args)
        {
            _logger.Info("service is going to start");

            var maxConnections = int.Parse(ConfigurationManager.AppSettings["MaxConnections"]);
            var listenIp = ConfigurationManager.AppSettings["ListenIP"];
            var listenPort = int.Parse(ConfigurationManager.AppSettings["ListenPort"]);
            var bufferSize = int.Parse(ConfigurationManager.AppSettings["BufferSize"]);

            var server = new OTAServer(maxConnections, bufferSize);
            server.Initialize();
            server.Start(new IPEndPoint(IPAddress.Parse(listenIp), listenPort));

            _logger.Info("service started");
        }

        protected override void OnStop()
        {
            _logger.Info("service stop");
        }
    }
}