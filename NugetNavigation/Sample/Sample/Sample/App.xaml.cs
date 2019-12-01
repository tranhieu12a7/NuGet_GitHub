using NugetNavigation;
using Sample.ViewModels;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new MainPage();
            ServiceLocator.Instance.RegisterViewModels();
            ServiceLocator.Instance.Build();

            initNavigationPage();



        }
        public async void initNavigationPage()
        {
            await ServiceLocator.Instance.Resolve<INavigationService>().NavigateToAsync(new List<Type>()
                 {
                   typeof(MasterDetailPageViewModel),typeof(Page1ViewModel)
                  });

        }


        protected override void OnStart()
        {
            // Handle when your app starts
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
