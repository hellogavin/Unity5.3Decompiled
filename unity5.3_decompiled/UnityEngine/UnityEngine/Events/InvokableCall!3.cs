namespace UnityEngine.Events
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEngineInternal;

    internal class InvokableCall<T1, T2, T3> : BaseInvokableCall
    {
        protected event UnityAction<T1, T2, T3> Delegate;

        public InvokableCall(UnityAction<T1, T2, T3> action)
        {
            this.Delegate = (UnityAction<T1, T2, T3>) System.Delegate.Combine(this.Delegate, action);
        }

        public InvokableCall(object target, MethodInfo theFunction) : base(target, theFunction)
        {
            this.Delegate = (UnityAction<T1, T2, T3>) theFunction.CreateDelegate(typeof(UnityAction<T1, T2, T3>), target);
        }

        public override bool Find(object targetObj, MethodInfo method)
        {
            return ((this.Delegate.Target == targetObj) && (this.Delegate.GetMethodInfo() == method));
        }

        public override void Invoke(object[] args)
        {
            if (args.Length != 3)
            {
                throw new ArgumentException("Passed argument 'args' is invalid size. Expected size is 1");
            }
            BaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
            BaseInvokableCall.ThrowOnInvalidArg<T2>(args[1]);
            BaseInvokableCall.ThrowOnInvalidArg<T3>(args[2]);
            if (BaseInvokableCall.AllowInvoke(this.Delegate))
            {
                this.Delegate((T1) args[0], (T2) args[1], (T3) args[2]);
            }
        }
    }
}

