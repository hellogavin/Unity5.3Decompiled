namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    internal class AvatarSkeletonDrawer
    {
        private static Color kDummyColor = new Color(0.2352941f, 0.2352941f, 0.2352941f, 0.25f);
        private static Color kErrorColor = new Color(1f, 0f, 0f, 0.25f);
        private static Color kErrorMessageColor = new Color(1f, 0f, 0f, 0.75f);
        private static Color kHumanColor = new Color(0f, 0.8235294f, 0.2901961f, 0.25f);
        private static Color kSelectedColor = new Color(0.5019608f, 0.7529412f, 1f, 0.15f);
        private static Color kSkeletonColor = new Color(0.4039216f, 0.4039216f, 0.4039216f, 0.25f);
        private static bool sPoseError;

        private static void DrawPoseError(Transform node, Bounds bounds)
        {
            if (Camera.current != null)
            {
                GUIStyle style = new GUIStyle(GUI.skin.label) {
                    normal = { textColor = Color.red },
                    wordWrap = false,
                    alignment = TextAnchor.MiddleLeft
                };
                Vector3 position = node.position;
                Vector3 vector2 = node.position + ((Vector3) (Vector3.up * 0.2f));
                if (node.position.x <= node.root.position.x)
                {
                    vector2.x = bounds.min.x;
                }
                else
                {
                    vector2.x = bounds.max.x;
                }
                GUIContent content = new GUIContent(node.name);
                Rect rect = HandleUtility.WorldPointToSizedRect(vector2, content, style);
                rect.x += 2f;
                if (node.position.x > node.root.position.x)
                {
                    rect.x -= rect.width;
                }
                Handles.BeginGUI();
                rect.y -= style.CalcSize(content).y / 4f;
                GUI.Label(rect, content, style);
                Handles.EndGUI();
                Handles.color = kErrorMessageColor;
                Handles.DrawLine(position, vector2);
            }
        }

        public static void DrawSkeleton(Transform reference, Dictionary<Transform, bool> actualBones)
        {
            DrawSkeleton(reference, actualBones, null);
        }

        public static void DrawSkeleton(Transform reference, Dictionary<Transform, bool> actualBones, AvatarSetupTool.BoneWrapper[] bones)
        {
            if ((reference != null) && (actualBones != null))
            {
                sPoseError = false;
                Bounds bounds = new Bounds();
                Renderer[] componentsInChildren = reference.root.GetComponentsInChildren<Renderer>();
                if (componentsInChildren != null)
                {
                    foreach (Renderer renderer in componentsInChildren)
                    {
                        bounds.Encapsulate(renderer.bounds.min);
                        bounds.Encapsulate(renderer.bounds.max);
                    }
                }
                Quaternion identity = Quaternion.identity;
                if (bones != null)
                {
                    identity = AvatarSetupTool.AvatarComputeOrientation(bones);
                }
                DrawSkeletonSubTree(actualBones, bones, identity, reference, bounds);
                Camera current = Camera.current;
                if (sPoseError && (current != null))
                {
                    GUIStyle style = new GUIStyle(GUI.skin.label) {
                        normal = { textColor = Color.red },
                        wordWrap = false,
                        alignment = TextAnchor.MiddleLeft,
                        fontSize = 20
                    };
                    GUIContent content = new GUIContent("Character is not in T pose");
                    Rect position = GUILayoutUtility.GetRect(content, style);
                    position.x = 30f;
                    position.y = 30f;
                    Handles.BeginGUI();
                    GUI.Label(position, content, style);
                    Handles.EndGUI();
                }
            }
        }

        private static bool DrawSkeletonSubTree(Dictionary<Transform, bool> actualBones, AvatarSetupTool.BoneWrapper[] bones, Quaternion orientation, Transform tr, Bounds bounds)
        {
            if (!actualBones.ContainsKey(tr))
            {
                return false;
            }
            int num = 0;
            IEnumerator enumerator = tr.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    if (DrawSkeletonSubTree(actualBones, bones, orientation, current, bounds))
                    {
                        num++;
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            if (!actualBones[tr] && (num <= 1))
            {
                return false;
            }
            int boneIndex = -1;
            if (bones != null)
            {
                for (int i = 0; i < bones.Length; i++)
                {
                    if (bones[i].bone == tr)
                    {
                        boneIndex = i;
                        break;
                    }
                }
            }
            bool flag = AvatarSetupTool.GetBoneAlignmentError(bones, orientation, boneIndex) > 0f;
            sPoseError |= flag;
            if (flag)
            {
                DrawPoseError(tr, bounds);
                Handles.color = kErrorColor;
            }
            else if (boneIndex != -1)
            {
                Handles.color = kHumanColor;
            }
            else if (!actualBones[tr])
            {
                Handles.color = kDummyColor;
            }
            else
            {
                Handles.color = kSkeletonColor;
            }
            Handles.DoBoneHandle(tr, actualBones);
            if (Selection.activeObject == tr)
            {
                Handles.color = kSelectedColor;
                Handles.DoBoneHandle(tr, actualBones);
            }
            return true;
        }
    }
}

