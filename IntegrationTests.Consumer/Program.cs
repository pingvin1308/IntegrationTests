using EasyNetQ;
using IntegrationTests.Consumer;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.RegisterEasyNetQ("amqp://guest:guest@localhost:5673/", register => { register.EnableSystemTextJson(); });
    })
    .Build();
host.Run();