using Core;
using Dapr;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure;

[ApiController]
public class EventsController : ControllerBase
{
    protected IEventsMediator Mediator { get; }

    public EventsController(IEventsMediator mediator)
    {
        Mediator = mediator;
    }

    [Topic("pub-sub", "notifications")]
    [HttpPost("notifications")]
    [Consumes("application/cloudevents+json")]
    public async Task<IActionResult> Publish([FromBody] Delivery body)
    {
        Mediator.Publish(body, body.Topic, body.PubSubName);
        return Ok();
    }
}