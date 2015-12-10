namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class SamsungTV
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern TouchPadMode GetTouchPadMode();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetSystemLanguage(SystemLanguage language);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool SetTouchPadMode(TouchPadMode value);

        public static bool airMouseConnected { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static GamePadMode gamePadMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static GestureMode gestureMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool gestureWorking { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static TouchPadMode touchPadMode
        {
            get
            {
                return GetTouchPadMode();
            }
            set
            {
                if (!SetTouchPadMode(value))
                {
                    throw new ArgumentException("Fail to set touchPadMode.");
                }
            }
        }

        public enum GamePadMode
        {
            Default,
            Mouse
        }

        public enum GestureMode
        {
            Off,
            Mouse,
            Joystick
        }

        public sealed class OpenAPI
        {
            public static string dUid { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

            public static OpenAPIServerType serverType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

            public static string timeOnTV { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

            public static string uid { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

            public enum OpenAPIServerType
            {
                Operating,
                Development,
                Developing,
                Invalid
            }
        }

        public enum TouchPadMode
        {
            Dpad,
            Joystick,
            Mouse
        }
    }
}

