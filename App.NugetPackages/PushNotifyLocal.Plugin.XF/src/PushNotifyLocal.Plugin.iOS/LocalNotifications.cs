using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using PushNotifyLocal.Plugin.Abstractions;
using UIKit;

namespace PushNotifyLocal.Plugin
{
    public class LocalNotifications : ILocalNotifications
    {
        public EventHandler<NotificationResponse> _onNotificationOpened;
        public event EventHandler<NotificationResponse> OnNotificationOpened
        {
            add
            {
                _onNotificationOpened += value;
            }
            remove
            {
                _onNotificationOpened -= value;
            }
        }

        private UNNotificationManager uNNotificationManager;
        private LocalNotificationManager localNotificationManager;
        private SnackbarManager snackbarNotification;
        private ScheduledAlarmManager scheduledAlarmManager;
        private IiOSOptions iOSOptions;

        public void Init(IiOSOptions iOSOptions)
        {
            this.iOSOptions = iOSOptions;
            this.snackbarNotification = new SnackbarManager();
            this.scheduledAlarmManager = new ScheduledAlarmManager();
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                uNNotificationManager = new UNNotificationManager();
            else
                localNotificationManager = new LocalNotificationManager();
        }
        public void PushNotify(string title, string body, IDictionary<string, string> data, bool isClickable)
        {
            NotificationOptions options = NotificationConfig(title, body, isClickable, false, data, null);

            _ = Notify(options);
        }

        public void PushNotify(string title, string body, IDictionary<string, string> data, Action<NotificationResult> callback)
        {
            NotificationOptions options = NotificationConfig(title, body,false, true, data, null);

            Task.Run(async () =>
            {
                return await Notify(options);
            }).ContinueWith((task) =>
            {
                callback.Invoke(task.Result);
            });
        }

        public void PushSnackbar(string title, string body, IDictionary<string, string> data, string urlAvatar, bool isClickable)
        {
            NotificationOptions options = NotificationConfig(title, body, isClickable, false, data, urlAvatar);

            _ = this.snackbarNotification.Notify(options);
        }

        public async void PushSnackbar(string title, string body, IDictionary<string, string> data, Action<NotificationResult> callback, string urlAvatar)
        {
            NotificationOptions options = NotificationConfig(title, body, false, true, data, urlAvatar);

            var result = await this.snackbarNotification.Notify(options);
            callback.Invoke(result);
        }
        public void AddSchedule(int id,string title, string body, DateTime dateTime, IDictionary<string, string> data, bool isClickable)
        {
            ScheduledOption options = ScheduleConfig(id, title, body, isClickable, false, dateTime);

            this.scheduledAlarmManager.AddScheduleAlarm(options, data);
        }
        public void RemoveSchedule(int id)
        {
            this.scheduledAlarmManager.RemoveScheduleAlarm(id);
        }
        public async Task<NotificationResult> Notify(INotificationOptions options)
        {
            return await Task.Run(() =>
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                    return this.uNNotificationManager.Notify(options);
                else
                {
                    NotificationResult result = null;
                    ManualResetEvent reset = new ManualResetEvent(false);
                    UIApplication.SharedApplication.InvokeOnMainThread(
                        () => {
                            result = this.localNotificationManager.Notify(options);
                            reset.Set();
                        });
                    reset.WaitOne();
                    return result;
                }
            });
        }
        public NotificationOptions NotificationConfig(string title, string body, bool isClickable, bool isCallback, IDictionary<string, string> data,string avatar, DateTime? notifyTime = null)
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
                iOSOptions = this.iOSOptions
            };
        }

        private ScheduledOption ScheduleConfig(int id, string title, string body, bool isClickable, bool isCallback, DateTime notifyTime)
        {
            return new ScheduledOption()
            {
                ScheduleID = id,
                Title = title,
                Body = body,
                DelayUntil = notifyTime,
                IsClickable = isClickable
            };
        }
    }
}