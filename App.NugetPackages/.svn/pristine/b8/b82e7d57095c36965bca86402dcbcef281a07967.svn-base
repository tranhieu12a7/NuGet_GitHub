using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Foundation;
using UIKit;

namespace PushNotifyLocal.Plugin
{
    public class Common
    {
        public static bool IsOnAppp = false;
        public const string NotificationId = "NOTIFICATION_ID";
        public const string NotificationCallback = "NOTIFICATION_CALLBACK";
        public const string NotificationOnClickHandler = "NOTIFICATION_ONCLICK_HANDLER";
        public const string NotificationCustomArgs = "NOTIFICATION_CUSTOM_ARGS";
        public static string SerializeDictionary(IDictionary<string, string> notification)
        {
            var xmlSerializer = new XmlSerializer(notification.GetType());
            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, notification);
                return stringWriter.ToString();
            }
        }
        public static IDictionary<string, string> DeserializeDictionary(string notificationString)
        {
            var xmlSerializer = new XmlSerializer(typeof(IDictionary<string, string>));
            using (var stringReader = new StringReader(notificationString))
            {
                var notification = (IDictionary<string, string>)xmlSerializer.Deserialize(stringReader);
                return notification;
            }
        }
    }
}