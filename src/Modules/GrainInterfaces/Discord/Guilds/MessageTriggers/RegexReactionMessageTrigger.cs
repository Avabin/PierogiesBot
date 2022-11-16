using System.Text.RegularExpressions;

namespace GrainInterfaces.Discord.Guilds.MessageTriggers;

[Immutable]
[GenerateSerializer]
public record RegexReactionMessageTrigger
    (string Trigger, string Emoji, string Name) : ReactionMessageTrigger(Trigger, Emoji, Name)
{
    public override bool IsMatch(string content)
    {
        var regex = new Regex(Trigger);
        return regex.IsMatch(content);
    }
}