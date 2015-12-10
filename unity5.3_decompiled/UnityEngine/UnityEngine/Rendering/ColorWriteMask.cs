namespace UnityEngine.Rendering
{
    using System;

    [Flags]
    public enum ColorWriteMask
    {
        All = 15,
        Alpha = 1,
        Blue = 2,
        Green = 4,
        Red = 8
    }
}

