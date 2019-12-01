﻿using NugetNavigation;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NugetNavigation.Mvvm
{
    public abstract class BindableBase : INotifyPropertyChanged
    {
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(propertyName);
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, Action action,
            [CallerMemberName] string propertyName = null)
        {
            var r = SetProperty(ref storage, value, propertyName);
            if (r) action?.Invoke();
            return r;
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            RaisePropertyChanged(propertyName);

            return true;
        }

        public abstract Task OnNavigationAsync(INavigationParameters parameters, NavigationType navigationType);
        public abstract void Destroy();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}