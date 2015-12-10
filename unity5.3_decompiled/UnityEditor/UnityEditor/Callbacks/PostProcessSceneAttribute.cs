namespace UnityEditor.Callbacks
{
    using System;
    using UnityEditor;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    public sealed class PostProcessSceneAttribute : CallbackOrderAttribute
    {
        private int m_version;

        public PostProcessSceneAttribute()
        {
            base.m_CallbackOrder = 1;
            this.m_version = 0;
        }

        public PostProcessSceneAttribute(int callbackOrder)
        {
            base.m_CallbackOrder = callbackOrder;
            this.m_version = 0;
        }

        public PostProcessSceneAttribute(int callbackOrder, int version)
        {
            base.m_CallbackOrder = callbackOrder;
            this.m_version = version;
        }

        internal int version
        {
            get
            {
                return this.m_version;
            }
        }
    }
}

