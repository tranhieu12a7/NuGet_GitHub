using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PushNotifyLocal.Plugin.Abstractions;

namespace PushNotifyLocal.Plugin
{
    [BroadcastReceiver(Enabled = true, Label = "Local Notifications Broadcast Receiver")]
    internal class NotificationBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var notificationId = intent.Extras.GetInt(Common.NotificationId, -1);
            if (notificationId > -1)
            {
                
                switch (intent.Action)
                {
                    case Common.OnClickIntent:
                        bool isOnClick = intent.Extras.GetBoolean(Common.NotificationOnClickHandler, false);
                        bool isCallback = intent.Extras.GetBoolean(Common.NotificationCallback, false);
                        if (isOnClick && !isCallback)
                        {
                            string customStr = intent.Extras.GetString(Common.NotificationCustomArgs);
                            var data = (customStr != null) ? Common.DeserializeDictionary(customStr) : null;
                            if (Common.IsOnAppp)
                            {
                                (CrossLocalNotifications.Current as LocalNotifications)._onNotificationOpened?.Invoke(this, new NotificationResponse(data, Common.ActionIdentifierKey));
                            }
                            else
                            {
                                (CrossLocalNotifications.Current as LocalNotifications).delayedNotificationResponse = new NotificationResponse(data);
                            }

                            Intent launchIntent = new Intent(context, Common.MainActivityType);
                            //launchIntent.SetFlags(ActivityFlags.NewTask);
                            context.StartActivity(launchIntent);
                        }
                        
                        // Click
                        if (LocalNotificationManager.EventResult != null && !LocalNotificationManager.EventResult.ContainsKey(notificationId.ToString()))
                        {
                            LocalNotificationManager.EventResult.Add(notificationId.ToString(), new NotificationResult() { Action = NotificationAction.Clicked, Id = notificationId });
                        }
                        break;

                    default:

                        // Dismiss/Default
                        if (LocalNotificationManager.EventResult != null && !LocalNotificationManager.EventResult.ContainsKey(notificationId.ToString()))
                        {
                            LocalNotificationManager.EventResult.Add(notificationId.ToString(), new NotificationResult() { Action = NotificationAction.Dismissed, Id = notificationId });
                        }
                        break;
                }

                if (LocalNotificationManager.ResetEvent != null && LocalNotificationManager.ResetEvent.ContainsKey(notificationId.ToString()))
                {
                    LocalNotificationManager.ResetEvent[notificationId.ToString()].Set();
                }
            }
        }
    }
}