// See https://aka.ms/new-console-template for more information

using Infrastructure;
using Microsoft.Extensions.Logging;

var builder = ConsoleApp.CreateBuilder(args);
builder.ConfigureLogging(x => x.ClearProviders());
builder.ConfigureServices((context, collection) =>
                              collection.AddClusterClient("PierogiesBot", "ConsoleClient", context.HostingEnvironment,
                                                          context.Configuration));
var app = builder.Build();

await app.AddAllCommandType()
         .RunAsync();