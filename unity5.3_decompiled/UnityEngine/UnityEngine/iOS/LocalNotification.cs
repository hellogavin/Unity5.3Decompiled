namespace UnityEngine.iOS
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    public sealed class LocalNotification
    {
        private static long m_NSReferenceDateTicks;
        private IntPtr notificationWrapper;

        static LocalNotification()
        {
            DateTime time = new DateTime(0x7d1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            m_NSReferenceDateTicks = time.Ticks;
        }

        public LocalNotification()
        {
            this.InitWrapper();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Destroy();
        ~LocalNotification()
        {
            this.Destroy();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern double GetFireDate();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void InitWrapper();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetFireDate(double dt);

        public string alertAction { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public string alertBody { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public string alertLaunchImage { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int applicationIconBadgeNumber { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static string defaultSoundName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public DateTime fireDate
        {
            get
            {
                return new DateTime(((long) (this.GetFireDate() * 10000000.0)) + m_NSReferenceDateTicks);
            }
            set
            {
                this.SetFireDate(((double) (value.ToUniversalTime().Ticks - m_NSReferenceDateTicks)) / 10000000.0);
            }
        }

        public bool hasAction { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public CalendarIdentifier repeatCalendar { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public CalendarUnit repeatInterval { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public string soundName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public string timeZone { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public IDictionary userInfo { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

