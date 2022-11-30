namespace Shared.MessageTriggers;

[Immutable]
[GenerateSerializer]
public record SimpleReactionMessageTrigger
    (string Trigger, string Emoji, string Name) : ReactionMessageTrigger(Trigger, Name, Emoji)
{
    // empty constructor for deserialization
    public SimpleReactionMessageTrigger() : this("", "", "")
    {
    }

    public override string ToString()
    {
        return base.ToString();
    }
}