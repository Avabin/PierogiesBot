namespace GrainInterfaces.Discord.Guilds.MessageTriggers;

[Immutable]
[GenerateSerializer]
public record SimpleReactionMessageTrigger
    (string Trigger, string Emoji, string Name) : ReactionMessageTrigger(Trigger, Emoji, Name);