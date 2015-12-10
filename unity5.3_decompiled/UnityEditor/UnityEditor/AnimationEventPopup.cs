namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [EditorWindowTitle(title="Edit Animation Event", useTypeNameAsIconName=false)]
    internal class AnimationEventPopup : EditorWindow
    {
        [CompilerGenerated]
        private static Func<ParameterInfo, Type> <>f__am$cache5;
        private const string kAmbiguousPostFix = " (Function Is Ambiguous)";
        private const string kNoneSelected = "(No Function Selected)";
        private const string kNotSupportedPostFix = " (Function Not Supported)";
        private AnimationClip m_Clip;
        private AnimationClipInfoProperties m_ClipInfo;
        private int m_EventIndex;
        private EditorWindow m_Owner;
        private GameObject m_Root;

        internal static void ClosePopup()
        {
            Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(AnimationEventPopup));
            AnimationEventPopup popup = (objArray.Length <= 0) ? null : ((AnimationEventPopup) objArray[0]);
            if (popup != null)
            {
                popup.Close();
            }
        }

        private static void CollectSupportedMethods(GameObject root, out List<string> supportedMethods, out List<Type> supportedMethodsParameter)
        {
            supportedMethods = new List<string>();
            supportedMethodsParameter = new List<Type>();
            MonoBehaviour[] components = root.GetComponents<MonoBehaviour>();
            HashSet<string> set = new HashSet<string>();
            foreach (MonoBehaviour behaviour in components)
            {
                if (behaviour != null)
                {
                    for (Type type = behaviour.GetType(); (type != typeof(MonoBehaviour)) && (type != null); type = type.BaseType)
                    {
                        foreach (MethodInfo info in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                        {
                            string name = info.Name;
                            if (IsSupportedMethodName(name))
                            {
                                ParameterInfo[] parameters = info.GetParameters();
                                if (parameters.Length <= 1)
                                {
                                    if (parameters.Length == 1)
                                    {
                                        Type parameterType = parameters[0].ParameterType;
                                        if ((((parameterType != typeof(string)) && (parameterType != typeof(float))) && ((parameterType != typeof(int)) && (parameterType != typeof(AnimationEvent)))) && (((parameterType != typeof(Object)) && !parameterType.IsSubclassOf(typeof(Object))) && !parameterType.IsEnum))
                                        {
                                            continue;
                                        }
                                        supportedMethodsParameter.Add(parameterType);
                                    }
                                    else
                                    {
                                        supportedMethodsParameter.Add(null);
                                    }
                                    if (supportedMethods.Contains(name))
                                    {
                                        set.Add(name);
                                    }
                                    supportedMethods.Add(name);
                                }
                            }
                        }
                    }
                }
            }
            foreach (string str2 in set)
            {
                for (int i = 0; i < supportedMethods.Count; i++)
                {
                    if (supportedMethods[i].Equals(str2))
                    {
                        supportedMethods.RemoveAt(i);
                        supportedMethodsParameter.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        internal static int Create(GameObject root, AnimationClip clip, float time, EditorWindow owner)
        {
            AnimationEvent evt = new AnimationEvent {
                time = time
            };
            int num = InsertAnimationEvent(ref AnimationUtility.GetAnimationEvents(clip), clip, evt);
            AnimationEventPopup window = EditorWindow.GetWindow<AnimationEventPopup>(true);
            InitWindow(window);
            window.m_Root = root;
            window.m_Clip = clip;
            window.eventIndex = num;
            window.m_Owner = owner;
            return num;
        }

        private static void DoEditRegularParameters(AnimationEvent evt, Type selectedParameter)
        {
            if ((selectedParameter == typeof(AnimationEvent)) || (selectedParameter == typeof(float)))
            {
                evt.floatParameter = EditorGUILayout.FloatField("Float", evt.floatParameter, new GUILayoutOption[0]);
            }
            if (selectedParameter.IsEnum)
            {
                evt.intParameter = EnumPopup("Enum", selectedParameter, evt.intParameter);
            }
            else if ((selectedParameter == typeof(AnimationEvent)) || (selectedParameter == typeof(int)))
            {
                evt.intParameter = EditorGUILayout.IntField("Int", evt.intParameter, new GUILayoutOption[0]);
            }
            if ((selectedParameter == typeof(AnimationEvent)) || (selectedParameter == typeof(string)))
            {
                evt.stringParameter = EditorGUILayout.TextField("String", evt.stringParameter, new GUILayoutOption[0]);
            }
            if (((selectedParameter == typeof(AnimationEvent)) || selectedParameter.IsSubclassOf(typeof(Object))) || (selectedParameter == typeof(Object)))
            {
                Type objType = typeof(Object);
                if (selectedParameter != typeof(AnimationEvent))
                {
                    objType = selectedParameter;
                }
                bool allowSceneObjects = false;
                evt.objectReferenceParameter = EditorGUILayout.ObjectField(ObjectNames.NicifyVariableName(objType.Name), evt.objectReferenceParameter, objType, allowSceneObjects, new GUILayoutOption[0]);
            }
        }

        internal static void Edit(AnimationClipInfoProperties clipInfo, int index)
        {
            Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(AnimationEventPopup));
            AnimationEventPopup window = (objArray.Length <= 0) ? null : ((AnimationEventPopup) objArray[0]);
            if (window == null)
            {
                window = EditorWindow.GetWindow<AnimationEventPopup>(true);
                InitWindow(window);
            }
            window.m_Root = null;
            window.m_Clip = null;
            window.m_ClipInfo = clipInfo;
            window.eventIndex = index;
            window.Repaint();
        }

        internal static void Edit(GameObject root, AnimationClip clip, int index, EditorWindow owner)
        {
            Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(AnimationEventPopup));
            AnimationEventPopup window = (objArray.Length <= 0) ? null : ((AnimationEventPopup) objArray[0]);
            if (window == null)
            {
                window = EditorWindow.GetWindow<AnimationEventPopup>(true);
                InitWindow(window);
            }
            window.m_Root = root;
            window.m_Clip = clip;
            window.eventIndex = index;
            window.m_Owner = owner;
            window.Repaint();
        }

        private static bool EnterPressed()
        {
            return ((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Return));
        }

        public static int EnumPopup(string label, Type enumType, int selected)
        {
            if (!enumType.IsEnum)
            {
                throw new Exception("parameter _enum must be of type System.Enum");
            }
            string[] names = Enum.GetNames(enumType);
            int index = Array.IndexOf<string>(names, Enum.GetName(enumType, selected));
            index = EditorGUILayout.Popup(label, index, names, EditorStyles.popup, new GUILayoutOption[0]);
            if (index == -1)
            {
                return selected;
            }
            Enum enum2 = (Enum) Enum.Parse(enumType, names[index]);
            return Convert.ToInt32(enum2);
        }

        private static bool EscapePressed()
        {
            return ((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Escape));
        }

        public static string FormatEvent(GameObject root, AnimationEvent evt)
        {
            if (string.IsNullOrEmpty(evt.functionName))
            {
                return "(No Function Selected)";
            }
            if (IsSupportedMethodName(evt.functionName))
            {
                foreach (MonoBehaviour behaviour in root.GetComponents<MonoBehaviour>())
                {
                    if (behaviour != null)
                    {
                        Type type = behaviour.GetType();
                        if ((type != typeof(MonoBehaviour)) && ((type.BaseType == null) || (type.BaseType.Name != "GraphBehaviour")))
                        {
                            MethodInfo method = type.GetMethod(evt.functionName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                            if (method != null)
                            {
                                if (<>f__am$cache5 == null)
                                {
                                    <>f__am$cache5 = p => p.ParameterType;
                                }
                                IEnumerable<Type> paramTypes = method.GetParameters().Select<ParameterInfo, Type>(<>f__am$cache5);
                                return (evt.functionName + FormatEventArguments(paramTypes, evt));
                            }
                        }
                    }
                }
            }
            return (evt.functionName + " (Function Not Supported)");
        }

        private static string FormatEventArguments(IEnumerable<Type> paramTypes, AnimationEvent evt)
        {
            if (!paramTypes.Any<Type>())
            {
                return " ( )";
            }
            if (paramTypes.Count<Type>() <= 1)
            {
                Type enumType = paramTypes.First<Type>();
                if (enumType == typeof(string))
                {
                    return (" ( \"" + evt.stringParameter + "\" )");
                }
                if (enumType == typeof(float))
                {
                    return (" ( " + evt.floatParameter + " )");
                }
                if (enumType == typeof(int))
                {
                    return (" ( " + evt.intParameter + " )");
                }
                if (enumType == typeof(int))
                {
                    return (" ( " + evt.intParameter + " )");
                }
                if (enumType.IsEnum)
                {
                    string[] textArray1 = new string[] { " ( ", enumType.Name, ".", Enum.GetName(enumType, evt.intParameter), " )" };
                    return string.Concat(textArray1);
                }
                if (enumType == typeof(AnimationEvent))
                {
                    object[] objArray1 = new object[] { " ( ", evt.floatParameter, " / ", evt.intParameter, " / \"", evt.stringParameter, "\" / ", (evt.objectReferenceParameter != null) ? evt.objectReferenceParameter.name : "null", " )" };
                    return string.Concat(objArray1);
                }
                if (enumType.IsSubclassOf(typeof(Object)) || (enumType == typeof(Object)))
                {
                    return (" ( " + ((evt.objectReferenceParameter != null) ? evt.objectReferenceParameter.name : "null") + " )");
                }
            }
            return " (Function Not Supported)";
        }

        internal static void InitWindow(AnimationEventPopup popup)
        {
            popup.minSize = new Vector2(400f, 140f);
            popup.maxSize = new Vector2(400f, 140f);
            popup.titleContent = EditorGUIUtility.TextContent("Edit Animation Event");
        }

        internal static int InsertAnimationEvent(ref AnimationEvent[] events, AnimationClip clip, AnimationEvent evt)
        {
            Undo.RegisterCompleteObjectUndo(clip, "Add Event");
            int length = events.Length;
            for (int i = 0; i < events.Length; i++)
            {
                if (events[i].time > evt.time)
                {
                    length = i;
                    break;
                }
            }
            ArrayUtility.Insert<AnimationEvent>(ref events, length, evt);
            AnimationUtility.SetAnimationEvents(clip, events);
            events = AnimationUtility.GetAnimationEvents(clip);
            if ((events[length].time != evt.time) || (events[length].functionName != events[length].functionName))
            {
                Debug.LogError("Failed insertion");
            }
            return length;
        }

        private static bool IsSupportedMethodName(string name)
        {
            return ((!(name == "Main") && !(name == "Start")) && (!(name == "Awake") && !(name == "Update")));
        }

        private void OnDestroy()
        {
            if (this.m_Owner != null)
            {
                this.m_Owner.Focus();
            }
        }

        private void OnEnable()
        {
            base.titleContent = base.GetLocalizedTitleContent();
        }

        public void OnGUI()
        {
            AnimationEvent[] events = null;
            if (this.m_Clip != null)
            {
                events = AnimationUtility.GetAnimationEvents(this.m_Clip);
            }
            else if (this.m_ClipInfo != null)
            {
                events = this.m_ClipInfo.GetEvents();
            }
            if (((events != null) && (this.eventIndex >= 0)) && (this.eventIndex < events.Length))
            {
                GUI.changed = false;
                AnimationEvent evt = events[this.eventIndex];
                if (this.m_Root != null)
                {
                    List<string> list;
                    List<Type> list2;
                    CollectSupportedMethods(this.m_Root, out list, out list2);
                    List<string> list3 = new List<string>(list.Count);
                    for (int i = 0; i < list.Count; i++)
                    {
                        string str = " ( )";
                        if (list2[i] != null)
                        {
                            if (list2[i] == typeof(float))
                            {
                                str = " ( float )";
                            }
                            else if (list2[i] == typeof(int))
                            {
                                str = " ( int )";
                            }
                            else
                            {
                                str = string.Format(" ( {0} )", list2[i].Name);
                            }
                        }
                        list3.Add(list[i] + str);
                    }
                    int count = list.Count;
                    int index = list.IndexOf(evt.functionName);
                    if (index == -1)
                    {
                        index = list.Count;
                        list.Add(evt.functionName);
                        if (string.IsNullOrEmpty(evt.functionName))
                        {
                            list3.Add("(No Function Selected)");
                        }
                        else
                        {
                            list3.Add(evt.functionName + " (Function Not Supported)");
                        }
                        list2.Add(null);
                    }
                    EditorGUIUtility.labelWidth = 130f;
                    int num4 = index;
                    index = EditorGUILayout.Popup("Function: ", index, list3.ToArray(), new GUILayoutOption[0]);
                    if (((num4 != index) && (index != -1)) && (index != count))
                    {
                        evt.functionName = list[index];
                        evt.stringParameter = string.Empty;
                    }
                    Type selectedParameter = list2[index];
                    if (selectedParameter != null)
                    {
                        EditorGUILayout.Space();
                        if (selectedParameter == typeof(AnimationEvent))
                        {
                            EditorGUILayout.PrefixLabel("Event Data");
                        }
                        else
                        {
                            EditorGUILayout.PrefixLabel("Parameters");
                        }
                        DoEditRegularParameters(evt, selectedParameter);
                    }
                }
                else
                {
                    evt.functionName = EditorGUILayout.TextField(new GUIContent("Function"), evt.functionName, new GUILayoutOption[0]);
                    DoEditRegularParameters(evt, typeof(AnimationEvent));
                }
                if (GUI.changed)
                {
                    if (this.m_Clip != null)
                    {
                        Undo.RegisterCompleteObjectUndo(this.m_Clip, "Animation Event Change");
                        AnimationUtility.SetAnimationEvents(this.m_Clip, events);
                    }
                    else if (this.m_ClipInfo != null)
                    {
                        this.m_ClipInfo.SetEvent(this.m_EventIndex, evt);
                    }
                }
            }
        }

        internal static void UpdateSelection(GameObject root, AnimationClip clip, int index, EditorWindow owner)
        {
            Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(AnimationEventPopup));
            AnimationEventPopup popup = (objArray.Length <= 0) ? null : ((AnimationEventPopup) objArray[0]);
            if (popup != null)
            {
                popup.m_Root = root;
                popup.m_Clip = clip;
                popup.eventIndex = index;
                popup.m_Owner = owner;
                popup.Repaint();
            }
        }

        public AnimationClipInfoProperties clipInfo
        {
            get
            {
                return this.m_ClipInfo;
            }
            set
            {
                this.m_ClipInfo = value;
            }
        }

        private int eventIndex
        {
            get
            {
                return this.m_EventIndex;
            }
            set
            {
                this.m_EventIndex = value;
            }
        }
    }
}

