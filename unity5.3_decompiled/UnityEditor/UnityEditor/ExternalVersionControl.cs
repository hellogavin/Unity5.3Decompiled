namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct ExternalVersionControl
    {
        private string m_Value;
        public static readonly string Disabled;
        public static readonly string AutoDetect;
        public static readonly string Generic;
        public static readonly string AssetServer;
        public ExternalVersionControl(string value)
        {
            this.m_Value = value;
        }

        static ExternalVersionControl()
        {
            Disabled = "Hidden Meta Files";
            AutoDetect = "Auto detect";
            Generic = "Visible Meta Files";
            AssetServer = "Asset Server";
        }

        public override string ToString()
        {
            return this.m_Value;
        }

        public static implicit operator string(ExternalVersionControl d)
        {
            return d.ToString();
        }

        public static implicit operator ExternalVersionControl(string d)
        {
            return new ExternalVersionControl(d);
        }
    }
}

