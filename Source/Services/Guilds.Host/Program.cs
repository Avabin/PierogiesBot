using Core.Actors.Implementations;
using Guilds.DataStore.Actors.Implementations;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddInfrastructure();
builder.Services.AddActors(x =>
{
    x.Actors.RegisterActor<Guild>();
    x.Actors.RegisterActor<GuildEventStore>();
    x.Actors.RegisterActor<EventStore>();
});
var app     = builder.Build();

app.UseInfrastructure();

await app.RunAsync();
