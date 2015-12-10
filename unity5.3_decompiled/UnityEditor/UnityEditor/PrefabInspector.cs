namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class PrefabInspector
    {
        private static void AddComponentGUI(Object prefab)
        {
            bool flag;
            SerializedObject obj2 = new SerializedObject(prefab);
            SerializedProperty x = obj2.FindProperty("m_Modification");
            SerializedProperty endProperty = x.GetEndProperty();
            do
            {
                flag = EditorGUILayout.PropertyField(x, new GUILayoutOption[0]);
            }
            while (x.NextVisible(flag) && !SerializedProperty.EqualContents(x, endProperty));
            obj2.ApplyModifiedProperties();
        }

        public static void OnOverridenPrefabsInspector(GameObject gameObject)
        {
            GUI.enabled = true;
            Object prefabObject = PrefabUtility.GetPrefabObject(gameObject);
            if (prefabObject != null)
            {
                EditorGUIUtility.labelWidth = 200f;
                if (PrefabUtility.GetPrefabType(gameObject) == PrefabType.PrefabInstance)
                {
                    PropertyModification[] propertyModifications = PrefabUtility.GetPropertyModifications(gameObject);
                    if ((propertyModifications != null) && (propertyModifications.Length != 0))
                    {
                        GUI.changed = false;
                        for (int i = 0; i < propertyModifications.Length; i++)
                        {
                            propertyModifications[i].value = EditorGUILayout.TextField(propertyModifications[i].propertyPath, propertyModifications[i].value, new GUILayoutOption[0]);
                        }
                        if (GUI.changed)
                        {
                            PrefabUtility.SetPropertyModifications(gameObject, propertyModifications);
                        }
                    }
                }
                AddComponentGUI(prefabObject);
            }
        }
    }
}

