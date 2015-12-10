namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public class TextAsset : Object
    {
        public override string ToString()
        {
            return this.text;
        }

        public byte[] bytes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string text { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

