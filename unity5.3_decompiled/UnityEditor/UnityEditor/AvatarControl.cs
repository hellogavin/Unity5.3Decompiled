namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class AvatarControl
    {
        private static Vector2[,] s_BonePositions = new Vector2[4, HumanTrait.BoneCount];
        private static Styles s_Styles;

        static AvatarControl()
        {
            int num = 0;
            s_BonePositions[num, 0] = new Vector2(0f, 0.08f);
            s_BonePositions[num, 1] = new Vector2(0.16f, 0.01f);
            s_BonePositions[num, 2] = new Vector2(-0.16f, 0.01f);
            s_BonePositions[num, 3] = new Vector2(0.21f, -0.4f);
            s_BonePositions[num, 4] = new Vector2(-0.21f, -0.4f);
            s_BonePositions[num, 5] = new Vector2(0.23f, -0.8f);
            s_BonePositions[num, 6] = new Vector2(-0.23f, -0.8f);
            s_BonePositions[num, 7] = new Vector2(0f, 0.25f);
            s_BonePositions[num, 8] = new Vector2(0f, 0.43f);
            s_BonePositions[num, 9] = new Vector2(0f, 0.66f);
            s_BonePositions[num, 10] = new Vector2(0f, 0.76f);
            s_BonePositions[num, 11] = new Vector2(0.14f, 0.6f);
            s_BonePositions[num, 12] = new Vector2(-0.14f, 0.6f);
            s_BonePositions[num, 13] = new Vector2(0.3f, 0.57f);
            s_BonePositions[num, 14] = new Vector2(-0.3f, 0.57f);
            s_BonePositions[num, 15] = new Vector2(0.48f, 0.3f);
            s_BonePositions[num, 0x10] = new Vector2(-0.48f, 0.3f);
            s_BonePositions[num, 0x11] = new Vector2(0.66f, 0.03f);
            s_BonePositions[num, 0x12] = new Vector2(-0.66f, 0.03f);
            s_BonePositions[num, 0x13] = new Vector2(0.25f, -0.89f);
            s_BonePositions[num, 20] = new Vector2(-0.25f, -0.89f);
            num = 1;
            s_BonePositions[num, 9] = new Vector2(-0.2f, -0.62f);
            s_BonePositions[num, 10] = new Vector2(-0.15f, -0.3f);
            s_BonePositions[num, 0x15] = new Vector2(0.63f, 0.16f);
            s_BonePositions[num, 0x16] = new Vector2(0.15f, 0.16f);
            s_BonePositions[num, 0x17] = new Vector2(0.45f, -0.4f);
            num = 2;
            s_BonePositions[num, 0x18] = new Vector2(-0.35f, 0.11f);
            s_BonePositions[num, 0x1b] = new Vector2(0.19f, 0.11f);
            s_BonePositions[num, 30] = new Vector2(0.22f, 0f);
            s_BonePositions[num, 0x21] = new Vector2(0.16f, -0.12f);
            s_BonePositions[num, 0x24] = new Vector2(0.09f, -0.23f);
            s_BonePositions[num, 0x1a] = new Vector2(-0.03f, 0.33f);
            s_BonePositions[num, 0x1d] = new Vector2(0.65f, 0.16f);
            s_BonePositions[num, 0x20] = new Vector2(0.74f, 0f);
            s_BonePositions[num, 0x23] = new Vector2(0.66f, -0.14f);
            s_BonePositions[num, 0x26] = new Vector2(0.45f, -0.25f);
            for (int i = 0; i < 5; i++)
            {
                s_BonePositions[num, 0x19 + (i * 3)] = Vector2.Lerp(s_BonePositions[num, 0x18 + (i * 3)], s_BonePositions[num, 0x1a + (i * 3)], 0.58f);
            }
            num = 3;
            for (int j = 0; j < 15; j++)
            {
                s_BonePositions[num, (0x18 + j) + 15] = Vector2.Scale(s_BonePositions[num - 1, 0x18 + j], new Vector2(-1f, 1f));
            }
        }

        protected static void DrawBodyPart(int shownBodyView, int i, Rect rect, BodyPartColor bodyPartColor)
        {
            if ((styles.BodyPart[shownBodyView, i] != null) && (styles.BodyPart[shownBodyView, i].image != null))
            {
                if ((bodyPartColor & BodyPartColor.Green) == BodyPartColor.Green)
                {
                    GUI.color = Color.green;
                }
                else if ((bodyPartColor & BodyPartColor.Red) == BodyPartColor.Red)
                {
                    GUI.color = Color.red;
                }
                else
                {
                    GUI.color = Color.gray;
                }
                GUI.DrawTexture(rect, styles.BodyPart[shownBodyView, i].image);
                GUI.color = Color.white;
            }
        }

        public static void DrawBodyParts(Rect rect, int shownBodyView, BodyPartFeedback bodyPartCallback)
        {
            GUI.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            if (styles.Silhouettes[shownBodyView] != null)
            {
                GUI.DrawTexture(rect, styles.Silhouettes[shownBodyView].image);
            }
            for (int i = 1; i < 9; i++)
            {
                DrawBodyPart(shownBodyView, i, rect, bodyPartCallback((BodyPart) i));
            }
        }

        protected static void DrawBone(int shownBodyView, int i, Rect rect, AvatarSetupTool.BoneWrapper bone, SerializedObject serializedObject, AvatarMappingEditor editor)
        {
            if (s_BonePositions[shownBodyView, i] != Vector2.zero)
            {
                Vector2 vector = s_BonePositions[shownBodyView, i];
                vector.y *= -1f;
                vector.Scale(new Vector2(rect.width * 0.5f, rect.height * 0.5f));
                vector = rect.center + vector;
                int num = 0x13;
                Rect rect2 = new Rect(vector.x - (num * 0.5f), vector.y - (num * 0.5f), (float) num, (float) num);
                bone.BoneDotGUI(rect2, i, true, true, serializedObject, editor);
            }
        }

        public static List<int> GetViewsThatContainBone(int bone)
        {
            List<int> list = new List<int>();
            if ((bone >= 0) && (bone < HumanTrait.BoneCount))
            {
                for (int i = 0; i < 4; i++)
                {
                    if (s_BonePositions[i, bone] != Vector2.zero)
                    {
                        list.Add(i);
                    }
                }
            }
            return list;
        }

        public static int ShowBoneMapping(int shownBodyView, BodyPartFeedback bodyPartCallback, AvatarSetupTool.BoneWrapper[] bones, SerializedObject serializedObject, AvatarMappingEditor editor)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if (styles.Silhouettes[shownBodyView].image != null)
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth((float) styles.Silhouettes[shownBodyView].image.width) };
                Rect rect = GUILayoutUtility.GetRect(styles.Silhouettes[shownBodyView], GUIStyle.none, options);
                DrawBodyParts(rect, shownBodyView, bodyPartCallback);
                for (int j = 0; j < bones.Length; j++)
                {
                    DrawBone(shownBodyView, j, rect, bones[j], serializedObject, editor);
                }
            }
            else
            {
                GUILayout.Label("texture missing,\nfix me!", new GUILayoutOption[0]);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            Rect lastRect = GUILayoutUtility.GetLastRect();
            string[] strArray = new string[] { "Body", "Head", "Left Hand", "Right Hand" };
            lastRect.x += 5f;
            lastRect.width = 70f;
            lastRect.yMin = lastRect.yMax - 69f;
            lastRect.height = 16f;
            for (int i = 0; i < strArray.Length; i++)
            {
                if (GUI.Toggle(lastRect, shownBodyView == i, strArray[i], EditorStyles.miniButton))
                {
                    shownBodyView = i;
                }
                lastRect.y += 16f;
            }
            return shownBodyView;
        }

        private static Styles styles
        {
            get
            {
                if (s_Styles == null)
                {
                    s_Styles = new Styles();
                }
                return s_Styles;
            }
        }

        public enum BodyPartColor
        {
            Green = 1,
            IKGreen = 4,
            IKRed = 8,
            Off = 0,
            Red = 2
        }

        public delegate AvatarControl.BodyPartColor BodyPartFeedback(BodyPart bodyPart);

        private class Styles
        {
            public GUIContent[,] BodyPart;
            public GUILayoutOption ButtonSize;
            public GUIContent[] Silhouettes = new GUIContent[] { EditorGUIUtility.IconContent("AvatarInspector/BodySilhouette"), EditorGUIUtility.IconContent("AvatarInspector/HeadZoomSilhouette"), EditorGUIUtility.IconContent("AvatarInspector/LeftHandZoomSilhouette"), EditorGUIUtility.IconContent("AvatarInspector/RightHandZoomSilhouette") };

            public Styles()
            {
                GUIContent[] contentArray2 = new GUIContent[4, 9];
                contentArray2[0, 1] = EditorGUIUtility.IconContent("AvatarInspector/Torso");
                contentArray2[0, 2] = EditorGUIUtility.IconContent("AvatarInspector/Head");
                contentArray2[0, 3] = EditorGUIUtility.IconContent("AvatarInspector/LeftArm");
                contentArray2[0, 4] = EditorGUIUtility.IconContent("AvatarInspector/LeftFingers");
                contentArray2[0, 5] = EditorGUIUtility.IconContent("AvatarInspector/RightArm");
                contentArray2[0, 6] = EditorGUIUtility.IconContent("AvatarInspector/RightFingers");
                contentArray2[0, 7] = EditorGUIUtility.IconContent("AvatarInspector/LeftLeg");
                contentArray2[0, 8] = EditorGUIUtility.IconContent("AvatarInspector/RightLeg");
                contentArray2[1, 2] = EditorGUIUtility.IconContent("AvatarInspector/HeadZoom");
                contentArray2[2, 4] = EditorGUIUtility.IconContent("AvatarInspector/LeftHandZoom");
                contentArray2[3, 6] = EditorGUIUtility.IconContent("AvatarInspector/RightHandZoom");
                this.BodyPart = contentArray2;
                this.ButtonSize = GUILayout.MaxWidth(120f);
            }
        }
    }
}

