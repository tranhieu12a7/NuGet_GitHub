using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using PushNotifyLocal.Plugin.Abstractions;

namespace PushNotifyLocal.Plugin
{
    internal class OnClickListener : Java.Lang.Object, View.IOnClickListener
    {
        private Action<NotificationResponse> _callback;
        private NotificationResponse _result;
        private Snackbar _snackbar;
        public OnClickListener(Snackbar snackbar, Action<NotificationResponse> callback, NotificationResponse result)
        {
            _snackbar = snackbar;
            _callback = callback;
            _result = result;
        }
        public void OnClick(View v)
        {
            _snackbar.Dismiss();
            _callback(_result);
        }
    }
}