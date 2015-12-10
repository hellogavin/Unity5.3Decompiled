namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    [UsedByNativeCode]
    public sealed class AndroidJNIHelper
    {
        private AndroidJNIHelper()
        {
        }

        public static ArrayType ConvertFromJNIArray<ArrayType>(IntPtr array)
        {
            return _AndroidJNIHelper.ConvertFromJNIArray<ArrayType>(array);
        }

        public static IntPtr ConvertToJNIArray(Array array)
        {
            return _AndroidJNIHelper.ConvertToJNIArray(array);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr CreateJavaProxy(AndroidJavaProxy proxy);
        public static IntPtr CreateJavaRunnable(AndroidJavaRunnable jrunnable)
        {
            return _AndroidJNIHelper.CreateJavaRunnable(jrunnable);
        }

        public static jvalue[] CreateJNIArgArray(object[] args)
        {
            return _AndroidJNIHelper.CreateJNIArgArray(args);
        }

        public static void DeleteJNIArgArray(object[] args, jvalue[] jniArgs)
        {
            _AndroidJNIHelper.DeleteJNIArgArray(args, jniArgs);
        }

        [ExcludeFromDocs]
        public static IntPtr GetConstructorID(IntPtr javaClass)
        {
            string signature = string.Empty;
            return GetConstructorID(javaClass, signature);
        }

        public static IntPtr GetConstructorID(IntPtr javaClass, [DefaultValue("\"\"")] string signature)
        {
            return _AndroidJNIHelper.GetConstructorID(javaClass, signature);
        }

        public static IntPtr GetConstructorID(IntPtr jclass, object[] args)
        {
            return _AndroidJNIHelper.GetConstructorID(jclass, args);
        }

        [ExcludeFromDocs]
        public static IntPtr GetFieldID(IntPtr javaClass, string fieldName)
        {
            bool isStatic = false;
            string signature = string.Empty;
            return GetFieldID(javaClass, fieldName, signature, isStatic);
        }

        public static IntPtr GetFieldID<FieldType>(IntPtr jclass, string fieldName, bool isStatic)
        {
            return _AndroidJNIHelper.GetFieldID<FieldType>(jclass, fieldName, isStatic);
        }

        [ExcludeFromDocs]
        public static IntPtr GetFieldID(IntPtr javaClass, string fieldName, string signature)
        {
            bool isStatic = false;
            return GetFieldID(javaClass, fieldName, signature, isStatic);
        }

        public static IntPtr GetFieldID(IntPtr javaClass, string fieldName, [DefaultValue("\"\"")] string signature, [DefaultValue("false")] bool isStatic)
        {
            return _AndroidJNIHelper.GetFieldID(javaClass, fieldName, signature, isStatic);
        }

        [ExcludeFromDocs]
        public static IntPtr GetMethodID(IntPtr javaClass, string methodName)
        {
            bool isStatic = false;
            string signature = string.Empty;
            return GetMethodID(javaClass, methodName, signature, isStatic);
        }

        [ExcludeFromDocs]
        public static IntPtr GetMethodID(IntPtr javaClass, string methodName, string signature)
        {
            bool isStatic = false;
            return GetMethodID(javaClass, methodName, signature, isStatic);
        }

        public static IntPtr GetMethodID(IntPtr javaClass, string methodName, [DefaultValue("\"\"")] string signature, [DefaultValue("false")] bool isStatic)
        {
            return _AndroidJNIHelper.GetMethodID(javaClass, methodName, signature, isStatic);
        }

        public static IntPtr GetMethodID(IntPtr jclass, string methodName, object[] args, bool isStatic)
        {
            return _AndroidJNIHelper.GetMethodID(jclass, methodName, args, isStatic);
        }

        public static IntPtr GetMethodID<ReturnType>(IntPtr jclass, string methodName, object[] args, bool isStatic)
        {
            return _AndroidJNIHelper.GetMethodID<ReturnType>(jclass, methodName, args, isStatic);
        }

        public static string GetSignature(object obj)
        {
            return _AndroidJNIHelper.GetSignature(obj);
        }

        public static string GetSignature(object[] args)
        {
            return _AndroidJNIHelper.GetSignature(args);
        }

        public static string GetSignature<ReturnType>(object[] args)
        {
            return _AndroidJNIHelper.GetSignature<ReturnType>(args);
        }

        public static bool debug { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

