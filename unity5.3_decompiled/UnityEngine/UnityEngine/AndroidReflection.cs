namespace UnityEngine
{
    using System;

    internal class AndroidReflection
    {
        private const string RELECTION_HELPER_CLASS_NAME = "com/unity3d/player/ReflectionHelper";
        private static IntPtr s_ReflectionHelperClass = AndroidJNI.NewGlobalRef(AndroidJNISafe.FindClass("com/unity3d/player/ReflectionHelper"));
        private static IntPtr s_ReflectionHelperGetConstructorID = GetStaticMethodID("com/unity3d/player/ReflectionHelper", "getConstructorID", "(Ljava/lang/Class;Ljava/lang/String;)Ljava/lang/reflect/Constructor;");
        private static IntPtr s_ReflectionHelperGetFieldID = GetStaticMethodID("com/unity3d/player/ReflectionHelper", "getFieldID", "(Ljava/lang/Class;Ljava/lang/String;Ljava/lang/String;Z)Ljava/lang/reflect/Field;");
        private static IntPtr s_ReflectionHelperGetMethodID = GetStaticMethodID("com/unity3d/player/ReflectionHelper", "getMethodID", "(Ljava/lang/Class;Ljava/lang/String;Ljava/lang/String;Z)Ljava/lang/reflect/Method;");
        private static IntPtr s_ReflectionHelperNewProxyInstance = GetStaticMethodID("com/unity3d/player/ReflectionHelper", "newProxyInstance", "(ILjava/lang/Class;)Ljava/lang/Object;");

        public static IntPtr GetConstructorMember(IntPtr jclass, string signature)
        {
            IntPtr ptr;
            jvalue[] args = new jvalue[2];
            try
            {
                args[0].l = jclass;
                args[1].l = AndroidJNISafe.NewStringUTF(signature);
                ptr = AndroidJNISafe.CallStaticObjectMethod(s_ReflectionHelperClass, s_ReflectionHelperGetConstructorID, args);
            }
            finally
            {
                AndroidJNISafe.DeleteLocalRef(args[1].l);
            }
            return ptr;
        }

        public static IntPtr GetFieldMember(IntPtr jclass, string fieldName, string signature, bool isStatic)
        {
            IntPtr ptr;
            jvalue[] args = new jvalue[4];
            try
            {
                args[0].l = jclass;
                args[1].l = AndroidJNISafe.NewStringUTF(fieldName);
                args[2].l = AndroidJNISafe.NewStringUTF(signature);
                args[3].z = isStatic;
                ptr = AndroidJNISafe.CallStaticObjectMethod(s_ReflectionHelperClass, s_ReflectionHelperGetFieldID, args);
            }
            finally
            {
                AndroidJNISafe.DeleteLocalRef(args[1].l);
                AndroidJNISafe.DeleteLocalRef(args[2].l);
            }
            return ptr;
        }

        public static IntPtr GetMethodMember(IntPtr jclass, string methodName, string signature, bool isStatic)
        {
            IntPtr ptr;
            jvalue[] args = new jvalue[4];
            try
            {
                args[0].l = jclass;
                args[1].l = AndroidJNISafe.NewStringUTF(methodName);
                args[2].l = AndroidJNISafe.NewStringUTF(signature);
                args[3].z = isStatic;
                ptr = AndroidJNISafe.CallStaticObjectMethod(s_ReflectionHelperClass, s_ReflectionHelperGetMethodID, args);
            }
            finally
            {
                AndroidJNISafe.DeleteLocalRef(args[1].l);
                AndroidJNISafe.DeleteLocalRef(args[2].l);
            }
            return ptr;
        }

        private static IntPtr GetStaticMethodID(string clazz, string methodName, string signature)
        {
            IntPtr ptr2;
            IntPtr ptr = AndroidJNISafe.FindClass(clazz);
            try
            {
                ptr2 = AndroidJNISafe.GetStaticMethodID(ptr, methodName, signature);
            }
            finally
            {
                AndroidJNISafe.DeleteLocalRef(ptr);
            }
            return ptr2;
        }

        public static bool IsAssignableFrom(Type t, Type from)
        {
            return t.IsAssignableFrom(from);
        }

        public static bool IsPrimitive(Type t)
        {
            return t.IsPrimitive;
        }

        public static IntPtr NewProxyInstance(int delegateHandle, IntPtr interfaze)
        {
            jvalue[] args = new jvalue[2];
            args[0].i = delegateHandle;
            args[1].l = interfaze;
            return AndroidJNISafe.CallStaticObjectMethod(s_ReflectionHelperClass, s_ReflectionHelperNewProxyInstance, args);
        }
    }
}

