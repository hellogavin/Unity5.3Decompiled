namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class DropInfo
    {
        public IDropArea dropArea;
        public Rect rect;
        public Type type = Type.Window;
        public object userData;

        public DropInfo(IDropArea source)
        {
            this.dropArea = source;
        }

        internal enum Type
        {
            Tab,
            Pane,
            Window
        }
    }
}

