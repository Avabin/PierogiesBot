// See https://aka.ms/new-console-template for more information

using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Wow;

var builder = Host.CreateApplicationBuilder(args)
                  .UseOrleans(11111, 30000, "PierogiesBot", "Silo")
                  .AddSeq();

builder.Services.AddWowServices(options =>
{
    options.ClientId = builder.Configuration.GetValue<string>("Blizzard:ClientId") ??
                       throw new InvalidOperationException("Blizzard:ClientId is not set");
    options.ClientSecret = builder.Configuration.GetValue<string>("Blizzard:ClientSecret") ??
                           throw new InvalidOperationException("Blizzard:ClientSecret is not set");
});
await builder
     .Build()
     .RunAsync();