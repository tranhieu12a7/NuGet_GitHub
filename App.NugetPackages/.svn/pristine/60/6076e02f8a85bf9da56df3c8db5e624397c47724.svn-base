using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PushNotifyLocal.Plugin.Abstractions;

namespace PushNotifyLocal.Plugin
{
    public class ScheduledOption : IScheduledOption
    {
        public int ScheduleID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsClickable { get; set; } = true;
        public DateTime DelayUntil { get; set; }
        public int? SmallDrawableIcon { get; set; } = null;
    }
}