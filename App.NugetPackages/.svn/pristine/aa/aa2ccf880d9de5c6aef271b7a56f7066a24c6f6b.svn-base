using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using PushNotifyLocal.Plugin.Abstractions;
using UIKit;

namespace PushNotifyLocal.Plugin
{
    public class LocalNotificationManager
    {
        private static object _lock = new object();

        public NotificationResult Notify(INotificationOptions options)
        {
            // create the notification
            var notification = new UILocalNotification();

            // set the fire date (the date time in which it will fire)
            notification.FireDate = NSDate.Now;

            // configure the alert
            notification.AlertTitle = options.Title;
            notification.AlertBody = options.Body;

            // set the sound to be the default sound
            notification.SoundName = UILocalNotification.DefaultSoundName;

            if (options.CustomArgs != null)
            {
                NSMutableDictionary dictionary = new NSMutableDictionary();
                foreach (var arg in options.CustomArgs)
                    dictionary.SetValueForKey(NSObject.FromObject(arg.Value), new NSString(arg.Key));

                // Don't document, this feature is most likely to change
                dictionary.SetValueForKey(NSObject.FromObject(System.Guid.NewGuid().ToString()), new NSString("Identifier"));

                notification.UserInfo = dictionary;
            }
            // schedule it
            UIApplication.SharedApplication.ScheduleLocalNotification(notification);

            return new NotificationResult() { Action = NotificationAction.NotApplicable };
        }
    }
}