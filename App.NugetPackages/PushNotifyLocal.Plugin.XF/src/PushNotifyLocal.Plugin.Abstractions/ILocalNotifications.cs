using System;
using System.Collections.Generic;

namespace PushNotifyLocal.Plugin.Abstractions
{
    public interface ILocalNotifications
    {
        event EventHandler<NotificationResponse> OnNotificationOpened;
        void PushNotify(string title, string body, IDictionary<string, string> data, bool isClickable = true);
        void PushNotify(string title, string body, IDictionary<string, string> data, Action<NotificationResult> callback);
        void PushSnackbar(string title, string body, IDictionary<string, string> data,string urlAvatar=null, bool isClickable = true);
        void PushSnackbar(string title, string body, IDictionary<string, string> data, Action<NotificationResult> callback, string urlAvatar = null);
        void AddSchedule(int id, string title, string body, DateTime dateTime, IDictionary<string, string> data, bool isClickable = true);
        void RemoveSchedule(int id);
    }
}
