namespace UnityEngine.iOS
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class NotificationServices
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CancelAllLocalNotifications();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CancelLocalNotification(LocalNotification notification);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ClearLocalNotifications();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ClearRemoteNotifications();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern LocalNotification GetLocalNotification(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern RemoteNotification GetRemoteNotification(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void PresentLocalNotificationNow(LocalNotification notification);
        public static void RegisterForNotifications(NotificationType notificationTypes)
        {
            RegisterForNotifications(notificationTypes, true);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RegisterForNotifications(NotificationType notificationTypes, bool registerForRemote);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ScheduleLocalNotification(LocalNotification notification);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void UnregisterForRemoteNotifications();

        public static byte[] deviceToken { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static NotificationType enabledNotificationTypes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int localNotificationCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static LocalNotification[] localNotifications
        {
            get
            {
                int localNotificationCount = NotificationServices.localNotificationCount;
                LocalNotification[] notificationArray = new LocalNotification[localNotificationCount];
                for (int i = 0; i < localNotificationCount; i++)
                {
                    notificationArray[i] = GetLocalNotification(i);
                }
                return notificationArray;
            }
        }

        public static string registrationError { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int remoteNotificationCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static RemoteNotification[] remoteNotifications
        {
            get
            {
                int remoteNotificationCount = NotificationServices.remoteNotificationCount;
                RemoteNotification[] notificationArray = new RemoteNotification[remoteNotificationCount];
                for (int i = 0; i < remoteNotificationCount; i++)
                {
                    notificationArray[i] = GetRemoteNotification(i);
                }
                return notificationArray;
            }
        }

        public static LocalNotification[] scheduledLocalNotifications { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

