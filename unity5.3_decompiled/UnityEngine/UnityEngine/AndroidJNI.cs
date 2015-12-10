namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class AndroidJNI
    {
        private AndroidJNI()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr AllocObject(IntPtr clazz);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int AttachCurrentThread();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool CallBooleanMethod(IntPtr obj, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern byte CallByteMethod(IntPtr obj, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern char CallCharMethod(IntPtr obj, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern double CallDoubleMethod(IntPtr obj, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float CallFloatMethod(IntPtr obj, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int CallIntMethod(IntPtr obj, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern long CallLongMethod(IntPtr obj, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr CallObjectMethod(IntPtr obj, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern short CallShortMethod(IntPtr obj, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool CallStaticBooleanMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern byte CallStaticByteMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern char CallStaticCharMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern double CallStaticDoubleMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float CallStaticFloatMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int CallStaticIntMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern long CallStaticLongMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr CallStaticObjectMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern short CallStaticShortMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string CallStaticStringMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CallStaticVoidMethod(IntPtr clazz, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string CallStringMethod(IntPtr obj, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CallVoidMethod(IntPtr obj, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DeleteGlobalRef(IntPtr obj);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DeleteLocalRef(IntPtr obj);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int DetachCurrentThread();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int EnsureLocalCapacity(int capacity);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ExceptionClear();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ExceptionDescribe();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr ExceptionOccurred();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void FatalError(string message);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr FindClass(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool[] FromBooleanArray(IntPtr array);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern byte[] FromByteArray(IntPtr array);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern char[] FromCharArray(IntPtr array);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern double[] FromDoubleArray(IntPtr array);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float[] FromFloatArray(IntPtr array);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int[] FromIntArray(IntPtr array);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern long[] FromLongArray(IntPtr array);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr[] FromObjectArray(IntPtr array);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr FromReflectedField(IntPtr refField);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr FromReflectedMethod(IntPtr refMethod);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern short[] FromShortArray(IntPtr array);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetArrayLength(IntPtr array);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetBooleanArrayElement(IntPtr array, int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetBooleanField(IntPtr obj, IntPtr fieldID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern byte GetByteArrayElement(IntPtr array, int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern byte GetByteField(IntPtr obj, IntPtr fieldID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern char GetCharArrayElement(IntPtr array, int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern char GetCharField(IntPtr obj, IntPtr fieldID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern double GetDoubleArrayElement(IntPtr array, int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern double GetDoubleField(IntPtr obj, IntPtr fieldID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr GetFieldID(IntPtr clazz, string name, string sig);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float GetFloatArrayElement(IntPtr array, int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float GetFloatField(IntPtr obj, IntPtr fieldID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetIntArrayElement(IntPtr array, int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetIntField(IntPtr obj, IntPtr fieldID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern long GetLongArrayElement(IntPtr array, int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern long GetLongField(IntPtr obj, IntPtr fieldID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr GetMethodID(IntPtr clazz, string name, string sig);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr GetObjectArrayElement(IntPtr array, int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr GetObjectClass(IntPtr obj);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr GetObjectField(IntPtr obj, IntPtr fieldID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern short GetShortArrayElement(IntPtr array, int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern short GetShortField(IntPtr obj, IntPtr fieldID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetStaticBooleanField(IntPtr clazz, IntPtr fieldID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern byte GetStaticByteField(IntPtr clazz, IntPtr fieldID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern char GetStaticCharField(IntPtr clazz, IntPtr fieldID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern double GetStaticDoubleField(IntPtr clazz, IntPtr fieldID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr GetStaticFieldID(IntPtr clazz, string name, string sig);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float GetStaticFloatField(IntPtr clazz, IntPtr fieldID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetStaticIntField(IntPtr clazz, IntPtr fieldID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern long GetStaticLongField(IntPtr clazz, IntPtr fieldID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr GetStaticMethodID(IntPtr clazz, string name, string sig);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr GetStaticObjectField(IntPtr clazz, IntPtr fieldID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern short GetStaticShortField(IntPtr clazz, IntPtr fieldID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetStaticStringField(IntPtr clazz, IntPtr fieldID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetStringField(IntPtr obj, IntPtr fieldID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetStringUTFChars(IntPtr str);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetStringUTFLength(IntPtr str);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr GetSuperclass(IntPtr clazz);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetVersion();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsAssignableFrom(IntPtr clazz1, IntPtr clazz2);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsInstanceOf(IntPtr obj, IntPtr clazz);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsSameObject(IntPtr obj1, IntPtr obj2);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr NewBooleanArray(int size);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr NewByteArray(int size);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr NewCharArray(int size);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr NewDoubleArray(int size);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr NewFloatArray(int size);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr NewGlobalRef(IntPtr obj);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr NewIntArray(int size);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr NewLocalRef(IntPtr obj);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr NewLongArray(int size);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr NewObject(IntPtr clazz, IntPtr methodID, jvalue[] args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr NewObjectArray(int size, IntPtr clazz, IntPtr obj);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr NewShortArray(int size);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr NewStringUTF(string bytes);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr PopLocalFrame(IntPtr result);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int PushLocalFrame(int capacity);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetBooleanArrayElement(IntPtr array, int index, byte val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetBooleanField(IntPtr obj, IntPtr fieldID, bool val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetByteArrayElement(IntPtr array, int index, sbyte val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetByteField(IntPtr obj, IntPtr fieldID, byte val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetCharArrayElement(IntPtr array, int index, char val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetCharField(IntPtr obj, IntPtr fieldID, char val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetDoubleArrayElement(IntPtr array, int index, double val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetDoubleField(IntPtr obj, IntPtr fieldID, double val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetFloatArrayElement(IntPtr array, int index, float val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetFloatField(IntPtr obj, IntPtr fieldID, float val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetIntArrayElement(IntPtr array, int index, int val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetIntField(IntPtr obj, IntPtr fieldID, int val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetLongArrayElement(IntPtr array, int index, long val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetLongField(IntPtr obj, IntPtr fieldID, long val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetObjectArrayElement(IntPtr array, int index, IntPtr obj);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetObjectField(IntPtr obj, IntPtr fieldID, IntPtr val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetShortArrayElement(IntPtr array, int index, short val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetShortField(IntPtr obj, IntPtr fieldID, short val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetStaticBooleanField(IntPtr clazz, IntPtr fieldID, bool val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetStaticByteField(IntPtr clazz, IntPtr fieldID, byte val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetStaticCharField(IntPtr clazz, IntPtr fieldID, char val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetStaticDoubleField(IntPtr clazz, IntPtr fieldID, double val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetStaticFloatField(IntPtr clazz, IntPtr fieldID, float val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetStaticIntField(IntPtr clazz, IntPtr fieldID, int val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetStaticLongField(IntPtr clazz, IntPtr fieldID, long val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetStaticObjectField(IntPtr clazz, IntPtr fieldID, IntPtr val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetStaticShortField(IntPtr clazz, IntPtr fieldID, short val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetStaticStringField(IntPtr clazz, IntPtr fieldID, string val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetStringField(IntPtr obj, IntPtr fieldID, string val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int Throw(IntPtr obj);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int ThrowNew(IntPtr clazz, string message);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr ToBooleanArray(bool[] array);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr ToByteArray(byte[] array);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr ToCharArray(char[] array);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr ToDoubleArray(double[] array);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr ToFloatArray(float[] array);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr ToIntArray(int[] array);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr ToLongArray(long[] array);
        public static IntPtr ToObjectArray(IntPtr[] array)
        {
            return ToObjectArray(array, IntPtr.Zero);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr ToObjectArray(IntPtr[] array, IntPtr arrayClass);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr ToReflectedField(IntPtr clazz, IntPtr fieldID, bool isStatic);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr ToReflectedMethod(IntPtr clazz, IntPtr methodID, bool isStatic);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern IntPtr ToShortArray(short[] array);
    }
}

