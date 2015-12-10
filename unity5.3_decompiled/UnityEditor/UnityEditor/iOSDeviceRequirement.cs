namespace UnityEditor
{
    using System;
    using System.Collections.Generic;

    public sealed class iOSDeviceRequirement
    {
        private SortedDictionary<string, string> m_Values = new SortedDictionary<string, string>();

        public IDictionary<string, string> values
        {
            get
            {
                return this.m_Values;
            }
        }
    }
}

