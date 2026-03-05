using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using osu.NET;
using osu.NET.Authorization;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Ashfur;

internal class Program {
    static async Task Main(string[] args) {
        String serilogOutputTemplate =
            "[{Timestamp:HH:mm:ss}] [{Level:u3}] [{AssemblyName}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .Enrich.WithAssemblyName()
            .Enrich.WithAssemblyVersion()
            .Enrich.WithClassName()
            .WriteTo.Console(outputTemplate: serilogOutputTemplate)
            .WriteTo.Async(a => {
                a.File(new CompactJsonFormatter(), $"logs/log-{DateTime.Now:yyyyMMddHHmmss}.json",
                    LogEventLevel.Verbose);
                a.File($"logs/log-{DateTime.Now:yyyyMMddHHmmss}.human.log",
                    LogEventLevel.Verbose);
            })
            .CreateLogger();

        var builder = Host.CreateApplicationBuilder(args);

        builder.Services
            .AddSerilog()
            .AddDiscordGateway(options => {
                options.Intents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent;
                options.Presence = new PresenceProperties(UserStatusType.DoNotDisturb) {
                    Activities = [
                        new UserActivityProperties("gay stuff 🏳️‍🌈", UserActivityType.Competing)
                    ]
                };
            })
            .AddGatewayHandlers(typeof(Program).Assembly)
            .AddApplicationCommands()
            .AddOsuApiClient(new OsuClientAccessTokenProvider(builder.Configuration["Osu:ClientId"],
                builder.Configuration["Osu:ClientSecret"]))
            .AddSingleton<IMongoClient>(service => new MongoClient(builder.Configuration["MongoDb:ConnectionString"]));

        var host = builder.Build();

        host.AddModules(typeof(Program).Assembly);

        // DEBUG TOOL - LIST COLLECTIONS
        var collections = (await host.Services.GetService<IMongoClient>()
            .GetDatabase("ashfur")
            .ListCollectionNamesAsync()).ToList();
        Log.Logger.Debug($"Found collections: {collections.Count}\n{String.Join("\n", collections)}");

        await host.RunAsync();
    }
}