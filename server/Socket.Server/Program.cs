using System;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using log4net;
using log4net.Config;

namespace Socket.Server
{
    internal static class Program
    {
        private static ILog _logger;

        private static void Main(string[] args)
        {
            InitializeLogger();

            if (args.Length > 0)
            {
                _logger.Info($"additional args(count: {args.Length}) exist");

                foreach (var arg in args)
                    switch (arg)
                    {
                        case "-i":
                        case "-install":
                            InstallService();
                            break;
                        default:
                            RunService();
                            break;
                    }
            }
            else
            {
                RunService();
            }
        }

        private static void InitializeLogger()
        {
            var assemblyFilePath = Assembly.GetExecutingAssembly().Location;
            var assemblyDirPath = Path.GetDirectoryName(assemblyFilePath);
            var configFilePath = assemblyDirPath + "\\log4net.config";
            XmlConfigurator.ConfigureAndWatch(new FileInfo(configFilePath));
            _logger = LogManager.GetLogger("SocketServer");
            _logger.Info("logger created successfully");
        }

        private static void InstallService()
        {
            const string serviceName = "SocketServer";
            string[] args = {"Socket.Server.exe"};

            var serviceExisted = ServiceController.GetServices().Any(s => s.ServiceName == serviceName);

            if (!serviceExisted)
            {
                try
                {
                    _logger.Info($"installing service {serviceName}");
                    ManagedInstallerClass.InstallHelper(args);
                    _logger.Info($"installing service {serviceName} succeed");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    Console.WriteLine(ex);
                }
            }
            else
            {
                _logger.Info($"service {serviceName} already exists, no need to install");
                Console.WriteLine("该服务已经存在，不用重复安装。");
            }
        }

        private static void RunService()
        {
            _logger.Info("start to run service Socket Server");

            var servicesToRun = new ServiceBase[]
            {
                new SocketServerService()
            };

            ServiceBase.Run(servicesToRun);
        }
    }
}