using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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
            using (var connection = factory.CreateConnection())
            {
                var queueName = "q-hello";

                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queueName,
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);
                    };
                    channel.BasicConsume(queue: queueName,
                                         noAck: true,
                                         consumer: consumer);


                }
            }
            Console.ReadLine();
        }
    }
}
