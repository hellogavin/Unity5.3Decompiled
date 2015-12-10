namespace UnityEngine
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    public class MonoBehaviour : Behaviour
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern MonoBehaviour();
        public void CancelInvoke()
        {
            this.Internal_CancelInvokeAll();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void CancelInvoke(string methodName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_CancelInvokeAll();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool Internal_IsInvokingAll();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Invoke(string methodName, float time);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InvokeRepeating(string methodName, float time, float repeatRate);
        public bool IsInvoking()
        {
            return this.Internal_IsInvokingAll();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool IsInvoking(string methodName);
        public static void print(object message)
        {
            Debug.Log(message);
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return this.StartCoroutine_Auto(routine);
        }

        [ExcludeFromDocs]
        public Coroutine StartCoroutine(string methodName)
        {
            object obj2 = null;
            return this.StartCoroutine(methodName, obj2);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Coroutine StartCoroutine_Auto(IEnumerator routine);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void StopAllCoroutines();
        public void StopCoroutine(IEnumerator routine)
        {
            this.StopCoroutineViaEnumerator_Auto(routine);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void StopCoroutine(string methodName);
        public void StopCoroutine(Coroutine routine)
        {
            this.StopCoroutine_Auto(routine);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void StopCoroutine_Auto(Coroutine routine);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void StopCoroutineViaEnumerator_Auto(IEnumerator routine);

        public bool useGUILayout { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

