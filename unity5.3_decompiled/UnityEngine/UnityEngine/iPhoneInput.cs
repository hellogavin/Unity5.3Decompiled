namespace UnityEngine
{
    using System;

    [Obsolete("iPhoneInput class is deprecated. Please use Input instead (UnityUpgradable) -> Input", true)]
    public class iPhoneInput
    {
        public static iPhoneAccelerationEvent GetAccelerationEvent(int index)
        {
            return new iPhoneAccelerationEvent();
        }

        public static iPhoneTouch GetTouch(int index)
        {
            return new iPhoneTouch();
        }

        public static Vector3 acceleration
        {
            get
            {
                return new Vector3();
            }
        }

        public static int accelerationEventCount
        {
            get
            {
                return 0;
            }
        }

        public static iPhoneAccelerationEvent[] accelerationEvents
        {
            get
            {
                return null;
            }
        }

        [Obsolete("lastLocation property is deprecated. Please use Input.location.lastData instead.", true)]
        public static LocationInfo lastLocation
        {
            get
            {
                return new LocationInfo();
            }
        }

        public static bool multiTouchEnabled
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        [Obsolete("orientation property is deprecated. Please use Input.deviceOrientation instead (UnityUpgradable) -> Input.deviceOrientation", true)]
        public static iPhoneOrientation orientation
        {
            get
            {
                return iPhoneOrientation.Unknown;
            }
        }

        public static int touchCount
        {
            get
            {
                return 0;
            }
        }

        public static iPhoneTouch[] touches
        {
            get
            {
                return null;
            }
        }
    }
}

