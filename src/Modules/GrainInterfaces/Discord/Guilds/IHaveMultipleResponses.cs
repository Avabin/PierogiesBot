namespace GrainInterfaces.Discord.Guilds;

public interface IHaveMultipleResponses
{
    IReadOnlyList<string> Responses      { get; }
    RespondingType        RespondingType { get; }
}

public enum RespondingType
{
    Random,
    Sequential,
    RandomNoRepeat
}