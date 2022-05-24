using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Core;

[DataContract]
[KnownType(typeof(Notification))]
[KnownType(typeof(Command))]
public abstract record Event() : IEvent {}