// See https://aka.ms/new-console-template for more information

using Infrastructure;

var builder = ConsoleApp.CreateBuilder(args);
builder.ConfigureServices((context, collection) =>
                              collection.AddClusterClient("PierogiesBot", "ConsoleClient", context.HostingEnvironment,
                                                          context.Configuration));
await builder.Build().AddAllCommandType().RunAsync();