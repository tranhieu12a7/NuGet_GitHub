using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Widget;
using Java.Util;
using PushNotifyLocal.Plugin.Abstractions;

namespace PushNotifyLocal.Plugin
{
    internal class ScheduledAlarmManager
    {
        public void AddScheduleAlarm(int id, IScheduledOption scheduledOption, IDictionary<string, string> data)
        {
            var serializedNotification = Common.SerializeScheduledOption(scheduledOption);
            var intent = CreateIntent(id);
            intent.PutExtra(Common.ScheduledId, id);
            intent.PutExtra(Common.ScheduledAlarmOption, serializedNotification);
            intent.PutExtra(Common.NotificationOnClickHandler, scheduledOption.IsClickable);
            if (data != null)
            {
                var notifyArgs = Common.SerializeDictionary(data);
                intent.PutExtra(Common.NotificationCustomArgs, notifyArgs);
            }

            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, id, intent, PendingIntentFlags.UpdateCurrent);

            var alarmManager = GetAlarmManager();
            alarmManager.Set(AlarmType.RtcWakeup, Common.ConvertToMilliseconds(scheduledOption.DelayUntil), pendingIntent);
            //if (Build.VERSION.SdkInt >= Build.VERSION_CODES.M)
            //{
            //    //alarmManager.SetAndAllowWhileIdle(AlarmType.RtcWakeup, calendar.TimeInMillis, pendingIntent);
            //    alarmManager.Set(AlarmType.RtcWakeup, Common.ConvertToMilliseconds(notification.DelayUntil), pendingIntent);
            //}
            //else if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Kitkat)
            //{
            //    alarmManager.SetExact(AlarmType.RtcWakeup, calendar.TimeInMillis, pendingIntent);
            //}
            //else
            //{
            //    alarmManager.Set(AlarmType.RtcWakeup, calendar.TimeInMillis, pendingIntent);
            //}
        }

        public void RemoveScheduleAlarm(int id)
        {
            var intent = CreateIntent(id);
            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, id, intent, PendingIntentFlags.CancelCurrent);
            var alarmManager = GetAlarmManager();
            alarmManager.Cancel(pendingIntent);
        }

        private AlarmManager GetAlarmManager()
        {
            var alarmManager = Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;
            return alarmManager;
        }
        private Intent CreateIntent(int id)
        {
            return new Intent(Application.Context, typeof(ScheduledAlarmBroadcastReceiver))
                .SetAction(Common.ScheduledAlarmAction + id.ToString());
        }
    }
}