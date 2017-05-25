using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Autofac;
using NLog;

namespace Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            ContainerBuilder builder = InitIocContainer();

            var container = builder.Build();

            var logger = container.Resolve<NLog.ILogger>();

            logger.Info("Publisher Start....");

            SendRabbitMqMsg(container.Resolve<IConnection>(), logger);

            logger.Info("Publisher End....");
        }

        private static void SendRabbitMqMsg(IConnection connection, NLog.ILogger logger)
        {
            using (connection)
            {
                var commandStr = string.Empty;
                using (var channel = connection.CreateModel())
                {
                    string exchangeName = "ex_hello";
                    channel.ExchangeDeclare(exchangeName, "fanout", true, false, null);

                    foreach (string queueName in new string[] { "q-hello", "q-hello2" })
                    {
                    
                        channel.QueueDeclare(queue: queueName,
                                             durable: true,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        channel.QueueBind(queueName, exchangeName, string.Empty);

                    }

                    while (commandStr != "q")
                    {

                        string message = string.Format("Hello World! [{0}]", Guid.NewGuid().ToString("d"));
                        var body = Encoding.UTF8.GetBytes(message);

                        var properties = channel.CreateBasicProperties();
                        properties.DeliveryMode = 2;
                        properties.Persistent = true;

                        channel.BasicPublish(exchange: exchangeName,
                                             routingKey: string.Empty,
                                             basicProperties: properties,
                                             body: body);

                        logger.Info("Sent msg: {0}", message);
                        Console.WriteLine(" Press anykey to send msg,[q] to exit");
                        commandStr = Console.ReadLine();
                    }
                }
            }
        }

        private static ContainerBuilder InitIocContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<AutoFacModule>();

            return builder;
        }

    }
}