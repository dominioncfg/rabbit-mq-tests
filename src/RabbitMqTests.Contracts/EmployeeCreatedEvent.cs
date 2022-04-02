using System.Text.Json;

namespace RabbitMqTests.Contracts;

public static class EmployeeTypes
{
    public const string Manager = nameof(Manager);
    public const string Minion = nameof(Minion);
}
public class EmployeeCreatedEvent
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Type { get; set; } = EmployeeTypes.Minion;
    public DateTime CreatedUtc { get; set; }
    public override string ToString() => JsonSerializer.Serialize(this);
}
