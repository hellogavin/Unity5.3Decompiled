namespace UnityEditor
{
    using System;

    [Serializable]
    internal class SavedFilter
    {
        public int m_Depth;
        public SearchFilter m_Filter;
        public int m_ID;
        public string m_Name;
        public float m_PreviewSize = -1f;

        public SavedFilter(string name, SearchFilter filter, int depth, float previewSize)
        {
            this.m_Name = name;
            this.m_Depth = depth;
            this.m_Filter = filter;
            this.m_PreviewSize = previewSize;
        }
    }
}

