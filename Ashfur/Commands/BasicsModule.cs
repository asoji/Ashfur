using System.Reflection;
using System.Runtime.InteropServices;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;

namespace Ashfur.Commands;

public class BasicsModule : ApplicationCommandModule<ApplicationCommandContext> {
    [SlashCommand("about", "About Ashfur")]
    public async Task About() {
        Assembly netcordAssembly = typeof(NetCord.Application).Assembly;
        var netcordVersionAttribute = netcordAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        string netcordVersion = netcordVersionAttribute?.InformationalVersion ?? "i ate the version";

        EmbedFooterProperties aboutFooter = new EmbedFooterProperties()
            .WithText("oh for fuck sakes");

        EmbedProperties aboutEmbed = new EmbedProperties()
            .WithTitle("Ashfur")
            .WithDescription("a really dumb discord bot for the dinner table.")
            .AddFields(
                new EmbedFieldProperties()
                    .WithName("NetCord Version")
                    .WithValue(netcordVersion),
                new EmbedFieldProperties()
                    .WithName(".NET Version")
                    .WithValue($"{RuntimeInformation.FrameworkDescription} on {RuntimeInformation.OSDescription}")
            ).WithFooter(aboutFooter);

        ReplyMessageProperties aboutEmbedMessage = new ReplyMessageProperties()
            .WithEmbeds([aboutEmbed]);

        await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties() {
            Embeds = aboutEmbedMessage.Embeds
        }));
    }
}