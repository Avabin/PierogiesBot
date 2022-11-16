namespace GrainInterfaces.Discord.Guilds.MessageTriggers;

[Immutable]
[GenerateSerializer]
public abstract record MessageTrigger([property: Id(0)] string Trigger, [property: Id(1)] string Name)
{
    public abstract bool IsMatch(string content);
}

[Immutable]
[GenerateSerializer]
public abstract record ReactionMessageTrigger
    (string Trigger, string Name, [property: Id(2)] string Emoji) : MessageTrigger(Trigger, Name)
{
    public override bool IsMatch(string content)
    {
        return content.Contains(Trigger, StringComparison.InvariantCultureIgnoreCase) ||
               content.Equals(Trigger, StringComparison.InvariantCultureIgnoreCase);
    }
}