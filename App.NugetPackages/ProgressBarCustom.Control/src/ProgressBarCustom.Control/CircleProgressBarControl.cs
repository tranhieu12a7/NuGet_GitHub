using System;
using Xamarin.Forms;

namespace ProgressBarCustom.Control
{
    public class CircleProgressBarControl : View
    {
        public static readonly BindableProperty ProgressCunrentProperty = BindableProperty.Create(
            propertyName: nameof(ProgressCunrent),
            returnType: typeof(int),
            declaringType: typeof(CircleProgressBarControl),
            defaultValue: default(int),
            defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty IsDownloadingProperty = BindableProperty.Create(
            propertyName: nameof(IsDownloading),
            returnType: typeof(bool),
            declaringType: typeof(CircleProgressBarControl),
            defaultValue: default(bool),
            defaultBindingMode: BindingMode.TwoWay);

        public EventHandler eventHandler;

        public int ProgressCunrent
        {
            get { return (int)GetValue(ProgressCunrentProperty); }
            set { SetValue(ProgressCunrentProperty, value); }
        }

        public bool IsDownloading
        {
            get { return (bool)GetValue(IsDownloadingProperty); }
            set { SetValue(IsDownloadingProperty, value); }
        }
    }
}
