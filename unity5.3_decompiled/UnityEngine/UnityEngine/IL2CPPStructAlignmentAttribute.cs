namespace UnityEngine
{
    using System;

    [AttributeUsage(AttributeTargets.Struct)]
    internal class IL2CPPStructAlignmentAttribute : Attribute
    {
        public int Align = 1;
    }
}

