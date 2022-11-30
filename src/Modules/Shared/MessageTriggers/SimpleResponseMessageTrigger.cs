namespace Shared.MessageTriggers;

[Immutable]
[GenerateSerializer]
public record SimpleResponseMessageTrigger
    (string Trigger, string Name, string Response) : ResponseMessageTrigger(Trigger, Name, Response)
{
    // empty constructor for deserialization
    public SimpleResponseMessageTrigger() : this("", "", "")
    {
    }

    public override string ToString()
    {
        return base.ToString();
    }
}