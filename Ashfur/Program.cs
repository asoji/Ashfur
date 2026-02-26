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

namespace Ashfur;

internal class Program {
    static async Task Main(string[] args) {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services
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

        builder.Logging.AddSimpleConsole(options => {
            options.SingleLine = false;
            options.TimestampFormat = "[HH:mm:ss] ";
            options.ColorBehavior = LoggerColorBehavior.Enabled;
            options.UseUtcTimestamp = false;
        });

        var host = builder.Build();

        host.AddModules(typeof(Program).Assembly);

        await host.RunAsync();
    }
}