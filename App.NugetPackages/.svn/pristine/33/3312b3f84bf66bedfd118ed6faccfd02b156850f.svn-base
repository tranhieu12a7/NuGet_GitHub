using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotifyLocal.Plugin.Abstractions
{
    public interface IAndroidOptions
    {
        /// <summary>
        /// Applicable only to Notification.Builder, if you want to replace the small icon, you must place an image in your drawables folder and pass the int through here. e.g. Resources.Drawable.MyNewIcon
        /// </summary>
        int? SmallDrawableIcon { get; }
        /// <summary>
        /// Gets or sets the hex colour for the notification to use.
        /// Colour usage changes based on the Android version.
        /// Example: #FF00CC
        /// </summary>
        /// <value>The hex colour</value>
        string HexColorBackground { get; }
        string HexColorText { get; }
        int TopMargin { get;}
        bool ForceOpenAppOnNotificationTap { get; }
        IAndroidChannelOptions ChannelOptions { get; }
    }
}
