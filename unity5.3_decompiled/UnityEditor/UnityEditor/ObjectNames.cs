namespace UnityEditor
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class ObjectNames
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetClassName(Object obj);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetDragAndDropTitle(Object obj);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetInspectorTitle(Object obj);
        [Obsolete("Please use GetInspectorTitle instead")]
        public static string GetPropertyEditorTitle(Object obj)
        {
            return GetInspectorTitle(obj);
        }

        internal static string GetTypeName(Object obj)
        {
            if (obj == null)
            {
                return "Object";
            }
            string path = AssetDatabase.GetAssetPath(obj).ToLower();
            if (path.EndsWith(".unity"))
            {
                return "Scene";
            }
            if (path.EndsWith(".guiskin"))
            {
                return "GUI Skin";
            }
            if (Directory.Exists(AssetDatabase.GetAssetPath(obj)))
            {
                return "Folder";
            }
            if (obj.GetType() == typeof(Object))
            {
                return (Path.GetExtension(path) + " File");
            }
            return GetClassName(obj);
        }

        [Obsolete("Please use NicifyVariableName instead")]
        public static string MangleVariableName(string name)
        {
            return NicifyVariableName(name);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string NicifyVariableName(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetNameSmart(Object obj, string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetNameSmartWithInstanceID(int instanceID, string name);
    }
}

