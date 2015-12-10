namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;

    [CustomEditor(typeof(MonoImporter))]
    internal class MonoScriptImporterInspector : AssetImporterInspector
    {
        private SerializedProperty m_Icon;
        private const int m_RowHeight = 0x10;
        private SerializedObject m_TargetObject;
        private static GUIContent s_HelpIcon;
        private static GUIContent s_TitleSettingsIcon;

        private static bool IsTypeCompatible(Type type)
        {
            return ((type != null) && (type.IsSubclassOf(typeof(MonoBehaviour)) || type.IsSubclassOf(typeof(ScriptableObject))));
        }

        internal override void OnHeaderControlsGUI()
        {
            TextAsset target = this.assetEditor.target as TextAsset;
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Open...", EditorStyles.miniButton, new GUILayoutOption[0]))
            {
                AssetDatabase.OpenAsset(target);
                GUIUtility.ExitGUI();
            }
            if ((target is MonoScript) && GUILayout.Button("Execution Order...", EditorStyles.miniButton, new GUILayoutOption[0]))
            {
                EditorApplication.ExecuteMenuItem("Edit/Project Settings/Script Execution Order");
                GUIUtility.ExitGUI();
            }
        }

        internal override void OnHeaderIconGUI(Rect iconRect)
        {
            if (this.m_Icon == null)
            {
                this.m_TargetObject = new SerializedObject(this.assetEditor.targets);
                this.m_Icon = this.m_TargetObject.FindProperty("m_Icon");
            }
            EditorGUI.ObjectIconDropDown(iconRect, this.assetEditor.targets, true, null, this.m_Icon);
        }

        public override void OnInspectorGUI()
        {
            MonoImporter target = this.target as MonoImporter;
            MonoScript script = target.GetScript();
            if (script != null)
            {
                Type type = script.GetClass();
                if (!IsTypeCompatible(type))
                {
                    EditorGUILayout.HelpBox("No MonoBehaviour scripts in the file, or their names do not match the file name.", MessageType.Info);
                }
                Vector2 iconSize = EditorGUIUtility.GetIconSize();
                EditorGUIUtility.SetIconSize(new Vector2(16f, 16f));
                List<string> names = new List<string>();
                List<Object> objects = new List<Object>();
                bool didModify = false;
                this.ShowFieldInfo(type, target, names, objects, ref didModify);
                EditorGUIUtility.SetIconSize(iconSize);
                if (didModify)
                {
                    target.SetDefaultReferences(names.ToArray(), objects.ToArray());
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));
                }
            }
        }

        [MenuItem("CONTEXT/MonoImporter/Reset")]
        private static void ResetDefaultReferences(MenuCommand command)
        {
            MonoImporter context = command.context as MonoImporter;
            context.SetDefaultReferences(new string[0], new Object[0]);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(context));
        }

        private void ShowFieldInfo(Type type, MonoImporter importer, List<string> names, List<Object> objects, ref bool didModify)
        {
            if (IsTypeCompatible(type))
            {
                this.ShowFieldInfo(type.BaseType, importer, names, objects, ref didModify);
                foreach (FieldInfo info in type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    if (!info.IsPublic)
                    {
                        object[] customAttributes = info.GetCustomAttributes(typeof(SerializeField), true);
                        if ((customAttributes == null) || (customAttributes.Length == 0))
                        {
                            continue;
                        }
                    }
                    if (info.FieldType.IsSubclassOf(typeof(Object)) || (info.FieldType == typeof(Object)))
                    {
                        Object defaultReference = importer.GetDefaultReference(info.Name);
                        Object item = EditorGUILayout.ObjectField(ObjectNames.NicifyVariableName(info.Name), defaultReference, info.FieldType, false, new GUILayoutOption[0]);
                        names.Add(info.Name);
                        objects.Add(item);
                        if (defaultReference != item)
                        {
                            didModify = true;
                        }
                    }
                }
            }
        }
    }
}

