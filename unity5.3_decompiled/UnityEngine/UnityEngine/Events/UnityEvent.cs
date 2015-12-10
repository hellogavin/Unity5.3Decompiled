namespace UnityEngine.Events
{
    using System;
    using System.Reflection;
    using UnityEngine;
    using UnityEngineInternal;

    [Serializable]
    public class UnityEvent : UnityEventBase
    {
        private readonly object[] m_InvokeArray = new object[0];

        public void AddListener(UnityAction call)
        {
            base.AddCall(GetDelegate(call));
        }

        internal void AddPersistentListener(UnityAction call)
        {
            this.AddPersistentListener(call, UnityEventCallState.RuntimeOnly);
        }

        internal void AddPersistentListener(UnityAction call, UnityEventCallState callState)
        {
            int persistentEventCount = base.GetPersistentEventCount();
            base.AddPersistentListener();
            this.RegisterPersistentListener(persistentEventCount, call);
            base.SetPersistentListenerState(persistentEventCount, callState);
        }

        protected override MethodInfo FindMethod_Impl(string name, object targetObj)
        {
            return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[0]);
        }

        private static BaseInvokableCall GetDelegate(UnityAction action)
        {
            return new InvokableCall(action);
        }

        internal override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
        {
            return new InvokableCall(target, theFunction);
        }

        public void Invoke()
        {
            base.Invoke(this.m_InvokeArray);
        }

        internal void RegisterPersistentListener(int index, UnityAction call)
        {
            if (call == null)
            {
                Debug.LogWarning("Registering a Listener requires an action");
            }
            else
            {
                base.RegisterPersistentListener(index, call.Target as Object, call.Method);
            }
        }

        public void RemoveListener(UnityAction call)
        {
            base.RemoveListener(call.Target, call.GetMethodInfo());
        }
    }
}

