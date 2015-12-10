namespace UnityEditor.Callbacks
{
    using System;
    using UnityEditor;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    public sealed class DidReloadScripts : CallbackOrderAttribute
    {
        public DidReloadScripts()
        {
            base.m_CallbackOrder = 1;
        }

        public DidReloadScripts(int callbackOrder)
        {
            base.m_CallbackOrder = callbackOrder;
        }
    }
}

