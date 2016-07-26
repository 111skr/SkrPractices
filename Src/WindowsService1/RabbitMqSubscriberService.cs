using System.ServiceProcess;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using NLog;

namespace WindowsService1
{
    public partial class RabbitMqSubscriberService : ServiceBase
    {
        private IModel channel;
        private readonly ILogger logger;
        private readonly string queueName;

        public RabbitMqSubscriberService(IModel channel, NLog.ILogger logger, string queueName = "q-hello")
        {
            InitializeComponent();

            this.channel = channel;
            this.logger = logger;
            this.queueName = queueName;

            logger.Info("{0} init", this.GetType().ToString());
        }

        protected override void OnStart(string[] args)
        {
            logger.Info("start");

            string queueName = "q-hello",
                   exchangeName = "ex_hello";

            channel.ExchangeDeclare(exchangeName, "fanout", true, false, null);

            channel.QueueDeclare(queue: queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.QueueBind(queueName, exchangeName, string.Empty);

            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (c, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                logger.Info("Received from queue:{1} msg:{0}", message, queueName);
                (c as EventingBasicConsumer).Model.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Registered += (model, ea) =>
            {
                logger.Info("Registered :{0}", ea.ConsumerTag);
            };

            channel.BasicConsume(queue: queueName,
                                 noAck: false,
                                 consumer: consumer);
        }



        protected override void OnStop()
        {
            logger.Info("stop");
            channel.Close();
        }
    }
}
