namespace UnityEditor
{
    using System;
    using System.Collections.Generic;

    internal class DebugUtils
    {
        internal static string ListToString<T>(IEnumerable<T> list)
        {
            if (list == null)
            {
                return "[null list]";
            }
            string str = "[";
            int num = 0;
            IEnumerator<T> enumerator = list.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    T current = enumerator.Current;
                    if (num != 0)
                    {
                        str = str + ", ";
                    }
                    if (current != null)
                    {
                        str = str + current.ToString();
                    }
                    else
                    {
                        str = str + "'null'";
                    }
                    num++;
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
            str = str + "]";
            if (num == 0)
            {
                return "[empty list]";
            }
            object[] objArray1 = new object[] { "(", num, ") ", str };
            return string.Concat(objArray1);
        }
    }
}

