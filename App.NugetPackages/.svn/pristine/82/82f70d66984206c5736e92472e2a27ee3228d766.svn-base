using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Java.Lang;
using PushNotifyLocal.Plugin.Abstractions;

namespace PushNotifyLocal.Plugin
{
    [BroadcastReceiver(Enabled = true, Label = "Scheduled Alarm Broadcast Receiver")]
    internal class ScheduledAlarmBroadcastReceiver : BroadcastReceiver
    {
        object _lock = new object();
        public override void OnReceive(Context context, Intent intent)
        {
            int notificationId = 0;
            lock (_lock)
            {
                notificationId = LocalNotifications.CountNotify;
                LocalNotifications.CountNotify++;
            }
            var extra = intent.GetStringExtra(Common.ScheduledAlarmOption);
            var option = Common.DeserializeScheduledOption(extra);
            string customStr = intent.Extras.GetString(Common.NotificationCustomArgs);

            int smallIcon;
            if (option.SmallDrawableIcon.HasValue)
            {
                smallIcon = option.SmallDrawableIcon.Value;
            }
            else
            {
                smallIcon = Resource.Drawable.ic_notify_white;
            }

            NotificationCompat.Builder builder = new NotificationCompat.Builder(context);

            var clickIntent = new Intent(Application.Context, typeof(NotificationBroadcastReceiver)).SetAction(Common.OnClickIntent);
            clickIntent.PutExtra(Common.NotificationId, notificationId);
            clickIntent.PutExtra(Common.NotificationOnClickHandler, option.IsClickable);
            clickIntent.PutExtra(Common.NotificationCallback, false);
            clickIntent.PutExtra(Common.NotificationCustomArgs, customStr);

            var pendingClickIntent = PendingIntent.GetBroadcast(context, (Common.StartId + notificationId), clickIntent, PendingIntentFlags.CancelCurrent);

            builder.SetSmallIcon(Resource.Drawable.ic_notify_white)
                .SetStyle(new NotificationCompat.BigTextStyle().BigText(option.Body))
                .SetPriority(NotificationCompat.PriorityHigh)
                .SetContentTitle(option.Title)
                .SetAutoCancel(true)
                .SetContentIntent(pendingClickIntent);

            IAndroidChannelOptions androidChannel = new AndroidChannelOptions();
            var notificationChannelId = Common.GetOrCreateChannel(androidChannel);
            if (!string.IsNullOrEmpty(notificationChannelId))
            {
                builder.SetChannelId(notificationChannelId);
            }

            if (Build.VERSION.SdkInt > BuildVersionCodes.Lollipop)
            {
                builder.SetSmallIcon(smallIcon);
                builder.SetColor(ContextCompat.GetColor(Application.Context, Resource.Color.notifyColor));
            }

            Notification notification = builder.Build();

            NotificationManager notificationManager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

            notificationManager.Notify(notificationId, notification);
        }
    }
}