namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class LayerMatrixGUI
    {
        public static void DoGUI(string title, ref bool show, ref Vector2 scrollPos, GetValueFunc getValue, SetValueFunc setValue)
        {
            int num5 = 0;
            for (int i = 0; i < 0x20; i++)
            {
                if (LayerMask.LayerToName(i) != string.Empty)
                {
                    num5++;
                }
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(0f);
            show = EditorGUILayout.Foldout(show, title);
            GUILayout.EndHorizontal();
            if (show)
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinHeight(120f), GUILayout.MaxHeight((float) (100 + ((num5 + 1) * 0x10))) };
                scrollPos = GUILayout.BeginScrollView(scrollPos, options);
                Rect rect = GUILayoutUtility.GetRect((float) ((0x10 * num5) + 100), 100f);
                Rect topmostRect = GUIClip.topmostRect;
                Vector2 vector = GUIClip.Unclip(new Vector2(rect.x, rect.y));
                int num7 = 0;
                for (int j = 0; j < 0x20; j++)
                {
                    if (LayerMask.LayerToName(j) != string.Empty)
                    {
                        float num9 = (130 + ((num5 - num7) * 0x10)) - (topmostRect.width + scrollPos.x);
                        if (num9 < 0f)
                        {
                            num9 = 0f;
                        }
                        Vector3 pos = new Vector3(((((130 + (0x10 * (num5 - num7))) + vector.y) + vector.x) + scrollPos.y) - num9, vector.y + scrollPos.y, 0f);
                        GUI.matrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one) * Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f), Vector3.one);
                        if (SystemInfo.graphicsDeviceVersion.StartsWith("Direct3D 9.0"))
                        {
                            GUI.matrix *= Matrix4x4.TRS(new Vector3(-0.5f, -0.5f, 0f), Quaternion.identity, Vector3.one);
                        }
                        GUI.Label(new Rect((2f - vector.x) - scrollPos.y, scrollPos.y - num9, 100f, 16f), LayerMask.LayerToName(j), "RightLabel");
                        num7++;
                    }
                }
                GUI.matrix = Matrix4x4.identity;
                num7 = 0;
                for (int k = 0; k < 0x20; k++)
                {
                    if (LayerMask.LayerToName(k) != string.Empty)
                    {
                        int num11 = 0;
                        Rect rect3 = GUILayoutUtility.GetRect((float) ((30 + (0x10 * num5)) + 100), 16f);
                        GUI.Label(new Rect(rect3.x + 30f, rect3.y, 100f, 16f), LayerMask.LayerToName(k), "RightLabel");
                        for (int m = 0x1f; m >= 0; m--)
                        {
                            if (LayerMask.LayerToName(m) != string.Empty)
                            {
                                if (num11 < (num5 - num7))
                                {
                                    GUIContent content = new GUIContent(string.Empty, LayerMask.LayerToName(k) + "/" + LayerMask.LayerToName(m));
                                    bool flag = getValue(k, m);
                                    bool val = GUI.Toggle(new Rect((130f + rect3.x) + (num11 * 0x10), rect3.y, 16f, 16f), flag, content);
                                    if (val != flag)
                                    {
                                        setValue(k, m, val);
                                    }
                                }
                                num11++;
                            }
                        }
                        num7++;
                    }
                }
                GUILayout.EndScrollView();
            }
        }

        public delegate bool GetValueFunc(int layerA, int layerB);

        public delegate void SetValueFunc(int layerA, int layerB, bool val);
    }
}

