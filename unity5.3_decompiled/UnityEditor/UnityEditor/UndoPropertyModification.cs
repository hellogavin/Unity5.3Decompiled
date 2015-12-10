namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public struct UndoPropertyModification
    {
        public PropertyModification previousValue;
        public PropertyModification currentValue;
        private int m_KeepPrefabOverride;
        public bool keepPrefabOverride
        {
            get
            {
                return (this.m_KeepPrefabOverride != 0);
            }
            set
            {
                this.m_KeepPrefabOverride = !value ? 0 : 1;
            }
        }
    }
}

