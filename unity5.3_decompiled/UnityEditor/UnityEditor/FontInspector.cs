namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(Font)), CanEditMultipleObjects]
    internal class FontInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            foreach (Object obj2 in base.targets)
            {
                if (obj2.hideFlags == HideFlags.NotEditable)
                {
                    return;
                }
            }
            base.DrawDefaultInspector();
        }
    }
}

