using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using PushNotifyLocal.Plugin.Abstractions;
using UIKit;
using UserNotifications;

namespace PushNotifyLocal.Plugin
{
    internal class ScheduledAlarmManager : NSObject
    {
        public const string NotificationKey = "NOTIFICATION_KEY";

        public void AddScheduleAlarm( ScheduledOption options, IDictionary<string, string> data)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                UNNotification(options, data);
            else
                LocalNotification(options, data);
        }
        public async void RemoveScheduleAlarm(int id)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                var deliveredNotifications = await UNUserNotificationCenter.Current.GetDeliveredNotificationsAsync();
                var deliveredNotificationsMatches = deliveredNotifications.Where(u => $"{u.Request.Content.UserInfo[NotificationKey]}".Equals($"{id}")).Select(s => s.Request.Identifier).ToArray();
                if (deliveredNotificationsMatches.Length > 0)
                {
                    UNUserNotificationCenter.Current.RemoveDeliveredNotifications(deliveredNotificationsMatches);
                }
            }
            else
            {
                var scheduledNotifications = UIApplication.SharedApplication.ScheduledLocalNotifications.Where(u => u.UserInfo[NotificationKey].Equals($"{id}"));
                foreach (var notification in scheduledNotifications)
                {
                    UIApplication.SharedApplication.CancelLocalNotification(notification);
                }
            }
        }
        public void LocalNotification(IScheduledOption options, IDictionary<string, string> data)
        {
            // create the notification
            var notification = new UILocalNotification();

            // set the fire date (the date time in which it will fire)
            notification.FireDate = options.DelayUntil.ToNSDate();

            // configure the alert
            notification.AlertTitle = options.Title;
            notification.AlertBody = options.Body;
            // set the sound to be the default sound
            notification.SoundName = UILocalNotification.DefaultSoundName;

            if (data != null)
            {
                NSMutableDictionary dictionary = new NSMutableDictionary();
                foreach (var arg in data)
                    dictionary.SetValueForKey(NSObject.FromObject(arg.Value), new NSString(arg.Key));

                // Don't document, this feature is most likely to change
                dictionary.SetValueForKey(NSObject.FromObject(options.ScheduleID.ToString()), new NSString(NotificationKey));

                notification.UserInfo = dictionary;
            }
            // schedule it
            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
        }

        public void UNNotification(IScheduledOption options, IDictionary<string, string> data)
        {
            // Create action
            var actionID = "reply";
            var title = "Reply";
            var action = UNNotificationAction.FromIdentifier(actionID, title, UNNotificationActionOptions.None);

            // Create category
            var categoryID = "message";
            var actions = new UNNotificationAction[] { action };
            var intentIDs = new string[] { };
            //var categoryOptions = new UNNotificationCategoryOptions[] { };
            var category = UNNotificationCategory.FromIdentifier(categoryID, actions, intentIDs, UNNotificationCategoryOptions.None);

            // Register category
            var categories = new UNNotificationCategory[] { category };

            ////////////////////////////////////////

            var content = new UNMutableNotificationContent();
            content.Title = options.Title;
            content.Body = options.Body;
            content.Sound = UNNotificationSound.Default;
            content.CategoryIdentifier = categoryID;

            UNNotificationTrigger trigger = UNCalendarNotificationTrigger.CreateTrigger(options.DelayUntil.ToNSDateComponents(), false);
            ArrayList arraykeys = new ArrayList();
            ArrayList arrayvalues = new ArrayList();

            if (data != null)
            {
                var notifyArgs = Common.SerializeDictionary(data);
                arraykeys.Add(Common.NotificationCustomArgs);
                arrayvalues.Add(notifyArgs);
            }

            arraykeys.Add(Common.NotificationId);
            arrayvalues.Add(options.ScheduleID);

            arraykeys.Add(Common.NotificationOnClickHandler);
            arrayvalues.Add(options.IsClickable.ToString());

            arraykeys.Add(Common.NotificationCallback);
            arrayvalues.Add("False");

            object[] keys = arraykeys.ToArray();
            object[] values = arrayvalues.ToArray();
            content.UserInfo = NSDictionary.FromObjectsAndKeys(values, keys);

            UNUserNotificationCenter notificationCenter = UNUserNotificationCenter.Current;

            notificationCenter.SetNotificationCategories(new NSSet<UNNotificationCategory>(categories));

            var request = UNNotificationRequest.FromIdentifier(options.ScheduleID.ToString(), content, trigger);

            notificationCenter.Delegate = new UserNotificationCenterDelegateManager();

            notificationCenter.AddNotificationRequest(request, (error) =>
            {

            });
        }
    }
}