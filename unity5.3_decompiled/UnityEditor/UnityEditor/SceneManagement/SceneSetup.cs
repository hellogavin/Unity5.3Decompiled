namespace UnityEditor.SceneManagement
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public class SceneSetup
    {
        private string m_Path;
        private bool m_IsLoaded;
        private bool m_IsActive;
        public string path
        {
            get
            {
                return this.m_Path;
            }
            set
            {
                this.m_Path = value;
            }
        }
        public bool isLoaded
        {
            get
            {
                return this.m_IsLoaded;
            }
            set
            {
                this.m_IsLoaded = value;
            }
        }
        public bool isActive
        {
            get
            {
                return this.m_IsActive;
            }
            set
            {
                this.m_IsActive = value;
            }
        }
    }
}

