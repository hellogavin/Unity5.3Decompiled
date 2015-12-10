namespace UnityEngine
{
    using System;

    public class AndroidJavaClass : AndroidJavaObject
    {
        internal AndroidJavaClass(IntPtr jclass)
        {
            if (jclass == IntPtr.Zero)
            {
                throw new Exception("JNI: Init'd AndroidJavaClass with null ptr!");
            }
            base.m_jclass = AndroidJNI.NewGlobalRef(jclass);
            base.m_jobject = IntPtr.Zero;
        }

        public AndroidJavaClass(string className)
        {
            this._AndroidJavaClass(className);
        }

        private void _AndroidJavaClass(string className)
        {
            base.DebugPrint("Creating AndroidJavaClass from " + className);
            using (AndroidJavaObject obj2 = AndroidJavaObject.FindClass(className))
            {
                base.m_jclass = AndroidJNI.NewGlobalRef(obj2.GetRawObject());
                base.m_jobject = IntPtr.Zero;
            }
        }
    }
}

