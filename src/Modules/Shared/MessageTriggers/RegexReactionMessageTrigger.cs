using System.Text.RegularExpressions;

namespace Shared.MessageTriggers;

[Immutable]
[GenerateSerializer]
public record RegexReactionMessageTrigger
    (string Trigger, string Emoji, string Name) : ReactionMessageTrigger(Trigger, Emoji, Name)
{
    // empty constructor for deserialization
    public RegexReactionMessageTrigger() : this("", "", "")
    {
    }

    public override bool IsMatch(string content)
    {
        var regex = new Regex(Trigger);
        return regex.IsMatch(content);
    }


    public override string ToString()
    {
        return base.ToString();
    }
}