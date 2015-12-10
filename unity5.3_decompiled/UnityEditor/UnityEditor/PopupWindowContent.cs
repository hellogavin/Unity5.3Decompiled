namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public abstract class PopupWindowContent
    {
        protected PopupWindowContent()
        {
        }

        public virtual Vector2 GetWindowSize()
        {
            return new Vector2(200f, 200f);
        }

        public virtual void OnClose()
        {
        }

        public abstract void OnGUI(Rect rect);
        public virtual void OnOpen()
        {
        }

        public EditorWindow editorWindow { get; internal set; }
    }
}

