namespace UnityEngine.Events
{
    using System;
    using System.Reflection;
    using UnityEngine;
    using UnityEngine.Serialization;

    [Serializable]
    public abstract class UnityEventBase : ISerializationCallbackReceiver
    {
        private InvokableCallList m_Calls = new InvokableCallList();
        private bool m_CallsDirty = true;
        [SerializeField, FormerlySerializedAs("m_PersistentListeners")]
        private PersistentCallGroup m_PersistentCalls = new PersistentCallGroup();
        [SerializeField]
        private string m_TypeName;

        protected UnityEventBase()
        {
            this.m_TypeName = base.GetType().AssemblyQualifiedName;
        }

        internal void AddBoolPersistentListener(UnityAction<bool> call, bool argument)
        {
            int persistentEventCount = this.GetPersistentEventCount();
            this.AddPersistentListener();
            this.RegisterBoolPersistentListener(persistentEventCount, call, argument);
        }

        internal void AddCall(BaseInvokableCall call)
        {
            this.m_Calls.AddListener(call);
        }

        internal void AddFloatPersistentListener(UnityAction<float> call, float argument)
        {
            int persistentEventCount = this.GetPersistentEventCount();
            this.AddPersistentListener();
            this.RegisterFloatPersistentListener(persistentEventCount, call, argument);
        }

        internal void AddIntPersistentListener(UnityAction<int> call, int argument)
        {
            int persistentEventCount = this.GetPersistentEventCount();
            this.AddPersistentListener();
            this.RegisterIntPersistentListener(persistentEventCount, call, argument);
        }

        protected void AddListener(object targetObj, MethodInfo method)
        {
            this.m_Calls.AddListener(this.GetDelegate(targetObj, method));
        }

        internal void AddObjectPersistentListener<T>(UnityAction<T> call, T argument) where T: Object
        {
            int persistentEventCount = this.GetPersistentEventCount();
            this.AddPersistentListener();
            this.RegisterObjectPersistentListener<T>(persistentEventCount, call, argument);
        }

        internal void AddPersistentListener()
        {
            this.m_PersistentCalls.AddListener();
        }

        internal void AddStringPersistentListener(UnityAction<string> call, string argument)
        {
            int persistentEventCount = this.GetPersistentEventCount();
            this.AddPersistentListener();
            this.RegisterStringPersistentListener(persistentEventCount, call, argument);
        }

        internal void AddVoidPersistentListener(UnityAction call)
        {
            int persistentEventCount = this.GetPersistentEventCount();
            this.AddPersistentListener();
            this.RegisterVoidPersistentListener(persistentEventCount, call);
        }

        private void DirtyPersistentCalls()
        {
            this.m_Calls.ClearPersistent();
            this.m_CallsDirty = true;
        }

        internal MethodInfo FindMethod(PersistentCall call)
        {
            Type argumentType = typeof(Object);
            if (!string.IsNullOrEmpty(call.arguments.unityObjectArgumentAssemblyTypeName))
            {
                Type type = Type.GetType(call.arguments.unityObjectArgumentAssemblyTypeName, false);
                if (type != null)
                {
                    argumentType = type;
                }
                else
                {
                    argumentType = typeof(Object);
                }
            }
            return this.FindMethod(call.methodName, call.target, call.mode, argumentType);
        }

