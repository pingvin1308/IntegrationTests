using System.Net.Http.Json;
using IntegrationTests.DataAccess;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace IntegrationTests.Tests;

public class UsersControllerTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;

    public UsersControllerTests()
    {
        Environment.SetEnvironmentVariable("DOCKER_HOST", "http://localhost:3376");
        _postgreSqlContainer= new PostgreSqlBuilder().Build();
    }

    [Fact]
    public async Task Get_ShouldReturnUsers()
    {
        var connectionString = _postgreSqlContainer.GetConnectionString();
        var appFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                });
            });
        var client = appFactory.CreateClient();

        var users = await client.GetFromJsonAsync<User[]>("users");

        Assert.NotNull(users);
        Assert.Empty(users);
    }

    public Task InitializeAsync()
    {
        return _postgreSqlContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _postgreSqlContainer.DisposeAsync().AsTask();
    }
}