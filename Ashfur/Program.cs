using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
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
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(new CompactJsonFormatter(), "logs/log.json", LogEventLevel.Verbose, rollingInterval: RollingInterval.Day)
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
                builder.Configuration["Osu:ClientSecret"]));

        var host = builder.Build();

        host.AddModules(typeof(Program).Assembly);

        await host.RunAsync();
    }
}