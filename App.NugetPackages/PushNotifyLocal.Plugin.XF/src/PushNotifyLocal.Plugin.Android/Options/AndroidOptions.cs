using PushNotifyLocal.Plugin.Abstractions;

namespace PushNotifyLocal.Plugin
{
    public class AndroidOptions : IAndroidOptions
    {
        public int? SmallDrawableIcon { get; set; } = null;
        public string HexColorBackground { get; set; } = null;
        public string HexColorText { get; set; } = null;
        public int TopMargin { get; set; } = 130;
        public bool ForceOpenAppOnNotificationTap { get; set; } = false;
        public string DismissText { get; set; } = "ĐÓNG";
        public string ViewText { get; set; } = "XEM";
        public IAndroidChannelOptions ChannelOptions { get; set; } = new AndroidChannelOptions();
    }
}