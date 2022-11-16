// See https://aka.ms/new-console-template for more information

using Infrastructure;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.AddClusterClient("PierogiesBot", "DiscordCommandsClient");

builder.Build().RunAsync();