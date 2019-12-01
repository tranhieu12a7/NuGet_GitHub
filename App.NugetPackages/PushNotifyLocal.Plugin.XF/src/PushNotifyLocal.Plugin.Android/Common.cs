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
using Android.Views;
using Android.Widget;
using Java.Util;
using PushNotifyLocal.Plugin.Abstractions;

namespace PushNotifyLocal.Plugin
{
    internal class Common
    {
        public static bool IsOnAppp = false;
        public static Type MainActivityType;
        public const int StartId = 101;
        public const string NotificationId = "NOTIFICATION_ID";
        public const string ScheduledId = "SCHEDULED_ID";
        public const string ScheduledAlarmOption = "SCHEDULED_ALARM_OPTION";
        public const string ScheduledAlarmAction = "SCHEDULED_ALARM_ACTION";
        public const string NotificationCustomArgs = "NOTIFICATION_CUSTOM_ARGS";
        public const string DismissedClickIntent = "android.intent.action.DISMISSED";
        public const string OnClickIntent = "android.intent.action.CLICK";
        public const string NotificationCallback = "NOTIFICATION_CALLBACK";
        public const string NotificationOnClickHandler = "NOTIFICATION_ONCLICK_HANDLER";
        public const string DefaultChannelName = "default";
        public const string ActionIdentifierKey = "ACTION_IDENTIFIER_KEY";
        public static long NotifyTimeInMilliseconds(DateTime notifyTime)
        {
            var utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
            var epochDifference = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;

            var utcAlarmTimeInMillis = utcTime.AddSeconds(-epochDifference).Ticks / 10000;
            return utcAlarmTimeInMillis;
        }
        public static string SerializeScheduledOption(IScheduledOption notification)
        {
            var xmlSerializer = new XmlSerializer(notification.GetType());
            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, notification);
                return stringWriter.ToString();
            }
        }
        public static ScheduledOption DeserializeScheduledOption(string notificationString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ScheduledOption));
            using (var stringReader = new StringReader(notificationString))
            {
                var notification = (ScheduledOption)xmlSerializer.Deserialize(stringReader);
                return notification;
            }
        }
        public static string SerializeDictionary(IDictionary<string,string> notification)
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
        public static long ConvertToMilliseconds(DateTime localAlarmTime)
        {
            // Convert the alarm time to UTC
            var utcAlarmTime = TimeZoneInfo.ConvertTimeToUtc(localAlarmTime);

            // Work out the difference between epoch (Java) and ticks (.NET)
            var t = new DateTime(1970, 1, 1) - DateTime.MinValue;
            var epochDifferenceInSeconds = t.TotalSeconds;

            // Convert from ticks to milliseconds
            var utcAlarmTimeInMillis = utcAlarmTime.AddSeconds(-epochDifferenceInSeconds).Ticks / 10000;
            return utcAlarmTimeInMillis;
        }
       
        public static string GetOrCreateChannel(IAndroidChannelOptions channelOptions)
        {
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var channelId = (channelOptions.Name == null) ? Common.DefaultChannelName
                                    : channelOptions.Name.Replace(" ", string.Empty).ToLower();

                    if (string.IsNullOrEmpty(channelId) && channelOptions != null)
                        channelOptions.Name = Common.DefaultChannelName;

                    // Create new channel.
                    var newChannel = new NotificationChannel(channelId, channelOptions.Name, NotificationImportance.High);
                    newChannel.EnableVibration(channelOptions.EnableVibration);
                    newChannel.SetShowBadge(channelOptions.ShowBadge);
                    if (!string.IsNullOrEmpty(channelOptions.Description))
                    {
                        newChannel.Description = channelOptions.Description;
                    }

                    // Register channel.
                    var notificationManager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;
                    notificationManager.CreateNotificationChannel(newChannel);
                    return channelId;
                }
            } catch { }
            return null;
        }
    }
}