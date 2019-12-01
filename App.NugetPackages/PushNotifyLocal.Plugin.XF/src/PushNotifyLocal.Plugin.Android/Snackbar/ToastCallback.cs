using System;
using Android.Support.Design.Widget;
using PushNotifyLocal.Plugin.Abstractions;

namespace PushNotifyLocal.Plugin
{
    internal class ToastCallback : Snackbar.Callback
    {
        private string _id = "";
        private Action<string, NotificationResult> _callback;
        public ToastCallback(string id, Action<string, NotificationResult> callback)
        {
            _id = id;
            _callback = callback;
        }

        public override void OnDismissed(Snackbar snackbar, int evt)
        {
            switch (evt)
            {
                case DismissEventAction:
                    return;  // Handled via OnClickListeners
                case DismissEventConsecutive:
                case DismissEventManual:
                case DismissEventSwipe:
                    _callback(_id, new NotificationResult() { Action = NotificationAction.Dismissed });
                    break;
                case DismissEventTimeout:
                default:
                    _callback(_id, new NotificationResult() { Action = NotificationAction.Timeout });
                    break;
            }
        }
    }
}