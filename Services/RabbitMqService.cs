using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MapLogger
{
    public class RabbitMqService
    {
        private readonly string _hostname;
        private const string QueueName = "logQueue";
        private readonly DbUpdateService _dbUpdateService;
        private readonly LogUpdateService _logUpdateService;

        public RabbitMqService(DbUpdateService dbUpdateService, LogUpdateService logUpdateService, string hostname = "localhost")
        {
            _hostname = hostname;
            _dbUpdateService = dbUpdateService;
            _logUpdateService = logUpdateService;
        }

        public void SendLogToRabbitMQ(string mapType, double longitude, double latitude)
        {
            var factory = new ConnectionFactory() { 
                HostName = _hostname, 
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            try
            {
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(queue: QueueName, 
                                    durable: false, 
                                    exclusive: false, 
                                    autoDelete: false, 
                                    arguments: null);

                var timestamp = DateTime.UtcNow;
                var message = $"{timestamp},{longitude},{latitude},{mapType}";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "", 
                                    routingKey: QueueName, 
                                    basicProperties: null, 
                                    body: body);
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error: {ex.Message}");
            }

        }

        public void ConsumeLogs()
        {
            var factory = new ConnectionFactory() { 
                HostName = _hostname, 
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: QueueName, 
                                durable: false, 
                                exclusive: false, 
                                autoDelete: false, 
                                arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                if (_dbUpdateService.SaveLogtoDb(message))
                {
                    _logUpdateService.LogInput(message);
                }

                if(channel.MessageCount(QueueName) > 1000)
                {
                    Console.WriteLine("Too much load!");
                }
            };

            channel.BasicConsume(queue: QueueName, 
                                autoAck: true, 
                                consumer: consumer);
        }
    }
}
