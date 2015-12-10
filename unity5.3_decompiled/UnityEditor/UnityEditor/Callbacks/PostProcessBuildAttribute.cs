namespace UnityEditor.Callbacks
{
    using System;
    using UnityEditor;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    public sealed class PostProcessBuildAttribute : CallbackOrderAttribute
    {
        public PostProcessBuildAttribute()
        {
            base.m_CallbackOrder = 1;
        }

        public PostProcessBuildAttribute(int callbackOrder)
        {
            base.m_CallbackOrder = callbackOrder;
        }
    }
}

