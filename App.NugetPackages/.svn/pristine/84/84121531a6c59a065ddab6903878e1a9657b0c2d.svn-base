using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using PushNotifyLocal.Plugin.Abstractions;
using UIKit;
using UserNotifications;

namespace PushNotifyLocal.Plugin
{
    public class UserNotificationCenterDelegateManager : UNUserNotificationCenterDelegate
    {
        private Action<string, NotificationResult> _action;
        private string _id;
        private bool _cancel;
        private bool _allowTapInNotificationCenter;
        public UserNotificationCenterDelegateManager()
        {
            _action = null;
        }
        public UserNotificationCenterDelegateManager(string id, Action<string, NotificationResult> action, bool cancel, bool allowTapInNotificationCenter)
        {
            _action = action;
            _id = id;
            _cancel = cancel;
            _allowTapInNotificationCenter = allowTapInNotificationCenter;
        }

        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            var parameters = notification.Request.Content.UserInfo.ToDictionary(i => i.Key.ToString(), i => i.Value.ToString());

            if (_action != null)
            {
                // Timer here for a timeout since no Toast Dismissed Event (7 seconds til auto dismiss)
                var timer = NSTimer.CreateScheduledTimer(TimeSpan.FromSeconds(7), (nsTimer) =>
                {
                    if (_cancel || !_allowTapInNotificationCenter)
                        _action(_id, new NotificationResult() { Action = NotificationAction.Timeout });

                    if (_cancel) // Clear notification from list
                        UNUserNotificationCenter.Current.RemoveDeliveredNotifications(new string[] { _id });

                    nsTimer.Invalidate();
                });
            }
            // check firebase
            var gcmid = notification.Request.Content.UserInfo.ValueForKey(new NSString("gcm.message_id"));
            if (gcmid != null)
            {
                bool isInBackground = UIApplication.SharedApplication.ApplicationState == UIApplicationState.Background;
                if (isInBackground)
                {
                    // Shows toast on screen
                    completionHandler(UNNotificationPresentationOptions.Alert);
                }
                else
                {
                    string title = notification.Request.Content.Title;
                    string body = notification.Request.Content.Body;
                    (CrossLocalNotifications.Current as LocalNotifications).PushSnackbar(title, body,
                        null, 
                        new Action<NotificationResult>(result =>
                        {
                            string customStr = parameters.GetValueOrDefault(Common.NotificationCustomArgs);
                            System.Collections.Generic.IDictionary<string,string> data = (customStr != null) ? Common.DeserializeDictionary(customStr) : parameters;
                            (CrossLocalNotifications.Current as LocalNotifications)._onNotificationOpened?.Invoke(this, new NotificationResponse(data));
                        }), null);
                }
                return;
            }

            completionHandler(UNNotificationPresentationOptions.Alert);
        }

        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            var parameters = response.Notification.Request.Content.UserInfo.ToDictionary(i => i.Key.ToString(), i => i.Value.ToString());
            bool isOnClick = Convert.ToBoolean(parameters.GetValueOrDefault(Common.NotificationOnClickHandler, "false"));
            bool isCallback = Convert.ToBoolean(parameters.GetValueOrDefault(Common.NotificationCallback, "false"));
            // check firebase
            var gcmid = parameters.GetValueOrDefault("gcm.message_id");
            if ((isOnClick && !isCallback) || gcmid != null) 
            {
                string customStr = parameters.GetValueOrDefault(Common.NotificationCustomArgs);
                System.Collections.Generic.IDictionary<string, string> data = (customStr != null) ? Common.DeserializeDictionary(customStr) : parameters;
                (CrossLocalNotifications.Current as LocalNotifications)._onNotificationOpened?.Invoke(this, new NotificationResponse(data, response.ActionIdentifier));
            }
            // I Clicked it :)
            if(_action != null)
                _action(_id, new NotificationResult() { Action = NotificationAction.Clicked });

            completionHandler();
        }
    }
}