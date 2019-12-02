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

          
            //ServiceLocator.Instance.assembly2 = System.Reflection.Assembly.GetExecutingAssembly();//GetType().Assembly;
            //MainPage = new MainPage();
            ServiceLocator.Instance.RegisterViewModels();
            ServiceLocator.Instance.Build();
            NavigationService.Init(ServiceLocator.Instance.Container, System.Reflection.Assembly.GetExecutingAssembly());

            initNavigationPage();



        }
        public async void initNavigationPage()
        {
            await ServiceLocator.Instance.Resolve<INavigationService>().NavigateToAsync(new List<Type>()
                 {
                //typeof(Page1ViewModel)
                   typeof(MasterDetailPageViewModel),typeof(TabbedPageViewModel),typeof(Page1ViewModel)
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
