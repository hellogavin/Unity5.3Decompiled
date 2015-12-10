namespace UnityEditor
{
    using System;
    using UnityEngine;
    using UnityEngine.Rendering;

    [CustomEditor(typeof(ShaderVariantCollection))]
    internal class ShaderVariantCollectionInspector : Editor
    {
        private SerializedProperty m_Shaders;

        private void DisplayAddVariantsWindow(Shader shader, ShaderVariantCollection collection)
        {
            string[] strArray;
            AddShaderVariantWindow.PopupData data = new AddShaderVariantWindow.PopupData {
                shader = shader,
                collection = collection
            };
            ShaderUtil.GetShaderVariantEntries(shader, collection, out data.types, out strArray);
            if (strArray.Length == 0)
            {
                EditorApplication.Beep();
            }
            else
            {
                data.keywords = new string[strArray.Length][];
                for (int i = 0; i < strArray.Length; i++)
                {
                    char[] separator = new char[] { ' ' };
                    data.keywords[i] = strArray[i].Split(separator);
                }
                AddShaderVariantWindow.ShowAddVariantWindow(data);
                GUIUtility.ExitGUI();
            }
        }

        private void DrawShaderEntry(int shaderIndex)
        {
            SerializedProperty arrayElementAtIndex = this.m_Shaders.GetArrayElementAtIndex(shaderIndex);
            Shader objectReferenceValue = (Shader) arrayElementAtIndex.FindPropertyRelative("first").objectReferenceValue;
            SerializedProperty property2 = arrayElementAtIndex.FindPropertyRelative("second.variants");
            using (new GUILayout.HorizontalScope(new GUILayoutOption[0]))
            {
                Rect r = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.boldLabel);
                Rect addRemoveButtonRect = GetAddRemoveButtonRect(r);
                r.xMax = addRemoveButtonRect.x;
                GUI.Label(r, objectReferenceValue.name, EditorStyles.boldLabel);
                if (GUI.Button(addRemoveButtonRect, Styles.iconRemove, Styles.invisibleButton))
                {
                    this.m_Shaders.DeleteArrayElementAtIndex(shaderIndex);
                    return;
                }
            }
            for (int i = 0; i < property2.arraySize; i++)
            {
                SerializedProperty property3 = property2.GetArrayElementAtIndex(i);
                string stringValue = property3.FindPropertyRelative("keywords").stringValue;
                if (string.IsNullOrEmpty(stringValue))
                {
                    stringValue = "<no keywords>";
                }
                PassType intValue = (PassType) property3.FindPropertyRelative("passType").intValue;
                Rect rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.miniLabel);
                Rect position = GetAddRemoveButtonRect(rect);
                rect.xMax = position.x;
                GUI.Label(rect, intValue + " " + stringValue, EditorStyles.miniLabel);
                if (GUI.Button(position, Styles.iconRemove, Styles.invisibleButton))
                {
                    property2.DeleteArrayElementAtIndex(i);
                }
            }
            if (GUI.Button(GetAddRemoveButtonRect(GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.miniLabel)), Styles.iconAdd, Styles.invisibleButton))
            {
                this.DisplayAddVariantsWindow(objectReferenceValue, this.target as ShaderVariantCollection);
            }
        }

        private static Rect GetAddRemoveButtonRect(Rect r)
        {
            Vector2 vector = Styles.invisibleButton.CalcSize(Styles.iconRemove);
            return new Rect(r.xMax - vector.x, r.y + ((int) ((r.height / 2f) - (vector.y / 2f))), vector.x, vector.y);
        }

        public virtual void OnEnable()
        {
            this.m_Shaders = base.serializedObject.FindProperty("m_Shaders");
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            for (int i = 0; i < this.m_Shaders.arraySize; i++)
            {
                this.DrawShaderEntry(i);
            }
            if (GUILayout.Button("Add shader", new GUILayoutOption[0]))
            {
                ObjectSelector.get.Show(null, typeof(Shader), null, false);
                ObjectSelector.get.objectSelectorID = "ShaderVariantSelector".GetHashCode();
                GUIUtility.ExitGUI();
            }
            if (((Event.current.type == EventType.ExecuteCommand) && (Event.current.commandName == "ObjectSelectorClosed")) && (ObjectSelector.get.objectSelectorID == "ShaderVariantSelector".GetHashCode()))
            {
                Shader currentObject = ObjectSelector.GetCurrentObject() as Shader;
                if (currentObject != null)
                {
                    ShaderUtil.AddNewShaderToCollection(currentObject, this.target as ShaderVariantCollection);
                }
                Event.current.Use();
                GUIUtility.ExitGUI();
            }
            base.serializedObject.ApplyModifiedProperties();
        }

        private class Styles
        {
            public static readonly GUIContent iconAdd = EditorGUIUtility.IconContent("Toolbar Plus", "Add variant");
            public static readonly GUIContent iconRemove = EditorGUIUtility.IconContent("Toolbar Minus", "Remove entry");
            public static readonly GUIStyle invisibleButton = "InvisibleButton";
        }
    }
}

