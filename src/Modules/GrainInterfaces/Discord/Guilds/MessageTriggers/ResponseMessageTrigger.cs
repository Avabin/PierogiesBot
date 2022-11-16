namespace GrainInterfaces.Discord.Guilds.MessageTriggers;

[Immutable]
[GenerateSerializer]
public abstract record ResponseMessageTrigger
    (string Trigger, string Name, [property: Id(2)] string Response) : MessageTrigger(Trigger, Name)
{
    public override bool IsMatch(string content)
    {
        return content.Contains(Trigger, StringComparison.InvariantCultureIgnoreCase) ||
               content.Equals(Trigger, StringComparison.InvariantCultureIgnoreCase);
    }
}