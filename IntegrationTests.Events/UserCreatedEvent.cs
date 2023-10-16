using EasyNetQ;

namespace IntegrationTests.Events;

[Queue("user-created")]
public record UserCreatedEvent
{
    public const string QueueName = "user-created";

    public int UserId { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }
}