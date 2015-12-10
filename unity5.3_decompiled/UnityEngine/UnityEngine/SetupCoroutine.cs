namespace UnityEngine
{
    using System;
    using System.Reflection;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    internal class SetupCoroutine
    {
        [RequiredByNativeCode]
        public static object InvokeMember(object behaviour, string name, object variable)
        {
            object[] args = null;
            if (variable != null)
            {
                args = new object[] { variable };
            }
            return behaviour.GetType().InvokeMember(name, BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, behaviour, args, null, null, null);
        }

        public static object InvokeStatic(Type klass, string name, object variable)
        {
            object[] args = null;
            if (variable != null)
            {
                args = new object[] { variable };
            }
            return klass.InvokeMember(name, BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, null, args, null, null, null);
        }
    }
}

