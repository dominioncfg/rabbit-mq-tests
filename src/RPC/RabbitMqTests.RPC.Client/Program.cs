using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqTests.Contracts;

Console.Title = "RPC Client";
Console.WriteLine("*****Welcome to the RPC Client*****");
const string HostName = "localhost";
const string RequestQueueName = "RPCRequestQueue";
const string ReplyQueueName = "RPCReplyQueue";

var respQueue = new BlockingCollection<CreateEmployeeCommandResponse>();
var factory = new ConnectionFactory() { HostName = HostName };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
var replyQueueName = channel.QueueDeclare(queue: ReplyQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
var consumer = new EventingBasicConsumer(channel);

var props = channel.CreateBasicProperties();
props.Persistent = true;
var correlationId = Guid.NewGuid().ToString();
props.CorrelationId = correlationId;
props.ReplyTo = ReplyQueueName;

consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var response = Encoding.UTF8.GetString(body);
    if (ea.BasicProperties.CorrelationId == correlationId)
    {
        respQueue.Add(Deserialize<CreateEmployeeCommandResponse>(response));
    }
};

channel.BasicConsume(
    consumer: consumer,
    queue: replyQueueName,
    autoAck: true);



CreateEmployeeCommandResponse Call()
{
    var request = new CreateEmployeeCommand()
    {
        Id = Guid.NewGuid(),
        FullName = "Perico Perez",
    };
    channel.BasicPublish(
        exchange: "",
        routingKey: RequestQueueName,
        basicProperties: props,
        body: Serialize(request));

    return respQueue.Take();
}


byte[] Serialize(object o)
{
    var body = JsonSerializer.Serialize(o);
    return Encoding.UTF8.GetBytes(body);
}

T Deserialize<T>(string s) => JsonSerializer.Deserialize<T>(s) ?? throw new Exception("Fail to parse");




Console.WriteLine(" [x] Requesting Employee Creation");
var response = Call();

Console.WriteLine("Received:");
Console.WriteLine(response);

Console.WriteLine("Press [enter] to exit.");
Console.ReadLine();