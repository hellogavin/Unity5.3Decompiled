namespace UnityEditor
{
    using System;
    using UnityEditorInternal;

    internal class AInfo : IComparable, IEquatable<AInfo>
    {
        public int m_ClassID;
        public string m_DisplayText;
        public int m_Flags;
        public bool m_GizmoEnabled;
        public bool m_IconEnabled;
        public string m_ScriptClass;

        public AInfo(bool gizmoEnabled, bool iconEnabled, int flags, int classID, string scriptClass)
        {
            this.m_GizmoEnabled = gizmoEnabled;
            this.m_IconEnabled = iconEnabled;
            this.m_ClassID = classID;
            this.m_ScriptClass = scriptClass;
            this.m_Flags = flags;
            if (this.m_ScriptClass == string.Empty)
            {
                this.m_DisplayText = BaseObjectTools.ClassIDToString(this.m_ClassID);
            }
            else
            {
                this.m_DisplayText = this.m_ScriptClass;
            }
        }

        public int CompareTo(object obj)
        {
            AInfo info = obj as AInfo;
            if (info == null)
            {
                throw new ArgumentException("Object is not an AInfo");
            }
            return this.m_DisplayText.CompareTo(info.m_DisplayText);
        }

        public bool Equals(AInfo other)
        {
            return ((this.m_ClassID == other.m_ClassID) && (this.m_ScriptClass == other.m_ScriptClass));
        }

        public bool HasGizmo()
        {
            return ((this.m_Flags & 2) > 0);
        }

        public bool HasIcon()
        {
            return ((this.m_Flags & 1) > 0);
        }

        private bool IsBitSet(byte b, int pos)
        {
            return ((b & (((int) 1) << pos)) != 0);
        }

        public enum Flags
        {
            kHasGizmo = 2,
            kHasIcon = 1
        }
    }
}

