using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core;


public static class EventExtensions
{
    public static readonly JsonSerializerSettings Settings = new()
    {
        TypeNameHandling               = TypeNameHandling.Objects,
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
    };
    public  static readonly JsonSerializer Serializer = JsonSerializer.Create(Settings);

    public static byte[] ToMessageBody(this IEvent @event, JsonSerializerSettings? settings = null)
    {
        // serialize events to byte array
        var json = JsonConvert.SerializeObject(@event, Formatting.None, settings ?? Settings);
        return Encoding.UTF8.GetBytes(json);
    }
    
    public static string ToMessageStringBody(this IEvent @event, JsonSerializerSettings? settings = null)
    {
        var json = JsonConvert.SerializeObject(@event, Formatting.None, settings ?? Settings);
        return json;
    }

    public static T ToEvent<T>(this byte[] body, JsonSerializerSettings? settings = null) where T : IEvent
    {
        // deserialize events from byte array
        var json = Encoding.UTF8.GetString(body);
        return JsonConvert.DeserializeObject<T>(json, settings ?? Settings) ??
               throw new InvalidOperationException($"Unable to deserialize event {typeof(T).Name}: {json}");
    }
    public static T ToEvent<T>(this string body, JsonSerializerSettings? settings = null) where T : IEvent =>
        JsonConvert.DeserializeObject<T>(body, settings ?? Settings) ??
        throw new InvalidOperationException($"Unable to deserialize event {typeof(T).Name}: {body}");

    public static T ToEvent<T>(this JToken body) where T : IEvent
    {
        // deserialize events from byte array
        var @event = body.ToObject<T>(Serializer);
        return @event ?? throw new InvalidOperationException($"Unable to deserialize event {typeof(T).Name}: {body}");
    }
}