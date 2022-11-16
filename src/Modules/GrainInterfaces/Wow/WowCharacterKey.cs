using System.Text.RegularExpressions;

namespace GrainInterfaces.Wow;

[Immutable]
[GenerateSerializer]
public readonly record struct WowCharacterKey([property: Id(0)] string Server, [property: Id(1)] string Realm,
                                              [property: Id(2)] string Name)
{
    private const string KeyPattern = @"\[(?<server>.*)\]-\[(?<realm>.*)\]-\[(?<name>.*)\]";

    private static readonly Regex KeyRegex =
        new(KeyPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);

    public static WowCharacterKey Parse(string key)
    {
        var match = KeyRegex.Match(key);
        if (!match.Success)
            throw new ArgumentException($"Key '{key}' does not match pattern '{KeyPattern}'");

        return new WowCharacterKey(
                                   match.Groups["server"].Value,
                                   match.Groups["realm"].Value,
                                   match.Groups["name"].Value);
    }

    public override string ToString()
    {
        return $"[{Server}]-[{Realm}]-[{Name}]";
    }
}