using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqTests.Contracts;
using System.Text;
using System.Text.Json;

Console.Title = "Consumer";
Console.WriteLine("*****Welcome to the Consumer*****");
const string HostName = "localhost";
const string QueueName = "OneToOne";
var factory = new ConnectionFactory() { HostName = HostName };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

channel.QueueDeclare(queue: QueueName,
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var @event = Deserialize<ResizeEmployeeImageCommand>(message);
    EmployeeCreatedEventReceived(@event);
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

    var delay = (new Random()).Next(10, 2000);
    await Task.Delay(delay);
};
channel.BasicConsume(queue: QueueName,
                     autoAck: false,
                     consumer: consumer);
Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();

void EmployeeCreatedEventReceived(ResizeEmployeeImageCommand @event)
{
    Console.WriteLine(" [x] Received {0}", @event);
}

T Deserialize<T>(string s) => JsonSerializer.Deserialize<T>(s) ?? throw new Exception("Fail to parse");