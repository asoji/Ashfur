using Ashfur.Utils;
using Microsoft.Extensions.Configuration;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using osu.NET;
using osu.NET.Enums;
using osu.NET.Models.Users;
using PrettyLogSharp;
using static PrettyLogSharp.PrettyLogger;

namespace Ashfur.Modules.Games;

[SlashCommand("osu", "Osu [lazer] player stats")]
public class OsuModule(OsuApiClient osuApiClient) : ApplicationCommandModule<ApplicationCommandContext> {
    [SubSlashCommand("standard", "View information about an osu! player on Standard mode")]
    public async Task StandardMode(
        [SlashCommandParameter(Name = "player", Description = "Username of said osu! player")] string player) {
        ApiResult<UserExtended> result;

        // ugh i wish i had kotlin's `var variable = try {} catch (e: Exception) {}` thing here, that's so much better syntactically.
        // this looks and feels unsightly.
        try {
            result = await osuApiClient.GetUserAsync(player, Ruleset.Osu);
        }
        catch (Exception e) {
            Log($"Could not fetch osu! data. {e}", LogType.Exception);
            await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties {
                Components = [ComponentUtils.BuildExceptionComponent("Could not fetch Osu data!", e)]
            }.WithFlags(MessageFlags.IsComponentsV2)));

            throw;
        }

        var userProfilePicture = new ComponentSectionThumbnailProperties(new ComponentMediaProperties(result.Value.AvatarUrl));

        ComponentContainerProperties component = new ComponentContainerProperties()
            .AddComponents([
                new ComponentSectionProperties(userProfilePicture).WithComponents([
                    new TextDisplayProperties($"# {result.Value.Username}")
                ]),
                new ComponentSeparatorProperties().WithDivider(),
            ]);

        await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties() {
            Components = [component]
        }.WithFlags(MessageFlags.IsComponentsV2)));
    }
}