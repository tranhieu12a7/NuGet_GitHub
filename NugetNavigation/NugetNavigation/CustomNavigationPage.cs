using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using NavigationPage = Xamarin.Forms.NavigationPage;
using Page = Xamarin.Forms.Page;

namespace NugetNavigation
{
    public class CustomNavigationPage : NavigationPage
    {
        public static readonly BindableProperty TransitionTypeProperty =
         BindableProperty.Create("TransitionType", typeof(TransitionType), typeof(CustomNavigationPage), TransitionType.SlideFromLeft);

        public TransitionType TransitionType
        {
            get { return (TransitionType)GetValue(TransitionTypeProperty); }
            set { SetValue(TransitionTypeProperty, value); }
        }
        public CustomNavigationPage() : base()
        {
        }
        public CustomNavigationPage(Page root) : base(root)
        {
            BarTextColor = Color.White;
            SetBackButtonTitle(this, "");
            On<iOS>()
                .SetStatusBarTextColorMode(StatusBarTextColorMode.MatchNavigationBarTextLuminosity);

        }

    }
    public enum TransitionType
    {
        /// <summary>
        /// Do not animate the transition.
        /// </summary>
        None = -1,

        /// <summary>
        /// Let the OS decide how to animate the transition.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Show a fade transition animation.
        /// </summary>
        Fade = 1,

        /// <summary>
        /// Show a flip transition animation.
        /// </summary>
        Flip = 2,

        /// <summary>
        /// Show a scale transition animation.
        /// </summary>
        Scale = 3,

        /// <summary>
        /// Show a slide form left transition animation.
        /// </summary>
        SlideFromLeft = 4,

        /// <summary>
        /// Show a slide form right transition animation.
        /// </summary>
        SlideFromRight = 5,

        /// <summary>
        /// Show a slide form top transition animation.
        /// </summary>
        SlideFromTop = 6,

        /// <summary>
        /// Show a slide form bottom transition animation.
        /// </summary>
        SlideFromBottom = 7
    }

}
