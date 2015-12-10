namespace UnityEngine
{
    using System;

    [Obsolete("iPhoneKeyboard class is deprecated. Please use TouchScreenKeyboard instead (UnityUpgradable) -> TouchScreenKeyboard", true)]
    public class iPhoneKeyboard
    {
        public bool active
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public static Rect area
        {
            get
            {
                return new Rect();
            }
        }

        public bool done
        {
            get
            {
                return false;
            }
        }

        public static bool hideInput
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public string text
        {
            get
            {
                return string.Empty;
            }
            set
            {
            }
        }

        public static bool visible
        {
            get
            {
                return false;
            }
        }
    }
}

