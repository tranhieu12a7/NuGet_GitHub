using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using ProgressBarCustom.Control;
using ProgressBarCustom.Control.Android;
using System;
using System.ComponentModel;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.ExportRenderer(typeof(CircleProgressBarControl), typeof(CircleProgressBarImplementation))]
namespace ProgressBarCustom.Control.Android
{
    public class CircleProgressBarImplementation : ViewRenderer<CircleProgressBarControl, View>
    {
        Activity activity;
        View view;
        ProgressBar circularbar;
        ImageView imageView;
        CircleProgressBarControl element;

        public CircleProgressBarImplementation(Context context) : base(context)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<CircleProgressBarControl> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe
                //CircleProgressBar.Click -= OnCircleProgressBarClicked;
            }
            if (e.NewElement != null)
            {
                element = (CircleProgressBarControl)this.Element;
                if (Control == null)
                {
                    SetupUserInterface();
                    SetNativeControl(view);
                }
                // Subscribe
                //CircleProgressBar.Click += OnCircleProgressBarClicked;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (element == null)
                return;

            if (e.PropertyName == CircleProgressBarControl.ProgressCunrentProperty.PropertyName)
            {
                circularbar.Progress = element.ProgressCunrent;
            }
            if (e.PropertyName == CircleProgressBarControl.IsDownloadingProperty.PropertyName)
            {   
                if (element.IsDownloading)
                {
                    circularbar.SecondaryProgress = 100;
                    imageView.SetImageResource(Resource.Drawable.cancel_blue_32dp);
                }
                else
                {
                    circularbar.Progress = 0;
                    circularbar.SecondaryProgress = 0;
                    imageView.SetImageResource(Resource.Drawable.download_file);
                }
            }

        }
        void SetupUserInterface()
        {
            activity = this.Context as Activity;
            view = activity.LayoutInflater.Inflate(Resource.Layout.ProccessBarDownloadDesign, this, false);
            circularbar = view.FindViewById<ProgressBar>(Resource.Id.circularProgressbar);
            circularbar.Max = 100;
            circularbar.Progress = 0;
            circularbar.SecondaryProgress = 0;
            view.SetBackgroundColor(Color.Transparent);
            imageView = view.FindViewById<ImageView>(Resource.Id.imageDownload);
            imageView.SetImageResource(Resource.Drawable.download_file);
        }
        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);
            var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);

            view.Measure(msw, msh);
            view.Layout(0, 0, r - l, b - t);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
