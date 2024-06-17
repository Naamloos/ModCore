using ModCore.Common.Discord.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.InteractionFramework.Attributes
{
    public class SlashCommandAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Nsfw { get; set; }
        public bool DmPermission { get; set; }
        public Permissions Permissions { get; set; }

        public SlashCommandAttribute(string name, string description, Permissions permissions = Permissions.None, bool nsfw = false, bool dm_permission = false) 
        {
            this.Name = name;
            this.Description = description;
            this.Nsfw = nsfw;
            this.DmPermission = dm_permission;
            this.Permissions = permissions;
        }
    }
}
