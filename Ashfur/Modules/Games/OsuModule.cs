using Ashfur.Utils;
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
        [SlashCommandParameter(Name = "player", Description = "Username of said osu! player")]
        string player) {
        ApiResult<UserExtended> userExtendedApiResult;
        // ApiResult<UserStatistics[]> userStatisticsApiResult;

        // ugh i wish i had kotlin's `var variable = try {} catch (e: Exception) {}` thing here, that's so much better syntactically.
        // this looks and feels unsightly.
        try {
            userExtendedApiResult = await osuApiClient.GetUserAsync(player, Ruleset.Osu);
            // userStatisticsApiResult =
            //     await osuApiClient.GetScoreRankingsAsync(Ruleset.Osu, userExtendedApiResult.Value.CountryCode);
        }
        catch (Exception e) {
            Log($"Could not fetch osu! data. {e}", LogType.Exception);
            await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties {
                Components = [ComponentUtils.BuildExceptionComponent("Could not fetch Osu data!", e)]
            }.WithFlags(MessageFlags.IsComponentsV2)));

            throw;
        }

        var userProfilePicture =
            new ComponentSectionThumbnailProperties(
                new ComponentMediaProperties(userExtendedApiResult.Value.AvatarUrl));
        var osuResult = userExtendedApiResult.Value;

        ComponentContainerProperties component = new ComponentContainerProperties()
            .AddComponents([
                new ComponentSectionProperties(userProfilePicture).WithComponents([
                    new TextDisplayProperties($"# {osuResult.Username}"),
                    new TextDisplayProperties("## osu! [standard] mode")
                ]),
                new ComponentSeparatorProperties().WithDivider(),
                new TextDisplayProperties($"County: {osuResult.Country.Name} [{osuResult.Country.Code}]"),
                new TextDisplayProperties(
                    $"Team: [{osuResult.Team.Name}](https://osu.ppy.sh/teams/{osuResult.Team.Id})"),
                new ComponentSeparatorProperties().WithDivider(),
                new TextDisplayProperties($"Global Ranking: {osuResult.Statistics.GlobalRank}"),
                new TextDisplayProperties($"Country Ranking: {osuResult.Statistics.CountryRank}"),
                new TextDisplayProperties(
                    $"Level: {osuResult.Statistics.Level.Current} [{osuResult.Statistics.Level.Progress}% of the way to the next level!]")
            ]);

        await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties {
            Components = [component]
        }.WithFlags(MessageFlags.IsComponentsV2)));
    }
}