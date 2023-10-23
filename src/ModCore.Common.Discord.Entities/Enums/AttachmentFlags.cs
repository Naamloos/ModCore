using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModCore.Common.Discord.Entities.Enums
{
    [Flags]
    public enum AttachmentFlags
    {
        IsRemix = 1 << 2
    }
}
