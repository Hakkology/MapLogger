using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MapLogger
{
    public class RabbitMqService
    {
        private readonly string _hostname;
        private IConnection _connection;
        private IModel _channel;
        private const string QueueName = "logQueue";
        private readonly DbUpdateService _dbUpdateService;
        private readonly LogUpdateService _logUpdateService;

        public RabbitMqService(DbUpdateService dbUpdateService, LogUpdateService logUpdateService, string hostname = "localhost")
        {
            _hostname = hostname;
            _dbUpdateService = dbUpdateService;
            _logUpdateService = logUpdateService;
            InitializeRabbitMq();
        }

        private void InitializeRabbitMq()
        {
            var factory = new ConnectionFactory() { 
                HostName = _hostname, 
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void SendLogToRabbitMQ(string mapType, double longitude, double latitude)
        {
            try
            {
                var timestamp = DateTime.UtcNow;
                var message = $"{timestamp},{longitude},{latitude},{mapType}";
                var body = Encoding.UTF8.GetBytes(message);

                _channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: null, body: body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending log to RabbitMQ: {ex.Message}");
            }
        }

        public void ConsumeLogs()
        {
            var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    try
                    {
                        if (_dbUpdateService.SaveLogtoDb(message))
                        {
                            _logUpdateService.LogInput(message);
                            _channel.BasicAck(ea.DeliveryTag, multiple: false);
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Failed to process message: {ex.Message}");
                    }
                };

            _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}



