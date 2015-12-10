namespace UnityEngine
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal sealed class UnityLogWriter : TextWriter
    {
        public static void Init()
        {
            Console.SetOut(new UnityLogWriter());
        }

        public override void Write(char value)
        {
            WriteStringToUnityLog(value.ToString());
        }

        public override void Write(string s)
        {
            WriteStringToUnityLog(s);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void WriteStringToUnityLog(string s);

        public override System.Text.Encoding Encoding
        {
            get
            {
                return System.Text.Encoding.UTF8;
            }
        }
    }
}

