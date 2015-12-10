namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;

    [CustomPropertyDrawer(typeof(UnityEventBase), true)]
    public class UnityEventDrawer : PropertyDrawer
    {
        [CompilerGenerated]
        private static Func<IGrouping<string, string>, bool> <>f__am$cache10;
        [CompilerGenerated]
        private static Func<IGrouping<string, string>, string> <>f__am$cache11;
        [CompilerGenerated]
        private static Func<Type, string> <>f__am$cache12;
        [CompilerGenerated]
        private static Func<ValidMethodMap, int> <>f__am$cache13;
        [CompilerGenerated]
        private static Func<ValidMethodMap, string> <>f__am$cache14;
        [CompilerGenerated]
        private static Func<ParameterInfo, Type> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<MethodInfo, bool> <>f__am$cache9;
        [CompilerGenerated]
        private static Func<PropertyInfo, bool> <>f__am$cacheA;
        [CompilerGenerated]
        private static Func<PropertyInfo, MethodInfo> <>f__am$cacheB;
        [CompilerGenerated]
        private static Func<ParameterInfo, Type> <>f__am$cacheC;
        [CompilerGenerated]
        private static Func<Component, bool> <>f__am$cacheD;
        [CompilerGenerated]
        private static Func<Component, string> <>f__am$cacheE;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cacheF;
        private const string kArgumentsPath = "m_Arguments";
        private const string kBoolArgument = "m_BoolArgument";
        private const string kCallStatePath = "m_CallState";
        private const int kExtraSpacing = 9;
        private const string kFloatArgument = "m_FloatArgument";
        private const string kInstancePath = "m_Target";
        private const string kIntArgument = "m_IntArgument";
        private const string kMethodNamePath = "m_MethodName";
        private const string kModePath = "m_Mode";
        private const string kNoFunctionString = "No Function";
        private const string kObjectArgument = "m_ObjectArgument";
        private const string kObjectArgumentAssemblyTypeName = "m_ObjectArgumentAssemblyTypeName";
        private const string kStringArgument = "m_StringArgument";
        private UnityEventBase m_DummyEvent;
        private int m_LastSelectedIndex;
        private SerializedProperty m_ListenersArray;
        private SerializedProperty m_Prop;
        private ReorderableList m_ReorderableList;
        private Dictionary<string, State> m_States = new Dictionary<string, State>();
        private Styles m_Styles;
        private string m_Text;

        private void AddEventListener(ReorderableList list)
        {
            if (this.m_ListenersArray.hasMultipleDifferentValues)
            {
                foreach (Object obj2 in this.m_ListenersArray.serializedObject.targetObjects)
                {
                    SerializedObject obj3 = new SerializedObject(obj2);
                    SerializedProperty property1 = obj3.FindProperty(this.m_ListenersArray.propertyPath);
                    property1.arraySize++;
                    obj3.ApplyModifiedProperties();
                }
                this.m_ListenersArray.serializedObject.SetIsDifferentCacheDirty();
                this.m_ListenersArray.serializedObject.Update();
                list.index = list.serializedProperty.arraySize - 1;
            }
            else
            {
                ReorderableList.defaultBehaviours.DoAddButton(list);
            }
            this.m_LastSelectedIndex = list.index;
            SerializedProperty arrayElementAtIndex = this.m_ListenersArray.GetArrayElementAtIndex(list.index);
            SerializedProperty property3 = arrayElementAtIndex.FindPropertyRelative("m_CallState");
            SerializedProperty property4 = arrayElementAtIndex.FindPropertyRelative("m_Target");
            SerializedProperty property5 = arrayElementAtIndex.FindPropertyRelative("m_MethodName");
            SerializedProperty property6 = arrayElementAtIndex.FindPropertyRelative("m_Mode");
            SerializedProperty property7 = arrayElementAtIndex.FindPropertyRelative("m_Arguments");
            property3.enumValueIndex = 2;
            property4.objectReferenceValue = null;
            property5.stringValue = null;
            property6.enumValueIndex = 1;
            property7.FindPropertyRelative("m_FloatArgument").floatValue = 0f;
            property7.FindPropertyRelative("m_IntArgument").intValue = 0;
            property7.FindPropertyRelative("m_ObjectArgument").objectReferenceValue = null;
            property7.FindPropertyRelative("m_StringArgument").stringValue = null;
            property7.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue = null;
        }

        private static void AddFunctionsForScript(GenericMenu menu, SerializedProperty listener, ValidMethodMap method, string targetName)
        {
            PersistentListenerMode mode = method.mode;
            Object objectReferenceValue = listener.FindPropertyRelative("m_Target").objectReferenceValue;
            string stringValue = listener.FindPropertyRelative("m_MethodName").stringValue;
            PersistentListenerMode mode2 = GetMode(listener.FindPropertyRelative("m_Mode"));
            SerializedProperty property = listener.FindPropertyRelative("m_Arguments").FindPropertyRelative("m_ObjectArgumentAssemblyTypeName");
            StringBuilder builder = new StringBuilder();
            int length = method.methodInfo.GetParameters().Length;
            for (int i = 0; i < length; i++)
            {
                ParameterInfo info = method.methodInfo.GetParameters()[i];
                builder.Append(string.Format("{0}", GetTypeName(info.ParameterType)));
                if (i < (length - 1))
                {
                    builder.Append(", ");
                }
            }
            bool on = ((objectReferenceValue == method.target) && (stringValue == method.methodInfo.Name)) && (mode == mode2);
            if ((on && (mode == PersistentListenerMode.Object)) && (method.methodInfo.GetParameters().Length == 1))
            {
                on &= method.methodInfo.GetParameters()[0].ParameterType.AssemblyQualifiedName == property.stringValue;
            }
            string text = GetFormattedMethodName(targetName, method.methodInfo.Name, builder.ToString(), mode == PersistentListenerMode.EventDefined);
            menu.AddItem(new GUIContent(text), on, new GenericMenu.MenuFunction2(UnityEventDrawer.SetEventFunction), new UnityEventFunction(listener, method.target, method.methodInfo, mode));
        }

        private static void AddMethodsToMenu(GenericMenu menu, SerializedProperty listener, List<ValidMethodMap> methods, string targetName)
        {
            if (<>f__am$cache13 == null)
            {
                <>f__am$cache13 = e => !e.methodInfo.Name.StartsWith("set_") ? 1 : 0;
            }
            if (<>f__am$cache14 == null)
            {
                <>f__am$cache14 = e => e.methodInfo.Name;
            }
            IEnumerator<ValidMethodMap> enumerator = methods.OrderBy<ValidMethodMap, int>(<>f__am$cache13).ThenBy<ValidMethodMap, string>(<>f__am$cache14).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ValidMethodMap current = enumerator.Current;
                    AddFunctionsForScript(menu, listener, current, targetName);
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

        private static GenericMenu BuildPopupList(Object target, UnityEventBase dummyEvent, SerializedProperty listener)
        {
            Object gameObject = target;
            if (gameObject is Component)
            {
                gameObject = (target as Component).gameObject;
            }
            SerializedProperty property = listener.FindPropertyRelative("m_MethodName");
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("No Function"), string.IsNullOrEmpty(property.stringValue), new GenericMenu.MenuFunction2(UnityEventDrawer.ClearEventFunction), new UnityEventFunction(listener, null, null, PersistentListenerMode.EventDefined));
            if (gameObject != null)
            {
                menu.AddSeparator(string.Empty);
                if (<>f__am$cacheC == null)
                {
                    <>f__am$cacheC = x => x.ParameterType;
                }
                Type[] delegateArgumentsTypes = dummyEvent.GetType().GetMethod("Invoke").GetParameters().Select<ParameterInfo, Type>(<>f__am$cacheC).ToArray<Type>();
                GeneratePopUpForType(menu, gameObject, false, listener, delegateArgumentsTypes);
                if (!(gameObject is GameObject))
                {
                    return menu;
                }
                Component[] components = (gameObject as GameObject).GetComponents<Component>();
                if (<>f__am$cacheD == null)
                {
                    <>f__am$cacheD = c => c != null;
                }
                if (<>f__am$cacheE == null)
                {
                    <>f__am$cacheE = c => c.GetType().Name;
                }
                if (<>f__am$cacheF == null)
                {
                    <>f__am$cacheF = x => x;
                }
                if (<>f__am$cache10 == null)
                {
                    <>f__am$cache10 = g => g.Count<string>() > 1;
                }
                if (<>f__am$cache11 == null)
                {
                    <>f__am$cache11 = g => g.Key;
                }
                List<string> list = components.Where<Component>(<>f__am$cacheD).Select<Component, string>(<>f__am$cacheE).GroupBy<string, string>(<>f__am$cacheF).Where<IGrouping<string, string>>(<>f__am$cache10).Select<IGrouping<string, string>, string>(<>f__am$cache11).ToList<string>();
                foreach (Component component in components)
                {
                    if (component != null)
                    {
                        GeneratePopUpForType(menu, component, list.Contains(component.GetType().Name), listener, delegateArgumentsTypes);
                    }
                }
            }
            return menu;
        }

        private static IEnumerable<ValidMethodMap> CalculateMethodMap(Object target, Type[] t, bool allowSubclasses)
        {
            List<ValidMethodMap> list = new List<ValidMethodMap>();
            if ((target != null) && (t != null))
            {
                Type type = target.GetType();
                if (<>f__am$cache9 == null)
                {
                    <>f__am$cache9 = x => !x.IsSpecialName;
                }
                List<MethodInfo> list2 = type.GetMethods().Where<MethodInfo>(<>f__am$cache9).ToList<MethodInfo>();
                if (<>f__am$cacheA == null)
                {
                    <>f__am$cacheA = x => (x.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length == 0) && (x.GetSetMethod() != null);
                }
                IEnumerable<PropertyInfo> source = type.GetProperties().AsEnumerable<PropertyInfo>().Where<PropertyInfo>(<>f__am$cacheA);
                if (<>f__am$cacheB == null)
                {
                    <>f__am$cacheB = x => x.GetSetMethod();
                }
                list2.AddRange(source.Select<PropertyInfo, MethodInfo>(<>f__am$cacheB));
                foreach (MethodInfo info in list2)
                {
                    ParameterInfo[] parameters = info.GetParameters();
                    if (((parameters.Length == t.Length) && (info.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length <= 0)) && (info.ReturnType == typeof(void)))
                    {
                        bool flag = true;
                        for (int i = 0; i < t.Length; i++)
                        {
                            if (!parameters[i].ParameterType.IsAssignableFrom(t[i]))
                            {
                                flag = false;
                            }
                            if (allowSubclasses && t[i].IsAssignableFrom(parameters[i].ParameterType))
                            {
                                flag = true;
                            }
                        }
                        if (flag)
                        {
                            ValidMethodMap item = new ValidMethodMap {
                                target = target,
                                methodInfo = info
                            };
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        private static void ClearEventFunction(object source)
        {
            ((UnityEventFunction) source).Clear();
        }

        protected virtual void DrawEventHeader(Rect headerRect)
        {
            headerRect.height = 16f;
            string text = (!string.IsNullOrEmpty(this.m_Text) ? this.m_Text : "Event") + GetEventParams(this.m_DummyEvent);
            GUI.Label(headerRect, text);
        }

        private void DrawEventListener(Rect rect, int index, bool isactive, bool isfocused)
        {
            SerializedProperty property7;
            GUIContent mixedValueContent;
            SerializedProperty arrayElementAtIndex = this.m_ListenersArray.GetArrayElementAtIndex(index);
            rect.y++;
            Rect[] rowRects = this.GetRowRects(rect);
            Rect position = rowRects[0];
            Rect rect3 = rowRects[1];
            Rect totalPosition = rowRects[2];
            Rect rect5 = rowRects[3];
            SerializedProperty property = arrayElementAtIndex.FindPropertyRelative("m_CallState");
            SerializedProperty property3 = arrayElementAtIndex.FindPropertyRelative("m_Mode");
            SerializedProperty property4 = arrayElementAtIndex.FindPropertyRelative("m_Arguments");
            SerializedProperty property5 = arrayElementAtIndex.FindPropertyRelative("m_Target");
            SerializedProperty property6 = arrayElementAtIndex.FindPropertyRelative("m_MethodName");
            Color backgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.white;
            EditorGUI.PropertyField(position, property, GUIContent.none);
            EditorGUI.BeginChangeCheck();
            GUI.Box(rect3, GUIContent.none);
            EditorGUI.PropertyField(rect3, property5, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                property6.stringValue = null;
            }
            PersistentListenerMode @void = GetMode(property3);
            if ((property5.objectReferenceValue == null) || string.IsNullOrEmpty(property6.stringValue))
            {
                @void = PersistentListenerMode.Void;
            }
            switch (@void)
            {
                case PersistentListenerMode.Object:
                    property7 = property4.FindPropertyRelative("m_ObjectArgument");
                    break;

                case PersistentListenerMode.Int:
                    property7 = property4.FindPropertyRelative("m_IntArgument");
                    break;

                case PersistentListenerMode.Float:
                    property7 = property4.FindPropertyRelative("m_FloatArgument");
                    break;

                case PersistentListenerMode.String:
                    property7 = property4.FindPropertyRelative("m_StringArgument");
                    break;

                case PersistentListenerMode.Bool:
                    property7 = property4.FindPropertyRelative("m_BoolArgument");
                    break;

                default:
                    property7 = property4.FindPropertyRelative("m_IntArgument");
                    break;
            }
            string stringValue = property4.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue;
            Type objType = typeof(Object);
            if (!string.IsNullOrEmpty(stringValue))
            {
                Type type = Type.GetType(stringValue, false);
                if (type != null)
                {
                    objType = type;
                }
                else
                {
                    objType = typeof(Object);
                }
            }
            if (@void == PersistentListenerMode.Object)
            {
                EditorGUI.BeginChangeCheck();
                Object obj2 = EditorGUI.ObjectField(rect5, GUIContent.none, property7.objectReferenceValue, objType, true);
                if (EditorGUI.EndChangeCheck())
                {
                    property7.objectReferenceValue = obj2;
                }
            }
            else if ((@void != PersistentListenerMode.Void) && (@void != PersistentListenerMode.EventDefined))
            {
                EditorGUI.PropertyField(rect5, property7, GUIContent.none);
            }
            EditorGUI.BeginDisabledGroup(property5.objectReferenceValue == null);
            EditorGUI.BeginProperty(totalPosition, GUIContent.none, property6);
            if (EditorGUI.showMixedValue)
            {
                mixedValueContent = EditorGUI.mixedValueContent;
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                if ((property5.objectReferenceValue == null) || string.IsNullOrEmpty(property6.stringValue))
                {
                    builder.Append("No Function");
                }
                else if (!IsPersistantListenerValid(this.m_DummyEvent, property6.stringValue, property5.objectReferenceValue, GetMode(property3), objType))
                {
                    string name = "UnknownComponent";
                    Object objectReferenceValue = property5.objectReferenceValue;
                    if (objectReferenceValue != null)
                    {
                        name = objectReferenceValue.GetType().Name;
                    }
                    builder.Append(string.Format("<Missing {0}.{1}>", name, property6.stringValue));
                }
                else
                {
                    builder.Append(property5.objectReferenceValue.GetType().Name);
                    if (!string.IsNullOrEmpty(property6.stringValue))
                    {
                        builder.Append(".");
                        if (property6.stringValue.StartsWith("set_"))
                        {
                            builder.Append(property6.stringValue.Substring(4));
                        }
                        else
                        {
                            builder.Append(property6.stringValue);
                        }
                    }
                }
                mixedValueContent = GUIContent.Temp(builder.ToString());
            }
            if (GUI.Button(totalPosition, mixedValueContent, EditorStyles.popup))
            {
                BuildPopupList(property5.objectReferenceValue, this.m_DummyEvent, arrayElementAtIndex).DropDown(totalPosition);
            }
            EditorGUI.EndProperty();
            EditorGUI.EndDisabledGroup();
            GUI.backgroundColor = backgroundColor;
        }

        private void EndDragChild(ReorderableList list)
        {
            this.m_LastSelectedIndex = list.index;
        }

        private static void GeneratePopUpForType(GenericMenu menu, Object target, bool useFullTargetName, SerializedProperty listener, Type[] delegateArgumentsTypes)
        {
            List<ValidMethodMap> methods = new List<ValidMethodMap>();
            string targetName = !useFullTargetName ? target.GetType().Name : target.GetType().FullName;
            bool flag = false;
            if (delegateArgumentsTypes.Length != 0)
            {
                GetMethodsForTargetAndMode(target, delegateArgumentsTypes, methods, PersistentListenerMode.EventDefined);
                if (methods.Count > 0)
                {
                    if (<>f__am$cache12 == null)
                    {
                        <>f__am$cache12 = e => GetTypeName(e);
                    }
                    menu.AddDisabledItem(new GUIContent(targetName + "/Dynamic " + string.Join(", ", delegateArgumentsTypes.Select<Type, string>(<>f__am$cache12).ToArray<string>())));
                    AddMethodsToMenu(menu, listener, methods, targetName);
                    flag = true;
                }
            }
            methods.Clear();
            Type[] typeArray1 = new Type[] { typeof(float) };
            GetMethodsForTargetAndMode(target, typeArray1, methods, PersistentListenerMode.Float);
            Type[] typeArray2 = new Type[] { typeof(int) };
            GetMethodsForTargetAndMode(target, typeArray2, methods, PersistentListenerMode.Int);
            Type[] typeArray3 = new Type[] { typeof(string) };
            GetMethodsForTargetAndMode(target, typeArray3, methods, PersistentListenerMode.String);
            Type[] typeArray4 = new Type[] { typeof(bool) };
            GetMethodsForTargetAndMode(target, typeArray4, methods, PersistentListenerMode.Bool);
            Type[] typeArray5 = new Type[] { typeof(Object) };
            GetMethodsForTargetAndMode(target, typeArray5, methods, PersistentListenerMode.Object);
            GetMethodsForTargetAndMode(target, new Type[0], methods, PersistentListenerMode.Void);
            if (methods.Count > 0)
            {
                if (flag)
                {
                    menu.AddItem(new GUIContent(targetName + "/ "), false, null);
                }
                if (delegateArgumentsTypes.Length != 0)
                {
                    menu.AddDisabledItem(new GUIContent(targetName + "/Static Parameters"));
                }
                AddMethodsToMenu(menu, listener, methods, targetName);
            }
        }

        private static UnityEventBase GetDummyEvent(SerializedProperty prop)
        {
            Type type = Type.GetType(prop.FindPropertyRelative("m_TypeName").stringValue, false);
            if (type == null)
            {
                return new UnityEvent();
            }
            return (Activator.CreateInstance(type) as UnityEventBase);
        }

        private static string GetEventParams(UnityEventBase evt)
        {
            MethodInfo info = evt.FindMethod("Invoke", evt, PersistentListenerMode.EventDefined, null);
            StringBuilder builder = new StringBuilder();
            builder.Append(" (");
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = x => x.ParameterType;
            }
            Type[] typeArray = info.GetParameters().Select<ParameterInfo, Type>(<>f__am$cache8).ToArray<Type>();
            for (int i = 0; i < typeArray.Length; i++)
            {
                builder.Append(typeArray[i].Name);
                if (i < (typeArray.Length - 1))
                {
                    builder.Append(", ");
                }
            }
            builder.Append(")");
            return builder.ToString();
        }

        private static string GetFormattedMethodName(string targetName, string methodName, string args, bool dynamic)
        {
            if (dynamic)
            {
                if (methodName.StartsWith("set_"))
                {
                    return string.Format("{0}/{1}", targetName, methodName.Substring(4));
                }
                return string.Format("{0}/{1}", targetName, methodName);
            }
            if (methodName.StartsWith("set_"))
            {
                return string.Format("{0}/{2} {1}", targetName, methodName.Substring(4), args);
            }
            return string.Format("{0}/{1} ({2})", targetName, methodName, args);
        }

        private static void GetMethodsForTargetAndMode(Object target, Type[] delegateArgumentsTypes, List<ValidMethodMap> methods, PersistentListenerMode mode)
        {
            IEnumerator<ValidMethodMap> enumerator = CalculateMethodMap(target, delegateArgumentsTypes, mode == PersistentListenerMode.Object).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ValidMethodMap item = enumerator.Current;
                    item.mode = mode;
                    methods.Add(item);
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

        private static PersistentListenerMode GetMode(SerializedProperty mode)
        {
            return (PersistentListenerMode) mode.enumValueIndex;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            this.RestoreState(property);
            float height = 0f;
            if (this.m_ReorderableList != null)
            {
                height = this.m_ReorderableList.GetHeight();
            }
            return height;
        }

        private Rect[] GetRowRects(Rect rect)
        {
            Rect[] rectArray = new Rect[4];
            rect.height = 16f;
            rect.y += 2f;
            Rect rect2 = rect;
            rect2.width *= 0.3f;
            Rect rect3 = rect2;
            rect3.y += EditorGUIUtility.singleLineHeight + 2f;
            Rect rect4 = rect;
            rect4.xMin = rect3.xMax + 5f;
            Rect rect5 = rect4;
            rect5.y += EditorGUIUtility.singleLineHeight + 2f;
            rectArray[0] = rect2;
            rectArray[1] = rect3;
            rectArray[2] = rect4;
            rectArray[3] = rect5;
            return rectArray;
        }

        private State GetState(SerializedProperty prop)
        {
            State state;
            string propertyPath = prop.propertyPath;
            this.m_States.TryGetValue(propertyPath, out state);
            if (state == null)
            {
                state = new State();
                SerializedProperty elements = prop.FindPropertyRelative("m_PersistentCalls.m_Calls");
                state.m_ReorderableList = new ReorderableList(prop.serializedObject, elements, false, true, true, true);
                state.m_ReorderableList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.DrawEventHeader);
                state.m_ReorderableList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.DrawEventListener);
                state.m_ReorderableList.onSelectCallback = new ReorderableList.SelectCallbackDelegate(this.SelectEventListener);
                state.m_ReorderableList.onReorderCallback = new ReorderableList.ReorderCallbackDelegate(this.EndDragChild);
                state.m_ReorderableList.onAddCallback = new ReorderableList.AddCallbackDelegate(this.AddEventListener);
                state.m_ReorderableList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(this.RemoveButton);
                state.m_ReorderableList.elementHeight = 43f;
                this.m_States[propertyPath] = state;
            }
            return state;
        }

        private static string GetTypeName(Type t)
        {
            if (t == typeof(int))
            {
                return "int";
            }
            if (t == typeof(float))
            {
                return "float";
            }
            if (t == typeof(string))
            {
                return "string";
            }
            if (t == typeof(bool))
            {
                return "bool";
            }
            return t.Name;
        }

        public static bool IsPersistantListenerValid(UnityEventBase dummyEvent, string methodName, Object uObject, PersistentListenerMode modeEnum, Type argumentType)
        {
            return (((uObject != null) && !string.IsNullOrEmpty(methodName)) && (dummyEvent.FindMethod(methodName, uObject, modeEnum, argumentType) != null));
        }

        public void OnGUI(Rect position)
        {
            if ((this.m_ListenersArray != null) && this.m_ListenersArray.isArray)
            {
                this.m_DummyEvent = GetDummyEvent(this.m_Prop);
                if (this.m_DummyEvent != null)
                {
                    if (this.m_Styles == null)
                    {
                        this.m_Styles = new Styles();
                    }
                    if (this.m_ReorderableList != null)
                    {
                        int indentLevel = EditorGUI.indentLevel;
                        EditorGUI.indentLevel = 0;
                        this.m_ReorderableList.DoList(position);
                        EditorGUI.indentLevel = indentLevel;
                    }
                }
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            this.m_Prop = property;
            this.m_Text = label.text;
            State state = this.RestoreState(property);
            this.OnGUI(position);
            state.lastSelectedIndex = this.m_LastSelectedIndex;
        }

        private void RemoveButton(ReorderableList list)
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
            this.m_LastSelectedIndex = list.index;
        }

        private State RestoreState(SerializedProperty property)
        {
            State state = this.GetState(property);
            this.m_ListenersArray = state.m_ReorderableList.serializedProperty;
            this.m_ReorderableList = state.m_ReorderableList;
            this.m_LastSelectedIndex = state.lastSelectedIndex;
            return state;
        }

        private void SelectEventListener(ReorderableList list)
        {
            this.m_LastSelectedIndex = list.index;
        }

        private static void SetEventFunction(object source)
        {
            ((UnityEventFunction) source).Assign();
        }

        protected class State
        {
            public int lastSelectedIndex;
            internal ReorderableList m_ReorderableList;
        }

        private class Styles
        {
            public readonly GUIStyle genericFieldStyle = EditorStyles.label;
            public readonly GUIContent iconToolbarMinus = EditorGUIUtility.IconContent("Toolbar Minus");
            public readonly GUIStyle removeButton = "InvisibleButton";
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct UnityEventFunction
        {
            private readonly SerializedProperty m_Listener;
            private readonly Object m_Target;
            private readonly MethodInfo m_Method;
            private readonly PersistentListenerMode m_Mode;
            public UnityEventFunction(SerializedProperty listener, Object target, MethodInfo method, PersistentListenerMode mode)
            {
                this.m_Listener = listener;
                this.m_Target = target;
                this.m_Method = method;
                this.m_Mode = mode;
            }

            public void Assign()
            {
                SerializedProperty property = this.m_Listener.FindPropertyRelative("m_Target");
                SerializedProperty property2 = this.m_Listener.FindPropertyRelative("m_MethodName");
                SerializedProperty property3 = this.m_Listener.FindPropertyRelative("m_Mode");
                SerializedProperty arguments = this.m_Listener.FindPropertyRelative("m_Arguments");
                property.objectReferenceValue = this.m_Target;
                property2.stringValue = this.m_Method.Name;
                property3.enumValueIndex = (int) this.m_Mode;
                if (this.m_Mode == PersistentListenerMode.Object)
                {
                    SerializedProperty property5 = arguments.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName");
                    ParameterInfo[] parameters = this.m_Method.GetParameters();
                    if ((parameters.Length == 1) && typeof(Object).IsAssignableFrom(parameters[0].ParameterType))
                    {
                        property5.stringValue = parameters[0].ParameterType.AssemblyQualifiedName;
                    }
                    else
                    {
                        property5.stringValue = typeof(Object).AssemblyQualifiedName;
                    }
                }
                this.ValidateObjectParamater(arguments, this.m_Mode);
                this.m_Listener.m_SerializedObject.ApplyModifiedProperties();
            }

            private void ValidateObjectParamater(SerializedProperty arguments, PersistentListenerMode mode)
            {
                SerializedProperty property = arguments.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName");
                SerializedProperty property2 = arguments.FindPropertyRelative("m_ObjectArgument");
                Object objectReferenceValue = property2.objectReferenceValue;
                if (mode != PersistentListenerMode.Object)
                {
                    property.stringValue = typeof(Object).AssemblyQualifiedName;
                    property2.objectReferenceValue = null;
                }
                else if (objectReferenceValue != null)
                {
                    Type c = Type.GetType(property.stringValue, false);
                    if (!typeof(Object).IsAssignableFrom(c) || !c.IsInstanceOfType(objectReferenceValue))
                    {
                        property2.objectReferenceValue = null;
                    }
                }
            }

            public void Clear()
            {
                this.m_Listener.FindPropertyRelative("m_MethodName").stringValue = null;
                this.m_Listener.FindPropertyRelative("m_Mode").enumValueIndex = 1;
                this.m_Listener.m_SerializedObject.ApplyModifiedProperties();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ValidMethodMap
        {
            public Object target;
            public MethodInfo methodInfo;
            public PersistentListenerMode mode;
        }
    }
}

