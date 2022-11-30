// See https://aka.ms/new-console-template for more information

using Discord;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddMemoryCache().AddDiscordCommands<Program>(builder.Configuration.GetRequiredSection("Discord"));

builder.AddClusterClient("PierogiesBot", "DiscordCommandsClient");

var app = builder.Build();

await app.RunAsync();