using System.Text;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ConsoleClient.Commands;

public class Watch : ConsoleAppBase
{
    private readonly IHostEnvironment _environment;

    public Watch(IClusterClient client, IHostEnvironment environment)
    {
        _environment = environment;
    }

    [Command("all", "watch incoming events")]
    public async Task ForAllEvents()
    {
        if (!_environment.IsEnvironment("Mongo")) return;

        var connectionFactory = new ConnectionFactory();

        var connection = connectionFactory.CreateConnection();

        var model = connection.CreateModel();

        var queue = model.QueueDeclare("consoleclient");

        model.QueueBind(queue, "orleans", "#");

        var consumer = new EventingBasicConsumer(model);

        consumer.Received += (sender, args) =>
        {
            var body    = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            // construct log message from delivery args
            var timestamp      = args.BasicProperties.Timestamp.UnixTime;
            var dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
            var logMessage =
                $"{dateTimeOffset:O} <> [{args.Exchange}] -> {args.BasicProperties.CorrelationId} => {message}";

            Console.WriteLine(logMessage);
        };

        model.BasicConsume(queue, true, consumer);

        Console.WriteLine("Press any key to exit");
        await Task.Run(Console.ReadKey);
    }
}