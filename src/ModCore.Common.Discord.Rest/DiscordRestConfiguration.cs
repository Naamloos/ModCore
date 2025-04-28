namespace ModCore.Common.Discord.Rest
{
    public record DiscordRestConfiguration
    {
        public string Token { get; set; } = "";
        public string RestProxy { get; set; } = "";
        public string AuthType { get; set; } = "Bot";
    }
}
