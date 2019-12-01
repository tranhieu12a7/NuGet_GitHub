using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace NugetNavigation
{
    public static class ViewModelLocator
    {
        internal static readonly BindableProperty PartialViewsProperty =
         BindableProperty.CreateAttached("PrismPartialViews", typeof(List<BindableObject>), typeof(PageUtilities), null);

        internal static List<BindableObject> GetPartialViews(this Page page)
        {
            return (List<BindableObject>)page.GetValue(PartialViewsProperty);
        }
    }
    public static class PageUtilities
    {

        public static void InvokeViewAndViewModelAction<T>(object view, Action<T> action) where T : class
        {
            if (view is T viewAsT)
                action(viewAsT);

            if (view is BindableObject element)
            {
                if (element.BindingContext is T viewModelAsT)
                {
                    action(viewModelAsT);
                }
            }

            if (view is Page page && page.GetPartialViews() is List<BindableObject> partials)
            {
                foreach (var partial in partials)
                {
                    InvokeViewAndViewModelAction(partial, action);
                }
            }
        }
        public static Page GetOnNavigatedToTargetFromChild(Page target)
        {
            Page child = null;
            if (target is MasterDetailPage)
            {
                child = ((MasterDetailPage)target).Detail;
            }
            else if (target is TabbedPage)
            {
                child = ((TabbedPage)target).CurrentPage;
            }
            else if (target is CarouselPage)
            {
                child = ((CarouselPage)target).CurrentPage;
            }
            else if (target is NavigationPage)
            {
                child = target.Navigation.NavigationStack.Last();
            }

            if (child != null)
                target = GetOnNavigatedToTargetFromChild(child);

            return target;
        }

        public static Page GetCurrentPage(Page mainPage)
        {
            var page = mainPage;

            var lastModal = page.Navigation.ModalStack.LastOrDefault();
            if (lastModal != null)
                page = lastModal;

            return GetOnNavigatedToTargetFromChild(page);
        }
    }
}
