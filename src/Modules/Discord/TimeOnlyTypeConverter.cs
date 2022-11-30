using Discord.Interactions;

namespace Discord;

public class TimeOnlyTypeConverter : TypeConverter<TimeOnly>
{
    public override ApplicationCommandOptionType GetDiscordType()
    {
        return ApplicationCommandOptionType.String;
    }

    public override Task<TypeConverterResult> ReadAsync(IInteractionContext                      context,
                                                        IApplicationCommandInteractionDataOption option,
                                                        IServiceProvider                         services)
    {
        return Task.FromResult(TimeOnly.TryParse(option.Value.ToString(), out var time)
                                   ? TypeConverterResult.FromSuccess(time)
                                   : TypeConverterResult.FromError(InteractionCommandError.ParseFailed,
                                                                   "Failed to parse time."));
    }
}