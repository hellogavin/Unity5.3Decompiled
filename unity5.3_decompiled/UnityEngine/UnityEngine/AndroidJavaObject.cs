namespace UnityEngine
{
    using System;
    using System.Text;

    public class AndroidJavaObject : IDisposable
    {
        private static bool enableDebugPrints;
        private bool m_disposed;
        protected IntPtr m_jclass;
        protected IntPtr m_jobject;
        private static AndroidJavaClass s_JavaLangClass;

        internal AndroidJavaObject()
        {
        }

        internal AndroidJavaObject(IntPtr jobject) : this()
        {
            if (jobject == IntPtr.Zero)
            {
                throw new Exception("JNI: Init'd AndroidJavaObject with null ptr!");
            }
            IntPtr objectClass = AndroidJNISafe.GetObjectClass(jobject);
            this.m_jobject = AndroidJNI.NewGlobalRef(jobject);
            this.m_jclass = AndroidJNI.NewGlobalRef(objectClass);
            AndroidJNISafe.DeleteLocalRef(objectClass);
        }

        public AndroidJavaObject(string className, params object[] args) : this()
        {
            this._AndroidJavaObject(className, args);
        }

        private void _AndroidJavaObject(string className, params object[] args)
        {
            this.DebugPrint("Creating AndroidJavaObject from " + className);
            if (args == null)
            {
                args = new object[1];
            }
            using (AndroidJavaObject obj2 = FindClass(className))
            {
                this.m_jclass = AndroidJNI.NewGlobalRef(obj2.GetRawObject());
                jvalue[] jvalueArray = AndroidJNIHelper.CreateJNIArgArray(args);
                try
                {
                    IntPtr constructorID = AndroidJNIHelper.GetConstructorID(this.m_jclass, args);
                    IntPtr ptr2 = AndroidJNISafe.NewObject(this.m_jclass, constructorID, jvalueArray);
                    this.m_jobject = AndroidJNI.NewGlobalRef(ptr2);
                    AndroidJNISafe.DeleteLocalRef(ptr2);
                }
                finally
                {
                    AndroidJNIHelper.DeleteJNIArgArray(args, jvalueArray);
                }
            }
        }

        protected void _Call(string methodName, params object[] args)
        {
            if (args == null)
            {
                args = new object[1];
            }
            IntPtr methodID = AndroidJNIHelper.GetMethodID(this.m_jclass, methodName, args, false);
            jvalue[] jvalueArray = AndroidJNIHelper.CreateJNIArgArray(args);
            try
            {
                AndroidJNISafe.CallVoidMethod(this.m_jobject, methodID, jvalueArray);
            }
            finally
            {
                AndroidJNIHelper.DeleteJNIArgArray(args, jvalueArray);
            }
        }

        protected ReturnType _Call<ReturnType>(string methodName, params object[] args)
        {
            ReturnType local;
            if (args == null)
            {
                args = new object[1];
            }
            IntPtr methodID = AndroidJNIHelper.GetMethodID<ReturnType>(this.m_jclass, methodName, args, false);
            jvalue[] jvalueArray = AndroidJNIHelper.CreateJNIArgArray(args);
            try
            {
                if (AndroidReflection.IsPrimitive(typeof(ReturnType)))
                {
                    if (typeof(ReturnType) == typeof(int))
                    {
                        return (ReturnType) AndroidJNISafe.CallIntMethod(this.m_jobject, methodID, jvalueArray);
                    }
                    if (typeof(ReturnType) == typeof(bool))
                    {
                        return (ReturnType) AndroidJNISafe.CallBooleanMethod(this.m_jobject, methodID, jvalueArray);
                    }
                    if (typeof(ReturnType) == typeof(byte))
                    {
                        return (ReturnType) AndroidJNISafe.CallByteMethod(this.m_jobject, methodID, jvalueArray);
                    }
                    if (typeof(ReturnType) == typeof(short))
                    {
                        return (ReturnType) AndroidJNISafe.CallShortMethod(this.m_jobject, methodID, jvalueArray);
                    }
                    if (typeof(ReturnType) == typeof(long))
                    {
                        return (ReturnType) AndroidJNISafe.CallLongMethod(this.m_jobject, methodID, jvalueArray);
                    }
                    if (typeof(ReturnType) == typeof(float))
                    {
                        return (ReturnType) AndroidJNISafe.CallFloatMethod(this.m_jobject, methodID, jvalueArray);
                    }
                    if (typeof(ReturnType) == typeof(double))
                    {
                        return (ReturnType) AndroidJNISafe.CallDoubleMethod(this.m_jobject, methodID, jvalueArray);
                    }
                    if (typeof(ReturnType) == typeof(char))
                    {
                        return (ReturnType) AndroidJNISafe.CallCharMethod(this.m_jobject, methodID, jvalueArray);
                    }
                }
                else
                {
                    if (typeof(ReturnType) == typeof(string))
                    {
                        return (ReturnType) AndroidJNISafe.CallStringMethod(this.m_jobject, methodID, jvalueArray);
                    }
                    if (typeof(ReturnType) == typeof(AndroidJavaClass))
                    {
                        return (ReturnType) AndroidJavaClassDeleteLocalRef(AndroidJNISafe.CallObjectMethod(this.m_jobject, methodID, jvalueArray));
                    }
                    if (typeof(ReturnType) == typeof(AndroidJavaObject))
                    {
                        return (ReturnType) AndroidJavaObjectDeleteLocalRef(AndroidJNISafe.CallObjectMethod(this.m_jobject, methodID, jvalueArray));
                    }
                    if (!AndroidReflection.IsAssignableFrom(typeof(Array), typeof(ReturnType)))
                    {
                        throw new Exception("JNI: Unknown return type '" + typeof(ReturnType) + "'");
                    }
                    return AndroidJNIHelper.ConvertFromJNIArray<ReturnType>(AndroidJNISafe.CallObjectMethod(this.m_jobject, methodID, jvalueArray));
                }
                local = default(ReturnType);
            }
            finally
            {
                AndroidJNIHelper.DeleteJNIArgArray(args, jvalueArray);
            }
            return local;
        }

        protected void _CallStatic(string methodName, params object[] args)
        {
            if (args == null)
            {
                args = new object[1];
            }
            IntPtr methodID = AndroidJNIHelper.GetMethodID(this.m_jclass, methodName, args, true);
            jvalue[] jvalueArray = AndroidJNIHelper.CreateJNIArgArray(args);
            try
            {
                AndroidJNISafe.CallStaticVoidMethod(this.m_jclass, methodID, jvalueArray);
            }
            finally
            {
                AndroidJNIHelper.DeleteJNIArgArray(args, jvalueArray);
            }
        }

        protected ReturnType _CallStatic<ReturnType>(string methodName, params object[] args)
        {
            ReturnType local;
            if (args == null)
            {
                args = new object[1];
            }
            IntPtr methodID = AndroidJNIHelper.GetMethodID<ReturnType>(this.m_jclass, methodName, args, true);
            jvalue[] jvalueArray = AndroidJNIHelper.CreateJNIArgArray(args);
            try
            {
                if (AndroidReflection.IsPrimitive(typeof(ReturnType)))
                {
                    if (typeof(ReturnType) == typeof(int))
                    {
                        return (ReturnType) AndroidJNISafe.CallStaticIntMethod(this.m_jclass, methodID, jvalueArray);
                    }
                    if (typeof(ReturnType) == typeof(bool))
                    {
                        return (ReturnType) AndroidJNISafe.CallStaticBooleanMethod(this.m_jclass, methodID, jvalueArray);
                    }
                    if (typeof(ReturnType) == typeof(byte))
                    {
                        return (ReturnType) AndroidJNISafe.CallStaticByteMethod(this.m_jclass, methodID, jvalueArray);
                    }
                    if (typeof(ReturnType) == typeof(short))
                    {
                        return (ReturnType) AndroidJNISafe.CallStaticShortMethod(this.m_jclass, methodID, jvalueArray);
                    }
                    if (typeof(ReturnType) == typeof(long))
                    {
                        return (ReturnType) AndroidJNISafe.CallStaticLongMethod(this.m_jclass, methodID, jvalueArray);
                    }
                    if (typeof(ReturnType) == typeof(float))
                    {
                        return (ReturnType) AndroidJNISafe.CallStaticFloatMethod(this.m_jclass, methodID, jvalueArray);
                    }
                    if (typeof(ReturnType) == typeof(double))
                    {
                        return (ReturnType) AndroidJNISafe.CallStaticDoubleMethod(this.m_jclass, methodID, jvalueArray);
                    }
                    if (typeof(ReturnType) == typeof(char))
                    {
                        return (ReturnType) AndroidJNISafe.CallStaticCharMethod(this.m_jclass, methodID, jvalueArray);
                    }
                }
                else
                {
                    if (typeof(ReturnType) == typeof(string))
                    {
                        return (ReturnType) AndroidJNISafe.CallStaticStringMethod(this.m_jclass, methodID, jvalueArray);
                    }
                    if (typeof(ReturnType) == typeof(AndroidJavaClass))
                    {
                        return (ReturnType) AndroidJavaClassDeleteLocalRef(AndroidJNISafe.CallStaticObjectMethod(this.m_jclass, methodID, jvalueArray));
                    }
                    if (typeof(ReturnType) == typeof(AndroidJavaObject))
                    {
                        return (ReturnType) AndroidJavaObjectDeleteLocalRef(AndroidJNISafe.CallStaticObjectMethod(this.m_jclass, methodID, jvalueArray));
                    }
                    if (!AndroidReflection.IsAssignableFrom(typeof(Array), typeof(ReturnType)))
                    {
                        throw new Exception("JNI: Unknown return type '" + typeof(ReturnType) + "'");
                    }
                    return AndroidJNIHelper.ConvertFromJNIArray<ReturnType>(AndroidJNISafe.CallStaticObjectMethod(this.m_jclass, methodID, jvalueArray));
                }
                local = default(ReturnType);
            }
            finally
            {
                AndroidJNIHelper.DeleteJNIArgArray(args, jvalueArray);
            }
            return local;
        }

        protected void _Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected FieldType _Get<FieldType>(string fieldName)
        {
            IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(this.m_jclass, fieldName, false);
            if (AndroidReflection.IsPrimitive(typeof(FieldType)))
            {
                if (typeof(FieldType) == typeof(int))
                {
                    return (FieldType) AndroidJNISafe.GetIntField(this.m_jobject, fieldID);
                }
                if (typeof(FieldType) == typeof(bool))
                {
                    return (FieldType) AndroidJNISafe.GetBooleanField(this.m_jobject, fieldID);
                }
                if (typeof(FieldType) == typeof(byte))
                {
                    return (FieldType) AndroidJNISafe.GetByteField(this.m_jobject, fieldID);
                }
                if (typeof(FieldType) == typeof(short))
                {
                    return (FieldType) AndroidJNISafe.GetShortField(this.m_jobject, fieldID);
                }
                if (typeof(FieldType) == typeof(long))
                {
                    return (FieldType) AndroidJNISafe.GetLongField(this.m_jobject, fieldID);
                }
                if (typeof(FieldType) == typeof(float))
                {
                    return (FieldType) AndroidJNISafe.GetFloatField(this.m_jobject, fieldID);
                }
                if (typeof(FieldType) == typeof(double))
                {
                    return (FieldType) AndroidJNISafe.GetDoubleField(this.m_jobject, fieldID);
                }
                if (typeof(FieldType) == typeof(char))
                {
                    return (FieldType) AndroidJNISafe.GetCharField(this.m_jobject, fieldID);
                }
                return default(FieldType);
            }
            if (typeof(FieldType) == typeof(string))
            {
                return (FieldType) AndroidJNISafe.GetStringField(this.m_jobject, fieldID);
            }
            if (typeof(FieldType) == typeof(AndroidJavaClass))
            {
                return (FieldType) AndroidJavaClassDeleteLocalRef(AndroidJNISafe.GetObjectField(this.m_jobject, fieldID));
            }
            if (typeof(FieldType) == typeof(AndroidJavaObject))
            {
                return (FieldType) AndroidJavaObjectDeleteLocalRef(AndroidJNISafe.GetObjectField(this.m_jobject, fieldID));
            }
            if (!AndroidReflection.IsAssignableFrom(typeof(Array), typeof(FieldType)))
            {
                throw new Exception("JNI: Unknown field type '" + typeof(FieldType) + "'");
            }
            return AndroidJNIHelper.ConvertFromJNIArray<FieldType>(AndroidJNISafe.GetObjectField(this.m_jobject, fieldID));
        }

        protected IntPtr _GetRawClass()
        {
            return this.m_jclass;
        }

        protected IntPtr _GetRawObject()
        {
            return this.m_jobject;
        }

        protected FieldType _GetStatic<FieldType>(string fieldName)
        {
            IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(this.m_jclass, fieldName, true);
            if (AndroidReflection.IsPrimitive(typeof(FieldType)))
            {
                if (typeof(FieldType) == typeof(int))
                {
                    return (FieldType) AndroidJNISafe.GetStaticIntField(this.m_jclass, fieldID);
                }
                if (typeof(FieldType) == typeof(bool))
                {
                    return (FieldType) AndroidJNISafe.GetStaticBooleanField(this.m_jclass, fieldID);
                }
                if (typeof(FieldType) == typeof(byte))
                {
                    return (FieldType) AndroidJNISafe.GetStaticByteField(this.m_jclass, fieldID);
                }
                if (typeof(FieldType) == typeof(short))
                {
                    return (FieldType) AndroidJNISafe.GetStaticShortField(this.m_jclass, fieldID);
                }
                if (typeof(FieldType) == typeof(long))
                {
                    return (FieldType) AndroidJNISafe.GetStaticLongField(this.m_jclass, fieldID);
                }
                if (typeof(FieldType) == typeof(float))
                {
                    return (FieldType) AndroidJNISafe.GetStaticFloatField(this.m_jclass, fieldID);
                }
                if (typeof(FieldType) == typeof(double))
                {
                    return (FieldType) AndroidJNISafe.GetStaticDoubleField(this.m_jclass, fieldID);
                }
                if (typeof(FieldType) == typeof(char))
                {
                    return (FieldType) AndroidJNISafe.GetStaticCharField(this.m_jclass, fieldID);
                }
                return default(FieldType);
            }
            if (typeof(FieldType) == typeof(string))
            {
                return (FieldType) AndroidJNISafe.GetStaticStringField(this.m_jclass, fieldID);
            }
            if (typeof(FieldType) == typeof(AndroidJavaClass))
            {
                return (FieldType) AndroidJavaClassDeleteLocalRef(AndroidJNISafe.GetStaticObjectField(this.m_jclass, fieldID));
            }
            if (typeof(FieldType) == typeof(AndroidJavaObject))
            {
                return (FieldType) AndroidJavaObjectDeleteLocalRef(AndroidJNISafe.GetStaticObjectField(this.m_jclass, fieldID));
            }
            if (!AndroidReflection.IsAssignableFrom(typeof(Array), typeof(FieldType)))
            {
                throw new Exception("JNI: Unknown field type '" + typeof(FieldType) + "'");
            }
            return AndroidJNIHelper.ConvertFromJNIArray<FieldType>(AndroidJNISafe.GetStaticObjectField(this.m_jclass, fieldID));
        }

        protected void _Set<FieldType>(string fieldName, FieldType val)
        {
            IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(this.m_jclass, fieldName, false);
            if (AndroidReflection.IsPrimitive(typeof(FieldType)))
            {
                if (typeof(FieldType) == typeof(int))
                {
                    AndroidJNISafe.SetIntField(this.m_jobject, fieldID, (int) val);
                }
                else if (typeof(FieldType) == typeof(bool))
                {
                    AndroidJNISafe.SetBooleanField(this.m_jobject, fieldID, (bool) val);
                }
                else if (typeof(FieldType) == typeof(byte))
                {
                    AndroidJNISafe.SetByteField(this.m_jobject, fieldID, (byte) val);
                }
                else if (typeof(FieldType) == typeof(short))
                {
                    AndroidJNISafe.SetShortField(this.m_jobject, fieldID, (short) val);
                }
                else if (typeof(FieldType) == typeof(long))
                {
                    AndroidJNISafe.SetLongField(this.m_jobject, fieldID, (long) val);
                }
                else if (typeof(FieldType) == typeof(float))
                {
                    AndroidJNISafe.SetFloatField(this.m_jobject, fieldID, (float) val);
                }
                else if (typeof(FieldType) == typeof(double))
                {
                    AndroidJNISafe.SetDoubleField(this.m_jobject, fieldID, (double) val);
                }
                else if (typeof(FieldType) == typeof(char))
                {
                    AndroidJNISafe.SetCharField(this.m_jobject, fieldID, (char) val);
                }
            }
            else if (typeof(FieldType) == typeof(string))
            {
                AndroidJNISafe.SetStringField(this.m_jobject, fieldID, (string) val);
            }
            else if (typeof(FieldType) == typeof(AndroidJavaClass))
            {
                AndroidJNISafe.SetObjectField(this.m_jobject, fieldID, ((AndroidJavaClass) val).m_jclass);
            }
            else if (typeof(FieldType) == typeof(AndroidJavaObject))
            {
                AndroidJNISafe.SetObjectField(this.m_jobject, fieldID, ((AndroidJavaObject) val).m_jobject);
            }
            else
            {
                if (!AndroidReflection.IsAssignableFrom(typeof(Array), typeof(FieldType)))
                {
                    throw new Exception("JNI: Unknown field type '" + typeof(FieldType) + "'");
                }
                IntPtr ptr2 = AndroidJNIHelper.ConvertToJNIArray((Array) val);
                AndroidJNISafe.SetObjectField(this.m_jclass, fieldID, ptr2);
            }
        }

        protected void _SetStatic<FieldType>(string fieldName, FieldType val)
        {
            IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(this.m_jclass, fieldName, true);
            if (AndroidReflection.IsPrimitive(typeof(FieldType)))
            {
                if (typeof(FieldType) == typeof(int))
                {
                    AndroidJNISafe.SetStaticIntField(this.m_jclass, fieldID, (int) val);
                }
                else if (typeof(FieldType) == typeof(bool))
                {
                    AndroidJNISafe.SetStaticBooleanField(this.m_jclass, fieldID, (bool) val);
                }
                else if (typeof(FieldType) == typeof(byte))
                {
                    AndroidJNISafe.SetStaticByteField(this.m_jclass, fieldID, (byte) val);
                }
                else if (typeof(FieldType) == typeof(short))
                {
                    AndroidJNISafe.SetStaticShortField(this.m_jclass, fieldID, (short) val);
                }
                else if (typeof(FieldType) == typeof(long))
                {
                    AndroidJNISafe.SetStaticLongField(this.m_jclass, fieldID, (long) val);
                }
                else if (typeof(FieldType) == typeof(float))
                {
                    AndroidJNISafe.SetStaticFloatField(this.m_jclass, fieldID, (float) val);
                }
                else if (typeof(FieldType) == typeof(double))
                {
                    AndroidJNISafe.SetStaticDoubleField(this.m_jclass, fieldID, (double) val);
                }
                else if (typeof(FieldType) == typeof(char))
                {
                    AndroidJNISafe.SetStaticCharField(this.m_jclass, fieldID, (char) val);
                }
            }
            else if (typeof(FieldType) == typeof(string))
            {
                AndroidJNISafe.SetStaticStringField(this.m_jclass, fieldID, (string) val);
            }
            else if (typeof(FieldType) == typeof(AndroidJavaClass))
            {
                AndroidJNISafe.SetStaticObjectField(this.m_jclass, fieldID, ((AndroidJavaClass) val).m_jclass);
            }
            else if (typeof(FieldType) == typeof(AndroidJavaObject))
            {
                AndroidJNISafe.SetStaticObjectField(this.m_jclass, fieldID, ((AndroidJavaObject) val).m_jobject);
            }
            else
            {
                if (!AndroidReflection.IsAssignableFrom(typeof(Array), typeof(FieldType)))
                {
                    throw new Exception("JNI: Unknown field type '" + typeof(FieldType) + "'");
                }
                IntPtr ptr2 = AndroidJNIHelper.ConvertToJNIArray((Array) val);
                AndroidJNISafe.SetStaticObjectField(this.m_jclass, fieldID, ptr2);
            }
        }

        internal static AndroidJavaClass AndroidJavaClassDeleteLocalRef(IntPtr jclass)
        {
            AndroidJavaClass class2;
            try
            {
                class2 = new AndroidJavaClass(jclass);
            }
            finally
            {
                AndroidJNISafe.DeleteLocalRef(jclass);
            }
            return class2;
        }

        internal static AndroidJavaObject AndroidJavaObjectDeleteLocalRef(IntPtr jobject)
        {
            AndroidJavaObject obj2;
            try
            {
                obj2 = new AndroidJavaObject(jobject);
            }
            finally
            {
                AndroidJNISafe.DeleteLocalRef(jobject);
            }
            return obj2;
        }

        public void Call(string methodName, params object[] args)
        {
            this._Call(methodName, args);
        }

        public ReturnType Call<ReturnType>(string methodName, params object[] args)
        {
            return this._Call<ReturnType>(methodName, args);
        }

        public void CallStatic(string methodName, params object[] args)
        {
            this._CallStatic(methodName, args);
        }

        public ReturnType CallStatic<ReturnType>(string methodName, params object[] args)
        {
            return this._CallStatic<ReturnType>(methodName, args);
        }

        protected void DebugPrint(string msg)
        {
            if (enableDebugPrints)
            {
                Debug.Log(msg);
            }
        }

        protected void DebugPrint(string call, string methodName, string signature, object[] args)
        {
            if (enableDebugPrints)
            {
                StringBuilder builder = new StringBuilder();
                foreach (object obj2 in args)
                {
                    builder.Append(", ");
                    builder.Append((obj2 != null) ? obj2.GetType().ToString() : "<null>");
                }
                Debug.Log(call + "(\"" + methodName + "\"" + builder.ToString() + ") = " + signature);
            }
        }

        public void Dispose()
        {
            this._Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.m_disposed)
            {
                this.m_disposed = true;
                AndroidJNISafe.DeleteGlobalRef(this.m_jobject);
                AndroidJNISafe.DeleteGlobalRef(this.m_jclass);
            }
        }

        ~AndroidJavaObject()
        {
            this.Dispose(true);
        }

        protected static AndroidJavaObject FindClass(string name)
        {
            object[] args = new object[] { name.Replace('/', '.') };
            return JavaLangClass.CallStatic<AndroidJavaObject>("forName", args);
        }

        public FieldType Get<FieldType>(string fieldName)
        {
            return this._Get<FieldType>(fieldName);
        }

        public IntPtr GetRawClass()
        {
            return this._GetRawClass();
        }

        public IntPtr GetRawObject()
        {
            return this._GetRawObject();
        }

        public FieldType GetStatic<FieldType>(string fieldName)
        {
            return this._GetStatic<FieldType>(fieldName);
        }

        public void Set<FieldType>(string fieldName, FieldType val)
        {
            this._Set<FieldType>(fieldName, val);
        }

        public void SetStatic<FieldType>(string fieldName, FieldType val)
        {
            this._SetStatic<FieldType>(fieldName, val);
        }

        protected static AndroidJavaClass JavaLangClass
        {
            get
            {
                if (s_JavaLangClass == null)
                {
                    s_JavaLangClass = new AndroidJavaClass(AndroidJNISafe.FindClass("java/lang/Class"));
                }
                return s_JavaLangClass;
            }
        }
    }
}

