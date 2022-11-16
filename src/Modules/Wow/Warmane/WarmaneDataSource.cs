using Microsoft.Extensions.Logging;
using Wow.Warmane.Client;

namespace Wow.Warmane;

internal class WarmaneDataSource : IWowDataSource
{
    private readonly ILogger<WarmaneDataSource> _logger;
    private readonly IWarmaneApi                _warmaneClient;

    public WarmaneDataSource(IWarmaneApi warmaneClient, ILogger<WarmaneDataSource> logger)
    {
        _warmaneClient = warmaneClient;
        _logger        = logger;
    }

    public string Server => "Warmane";

    public async Task<Character?> GetCharacterAsync(string realm, string name)
    {
        try
        {
            var response = await _warmaneClient.GetCharacterAsyncResponse(name, realm);

            if (response.ResponseMessage.IsSuccessStatusCode)
            {
                if (response.GetContent() is { } content) return new Character(name, realm, content.Level);
                _logger.LogWarning("No content returned from Warmane API");
                return null;
            }
            else
            {
                var statusCode = response.ResponseMessage.StatusCode;
                var reason     = response.ResponseMessage.ReasonPhrase;
                var content    = response.StringContent;

                _logger.LogError("Error while getting character {Name} from {Realm} on {Server} server. Status code: {StatusCode}, reason: {Reason}, content: {Content}",
                                 name, realm, Server, statusCode, reason, content);
            }

            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting character {Name} from {Realm} on {Server} server", name, realm,
                             Server);
            return null;
        }
    }
}