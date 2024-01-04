using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.InteractionFramework.Attributes
{
    public class SlashCommandAttribute : Attribute
    {
        public string Description { get; set; }
        public bool Nsfw { get; set; }
        public bool DmPermission { get; set; }

        public SlashCommandAttribute(string description, bool nsfw = false, bool dm_permission = false) 
        {
            this.Description = description;
            this.Nsfw = nsfw;
            this.DmPermission = dm_permission;
        }
    }
}
