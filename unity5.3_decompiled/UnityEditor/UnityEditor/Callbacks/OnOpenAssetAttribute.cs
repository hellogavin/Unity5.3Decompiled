namespace UnityEditor.Callbacks
{
    using System;
    using UnityEditor;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    public sealed class OnOpenAssetAttribute : CallbackOrderAttribute
    {
        public OnOpenAssetAttribute()
        {
            base.m_CallbackOrder = 1;
        }

        public OnOpenAssetAttribute(int callbackOrder)
        {
            base.m_CallbackOrder = callbackOrder;
        }
    }
}

