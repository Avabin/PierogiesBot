namespace GrainInterfaces.Discord.Guilds.MessageTriggers;

[Immutable]
[GenerateSerializer]
public record SimpleResponseMessageTrigger
    (string Trigger, string Name, string Response) : ResponseMessageTrigger(Trigger, Name, Response);