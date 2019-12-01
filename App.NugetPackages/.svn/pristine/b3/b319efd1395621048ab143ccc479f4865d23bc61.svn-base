using PushNotifyLocal.Plugin.Abstractions;
using System;
using System.Threading;

namespace PushNotifyLocal.Plugin
{
    public class CrossLocalNotifications
    {
        private static Lazy<ILocalNotifications> _impl = new Lazy<ILocalNotifications>(CreateLocalNotifications, LazyThreadSafetyMode.PublicationOnly);
        public static ILocalNotifications Current
        {
            get
            {
                var val = _impl.Value;
                if (val == null)
                    throw NotImplementedInReferenceAssembly();
                return val;
            }
        }

        private static ILocalNotifications CreateLocalNotifications()
        {
#if NETSTANDARD2_0
            return null;
#else
            return new LocalNotifications();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException();
        }
    }
}
