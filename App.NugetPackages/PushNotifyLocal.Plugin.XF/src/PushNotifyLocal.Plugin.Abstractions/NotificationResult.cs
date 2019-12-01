using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotifyLocal.Plugin.Abstractions
{
    public class NotificationResult
    {
        public NotificationAction Action { get; set; }
        public int Id { get; set; }
    }
    public enum NotificationAction
    {
        Timeout = 1, // Hides by itself
        Clicked = 2, // User clicked on notification
        Dismissed = 4, // User manually dismissed notification
        ApplicationHidden = 8, // Application went to background
        Failed = 16, // When failed to display the toast,
        NotApplicable = 32 // When the ability to determine the outcome is not available
    }
}
