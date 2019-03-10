using System.IO;
using System.Net;
using System.Reflection;
using log4net;
using log4net.Config;
using Socket.Server;

namespace Console.Server
{
    internal class Program
    {
        private static ILog _logger;

        private static void Main(string[] args)
        {
            InitializeLogger();

            var server = new OTAServer(100, 1024);
            server.Initialize();
            server.Start(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8044));

            System.Console.WriteLine("server started, listening on 127.0.0.1:8044");

            System.Console.ReadLine();
        }

        private static void InitializeLogger()
        {
            var assemblyFilePath = Assembly.GetExecutingAssembly().Location;
            var assemblyDirPath = Path.GetDirectoryName(assemblyFilePath);
            var configFilePath = assemblyDirPath + "\\log4net.config";
            var col = XmlConfigurator.ConfigureAndWatch(new FileInfo(configFilePath));
            _logger = LogManager.GetLogger("SocketServer");
            _logger.Info("logger created successfully");
        }
    }
}