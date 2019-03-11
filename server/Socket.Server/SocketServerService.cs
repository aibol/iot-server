using System;
using System.Configuration;
using System.Net;
using System.ServiceProcess;
using log4net;

namespace Socket.Server
{
    internal partial class SocketServerService : ServiceBase
    {
        private readonly ILog _logger;
        private OTAServer _server;

        public SocketServerService()
        {
            InitializeComponent();
            _logger = LogManager.GetLogger("SocketServerService");
        }

        protected override void OnStart(string[] args)
        {
            _logger.Info("service is going to start");

            try
            {
                var maxConnections = int.Parse(ConfigurationManager.AppSettings["MaxConnections"]);
                var listenIp = ConfigurationManager.AppSettings["ListenIP"];
                var listenPort = int.Parse(ConfigurationManager.AppSettings["ListenPort"]);
                var bufferSize = int.Parse(ConfigurationManager.AppSettings["BufferSize"]);

                _server = new OTAServer(maxConnections, bufferSize);
                _server.Initialize();
                _server.Start(new IPEndPoint(IPAddress.Parse(listenIp), listenPort));

                _logger.Info($"service started, listening on {listenIp}:{listenPort}");
            }
            catch (Exception e)
            {
                _logger.Error($"service started error:{e.Message}");
                throw;
            }
        }

        protected override void OnStop()
        {
            _server?.Stop();

            _logger.Info("service stop");
        }
    }
}