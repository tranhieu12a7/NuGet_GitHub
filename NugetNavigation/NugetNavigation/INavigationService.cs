using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NugetNavigation
{
    public interface INavigationService
    {

        /// <summary>
        /// source lam moi 
        /// </summary>
        /// <param name="listPage"></param>
        /// <returns></returns>
        Task NavigateToAsync(List<Type> listViewModel);
        Task NavigateToAsync(List<Type> listViewModel, INavigationParameters parameters);
        Task NavigateToAsync(Type ViewModel);
        Task NavigateToAsync(Type ViewModel, PageMode pageMode);
        Task NavigateToAsync(Type ViewModel, INavigationParameters parameters);
        Task NavigateToAsync(Type ViewModel, INavigationParameters parameters, PageMode pageMode);
        Task NavigateBackAsync();
        Task NavigateBackToMainPageAsync();
        Task NavigateBackAsync(INavigationParameters parameters);
        Task NavigateBackAsync(INavigationParameters parameters, PageMode pageMode);
        Task NavigateBackAsyncWithPage(Type viewModelType, NavigationParameters parameters = null, int pageIndex = 1);
        Type StrClassViewModelByType(string strClass);
    }
}
