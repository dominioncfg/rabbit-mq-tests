using System.Text.Json;

namespace RabbitMqTests.Contracts;

public class CreateEmployeeCommand
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public override string ToString() => JsonSerializer.Serialize(this);
}

public class CreateEmployeeCommandResponse
{
    public Guid Id { get; set; }
    public DateTime CreatedUtc { get; set; }
    public override string ToString() => JsonSerializer.Serialize(this);
}
