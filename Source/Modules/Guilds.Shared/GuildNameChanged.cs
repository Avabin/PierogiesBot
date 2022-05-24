using System.Runtime.Serialization;
using Core;

namespace Guilds.Shared;

[DataContract]
public record GuildNameChanged([property: DataMember] ulong GuildId, [property: DataMember] string Name) : GuildNotification(GuildId);