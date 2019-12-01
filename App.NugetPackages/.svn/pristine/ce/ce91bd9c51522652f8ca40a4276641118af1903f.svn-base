using System;
using System.Collections.Concurrent;
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
    internal class SnackbarManager
    {
        private IDictionary<string, ManualResetEvent> _resetEvents = new ConcurrentDictionary<string, ManualResetEvent>();
        private IDictionary<string, NotificationResult> _eventResult = new ConcurrentDictionary<string, NotificationResult>();
        private object _lock = new object();
        private IList<TTGSnackbar> _snackBars = new List<TTGSnackbar>();
        private int _count = 0;

        public async Task<NotificationResult> Notify(INotificationOptions options)
        {
            var id = _count.ToString();
            _count++;
            var snackbar = new TTGSnackbar(options.Title, options.Body, options.Avatar);

            snackbar.AnimationType = TTGSnackbarAnimationType.FadeInFadeOut;

            snackbar.BackgroundColor = UIColor.Clear.FromHexString(options.iOSOptions.HexColorBackground);
            //snackbar.SeperateView.Alpha = 0;
            snackbar.MessageTextColor = UIColor.Clear.FromHexString(options.iOSOptions.HexColorText);
            snackbar.TintColor = UIColor.Clear.FromHexString(options.iOSOptions.HexColorText);
            //if (options.IsClickable)
            //{
            //    // Action 1
            //    //snackbar.ActionText = options.iOSOptions.ViewText;
            //    //snackbar.ActionTextColor = UIColor.FromRGB(236, 64, 122);
            //    snackbar.ActionBlock = (t) =>
            //    {
            //        ToastClosed(id, new NotificationResult { Action = NotificationAction.Clicked });
            //    };
            //}
            if (options.IsCallback)
            {
                snackbar.ActionBlock = (t) =>
                {
                    ToastClosed(id, new NotificationResult { Action = NotificationAction.Clicked });
                };
            }
            else if (options.IsClickable)
            {
                snackbar.ActionBlock = (t) =>
                {
                    (CrossLocalNotifications.Current as LocalNotifications)._onNotificationOpened?.Invoke(this, new NotificationResponse(options.CustomArgs));
                    ToastClosed(id, new NotificationResult() { Action = NotificationAction.Dismissed });
                };
            }
            else
            {
                snackbar.ActionBlock = (t) =>
                {
                    ToastClosed(id, new NotificationResult { Action = NotificationAction.Dismissed });
                };
            }
            // Dismiss Callback
            // Dismiss Callback
            snackbar.DismissBlock = (t) => {
                ToastClosed(id, new NotificationResult { Action = NotificationAction.Dismissed });
            };

            //snackbar.Icon = UIImage.FromBundle("EmojiCool");
            snackbar.LocationType = TTGSnackbarLocation.Top;
            snackbar.Show();
            return await Task.Run(() =>
            {
                // Setup reset events
                var resetEvent = new ManualResetEvent(false);
                _resetEvents.Add(id, resetEvent);
                _snackBars.Add(snackbar);

                resetEvent.WaitOne(); // Wait for a result

                var notificationResult = _eventResult[id];

                _eventResult.Remove(id);
                _resetEvents.Remove(id);

                if (_snackBars.Contains(snackbar))
                    _snackBars.Remove(snackbar);

                return notificationResult;
            });
        }
        
        private void OnActionText(TTGSnackbar obj)
        {
            throw new NotImplementedException();
        }

        private void ToastClosed(string id, NotificationResult result)
        {
            lock (_lock)
            {
                if (_resetEvents.ContainsKey(id))
                {
                    _eventResult.Add(id, result);
                    _resetEvents[id].Set();
                }
            }
        }
    }

    public static class UIColorExtensions
    {
        public static UIColor FromHexString(this UIColor color, string hexValue, float alpha = 1.0f)
        {
            var colorString = hexValue.Replace("#", "");
            if (alpha > 1.0f)
            {
                alpha = 1.0f;
            }
            else if (alpha < 0.0f)
            {
                alpha = 0.0f;
            }

            float red, green, blue;

            switch (colorString.Length)
            {
                case 3: // #RGB
                    {
                        red = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(0, 1)), 16) / 255f;
                        green = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(1, 1)), 16) / 255f;
                        blue = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(2, 1)), 16) / 255f;
                        return UIColor.FromRGBA(red, green, blue, alpha);
                    }
                case 6: // #RRGGBB
                    {
                        red = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
                        green = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
                        blue = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
                        return UIColor.FromRGBA(red, green, blue, alpha);
                    }

                default:
                    throw new ArgumentOutOfRangeException(string.Format("Invalid color value {0} is invalid. It should be a hex value of the form #RBG, #RRGGBB", hexValue));

            }
        }
    }

}