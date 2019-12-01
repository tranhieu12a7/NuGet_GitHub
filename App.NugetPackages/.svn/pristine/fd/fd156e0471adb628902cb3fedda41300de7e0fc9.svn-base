using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Views;
using Android.Widget;
using PushNotifyLocal.Plugin.Abstractions;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Graphics;
using Android;
using Android.Content;
using Android.Telephony;
using Android.Content.Res;
using System.Net;
using Android.Views.Animations;
using System;

namespace PushNotifyLocal.Plugin
{
    internal class SnackbarManager
    {
        private IDictionary<string, ManualResetEvent> _resetEvents = new ConcurrentDictionary<string, ManualResetEvent>();
        private IDictionary<string, NotificationResult> _eventResult = new ConcurrentDictionary<string, NotificationResult>();
        private IList<Snackbar> _snackBars = new List<Snackbar>();

        private int _count = 0;
        private object _lock = new object();
        public NotificationResult Notify(Activity activity, NotificationOptions notificationOptions)
        {
            var id = _count.ToString();
            _count++;

            var snackbar = SnackbarCustom(activity, notificationOptions, id);

            // Setup reset events
            var resetEvent = new ManualResetEvent(false);
            _resetEvents.Add(id, resetEvent);
            _snackBars.Add(snackbar);
            snackbar.Show();

            resetEvent.WaitOne(); // Wait for a result

            var notificationResult = _eventResult[id];

            _eventResult.Remove(id);
            _resetEvents.Remove(id);

            if (_snackBars.Contains(snackbar))
                _snackBars.Remove(snackbar);

            return notificationResult;
        }

        private Snackbar SnackbarCustom(Activity activity, INotificationOptions notificationOptions, string id)
        {
            var view = activity.FindViewById(Android.Resource.Id.Content);

            string text = string.Empty;
            text += notificationOptions.Title;
            if (!string.IsNullOrEmpty(notificationOptions.Title) && !string.IsNullOrEmpty(notificationOptions.Body))
                text += "\n";

            text += notificationOptions.Body;

            var snackbar = Snackbar.Make(view, string.Empty, Snackbar.LengthLong);

            Color colorBackground = Color.White;
            if (!string.IsNullOrEmpty(notificationOptions.AndroidOptions.HexColorBackground))
                colorBackground = Color.ParseColor(notificationOptions.AndroidOptions.HexColorBackground);

            Color colorText = Color.Black;
            if (!string.IsNullOrEmpty(notificationOptions.AndroidOptions.HexColorText))
                colorText = Color.ParseColor(notificationOptions.AndroidOptions.HexColorText);

            //inflate view
            LayoutInflater inflater = (LayoutInflater)Application.Context.GetSystemService(Android.Content.Context.LayoutInflaterService);
            View snackView = inflater.Inflate(Resource.Layout.snackbar_view_avatar, null, false);

            snackView.SetPadding(0, 0, 0, 0);
            Snackbar.SnackbarLayout snackBarView = (Snackbar.SnackbarLayout)snackbar.View;
            FrameLayout.LayoutParams parentParams = (FrameLayout.LayoutParams)snackBarView.LayoutParameters;
            parentParams.Height = FrameLayout.LayoutParams.WrapContent;
            parentParams.Gravity = GravityFlags.Start;
            parentParams.Width = FrameLayout.LayoutParams.MatchParent;
            parentParams.SetMargins(-22, notificationOptions.AndroidOptions.TopMargin, -22, 0);
            snackBarView.LayoutParameters = parentParams;
            snackBarView.SetBackgroundColor(Color.Transparent);
            snackBarView.AddView(snackView, 0);

            var animation = new TranslateAnimation(-100, 0, 0, 0);
            animation.Duration = 500;
            animation.FillBefore = true;
            snackBarView.StartAnimation(animation);

            ImageView mainImageView = snackView.FindViewById<ImageView>(Resource.Id.imgavatar);
            var imageBitmap = GetImageBitmapFromUrl(notificationOptions.Avatar);
            if (imageBitmap != null)
            {
                mainImageView.SetImageBitmap(imageBitmap);
                //mainImageView.SetBorderWidth(0);
                mainImageView.Visibility = ViewStates.Visible;
            }
            else
                mainImageView.Visibility = ViewStates.Gone;

            TextView mainTextView = snackView.FindViewById<TextView>(Resource.Id.textviewTitle);
            mainTextView.SetTextColor(Color.Black);
            mainTextView.SetTextSize(Android.Util.ComplexUnitType.Sp, 16);
            mainTextView.Text = notificationOptions.Title;

            TextView mainTextView_body = snackView.FindViewById<TextView>(Resource.Id.textviewBody);
            mainTextView_body.SetTextColor(Color.Black);
            mainTextView_body.SetTextSize(Android.Util.ComplexUnitType.Sp, 16);
            mainTextView_body.Text = notificationOptions.Body;

            if (notificationOptions.IsCallback)
            {
                snackBarView.SetOnClickListener(new CallbackOnClickListener(snackbar,id, (toastId, result) =>
                {
                    ToastClosed(toastId, result);
                }, new NotificationResult()
                {
                    Action = NotificationAction.Clicked
                }));
            }
            else if (notificationOptions.IsClickable)
            {
                snackBarView.SetOnClickListener(new OnClickListener(snackbar, result =>
                {
                    (CrossLocalNotifications.Current as LocalNotifications)._onNotificationOpened?.Invoke(this, result);
                    ToastClosed(id, new NotificationResult() { Action = NotificationAction.Dismissed });
                }, new NotificationResponse(notificationOptions.CustomArgs)));
            }

            _ = snackbar.SetCallback(new ToastCallback(id, (toastId, result) => {
                ToastClosed(toastId, result);
            }));

            return snackbar;
        }

        public void CancelAll()
        {
            foreach (var snackbar in _snackBars)
                snackbar.Dismiss();
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
        private Bitmap GetImageBitmapFromUrl(string url)
        {
            if (url == null || url == "")
                return null;
            Bitmap imageBitmap = null;
            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }
            return imageBitmap;
        }
    }
}