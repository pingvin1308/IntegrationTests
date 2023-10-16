using System.Reflection;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using EasyNetQ.Consumer;
using IntegrationTests.DataAccess;
using IntegrationTests.Events;

namespace IntegrationTests.Consumer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IBus _bus;

    public Worker(ILogger<Worker> logger, IBus bus)
    {
        _logger = logger;
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriber = new AutoSubscriber(_bus, "IntegrationTests.Consumer");
        await subscriber.SubscribeAsync(Assembly.GetExecutingAssembly().GetTypes(), stoppingToken);
        // while (!stoppingToken.IsCancellationRequested)
        // {
        //     _bus.SendReceive.Receive<UserCreatedEvent>(UserCreatedEvent.QueueName, 
        //         message => _logger.LogInformation(message.ToString()));
        //     _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //     await Task.Delay(1000, stoppingToken);
        // }
    }
}

public class UserCreatedEventConsumer : IConsume<UserCreatedEvent>
{
    public void Consume(UserCreatedEvent message, CancellationToken cancellationToken = new CancellationToken())
    {
        
    }
}