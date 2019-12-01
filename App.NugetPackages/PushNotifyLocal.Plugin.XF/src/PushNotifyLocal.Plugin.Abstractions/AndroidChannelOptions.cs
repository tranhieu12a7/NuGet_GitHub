using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotifyLocal.Plugin.Abstractions
{
    public class AndroidChannelOptions : IAndroidChannelOptions
    {
        public string Name { get; set; } = "default";
        public string Description { get; set; } = null;
        public bool EnableVibration { get; set; } = true;
        public bool ShowBadge { get; set; } = false;
    }
}
