using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NugetNavigation;

namespace Sample.ViewModels
{
   public class MasterDetailPageViewModel : ViewModelBase
    {
        public MasterDetailPageViewModel()
        {
        }
        public override Task OnNavigationAsync(INavigationParameters parameters, NavigationType navigationType)
        {

            return base.OnNavigationAsync(parameters, navigationType);
        }
    }
}
