using NugetNavigation.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NugetNavigation
{
    public class NavigationService : INavigationService
    {
        protected Application CurrentApplication => Application.Current;



        /// <summary>
        /// soucre lam moi
        /// </summary>
        /// <param name="listPage"></param>
        /// <returns></returns>

        public Task NavigateToAsync(Type ViewModel)
        {
            return NavigateToAsync(ViewModel, new NavigationParameters());
        }
        public Task NavigateToAsync(Type ViewModel, PageMode pageMode)
        {
            return NavigateToAsync(ViewModel, new NavigationParameters(), pageMode);
        }
        public Task NavigateToAsync(Type ViewModel, INavigationParameters parameters)
        {
            return NavigateToAsync(ViewModel, parameters, PageMode.Default);
        }
        public async Task NavigateToAsync(Type ViewModel, INavigationParameters parameters, PageMode pageMode)
        {
            var view = TypeByPage(ViewModel);
            BindingContext(view, parameters, ViewModel);
            if (view != null)
            {
                if (pageMode == PageMode.RemovePage)
                    CurrentApplication.MainPage = CreateCustomNavigationPage(view);
                else if (pageMode == PageMode.Modal)
                {
                    var masterDetail = CurrentApplication.MainPage as Page;
                    await masterDetail.Navigation.PushModalAsync(view);
                }
                else
                {
                    await PushPage(view);
                }
                await _OnNavigation(view, parameters);
            }
        }
        public Task NavigateToAsync(List<Type> listPage)
        {
            return NavigateToAsync(listPage, new NavigationParameters());
        }
        public Task NavigateToAsync(List<Type> listPage, INavigationParameters parameters)
        {
            return NavigatedToPage(0, listPage, parameters);
        }
        public Task NavigateBackAsync()
        {
            return NavigateBackAsync(new NavigationParameters());
        }
        public Task NavigateBackAsync(INavigationParameters parameters)
        {
            return NavigateBackAsync(new NavigationParameters(), PageMode.Default);
        }
        public async Task NavigateBackToMainPageAsync()
        {
            if (!(CurrentApplication.MainPage is CustomNavigationPage))
                return;

            for (var i = CurrentApplication.MainPage.Navigation.NavigationStack.Count - 2; i > 0; i--)
                CurrentApplication.MainPage?.Navigation.RemovePage(CurrentApplication.MainPage.Navigation
                    .NavigationStack[i]);

            await CurrentApplication.MainPage.Navigation.PopAsync();
        }
        public async Task NavigateBackAsync(INavigationParameters parameters, PageMode pageMode)
        {
            paramMain = parameters;

            if (CurrentApplication.MainPage is CustomNavigationPage navigationPage)
            {
                await navigationPage.PopAsync();
            }
            else if (CurrentApplication.MainPage is MasterDetailPage masterDetail)
            {
                if (masterDetail.Detail is CustomNavigationPage _navigationPage)
                {
                    await _navigationPage.PopAsync();
                    masterDetail.IsPresented = false;
                }
            }
        }
        public async Task NavigateBackAsyncWithPage(Type viewModelType, NavigationParameters parameters = null, int pageIndex = 1)
        {
            if (CurrentApplication.MainPage is MasterDetailPage masterDetail)
            {
                if (masterDetail.Detail is CustomNavigationPage customNavigation)
                {
                    await NavigateToAsync(viewModelType, parameters);
                    for (int i = 0; i < pageIndex; i++)
                    {
                        customNavigation.Navigation.RemovePage(customNavigation.Navigation.NavigationStack[customNavigation.Navigation.NavigationStack.Count() - 2]);
                    }
                }
            }
            else if (CurrentApplication.MainPage is CustomNavigationPage customNavigation1)
            {
                await NavigateToAsync(viewModelType, parameters);
                for (int i = 0; i < pageIndex; i++)
                {
                    customNavigation1.Navigation.RemovePage(customNavigation1.Navigation.NavigationStack[customNavigation1.Navigation.NavigationStack.Count() - 2]);
                }
            }
        }

        public async Task NavigatedToPage(int index, List<Type> listPage, INavigationParameters parameters,
            MasterDetailPage _masterDetailPage = null)
        {
            try
            {
                if (index < listPage.Count)
                {
                    var pagetarget = TypeByPage(listPage[index]);
                    if (pagetarget is MasterDetailPage masterDetailPage)
                    {
                        OnNavigation_MasterDetailPage(index, masterDetailPage, listPage, parameters);
                    }
                    else if (pagetarget is TabbedPage tabbedPage)
                    {
                        OnNavigation_TabbedPage(index, tabbedPage, listPage, parameters, _masterDetailPage);

                    }
                    else if (pagetarget is Page navigationPage)
                    {
                        OnNavigation_NavigationPage(index, navigationPage, listPage, parameters, _masterDetailPage);
                        //var page_ = BindingContext2(navigationPage, parameters, listPage[index]);
                        //await _OnNavigation2(page_, parameters);
                    }

                    //if (listPage[index+1] != null)
                    //   await OnNavigatedToPage(index+1, listPage, parameters);
                }
            }
            catch (Exception exx)
            {
                Debugger.Break();
            }

        }
        private async void OnNavigation_NavigationPage(int index, Page navigationPage, List<Type> listPage, INavigationParameters parameters,
            MasterDetailPage _masterDetailPage = null)
        {
            try
            {

                if (CurrentApplication.MainPage is MasterDetailPage masterDetailPage)
                {
                    if (masterDetailPage.Detail is CustomNavigationPage customNavigation)
                        if (customNavigation.CurrentPage is TabbedPage tabbedPage)
                        {
                            int pageChild = 0;
                            foreach (var itemPage in tabbedPage.Children.ToArray())
                            {
                                if (itemPage.GetType().FullName == navigationPage.GetType().FullName.Replace("ViewModel", "View"))
                                {
                                    tabbedPage.CurrentPage = itemPage;
                                    pageChild = 1;
                                }
                            }
                            if (pageChild == 0)
                            {
                                masterDetailPage.Detail = CreateCustomNavigationPage(navigationPage);
                                CurrentApplication.MainPage = masterDetailPage;
                                var aaa = CurrentApplication.MainPage as MasterDetailPage;
                                aaa.IsPresented = false;
                                await NavigatedToPage(index + 1, listPage, parameters);
                                var page_ = BindingContext(navigationPage, parameters, listPage[index]);
                                await _OnNavigation(page_, parameters);
                            }
                            else
                                await NavigatedToPage(index + 1, listPage, parameters);
                        }
                        else if (customNavigation.CurrentPage.GetType().FullName != navigationPage.GetType().FullName)
                        {
                            masterDetailPage.Detail = CreateCustomNavigationPage(navigationPage);
                            await NavigatedToPage(index + 1, listPage, parameters);
                            var page_ = BindingContext(navigationPage, parameters, listPage[index]);
                            await _OnNavigation(page_, parameters);
                        }
                }
                else
                {
                    if (_masterDetailPage == null)
                    {
                        await PushPage(navigationPage);
                        await NavigatedToPage(index + 1, listPage, parameters);
                    }
                    else
                    {
                        _masterDetailPage.Detail = CreateCustomNavigationPage(navigationPage);
                        CurrentApplication.MainPage = _masterDetailPage;
                        await NavigatedToPage(index + 1, listPage, parameters);
                    }
                    var page_ = BindingContext(navigationPage, parameters, listPage[index]);
                    await _OnNavigation(page_, parameters);
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
                throw;
            }
        }
        private async void OnNavigation_TabbedPage(int index, TabbedPage tabbedPage, List<Type> listPage, INavigationParameters parameters,
            MasterDetailPage _masterDetailPage = null)
        {

            if (CurrentApplication.MainPage is MasterDetailPage masterDetailPage)
            {
                if (masterDetailPage.Detail is CustomNavigationPage customNavigationPage)
                    if (customNavigationPage.CurrentPage is TabbedPage _tabbedPage)
                    {
                        if (_tabbedPage.GetType().FullName == tabbedPage.GetType().FullName)
                        {
                            await NavigatedToPage(index + 1, listPage, parameters);
                            return;
                        }
                        else
                        {
                            masterDetailPage.Detail = CreateCustomNavigationPage(tabbedPage);
                            CurrentApplication.MainPage = _tabbedPage;
                            await NavigatedToPage(index + 1, listPage, parameters);

                        }
                    }
                    else
                    {
                        masterDetailPage.Detail = CreateCustomNavigationPage(tabbedPage);
                        await NavigatedToPage(index + 1, listPage, parameters);

                    }
            }
            else
            {
                if (_masterDetailPage == null)
                {
                    CurrentApplication.MainPage = tabbedPage;
                    await NavigatedToPage(index + 1, listPage, parameters);
                }
                else
                {
                    _masterDetailPage.Detail = CreateCustomNavigationPage(tabbedPage);
                    CurrentApplication.MainPage = _masterDetailPage;
                    await NavigatedToPage(index + 1, listPage, parameters);
                }
            }

            var page_ = BindingContext(tabbedPage, parameters, listPage[index]);
            await _OnNavigation(page_, parameters);
        }
        private async void OnNavigation_MasterDetailPage(int index, MasterDetailPage masterDetailPage, List<Type> listPage,
            INavigationParameters parameters)
        {

            if (CurrentApplication.MainPage == null)
            {
                await NavigatedToPage(index + 1, listPage, parameters, masterDetailPage);
                //CurrentApplication.MainPage = masterDetailPage;
            }
            else if (CurrentApplication.MainPage is MasterDetailPage masterDetail)
            {
                if (masterDetail.GetType().FullName == masterDetailPage.GetType().FullName)
                {
                    await NavigatedToPage(index + 1, listPage, parameters);
                    var aaaa = CurrentApplication.MainPage as MasterDetailPage;
                    aaaa.IsPresented = false;
                    return;
                }
                await NavigatedToPage(index + 1, listPage, parameters, masterDetailPage);
                var aaa = CurrentApplication.MainPage as MasterDetailPage;
                aaa.IsPresented = false;
            }
            else
            {
                await NavigatedToPage(index + 1, listPage, parameters, masterDetailPage);
                var aaa = CurrentApplication.MainPage as MasterDetailPage;
                aaa.IsPresented = false;
            }

            var page_ = BindingContext(masterDetailPage, parameters, listPage[index]);
            await _OnNavigation(page_, parameters);
        }

        public Type StrClassViewModelByType(string strClass)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            return asm?.GetTypes().FirstOrDefault(x => x.Name == strClass);
        }
        public async Task PushPage(Page navigationPage)
        {

            if (CurrentApplication.MainPage is MasterDetailPage masterDetail)
            {
                if (masterDetail.Detail is CustomNavigationPage customNavigation)
                {
                    await customNavigation.PushAsync(navigationPage);
                }
                masterDetail.IsPresented = false;
            }
            else if (CurrentApplication.MainPage is CustomNavigationPage customNavigation)
            {
                await customNavigation.PushAsync(navigationPage);
            }
            else
            {
                CurrentApplication.MainPage = CreateCustomNavigationPage(navigationPage);
            }
        }
        public Page TypeByPage(Type type)
        {
            try
            {
                var NamePage = type.FullName.Replace("ViewModel", "View");
                var aaa = ServiceLocator.Instance._containerBuilder.GetType();
                var viewType = Type.GetType(NamePage);
                return Activator.CreateInstance(aaa) as Page;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task _OnNavigation(Page page, INavigationParameters param)
        {
            if (page.BindingContext is BindableBase vm)
            {
                await vm.OnNavigationAsync(param, NavigationType.New);

                if (page is TabbedPage tabbedPage)
                {
                    for (int i = 0; i < tabbedPage.Children.ToArray().Length; i++)
                    {
                        await _OnNavigation(tabbedPage.Children.ToArray()[i], param);
                    }
                }
            }
        }
        public Page BindingContext(Page page, INavigationParameters param, Type viewModelType = null)
        {
            try
            {
                Type ClassViewModel;
                if (viewModelType == null)
                {
                    ClassViewModel = Type.GetType(page?.GetType().FullName.Replace("View", "ViewModel"));
                }
                else
                    ClassViewModel = viewModelType;
                if (page != null)
                {
                    page.BindingContext = ServiceLocator.Instance.Resolve(ClassViewModel);

                    if (page is TabbedPage tabbedPage)
                    {
                        for (int i = 0; i < tabbedPage.Children.ToArray().Length; i++)
                        {
                            tabbedPage.Children.ToArray()[i] = CreateCustomNavigationPage(BindingContext(tabbedPage.Children.ToArray()[i], param));
                        }
                    }
                }
                return page;
            }
            catch (Exception ex)
            {
                Debugger.Break();
                return null;

            }
        }
        private INavigationParameters paramMain = new NavigationParameters();
        public CustomNavigationPage CreateCustomNavigationPage(Page page)
        {
            var navigationPage = new CustomNavigationPage(page);
            //navigationPage.TransitionType = TransitionType.SlideFromRight;
            navigationPage.Popped += CustomNavigationPage_Popped;
            return navigationPage;
        }
        private async void CustomNavigationPage_Popped(object sender, NavigationEventArgs e)
        {
            var newPageBack = sender as CustomNavigationPage;
            if (PageUtilities.GetCurrentPage(newPageBack).BindingContext is BindableBase vm)
            {
                await vm.OnNavigationAsync(paramMain, NavigationType.Back);
            }
            if (e.Page.BindingContext is BindableBase vmD)
            {
                vmD.Destroy();
            }
            paramMain = new NavigationParameters();
        }


    }
}
