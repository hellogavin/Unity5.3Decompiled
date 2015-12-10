namespace UnityEngine.Events
{
    using System;
    using System.Reflection;
    using UnityEngine;
    using UnityEngine.Scripting;
    using UnityEngineInternal;

    [Serializable]
    public abstract class UnityEvent<T0, T1> : UnityEventBase
    {
        private readonly object[] m_InvokeArray;

        [RequiredByNativeCode]
        public UnityEvent()
        {
            this.m_InvokeArray = new object[2];
        }

        public void AddListener(UnityAction<T0, T1> call)
        {
            base.AddCall(UnityEvent<T0, T1>.GetDelegate(call));
        }

        internal void AddPersistentListener(UnityAction<T0, T1> call)
        {
            this.AddPersistentListener(call, UnityEventCallState.RuntimeOnly);
        }

        internal void AddPersistentListener(UnityAction<T0, T1> call, UnityEventCallState callState)
        {
            int persistentEventCount = base.GetPersistentEventCount();
            base.AddPersistentListener();
            this.RegisterPersistentListener(persistentEventCount, call);
            base.SetPersistentListenerState(persistentEventCount, callState);
        }

        protected override MethodInfo FindMethod_Impl(string name, object targetObj)
        {
            Type[] argumentTypes = new Type[] { typeof(T0), typeof(T1) };
            return UnityEventBase.GetValidMethodInfo(targetObj, name, argumentTypes);
        }

        private static BaseInvokableCall GetDelegate(UnityAction<T0, T1> action)
        {
            return new InvokableCall<T0, T1>(action);
        }

        internal override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
        {
            return new InvokableCall<T0, T1>(target, theFunction);
        }

        public void Invoke(T0 arg0, T1 arg1)
        {
            this.m_InvokeArray[0] = arg0;
            this.m_InvokeArray[1] = arg1;
            base.Invoke(this.m_InvokeArray);
        }

        internal void RegisterPersistentListener(int index, UnityAction<T0, T1> call)
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

        public void RemoveListener(UnityAction<T0, T1> call)
        {
            base.RemoveListener(call.Target, call.GetMethodInfo());
        }
    }
}

