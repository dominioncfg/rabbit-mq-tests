using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqTests.Contracts;
using System.Text;
using System.Text.Json;

const string HostName = "localhost";
const string QueueName = "RPCRequestQueue";

Console.Title = "RPC Server";
Console.WriteLine("*****Welcome to the RPC Server*****");
var factory = new ConnectionFactory() { HostName = HostName };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
channel.BasicQos(0, 1, false);

var consumer = new EventingBasicConsumer(channel);
channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

Console.WriteLine(" [x] Awaiting RPC requests");
consumer.Received += (model, ea) =>
{
    byte[] response = Array.Empty<byte>();

    var body = ea.Body.ToArray();
    var props = ea.BasicProperties;

    var replyProps = channel.CreateBasicProperties();
    replyProps.Persistent = true;
    replyProps.CorrelationId = props.CorrelationId;

    try
    {
        var message = Encoding.UTF8.GetString(body);
        var @event = Deserialize<CreateEmployeeCommand>(message);
        var commandResponse = CreateEmployee(@event);
        response = Serialize(commandResponse);

    }
    catch (Exception e)
    {
        Console.WriteLine("Fail to proccess" + e.Message);
        response = Serialize(new { Message = "Fail to process" });
    }
    finally
    {
        channel.BasicPublish(exchange: string.Empty,
                             routingKey: props.ReplyTo,
                             basicProperties: replyProps,
                             body: response);

        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
    }
};

Console.WriteLine("Press [enter] to exit.");
Console.ReadLine();



CreateEmployeeCommandResponse CreateEmployee(CreateEmployeeCommand @event)
{
    Console.WriteLine("[x] Received {0}", @event);
    //Create ...

    return new CreateEmployeeCommandResponse()
    {
        Id = @event.Id,
        CreatedUtc = DateTime.UtcNow,
    };
}

T Deserialize<T>(string s) => JsonSerializer.Deserialize<T>(s) ?? throw new Exception("Fail to parse");

byte[] Serialize(object o)
{
    var body = JsonSerializer.Serialize(o);
    return Encoding.UTF8.GetBytes(body);
}