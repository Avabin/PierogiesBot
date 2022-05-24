using System.Runtime.Serialization;
using Core;

namespace Guilds.Shared;

[DataContract]
[KnownType(typeof(ChangeGuildNameCommand))]
public record GuildCommand([property: DataMember]ulong GuildId) : Command
{
    
}