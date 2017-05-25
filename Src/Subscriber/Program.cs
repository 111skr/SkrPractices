using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;

namespace Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "192.168.87.187",
                Port = 35672,
                UserName = "ibitauto",
                Password = "ibitauto",
                VirtualHost = "VH-Test"
            };

            var connection = factory.CreateConnection();

            foreach (string queueName in new string[] { "q-hello", "q-hello2" })
            {
                //var channel = connection.CreateModel();

                var rabbitSubscribModel = new RabbitSubscribModel()
                {
                    Model = connection.CreateModel(),
                    QueueName = queueName
                };

                //Task.Factory.StartNew((m) => {

                //var channel = (m as RabbitSubscribModel).Model;
                //string currentQueueName = (m as RabbitSubscribModel).QueueName;

                var channel = rabbitSubscribModel.Model;
                string currentQueueName = rabbitSubscribModel.QueueName;

                channel.QueueDeclare(queue: currentQueueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (c, ea) =>
                {
                    var currentConsumer = (c as EventingBasicConsumer);
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("Task.CurrentId:{2} [x] Received From Queue {1} {0}", message, currentQueueName, Task.CurrentId);
                    currentConsumer.Model.BasicAck(ea.DeliveryTag, false);
                };

                Console.WriteLine("Task.CurrentId:{0}", Task.CurrentId);
                channel.BasicConsume(queue: currentQueueName,
                                     noAck: false,
                                     consumer: consumer);

                //}, rabbitSubscribModel);
            }

            Console.ReadLine();
        }

        public class RabbitSubscribModel
        {
            public IModel Model { get; set; }
            public string QueueName { get; set; }
        }

        private static void SingleSubscrib()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "192.168.87.187",
                Port = 35672,
                UserName = "ibitauto",
                Password = "ibitauto",
                VirtualHost = "VH-Test"
            };
            using (var connection = factory.CreateConnection())
            {
                foreach (string queueName in new string[] { "q-hello", "q-hello2" })
                {
                    var channel = connection.CreateModel();

                    channel.QueueDeclare(queue: queueName.ToString(),
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (c, ea) =>
                    {
                        var currentConsumer = (c as EventingBasicConsumer);
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine("Task.CurrentId:{2} [x] Received From Queue {1} {0}", message, queueName.ToString(), Task.CurrentId);
                        currentConsumer.Model.BasicAck(ea.DeliveryTag, false);
                    };
                    Task.Factory.StartNew(() =>
                    {
                        Console.WriteLine("Task.CurrentId:{0}", Task.CurrentId);
                        channel.BasicConsume(queue: queueName,
                                             noAck: false,
                                             consumer: consumer);
                    });

                }
            }
        }
    }
}
