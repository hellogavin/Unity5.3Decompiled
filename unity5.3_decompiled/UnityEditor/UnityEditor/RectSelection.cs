namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class RectSelection
    {
        private Object[] m_CurrentSelection;
        private Dictionary<GameObject, bool> m_LastSelection;
        private bool m_RectSelecting;
        private Object[] m_SelectionStart;
        private Vector2 m_SelectMousePoint;
        private Vector2 m_SelectStartPoint;
        private EditorWindow m_Window;
        private static int s_RectSelectionID = GUIUtility.GetPermanentControlID();

        public RectSelection(EditorWindow window)
        {
            this.m_Window = window;
        }

        public void OnGUI()
        {
            Event current = Event.current;
            Handles.BeginGUI();
            Vector2 mousePosition = current.mousePosition;
            int controlID = s_RectSelectionID;
            EventType typeForControl = current.GetTypeForControl(controlID);
            switch (typeForControl)
            {
                case EventType.MouseDown:
                    if ((HandleUtility.nearestControl == controlID) && (current.button == 0))
                    {
                        GUIUtility.hotControl = controlID;
                        this.m_SelectStartPoint = mousePosition;
                        this.m_SelectionStart = Selection.objects;
                        this.m_RectSelecting = false;
                    }
                    goto Label_04D2;

                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == controlID) && (current.button == 0))
                    {
                        GUIUtility.hotControl = 0;
                        if (!this.m_RectSelecting)
                        {
                            if (current.shift || EditorGUI.actionKey)
                            {
                                GameObject[] gameObjects = !current.shift ? Selection.gameObjects : new GameObject[] { Selection.activeGameObject };
                                GameObject hovered = SceneViewPicking.GetHovered(current.mousePosition, gameObjects);
                                if (hovered != null)
                                {
                                    UpdateSelection(this.m_SelectionStart, hovered, SelectionType.Subtractive, this.m_RectSelecting);
                                }
                                else
                                {
                                    UpdateSelection(this.m_SelectionStart, HandleUtility.PickGameObject(current.mousePosition, true), SelectionType.Additive, this.m_RectSelecting);
                                }
                            }
                            else
                            {
                                GameObject newObject = SceneViewPicking.PickGameObject(current.mousePosition);
                                UpdateSelection(this.m_SelectionStart, newObject, SelectionType.Normal, this.m_RectSelecting);
                            }
                            current.Use();
                        }
                        else
                        {
                            EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.SendCommandsOnModifierKeys));
                            this.m_RectSelecting = false;
                            this.m_SelectionStart = new Object[0];
                            current.Use();
                        }
                    }
                    goto Label_04D2;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl != controlID)
                    {
                        goto Label_04D2;
                    }
                    if (!this.m_RectSelecting)
                    {
                        Vector2 vector2 = mousePosition - this.m_SelectStartPoint;
                        if (vector2.magnitude > 6f)
                        {
                            EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.SendCommandsOnModifierKeys));
                            this.m_RectSelecting = true;
                            this.m_LastSelection = null;
                            this.m_CurrentSelection = null;
                        }
                    }
                    if (this.m_RectSelecting)
                    {
                        float x = Mathf.Max(mousePosition.x, 0f);
                        this.m_SelectMousePoint = new Vector2(x, Mathf.Max(mousePosition.y, 0f));
                        GameObject[] newObjects = HandleUtility.PickRectObjects(EditorGUIExt.FromToRect(this.m_SelectStartPoint, this.m_SelectMousePoint));
                        this.m_CurrentSelection = newObjects;
                        bool flag = false;
                        if (this.m_LastSelection == null)
                        {
                            this.m_LastSelection = new Dictionary<GameObject, bool>();
                            flag = true;
                        }
                        flag |= this.m_LastSelection.Count != newObjects.Length;
                        if (!flag)
                        {
                            Dictionary<GameObject, bool> dictionary = new Dictionary<GameObject, bool>(newObjects.Length);
                            foreach (GameObject obj2 in newObjects)
                            {
                                dictionary.Add(obj2, false);
                            }
                            foreach (GameObject obj3 in this.m_LastSelection.Keys)
                            {
                                if (!dictionary.ContainsKey(obj3))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                        if (flag)
                        {
                            this.m_LastSelection = new Dictionary<GameObject, bool>(newObjects.Length);
                            foreach (GameObject obj4 in newObjects)
                            {
                                this.m_LastSelection.Add(obj4, false);
                            }
                            if (newObjects != null)
                            {
                                if (current.shift)
                                {
                                    UpdateSelection(this.m_SelectionStart, newObjects, SelectionType.Additive, this.m_RectSelecting);
                                }
                                else if (EditorGUI.actionKey)
                                {
                                    UpdateSelection(this.m_SelectionStart, newObjects, SelectionType.Subtractive, this.m_RectSelecting);
                                }
                                else
                                {
                                    UpdateSelection(this.m_SelectionStart, newObjects, SelectionType.Normal, this.m_RectSelecting);
                                }
                            }
                        }
                    }
                    break;

                case EventType.Repaint:
                    if ((GUIUtility.hotControl == controlID) && this.m_RectSelecting)
                    {
                        EditorStyles.selectionRect.Draw(EditorGUIExt.FromToRect(this.m_SelectStartPoint, this.m_SelectMousePoint), GUIContent.none, false, false, false, false);
                    }
                    goto Label_04D2;

                case EventType.Layout:
                    if (!Tools.viewToolActive)
                    {
                        HandleUtility.AddDefaultControl(controlID);
                    }
                    goto Label_04D2;

                default:
                    if ((typeForControl == EventType.ExecuteCommand) && ((controlID == GUIUtility.hotControl) && (current.commandName == "ModifierKeysChanged")))
                    {
                        if (current.shift)
                        {
                            UpdateSelection(this.m_SelectionStart, this.m_CurrentSelection, SelectionType.Additive, this.m_RectSelecting);
                        }
                        else if (EditorGUI.actionKey)
                        {
                            UpdateSelection(this.m_SelectionStart, this.m_CurrentSelection, SelectionType.Subtractive, this.m_RectSelecting);
                        }
                        else
                        {
                            UpdateSelection(this.m_SelectionStart, this.m_CurrentSelection, SelectionType.Normal, this.m_RectSelecting);
                        }
                        current.Use();
                    }
                    goto Label_04D2;
            }
            current.Use();
        Label_04D2:
            Handles.EndGUI();
        }

        internal void SendCommandsOnModifierKeys()
        {
            this.m_Window.SendEvent(EditorGUIUtility.CommandEvent("ModifierKeysChanged"));
        }

        private static void UpdateSelection(Object[] existingSelection, Object newObject, SelectionType type, bool isRectSelection)
        {
            Object[] objArray;
            if (newObject == null)
            {
                objArray = new Object[0];
            }
            else
            {
                objArray = new Object[] { newObject };
            }
            UpdateSelection(existingSelection, objArray, type, isRectSelection);
        }

        private static void UpdateSelection(Object[] existingSelection, Object[] newObjects, SelectionType type, bool isRectSelection)
        {
            Object[] objArray;
            switch (type)
            {
                case SelectionType.Additive:
                    if (newObjects.Length <= 0)
                    {
                        Selection.objects = existingSelection;
                        return;
                    }
                    objArray = new Object[existingSelection.Length + newObjects.Length];
                    Array.Copy(existingSelection, objArray, existingSelection.Length);
                    for (int i = 0; i < newObjects.Length; i++)
                    {
                        objArray[existingSelection.Length + i] = newObjects[i];
                    }
                    if (!isRectSelection)
                    {
                        Selection.activeObject = newObjects[0];
                    }
                    else
                    {
                        Selection.activeObject = objArray[0];
                    }
                    Selection.objects = objArray;
                    return;

                case SelectionType.Subtractive:
                {
                    Dictionary<Object, bool> dictionary = new Dictionary<Object, bool>(existingSelection.Length);
                    foreach (Object obj2 in existingSelection)
                    {
                        dictionary.Add(obj2, false);
                    }
                    foreach (Object obj3 in newObjects)
                    {
                        if (dictionary.ContainsKey(obj3))
                        {
                            dictionary.Remove(obj3);
                        }
                    }
                    objArray = new Object[dictionary.Keys.Count];
                    dictionary.Keys.CopyTo(objArray, 0);
                    Selection.objects = objArray;
                    return;
                }
            }
            Selection.objects = newObjects;
        }

        private enum SelectionType
        {
            Normal,
            Additive,
            Subtractive
        }
    }
}

