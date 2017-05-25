using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using NLog;
using RabbitMQ;
using RabbitMQ.Client;

namespace Publisher
{
    public class AutoFacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<ILogger>(c => LogManager.GetCurrentClassLogger());

            ushort heartBeat = 12000;
            builder.Register<IConnectionFactory>(c => new ConnectionFactory()
            {
                HostName = "192.168.87.187",
                Port = 35672,
                UserName = "ibitauto",
                Password = "ibitauto",
                VirtualHost = "VH-Test",
                RequestedConnectionTimeout = 120000,
                AutomaticRecoveryEnabled = true,
                RequestedHeartbeat = heartBeat
            }).SingleInstance();

            builder.Register<IConnection>(c => c.Resolve<IConnectionFactory>().CreateConnection());

        }
    }
}
