namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal sealed class VerticalLayout : IDisposable
    {
        private static readonly VerticalLayout instance = new VerticalLayout();

        private VerticalLayout()
        {
        }

        public static IDisposable DoLayout()
        {
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            return instance;
        }

        void IDisposable.Dispose()
        {
            GUILayout.EndVertical();
        }
    }
}