        internal MethodInfo FindMethod(string name, object listener, PersistentListenerMode mode, Type argumentType)
        {
            Type expressionStack_C9_0;
            int expressionStack_C9_1;
            Type[] expressionStack_C9_2;
            Type[] expressionStack_C9_3;
            string expressionStack_C9_4;
            object expressionStack_C9_5;
            switch (mode)
            {
                case PersistentListenerMode.EventDefined:
                    return this.FindMethod_Impl(name, listener);

                case PersistentListenerMode.Void:
                    return GetValidMethodInfo(listener, name, new Type[0]);

                case PersistentListenerMode.Object:
                {
                    int expressionStack_BE_1;
                    Type[] expressionStack_BE_2;
                    Type[] expressionStack_BE_3;
                    string expressionStack_BE_4;
                    object expressionStack_BE_5;
                    Type[] typeArray10 = new Type[1];
                    if (argumentType != null)
                    {
                        expressionStack_C9_5 = listener;
                        expressionStack_C9_4 = name;
                        expressionStack_C9_3 = typeArray10;
                        expressionStack_C9_2 = typeArray10;
                        expressionStack_C9_1 = 0;
                        expressionStack_C9_0 = argumentType;
                        break;
                    }
                    else
                    {
                        expressionStack_BE_5 = listener;
                        expressionStack_BE_4 = name;
                        expressionStack_BE_3 = typeArray10;
                        expressionStack_BE_2 = typeArray10;
                        expressionStack_BE_1 = 0;
                        Type expressionStack_BE_0 = argumentType;
                    }
                    expressionStack_C9_5 = expressionStack_BE_5;
                    expressionStack_C9_4 = expressionStack_BE_4;
                    expressionStack_C9_3 = expressionStack_BE_3;
                    expressionStack_C9_2 = expressionStack_BE_2;
                    expressionStack_C9_1 = expressionStack_BE_1;
                    expressionStack_C9_0 = typeof(Object);
                    break;
                }
                case PersistentListenerMode.Int:
                {
                    Type[] argumentTypes = new Type[] { typeof(int) };
                    return GetValidMethodInfo(listener, name, argumentTypes);
                }
                case PersistentListenerMode.Float:
                {
                    Type[] typeArray6 = new Type[] { typeof(float) };
                    return GetValidMethodInfo(listener, name, typeArray6);
                }
                case PersistentListenerMode.String:
                {
                    Type[] typeArray9 = new Type[] { typeof(string) };
                    return GetValidMethodInfo(listener, name, typeArray9);
                }
                case PersistentListenerMode.Bool:
                {
                    Type[] typeArray8 = new Type[] { typeof(bool) };
                    return GetValidMethodInfo(listener, name, typeArray8);
                }
                default:
                    return null;
            }
            expressionStack_C9_2[expressionStack_C9_1] = expressionStack_C9_0;
            return GetValidMethodInfo(expressionStack_C9_5, expressionStack_C9_4, expressionStack_C9_3);
        }

        protected abstract MethodInfo FindMethod_Impl(string name, object targetObj);
        internal abstract BaseInvokableCall GetDelegate(object target, MethodInfo theFunction);
        public int GetPersistentEventCount()
        {
            return this.m_PersistentCalls.Count;
        }

        public string GetPersistentMethodName(int index)
        {
            PersistentCall listener = this.m_PersistentCalls.GetListener(index);
            return ((listener == null) ? string.Empty : listener.methodName);
        }

        public Object GetPersistentTarget(int index)
        {
            PersistentCall listener = this.m_PersistentCalls.GetListener(index);
            return ((listener == null) ? null : listener.target);
        }

        public static MethodInfo GetValidMethodInfo(object obj, string functionName, Type[] argumentTypes)
        {
            for (Type type = obj.GetType(); (type != typeof(object)) && (type != null); type = type.BaseType)
            {
                MethodInfo info = type.GetMethod(functionName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, argumentTypes, null);
                if (info != null)
                {
                    ParameterInfo[] parameters = info.GetParameters();
                    bool flag = true;
                    int index = 0;
                    foreach (ParameterInfo info2 in parameters)
                    {
                        Type type2 = argumentTypes[index];
                        Type parameterType = info2.ParameterType;
                        flag = type2.IsPrimitive == parameterType.IsPrimitive;
                        if (!flag)
                        {
                            break;
                        }
                        index++;
                    }
                    if (flag)
                    {
                        return info;
                    }
                }
            }
            return null;
        }

        protected void Invoke(object[] parameters)
        {
            this.RebuildPersistentCallsIfNeeded();
            this.m_Calls.Invoke(parameters);
        }

        private void RebuildPersistentCallsIfNeeded()
        {
            if (this.m_CallsDirty)
            {
                this.m_PersistentCalls.Initialize(this.m_Calls, this);
                this.m_CallsDirty = false;
            }
        }

        internal void RegisterBoolPersistentListener(int index, UnityAction<bool> call, bool argument)
        {
            if (call == null)
            {
                Debug.LogWarning("Registering a Listener requires an action");
            }
            else if (this.ValidateRegistration(call.Method, call.Target, PersistentListenerMode.Bool))
            {
                this.m_PersistentCalls.RegisterBoolPersistentListener(index, call.Target as Object, argument, call.Method.Name);
                this.DirtyPersistentCalls();
            }
        }

        internal void RegisterFloatPersistentListener(int index, UnityAction<float> call, float argument)
        {
            if (call == null)
            {
                Debug.LogWarning("Registering a Listener requires an action");
            }
            else if (this.ValidateRegistration(call.Method, call.Target, PersistentListenerMode.Float))
            {
                this.m_PersistentCalls.RegisterFloatPersistentListener(index, call.Target as Object, argument, call.Method.Name);
                this.DirtyPersistentCalls();
            }
        }

        internal void RegisterIntPersistentListener(int index, UnityAction<int> call, int argument)
        {
            if (call == null)
            {
                Debug.LogWarning("Registering a Listener requires an action");
            }
            else if (this.ValidateRegistration(call.Method, call.Target, PersistentListenerMode.Int))
            {
                this.m_PersistentCalls.RegisterIntPersistentListener(index, call.Target as Object, argument, call.Method.Name);
                this.DirtyPersistentCalls();
            }
        }

