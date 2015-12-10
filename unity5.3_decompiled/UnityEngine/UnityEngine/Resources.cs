namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngineInternal;

    public sealed class Resources
    {
        internal static T[] ConvertObjects<T>(Object[] rawObjects) where T: Object
        {
            if (rawObjects == null)
            {
                return null;
            }
            T[] localArray = new T[rawObjects.Length];
            for (int i = 0; i < localArray.Length; i++)
            {
                localArray[i] = (T) rawObjects[i];
            }
            return localArray;
        }

        public static T[] FindObjectsOfTypeAll<T>() where T: Object
        {
            return ConvertObjects<T>(FindObjectsOfTypeAll(typeof(T)));
        }

        [MethodImpl(MethodImplOptions.InternalCall), TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument), WrapperlessIcall]
        public static extern Object[] FindObjectsOfTypeAll(Type type);
        public static T GetBuiltinResource<T>(string path) where T: Object
        {
            return (T) GetBuiltinResource(typeof(T), path);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public static extern Object GetBuiltinResource(Type type, string path);
        public static Object Load(string path)
        {
            return Load(path, typeof(Object));
        }

        public static T Load<T>(string path) where T: Object
        {
            return (T) Load(path, typeof(T));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
        public static extern Object Load(string path, Type systemTypeInstance);
        public static Object[] LoadAll(string path)
        {
            return LoadAll(path, typeof(Object));
        }

        public static T[] LoadAll<T>(string path) where T: Object
        {
            return ConvertObjects<T>(LoadAll(path, typeof(T)));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Object[] LoadAll(string path, Type systemTypeInstance);
        [Obsolete("Use AssetDatabase.LoadAssetAtPath<T>() instead (UnityUpgradable) -> * [UnityEditor] UnityEditor.AssetDatabase.LoadAssetAtPath<T>(*)", true)]
        public static T LoadAssetAtPath<T>(string assetPath) where T: Object
        {
            return null;
        }

        [Obsolete("Use AssetDatabase.LoadAssetAtPath instead (UnityUpgradable) -> * [UnityEditor] UnityEditor.AssetDatabase.LoadAssetAtPath(*)", true), TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
        public static Object LoadAssetAtPath(string assetPath, Type type)
        {
            return null;
        }

        public static ResourceRequest LoadAsync(string path)
        {
            return LoadAsync(path, typeof(Object));
        }

        public static ResourceRequest LoadAsync<T>(string path) where T: Object
        {
            return LoadAsync(path, typeof(T));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern ResourceRequest LoadAsync(string path, Type type);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void UnloadAsset(Object assetToUnload);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AsyncOperation UnloadUnusedAssets();
    }
}

