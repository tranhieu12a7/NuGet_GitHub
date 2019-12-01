using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NugetNavigation;

namespace Sample.ViewModels
{
    class Page3ViewModel : ViewModelBase
    {
        public Page3ViewModel()
        {
        }
        public override Task OnNavigationAsync(INavigationParameters parameters, NavigationType navigationType)
        {
            return base.OnNavigationAsync(parameters, navigationType);
        }
    }
}
