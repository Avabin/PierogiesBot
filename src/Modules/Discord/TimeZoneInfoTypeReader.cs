using Discord.Interactions;
using TimeZoneConverter;

namespace Discord;

public class TimeZoneInfoTypeReader : TypeConverter<TimeZoneInfo>
{
    public override Task<TypeConverterResult> ReadAsync(IInteractionContext                      context,
                                                        IApplicationCommandInteractionDataOption option,
                                                        IServiceProvider                         services)
    {
        try
        {
            if (option.Type != ApplicationCommandOptionType.String)
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.BadArgs,
                                                                     "Option must be a string."));
            var tzInfo = TZConvert.GetTimeZoneInfo(option.Value as string ?? "UTC");
            return Task.FromResult(TypeConverterResult.FromSuccess(tzInfo));
        }
        catch (Exception e)
        {
            return Task.FromResult(TypeConverterResult.FromError(e));
        }
    }

    public override ApplicationCommandOptionType GetDiscordType()
    {
        return ApplicationCommandOptionType.String;
    }
}