        internal void RegisterObjectPersistentListener<T>(int index, UnityAction<T> call, T argument) where T: Object
        {
            if (call == null)
            {
                throw new ArgumentNullException("call", "Registering a Listener requires a non null call");
            }
            if (this.ValidateRegistration(call.Method, call.Target, PersistentListenerMode.Object, (argument != null) ? argument.GetType() : typeof(Object)))
            {
                this.m_PersistentCalls.RegisterObjectPersistentListener(index, call.Target as Object, argument, call.Method.Name);
                this.DirtyPersistentCalls();
            }
        }

        protected void RegisterPersistentListener(int index, object targetObj, MethodInfo method)
        {
            if (this.ValidateRegistration(method, targetObj, PersistentListenerMode.EventDefined))
            {
                this.m_PersistentCalls.RegisterEventPersistentListener(index, targetObj as Object, method.Name);
                this.DirtyPersistentCalls();
            }
        }

        internal void RegisterStringPersistentListener(int index, UnityAction<string> call, string argument)
        {
            if (call == null)
            {
                Debug.LogWarning("Registering a Listener requires an action");
            }
            else if (this.ValidateRegistration(call.Method, call.Target, PersistentListenerMode.String))
            {
                this.m_PersistentCalls.RegisterStringPersistentListener(index, call.Target as Object, argument, call.Method.Name);
                this.DirtyPersistentCalls();
            }
        }

        internal void RegisterVoidPersistentListener(int index, UnityAction call)
        {
            if (call == null)
            {
                Debug.LogWarning("Registering a Listener requires an action");
            }
            else if (this.ValidateRegistration(call.Method, call.Target, PersistentListenerMode.Void))
            {
                this.m_PersistentCalls.RegisterVoidPersistentListener(index, call.Target as Object, call.Method.Name);
                this.DirtyPersistentCalls();
            }
        }

        public void RemoveAllListeners()
        {
            this.m_Calls.Clear();
        }

        protected void RemoveListener(object targetObj, MethodInfo method)
        {
            this.m_Calls.RemoveListener(targetObj, method);
        }

        internal void RemovePersistentListener(int index)
        {
            this.m_PersistentCalls.RemoveListener(index);
            this.DirtyPersistentCalls();
        }

        internal void RemovePersistentListener(Object target, MethodInfo method)
        {
            if (((method != null) && !method.IsStatic) && ((target != null) && (target.GetInstanceID() != 0)))
            {
                this.m_PersistentCalls.RemoveListeners(target, method.Name);
                this.DirtyPersistentCalls();
            }
        }

        public void SetPersistentListenerState(int index, UnityEventCallState state)
        {
            PersistentCall listener = this.m_PersistentCalls.GetListener(index);
            if (listener != null)
            {
                listener.callState = state;
            }
            this.DirtyPersistentCalls();
        }

        public override string ToString()
        {
            return (base.ToString() + " " + base.GetType().FullName);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.DirtyPersistentCalls();
            this.m_TypeName = base.GetType().AssemblyQualifiedName;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        internal void UnregisterPersistentListener(int index)
        {
            this.m_PersistentCalls.UnregisterPersistentListener(index);
            this.DirtyPersistentCalls();
        }

        protected bool ValidateRegistration(MethodInfo method, object targetObj, PersistentListenerMode mode)
        {
            return this.ValidateRegistration(method, targetObj, mode, typeof(Object));
        }

        protected bool ValidateRegistration(MethodInfo method, object targetObj, PersistentListenerMode mode, Type argumentType)
        {
            if (method == null)
            {
                object[] args = new object[] { targetObj };
                throw new ArgumentNullException("method", UnityString.Format("Can not register null method on {0} for callback!", args));
            }
            Object obj2 = targetObj as Object;
            if ((obj2 == null) || (obj2.GetInstanceID() == 0))
            {
                object[] objArray2 = new object[] { method.Name, targetObj, (targetObj != null) ? targetObj.GetType().ToString() : "null" };
                throw new ArgumentException(UnityString.Format("Could not register callback {0} on {1}. The class {2} does not derive from UnityEngine.Object", objArray2));
            }
            if (method.IsStatic)
            {
                object[] objArray3 = new object[] { method, base.GetType() };
                throw new ArgumentException(UnityString.Format("Could not register listener {0} on {1} static functions are not supported.", objArray3));
            }
            if (this.FindMethod(method.Name, targetObj, mode, argumentType) == null)
            {
                object[] objArray4 = new object[] { targetObj, method, base.GetType() };
                Debug.LogWarning(UnityString.Format("Could not register listener {0}.{1} on {2} the method could not be found.", objArray4));
                return false;
            }
            return true;
        }
    }
}

