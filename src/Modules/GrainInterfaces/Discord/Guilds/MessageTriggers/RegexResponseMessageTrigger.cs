using System.Text.RegularExpressions;

namespace GrainInterfaces.Discord.Guilds.MessageTriggers;

[Immutable]
[GenerateSerializer]
public record RegexResponseMessageTrigger
    (string Trigger, string Name, string Response) : ResponseMessageTrigger(Trigger, Name, Response)
{
    public override bool IsMatch(string content)
    {
        return Regex.IsMatch(content, Trigger);
    }
}