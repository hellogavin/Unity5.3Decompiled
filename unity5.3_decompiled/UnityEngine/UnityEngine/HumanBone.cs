namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct HumanBone
    {
        private string m_BoneName;
        private string m_HumanName;
        public HumanLimit limit;
        public string boneName
        {
            get
            {
                return this.m_BoneName;
            }
            set
            {
                this.m_BoneName = value;
            }
        }
        public string humanName
        {
            get
            {
                return this.m_HumanName;
            }
            set
            {
                this.m_HumanName = value;
            }
        }
    }
}

