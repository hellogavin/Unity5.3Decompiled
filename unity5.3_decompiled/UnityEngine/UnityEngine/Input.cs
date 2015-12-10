namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class Input
    {
        private static Compass compassInstance;
        private static LocationService locationServiceInstance;
        private static Gyroscope m_MainGyro;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AccelerationEvent GetAccelerationEvent(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float GetAxis(string axisName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float GetAxisRaw(string axisName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetButton(string buttonName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetButtonDown(string buttonName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetButtonUp(string buttonName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string[] GetJoystickNames();
        public static bool GetKey(string name)
        {
            return GetKeyString(name);
        }

        public static bool GetKey(KeyCode key)
        {
            return GetKeyInt((int) key);
        }

        public static bool GetKeyDown(string name)
        {
            return GetKeyDownString(name);
        }

        public static bool GetKeyDown(KeyCode key)
        {
            return GetKeyDownInt((int) key);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool GetKeyDownInt(int key);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool GetKeyDownString(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool GetKeyInt(int key);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool GetKeyString(string name);
        public static bool GetKeyUp(string name)
        {
            return GetKeyUpString(name);
        }

        public static bool GetKeyUp(KeyCode key)
        {
            return GetKeyUpInt((int) key);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool GetKeyUpInt(int key);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool GetKeyUpString(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetMouseButton(int button);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetMouseButtonDown(int button);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetMouseButtonUp(int button);
        [Obsolete("Use ps3 move API instead", true)]
        public static Vector3 GetPosition(int deviceID)
        {
            return Vector3.zero;
        }

        [Obsolete("Use ps3 move API instead", true)]
        public static Quaternion GetRotation(int deviceID)
        {
            return Quaternion.identity;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Touch GetTouch(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_get_acceleration(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_get_compositionCursorPos(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_get_mousePosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_get_mouseScrollDelta(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_set_compositionCursorPos(ref Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsJoystickPreconfigured(string joystickName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int mainGyroIndex_Internal();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ResetInputAxes();

        public static Vector3 acceleration
        {
            get
            {
                Vector3 vector;
                INTERNAL_get_acceleration(out vector);
                return vector;
            }
        }

        public static int accelerationEventCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static AccelerationEvent[] accelerationEvents
        {
            get
            {
                int accelerationEventCount = Input.accelerationEventCount;
                AccelerationEvent[] eventArray = new AccelerationEvent[accelerationEventCount];
                for (int i = 0; i < accelerationEventCount; i++)
                {
                    eventArray[i] = GetAccelerationEvent(i);
                }
                return eventArray;
            }
        }

        public static bool anyKey { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool anyKeyDown { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool backButtonLeavesApp { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static Compass compass
        {
            get
            {
                if (compassInstance == null)
                {
                    compassInstance = new Compass();
                }
                return compassInstance;
            }
        }

        public static bool compensateSensors { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static Vector2 compositionCursorPos
        {
            get
            {
                Vector2 vector;
                INTERNAL_get_compositionCursorPos(out vector);
                return vector;
            }
            set
            {
                INTERNAL_set_compositionCursorPos(ref value);
            }
        }

        public static string compositionString { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static DeviceOrientation deviceOrientation { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("eatKeyPressOnTextFieldFocus property is deprecated, and only provided to support legacy behavior.")]
        public static bool eatKeyPressOnTextFieldFocus { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static Gyroscope gyro
        {
            get
            {
                if (m_MainGyro == null)
                {
                    m_MainGyro = new Gyroscope(mainGyroIndex_Internal());
                }
                return m_MainGyro;
            }
        }

        public static IMECompositionMode imeCompositionMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool imeIsSelected { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string inputString { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("isGyroAvailable property is deprecated. Please use SystemInfo.supportsGyroscope instead.")]
        public static bool isGyroAvailable { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static LocationService location
        {
            get
            {
                if (locationServiceInstance == null)
                {
                    locationServiceInstance = new LocationService();
                }
                return locationServiceInstance;
            }
        }

        public static Vector3 mousePosition
        {
            get
            {
                Vector3 vector;
                INTERNAL_get_mousePosition(out vector);
                return vector;
            }
        }

        public static bool mousePresent { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static Vector2 mouseScrollDelta
        {
            get
            {
                Vector2 vector;
                INTERNAL_get_mouseScrollDelta(out vector);
                return vector;
            }
        }

        public static bool multiTouchEnabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool simulateMouseWithTouches { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool stylusTouchSupported { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int touchCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static Touch[] touches
        {
            get
            {
                int touchCount = Input.touchCount;
                Touch[] touchArray = new Touch[touchCount];
                for (int i = 0; i < touchCount; i++)
                {
                    touchArray[i] = GetTouch(i);
                }
                return touchArray;
            }
        }

        public static bool touchPressureSupported { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool touchSupported
        {
            get
            {
                return false;
            }
        }
    }
}

