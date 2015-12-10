namespace UnityEngine
{
    using System;

    internal class SystemClock
    {
        private static readonly DateTime s_Epoch = new DateTime(0x7b2, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixTimeMilliseconds(DateTime date)
        {
            TimeSpan span = (TimeSpan) (date.ToUniversalTime() - s_Epoch);
            return Convert.ToInt64(span.TotalMilliseconds);
        }

        public static long ToUnixTimeSeconds(DateTime date)
        {
            TimeSpan span = (TimeSpan) (date.ToUniversalTime() - s_Epoch);
            return Convert.ToInt64(span.TotalSeconds);
        }

        public static DateTime now
        {
            get
            {
                return DateTime.Now;
            }
        }
    }
}

