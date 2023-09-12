using MapLogger;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace MapLogger{

    public class RabbitMqConsumerService : IHostedService, IDisposable
    {
        private readonly IServiceScope _scope;
        private readonly RabbitMqService _rabbitMqService;

        public RabbitMqConsumerService(IServiceProvider serviceProvider)
        {
            _scope = serviceProvider.CreateScope();
            _rabbitMqService = _scope.ServiceProvider.GetRequiredService<RabbitMqService>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _rabbitMqService.ConsumeLogs();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }


}


