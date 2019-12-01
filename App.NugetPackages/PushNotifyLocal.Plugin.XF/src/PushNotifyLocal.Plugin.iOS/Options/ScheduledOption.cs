using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using PushNotifyLocal.Plugin.Abstractions;
using UIKit;

namespace PushNotifyLocal.Plugin
{
    public class ScheduledOption : IScheduledOption
    {
        public int ScheduleID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsClickable { get; set; } = false;
        public DateTime DelayUntil { get; set; }
        public int? SmallDrawableIcon { get; set; } = null;
    }
}