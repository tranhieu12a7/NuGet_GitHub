using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotifyLocal.Plugin.Abstractions
{
    public delegate void PushNotificationResponseEventHandler(object source, PushNotificationResponseEventArgs e);

    public class PushNotificationResponseEventArgs : EventArgs
    {
        public string Identifier { get; }

        public IDictionary<string, object> Data { get; }

        public NotificationResponseType Type { get; }

        public PushNotificationResponseEventArgs(IDictionary<string, object> data, string identifier = "", NotificationResponseType type = NotificationResponseType.Default)
        {
            Identifier = identifier;
            Data = data;
            Type = type;
        }

    }
}
