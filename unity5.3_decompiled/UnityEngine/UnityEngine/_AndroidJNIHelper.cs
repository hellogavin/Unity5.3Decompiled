namespace UnityEngine
{
    using System;
    using System.Text;
    using UnityEngine.Scripting;

    [UsedByNativeCode]
    internal sealed class _AndroidJNIHelper
    {
        public static AndroidJavaObject Box(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (AndroidReflection.IsPrimitive(obj.GetType()))
            {
                if (obj is int)
                {
                    return new AndroidJavaObject("java.lang.Integer", new object[] { (int) obj });
                }
                if (obj is bool)
                {
                    return new AndroidJavaObject("java.lang.Boolean", new object[] { (bool) obj });
                }
                if (obj is byte)
                {
                    return new AndroidJavaObject("java.lang.Byte", new object[] { (byte) obj });
                }
                if (obj is short)
                {
                    return new AndroidJavaObject("java.lang.Short", new object[] { (short) obj });
                }
                if (obj is long)
                {
                    return new AndroidJavaObject("java.lang.Long", new object[] { (long) obj });
                }
                if (obj is float)
                {
                    return new AndroidJavaObject("java.lang.Float", new object[] { (float) obj });
                }
                if (obj is double)
                {
                    return new AndroidJavaObject("java.lang.Double", new object[] { (double) obj });
                }
                if (!(obj is char))
                {
                    throw new Exception("JNI; Unknown argument type '" + obj.GetType() + "'");
                }
                return new AndroidJavaObject("java.lang.Character", new object[] { (char) obj });
            }
            if (obj is string)
            {
                return new AndroidJavaObject("java.lang.String", new object[] { (string) obj });
            }
            if (obj is AndroidJavaClass)
            {
                return new AndroidJavaObject(((AndroidJavaClass) obj).GetRawClass());
            }
            if (obj is AndroidJavaObject)
            {
                return (AndroidJavaObject) obj;
            }
            if (obj is Array)
            {
                return AndroidJavaObject.AndroidJavaObjectDeleteLocalRef(ConvertToJNIArray((Array) obj));
            }
            if (obj is AndroidJavaProxy)
            {
                return AndroidJavaObject.AndroidJavaObjectDeleteLocalRef(AndroidJNIHelper.CreateJavaProxy((AndroidJavaProxy) obj));
            }
            if (!(obj is AndroidJavaRunnable))
            {
                throw new Exception("JNI; Unknown argument type '" + obj.GetType() + "'");
            }
            return AndroidJavaObject.AndroidJavaObjectDeleteLocalRef(AndroidJNIHelper.CreateJavaRunnable((AndroidJavaRunnable) obj));
        }

        public static ArrayType ConvertFromJNIArray<ArrayType>(IntPtr array)
        {
            Type elementType = typeof(ArrayType).GetElementType();
            if (AndroidReflection.IsPrimitive(elementType))
            {
                if (elementType == typeof(int))
                {
                    return (ArrayType) AndroidJNISafe.FromIntArray(array);
                }
                if (elementType == typeof(bool))
                {
                    return (ArrayType) AndroidJNISafe.FromBooleanArray(array);
                }
                if (elementType == typeof(byte))
                {
                    return (ArrayType) AndroidJNISafe.FromByteArray(array);
                }
                if (elementType == typeof(short))
                {
                    return (ArrayType) AndroidJNISafe.FromShortArray(array);
                }
                if (elementType == typeof(long))
                {
                    return (ArrayType) AndroidJNISafe.FromLongArray(array);
                }
                if (elementType == typeof(float))
                {
                    return (ArrayType) AndroidJNISafe.FromFloatArray(array);
                }
                if (elementType == typeof(double))
                {
                    return (ArrayType) AndroidJNISafe.FromDoubleArray(array);
                }
                if (elementType != typeof(char))
                {
                    return default(ArrayType);
                }
                return (ArrayType) AndroidJNISafe.FromCharArray(array);
            }
            if (elementType == typeof(string))
            {
                int num = AndroidJNISafe.GetArrayLength(array);
                string[] strArray = new string[num];
                for (int j = 0; j < num; j++)
                {
                    IntPtr objectArrayElement = AndroidJNI.GetObjectArrayElement(array, j);
                    strArray[j] = AndroidJNISafe.GetStringUTFChars(objectArrayElement);
                    AndroidJNISafe.DeleteLocalRef(objectArrayElement);
                }
                return (ArrayType) strArray;
            }
            if (elementType != typeof(AndroidJavaObject))
            {
                throw new Exception("JNI: Unknown generic array type '" + elementType + "'");
            }
            int arrayLength = AndroidJNISafe.GetArrayLength(array);
            AndroidJavaObject[] objArray = new AndroidJavaObject[arrayLength];
            for (int i = 0; i < arrayLength; i++)
            {
                IntPtr jobject = AndroidJNI.GetObjectArrayElement(array, i);
                objArray[i] = new AndroidJavaObject(jobject);
                AndroidJNISafe.DeleteLocalRef(jobject);
            }
            return (ArrayType) objArray;
        }

        public static IntPtr ConvertToJNIArray(Array array)
        {
            Type elementType = array.GetType().GetElementType();
            if (AndroidReflection.IsPrimitive(elementType))
            {
                if (elementType == typeof(int))
                {
                    return AndroidJNISafe.ToIntArray((int[]) array);
                }
                if (elementType == typeof(bool))
                {
                    return AndroidJNISafe.ToBooleanArray((bool[]) array);
                }
                if (elementType == typeof(byte))
                {
                    return AndroidJNISafe.ToByteArray((byte[]) array);
                }
                if (elementType == typeof(short))
                {
                    return AndroidJNISafe.ToShortArray((short[]) array);
                }
                if (elementType == typeof(long))
                {
                    return AndroidJNISafe.ToLongArray((long[]) array);
                }
                if (elementType == typeof(float))
                {
                    return AndroidJNISafe.ToFloatArray((float[]) array);
                }
                if (elementType == typeof(double))
                {
                    return AndroidJNISafe.ToDoubleArray((double[]) array);
                }
                if (elementType != typeof(char))
                {
                    return IntPtr.Zero;
                }
                return AndroidJNISafe.ToCharArray((char[]) array);
            }
            if (elementType == typeof(string))
            {
                string[] strArray = (string[]) array;
                int size = array.GetLength(0);
                IntPtr clazz = AndroidJNISafe.FindClass("java/lang/String");
                IntPtr ptr2 = AndroidJNI.NewObjectArray(size, clazz, IntPtr.Zero);
                for (int j = 0; j < size; j++)
                {
                    IntPtr ptr3 = AndroidJNISafe.NewStringUTF(strArray[j]);
                    AndroidJNI.SetObjectArrayElement(ptr2, j, ptr3);
                    AndroidJNISafe.DeleteLocalRef(ptr3);
                }
                AndroidJNISafe.DeleteLocalRef(clazz);
                return ptr2;
            }
            if (elementType != typeof(AndroidJavaObject))
            {
                throw new Exception("JNI; Unknown array type '" + elementType + "'");
            }
            AndroidJavaObject[] objArray = (AndroidJavaObject[]) array;
            int length = array.GetLength(0);
            IntPtr[] ptrArray = new IntPtr[length];
            IntPtr localref = AndroidJNISafe.FindClass("java/lang/Object");
            IntPtr zero = IntPtr.Zero;
            for (int i = 0; i < length; i++)
            {
                if (objArray[i] != null)
                {
                    ptrArray[i] = objArray[i].GetRawObject();
                    IntPtr rawClass = objArray[i].GetRawClass();
                    if (zero != rawClass)
                    {
                        if (zero == IntPtr.Zero)
                        {
                            zero = rawClass;
                        }
                        else
                        {
                            zero = localref;
                        }
                    }
                }
                else
                {
                    ptrArray[i] = IntPtr.Zero;
                }
            }
            IntPtr ptr7 = AndroidJNISafe.ToObjectArray(ptrArray, zero);
            AndroidJNISafe.DeleteLocalRef(localref);
            return ptr7;
        }

        public static IntPtr CreateJavaProxy(int delegateHandle, AndroidJavaProxy proxy)
        {
            return AndroidReflection.NewProxyInstance(delegateHandle, proxy.javaInterface.GetRawClass());
        }

        public static IntPtr CreateJavaRunnable(AndroidJavaRunnable jrunnable)
        {
            return AndroidJNIHelper.CreateJavaProxy(new AndroidJavaRunnableProxy(jrunnable));
        }

        public static jvalue[] CreateJNIArgArray(object[] args)
        {
            jvalue[] jvalueArray = new jvalue[args.GetLength(0)];
            int index = 0;
            foreach (object obj2 in args)
            {
                if (obj2 == null)
                {
                    jvalueArray[index].l = IntPtr.Zero;
                }
                else if (AndroidReflection.IsPrimitive(obj2.GetType()))
                {
                    if (obj2 is int)
                    {
                        jvalueArray[index].i = (int) obj2;
                    }
                    else if (obj2 is bool)
                    {
                        jvalueArray[index].z = (bool) obj2;
                    }
                    else if (obj2 is byte)
                    {
                        jvalueArray[index].b = (byte) obj2;
                    }
                    else if (obj2 is short)
                    {
                        jvalueArray[index].s = (short) obj2;
                    }
                    else if (obj2 is long)
                    {
                        jvalueArray[index].j = (long) obj2;
                    }
                    else if (obj2 is float)
                    {
                        jvalueArray[index].f = (float) obj2;
                    }
                    else if (obj2 is double)
                    {
                        jvalueArray[index].d = (double) obj2;
                    }
                    else if (obj2 is char)
                    {
                        jvalueArray[index].c = (char) obj2;
                    }
                }
                else if (obj2 is string)
                {
                    jvalueArray[index].l = AndroidJNISafe.NewStringUTF((string) obj2);
                }
                else if (obj2 is AndroidJavaClass)
                {
                    jvalueArray[index].l = ((AndroidJavaClass) obj2).GetRawClass();
                }
                else if (obj2 is AndroidJavaObject)
                {
                    jvalueArray[index].l = ((AndroidJavaObject) obj2).GetRawObject();
                }
                else if (obj2 is Array)
                {
                    jvalueArray[index].l = ConvertToJNIArray((Array) obj2);
                }
                else if (obj2 is AndroidJavaProxy)
                {
                    jvalueArray[index].l = AndroidJNIHelper.CreateJavaProxy((AndroidJavaProxy) obj2);
                }
                else
                {
                    if (!(obj2 is AndroidJavaRunnable))
                    {
                        throw new Exception("JNI; Unknown argument type '" + obj2.GetType() + "'");
                    }
                    jvalueArray[index].l = AndroidJNIHelper.CreateJavaRunnable((AndroidJavaRunnable) obj2);
                }
                index++;
            }
            return jvalueArray;
        }

        public static void DeleteJNIArgArray(object[] args, jvalue[] jniArgs)
        {
            int index = 0;
            foreach (object obj2 in args)
            {
                if (((obj2 is string) || (obj2 is AndroidJavaRunnable)) || ((obj2 is AndroidJavaProxy) || (obj2 is Array)))
                {
                    AndroidJNISafe.DeleteLocalRef(jniArgs[index].l);
                }
                index++;
            }
        }

        public static IntPtr GetConstructorID(IntPtr jclass, object[] args)
        {
            return AndroidJNIHelper.GetConstructorID(jclass, GetSignature(args));
        }

        public static IntPtr GetConstructorID(IntPtr jclass, string signature)
        {
            IntPtr ptr3;
            IntPtr zero = IntPtr.Zero;
            try
            {
                zero = AndroidReflection.GetConstructorMember(jclass, signature);
                ptr3 = AndroidJNISafe.FromReflectedMethod(zero);
            }
            catch (Exception exception)
            {
                IntPtr ptr2 = AndroidJNISafe.GetMethodID(jclass, "<init>", signature);
                if (ptr2 == IntPtr.Zero)
                {
                    throw exception;
                }
                return ptr2;
            }
            finally
            {
                AndroidJNISafe.DeleteLocalRef(zero);
            }
            return ptr3;
        }

        public static IntPtr GetFieldID<ReturnType>(IntPtr jclass, string fieldName, bool isStatic)
        {
            return AndroidJNIHelper.GetFieldID(jclass, fieldName, GetSignature(typeof(ReturnType)), isStatic);
        }

        public static IntPtr GetFieldID(IntPtr jclass, string fieldName, string signature, bool isStatic)
        {
            IntPtr ptr3;
            IntPtr zero = IntPtr.Zero;
            try
            {
                zero = AndroidReflection.GetFieldMember(jclass, fieldName, signature, isStatic);
                ptr3 = AndroidJNISafe.FromReflectedField(zero);
            }
            catch (Exception exception)
            {
                IntPtr ptr2 = !isStatic ? AndroidJNISafe.GetFieldID(jclass, fieldName, signature) : AndroidJNISafe.GetStaticFieldID(jclass, fieldName, signature);
                if (ptr2 == IntPtr.Zero)
                {
                    throw exception;
                }
                return ptr2;
            }
            finally
            {
                AndroidJNISafe.DeleteLocalRef(zero);
            }
            return ptr3;
        }

        public static IntPtr GetMethodID(IntPtr jclass, string methodName, object[] args, bool isStatic)
        {
            return AndroidJNIHelper.GetMethodID(jclass, methodName, GetSignature(args), isStatic);
        }

        public static IntPtr GetMethodID(IntPtr jclass, string methodName, string signature, bool isStatic)
        {
            IntPtr ptr3;
            IntPtr zero = IntPtr.Zero;
            try
            {
                zero = AndroidReflection.GetMethodMember(jclass, methodName, signature, isStatic);
                ptr3 = AndroidJNISafe.FromReflectedMethod(zero);
            }
            catch (Exception exception)
            {
                IntPtr ptr2 = !isStatic ? AndroidJNISafe.GetMethodID(jclass, methodName, signature) : AndroidJNISafe.GetStaticMethodID(jclass, methodName, signature);
                if (ptr2 == IntPtr.Zero)
                {
                    throw exception;
                }
                return ptr2;
            }
            finally
            {
                AndroidJNISafe.DeleteLocalRef(zero);
            }
            return ptr3;
        }

        public static IntPtr GetMethodID<ReturnType>(IntPtr jclass, string methodName, object[] args, bool isStatic)
        {
            return AndroidJNIHelper.GetMethodID(jclass, methodName, GetSignature<ReturnType>(args), isStatic);
        }

        public static string GetSignature(object obj)
        {
            if (obj == null)
            {
                return "Ljava/lang/Object;";
            }
            Type t = !(obj is Type) ? obj.GetType() : ((Type) obj);
            if (AndroidReflection.IsPrimitive(t))
            {
                if (t.Equals(typeof(int)))
                {
                    return "I";
                }
                if (t.Equals(typeof(bool)))
                {
                    return "Z";
                }
                if (t.Equals(typeof(byte)))
                {
                    return "B";
                }
                if (t.Equals(typeof(short)))
                {
                    return "S";
                }
                if (t.Equals(typeof(long)))
                {
                    return "J";
                }
                if (t.Equals(typeof(float)))
                {
                    return "F";
                }
                if (t.Equals(typeof(double)))
                {
                    return "D";
                }
                if (!t.Equals(typeof(char)))
                {
                    return string.Empty;
                }
                return "C";
            }
            if (t.Equals(typeof(string)))
            {
                return "Ljava/lang/String;";
            }
            if (obj is AndroidJavaProxy)
            {
                AndroidJavaObject obj2 = new AndroidJavaObject(((AndroidJavaProxy) obj).javaInterface.GetRawClass());
                return ("L" + obj2.Call<string>("getName", new object[0]) + ";");
            }
            if (t.Equals(typeof(AndroidJavaRunnable)))
            {
                return "Ljava/lang/Runnable;";
            }
            if (t.Equals(typeof(AndroidJavaClass)))
            {
                return "Ljava/lang/Class;";
            }
            if (t.Equals(typeof(AndroidJavaObject)))
            {
                if (obj == t)
                {
                    return "Ljava/lang/Object;";
                }
                AndroidJavaObject obj3 = (AndroidJavaObject) obj;
                using (AndroidJavaObject obj4 = obj3.Call<AndroidJavaObject>("getClass", new object[0]))
                {
                    return ("L" + obj4.Call<string>("getName", new object[0]) + ";");
                }
            }
            if (AndroidReflection.IsAssignableFrom(typeof(Array), t))
            {
                if (t.GetArrayRank() != 1)
                {
                    throw new Exception("JNI: System.Array in n dimensions is not allowed");
                }
                StringBuilder builder = new StringBuilder();
                builder.Append('[');
                builder.Append(GetSignature(t.GetElementType()));
                return builder.ToString();
            }
            object[] objArray1 = new object[] { "JNI: Unknown signature for type '", t, "' (obj = ", obj, ") ", (t != obj) ? "instance" : "equal" };
            throw new Exception(string.Concat(objArray1));
        }

        public static string GetSignature(object[] args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('(');
            foreach (object obj2 in args)
            {
                builder.Append(GetSignature(obj2));
            }
            builder.Append(")V");
            return builder.ToString();
        }

        public static string GetSignature<ReturnType>(object[] args)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('(');
            foreach (object obj2 in args)
            {
                builder.Append(GetSignature(obj2));
            }
            builder.Append(')');
            builder.Append(GetSignature(typeof(ReturnType)));
            return builder.ToString();
        }

        public static IntPtr InvokeJavaProxyMethod(AndroidJavaProxy proxy, IntPtr jmethodName, IntPtr jargs)
        {
            int arrayLength = 0;
            if (jargs != IntPtr.Zero)
            {
                arrayLength = AndroidJNISafe.GetArrayLength(jargs);
            }
            AndroidJavaObject[] javaArgs = new AndroidJavaObject[arrayLength];
            for (int i = 0; i < arrayLength; i++)
            {
                IntPtr objectArrayElement = AndroidJNISafe.GetObjectArrayElement(jargs, i);
                javaArgs[i] = !(objectArrayElement != IntPtr.Zero) ? null : new AndroidJavaObject(objectArrayElement);
            }
            using (AndroidJavaObject obj2 = proxy.Invoke(AndroidJNI.GetStringUTFChars(jmethodName), javaArgs))
            {
                if (obj2 == null)
                {
                    return IntPtr.Zero;
                }
                return AndroidJNI.NewLocalRef(obj2.GetRawObject());
            }
        }

        public static object Unbox(AndroidJavaObject obj)
        {
            if (obj == null)
            {
                return null;
            }
            AndroidJavaObject obj2 = obj.Call<AndroidJavaObject>("getClass", new object[0]);
            string str = obj2.Call<string>("getName", new object[0]);
            if ("java.lang.Integer" == str)
            {
                return obj.Call<int>("intValue", new object[0]);
            }
            if ("java.lang.Boolean" == str)
            {
                return obj.Call<bool>("booleanValue", new object[0]);
            }
            if ("java.lang.Byte" == str)
            {
                return obj.Call<byte>("byteValue", new object[0]);
            }
            if ("java.lang.Short" == str)
            {
                return obj.Call<short>("shortValue", new object[0]);
            }
            if ("java.lang.Long" == str)
            {
                return obj.Call<long>("longValue", new object[0]);
            }
            if ("java.lang.Float" == str)
            {
                return obj.Call<float>("floatValue", new object[0]);
            }
            if ("java.lang.Double" == str)
            {
                return obj.Call<double>("doubleValue", new object[0]);
            }
            if ("java.lang.Character" == str)
            {
                return obj.Call<char>("charValue", new object[0]);
            }
            if ("java.lang.String" == str)
            {
                return obj.Call<string>("toString", new object[0]);
            }
            if ("java.lang.Class" == str)
            {
                return new AndroidJavaClass(obj.GetRawObject());
            }
            if (obj2.Call<bool>("isArray", new object[0]))
            {
                return UnboxArray(obj);
            }
            return obj;
        }

        public static object UnboxArray(AndroidJavaObject obj)
        {
            Array array;
            if (obj == null)
            {
                return null;
            }
            AndroidJavaClass class2 = new AndroidJavaClass("java/lang/reflect/Array");
            AndroidJavaObject obj3 = obj.Call<AndroidJavaObject>("getClass", new object[0]).Call<AndroidJavaObject>("getComponentType", new object[0]);
            string str = obj3.Call<string>("getName", new object[0]);
            object[] args = new object[] { obj };
            int num = class2.Call<int>("getLength", args);
            if (obj3.Call<bool>("IsPrimitive", new object[0]))
            {
                if ("I" != str)
                {
                    if ("Z" != str)
                    {
                        if ("B" != str)
                        {
                            if ("S" != str)
                            {
                                if ("J" != str)
                                {
                                    if ("F" != str)
                                    {
                                        if ("D" != str)
                                        {
                                            if ("C" != str)
                                            {
                                                throw new Exception("JNI; Unknown argument type '" + str + "'");
                                            }
                                            array = new char[num];
                                        }
                                        else
                                        {
                                            array = new double[num];
                                        }
                                    }
                                    else
                                    {
                                        array = new float[num];
                                    }
                                }
                                else
                                {
                                    array = new long[num];
                                }
                            }
                            else
                            {
                                array = new short[num];
                            }
                        }
                        else
                        {
                            array = new byte[num];
                        }
                    }
                    else
                    {
                        array = new bool[num];
                    }
                }
                else
                {
                    array = new int[num];
                }
            }
            else if ("java.lang.String" == str)
            {
                array = new string[num];
            }
            else if ("java.lang.Class" == str)
            {
                array = new AndroidJavaClass[num];
            }
            else
            {
                array = new AndroidJavaObject[num];
            }
            for (int i = 0; i < num; i++)
            {
                object[] objArray2 = new object[] { obj, i };
                array.SetValue(Unbox(class2.CallStatic<AndroidJavaObject>("get", objArray2)), i);
            }
            return array;
        }
    }
}

