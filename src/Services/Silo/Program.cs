// See https://aka.ms/new-console-template for more information

using System.Globalization;
using Discord;
using GrainInterfaces;
using Infrastructure;
using RestEase.HttpClientFactory;
using Wow;

CultureInfo.CurrentCulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en");
var builder = WebApplication.CreateBuilder(args);
// .UseOrleans(11111, 30000, "PierogiesBot", "Silo")
builder.UseOrleans(11111, 30000, "PierogiesBot", "Silo");
builder.Services.AddWowServices(builder.Configuration.GetRequiredSection("Blizzard"));
builder.Services.AddDiscordCommands(builder.Configuration.GetRequiredSection("Discord"));
builder.Services.AddRestEaseClient<IMathApi>("http://localhost:5000");
await builder
    .Build()
    .RunAsync();