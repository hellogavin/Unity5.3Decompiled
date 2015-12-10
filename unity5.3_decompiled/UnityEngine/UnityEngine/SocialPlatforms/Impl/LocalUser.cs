namespace UnityEngine.SocialPlatforms.Impl
{
    using System;
    using UnityEngine.SocialPlatforms;

    public class LocalUser : UserProfile, ILocalUser, IUserProfile
    {
        private bool m_Authenticated = false;
        private IUserProfile[] m_Friends = new UserProfile[0];
        private bool m_Underage = false;

        public void Authenticate(Action<bool> callback)
        {
            ActivePlatform.Instance.Authenticate(this, callback);
        }

        public void LoadFriends(Action<bool> callback)
        {
            ActivePlatform.Instance.LoadFriends(this, callback);
        }

        public void SetAuthenticated(bool value)
        {
            this.m_Authenticated = value;
        }

        public void SetFriends(IUserProfile[] friends)
        {
            this.m_Friends = friends;
        }

        public void SetUnderage(bool value)
        {
            this.m_Underage = value;
        }

        public bool authenticated
        {
            get
            {
                return this.m_Authenticated;
            }
        }

        public IUserProfile[] friends
        {
            get
            {
                return this.m_Friends;
            }
        }

        public bool underage
        {
            get
            {
                return this.m_Underage;
            }
        }
    }
}

