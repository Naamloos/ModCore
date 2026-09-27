using System;
using System.Collections.Generic;
using System.Text;
using ModCore.Services.Consumer.Interactions.Framework;

namespace ModCore.Services.Consumer.Interactions.Remind
{
    public class RemindCommand : BaseApplicationCommand
    {
        public override string Name => "remind";

        public override string Description => "Commands related to reminders";
    }
}
