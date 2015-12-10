namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class TargetChoiceHandler
    {
        internal static void AddSetToValueOfTargetMenuItems(GenericMenu menu, SerializedProperty property, TargetChoiceMenuFunction func)
        {
            SerializedProperty property2 = property.serializedObject.FindProperty(property.propertyPath);
            Object[] targetObjects = property.serializedObject.targetObjects;
            List<string> list = new List<string>();
            foreach (Object obj2 in targetObjects)
            {
                string item = "Set to Value of " + obj2.name;
                if (list.Contains(item))
                {
                    int num2 = 1;
                    while (true)
                    {
                        object[] objArray1 = new object[] { "Set to Value of ", obj2.name, " (", num2, ")" };
                        item = string.Concat(objArray1);
                        if (!list.Contains(item))
                        {
                            break;
                        }
                        num2++;
                    }
                }
                list.Add(item);
                menu.AddItem(EditorGUIUtility.TextContent(item), false, new GenericMenu.MenuFunction2(TargetChoiceHandler.TargetChoiceForwardFunction), new PropertyAndTargetHandler(property2, obj2, func));
            }
        }

        internal static void DeleteArrayElement(object userData)
        {
            SerializedProperty property = (SerializedProperty) userData;
            property.DeleteCommand();
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.ForceReloadInspectors();
        }

        internal static void DuplicateArrayElement(object userData)
        {
            SerializedProperty property = (SerializedProperty) userData;
            property.DuplicateCommand();
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.ForceReloadInspectors();
        }

        internal static void SetPrefabOverride(object userData)
        {
            SerializedProperty property = (SerializedProperty) userData;
            property.prefabOverride = false;
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.ForceReloadInspectors();
        }

        internal static void SetToValueOfTarget(SerializedProperty property, Object target)
        {
            property.SetToValueOfTarget(target);
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.ForceReloadInspectors();
        }

        private static void TargetChoiceForwardFunction(object userData)
        {
            PropertyAndTargetHandler handler = (PropertyAndTargetHandler) userData;
            handler.function(handler.property, handler.target);
        }

        internal delegate void TargetChoiceMenuFunction(SerializedProperty property, Object target);
    }
}

