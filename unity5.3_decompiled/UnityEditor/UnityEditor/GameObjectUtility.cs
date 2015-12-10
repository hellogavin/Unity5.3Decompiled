namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class GameObjectUtility
    {
        [CompilerGenerated]
        private static Func<GameObject, bool> <>f__am$cache0;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool AreStaticEditorFlagsSet(GameObject go, StaticEditorFlags flags);
        internal static bool ContainsStatic(GameObject[] objects)
        {
            if ((objects != null) && (objects.Length != 0))
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    if ((objects[i] != null) && objects[i].isStatic)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        [Obsolete("use AnimatorUtility.DeoptimizeTransformHierarchy instead.")]
        private static void DeoptimizeTransformHierarchy(GameObject go)
        {
            AnimatorUtility.DeoptimizeTransformHierarchy(go);
        }

        internal static ShouldIncludeChildren DisplayUpdateChildrenDialogIfNeeded(IEnumerable<GameObject> gameObjects, string title, string message)
        {
            if (!HasChildren(gameObjects))
            {
                return ShouldIncludeChildren.HasNoChildren;
            }
            return (ShouldIncludeChildren) EditorUtility.DisplayDialogComplex(title, message, "Yes, change children", "No, this object only", "Cancel");
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetNavMeshArea(GameObject go);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetNavMeshAreaFromName(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string[] GetNavMeshAreaNames();
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("GetNavMeshArea instead."), WrapperlessIcall]
        public static extern int GetNavMeshLayer(GameObject go);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("GetNavMeshAreaFromName instead.")]
        public static extern int GetNavMeshLayerFromName(string name);
        [Obsolete("GetNavMeshAreaNames instead.")]
        public static string[] GetNavMeshLayerNames()
        {
            return GetNavMeshAreaNames();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern StaticEditorFlags GetStaticEditorFlags(GameObject go);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetUniqueNameForSibling(Transform parent, string name);
        internal static bool HasChildren(IEnumerable<GameObject> gameObjects)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = go => go.transform.childCount > 0;
            }
            return gameObjects.Any<GameObject>(<>f__am$cache0);
        }

        [Obsolete("use AnimatorUtility.OptimizeTransformHierarchy instead.")]
        private static void OptimizeTransformHierarchy(GameObject go)
        {
            AnimatorUtility.OptimizeTransformHierarchy(go, null);
        }

        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform transform = go.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                SetLayerRecursively(transform.GetChild(i).gameObject, layer);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetNavMeshArea(GameObject go, int areaIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("SetNavMeshArea instead.")]
        public static extern void SetNavMeshLayer(GameObject go, int areaIndex);
        public static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent != null)
            {
                child.transform.SetParent(parent.transform, false);
                RectTransform transform = child.transform as RectTransform;
                if (transform != null)
                {
                    transform.anchoredPosition = Vector2.zero;
                    Vector3 localPosition = transform.localPosition;
                    localPosition.z = 0f;
                    transform.localPosition = localPosition;
                }
                else
                {
                    child.transform.localPosition = Vector3.zero;
                }
                child.transform.localRotation = Quaternion.identity;
                child.transform.localScale = Vector3.one;
                SetLayerRecursively(child, parent.layer);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetStaticEditorFlags(GameObject go, StaticEditorFlags flags);

        internal enum ShouldIncludeChildren
        {
            Cancel = 2,
            DontIncludeChildren = 1,
            HasNoChildren = -1,
            IncludeChildren = 0
        }
    }
}

