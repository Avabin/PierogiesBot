using System.Text.RegularExpressions;

namespace Shared.MessageTriggers;

[Immutable]
[GenerateSerializer]
public record RegexResponseMessageTrigger
    (string Trigger, string Name, string Response) : ResponseMessageTrigger(Trigger, Name, Response)
{
    // empty constructor for deserialization
    public RegexResponseMessageTrigger() : this("", "", "")
    {
    }

    public override bool IsMatch(string content)
    {
        return Regex.IsMatch(content, Trigger);
    }


    public override string ToString()
    {
        return base.ToString();
    }
}