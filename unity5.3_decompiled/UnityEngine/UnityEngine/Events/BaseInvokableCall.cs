namespace UnityEngine.Events
{
    using System;
    using System.Reflection;
    using UnityEngine;

    internal abstract class BaseInvokableCall
    {
        protected BaseInvokableCall()
        {
        }

        protected BaseInvokableCall(object target, MethodInfo function)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (function == null)
            {
                throw new ArgumentNullException("function");
            }
        }

        protected static bool AllowInvoke(Delegate @delegate)
        {
            object target = @delegate.Target;
            if (target != null)
            {
                Object objA = target as Object;
                if (!object.ReferenceEquals(objA, null))
                {
                    return (objA != null);
                }
            }
            return true;
        }

        public abstract bool Find(object targetObj, MethodInfo method);
        public abstract void Invoke(object[] args);
        protected static void ThrowOnInvalidArg<T>(object arg)
        {
            if ((arg != null) && !(arg is T))
            {
                object[] args = new object[] { arg.GetType(), typeof(T) };
                throw new ArgumentException(UnityString.Format("Passed argument 'args[0]' is of the wrong type. Type:{0} Expected:{1}", args));
            }
        }
    }
}

