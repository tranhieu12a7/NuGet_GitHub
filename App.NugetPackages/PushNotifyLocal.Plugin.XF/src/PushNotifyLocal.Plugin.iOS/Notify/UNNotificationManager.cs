using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Foundation;
using PushNotifyLocal.Plugin.Abstractions;
using UIKit;
using UserNotifications;

namespace PushNotifyLocal.Plugin
{
    public class UNNotificationManager
    {
        private IDictionary<string, ManualResetEvent> _resetEvents = new ConcurrentDictionary<string, ManualResetEvent>();
        private IDictionary<string, NotificationResult> _eventResult = new ConcurrentDictionary<string, NotificationResult>();
        private int _count = 0;
        private static object _lock = new object();

        public NotificationResult Notify(INotificationOptions options)
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

            var notificationCenter = UNUserNotificationCenter.Current;

            var content = new UNMutableNotificationContent();
            content.Title = options.Title;
            content.Body = options.Body;
            content.Sound = UNNotificationSound.Default;
            if (options.iOSOptions != null && options.iOSOptions.SetBadgeCount)
                content.Badge = options.iOSOptions.BadgeCount;
            content.CategoryIdentifier = "message";
            UNNotificationTrigger trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(0.1, false);

            var id = _count.ToString();
            _count++;

            ArrayList arraykeys = new ArrayList();
            ArrayList arrayvalues = new ArrayList();
            if (options.CustomArgs?.Count > 0)
            {
                var data = options.CustomArgs;
                var notifyArgs = Common.SerializeDictionary(data);
                arraykeys.Add(Common.NotificationCustomArgs);
                arrayvalues.Add(notifyArgs);
            }

            arraykeys.Add(Common.NotificationId);
            arrayvalues.Add(id);

            arraykeys.Add(Common.NotificationOnClickHandler);
            arrayvalues.Add(options.IsClickable.ToString());

            arraykeys.Add(Common.NotificationCallback);
            arrayvalues.Add(options.IsCallback.ToString());

            object[] keys = arraykeys.ToArray();
            object[] values = arrayvalues.ToArray();
            content.UserInfo = NSDictionary.FromObjectsAndKeys(values, keys);

            
            notificationCenter.SetNotificationCategories(new NSSet<UNNotificationCategory>(categories));

            var request = UNNotificationRequest.FromIdentifier(id, content, trigger);
            notificationCenter.Delegate = new UserNotificationCenterDelegateManager(id, (identifier, notificationResult) =>
            {
                lock (_lock)
                    if (_resetEvents?.ContainsKey(identifier) == true && _eventResult?.ContainsKey(identifier) == false)
                    {
                        _eventResult.Add(identifier, notificationResult);
                        _resetEvents[identifier].Set();
                    }
            }, options.ClearFromHistory, false);

            var resetEvent = new ManualResetEvent(false);
            _resetEvents.Add(id, resetEvent);

            notificationCenter.AddNotificationRequest(request, (error) =>
            {
                if (error != null)
                    _eventResult?.Add(request.Identifier, new NotificationResult() { Action = NotificationAction.Failed });
            });

            if (options.DelayUntil.HasValue)
                return new NotificationResult() { Action = NotificationAction.NotApplicable };

            resetEvent.WaitOne();

            var result = _eventResult[id];

            _resetEvents.Remove(id);
            _eventResult.Remove(id);

            return result;
        }
    }
}