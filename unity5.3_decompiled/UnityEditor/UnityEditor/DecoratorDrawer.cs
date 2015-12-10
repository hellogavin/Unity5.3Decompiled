namespace UnityEditor
{
    using System;
    using UnityEngine;

    public abstract class DecoratorDrawer : GUIDrawer
    {
        internal PropertyAttribute m_Attribute;

        protected DecoratorDrawer()
        {
        }

        public virtual float GetHeight()
        {
            return 16f;
        }

        public virtual void OnGUI(Rect position)
        {
        }

        public PropertyAttribute attribute
        {
            get
            {
                return this.m_Attribute;
            }
        }
    }
}

