using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using PushNotifyLocal.Plugin.Abstractions;

namespace PushNotifyLocal.Plugin
{
    public class NotificationOptions : INotificationOptions
    {
        public string Body { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string Avatar { get; set; } = null;

        public bool IsClickable { get; set; } = false;
        public bool IsCallback { get; set; } = false;

        public IAndroidOptions AndroidOptions { get; set; } = null;

        public IiOSOptions iOSOptions { get; set; } = new IOSOptions();

        public IDictionary<string, string> CustomArgs { get; set; } = new ConcurrentDictionary<string, string>();

        public bool ClearFromHistory { get; set; } = false;

        public DateTime? DelayUntil { get; set; } = null;

        public bool AllowTapInNotificationCenter { get; set; } = true;
    }
}