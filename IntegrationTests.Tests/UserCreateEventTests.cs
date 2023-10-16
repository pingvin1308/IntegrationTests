using Testcontainers.RabbitMq;

namespace IntegrationTests.Tests;

public class UserCreateEventTests : IAsyncLifetime
{
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder().Build();

    [Fact]
    public async Task A()
    {
    }

    public Task InitializeAsync()
    {
        return _rabbitMqContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _rabbitMqContainer.DisposeAsync().AsTask();
    }
}