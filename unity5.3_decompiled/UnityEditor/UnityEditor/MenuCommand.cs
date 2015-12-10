namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public sealed class MenuCommand
    {
        public Object context;
        public int userData;
        public MenuCommand(Object inContext, int inUserData)
        {
            this.context = inContext;
            this.userData = inUserData;
        }

        public MenuCommand(Object inContext)
        {
            this.context = inContext;
            this.userData = 0;
        }
    }
}

