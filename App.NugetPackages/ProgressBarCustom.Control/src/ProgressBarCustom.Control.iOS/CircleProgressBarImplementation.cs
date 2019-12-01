using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using ProgressBarCustom.Control;
using ProgressBarCustom.Control.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.ExportRenderer(typeof(CircleProgressBarControl), typeof(CircleProgressBarImplementation))]
namespace ProgressBarCustom.Control.iOS
{
    public class CircleProgressBarImplementation : ViewRenderer<CircleProgressBarControl, UIProgressBarCircleGraph>
    {
        UIProgressBarCircleGraph uIProgressBarCircleGraph;
        CircleProgressBarControl element;
        UIColor defaultColor = UIColor.FromRGB(189, 189, 189);
        UIColor backColor = UIColor.FromRGB(100, 181, 246);
        public static void Init()
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<CircleProgressBarControl> e)
        {
            base.OnElementChanged(e);
            element = e.NewElement as CircleProgressBarControl;
            if (element == null)
                return;

            if (Control == null)
            {
                // Instantiate the native control and assign it to the Control property with
                // the SetNativeControl method
                CGRect cGRect = new CGRect(0, 0, 80, 80);
                UIColor _frontColor = UIColor.FromRGB(0, 96, 15);
                string iconName = "download_file";
                uIProgressBarCircleGraph = new UIProgressBarCircleGraph(cGRect, 3, 0.0f, defaultColor, _frontColor, iconName);
                this.SetNativeControl(uIProgressBarCircleGraph);
            }

           
            var gesture = new UITapGestureRecognizer(someAction);

            if (e.OldElement != null)
            {
                // Unsubscribe from event handlers and cleanup any resources
                this.RemoveGestureRecognizer(gesture);
            }

            if (e.NewElement != null)
            {
                // Configure the control and subscribe to event handlers
                this.AddGestureRecognizer(gesture);
            }
        }

        private void someAction()
        {
            element.eventHandler?.Invoke(null, null);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (element == null)
                return;

            if (e.PropertyName.Equals(CircleProgressBarControl.ProgressCunrentProperty.PropertyName))
            {
                var process = (float)element.ProgressCunrent / 100.00f;
                if (process > 0)
                    uIProgressBarCircleGraph.AddProcess(process);
            }
            if (e.PropertyName.Equals(CircleProgressBarControl.IsDownloadingProperty.PropertyName))
            {
                if (element.IsDownloading)
                {
                    uIProgressBarCircleGraph.BackColor = backColor;
                    uIProgressBarCircleGraph.UpdateImageView("cancel_blue_32dp");
                }
                else
                {
                    uIProgressBarCircleGraph.PercentComplete = 0;
                    uIProgressBarCircleGraph.UpdateImageView("download_file");
                    uIProgressBarCircleGraph.BackColor = defaultColor;
                }
            }
        }
    }
    public class UIProgressBarCircleGraph : UIView
    {
        const float FULL_CIRCLE = 2 * (float)Math.PI;
        int _radius = 10;
        int _lineWidth = 10;
        public nfloat PercentComplete { get; set; }
        public UIColor BackColor { get; set; }
        public UIImageView ImageView { get; set; }

        //UIColor _defaultColor = UIColor.FromRGB(189, 189, 189); //UIColor.FromRGB(46, 60, 76);
        //UIColor _backColor = UIColor.FromRGB(100, 181, 246); //UIColor.FromRGB(46, 60, 76);
        UIColor _frontColor = UIColor.FromRGB(0, 96, 15); //UIColor.FromRGB(234, 105, 92);

        public UIProgressBarCircleGraph(CGRect frame, int lineWidth, nfloat percentComplete, UIColor backColor, UIColor frontColor, string iconName)
        {
            _lineWidth = lineWidth;
            PercentComplete = percentComplete;
            this.Frame = new CGRect(frame.X, frame.Y, frame.Width, frame.Height);
            this.BackgroundColor = UIColor.Clear;
            BackColor = backColor;
            _frontColor = frontColor;
            ImageView = new UIImageView();
            AddImageView(iconName);
        }

        public override void Draw(CoreGraphics.CGRect rect)
        {
            base.Draw(rect);

            using (CGContext g = UIGraphics.GetCurrentContext())
            {
                var diameter = Math.Min(this.Bounds.Width, this.Bounds.Height);
                _radius = (int)(diameter / 2) - _lineWidth;

                DrawGraph(g, this.Bounds.GetMidX(), this.Bounds.GetMidY());
            };
        }

        public void DrawGraph(CGContext g, nfloat x, nfloat y)
        {
            g.SetLineWidth(_lineWidth);

            // Draw background circle
            CGPath path = new CGPath();
            BackColor.SetStroke();
            path.AddArc(x, y, _radius, 0, FULL_CIRCLE, true);
            g.AddPath(path);
            g.DrawPath(CGPathDrawingMode.Stroke);

            // Draw overlay circle
            var pathStatus = new CGPath();
            _frontColor.SetStroke();

            // Same Arc params except direction so colors don't overlap
            pathStatus.AddArc(x, y, _radius, 0, PercentComplete * FULL_CIRCLE, false);
            g.AddPath(pathStatus);
            g.DrawPath(CGPathDrawingMode.Stroke);
        }

        private void AddImageView(string iconName)
        {
            var image = new UIImage(iconName);
            ImageView.Image = image;
            ImageView.Frame = new CGRect(x: 13, y: 11, width: 15, height: 15);
            this.AddSubview(ImageView);
        }

        public void UpdateImageView(string iconName)
        {
            var image = new UIImage(iconName);
            ImageView.Image = image;
            ImageView.Frame = new CGRect(x: 13, y: 11, width: 15, height: 15);
            BeginInvokeOnMainThread(this.SetNeedsDisplay);
        }
        public void AddProcess(nfloat process)
        {
            PercentComplete = process;
            BeginInvokeOnMainThread(this.SetNeedsDisplay);
        }

    }
}
