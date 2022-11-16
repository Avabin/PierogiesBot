using System.Collections.Immutable;
using System.Text;
using Newtonsoft.Json;
using Orleans.Providers.Streams.Common;
using Orleans.Runtime;
using Orleans.Streams;

namespace Streams;

[GenerateSerializer]
[Immutable]
public class RabbitMQBatchContainer : IBatchContainer
{
    private static readonly JsonSerializerSettings _serializerSettings = new()
    {
        TypeNameHandling               = TypeNameHandling.Objects,
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
    };

    [Id(0)] public readonly ImmutableList<Delivery> Batch;

    public RabbitMQBatchContainer(StreamId              streamId, StreamSequenceToken sequenceToken,
                                  IEnumerable<Delivery> batch)
    {
        Batch         = batch.ToImmutableList();
        StreamId      = streamId;
        SequenceToken = sequenceToken;
    }

    public IEnumerable<Tuple<T, StreamSequenceToken>> GetEvents<T>()
    {
        return Batch.Skip(SequenceToken.EventIndex).Select((x, i) =>
        {
            var body   = Encoding.UTF8.GetString(x.Body.ToArray());
            var @event = JsonConvert.DeserializeObject(body, _serializerSettings);
            return new Tuple<T, StreamSequenceToken>((T)@event!, new EventSequenceToken(i));
        });
    }

    public bool ImportRequestContext()
    {
        return true;
    }

    [Id(1)] public StreamId            StreamId      { get; }
    [Id(2)] public StreamSequenceToken SequenceToken { get; }

    public long GetLatestDeliveryTag()
    {
        return (long)Batch.Last().DeliveryTag;
    }
}