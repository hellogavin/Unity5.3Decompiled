namespace UnityEngine
{
    using System;
    using System.Collections;

    [Obsolete("LocalNotification is deprecated. Please use iOS.LocalNotification instead (UnityUpgradable) -> UnityEngine.iOS.LocalNotification", true)]
    public sealed class LocalNotification
    {
        public string alertAction
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public string alertBody
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public string alertLaunchImage
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public int applicationIconBadgeNumber
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        public static string defaultSoundName
        {
            get
            {
                return null;
            }
        }

        public DateTime fireDate
        {
            get
            {
                return new DateTime();
            }
            set
            {
            }
        }

        public bool hasAction
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public CalendarIdentifier repeatCalendar
        {
            get
            {
                return CalendarIdentifier.GregorianCalendar;
            }
            set
            {
            }
        }

        public CalendarUnit repeatInterval
        {
            get
            {
                return CalendarUnit.Era;
            }
            set
            {
            }
        }

        public string soundName
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public string timeZone
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public IDictionary userInfo
        {
            get
            {
                return null;
            }
            set
            {
            }
        }
    }
}

