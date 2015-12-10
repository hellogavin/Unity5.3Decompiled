namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class ScriptAttributeUtility
    {
        [CompilerGenerated]
        private static Func<Assembly, IEnumerable<Type>> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<object, PropertyAttribute> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<PropertyAttribute, int> <>f__am$cache9;
        private static Dictionary<string, List<PropertyAttribute>> s_BuiltinAttributes = null;
        private static PropertyHandlerCache s_CurrentCache = null;
        internal static Stack<PropertyDrawer> s_DrawerStack = new Stack<PropertyDrawer>();
        private static Dictionary<Type, DrawerKeySet> s_DrawerTypeForType = null;
        private static PropertyHandlerCache s_GlobalCache = new PropertyHandlerCache();
        private static PropertyHandler s_NextHandler = new PropertyHandler();
        private static PropertyHandler s_SharedNullHandler = new PropertyHandler();

        private static void AddBuiltinAttribute(string componentTypeName, string propertyPath, PropertyAttribute attr)
        {
            string key = componentTypeName + "_" + propertyPath;
            if (!s_BuiltinAttributes.ContainsKey(key))
            {
                s_BuiltinAttributes.Add(key, new List<PropertyAttribute>());
            }
            s_BuiltinAttributes[key].Add(attr);
        }

        private static void BuildDrawerTypeForTypeDictionary()
        {
            s_DrawerTypeForType = new Dictionary<Type, DrawerKeySet>();
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = (Func<Assembly, IEnumerable<Type>>) (x => AssemblyHelper.GetTypesFromAssembly(x));
            }
            Type[] source = AppDomain.CurrentDomain.GetAssemblies().SelectMany<Assembly, Type>(<>f__am$cache7).ToArray<Type>();
            IEnumerator<Type> enumerator = EditorAssemblies.SubclassesOf(typeof(GUIDrawer)).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Type current = enumerator.Current;
                    object[] customAttributes = current.GetCustomAttributes(typeof(CustomPropertyDrawer), true);
                    <BuildDrawerTypeForTypeDictionary>c__AnonStoreyA8 ya = new <BuildDrawerTypeForTypeDictionary>c__AnonStoreyA8();
                    object[] objArray2 = customAttributes;
                    for (int i = 0; i < objArray2.Length; i++)
                    {
                        ya.editor = (CustomPropertyDrawer) objArray2[i];
                        DrawerKeySet set = new DrawerKeySet {
                            drawer = current,
                            type = ya.editor.m_Type
                        };
                        s_DrawerTypeForType[ya.editor.m_Type] = set;
                        if (ya.editor.m_UseForChildren)
                        {
                            IEnumerator<Type> enumerator2 = source.Where<Type>(new Func<Type, bool>(ya.<>m__1F0)).GetEnumerator();
                            try
                            {
                                while (enumerator2.MoveNext())
                                {
                                    Type key = enumerator2.Current;
                                    if (s_DrawerTypeForType.ContainsKey(key))
                                    {
                                        DrawerKeySet set2 = s_DrawerTypeForType[key];
                                        if (ya.editor.m_Type.IsAssignableFrom(set2.type))
                                        {
                                            continue;
                                        }
                                    }
                                    DrawerKeySet set3 = new DrawerKeySet {
                                        drawer = current,
                                        type = ya.editor.m_Type
                                    };
                                    s_DrawerTypeForType[key] = set3;
                                }
                            }
                            finally
                            {
                                if (enumerator2 == null)
                                {
                                }
                                enumerator2.Dispose();
                            }
                        }
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
        }

        internal static void ClearGlobalCache()
        {
            s_GlobalCache.Clear();
        }

        private static List<PropertyAttribute> GetBuiltinAttributes(SerializedProperty property)
        {
            if (property.serializedObject.targetObject == null)
            {
                return null;
            }
            Type type = property.serializedObject.targetObject.GetType();
            if (type == null)
            {
                return null;
            }
            string key = type.Name + "_" + property.propertyPath;
            List<PropertyAttribute> list = null;
            s_BuiltinAttributes.TryGetValue(key, out list);
            return list;
        }

        internal static Type GetDrawerTypeForType(Type type)
        {
            DrawerKeySet set;
            if (s_DrawerTypeForType == null)
            {
                BuildDrawerTypeForTypeDictionary();
            }
            s_DrawerTypeForType.TryGetValue(type, out set);
            if ((set.drawer == null) && type.IsGenericType)
            {
                s_DrawerTypeForType.TryGetValue(type.GetGenericTypeDefinition(), out set);
            }
            return set.drawer;
        }

        private static List<PropertyAttribute> GetFieldAttributes(FieldInfo field)
        {
            if (field == null)
            {
                return null;
            }
            object[] customAttributes = field.GetCustomAttributes(typeof(PropertyAttribute), true);
            if ((customAttributes == null) || (customAttributes.Length <= 0))
            {
                return null;
            }
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = e => e as PropertyAttribute;
            }
            if (<>f__am$cache9 == null)
            {
                <>f__am$cache9 = e => -e.order;
            }
            return new List<PropertyAttribute>(customAttributes.Select<object, PropertyAttribute>(<>f__am$cache8).OrderBy<PropertyAttribute, int>(<>f__am$cache9));
        }

        private static FieldInfo GetFieldInfoFromProperty(SerializedProperty property, out Type type)
        {
            Type scriptTypeFromProperty = GetScriptTypeFromProperty(property);
            if (scriptTypeFromProperty == null)
            {
                type = null;
                return null;
            }
            return GetFieldInfoFromPropertyPath(scriptTypeFromProperty, property.propertyPath, out type);
        }

        private static FieldInfo GetFieldInfoFromPropertyPath(Type host, string path, out Type type)
        {
            FieldInfo info = null;
            type = host;
            char[] separator = new char[] { '.' };
            string[] strArray = path.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                string name = strArray[i];
                if (((i < (strArray.Length - 1)) && (name == "Array")) && strArray[i + 1].StartsWith("data["))
                {
                    if (type.IsArrayOrList())
                    {
                        type = type.GetArrayOrListElementType();
                    }
                    i++;
                }
                else
                {
                    FieldInfo field = null;
                    for (Type type2 = type; (field == null) && (type2 != null); type2 = type2.BaseType)
                    {
                        field = type2.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    }
                    if (field == null)
                    {
                        type = null;
                        return null;
                    }
                    info = field;
                    type = info.FieldType;
                }
            }
            return info;
        }

        internal static PropertyHandler GetHandler(SerializedProperty property)
        {
            if (property == null)
            {
                return s_SharedNullHandler;
            }
            if (property.serializedObject.inspectorMode != InspectorMode.Normal)
            {
                return s_SharedNullHandler;
            }
            PropertyHandler handler = propertyHandlerCache.GetHandler(property);
            if (handler == null)
            {
                Type type = null;
                List<PropertyAttribute> fieldAttributes = null;
                FieldInfo field = null;
                Object targetObject = property.serializedObject.targetObject;
                if ((targetObject is MonoBehaviour) || (targetObject is ScriptableObject))
                {
                    field = GetFieldInfoFromProperty(property, out type);
                    fieldAttributes = GetFieldAttributes(field);
                }
                else
                {
                    if (s_BuiltinAttributes == null)
                    {
                        PopulateBuiltinAttributes();
                    }
                    if (fieldAttributes == null)
                    {
                        fieldAttributes = GetBuiltinAttributes(property);
                    }
                }
                handler = s_NextHandler;
                if (fieldAttributes != null)
                {
                    for (int i = fieldAttributes.Count - 1; i >= 0; i--)
                    {
                        handler.HandleAttribute(fieldAttributes[i], field, type);
                    }
                }
                if (!handler.hasPropertyDrawer && (type != null))
                {
                    handler.HandleDrawnType(type, type, field, null);
                }
                if (handler.empty)
                {
                    propertyHandlerCache.SetHandler(property, s_SharedNullHandler);
                    return s_SharedNullHandler;
                }
                propertyHandlerCache.SetHandler(property, handler);
                s_NextHandler = new PropertyHandler();
            }
            return handler;
        }

        private static Type GetScriptTypeFromProperty(SerializedProperty property)
        {
            SerializedProperty property2 = property.serializedObject.FindProperty("m_Script");
            if (property2 == null)
            {
                return null;
            }
            MonoScript objectReferenceValue = property2.objectReferenceValue as MonoScript;
            if (objectReferenceValue == null)
            {
                return null;
            }
            return objectReferenceValue.GetClass();
        }

        private static void PopulateBuiltinAttributes()
        {
            s_BuiltinAttributes = new Dictionary<string, List<PropertyAttribute>>();
            AddBuiltinAttribute("GUIText", "m_Text", new MultilineAttribute());
            AddBuiltinAttribute("TextMesh", "m_Text", new MultilineAttribute());
        }

        internal static PropertyHandlerCache propertyHandlerCache
        {
            get
            {
                if (s_CurrentCache == null)
                {
                }
                return s_GlobalCache;
            }
            set
            {
                s_CurrentCache = value;
            }
        }

        [CompilerGenerated]
        private sealed class <BuildDrawerTypeForTypeDictionary>c__AnonStoreyA8
        {
            internal CustomPropertyDrawer editor;

            internal bool <>m__1F0(Type x)
            {
                return x.IsSubclassOf(this.editor.m_Type);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DrawerKeySet
        {
            public Type drawer;
            public Type type;
        }
    }
}

