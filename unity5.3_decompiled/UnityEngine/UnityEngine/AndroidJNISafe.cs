namespace UnityEngine
{
    using System;

    internal class AndroidJNISafe
    {
        public static bool CallBooleanMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
        {
            bool flag;
            try
            {
                flag = AndroidJNI.CallBooleanMethod(obj, methodID, args);
            }
            finally
            {
                CheckException();
            }
            return flag;
        }

        public static byte CallByteMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
        {
            byte num;
            try
            {
                num = AndroidJNI.CallByteMethod(obj, methodID, args);
            }
            finally
            {
                CheckException();
            }
            return num;
        }

        public static char CallCharMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
        {
            char ch;
            try
            {
                ch = AndroidJNI.CallCharMethod(obj, methodID, args);
            }
            finally
            {
                CheckException();
            }
            return ch;
        }

        public static double CallDoubleMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
        {
            double num;
            try
            {
                num = AndroidJNI.CallDoubleMethod(obj, methodID, args);
            }
            finally
            {
                CheckException();
            }
            return num;
        }

        public static float CallFloatMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
        {
            float num;
            try
            {
                num = AndroidJNI.CallFloatMethod(obj, methodID, args);
            }
            finally
            {
                CheckException();
            }
            return num;
        }

        public static int CallIntMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
        {
            int num;
            try
            {
                num = AndroidJNI.CallIntMethod(obj, methodID, args);
            }
            finally
            {
                CheckException();
            }
            return num;
        }

        public static long CallLongMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
        {
            long num;
            try
            {
                num = AndroidJNI.CallLongMethod(obj, methodID, args);
            }
            finally
            {
                CheckException();
            }
            return num;
        }

        public static IntPtr CallObjectMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
        {
            IntPtr ptr;
            try
            {
                ptr = AndroidJNI.CallObjectMethod(obj, methodID, args);
            }
            finally
            {
                CheckException();
            }
            return ptr;
        }

        public static short CallShortMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
        {
            short num;
            try
            {
                num = AndroidJNI.CallShortMethod(obj, methodID, args);
            }
            finally
            {
                CheckException();
            }
            return num;
        }

        public static bool CallStaticBooleanMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
        {
            bool flag;
            try
            {
                flag = AndroidJNI.CallStaticBooleanMethod(clazz, methodID, args);
            }
            finally
            {
                CheckException();
            }
            return flag;
        }

        public static byte CallStaticByteMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
        {
            byte num;
            try
            {
                num = AndroidJNI.CallStaticByteMethod(clazz, methodID, args);
            }
            finally
            {
                CheckException();
            }
            return num;
        }

        public static char CallStaticCharMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
        {
            char ch;
            try
            {
                ch = AndroidJNI.CallStaticCharMethod(clazz, methodID, args);
            }
            finally
            {
                CheckException();
            }
            return ch;
        }

        public static double CallStaticDoubleMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
        {
            double num;
            try
            {
                num = AndroidJNI.CallStaticDoubleMethod(clazz, methodID, args);
            }
            finally
            {
                CheckException();
            }
            return num;
        }

        public static float CallStaticFloatMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
        {
            float num;
            try
            {
                num = AndroidJNI.CallStaticFloatMethod(clazz, methodID, args);
            }
            finally
            {
                CheckException();
            }
            return num;
        }

        public static int CallStaticIntMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
        {
            int num;
            try
            {
                num = AndroidJNI.CallStaticIntMethod(clazz, methodID, args);
            }
            finally
            {
                CheckException();
            }
            return num;
        }

        public static long CallStaticLongMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
        {
            long num;
            try
            {
                num = AndroidJNI.CallStaticLongMethod(clazz, methodID, args);
            }
            finally
            {
                CheckException();
            }
            return num;
        }

        public static IntPtr CallStaticObjectMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
        {
            IntPtr ptr;
            try
            {
                ptr = AndroidJNI.CallStaticObjectMethod(clazz, methodID, args);
            }
            finally
            {
                CheckException();
            }
            return ptr;
        }

        public static short CallStaticShortMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
        {
            short num;
            try
            {
                num = AndroidJNI.CallStaticShortMethod(clazz, methodID, args);
            }
            finally
            {
                CheckException();
            }
            return num;
        }

        public static string CallStaticStringMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
        {
            string str;
            try
            {
                str = AndroidJNI.CallStaticStringMethod(clazz, methodID, args);
            }
            finally
            {
                CheckException();
            }
            return str;
        }

        public static void CallStaticVoidMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
        {
            try
            {
                AndroidJNI.CallStaticVoidMethod(clazz, methodID, args);
            }
            finally
            {
                CheckException();
            }
        }

        public static string CallStringMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
        {
            string str;
            try
            {
                str = AndroidJNI.CallStringMethod(obj, methodID, args);
            }
            finally
            {
                CheckException();
            }
            return str;
        }

        public static void CallVoidMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
        {
            try
            {
                AndroidJNI.CallVoidMethod(obj, methodID, args);
            }
            finally
            {
                CheckException();
            }
        }

        public static void CheckException()
        {
            IntPtr ptr = AndroidJNI.ExceptionOccurred();
            if (ptr != IntPtr.Zero)
            {
                AndroidJNI.ExceptionClear();
                IntPtr clazz = AndroidJNI.FindClass("java/lang/Throwable");
                IntPtr ptr3 = AndroidJNI.FindClass("android/util/Log");
                try
                {
                    IntPtr methodID = AndroidJNI.GetMethodID(clazz, "toString", "()Ljava/lang/String;");
                    IntPtr ptr5 = AndroidJNI.GetStaticMethodID(ptr3, "getStackTraceString", "(Ljava/lang/Throwable;)Ljava/lang/String;");
                    string message = AndroidJNI.CallStringMethod(ptr, methodID, new jvalue[0]);
                    jvalue[] args = new jvalue[1];
                    args[0].l = ptr;
                    string javaStackTrace = AndroidJNI.CallStaticStringMethod(ptr3, ptr5, args);
                    throw new AndroidJavaException(message, javaStackTrace);
                }
                finally
                {
                    DeleteLocalRef(ptr);
                    DeleteLocalRef(clazz);
                    DeleteLocalRef(ptr3);
                }
            }
        }

        public static void DeleteGlobalRef(IntPtr globalref)
        {
            if (globalref != IntPtr.Zero)
            {
                AndroidJNI.DeleteGlobalRef(globalref);
            }
        }

        public static void DeleteLocalRef(IntPtr localref)
        {
            if (localref != IntPtr.Zero)
            {
                AndroidJNI.DeleteLocalRef(localref);
            }
        }

        public static IntPtr FindClass(string name)
        {
            IntPtr ptr;
            try
            {
                ptr = AndroidJNI.FindClass(name);
            }
            finally
            {
                CheckException();
            }
            return ptr;
        }

        public static bool[] FromBooleanArray(IntPtr array)
        {
            bool[] flagArray;
            try
            {
                flagArray = AndroidJNI.FromBooleanArray(array);
            }
            finally
            {
                CheckException();
            }
            return flagArray;
        }

        public static byte[] FromByteArray(IntPtr array)
        {
            byte[] buffer;
            try
            {
                buffer = AndroidJNI.FromByteArray(array);
            }
            finally
            {
                CheckException();
            }
            return buffer;
        }

        public static char[] FromCharArray(IntPtr array)
        {
            char[] chArray;
            try
            {
                chArray = AndroidJNI.FromCharArray(array);
            }
            finally
            {
                CheckException();
            }
            return chArray;
        }

        public static double[] FromDoubleArray(IntPtr array)
        {
            double[] numArray;
            try
            {
                numArray = AndroidJNI.FromDoubleArray(array);
            }
            finally
            {
                CheckException();
            }
            return numArray;
        }

        public static float[] FromFloatArray(IntPtr array)
        {
            float[] numArray;
            try
            {
                numArray = AndroidJNI.FromFloatArray(array);
            }
            finally
            {
                CheckException();
            }
            return numArray;
        }

        public static int[] FromIntArray(IntPtr array)
        {
            int[] numArray;
            try
            {
                numArray = AndroidJNI.FromIntArray(array);
            }
            finally
            {
                CheckException();
            }
            return numArray;
        }

        public static long[] FromLongArray(IntPtr array)
        {
            long[] numArray;
            try
            {
                numArray = AndroidJNI.FromLongArray(array);
            }
            finally
            {
                CheckException();
            }
            return numArray;
        }

        public static IntPtr[] FromObjectArray(IntPtr array)
        {
            IntPtr[] ptrArray;
            try
            {
                ptrArray = AndroidJNI.FromObjectArray(array);
            }
            finally
            {
                CheckException();
            }
            return ptrArray;
        }

        public static IntPtr FromReflectedField(IntPtr refField)
        {
            IntPtr ptr;
            try
            {
                ptr = AndroidJNI.FromReflectedField(refField);
            }
            finally
            {
                CheckException();
            }
            return ptr;
        }

        public static IntPtr FromReflectedMethod(IntPtr refMethod)
        {
            IntPtr ptr;
            try
            {
                ptr = AndroidJNI.FromReflectedMethod(refMethod);
            }
            finally
            {
                CheckException();
            }
            return ptr;
        }

        public static short[] FromShortArray(IntPtr array)
        {
            short[] numArray;
            try
            {
                numArray = AndroidJNI.FromShortArray(array);
            }
            finally
            {
                CheckException();
            }
            return numArray;
        }

        public static int GetArrayLength(IntPtr array)
        {
            int arrayLength;
            try
            {
                arrayLength = AndroidJNI.GetArrayLength(array);
            }
            finally
            {
                CheckException();
            }
            return arrayLength;
        }

        public static bool GetBooleanField(IntPtr obj, IntPtr fieldID)
        {
            bool booleanField;
            try
            {
                booleanField = AndroidJNI.GetBooleanField(obj, fieldID);
            }
            finally
            {
                CheckException();
            }
            return booleanField;
        }

        public static byte GetByteField(IntPtr obj, IntPtr fieldID)
        {
            byte byteField;
            try
            {
                byteField = AndroidJNI.GetByteField(obj, fieldID);
            }
            finally
            {
                CheckException();
            }
            return byteField;
        }

        public static char GetCharField(IntPtr obj, IntPtr fieldID)
        {
            char charField;
            try
            {
                charField = AndroidJNI.GetCharField(obj, fieldID);
            }
            finally
            {
                CheckException();
            }
            return charField;
        }

        public static double GetDoubleField(IntPtr obj, IntPtr fieldID)
        {
            double doubleField;
            try
            {
                doubleField = AndroidJNI.GetDoubleField(obj, fieldID);
            }
            finally
            {
                CheckException();
            }
            return doubleField;
        }

        public static IntPtr GetFieldID(IntPtr clazz, string name, string sig)
        {
            IntPtr ptr;
            try
            {
                ptr = AndroidJNI.GetFieldID(clazz, name, sig);
            }
            finally
            {
                CheckException();
            }
            return ptr;
        }

        public static float GetFloatField(IntPtr obj, IntPtr fieldID)
        {
            float floatField;
            try
            {
                floatField = AndroidJNI.GetFloatField(obj, fieldID);
            }
            finally
            {
                CheckException();
            }
            return floatField;
        }

        public static int GetIntField(IntPtr obj, IntPtr fieldID)
        {
            int intField;
            try
            {
                intField = AndroidJNI.GetIntField(obj, fieldID);
            }
            finally
            {
                CheckException();
            }
            return intField;
        }

        public static long GetLongField(IntPtr obj, IntPtr fieldID)
        {
            long longField;
            try
            {
                longField = AndroidJNI.GetLongField(obj, fieldID);
            }
            finally
            {
                CheckException();
            }
            return longField;
        }

        public static IntPtr GetMethodID(IntPtr obj, string name, string sig)
        {
            IntPtr ptr;
            try
            {
                ptr = AndroidJNI.GetMethodID(obj, name, sig);
            }
            finally
            {
                CheckException();
            }
            return ptr;
        }

        public static IntPtr GetObjectArrayElement(IntPtr array, int index)
        {
            IntPtr objectArrayElement;
            try
            {
                objectArrayElement = AndroidJNI.GetObjectArrayElement(array, index);
            }
            finally
            {
                CheckException();
            }
            return objectArrayElement;
        }

        public static IntPtr GetObjectClass(IntPtr ptr)
        {
            IntPtr objectClass;
            try
            {
                objectClass = AndroidJNI.GetObjectClass(ptr);
            }
            finally
            {
                CheckException();
            }
            return objectClass;
        }

        public static IntPtr GetObjectField(IntPtr obj, IntPtr fieldID)
        {
            IntPtr objectField;
            try
            {
                objectField = AndroidJNI.GetObjectField(obj, fieldID);
            }
            finally
            {
                CheckException();
            }
            return objectField;
        }

        public static short GetShortField(IntPtr obj, IntPtr fieldID)
        {
            short shortField;
            try
            {
                shortField = AndroidJNI.GetShortField(obj, fieldID);
            }
            finally
            {
                CheckException();
            }
            return shortField;
        }

        public static bool GetStaticBooleanField(IntPtr clazz, IntPtr fieldID)
        {
            bool staticBooleanField;
            try
            {
                staticBooleanField = AndroidJNI.GetStaticBooleanField(clazz, fieldID);
            }
            finally
            {
                CheckException();
            }
            return staticBooleanField;
        }

        public static byte GetStaticByteField(IntPtr clazz, IntPtr fieldID)
        {
            byte staticByteField;
            try
            {
                staticByteField = AndroidJNI.GetStaticByteField(clazz, fieldID);
            }
            finally
            {
                CheckException();
            }
            return staticByteField;
        }

        public static char GetStaticCharField(IntPtr clazz, IntPtr fieldID)
        {
            char staticCharField;
            try
            {
                staticCharField = AndroidJNI.GetStaticCharField(clazz, fieldID);
            }
            finally
            {
                CheckException();
            }
            return staticCharField;
        }

        public static double GetStaticDoubleField(IntPtr clazz, IntPtr fieldID)
        {
            double staticDoubleField;
            try
            {
                staticDoubleField = AndroidJNI.GetStaticDoubleField(clazz, fieldID);
            }
            finally
            {
                CheckException();
            }
            return staticDoubleField;
        }

        public static IntPtr GetStaticFieldID(IntPtr clazz, string name, string sig)
        {
            IntPtr ptr;
            try
            {
                ptr = AndroidJNI.GetStaticFieldID(clazz, name, sig);
            }
            finally
            {
                CheckException();
            }
            return ptr;
        }

        public static float GetStaticFloatField(IntPtr clazz, IntPtr fieldID)
        {
            float staticFloatField;
            try
            {
                staticFloatField = AndroidJNI.GetStaticFloatField(clazz, fieldID);
            }
            finally
            {
                CheckException();
            }
            return staticFloatField;
        }

        public static int GetStaticIntField(IntPtr clazz, IntPtr fieldID)
        {
            int staticIntField;
            try
            {
                staticIntField = AndroidJNI.GetStaticIntField(clazz, fieldID);
            }
            finally
            {
                CheckException();
            }
            return staticIntField;
        }

        public static long GetStaticLongField(IntPtr clazz, IntPtr fieldID)
        {
            long staticLongField;
            try
            {
                staticLongField = AndroidJNI.GetStaticLongField(clazz, fieldID);
            }
            finally
            {
                CheckException();
            }
            return staticLongField;
        }

        public static IntPtr GetStaticMethodID(IntPtr clazz, string name, string sig)
        {
            IntPtr ptr;
            try
            {
                ptr = AndroidJNI.GetStaticMethodID(clazz, name, sig);
            }
            finally
            {
                CheckException();
            }
            return ptr;
        }

        public static IntPtr GetStaticObjectField(IntPtr clazz, IntPtr fieldID)
        {
            IntPtr staticObjectField;
            try
            {
                staticObjectField = AndroidJNI.GetStaticObjectField(clazz, fieldID);
            }
            finally
            {
                CheckException();
            }
            return staticObjectField;
        }

        public static short GetStaticShortField(IntPtr clazz, IntPtr fieldID)
        {
            short staticShortField;
            try
            {
                staticShortField = AndroidJNI.GetStaticShortField(clazz, fieldID);
            }
            finally
            {
                CheckException();
            }
            return staticShortField;
        }

        public static string GetStaticStringField(IntPtr clazz, IntPtr fieldID)
        {
            string staticStringField;
            try
            {
                staticStringField = AndroidJNI.GetStaticStringField(clazz, fieldID);
            }
            finally
            {
                CheckException();
            }
            return staticStringField;
        }

        public static string GetStringField(IntPtr obj, IntPtr fieldID)
        {
            string stringField;
            try
            {
                stringField = AndroidJNI.GetStringField(obj, fieldID);
            }
            finally
            {
                CheckException();
            }
            return stringField;
        }

        public static string GetStringUTFChars(IntPtr str)
        {
            string stringUTFChars;
            try
            {
                stringUTFChars = AndroidJNI.GetStringUTFChars(str);
            }
            finally
            {
                CheckException();
            }
            return stringUTFChars;
        }

        public static IntPtr NewObject(IntPtr clazz, IntPtr methodID, jvalue[] args)
        {
            IntPtr ptr;
            try
            {
                ptr = AndroidJNI.NewObject(clazz, methodID, args);
            }
            finally
            {
                CheckException();
            }
            return ptr;
        }

        public static IntPtr NewStringUTF(string bytes)
        {
            IntPtr ptr;
            try
            {
                ptr = AndroidJNI.NewStringUTF(bytes);
            }
            finally
            {
                CheckException();
            }
            return ptr;
        }

        public static void SetBooleanField(IntPtr obj, IntPtr fieldID, bool val)
        {
            try
            {
                AndroidJNI.SetBooleanField(obj, fieldID, val);
            }
            finally
            {
                CheckException();
            }
        }

        public static void SetByteField(IntPtr obj, IntPtr fieldID, byte val)
        {
            try
            {
                AndroidJNI.SetByteField(obj, fieldID, val);
            }
            finally
            {
                CheckException();
            }
        }

        public static void SetCharField(IntPtr obj, IntPtr fieldID, char val)
        {
            try
            {
                AndroidJNI.SetCharField(obj, fieldID, val);
            }
            finally
            {
                CheckException();
            }
        }

        public static void SetDoubleField(IntPtr obj, IntPtr fieldID, double val)
        {
            try
            {
                AndroidJNI.SetDoubleField(obj, fieldID, val);
            }
            finally
            {
                CheckException();
            }
        }

        public static void SetFloatField(IntPtr obj, IntPtr fieldID, float val)
        {
            try
            {
                AndroidJNI.SetFloatField(obj, fieldID, val);
            }
            finally
            {
                CheckException();
            }
        }

        public static void SetIntField(IntPtr obj, IntPtr fieldID, int val)
        {
            try
            {
                AndroidJNI.SetIntField(obj, fieldID, val);
            }
            finally
            {
                CheckException();
            }
        }

        public static void SetLongField(IntPtr obj, IntPtr fieldID, long val)
        {
            try
            {
                AndroidJNI.SetLongField(obj, fieldID, val);
            }
            finally
            {
                CheckException();
            }
        }

        public static void SetObjectField(IntPtr obj, IntPtr fieldID, IntPtr val)
        {
            try
            {
                AndroidJNI.SetObjectField(obj, fieldID, val);
            }
            finally
            {
                CheckException();
            }
        }

        public static void SetShortField(IntPtr obj, IntPtr fieldID, short val)
        {
            try
            {
                AndroidJNI.SetShortField(obj, fieldID, val);
            }
            finally
            {
                CheckException();
            }
        }

        public static void SetStaticBooleanField(IntPtr clazz, IntPtr fieldID, bool val)
        {
            try
            {
                AndroidJNI.SetStaticBooleanField(clazz, fieldID, val);
            }
            finally
            {
                CheckException();
            }
        }

        public static void SetStaticByteField(IntPtr clazz, IntPtr fieldID, byte val)
        {
            try
            {
                AndroidJNI.SetStaticByteField(clazz, fieldID, val);
            }
            finally
            {
                CheckException();
            }
        }

        public static void SetStaticCharField(IntPtr clazz, IntPtr fieldID, char val)
        {
            try
            {
                AndroidJNI.SetStaticCharField(clazz, fieldID, val);
            }
            finally
            {
                CheckException();
            }
        }

        public static void SetStaticDoubleField(IntPtr clazz, IntPtr fieldID, double val)
        {
            try
            {
                AndroidJNI.SetStaticDoubleField(clazz, fieldID, val);
            }
            finally
            {
                CheckException();
            }
        }

        public static void SetStaticFloatField(IntPtr clazz, IntPtr fieldID, float val)
        {
            try
            {
                AndroidJNI.SetStaticFloatField(clazz, fieldID, val);
            }
            finally
            {
                CheckException();
            }
        }

        public static void SetStaticIntField(IntPtr clazz, IntPtr fieldID, int val)
        {
            try
            {
                AndroidJNI.SetStaticIntField(clazz, fieldID, val);
            }
            finally
            {
                CheckException();
            }
        }

        public static void SetStaticLongField(IntPtr clazz, IntPtr fieldID, long val)
        {
            try
            {
                AndroidJNI.SetStaticLongField(clazz, fieldID, val);
            }
            finally
            {
                CheckException();
            }
        }

        public static void SetStaticObjectField(IntPtr clazz, IntPtr fieldID, IntPtr val)
        {
            try
            {
                AndroidJNI.SetStaticObjectField(clazz, fieldID, val);
            }
            finally
            {
                CheckException();
            }
        }

        public static void SetStaticShortField(IntPtr clazz, IntPtr fieldID, short val)
        {
            try
            {
                AndroidJNI.SetStaticShortField(clazz, fieldID, val);
            }
            finally
            {
                CheckException();
            }
        }

        public static void SetStaticStringField(IntPtr clazz, IntPtr fieldID, string val)
        {
            try
            {
                AndroidJNI.SetStaticStringField(clazz, fieldID, val);
            }
            finally
            {
                CheckException();
            }
        }

        public static void SetStringField(IntPtr obj, IntPtr fieldID, string val)
        {
            try
            {
                AndroidJNI.SetStringField(obj, fieldID, val);
            }
            finally
            {
                CheckException();
            }
        }

        public static IntPtr ToBooleanArray(bool[] array)
        {
            IntPtr ptr;
            try
            {
                ptr = AndroidJNI.ToBooleanArray(array);
            }
            finally
            {
                CheckException();
            }
            return ptr;
        }

        public static IntPtr ToByteArray(byte[] array)
        {
            IntPtr ptr;
            try
            {
                ptr = AndroidJNI.ToByteArray(array);
            }
            finally
            {
                CheckException();
            }
            return ptr;
        }

        public static IntPtr ToCharArray(char[] array)
        {
            IntPtr ptr;
            try
            {
                ptr = AndroidJNI.ToCharArray(array);
            }
            finally
            {
                CheckException();
            }
            return ptr;
        }

        public static IntPtr ToDoubleArray(double[] array)
        {
            IntPtr ptr;
            try
            {
                ptr = AndroidJNI.ToDoubleArray(array);
            }
            finally
            {
                CheckException();
            }
            return ptr;
        }

        public static IntPtr ToFloatArray(float[] array)
        {
            IntPtr ptr;
            try
            {
                ptr = AndroidJNI.ToFloatArray(array);
            }
            finally
            {
                CheckException();
            }
            return ptr;
        }

        public static IntPtr ToIntArray(int[] array)
        {
            IntPtr ptr;
            try
            {
                ptr = AndroidJNI.ToIntArray(array);
            }
            finally
            {
                CheckException();
            }
            return ptr;
        }

        public static IntPtr ToLongArray(long[] array)
        {
            IntPtr ptr;
            try
            {
                ptr = AndroidJNI.ToLongArray(array);
            }
            finally
            {
                CheckException();
            }
            return ptr;
        }

        public static IntPtr ToObjectArray(IntPtr[] array)
        {
            IntPtr ptr;
            try
            {
                ptr = AndroidJNI.ToObjectArray(array);
            }
            finally
            {
                CheckException();
            }
            return ptr;
        }

        public static IntPtr ToObjectArray(IntPtr[] array, IntPtr type)
        {
            IntPtr ptr;
            try
            {
                ptr = AndroidJNI.ToObjectArray(array, type);
            }
            finally
            {
                CheckException();
            }
            return ptr;
        }

        public static IntPtr ToShortArray(short[] array)
        {
            IntPtr ptr;
            try
            {
                ptr = AndroidJNI.ToShortArray(array);
            }
            finally
            {
                CheckException();
            }
            return ptr;
        }
    }
}

