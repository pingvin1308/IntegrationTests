using System.Net.Http.Json;
using IntegrationTests.DataAccess;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace IntegrationTests.Tests;

public class UsersControllerTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;
    private HttpClient _client;

    public UsersControllerTests()
    {
        Environment.SetEnvironmentVariable("DOCKER_HOST", "http://localhost:3376");
        _postgreSqlContainer = new PostgreSqlBuilder().Build();
    }

    [Fact]
    public async Task Get_ShouldReturnUsers()
    {
        var users = await _client.GetFromJsonAsync<User[]>("users");

        Assert.NotNull(users);
        Assert.Empty(users);
    }

    [Fact]
    public async Task Create_ShouldCreateNewUser()
    {
        var user = new User
        {
            Email = "test@mail.com",
            Name = "Test",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var response = await _client.PostAsJsonAsync("users", user);
        response.EnsureSuccessStatusCode();

        var createdUser = await response.Content.ReadFromJsonAsync<User>();

        Assert.NotNull(createdUser);
        Assert.Equal(user.Email, createdUser.Email);
        Assert.Equal(user.Name, createdUser.Name);
        Assert.True(createdUser.Id > 0);
    }
    
    [Fact]
    public async Task Update_ShouldUpdateUser()
    {
        var user = new User
        {
            Email = "Test@mail.com",
            Name = "Test",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var response = await _client.PostAsJsonAsync("users", user);
        response.EnsureSuccessStatusCode();

        var createdUser = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(createdUser);

        createdUser.Name = "Test2";
        createdUser.Email = "Test2@mail.com";
        
        var updatedUserResponse = await _client.PutAsJsonAsync($"users/{createdUser.Id}", createdUser);
        updatedUserResponse.EnsureSuccessStatusCode();
        var updatedUser = await updatedUserResponse.Content.ReadFromJsonAsync<User>();

        Assert.NotNull(updatedUser);
        Assert.Equal(updatedUser.Email, createdUser.Email);
        Assert.Equal(updatedUser.Name, createdUser.Name);
        Assert.True(updatedUser.Id > 0);
    }
    
    [Fact]
    public async Task Delete_ShouldDeleteUser()
    {
        var user = new User
        {
            Email = "Test@mail.com",
            Name = "Test",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var response = await _client.PostAsJsonAsync("users", user);
        response.EnsureSuccessStatusCode();

        var createdUser = await response.Content.ReadFromJsonAsync<User>();
        Assert.NotNull(createdUser);

        var deleteResponse = await _client.DeleteAsync($"users/{createdUser.Id}");
        deleteResponse.EnsureSuccessStatusCode();
        
        var users = await _client.GetFromJsonAsync<User[]>("users");
        Assert.NotNull(users);
        Assert.Empty(users);
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        var connectionString = _postgreSqlContainer.GetConnectionString();
        var appFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, configurationBuilder) =>
                {
                    configurationBuilder.AddInMemoryCollection(new List<KeyValuePair<string, string>>
                    {
                        new("ConnectionStrings:AppDbContext", connectionString)
                    });
                });
            });

        using var scope = appFactory.Services.CreateScope();
        var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await appDbContext.Database.MigrateAsync();

        _client = appFactory.CreateClient();
    }

    public Task DisposeAsync()
    {
        return _postgreSqlContainer.DisposeAsync().AsTask();
    }
}