using NugetNavigation;
using NugetNavigation.Mvvm;
using NugetNavigation.Mvvm.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Sample.ViewModels
{
    public class ViewModelBase : BindableBase
    {
        private string _title;
        private bool _isBusy;

        protected bool IsPhanTrang { get; set; }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private bool _isLoadingInit = false;
        public bool IsLoadingInit
        {
            get { return _isLoadingInit; }
            set { SetProperty(ref _isLoadingInit, value); }
        }

        private bool _isHaveData = true;
        public bool IsHaveData
        {
            get { return _isHaveData; }
            set { SetProperty(ref _isHaveData, value); }
        }
        private bool _isOption_Coppy = true, _IsOption_Replay = false;
        public bool IsOption_Coppy
        {
            get => _isOption_Coppy;
            set { SetProperty(ref _isOption_Coppy, value); }
        }
        public bool IsOption_Replay
        {
            get => _IsOption_Replay;
            set { SetProperty(ref _IsOption_Replay, value); }
        }
        public CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");


        protected INavigationService NavigationService { get; }

        public ViewModelBase()
        {
            NavigationService = ServiceLocator.Instance.Resolve<INavigationService>();
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }


        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value, () => RaisePropertyChanged(nameof(IsNotBusy)));
        }

        public bool IsNotBusy => !IsBusy;

       
        private DelegateCommand _backCommand;
        public DelegateCommand BackCommand => _backCommand ?? (_backCommand = new DelegateCommand(Back));
        private async void Back()
        {
            await NavigationService.NavigateBackAsync();
        }

        public override Task OnNavigationAsync(INavigationParameters parameters, NavigationType navigationType)
        {
            return Task.CompletedTask;
        }

        public override void Destroy()
        {
           
        }

   

     
    }
}
