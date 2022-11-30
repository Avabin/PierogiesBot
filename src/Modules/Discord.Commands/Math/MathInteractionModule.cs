using System.ComponentModel;
using Discord.Interactions;
using GrainInterfaces;
using Microsoft.Extensions.Logging;

namespace Discord.Commands.Math;

public class MathInteractionModule : GrainedInteractionModuleBase
{
    private readonly IMathApi                       _api;
    private readonly ILogger<MathInteractionModule> _logger;

    public MathInteractionModule(IClusterClient clusterClient, IMathApi api, ILogger<MathInteractionModule> logger) :
        base(clusterClient)
    {
        _api    = api;
        _logger = logger;
    }

    [SlashCommand("render", "Renders a math expression in LaTeX")]
    public async Task RenderMath([Description("LaTeX expression")]    string expression,
                                 [Description("DPI (dots per inch)")] int    dpi = 120)
    {
        await DeferAsync();
        await using var stream = await _api.GetRenderAsync(expression, dpi: dpi);
        _logger.LogDebug("Rendered math expression {Expression}", expression);
        _logger.LogInformation("Responding with file attachment");
        await FollowupWithFileAsync(stream, "render.png");
    }
}