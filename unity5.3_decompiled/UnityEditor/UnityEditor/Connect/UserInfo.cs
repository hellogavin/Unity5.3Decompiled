namespace UnityEditor.Connect
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct UserInfo
    {
        private int m_Valid;
        private string m_UserName;
        private string m_DisplayName;
        private string m_PrimaryOrg;
        private string m_AccessToken;
        private int m_AccessTokenValiditySeconds;
        public bool valid
        {
            get
            {
                return (this.m_Valid != 0);
            }
        }
        public string userName
        {
            get
            {
                return this.m_UserName;
            }
        }
        public string displayName
        {
            get
            {
                return this.m_DisplayName;
            }
        }
        public string primaryOrg
        {
            get
            {
                return this.m_PrimaryOrg;
            }
        }
        public string accessToken
        {
            get
            {
                return this.m_AccessToken;
            }
        }
        public int accessTokenValiditySeconds
        {
            get
            {
                return this.m_AccessTokenValiditySeconds;
            }
        }
    }
}

