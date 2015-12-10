namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal struct ListViewElement
    {
        public int row;
        public int column;
        public Rect position;
    }
}

