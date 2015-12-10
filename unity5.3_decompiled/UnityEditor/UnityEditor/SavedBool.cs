namespace UnityEditor
{
    using System;

    internal class SavedBool
    {
        private string m_Name;
        private bool m_Value;

        public SavedBool(string name, bool value)
        {
            this.m_Name = name;
            this.m_Value = EditorPrefs.GetBool(name, value);
        }

        public static implicit operator bool(SavedBool s)
        {
            return s.value;
        }

        public bool value
        {
            get
            {
                return this.m_Value;
            }
            set
            {
                if (this.m_Value != value)
                {
                    this.m_Value = value;
                    EditorPrefs.SetBool(this.m_Name, value);
                }
            }
        }
    }
}

