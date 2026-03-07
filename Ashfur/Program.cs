using Microsoft.Extensions.Configuration;
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
        HostApplicationBuilderSettings settings = new() {
            Args = args,
            Configuration = new ConfigurationManager(),
            ContentRootPath = Directory.GetCurrentDirectory(),
        };

        settings.Configuration.AddEnvironmentVariables(prefix: "ASHFUR_");
        
        var builder = Host.CreateApplicationBuilder(settings);
        
        String serilogOutputTemplate =
            "[{Timestamp:HH:mm:ss}] [{Level:u3}] [{AssemblyName}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .Enrich.WithAssemblyName()
            .Enrich.WithAssemblyVersion()
            .Enrich.WithClassName()
            .WriteTo.Sentry(options => {
                // A Sentry Data Source Name (DSN) is required.
                // See https://docs.sentry.io/product/sentry-basics/dsn-explainer/
                // You can set it in the SENTRY_DSN environment variable, or you can set it in code here.
                options.Dsn = "https://2af6737f46dc537c11428502f09c9f10@o4510992148267008.ingest.us.sentry.io/4510992207314944";

                // When debug is enabled, the Sentry client will emit detailed debugging information to the console.
                // This might be helpful, or might interfere with the normal operation of your application.
                // We enable it here for demonstration purposes when first trying Sentry.
                // You shouldn't do this in your applications unless you're troubleshooting issues with Sentry.
                options.Debug = true;

                // This option is recommended. It enables Sentry's "Release Health" feature.
                options.AutoSessionTracking = true;

                // Set TracesSampleRate to 1.0 to capture 100%
                // of transactions for tracing.
                // We recommend adjusting this value in production.
                options.TracesSampleRate = 1.0;

                // Sample rate for profiling, applied on top of othe TracesSampleRate,
                // e.g. 0.2 means we want to profile 20 % of the captured transactions.
                // We recommend adjusting this value in production.
                options.ProfilesSampleRate = 1.0;
                // Requires NuGet package: Sentry.Profiling
                // Note: By default, the profiler is initialized asynchronously. This can
                // be tuned by passing a desired initialization timeout to the constructor.
                options.AddIntegration(new ProfilingIntegration(
                    // During startup, wait up to 500ms to profile the app startup code.
                    // This could make launching the app a bit slower so comment it out if you
                    // prefer profiling to start asynchronously
                    TimeSpan.FromMilliseconds(500)
                ));
                // Enable logs to be sent to Sentry
                options.EnableLogs = true;

                // What are we, fuckin Palantir?
                options.SendDefaultPii = false;

                // options.TextFormatter = new MessageTemplateTextFormatter(serilogOutputTemplate);
            })
            .WriteTo.Console(outputTemplate: serilogOutputTemplate)
            .WriteTo.Async(a => {
                a.File(new CompactJsonFormatter(), $"logs/log-{DateTime.Now:yyyyMMddHHmmss}.json",
                    LogEventLevel.Verbose);
                a.File($"logs/log-{DateTime.Now:yyyyMMddHHmmss}.human.log",
                    LogEventLevel.Verbose);
            })
            .CreateLogger();

        builder.Services
            .AddSerilog()
            .AddDiscordGateway(options => {
                options.Intents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent;
                options.Presence = new PresenceProperties(UserStatusType.DoNotDisturb) {
                    Activities = [
                        new UserActivityProperties("gay stuff 🏳️‍🌈", UserActivityType.Competing)
                    ]
                };
                options.Token = builder.Configuration["DISCORD_TOKEN"];
            })
            .AddGatewayHandlers(typeof(Program).Assembly)
            .AddApplicationCommands()
            .AddOsuApiClient(new OsuClientAccessTokenProvider(builder.Configuration["OSU_CLIENT_ID"],
                builder.Configuration["OSU_CLIENT_SECRET"]))
            .AddSingleton<IMongoClient>(_ => new MongoClient(builder.Configuration["MONGODB_CONNECTION_STRING"]));

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