using FileManager.Plugin.Abstractions;
using System;
using System.Threading;

namespace FileManager.Plugin
{
    public class CrossFileManager
    {
        private static Lazy<IFileManager> Implementation = new Lazy<IFileManager>(() => CreateDownloadManager(), LazyThreadSafetyMode.PublicationOnly);

#if __IOS__
        /// <summary>
        /// Set the background session completion handler.
        /// @see: https://developer.xamarin.com/guides/ios/application_fundamentals/backgrounding/part_4_ios_backgrounding_walkthroughs/background_transfer_walkthrough/#Handling_Transfer_Completion
        /// </summary>
        public static Action BackgroundSessionCompletionHandler;
#endif
        /// <summary>
        /// The platform-implementation
        /// </summary>
        public static IFileManager Current
        {
            get
            {
                var ret = Implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        private static IFileManager CreateDownloadManager()
        {
#if __IOS__
            return new FileManagerImplementation();
#elif __ANDROID__ || __UNIFIED__ || WINDOWS_UWP
            return new FileManagerImplementation();
#else
            return null;
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
        }
    }
}
