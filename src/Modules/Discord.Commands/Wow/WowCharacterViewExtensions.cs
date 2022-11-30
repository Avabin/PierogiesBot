using GrainInterfaces.Wow;

namespace Discord.Commands.Wow;

public static class WowCharacterViewExtensions
{
    public static EmbedBuilder ToEmbed(this WowCharacterView view)
    {
        var builder = new EmbedBuilder();
        builder.WithColor(Color.Purple);
        builder.WithTitle(view.Name);
        builder.WithFields(Field("Server", view.Server), Field("Realm", view.Realm), Field("Level", view.Level));

        return builder;
    }

    private static EmbedFieldBuilder Field(string title, object value, bool inline = true)
    {
        return new EmbedFieldBuilder().WithName(title).WithValue(value).WithIsInline(inline);
    }
}