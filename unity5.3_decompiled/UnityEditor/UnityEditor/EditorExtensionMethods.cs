namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal static class EditorExtensionMethods
    {
        internal static List<Enum> EnumGetNonObsoleteValues(this Type type)
        {
            string[] names = Enum.GetNames(type);
            Enum[] enumArray = Enum.GetValues(type).Cast<Enum>().ToArray<Enum>();
            List<Enum> list = new List<Enum>();
            for (int i = 0; i < names.Length; i++)
            {
                object[] customAttributes = type.GetMember(names[i])[0].GetCustomAttributes(typeof(ObsoleteAttribute), false);
                bool flag = false;
                foreach (object obj2 in customAttributes)
                {
                    if (obj2 is ObsoleteAttribute)
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    list.Add(enumArray[i]);
                }
            }
            return list;
        }

        internal static Type GetArrayOrListElementType(this Type listType)
        {
            if (listType.IsArray)
            {
                return listType.GetElementType();
            }
            if (listType.IsGenericType && (listType.GetGenericTypeDefinition() == typeof(List<>)))
            {
                return listType.GetGenericArguments()[0];
            }
            return null;
        }

        internal static bool IsArrayOrList(this Type listType)
        {
            return (listType.IsArray || (listType.IsGenericType && (listType.GetGenericTypeDefinition() == typeof(List<>))));
        }

        internal static bool MainActionKeyForControl(this Event evt, int controlId)
        {
            if (GUIUtility.keyboardControl != controlId)
            {
                return false;
            }
            bool flag = ((evt.alt || evt.shift) || evt.command) || evt.control;
            if (((evt.type == EventType.KeyDown) && (evt.character == ' ')) && !flag)
            {
                evt.Use();
                return false;
            }
            return (((evt.type == EventType.KeyDown) && (((evt.keyCode == KeyCode.Space) || (evt.keyCode == KeyCode.Return)) || (evt.keyCode == KeyCode.KeypadEnter))) && !flag);
        }
    }
}

