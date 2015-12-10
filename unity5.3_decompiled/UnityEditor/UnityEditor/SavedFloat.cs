namespace UnityEditor
{
    using System;

    internal class SavedFloat
    {
        private string m_Name;
        private float m_Value;

        public SavedFloat(string name, float value)
        {
            this.m_Name = name;
            this.m_Value = EditorPrefs.GetFloat(name, value);
        }

        public static implicit operator float(SavedFloat s)
        {
            return s.value;
        }

        public float value
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
                    EditorPrefs.SetFloat(this.m_Name, value);
                }
            }
        }
    }
}

