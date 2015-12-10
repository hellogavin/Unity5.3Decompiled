namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class BodyMaskEditor
    {
        protected static Color[] m_MaskBodyPartPicker = new Color[] { new Color(1f, 0.5647059f, 0f), new Color(0f, 0.682353f, 0.9411765f), new Color(0.6705883f, 0.627451f, 0f), new Color(0f, 1f, 1f), new Color(0.9686275f, 0.5921569f, 0.4745098f), new Color(0f, 1f, 0f), new Color(0.3372549f, 0.454902f, 0.7254902f), new Color(1f, 1f, 0f), new Color(0.509804f, 0.7921569f, 0.6117647f), new Color(0.3215686f, 0.3215686f, 0.3215686f), new Color(1f, 0.4509804f, 0.4509804f), new Color(0.6235294f, 0.6235294f, 0.6235294f), new Color(0.7921569f, 0.7921569f, 0.7921569f), new Color(0.3960784f, 0.3960784f, 0.3960784f) };
        private static int s_Hint = sAvatarBodyMaskStr.GetHashCode();
        private static string sAvatarBodyMaskStr = "AvatarMask";
        private static Styles styles = new Styles();

        protected static void DoPicking(Rect rect, SerializedProperty bodyMask, int count)
        {
            if (styles.PickingTexture.image != null)
            {
                int controlID = GUIUtility.GetControlID(s_Hint, FocusType.Native, rect);
                Event current = Event.current;
                if ((current.GetTypeForControl(controlID) == EventType.MouseDown) && rect.Contains(current.mousePosition))
                {
                    current.Use();
                    int x = ((int) current.mousePosition.x) - ((int) rect.x);
                    int y = styles.UnityDude.image.height - (((int) current.mousePosition.y) - ((int) rect.y));
                    Color pixel = (styles.PickingTexture.image as Texture2D).GetPixel(x, y);
                    bool flag = false;
                    for (int i = 0; i < count; i++)
                    {
                        if (m_MaskBodyPartPicker[i] == pixel)
                        {
                            GUI.changed = true;
                            bodyMask.GetArrayElementAtIndex(i).intValue = (bodyMask.GetArrayElementAtIndex(i).intValue != 1) ? 1 : 0;
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        bool flag2 = false;
                        for (int j = 0; (j < count) && !flag2; j++)
                        {
                            flag2 = bodyMask.GetArrayElementAtIndex(j).intValue == 1;
                        }
                        for (int k = 0; k < count; k++)
                        {
                            bodyMask.GetArrayElementAtIndex(k).intValue = flag2 ? 0 : 1;
                        }
                        GUI.changed = true;
                    }
                }
            }
        }

        public static void Show(SerializedProperty bodyMask, int count)
        {
            if (styles.UnityDude.image != null)
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth((float) styles.UnityDude.image.width) };
                Rect position = GUILayoutUtility.GetRect(styles.UnityDude, GUIStyle.none, options);
                position.x += (GUIView.current.position.width - position.width) / 2f;
                Color color = GUI.color;
                GUI.color = (bodyMask.GetArrayElementAtIndex(0).intValue != 1) ? Color.red : Color.green;
                if (styles.BodyPart[0].image != null)
                {
                    GUI.DrawTexture(position, styles.BodyPart[0].image);
                }
                GUI.color = new Color(0.2f, 0.2f, 0.2f, 1f);
                GUI.DrawTexture(position, styles.UnityDude.image);
                for (int i = 1; i < count; i++)
                {
                    GUI.color = (bodyMask.GetArrayElementAtIndex(i).intValue != 1) ? Color.red : Color.green;
                    if (styles.BodyPart[i].image != null)
                    {
                        GUI.DrawTexture(position, styles.BodyPart[i].image);
                    }
                }
                GUI.color = color;
                DoPicking(position, bodyMask, count);
            }
        }

        private class Styles
        {
            public GUIContent[] BodyPart = new GUIContent[] { EditorGUIUtility.IconContent("AvatarInspector/MaskEditor_Root"), EditorGUIUtility.IconContent("AvatarInspector/Torso"), EditorGUIUtility.IconContent("AvatarInspector/Head"), EditorGUIUtility.IconContent("AvatarInspector/LeftLeg"), EditorGUIUtility.IconContent("AvatarInspector/RightLeg"), EditorGUIUtility.IconContent("AvatarInspector/LeftArm"), EditorGUIUtility.IconContent("AvatarInspector/RightArm"), EditorGUIUtility.IconContent("AvatarInspector/LeftFingers"), EditorGUIUtility.IconContent("AvatarInspector/RightFingers"), EditorGUIUtility.IconContent("AvatarInspector/LeftFeetIk"), EditorGUIUtility.IconContent("AvatarInspector/RightFeetIk"), EditorGUIUtility.IconContent("AvatarInspector/LeftFingersIk"), EditorGUIUtility.IconContent("AvatarInspector/RightFingersIk") };
            public GUIContent PickingTexture = EditorGUIUtility.IconContent("AvatarInspector/BodyPartPicker");
            public GUIContent UnityDude = EditorGUIUtility.IconContent("AvatarInspector/BodySIlhouette");
        }
    }
}

