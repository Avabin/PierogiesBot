using System.Runtime.Serialization;
using Core;

namespace Guilds.Shared;

[DataContract]
[KnownType(typeof(GuildNameChanged))]
public record GuildNotification([property: DataMember]ulong GuildId) : Notification
{
    
}