using ModCore.Entities;

namespace ModCore.Commands.Base
{
    public abstract class LinkfilterConfigModule : SimpleConfigModule
    {
        protected override ref bool GetSetting(GuildSettings cfg) => ref GetSetting(cfg.Linkfilter);

        protected abstract ref bool GetSetting(GuildLinkfilterSettings cfgLinkfilter);
    }
}