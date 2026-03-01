using JetBrains.Annotations;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace Ashfur.Modules.Fun;

[UsedImplicitly]
public class HttpCat : ApplicationCommandModule<ApplicationCommandContext>  {
    [SlashCommand("httpcat", "HTTP Codes but Cats!")]
    public async Task HttpCatTime(
        [SlashCommandParameter(Name = "code", Description = "Which HTTP code to use", MinLength = 3,
            MaxLength = 3)]
        int code) {
        
        // theres obv no check atm if it's an invalid HTTP code so anything invalid will just be a weird null image lmao
        ComponentContainerProperties component = new ComponentContainerProperties()
            .AddComponents([
                new MediaGalleryProperties([
                    new MediaGalleryItemProperties(new ComponentMediaProperties($"https://http.cat/{code.ToString()}"))
                ])
            ]);
        
        await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties() {
            Components = [ component ]
        }.WithFlags(MessageFlags.IsComponentsV2)));
    }
}