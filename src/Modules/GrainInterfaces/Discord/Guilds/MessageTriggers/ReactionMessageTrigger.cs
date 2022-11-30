namespace GrainInterfaces.Discord.Guilds.MessageTriggers;

[Immutable]
[GenerateSerializer]
public abstract record MessageTrigger([property: Id(0)] string Trigger, [property: Id(1)] string Name,
                                      [property: Id(2)] string Response)
{
    public abstract bool IsMatch(string content);

    public override string ToString()
    {
        return $"{Name}: {Trigger} -> {Response}";
    }
}

[Immutable]
[GenerateSerializer]
public abstract record ReactionMessageTrigger
    (string Trigger, string Name, [property: Id(3)] string Emoji) : MessageTrigger(Trigger, Name, Emoji)
{
    // empty constructor for deserialization
    protected ReactionMessageTrigger() : this("", "", "")
    {
    }

    public override bool IsMatch(string content)
    {
        return content.Contains(Trigger, StringComparison.InvariantCultureIgnoreCase) ||
               content.Equals(Trigger, StringComparison.InvariantCultureIgnoreCase);
    }


    public override string ToString()
    {
        return base.ToString();
    }
}