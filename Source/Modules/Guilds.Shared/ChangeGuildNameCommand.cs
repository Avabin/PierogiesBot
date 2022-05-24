using System.Runtime.Serialization;
using Core;

namespace Guilds.Shared;

[DataContract]
public record ChangeGuildNameCommand([property: DataMember] string Name, [property: DataMember] ulong GuildId) : GuildCommand(GuildId);