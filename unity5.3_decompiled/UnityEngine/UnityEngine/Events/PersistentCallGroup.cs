namespace UnityEngine.Events
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Serialization;

    [Serializable]
    internal class PersistentCallGroup
    {
        [SerializeField, FormerlySerializedAs("m_Listeners")]
        private List<PersistentCall> m_Calls = new List<PersistentCall>();

        public void AddListener()
        {
            this.m_Calls.Add(new PersistentCall());
        }

        public void AddListener(PersistentCall call)
        {
            this.m_Calls.Add(call);
        }

        public void Clear()
        {
            this.m_Calls.Clear();
        }

        public PersistentCall GetListener(int index)
        {
            return this.m_Calls[index];
        }

        public IEnumerable<PersistentCall> GetListeners()
        {
            return this.m_Calls;
        }

        public void Initialize(InvokableCallList invokableList, UnityEventBase unityEventBase)
        {
            foreach (PersistentCall call in this.m_Calls)
            {
                if (call.IsValid())
                {
                    BaseInvokableCall runtimeCall = call.GetRuntimeCall(unityEventBase);
                    if (runtimeCall != null)
                    {
                        invokableList.AddPersistentInvokableCall(runtimeCall);
                    }
                }
            }
        }

        public void RegisterBoolPersistentListener(int index, Object targetObj, bool argument, string methodName)
        {
            PersistentCall listener = this.GetListener(index);
            listener.RegisterPersistentListener(targetObj, methodName);
            listener.mode = PersistentListenerMode.Bool;
            listener.arguments.boolArgument = argument;
        }

        public void RegisterEventPersistentListener(int index, Object targetObj, string methodName)
        {
            PersistentCall listener = this.GetListener(index);
            listener.RegisterPersistentListener(targetObj, methodName);
            listener.mode = PersistentListenerMode.EventDefined;
        }

        public void RegisterFloatPersistentListener(int index, Object targetObj, float argument, string methodName)
        {
            PersistentCall listener = this.GetListener(index);
            listener.RegisterPersistentListener(targetObj, methodName);
            listener.mode = PersistentListenerMode.Float;
            listener.arguments.floatArgument = argument;
        }

        public void RegisterIntPersistentListener(int index, Object targetObj, int argument, string methodName)
        {
            PersistentCall listener = this.GetListener(index);
            listener.RegisterPersistentListener(targetObj, methodName);
            listener.mode = PersistentListenerMode.Int;
            listener.arguments.intArgument = argument;
        }

        public void RegisterObjectPersistentListener(int index, Object targetObj, Object argument, string methodName)
        {
            PersistentCall listener = this.GetListener(index);
            listener.RegisterPersistentListener(targetObj, methodName);
            listener.mode = PersistentListenerMode.Object;
            listener.arguments.unityObjectArgument = argument;
        }

        public void RegisterStringPersistentListener(int index, Object targetObj, string argument, string methodName)
        {
            PersistentCall listener = this.GetListener(index);
            listener.RegisterPersistentListener(targetObj, methodName);
            listener.mode = PersistentListenerMode.String;
            listener.arguments.stringArgument = argument;
        }

        public void RegisterVoidPersistentListener(int index, Object targetObj, string methodName)
        {
            PersistentCall listener = this.GetListener(index);
            listener.RegisterPersistentListener(targetObj, methodName);
            listener.mode = PersistentListenerMode.Void;
        }

        public void RemoveListener(int index)
        {
            this.m_Calls.RemoveAt(index);
        }

        public void RemoveListeners(Object target, string methodName)
        {
            List<PersistentCall> list = new List<PersistentCall>();
            for (int i = 0; i < this.m_Calls.Count; i++)
            {
                if ((this.m_Calls[i].target == target) && (this.m_Calls[i].methodName == methodName))
                {
                    list.Add(this.m_Calls[i]);
                }
            }
            this.m_Calls.RemoveAll(new Predicate<PersistentCall>(list.Contains));
        }

        public void UnregisterPersistentListener(int index)
        {
            this.GetListener(index).UnregisterPersistentListener();
        }

        public int Count
        {
            get
            {
                return this.m_Calls.Count;
            }
        }
    }
}

