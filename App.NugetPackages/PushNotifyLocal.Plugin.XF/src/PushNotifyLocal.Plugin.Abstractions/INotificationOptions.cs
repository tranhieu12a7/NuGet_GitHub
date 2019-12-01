using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotifyLocal.Plugin.Abstractions
{
    public interface INotificationOptions
    {
        string Title { get; }
        string Body { get; }
        string Avatar { get; }
        bool IsClickable { get; }
        bool IsCallback { get; }
        bool ClearFromHistory { get; }
        Nullable<DateTime> DelayUntil { get; }
        IDictionary<string, string> CustomArgs { get; }
        IAndroidOptions AndroidOptions { get; }
        IiOSOptions iOSOptions { get; }
        bool AllowTapInNotificationCenter { get; }
    }
}
