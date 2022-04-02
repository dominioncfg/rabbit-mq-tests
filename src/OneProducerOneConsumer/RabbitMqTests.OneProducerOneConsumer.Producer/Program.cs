using RabbitMQ.Client;
using RabbitMqTests.Contracts;
using System.Text;
using System.Text.Json;


async Task SendMessages()
{
    const string HostName = "localhost";
    const string QueueName = "OneToOne";
    var factory = new ConnectionFactory() { HostName = HostName };
    using var connection = factory.CreateConnection();
    using var channel = connection.CreateModel();
    channel.QueueDeclare(queue: QueueName,
                         durable: true,
                         exclusive: false,
                         autoDelete: false,
                         arguments: null);
    
    while (true)
    {
        var indx = (new Random()).Next();
        var message = new ResizeEmployeeImageCommand()
        {
            Id = Guid.NewGuid(),
            FullName = $"Perico Perez {indx}",
            ImageFilePath = $"image{indx}.jpg",
        };
        var body = Serialize(message);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        channel.BasicPublish(exchange: "",
                             routingKey: QueueName,
                             body: body,
                             basicProperties: properties);
        Console.WriteLine(" [x] Sent {0}", message);

        var delay = (new Random()).Next(10, 100);
        await Task.Delay(delay);
    }
}
Byte[] Serialize(object o)
{
    var body = JsonSerializer.Serialize(o);
    return Encoding.UTF8.GetBytes(body);
}



Console.Title = "Producer";
Console.WriteLine("*****Welcome to the Producer*****");
Task.Run(() => SendMessages());
Console.WriteLine("Press [enter] to exit.");
Console.ReadLine();