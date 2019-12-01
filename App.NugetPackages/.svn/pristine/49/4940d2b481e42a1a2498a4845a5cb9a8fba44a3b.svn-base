
using System.Collections.Generic;

namespace PushNotifyLocal.Plugin.Abstractions
{
    public class NotificationResponse
    {
        public string Identifier { get; }
        public IDictionary<string, string> Data { get; }

        public NotificationResponseType Type { get; }

        public NotificationResponse(IDictionary<string, string> data, string identifier = "", NotificationResponseType type = NotificationResponseType.Default)
        {
            Identifier = identifier;
            Data = data;
            Type = type;
        }
    }
    public enum NotificationResponseType
    {
        Default,
        Custom,
        Dismiss
    }
}
