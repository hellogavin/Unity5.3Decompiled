namespace UnityEditorInternal
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal static class CurveBindingUtility
    {
        private static GameObject s_Root;

        public static void Cleanup()
        {
            if (s_Root != null)
            {
                Object.DestroyImmediate(s_Root);
                EditorUtility.UnloadUnusedAssetsImmediate();
            }
        }

        private static GameObject CreateNewGameObject(Transform parent, string name)
        {
            GameObject obj2 = new GameObject(name);
            if (parent != null)
            {
                obj2.transform.parent = parent;
            }
            obj2.hideFlags = HideFlags.HideAndDontSave;
            return obj2;
        }

        private static GameObject CreateOrGetGameObject(string path)
        {
            if (s_Root == null)
            {
                s_Root = CreateNewGameObject(null, "Root");
            }
            if (path.Length == 0)
            {
                return s_Root;
            }
            char[] separator = new char[] { '/' };
            string[] strArray = path.Split(separator);
            Transform parent = s_Root.transform;
            foreach (string str in strArray)
            {
                Transform transform2 = parent.FindChild(str);
                if (transform2 == null)
                {
                    parent = CreateNewGameObject(parent, str).transform;
                }
                else
                {
                    parent = transform2;
                }
            }
            return parent.gameObject;
        }

        private static object GetCurrentValue(EditorCurveBinding curveBinding)
        {
            PrepareHierarchy(curveBinding);
            return AnimationWindowUtility.GetCurrentValue(s_Root, curveBinding);
        }

        public static object GetCurrentValue(GameObject rootGameObject, EditorCurveBinding curveBinding)
        {
            if (rootGameObject != null)
            {
                return AnimationWindowUtility.GetCurrentValue(rootGameObject, curveBinding);
            }
            return GetCurrentValue(curveBinding);
        }

        private static Type GetEditorCurveValueType(EditorCurveBinding curveBinding)
        {
            PrepareHierarchy(curveBinding);
            return AnimationUtility.GetEditorCurveValueType(s_Root, curveBinding);
        }

        public static Type GetEditorCurveValueType(GameObject rootGameObject, EditorCurveBinding curveBinding)
        {
            if (rootGameObject != null)
            {
                return AnimationUtility.GetEditorCurveValueType(rootGameObject, curveBinding);
            }
            return GetEditorCurveValueType(curveBinding);
        }

        private static void PrepareHierarchy(EditorCurveBinding curveBinding)
        {
            GameObject obj2 = CreateOrGetGameObject(curveBinding.path);
            if (obj2.GetComponent(curveBinding.type) == null)
            {
                obj2.AddComponent(curveBinding.type);
            }
        }

        private static void PrepareHierarchy(AnimationClip clip)
        {
            foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip))
            {
                GameObject obj2 = CreateOrGetGameObject(binding.path);
                if (obj2.GetComponent(binding.type) == null)
                {
                    obj2.AddComponent(binding.type);
                }
            }
        }

        private static void SampleAnimationClip(AnimationClip clip, float time)
        {
            PrepareHierarchy(clip);
            AnimationMode.SampleAnimationClip(s_Root, clip, time);
        }

        public static void SampleAnimationClip(GameObject rootGameObject, AnimationClip clip, float time)
        {
            if (rootGameObject != null)
            {
                AnimationMode.SampleAnimationClip(rootGameObject, clip, time);
            }
            else
            {
                SampleAnimationClip(clip, time);
            }
        }
    }
}

