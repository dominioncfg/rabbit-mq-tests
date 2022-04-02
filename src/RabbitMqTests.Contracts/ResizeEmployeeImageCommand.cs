using System.Text.Json;

namespace RabbitMqTests.Contracts;

public class ResizeEmployeeImageCommand
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string ImageFilePath { get; set; } = string.Empty;
    public override string ToString() => JsonSerializer.Serialize(this);
}
