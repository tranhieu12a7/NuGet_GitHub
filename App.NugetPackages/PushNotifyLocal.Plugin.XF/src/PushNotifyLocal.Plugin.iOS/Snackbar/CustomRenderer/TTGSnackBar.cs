using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace PushNotifyLocal.Plugin
{
    public class TTGSnackbar : UIView
    {
        /// Snackbar action button max width.
        private const float snackbarActionButtonMaxWidth = 64;

        // Snackbar action button min width.
        private const float snackbarActionButtonMinWidth = 44;

        // Snackbar icon imageView default width
        private const float snackbarIconImageViewWidth = 32;

        private NSLayoutConstraint[] hConstraints;
        private NSLayoutConstraint[] vConstraints;

        // Action callback.
        public Action<TTGSnackbar> ActionBlock { get; set; }

        // Dismiss block
        public Action<TTGSnackbar> DismissBlock { get; set; }

        // Snackbar display duration. Default is Short - 1 second.
        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(3);

        // Snackbar animation type. Default is SlideFromBottomBackToBottom.
        public TTGSnackbarAnimationType AnimationType = TTGSnackbarAnimationType.SlideFromLeftToRight;

        // Snackbar location
        public TTGSnackbarLocation LocationType = TTGSnackbarLocation.Bottom;

        // Show and hide animation duration. Default is 0.3
        public float AnimationDuration = 0.1f;

        private float _cornerRadius = 4;
        public float CornerRadius
        {
            get { return _cornerRadius; }
            set
            {
                _cornerRadius = value;
                if (_cornerRadius > Height)
                {
                    _cornerRadius = Height / 2;
                }
                if (_cornerRadius < 0)
                    _cornerRadius = 0;

                this.Layer.CornerRadius = _cornerRadius;
                this.Layer.MasksToBounds = true;
            }
        }

        /// Top margin. Default is 4
        private float _topMargin = 48;
        public float TopMargin
        {
            get { return _topMargin; }
            set
            {
                _topMargin = value; if (topMarginConstraint != null) { topMarginConstraint.Constant = _topMargin; this.LayoutIfNeeded(); }
            }
        }

        /// Left margin. Default is 4
        private float _leftMargin = 4;
        public float LeftMargin
        {
            get { return _leftMargin; }
            set { _leftMargin = value; if (leftMarginConstraint != null) { leftMarginConstraint.Constant = _leftMargin; this.LayoutIfNeeded(); } }
        }

        private float _rightMargin = 4;
        public float RightMargin
        {
            get { return _rightMargin; }
            set { _rightMargin = value; if (rightMarginConstraint != null) { rightMarginConstraint.Constant = _leftMargin; this.LayoutIfNeeded(); } }
        }

        /// Bottom margin. Default is 4
        private float _bottomMargin = 4;
        public float BottomMargin
        {
            get { return _bottomMargin; }
            set { _bottomMargin = value; if (bottomMarginConstraint != null) { bottomMarginConstraint.Constant = _bottomMargin; this.LayoutIfNeeded(); } }
        }

        private float _height = 58;
        public float Height
        {
            get { return _height; }
            set { _height = value; if (heightConstraint != null) { heightConstraint.Constant = _height; this.LayoutIfNeeded(); } }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; if (this.MessageLabel != null) { this.MessageLabel.Text = _message; } }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; if (this.MessageLabel != null) { this.MessageLabel.Text = _title; } }
        }

        private UIColor _messageTextColor = UIColor.White;
        public UIColor MessageTextColor
        {
            get { return _messageTextColor; }
            set { _messageTextColor = value; this.MessageLabel.TextColor = _messageTextColor; }
        }

        private UIFont _messageTextFont = UIFont.SystemFontOfSize(14);
        public UIFont MessageTextFont
        {
            get { return _messageTextFont; }
            set { _messageTextFont = value; this.MessageLabel.Font = _messageTextFont; }
        }

        private UITextAlignment _messageTextAlign;
        public UITextAlignment MessageTextAlign
        {
            get { return _messageTextAlign; }
            set { _messageTextAlign = value; this.MessageLabel.TextAlignment = _messageTextAlign; }
        }

        private nfloat _messageMarginLeft = 2;
        public nfloat MessageMarginLeft
        {
            get { return _messageMarginLeft; }
            set { _messageMarginLeft = value; }
        }

        private nfloat _messageMarginRight = 2;
        public nfloat MessageMarginRight
        {
            get { return _messageMarginRight; }
            set { _messageMarginRight = value; }
        }

        private string _actionText;
        public string ActionText
        {
            get { return _actionText; }
            set
            {
                _actionText = value;
                if (this.ActionButton != null)
                {
                    this.ActionButton.SetTitle(_actionText, UIControlState.Normal);
                    this.ActionButton.Hidden = string.IsNullOrEmpty(value);
                }
            }
        }

        // Action button title color. Default is white.
        private UIColor _actionTextColor = UIColor.Black;
        public UIColor ActionTextColor
        {
            get { return _actionTextColor; }
            set { _actionTextColor = value; this.ActionButton.SetTitleColor(_actionTextColor, UIControlState.Normal); }
        }

        // First action text font. Default is Bold system font (16).
        private UIFont _actionTextFont = UIFont.BoldSystemFontOfSize(16);
        public UIFont ActionTextFont
        {
            get { return _actionTextFont; }
            set { _actionTextFont = value; this.ActionButton.TitleLabel.Font = _actionTextFont; }
        }

        private UIImage _icon;
        public UIImage Icon
        {
            get { return _icon; }
            set
            {
                _icon = value;
                IconImageView.Image = _icon;
            }
        }

        private UIViewContentMode _iconContentMode = UIViewContentMode.Center;
        public UIViewContentMode IconContentMode
        {
            get { return _iconContentMode; }
            set
            {
                _iconContentMode = value;
                IconImageView.ContentMode = _iconContentMode;
            }
        }

        public UIImageView IconImageView;
        public UILabel TitleLabel;
        public UILabel MessageLabel;
        public UIView BodyView;
        public UIView SeperateView;
        public UIButton ActionButton;
        public UIActivityIndicatorView ActivityIndicatorView;

        // Timer to dismiss the snackbar.
        private NSTimer dismissTimer;

        // Constraints.
        private NSLayoutConstraint heightConstraint;
        private NSLayoutConstraint leftMarginConstraint;
        private NSLayoutConstraint rightMarginConstraint;
        private NSLayoutConstraint bottomMarginConstraint;
        private NSLayoutConstraint topMarginConstraint;
        private NSLayoutConstraint actionButtonWidthConstraint;
        private NSLayoutConstraint iconImageViewWidthConstraint;

        /// <summary>
        /// Show a single message like an Android snackbar.
        /// - parameter message:  Message text.
        /// </summary>
        public TTGSnackbar(string title, string message, string urlImage) : base(CoreGraphics.CGRect.FromLTRB(0, 0, 320, 44))
        {
            this.Title = title;
            this.Message = message;
            
            configure(urlImage);
        }

        /// <summary>
        /// Show a message with action button.
        /// - parameter message:     Message text.
        /// - parameter actionText:  Action button title.
        /// - parameter actionBlock: Action callback closure.
        /// - returns: Void
        /// </summary>
        public TTGSnackbar(string title, string message, string urlImage, string actionText, Action<TTGSnackbar> ttgAction) : base(CoreGraphics.CGRect.FromLTRB(0, 0, 320, 44))
        {
            this.Title = title;
            this.Message = message;
            this.ActionText = actionText;
            this.ActionBlock = ttgAction;

            configure(urlImage);
        }

        /// <summary>
        /// Show a custom message with action button.
        /// - parameter message:          Message text.
        /// - parameter actionText:       Action button title.
        /// - parameter messageFont:      Message label font.
        /// - parameter actionButtonFont: Action button font.
        /// - parameter actionBlock:      Action callback closure.
        /// - returns: Void
        /// </summary>
        public TTGSnackbar(string title, string message,string urlImage, string actionText, UIFont messageFont, UIFont actionTextFont, Action<TTGSnackbar> ttgAction) : base(CoreGraphics.CGRect.FromLTRB(0, 0, 320, 44))
        {
            this.Title = title;
            this.Message = message;
            this.ActionText = actionText;
            this.ActionBlock = ttgAction;
            this.MessageTextFont = messageFont;
            this.ActionTextFont = actionTextFont;

            configure(urlImage);
        }


        /// <summary>
        /// Show the snackbar.
        /// </summary>
        public void Show()
        {
            // Only show once
            if (this.Superview != null)
                return;
          
            // Create dismiss timer
            dismissTimer = NSTimer.CreateScheduledTimer(Duration, (t) => Dismiss());

            if(iconImageViewWidthConstraint != null)
            {
                iconImageViewWidthConstraint.Constant = TTGSnackbar.snackbarIconImageViewWidth;
            }

            this.LayoutIfNeeded();
            
            var localSuperView = UIApplication.SharedApplication.KeyWindow;
            if (localSuperView != null)
            {
                NSObject layoutGuide = localSuperView;

                if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
                {
                    layoutGuide = localSuperView.SafeAreaLayoutGuide;
                }

                localSuperView.AddSubview(this);

                topMarginConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Top,
                    NSLayoutRelation.Equal,
                    layoutGuide,
                    NSLayoutAttribute.Top,
                    1,
                    TopMargin);

                heightConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Height,
                    NSLayoutRelation.GreaterThanOrEqual,
                    null,
                    NSLayoutAttribute.NoAttribute,
                    1,
                    Height);

                leftMarginConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Left,
                    NSLayoutRelation.Equal,
                    layoutGuide,
                    NSLayoutAttribute.Left,
                    1,
                    LeftMargin);

                rightMarginConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Right,
                    NSLayoutRelation.Equal,
                    layoutGuide,
                    NSLayoutAttribute.Right,
                    1,
                    -RightMargin);

                bottomMarginConstraint = NSLayoutConstraint.Create(
                    this,
                    NSLayoutAttribute.Bottom,
                    NSLayoutRelation.Equal,
                    layoutGuide,
                    NSLayoutAttribute.Bottom,
                    1,
                    -BottomMargin);

                // Avoid the "UIView-Encapsulated-Layout-Height" constraint conflicts
                // http://stackoverflow.com/questions/25059443/what-is-nslayoutconstraint-uiview-encapsulated-layout-height-and-how-should-i
                leftMarginConstraint.Priority = 999;
                rightMarginConstraint.Priority = 999;

                this.AddConstraint(heightConstraint);
                localSuperView.AddConstraint(leftMarginConstraint);
                localSuperView.AddConstraint(rightMarginConstraint);

                switch (LocationType)
                {
                    case TTGSnackbarLocation.Top:
                        localSuperView.AddConstraint(topMarginConstraint);
                        break;
                    default:
                        localSuperView.AddConstraint(bottomMarginConstraint);
                        break;
                }

                // Show
                //showWithAnimation();
            }
            else
            {
                Console.WriteLine("TTGSnackbar needs a keyWindows to display.");
            }
        }

        /// <summary>
        /// Dismiss the snackbar manually..
        /// </summary>
        public void Dismiss()
        {
            this.dismissAnimated(true);
        }

        /// <summary>
        /// Configure this instance.
        /// </summary>
        private async void configure(string urlImg)
        {
            bool isShowImg = false;
            this.TranslatesAutoresizingMaskIntoConstraints = false;
            this.BackgroundColor = UIColor.White;
            this.Layer.CornerRadius = CornerRadius;
            this.Layer.MasksToBounds = true;

            if (!string.IsNullOrEmpty(urlImg))
            {
                NSUrl imageURL = new NSUrl(urlImg);
                var imgData = NSData.FromUrl(imageURL);
                //hinh link online khong lay duoc
                if(imgData != null)
                {
                    var Iconimage = UIImage.LoadFromData(imgData);

                    IconImageView = new UIImageView(Iconimage);
                    IconImageView.TranslatesAutoresizingMaskIntoConstraints = true;
                    IconImageView.BackgroundColor = UIColor.Clear;
                    IconImageView.ContentMode = UIViewContentMode.ScaleToFill;
                 
                    IconImageView.Frame = new CGRect(5, 5, 45, 45);

                    IconImageView.Layer.BorderWidth = new nfloat(1.0);
                    IconImageView.Layer.MasksToBounds = false;
                    IconImageView.Layer.BorderColor = UIColor.White.CGColor;
                    IconImageView.Layer.CornerRadius = IconImageView.Frame.Size.Width / 2;
                    IconImageView.ClipsToBounds = true;

                    this.AddSubview(IconImageView);

                    var vConstraintsForIconImageView = NSLayoutConstraint.FromVisualFormat(
                    "V:|[iconImageView]", NSLayoutFormatOptions.DirectionLeadingToTrailing
                    , new NSDictionary(), NSDictionary.FromObjectsAndKeys(new NSObject[] { IconImageView }, new NSObject[] { new NSString("iconImageView") })
                        );
                    iconImageViewWidthConstraint = NSLayoutConstraint.Create(IconImageView, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, TTGSnackbar.snackbarIconImageViewWidth);

                    this.AddConstraints(vConstraintsForIconImageView);
                    IconImageView.AddConstraint(iconImageViewWidthConstraint);

                    isShowImg = true;
                }
            }

            BodyView = new UIView();
            BodyView.TranslatesAutoresizingMaskIntoConstraints = false;
            BodyView.BackgroundColor = UIColor.Clear;

            TitleLabel = new UILabel();
            TitleLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            TitleLabel.TextColor = UIColor.Black;
            TitleLabel.Font = UIFont.BoldSystemFontOfSize(16);
            TitleLabel.BackgroundColor = UIColor.Clear;
            TitleLabel.LineBreakMode = UILineBreakMode.CharacterWrap;
            TitleLabel.Lines = 1;
            TitleLabel.TextAlignment = UITextAlignment.Left;
            TitleLabel.Text = Title;

            MessageLabel = new UILabel();
            MessageLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            MessageLabel.TextColor = UIColor.Black;
            MessageLabel.Font = UIFont.BoldSystemFontOfSize(16);
            MessageLabel.BackgroundColor = UIColor.Clear;
            MessageLabel.LineBreakMode = UILineBreakMode.CharacterWrap;
            MessageLabel.Lines = 2;
            MessageLabel.TextAlignment = UITextAlignment.Left;
            MessageLabel.Text = Message;

            BodyView.AddSubview(TitleLabel);
            BodyView.AddSubview(MessageLabel);
            
            BodyView.UserInteractionEnabled = true;
            UITapGestureRecognizer tapGesture = new UITapGestureRecognizer(TapGesture);
            BodyView.AddGestureRecognizer(tapGesture);

            invalidateVerticalConstraints_Body();

            this.AddSubview(BodyView);

            // Add constraints
            

            var hConstraintsForTitleLabel = NSLayoutConstraint.FromVisualFormat(
                "H:|[titleLabel]|", NSLayoutFormatOptions.DirectionLeadingToTrailing
                , new NSDictionary(), NSDictionary.FromObjectsAndKeys(new NSObject[] { TitleLabel }, new NSObject[] { new NSString("titleLabel") })
            );
            var hConstraintsForMessageLabel = NSLayoutConstraint.FromVisualFormat(
             "H:|[messageLabel]|", NSLayoutFormatOptions.DirectionLeadingToTrailing
             , new NSDictionary(), NSDictionary.FromObjectsAndKeys(new NSObject[] { MessageLabel }, new NSObject[] { new NSString("messageLabel") })
            );

            var vConstraintsForBodyView = NSLayoutConstraint.FromVisualFormat(
                "V:|[bodyView]|", NSLayoutFormatOptions.DirectionLeadingToTrailing
                , new NSDictionary(), NSDictionary.FromObjectsAndKeys(new NSObject[] { BodyView }, new NSObject[] { new NSString("bodyView") })
            );

            BodyView.AddConstraints(vConstraints);
            BodyView.AddConstraints(hConstraintsForMessageLabel);
            BodyView.AddConstraints(hConstraintsForTitleLabel);

            

            invalidateHorizontalConstraints_MainView(isShowImg);
            
            
            this.AddConstraints(vConstraintsForBodyView);
            this.AddConstraints(hConstraints);
        }

        /// <summary>
        /// Invalid the dismiss timer.
        /// </summary>
        private void invalidDismissTimer()
        {
            if (dismissTimer != null)
            {
                dismissTimer.Invalidate();
                dismissTimer = null;
            }
        }

        private void invalidateVerticalConstraints_Body()
        {
            if (vConstraints != null && vConstraints.Length > 0)
                this.RemoveConstraints(vConstraints);

            vConstraints = NSLayoutConstraint.FromVisualFormat(
                $"V:|[titleLabel][messageLabel]-|",
                NSLayoutFormatOptions.DirectionLeadingToTrailing, new NSDictionary(),
                NSDictionary.FromObjectsAndKeys(
                    new NSObject[] {
                        TitleLabel,
                        MessageLabel,
                }, new NSObject[] {
                    new NSString("titleLabel"),
                    new NSString("messageLabel"),
                })
            );
            //BodyView.AddConstraints(vConstraints);
        }

        private void invalidateHorizontalConstraints_MainView(bool isShowImg)
        {
            if (hConstraints != null && hConstraints.Length > 0)
                this.RemoveConstraints(hConstraints);

            if(isShowImg)
            {
                hConstraints = NSLayoutConstraint.FromVisualFormat(
                    $"H:|[iconImageView]-[bodyView]|",
                    NSLayoutFormatOptions.DirectionLeadingToTrailing, new NSDictionary(),
                    NSDictionary.FromObjectsAndKeys(
                        new NSObject[] {
                            IconImageView,
                            BodyView
                    }, new NSObject[] {
                        new NSString("iconImageView"),
                        new NSString("bodyView")
                    })
                );
            }
            else
            {
                hConstraints = NSLayoutConstraint.FromVisualFormat(
                    $"H:|-10-[bodyView]|",
                    NSLayoutFormatOptions.DirectionLeadingToTrailing, new NSDictionary(),
                    NSDictionary.FromObjectsAndKeys(
                        new NSObject[] {
                            BodyView
                    }, new NSObject[] {
                        new NSString("bodyView")
                    })
                );
            }
            
            //this.AddConstraints(hConstraints);
        }

        /// <summary>
        /// If dismiss with animation.
        /// </summary>
        private void dismissAnimated(bool animated)
        {
            invalidDismissTimer();

            //ActivityIndicatorView.StopAnimating();

            nfloat superViewWidth = 0;

            if (Superview != null)
                superViewWidth = Superview.Frame.Width;

            if (!animated)
            {
                DismissAndPerformAction();
                return;
            }

            Action animationBlock = () => { };

            switch (AnimationType)
            {
                case TTGSnackbarAnimationType.FadeInFadeOut:
                    animationBlock = () => { this.Alpha = 0; };
                    break;
                case TTGSnackbarAnimationType.SlideFromBottomBackToBottom:
                    animationBlock = () => { bottomMarginConstraint.Constant = Height; };
                    break;
                case TTGSnackbarAnimationType.SlideFromBottomToTop:
                    animationBlock = () => { this.Alpha = 0; bottomMarginConstraint.Constant = -Height - BottomMargin; };
                    break;
                case TTGSnackbarAnimationType.SlideFromLeftToRight:
                    animationBlock = () => { leftMarginConstraint.Constant = LeftMargin + superViewWidth; rightMarginConstraint.Constant = -RightMargin + superViewWidth; };
                    break;
                case TTGSnackbarAnimationType.SlideFromRightToLeft:
                    animationBlock = () =>
                    {
                        leftMarginConstraint.Constant = LeftMargin - superViewWidth;
                        rightMarginConstraint.Constant = -RightMargin - superViewWidth;
                    };
                    break;
            };

            this.SetNeedsLayout();

            UIView.Animate(AnimationDuration, 0, UIViewAnimationOptions.CurveEaseIn, animationBlock, DismissAndPerformAction);
        }

        void TapGesture(UITapGestureRecognizer tap)
        {
            if (tap.NumberOfTapsRequired == 1)
            {
                ActionBlock(this);
                //dismissAnimated(true);
            }
        }
        void DismissAndPerformAction()
        {
            if (DismissBlock != null)
            {
                DismissBlock(this);
            }

            this.RemoveFromSuperview();
        }

        /// <summary>
        /// Shows with animation.
        /// </summary>
        private void showWithAnimation()
        {
            Action animationBlock = () => { this.LayoutIfNeeded(); };
            var superViewWidth = Superview.Frame.Width;

            switch (AnimationType)
            {
                case TTGSnackbarAnimationType.FadeInFadeOut:
                    this.Alpha = 0;
                    this.SetNeedsLayout();

                    animationBlock = () => { this.Alpha = 1; };
                    break;
                case TTGSnackbarAnimationType.SlideFromBottomBackToBottom:
                case TTGSnackbarAnimationType.SlideFromBottomToTop:
                    bottomMarginConstraint.Constant = -BottomMargin;
                    this.LayoutIfNeeded();
                    break;
                case TTGSnackbarAnimationType.SlideFromLeftToRight:
                    leftMarginConstraint.Constant = LeftMargin - superViewWidth;
                    rightMarginConstraint.Constant = -RightMargin - superViewWidth;
                    bottomMarginConstraint.Constant = -BottomMargin;
                    this.LayoutIfNeeded();
                    break;
                case TTGSnackbarAnimationType.SlideFromRightToLeft:
                    leftMarginConstraint.Constant = LeftMargin + superViewWidth;
                    rightMarginConstraint.Constant = -RightMargin + superViewWidth;
                    bottomMarginConstraint.Constant = -BottomMargin;
                    this.LayoutIfNeeded();
                    break;
            };

            // Final state
            bottomMarginConstraint.Constant = -BottomMargin;
            leftMarginConstraint.Constant = LeftMargin;
            rightMarginConstraint.Constant = -RightMargin;
            topMarginConstraint.Constant = TopMargin;

            UIView.AnimateNotify(
                    AnimationDuration,
                    0,
                    0.7f,
                    5f,
                    UIViewAnimationOptions.CurveEaseInOut,
                      animationBlock,
                    null
                );
        }

        private void doAction(UIButton button)
        {
            // Call action block first
            if (button == ActionButton)
            {
                ActionBlock(this);
            }

            dismissAnimated(true);
        }
    }
}
