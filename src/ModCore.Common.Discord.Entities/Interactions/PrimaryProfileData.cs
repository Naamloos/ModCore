using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModCore.Common.Discord.Entities.Interactions
{
    public record PrimaryProfileData
    {
        [JsonPropertyName("season")]
        public Optional<string> Season { get; set; }

        [JsonPropertyName("rank_name")]
        public Optional<string> RankName { get; set; }

        [JsonPropertyName("rank_image")]
        public Optional<Media> RankImage { get; set; }

        [JsonPropertyName("highest_rank")]
        public Optional<string> HighestRank { get; set; }

        [JsonPropertyName("highest_rank_image")]
        public Optional<Media> HighestRankImage { get; set; }

        [JsonPropertyName("featured_played_character")]
        public Optional<string> FeaturedPlayedCharacter { get; set; }

        [JsonPropertyName("featured_played_character_image")]
        public Optional<Media> FeaturedPlayedCharacterImage { get; set; }

        [JsonPropertyName("playtime_hours")]
        public Optional<double> PlaytimeHours { get; set; }

        [JsonPropertyName("total_wins")]
        public Optional<int> TotalWins { get; set; }

        [JsonPropertyName("current_period_wins")]
        public Optional<int> CurrentPeriodWins { get; set; }

        [JsonPropertyName("total_games")]
        public Optional<int> TotalGames { get; set; }

        [JsonPropertyName("current_period_games")]
        public Optional<int> CurrentPeriodGames { get; set; }

        [JsonPropertyName("total_kills")]
        public Optional<int> TotalKills { get; set; }

        [JsonPropertyName("current_period_kills")]
        public Optional<int> CurrentPeriodKills { get; set; }

        [JsonPropertyName("total_assists")]
        public Optional<int> TotalAssists { get; set; }

        [JsonPropertyName("current_period_assists")]
        public Optional<int> CurrentPeriodAssists { get; set; }

        [JsonPropertyName("total_deaths")]
        public Optional<int> TotalDeaths { get; set; }

        [JsonPropertyName("current_period_deaths")]
        public Optional<int> CurrentPeriodDeaths { get; set; }
    }
}
