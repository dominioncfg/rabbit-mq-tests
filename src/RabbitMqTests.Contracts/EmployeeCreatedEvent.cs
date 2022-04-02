using System.Text.Json;

namespace RabbitMqTests.Contracts;

public class EmployeeCreatedEvent
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
        public DateTime CreatedUtc { get; set; }

    public override string ToString() => JsonSerializer.Serialize(this);
}
