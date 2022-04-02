using RabbitMQ.Client;
using RabbitMqTests.Contracts;
using System.Text;
using System.Text.Json;

async Task SendMessages()
{
    const string HostName = "localhost";
    const string ExchangeName = "PublishSubscribeExchange";

    var factory = new ConnectionFactory() { HostName = HostName };
    using var connection = factory.CreateConnection();
    using var channel = connection.CreateModel();
    channel.ExchangeDeclare(exchange: ExchangeName,
                            type: ExchangeType.Fanout,
                            durable: true);

    while (true)
    {
        var indx = (new Random()).Next();
        var message = new EmployeeCreatedEvent()
        {
            Id = Guid.NewGuid(),
            FullName = $"Perico Perez {indx}",
            CreatedUtc = DateTime.UtcNow,
        };
        var body = Serialize(message);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        channel.BasicPublish(exchange: ExchangeName,
                             routingKey: string.Empty,
                             body: body,
                             basicProperties: properties);
        Console.WriteLine(" [x] Sent {0}", message);

        var delay = (new Random()).Next(10, 100);
        await Task.Delay(delay);
    }
}

byte[] Serialize(object o)
{
    var body = JsonSerializer.Serialize(o);
    return Encoding.UTF8.GetBytes(body);
}

Console.Title = "Publisher";
Console.WriteLine("*****Welcome to the Publisher*****");
Task.Run(() => SendMessages());
Console.WriteLine("Press [enter] to exit.");
Console.ReadLine();