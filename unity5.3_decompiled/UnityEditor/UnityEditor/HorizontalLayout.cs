namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal sealed class HorizontalLayout : IDisposable
    {
        private static readonly HorizontalLayout instance = new HorizontalLayout();

        private HorizontalLayout()
        {
        }

        public static IDisposable DoLayout()
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            return instance;
        }

        void IDisposable.Dispose()
        {
            GUILayout.EndHorizontal();
        }
    }
}

