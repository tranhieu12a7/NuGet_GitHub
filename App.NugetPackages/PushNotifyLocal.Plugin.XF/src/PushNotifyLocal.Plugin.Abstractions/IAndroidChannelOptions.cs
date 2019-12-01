using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotifyLocal.Plugin.Abstractions
{
    public interface IAndroidChannelOptions
    {
        /// <summary>
        /// The name of the notification channel.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// The description of the notification channel.
        /// (Optional)
        /// </summary>
        /// <value>The description.</value>
        string Description { get;}

        /// <summary>
        /// Whether the notifications can use vibration or not.
        /// (Default: True)
        /// </summary>
        /// <value><c>true</c> if enable vibration; otherwise, <c>false</c>.</value>
        bool EnableVibration { get;}

        /// <summary>
        /// Whether the badge is shown over the app icon when notifications are available.
        /// (Default: True)
        /// </summary>
        /// <value><c>true</c> if show badge; otherwise, <c>false</c>.</value>
        bool ShowBadge { get;}
    }
}
