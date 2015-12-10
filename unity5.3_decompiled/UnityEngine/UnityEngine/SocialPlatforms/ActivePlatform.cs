namespace UnityEngine.SocialPlatforms
{
    using System;

    internal static class ActivePlatform
    {
        private static ISocialPlatform _active;

        private static ISocialPlatform SelectSocialPlatform()
        {
            return new Local();
        }

        internal static ISocialPlatform Instance
        {
            get
            {
                if (_active == null)
                {
                    _active = SelectSocialPlatform();
                }
                return _active;
            }
            set
            {
                _active = value;
            }
        }
    }
}

