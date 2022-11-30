namespace Core;

public static class StreamNamespaces
{
    public const string DiscordUser       = @"discord\.user\..+";
    public const string DiscordGuild      = @"discord\.guild\..+";
    public const string MessagesWatcher   = $@"{DiscordGuild}\.messages_watcher";
    public const string Triggers          = $@"{DiscordGuild}\.rules";
    public const string ScheduledMessages = $@"{DiscordGuild}\.scheduled_messages";
    public const string All               = ".*";

    public static string ForDiscordUser(ulong id)
    {
        return $"discord.user.{id}";
    }

    public static string ForDiscordGuild(ulong id)
    {
        return $"discord.guild.{id}";
    }

    public static string ForMessagesWatcher(ulong id)
    {
        return $"{ForDiscordGuild(id)}.messages_watcher";
    }

    public static string ForTriggers(ulong id)
    {
        return $"{ForDiscordGuild(id)}.rules";
    }

    public static string ForScheduledMessages(ulong guildId)
    {
        return $"{ForDiscordGuild(guildId)}.scheduled_messages";
    }
}