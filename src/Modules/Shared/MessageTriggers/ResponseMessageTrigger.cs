namespace Shared.MessageTriggers;

[Immutable]
[GenerateSerializer]
public abstract record ResponseMessageTrigger
    (string Trigger, string Name, string Response) : MessageTrigger(Trigger, Name, Response)
{
    // empty constructor for deserialization
    protected ResponseMessageTrigger() : this(string.Empty, string.Empty, string.Empty)
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