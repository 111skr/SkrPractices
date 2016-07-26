using Autofac;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace WindowsService1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ContainerBuilder builder = InitIocContainer();

            var container = builder.Build();

            var logger = container.Resolve<NLog.ILogger>();

            var rabbitConn = container.Resolve<IConnection>();

            logger.Info("init Services");

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new RabbitMqSubscriberService(rabbitConn.CreateModel(), container.Resolve<NLog.ILogger>())
            };

            logger.Info("Main begin run Services");
            ServiceBase.Run(ServicesToRun);
        }

        private static ContainerBuilder InitIocContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<AutoFacModule>();

            return builder;
        }
    }
}
