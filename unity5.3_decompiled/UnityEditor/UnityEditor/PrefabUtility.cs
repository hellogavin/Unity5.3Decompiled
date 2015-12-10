namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Internal;
    using UnityEngine.SceneManagement;

    public sealed class PrefabUtility
    {
        public static PrefabInstanceUpdated prefabInstanceUpdated;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Object CreateEmptyPrefab(string path);
        [ExcludeFromDocs]
        public static GameObject CreatePrefab(string path, GameObject go)
        {
            ReplacePrefabOptions options = ReplacePrefabOptions.Default;
            return CreatePrefab(path, go, options);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern GameObject CreatePrefab(string path, GameObject go, [DefaultValue("ReplacePrefabOptions.Default")] ReplacePrefabOptions options);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DisconnectPrefabInstance(Object targetObject);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern GameObject FindPrefabRoot(GameObject source);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern GameObject FindRootGameObjectWithSameParentPrefab(GameObject target);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern GameObject FindValidUploadPrefabInstanceRoot(GameObject target);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Object GetPrefabObject(Object targetObject);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Object GetPrefabParent(Object source);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern PrefabType GetPrefabType(Object target);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern PropertyModification[] GetPropertyModifications(Object targetPrefab);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Object InstantiateAttachedAsset(Object targetObject);
        public static Object InstantiatePrefab(Object target)
        {
            return InternalInstantiatePrefab(target, SceneManager.GetActiveScene());
        }

        public static Object InstantiatePrefab(Object target, Scene destinationScene)
        {
            return InternalInstantiatePrefab(target, destinationScene);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Object INTERNAL_CALL_InternalInstantiatePrefab(Object target, ref Scene destinationScene);
        private static void Internal_CallPrefabInstanceUpdated(GameObject instance)
        {
            if (prefabInstanceUpdated != null)
            {
                prefabInstanceUpdated(instance);
            }
        }

        private static Object InternalInstantiatePrefab(Object target, Scene destinationScene)
        {
            return INTERNAL_CALL_InternalInstantiatePrefab(target, ref destinationScene);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsComponentAddedToPrefabInstance(Object source);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void MergeAllPrefabInstances(Object targetObject);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool ReconnectToLastPrefab(GameObject go);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RecordPrefabInstancePropertyModifications(Object targetObject);
        [ExcludeFromDocs]
        public static GameObject ReplacePrefab(GameObject go, Object targetPrefab)
        {
            ReplacePrefabOptions options = ReplacePrefabOptions.Default;
            return ReplacePrefab(go, targetPrefab, options);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern GameObject ReplacePrefab(GameObject go, Object targetPrefab, [DefaultValue("ReplacePrefabOptions.Default")] ReplacePrefabOptions options);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool ResetToPrefabState(Object obj);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool RevertPrefabInstance(GameObject go);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetPropertyModifications(Object targetPrefab, PropertyModification[] modifications);

        public delegate void PrefabInstanceUpdated(GameObject instance);
    }
}

