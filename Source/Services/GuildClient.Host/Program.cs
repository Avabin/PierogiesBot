using System.Reactive.Linq;
using Core;
using Dapr.Actors;
using Dapr.Actors.Client;
using Guilds.DataStore.Actors.Interfaces;
using Guilds.Shared;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddInfrastructure();
builder.Services.AddActors(x => x.DrainRebalancedActors = true);
var app     = builder.Build();
app.UseInfrastructure();
await app.StartAsync();

var clusterClient = app.Services.GetRequiredService<IClusterClient>();
var guildId       = 123123123ul;
var guild         = ActorProxy.Create<IGuild>(new ActorId(guildId.ToString("D")), "Guild");

var tcs        = new TaskCompletionSource<GuildNameChanged>();
var observable = clusterClient.GetObservable<GuildNameChanged>(Namespaces.Notifications);
observable.Do(x => tcs.SetResult(x)).Timeout(TimeSpan.FromSeconds(30)).Subscribe();
await guild.ChangeNameAsync("NewName");
var result = await tcs.Task;

Console.WriteLine(result.GuildId);
Console.WriteLine(result.Name);

await app.StopAsync();