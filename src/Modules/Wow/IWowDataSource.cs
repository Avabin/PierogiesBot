namespace Wow;

internal interface IWowDataSource
{
    /// <summary>
    /// Supported World of Warcraft server
    /// </summary>
    string Server { get; }

    /// <summary>
    /// Fetches the character data from the server
    /// </summary>
    /// <param name="realm">The realm to fetch the character from</param>
    /// <param name="name">The name of the character to fetch</param>
    /// <returns>The character data</returns>
    Task<Character?> GetCharacterAsync(string realm, string name);
}