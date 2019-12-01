using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PushNotifyLocal.Plugin.Abstractions;

namespace PushNotifyLocal.Plugin
{
    public class LocalNotifications : ILocalNotifications
    {
        internal NotificationResponse delayedNotificationResponse = null;
        public EventHandler<NotificationResponse> _onNotificationOpened;
        public event EventHandler<NotificationResponse> OnNotificationOpened
        {
            add
            {
                var previousVal = _onNotificationOpened;
                _onNotificationOpened += value;
                if ((CrossLocalNotifications.Current as LocalNotifications).delayedNotificationResponse != null && previousVal == null)
                {
                    var tmpParams = (CrossLocalNotifications.Current as LocalNotifications).delayedNotificationResponse;
                    if (string.IsNullOrEmpty(tmpParams.Identifier))
                    {
                        _onNotificationOpened?.Invoke(CrossLocalNotifications.Current, tmpParams);
                        (CrossLocalNotifications.Current as LocalNotifications).delayedNotificationResponse = null;
                    }
                }
            }
            remove
            {
                _onNotificationOpened -= value;
            }
        }
        public static int CountNotify = 0;
        internal static Context ContextMain { get; set; }

        private Activity activity;
        private LocalNotificationManager localNotificationManager;
        private SnackbarManager snackbarNotification;
        private ScheduledAlarmManager scheduledAlarmManager;
        private IAndroidOptions androidOptions;
        public void Init(Activity activity, IAndroidOptions androidOptions)
        {
            this.activity = activity;
            this.androidOptions = androidOptions;
            this.localNotificationManager = new LocalNotificationManager();
            this.snackbarNotification = new SnackbarManager();
            this.scheduledAlarmManager = new ScheduledAlarmManager();
            Common.IsOnAppp = true;
        }

        public void ProcessIntent(Type activityMainType)
        {
            Common.MainActivityType = activityMainType;
        }
        public void PushNotify(string title, string body, IDictionary<string, string> data, bool isClickable)
        {
            NotificationOptions options = NotificationConfig(title, body, isClickable, false, data, null);
            Task.Run(() =>
            {
                _ = this.localNotificationManager.Notify(options);
            });
        }

        public void PushNotify(string title, string body, IDictionary<string, string> data, Action<NotificationResult> callback)
        {
            NotificationOptions options = NotificationConfig(title, body, false, true, data, null);

            Task.Run(() =>
            {
                return this.localNotificationManager.Notify(options);
            }).ContinueWith((task) =>
            {
                callback.Invoke(task.Result);
            });
        }

        public void PushSnackbar(string title, string body, IDictionary<string, string> data,string urlAvatar, bool isClickable)
        {
            NotificationOptions options = NotificationConfig(title, body, isClickable, false, data, urlAvatar);
            Task.Run(() =>
            {
                _ = this.snackbarNotification.Notify(activity, options);
            });
        }

        public void PushSnackbar(string title, string body, IDictionary<string, string> data, Action<NotificationResult> callback, string urlAvatar)
        {
            NotificationOptions options = NotificationConfig(title, body, false, true, data, urlAvatar);

            Task.Run(() =>
            {
                return this.snackbarNotification.Notify(activity, options);
            }).ContinueWith((task) =>
            {
                callback.Invoke(task.Result);
            });
        }

        public void AddSchedule(int id,string title, string body, DateTime dateTime, IDictionary<string, string> data, bool isClickable)
        {
            IScheduledOption options = ScheduleConfig(id, title, body, isClickable, false, dateTime);

            this.scheduledAlarmManager.AddScheduleAlarm(id, options, data);
        }

        public void RemoveSchedule(int id)
        {
            this.scheduledAlarmManager.RemoveScheduleAlarm(id);
        }

        private NotificationOptions NotificationConfig(string title, string body, bool isClickable,bool isCallback, IDictionary<string, string> data, string avatar, DateTime? notifyTime = null)
        {
            return new NotificationOptions()
            {
                Title = title,
                Body = body,
                CustomArgs = data,
                IsClickable = isClickable,
                IsCallback = isCallback,
                DelayUntil = notifyTime,
                Avatar = avatar,
                AndroidOptions = this.androidOptions
            };
        }
        private ScheduledOption ScheduleConfig(int id, string title, string body, bool isClickable, bool isCallback,DateTime notifyTime)
        {
            return  new ScheduledOption()
            {
                ScheduleID = id,
                Title = title,
                Body = body,
                DelayUntil = notifyTime,
                IsClickable = isClickable,
                SmallDrawableIcon = androidOptions.SmallDrawableIcon
            };
        }
    }
}