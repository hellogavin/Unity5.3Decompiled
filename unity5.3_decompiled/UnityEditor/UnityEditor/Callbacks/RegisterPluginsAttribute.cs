namespace UnityEditor.Callbacks
{
    using System;
    using UnityEditor;

    internal sealed class RegisterPluginsAttribute : CallbackOrderAttribute
    {
        public RegisterPluginsAttribute()
        {
            base.m_CallbackOrder = 1;
        }

        public RegisterPluginsAttribute(int callbackOrder)
        {
            base.m_CallbackOrder = callbackOrder;
        }
    }
}

