using PushNotifyLocal.Plugin.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PushNotifyLocal.Plugin
{
    public class NotificationOptions : INotificationOptions
    {
        public string Body { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string Avatar { get; set; } = null;

        public bool IsClickable { get; set; } = false;
        public bool IsCallback { get; set; } = false;
        
        public IAndroidOptions AndroidOptions { get; set; } = new AndroidOptions();

        public IiOSOptions iOSOptions { get; set; } = null;

        public IDictionary<string, string> CustomArgs { get; set; } = new ConcurrentDictionary<string, string>();

        public bool ClearFromHistory { get; set; } = false;

        public DateTime? DelayUntil { get; set; } = null;

        public bool AllowTapInNotificationCenter { get; set; } = true;

    }
}