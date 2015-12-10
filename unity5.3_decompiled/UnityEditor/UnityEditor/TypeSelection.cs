namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class TypeSelection : IComparable
    {
        public GUIContent label;
        public Object[] objects;

        public TypeSelection(string typeName, Object[] objects)
        {
            this.objects = objects;
            object[] objArray1 = new object[] { objects.Length, " ", ObjectNames.NicifyVariableName(typeName), (objects.Length <= 1) ? string.Empty : "s" };
            this.label = new GUIContent(string.Concat(objArray1));
            this.label.image = AssetPreview.GetMiniTypeThumbnail(objects[0]);
        }

        public int CompareTo(object o)
        {
            TypeSelection selection = (TypeSelection) o;
            if (selection.objects.Length != this.objects.Length)
            {
                int length = selection.objects.Length;
                return length.CompareTo(this.objects.Length);
            }
            return this.label.text.CompareTo(selection.label.text);
        }
    }
}

