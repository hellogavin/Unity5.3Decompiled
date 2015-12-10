namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class GenericMenu
    {
        private ArrayList menuItems = new ArrayList();

        public void AddDisabledItem(GUIContent content)
        {
            this.menuItems.Add(new MenuItem(content, false, false, null));
        }

        public void AddItem(GUIContent content, bool on, MenuFunction func)
        {
            this.menuItems.Add(new MenuItem(content, false, on, func));
        }

        public void AddItem(GUIContent content, bool on, MenuFunction2 func, object userData)
        {
            this.menuItems.Add(new MenuItem(content, false, on, func, userData));
        }

        public void AddSeparator(string path)
        {
            this.menuItems.Add(new MenuItem(new GUIContent(path), true, false, null));
        }

        private void CatchMenu(object userData, string[] options, int selected)
        {
            MenuItem item = (MenuItem) this.menuItems[selected];
            if (item.func2 != null)
            {
                item.func2(item.userData);
            }
            else if (item.func != null)
            {
                item.func();
            }
        }

        public void DropDown(Rect position)
        {
            string[] options = new string[this.menuItems.Count];
            bool[] enabled = new bool[this.menuItems.Count];
            ArrayList list = new ArrayList();
            bool[] separator = new bool[this.menuItems.Count];
            for (int i = 0; i < this.menuItems.Count; i++)
            {
                MenuItem item = (MenuItem) this.menuItems[i];
                options[i] = item.content.text;
                enabled[i] = (item.func != null) || (item.func2 != null);
                separator[i] = item.separator;
                if (item.on)
                {
                    list.Add(i);
                }
            }
            EditorUtility.DisplayCustomMenuWithSeparators(position, options, enabled, separator, (int[]) list.ToArray(typeof(int)), new EditorUtility.SelectMenuItemFunction(this.CatchMenu), null);
        }

        public int GetItemCount()
        {
            return this.menuItems.Count;
        }

        internal void Popup(Rect position, int selectedIndex)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                this.DropDown(position);
            }
            else
            {
                this.DropDown(position);
            }
        }

        public void ShowAsContext()
        {
            if (Event.current != null)
            {
                this.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0f, 0f));
            }
        }

        public delegate void MenuFunction();

        public delegate void MenuFunction2(object userData);

        private sealed class MenuItem
        {
            public GUIContent content;
            public GenericMenu.MenuFunction func;
            public GenericMenu.MenuFunction2 func2;
            public bool on;
            public bool separator;
            public object userData;

            public MenuItem(GUIContent _content, bool _separator, bool _on, GenericMenu.MenuFunction _func)
            {
                this.content = _content;
                this.separator = _separator;
                this.on = _on;
                this.func = _func;
            }

            public MenuItem(GUIContent _content, bool _separator, bool _on, GenericMenu.MenuFunction2 _func, object _userData)
            {
                this.content = _content;
                this.separator = _separator;
                this.on = _on;
                this.func2 = _func;
                this.userData = _userData;
            }
        }
    }
}

