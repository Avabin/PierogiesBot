using System.Text.RegularExpressions;

namespace GrainInterfaces.Wow;

[Immutable]
[GenerateSerializer]
public readonly record struct WowCharacterKey([property: Id(0)] string Server, [property: Id(1)] string Realm,
                                              [property: Id(2)] string Name) : IParsable<WowCharacterKey>
{
    private const string KeyPattern = @"\[(?<server>.*)\]-\[(?<realm>.*)\]-\[(?<name>.*)\]";

    private static readonly Regex KeyRegex =
        new(KeyPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);

    public static WowCharacterKey Parse(string s, IFormatProvider? provider = null)
    {
        var match = KeyRegex.Match(s);
        if (!match.Success)
            throw new ArgumentException($"Key '{s}' does not match pattern '{KeyPattern}'");

        return new WowCharacterKey(
                                   match.Groups["server"].Value,
                                   match.Groups["realm"].Value,
                                   match.Groups["name"].Value);
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out WowCharacterKey result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        var match = KeyRegex.Match(s);

        if (!match.Success)
        {
            result = default;
            return false;
        }

        result = new WowCharacterKey(
                                     match.Groups["server"].Value,
                                     match.Groups["realm"].Value,
                                     match.Groups["name"].Value);

        return true;
    }

    public override string ToString()
    {
        return $"[{Server}]-[{Realm}]-[{Name}]";
    }

    public static implicit operator string(WowCharacterKey key)
    {
        return key.ToString();
    }
}