using Microsoft.Extensions.Hosting;
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

        var host = builder.Build();

        host.AddModules(typeof(Program).Assembly);

        await host.RunAsync();
    }
}