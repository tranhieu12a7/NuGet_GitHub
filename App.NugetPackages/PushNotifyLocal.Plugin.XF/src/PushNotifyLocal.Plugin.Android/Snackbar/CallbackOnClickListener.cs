using Android.Support.Design.Widget;
using Android.Views;
using PushNotifyLocal.Plugin.Abstractions;
using System;

namespace PushNotifyLocal.Plugin
{
    internal class CallbackOnClickListener : Java.Lang.Object, View.IOnClickListener
    {
        private string _id = "";
        private Action<string, NotificationResult> _callback;
        private NotificationResult _result = new NotificationResult() { Action = NotificationAction.Dismissed };
        private Snackbar _snackbar;
        public CallbackOnClickListener(Snackbar snackbar, string id, Action<string, NotificationResult> callback, NotificationResult result)
        {
            _id = id;
            _callback = callback;
            _result = result;
            _snackbar = snackbar;
        }
        public void OnClick(View v)
        {
            _snackbar.Dismiss();
            _callback(_id, _result);
        }
    }
}