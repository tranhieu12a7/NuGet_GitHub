using PushNotifyLocal.Plugin;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SamplePlugin
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            CrossLocalNotifications.Current.OnNotificationOpened += (obj, data) =>
            {
                MainPage = new Page1();
                DependencyService.Get<IMessage>().LongAlert("MTL da vao");
            };
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
