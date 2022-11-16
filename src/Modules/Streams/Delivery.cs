using RabbitMQ.Client.Events;

namespace Streams;

[GenerateSerializer]
[Immutable]
public record Delivery([property: Id(0)] string               CorrelationId,
                       [property: Id(1)] ReadOnlyMemory<byte> Body,
                       [property: Id(2)] ulong                DeliveryTag,
                       [property: Id(3)] bool                 Redelivered,
                       [property: Id(4)] string               Exchange,
                       [property: Id(5)] string               RoutingKey,
                       [property: Id(6)] string               ConsumerTag,
                       [property: Id(7)] string               Type)
{
    public static Delivery From(BasicDeliverEventArgs args)
    {
        return new Delivery(args.BasicProperties.CorrelationId, args.Body, args.DeliveryTag, args.Redelivered,
                            args.Exchange,
                            args.RoutingKey, args.ConsumerTag, args.BasicProperties.Type);
    }
}