namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Security;

    internal class GUIStateObjects
    {
        private static Dictionary<int, object> s_StateCache = new Dictionary<int, object>();

        [SecuritySafeCritical]
        internal static object GetStateObject(Type t, int controlID)
        {
            object obj2;
            if (!s_StateCache.TryGetValue(controlID, out obj2) || (obj2.GetType() != t))
            {
                obj2 = Activator.CreateInstance(t);
                s_StateCache[controlID] = obj2;
            }
            return obj2;
        }

        internal static object QueryStateObject(Type t, int controlID)
        {
            object o = s_StateCache[controlID];
            if (t.IsInstanceOfType(o))
            {
                return o;
            }
            return null;
        }
    }
}

