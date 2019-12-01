using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using PushNotifyLocal.Plugin.Abstractions;

namespace PushNotifyLocal.Plugin
{
    internal class LocalNotificationManager
    {
        
        object _lock = new object();
        NotificationManager _manager => (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);
        public static IDictionary<string, ManualResetEvent> ResetEvent = new ConcurrentDictionary<string, ManualResetEvent>();
        public static IDictionary<string, NotificationResult> EventResult = new ConcurrentDictionary<string, NotificationResult>();
        public static List<string> Channels = new List<string>();

        public NotificationResult Notify(NotificationOptions notificationOptions)
        {
            NotificationResult notificationResult = new NotificationResult();
            var notificationId = 0;
            var id = "";
            lock (_lock)
            {
                notificationId = LocalNotifications.CountNotify;
                id = notificationId.ToString();
                LocalNotifications.CountNotify++;
            }

            int smallIcon;
            if (notificationOptions.AndroidOptions.SmallDrawableIcon.HasValue)
            {
                smallIcon = notificationOptions.AndroidOptions.SmallDrawableIcon.Value;
            }
            else
            {
                smallIcon = Resource.Drawable.ic_notify_white;
            }

            // Show Notification Right Now
            var dismissIntent = new Intent(Application.Context, typeof(NotificationBroadcastReceiver)).SetAction(Common.DismissedClickIntent);
            dismissIntent.PutExtra(Common.NotificationId, notificationId);

            var pendingDismissIntent = PendingIntent.GetBroadcast(Application.Context, (Common.StartId + notificationId), dismissIntent, PendingIntentFlags.CancelCurrent);

            var clickIntent = new Intent(Application.Context, typeof(NotificationBroadcastReceiver)).SetAction(Common.OnClickIntent);
            clickIntent.PutExtra(Common.NotificationId, notificationId);
            clickIntent.PutExtra(Common.NotificationOnClickHandler, notificationOptions.IsClickable);
            clickIntent.PutExtra(Common.NotificationCallback, notificationOptions.IsCallback);
            if (notificationOptions.CustomArgs != null)
            {
                var notifyArgs = Common.SerializeDictionary(notificationOptions.CustomArgs);
                clickIntent.PutExtra(Common.NotificationCustomArgs, notifyArgs);
            }
           
            var pendingClickIntent = PendingIntent.GetBroadcast(Application.Context, (Common.StartId + notificationId), clickIntent, PendingIntentFlags.CancelCurrent);

            int background = Color.Gray;
            if (!string.IsNullOrEmpty(notificationOptions.AndroidOptions.HexColorBackground) && notificationOptions.AndroidOptions.HexColorBackground.Substring(0, 1) != "#")
            {
                string backgroundStr = "#" + notificationOptions.AndroidOptions.HexColorBackground;
                background = Color.ParseColor(backgroundStr);
            }

            var builder = new Notification.Builder(Application.Context)
                .SetContentTitle(notificationOptions.Title)
                .SetContentText(notificationOptions.Body)
                .SetPriority((int)NotificationPriority.High) // Must be set to High to get Heads-up notification
                .SetDefaults(NotificationDefaults.All) // Must also include vibrate to get Heads-up notification
                .SetAutoCancel(true) // To allow click event to trigger delete Intent
                .SetContentIntent(pendingClickIntent) // Must have Intent to accept the click                   
                .SetDeleteIntent(pendingDismissIntent)
                .SetColor(background);

            if (Build.VERSION.SdkInt > BuildVersionCodes.Lollipop)
            {
                builder.SetSmallIcon(smallIcon);
                builder.SetColor(ContextCompat.GetColor(Application.Context, Resource.Color.notifyColor));
            }

            var notificationChannelId = Common.GetOrCreateChannel(notificationOptions.AndroidOptions.ChannelOptions);
            if (!string.IsNullOrEmpty(notificationChannelId))
            {
                builder.SetChannelId(notificationChannelId);
            }

            // System.MissingMethodException: Method 'Android.App.Notification/Builder.SetChannelId' not found.
            // I know this is bad, but I can't replicate it on any version, and many people are experiencing it.

            Notification notification = builder.Build();

            NotificationManager notificationManager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

            notificationManager.Notify(notificationId, notification);

            //if (notificationOptions.DelayUntil.HasValue)
            //{
            //    return new NotificationResult() { Action = NotificationAction.NotApplicable, Id = notificationId };
            //}

            var timer = new System.Threading.Timer(x => TimerFinished(id, notificationOptions.ClearFromHistory, notificationOptions.AllowTapInNotificationCenter), null, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(-1));

            var resetEvent = new ManualResetEvent(false);
            ResetEvent.Add(id, resetEvent);

            resetEvent.WaitOne(); // Wait for a result

            notificationResult = EventResult[id];

            //if (!notificationOptions.IsClickable && notificationResult.Action == NotificationAction.Clicked)
            //{
            //    notificationResult.Action = NotificationAction.Dismissed;
            //    notificationResult.Id = notificationId;
            //}

            if (EventResult.ContainsKey(id))
            {
                EventResult.Remove(id);
            }
            if (ResetEvent.ContainsKey(id))
            {
                ResetEvent.Remove(id);
            }

            // Dispose of Intents and Timer
            pendingClickIntent.Cancel();
            pendingDismissIntent.Cancel();
            timer.Dispose();
            return notificationResult;
        }
        
        void TimerFinished(string id, bool cancel, bool allowTapInNotificationCenter)
        {
            if (string.IsNullOrEmpty(id))
                return;

            if (cancel) // Will clear from Notification Center
            {
                using (NotificationManager notificationManager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager)
                {
                    notificationManager.Cancel(Convert.ToInt32(id));
                }
            }

            if (!allowTapInNotificationCenter || cancel)
            {
                if (ResetEvent.ContainsKey(id))
                {
                    if (EventResult != null)
                    {
                        EventResult.Add(id, new NotificationResult() { Action = NotificationAction.Timeout, Id = int.Parse(id) });
                    }
                    if (ResetEvent != null && ResetEvent.ContainsKey(id))
                    {
                        ResetEvent[id].Set();
                    }
                }
            }
        }
    }
}