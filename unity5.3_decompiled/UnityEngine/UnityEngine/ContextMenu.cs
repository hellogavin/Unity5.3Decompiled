namespace UnityEngine
{
    using System;

    public sealed class ContextMenu : Attribute
    {
        private string m_ItemName;

        public ContextMenu(string name)
        {
            this.m_ItemName = name;
        }

        public string menuItem
        {
            get
            {
                return this.m_ItemName;
            }
        }
    }
}

