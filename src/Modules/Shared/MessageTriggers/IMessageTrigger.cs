namespace Shared.MessageTriggers;

public interface IMessageTrigger
{
    string Trigger { get; init; }
    string Name { get; init; }
    string Response { get; init; }
    bool IsMatch(string content);
}