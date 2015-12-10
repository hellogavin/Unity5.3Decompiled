namespace UnityEditor
{
    using System;

    internal class SavedInt
    {
        private string m_Name;
        private int m_Value;

        public SavedInt(string name, int value)
        {
            this.m_Name = name;
            this.m_Value = EditorPrefs.GetInt(name, value);
        }

        public static implicit operator int(SavedInt s)
        {
            return s.value;
        }

        public int value
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
                    EditorPrefs.SetInt(this.m_Name, value);
                }
            }
        }
    }
}

