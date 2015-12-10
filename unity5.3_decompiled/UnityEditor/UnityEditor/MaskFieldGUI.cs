namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal static class MaskFieldGUI
    {
        internal static int DoMaskField(Rect position, int controlID, int mask, string[] flagNames, GUIStyle style)
        {
            int num;
            bool flag;
            return DoMaskField(position, controlID, mask, flagNames, style, out num, out flag);
        }

        internal static int DoMaskField(Rect position, int controlID, int mask, string[] flagNames, GUIStyle style, out int changedFlags, out bool changedToValue)
        {
            Event event2;
            mask = MaskCallbackInfo.GetSelectedValueForControl(controlID, mask, out changedFlags, out changedToValue);
            List<int> list = new List<int>();
            List<string> list2 = new List<string> { "Nothing", "Everything" };
            for (int i = 0; i < flagNames.Length; i++)
            {
                if ((mask & (((int) 1) << i)) != 0)
                {
                    list.Add(i + 2);
                }
            }
            list2.AddRange(flagNames);
            GUIContent mixedValueContent = EditorGUI.mixedValueContent;
            if (!EditorGUI.showMixedValue)
            {
                switch (list.Count)
                {
                    case 0:
                        mixedValueContent = EditorGUIUtility.TempContent("Nothing");
                        list.Add(0);
                        goto Label_00F9;

                    case 1:
                        mixedValueContent = new GUIContent(list2[list[0]]);
                        goto Label_00F9;
                }
                if (list.Count >= flagNames.Length)
                {
                    mixedValueContent = EditorGUIUtility.TempContent("Everything");
                    list.Add(1);
                    mask = -1;
                }
                else
                {
                    mixedValueContent = EditorGUIUtility.TempContent("Mixed ...");
                }
            }
        Label_00F9:
            event2 = Event.current;
            if (event2.type == EventType.Repaint)
            {
                style.Draw(position, mixedValueContent, controlID, false);
                return mask;
            }
            if (((event2.type == EventType.MouseDown) && position.Contains(event2.mousePosition)) || event2.MainActionKeyForControl(controlID))
            {
                MaskCallbackInfo.m_Instance = new MaskCallbackInfo(controlID);
                event2.Use();
                EditorUtility.DisplayCustomMenu(position, list2.ToArray(), !EditorGUI.showMixedValue ? list.ToArray() : new int[0], new EditorUtility.SelectMenuItemFunction(MaskCallbackInfo.m_Instance.SetMaskValueDelegate), null);
            }
            return mask;
        }

        private class MaskCallbackInfo
        {
            private const string kMaskMenuChangedMessage = "MaskMenuChanged";
            private bool m_ClearAll;
            private readonly int m_ControlID;
            private bool m_DoNothing;
            public static MaskFieldGUI.MaskCallbackInfo m_Instance;
            private int m_Mask;
            private bool m_SetAll;
            private readonly GUIView m_SourceView;

            public MaskCallbackInfo(int controlID)
            {
                this.m_ControlID = controlID;
                this.m_SourceView = GUIView.current;
            }

            public static int GetSelectedValueForControl(int controlID, int mask, out int changedFlags, out bool changedToValue)
            {
                Event current = Event.current;
                changedFlags = 0;
                changedToValue = false;
                if ((current.type == EventType.ExecuteCommand) && (current.commandName == "MaskMenuChanged"))
                {
                    if (m_Instance == null)
                    {
                        Debug.LogError("Mask menu has no instance");
                        return mask;
                    }
                    if (m_Instance.m_ControlID != controlID)
                    {
                        return mask;
                    }
                    if (!m_Instance.m_DoNothing)
                    {
                        if (m_Instance.m_ClearAll)
                        {
                            mask = 0;
                            changedFlags = -1;
                            changedToValue = false;
                        }
                        else if (m_Instance.m_SetAll)
                        {
                            mask = -1;
                            changedFlags = -1;
                            changedToValue = true;
                        }
                        else
                        {
                            mask ^= m_Instance.m_Mask;
                            changedFlags = m_Instance.m_Mask;
                            changedToValue = (mask & m_Instance.m_Mask) != 0;
                        }
                        GUI.changed = true;
                    }
                    m_Instance.m_DoNothing = false;
                    m_Instance.m_ClearAll = false;
                    m_Instance.m_SetAll = false;
                    m_Instance = null;
                    current.Use();
                }
                return mask;
            }

            internal void SetMaskValueDelegate(object userData, string[] options, int selected)
            {
                switch (selected)
                {
                    case 0:
                        this.m_ClearAll = true;
                        break;

                    case 1:
                        this.m_SetAll = true;
                        break;

                    default:
                        this.m_Mask = ((int) 1) << (selected - 2);
                        break;
                }
                if (this.m_SourceView != null)
                {
                    this.m_SourceView.SendEvent(EditorGUIUtility.CommandEvent("MaskMenuChanged"));
                }
            }
        }
    }
}

