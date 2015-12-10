namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class BaseObjectTools
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string ClassIDToString(int ID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetSuperClassID(int ID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsBaseObject(int ID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsDerivedFromClassID(int classID, int derivedFromClassID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int StringToClassID(string classString);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int StringToClassIDCaseInsensitive(string classString);
    }
}

