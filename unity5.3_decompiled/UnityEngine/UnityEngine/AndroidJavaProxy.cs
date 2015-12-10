namespace UnityEngine
{
    using System;
    using System.Reflection;

    public class AndroidJavaProxy
    {
        public readonly AndroidJavaClass javaInterface;

        public AndroidJavaProxy(string javaInterface) : this(new AndroidJavaClass(javaInterface))
        {
        }

        public AndroidJavaProxy(AndroidJavaClass javaInterface)
        {
            this.javaInterface = javaInterface;
        }

        public virtual AndroidJavaObject Invoke(string methodName, object[] args)
        {
            Exception inner = null;
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
            Type[] types = new Type[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                types[i] = (args[i] != null) ? args[i].GetType() : typeof(AndroidJavaObject);
            }
            try
            {
                MethodInfo info = base.GetType().GetMethod(methodName, bindingAttr, null, types, null);
                if (info != null)
                {
                    return _AndroidJNIHelper.Box(info.Invoke(this, args));
                }
            }
            catch (TargetInvocationException exception2)
            {
                inner = exception2.InnerException;
            }
            catch (Exception exception3)
            {
                inner = exception3;
            }
            string[] strArray = new string[args.Length];
            for (int j = 0; j < types.Length; j++)
            {
                strArray[j] = types[j].ToString();
            }
            if (inner != null)
            {
                object[] objArray1 = new object[] { base.GetType(), ".", methodName, "(", string.Join(",", strArray), ")" };
                throw new TargetInvocationException(string.Concat(objArray1), inner);
            }
            object[] objArray2 = new object[] { "No such proxy method: ", base.GetType(), ".", methodName, "(", string.Join(",", strArray), ")" };
            throw new Exception(string.Concat(objArray2));
        }

        public virtual AndroidJavaObject Invoke(string methodName, AndroidJavaObject[] javaArgs)
        {
            object[] args = new object[javaArgs.Length];
            for (int i = 0; i < javaArgs.Length; i++)
            {
                args[i] = _AndroidJNIHelper.Unbox(javaArgs[i]);
            }
            return this.Invoke(methodName, args);
        }
    }
}

