using System.Reflection;
using System.Runtime.InteropServices;
using Ashfur.Utils;
using JetBrains.Annotations;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace Ashfur.Modules;

[UsedImplicitly]
public class BasicsModule : ApplicationCommandModule<ApplicationCommandContext> {
    [SlashCommand("about", "About Ashfur")]
    public async Task About() {
        Assembly netcordAssembly = typeof(Application).Assembly;
        var netcordVersionAttribute = netcordAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        string netcordVersion = netcordVersionAttribute?.InformationalVersion ?? "i ate the version";

        ComponentContainerProperties component = new ComponentContainerProperties()
            .WithAccentColor(new(0x00A86B))
            .AddComponents([
                new TextDisplayProperties("# Ashfur About"),
                new ComponentSeparatorProperties().WithDivider(),
                new TextDisplayProperties("a really dumb discord bot for the weird dinner table. in development forever. :p"),
                new ComponentSeparatorProperties().WithDivider(),
                new TextDisplayProperties("## NetCord Version"),
                new TextDisplayProperties(netcordVersion),
                new ActionRowProperties().AddComponents(new LinkButtonProperties("https://github.com/NetCordDev/NetCord", "NetCord GitHub")),
                new ComponentSeparatorProperties().WithDivider(),
                new TextDisplayProperties("## .NET Version"),
                new TextDisplayProperties($"{RuntimeInformation.FrameworkDescription} on {RuntimeInformation.OSDescription}"),
                new ActionRowProperties().AddComponents(new LinkButtonProperties("https://dotnet.microsoft.com/", ".NET Site")),
                new ComponentSeparatorProperties().WithDivider(),
                new TextDisplayProperties("-# oh for fuck sakes.")
            ]);

        await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties() {
            Components = [ component ]
        }.WithFlags(MessageFlags.IsComponentsV2)));
    }

    [SlashCommand("help", "no")]
    public async Task Help() {
        ComponentContainerProperties component = new ComponentContainerProperties()
            .WithAccentColor(new(0x00A86B))
            .AddComponents(new TextDisplayProperties("# Help"))
            .AddComponents(new ComponentSeparatorProperties().WithDivider())
            .AddComponents(new TextDisplayProperties("no."));

        await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties() {
            Components = [ component ]
        }.WithFlags(MessageFlags.IsComponentsV2)));
    }
}