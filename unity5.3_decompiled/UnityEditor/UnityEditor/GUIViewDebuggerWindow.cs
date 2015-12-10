namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class GUIViewDebuggerWindow : EditorWindow
    {
        [NonSerialized]
        private CachedInstructionInfo m_CachedinstructionInfo;
        private GUIView m_Inspected;
        [NonSerialized]
        private GUIInstruction m_Instruction;
        private Vector2 m_InstructionDetailsScrollPos = new Vector2();
        private readonly SplitterState m_InstructionDetailStacktraceSplitter;
        private readonly SplitterState m_InstructionListDetailSplitter;
        private InstructionOverlayWindow m_InstructionOverlayWindow;
        [NonSerialized]
        private int m_LastSelectedRow;
        [NonSerialized]
        private readonly ListViewState m_ListViewState = new ListViewState();
        [NonSerialized]
        private Vector2 m_PointToInspect;
        [NonSerialized]
        private bool m_QueuedPointInspection;
        private bool m_ShowOverlay = true;
        private Vector2 m_StacktraceScrollPos = new Vector2();
        private static GUIViewDebuggerWindow s_ActiveInspector;
        private static Styles s_Styles;

        public GUIViewDebuggerWindow()
        {
            float[] relativeSizes = new float[] { 30f, 70f };
            int[] minSizes = new int[] { 0x20, 0x20 };
            this.m_InstructionListDetailSplitter = new SplitterState(relativeSizes, minSizes, null);
            float[] singleArray2 = new float[] { 80f, 20f };
            int[] numArray2 = new int[] { 100, 100 };
            this.m_InstructionDetailStacktraceSplitter = new SplitterState(singleArray2, numArray2, null);
        }

        private bool CanInspectView(GUIView view)
        {
            EditorWindow editorWindow = GetEditorWindow(view);
            return ((editorWindow == null) || ((editorWindow != this) && (editorWindow != this.m_InstructionOverlayWindow)));
        }

        private void DoInstructionOverlayToggle()
        {
            EditorGUI.BeginChangeCheck();
            this.m_ShowOverlay = GUILayout.Toggle(this.m_ShowOverlay, GUIContent.Temp("Show overlay"), EditorStyles.toolbarButton, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.OnShowOverlayChanged();
            }
        }

        private void DoToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
            this.DoWindowPopup();
            this.DoInstructionOverlayToggle();
            GUILayout.EndHorizontal();
        }

        private void DoWindowPopup()
        {
            string t = "<Please Select>";
            if (this.m_Inspected != null)
            {
                t = GetViewName(this.m_Inspected);
            }
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            GUILayout.Label("Inspected Window: ", options);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            Rect position = GUILayoutUtility.GetRect(GUIContent.Temp(t), EditorStyles.toolbarDropDown, optionArray2);
            if (GUI.Button(position, GUIContent.Temp(t), EditorStyles.toolbarDropDown))
            {
                List<GUIView> views = new List<GUIView>();
                GUIViewDebuggerHelper.GetViews(views);
                List<GUIContent> list2 = new List<GUIContent>(views.Count + 1) {
                    new GUIContent("None")
                };
                int selected = 0;
                List<GUIView> userData = new List<GUIView>(views.Count + 1);
                for (int i = 0; i < views.Count; i++)
                {
                    GUIView view = views[i];
                    if (this.CanInspectView(view))
                    {
                        GUIContent item = new GUIContent(list2.Count + ". " + GetViewName(view));
                        list2.Add(item);
                        userData.Add(view);
                        if (view == this.m_Inspected)
                        {
                            selected = userData.Count;
                        }
                    }
                }
                EditorUtility.DisplayCustomMenu(position, list2.ToArray(), selected, new EditorUtility.SelectMenuItemFunction(this.OnWindowSelected), userData);
            }
        }

        private void DrawInspectedGUIContent()
        {
            GUILayout.Label(GUIContent.Temp("GUIContent"), new GUILayoutOption[0]);
            EditorGUI.indentLevel++;
            EditorGUILayout.TextField(this.m_Instruction.usedGUIContent.text, new GUILayoutOption[0]);
            EditorGUILayout.ObjectField(this.m_Instruction.usedGUIContent.image, typeof(Texture2D), false, new GUILayoutOption[0]);
            EditorGUI.indentLevel--;
        }

        private void DrawInspectedRect()
        {
            EditorGUILayout.RectField(GUIContent.Temp("Rect"), this.m_Instruction.rect, new GUILayoutOption[0]);
        }

        private void DrawInspectedStacktrace()
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandHeight(false) };
            this.m_StacktraceScrollPos = EditorGUILayout.BeginScrollView(this.m_StacktraceScrollPos, s_Styles.stacktraceBackground, options);
            if (this.m_Instruction.stackframes != null)
            {
                foreach (StackFrame frame in this.m_Instruction.stackframes)
                {
                    if (!string.IsNullOrEmpty(frame.sourceFile))
                    {
                        GUILayout.Label(string.Format("{0} [{1}:{2}]", frame.signature, frame.sourceFile, frame.lineNumber), s_Styles.stackframeStyle, new GUILayoutOption[0]);
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawInspectedStyle()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.m_CachedinstructionInfo.styleSerializedProperty, GUIContent.Temp("Style"), true, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_CachedinstructionInfo.styleContainerSerializedObject.ApplyModifiedPropertiesWithoutUndo();
                this.m_Inspected.Repaint();
            }
        }

        private void DrawInstructionList()
        {
            Event current = Event.current;
            EditorGUILayout.BeginVertical(s_Styles.listBackgroundStyle, new GUILayoutOption[0]);
            GUILayout.Label("Instructions", new GUILayoutOption[0]);
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
            IEnumerator enumerator = ListViewGUI.ListView(this.m_ListViewState, s_Styles.listBackgroundStyle, new GUILayoutOption[0]).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ListViewElement el = (ListViewElement) enumerator.Current;
                    if (((current.type == EventType.MouseDown) && (current.button == 0)) && (el.position.Contains(current.mousePosition) && (current.clickCount == 2)))
                    {
                        this.ShowInstructionInExternalEditor(el.row);
                    }
                    if (current.type == EventType.Repaint)
                    {
                        GUIContent content = GUIContent.Temp(this.GetInstructionName(el));
                        s_Styles.listItemBackground.Draw(el.position, false, false, this.m_ListViewState.row == el.row, false);
                        s_Styles.listItem.Draw(el.position, content, controlID, this.m_ListViewState.row == el.row);
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawSelectedInstructionDetails()
        {
            if (this.m_Instruction == null)
            {
                EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                GUILayout.Label("Select a Instruction on the left to see details", s_Styles.centeredText, new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();
            }
            else
            {
                SplitterGUILayout.BeginVerticalSplit(this.m_InstructionDetailStacktraceSplitter, new GUILayoutOption[0]);
                this.m_InstructionDetailsScrollPos = EditorGUILayout.BeginScrollView(this.m_InstructionDetailsScrollPos, s_Styles.boxStyle, new GUILayoutOption[0]);
                EditorGUI.BeginDisabledGroup(true);
                this.DrawInspectedRect();
                EditorGUI.EndDisabledGroup();
                this.DrawInspectedStyle();
                EditorGUI.BeginDisabledGroup(true);
                this.DrawInspectedGUIContent();
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndScrollView();
                this.DrawInspectedStacktrace();
                SplitterGUILayout.EndVerticalSplit();
            }
        }

        private int FindInstructionUnderPoint(Vector2 point)
        {
            int instructionCount = GUIViewDebuggerHelper.GetInstructionCount();
            for (int i = 0; i < instructionCount; i++)
            {
                if (GUIViewDebuggerHelper.GetRectFromInstruction(i).Contains(point))
                {
                    return i;
                }
            }
            return -1;
        }

        private static EditorWindow GetEditorWindow(GUIView view)
        {
            HostView view2 = view as HostView;
            if (view2 != null)
            {
                return view2.actualView;
            }
            return null;
        }

        private string GetInstructionListName(StackFrame[] stacktrace)
        {
            int interestingFrameIndex = this.GetInterestingFrameIndex(stacktrace);
            if (interestingFrameIndex > 0)
            {
                interestingFrameIndex--;
            }
            StackFrame frame = stacktrace[interestingFrameIndex];
            return frame.methodName;
        }

        private string GetInstructionName(ListViewElement el)
        {
            int row = el.row;
            StackFrame[] managedStackTrace = GUIViewDebuggerHelper.GetManagedStackTrace(row);
            string instructionListName = this.GetInstructionListName(managedStackTrace);
            return string.Format("{0}. {1}", row, instructionListName);
        }

        private int GetInterestingFrameIndex(StackFrame[] stacktrace)
        {
            string dataPath = Application.dataPath;
            int num = -1;
            for (int i = 0; i < stacktrace.Length; i++)
            {
                StackFrame frame = stacktrace[i];
                if ((!string.IsNullOrEmpty(frame.sourceFile) && !frame.signature.StartsWith("UnityEngine.GUI")) && !frame.signature.StartsWith("UnityEditor.EditorGUI"))
                {
                    if (num == -1)
                    {
                        num = i;
                    }
                    if (frame.sourceFile.StartsWith(dataPath))
                    {
                        return i;
                    }
                }
            }
            if (num != -1)
            {
                return num;
            }
            return (stacktrace.Length - 1);
        }

        private void GetSelectedStyleProperty(out SerializedObject serializedObject, out SerializedProperty styleProperty)
        {
            GUISkin skin = null;
            GUISkin current = GUISkin.current;
            GUIStyle style = current.FindStyle(this.m_Instruction.usedGUIStyle.name);
            if ((style != null) && (style == this.m_Instruction.usedGUIStyle))
            {
                skin = current;
            }
            styleProperty = null;
            if (skin != null)
            {
                serializedObject = new SerializedObject(skin);
                SerializedProperty iterator = serializedObject.GetIterator();
                bool enterChildren = true;
                while (iterator.NextVisible(enterChildren))
                {
                    if (iterator.type == "GUIStyle")
                    {
                        enterChildren = false;
                        if (iterator.FindPropertyRelative("m_Name").stringValue == this.m_Instruction.usedGUIStyle.name)
                        {
                            styleProperty = iterator;
                            return;
                        }
                    }
                    else
                    {
                        enterChildren = true;
                    }
                }
                Debug.Log(string.Format("Showing editable Style from GUISkin: {0}, IsPersistant: {1}", skin.name, EditorUtility.IsPersistent(skin)));
            }
            serializedObject = new SerializedObject(this.m_CachedinstructionInfo.styleContainer);
            styleProperty = serializedObject.FindProperty("inspectedStyle");
        }

        private static string GetViewName(GUIView view)
        {
            EditorWindow editorWindow = GetEditorWindow(view);
            if (editorWindow != null)
            {
                return editorWindow.titleContent.text;
            }
            return view.GetType().Name;
        }

        private void HighlightInstruction(GUIView view, Rect instructionRect, GUIStyle style)
        {
            if ((this.m_ListViewState.row >= 0) && this.m_ShowOverlay)
            {
                if (this.m_InstructionOverlayWindow == null)
                {
                    this.m_InstructionOverlayWindow = ScriptableObject.CreateInstance<InstructionOverlayWindow>();
                }
                this.m_InstructionOverlayWindow.Show(view, instructionRect, style);
                base.Focus();
            }
        }

        private static void Init()
        {
            if (s_ActiveInspector == null)
            {
                GUIViewDebuggerWindow window = (GUIViewDebuggerWindow) EditorWindow.GetWindow(typeof(GUIViewDebuggerWindow));
                s_ActiveInspector = window;
            }
            s_ActiveInspector.Show();
        }

        private void InitializeStylesIfNeeded()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
        }

        private static void InspectPoint(Vector2 point)
        {
            Debug.Log("Inspecting " + point);
            s_ActiveInspector.InspectPointAt(point);
        }

        private void InspectPointAt(Vector2 point)
        {
            this.m_PointToInspect = point;
            this.m_QueuedPointInspection = true;
            this.m_Inspected.Repaint();
            base.Repaint();
        }

        private void OnDisable()
        {
            if (this.m_Inspected != null)
            {
                GUIViewDebuggerHelper.DebugWindow(this.m_Inspected);
            }
            if (this.m_InstructionOverlayWindow != null)
            {
                this.m_InstructionOverlayWindow.Close();
            }
        }

        private void OnEnable()
        {
            base.titleContent = new GUIContent("GUI Inspector");
        }

        private void OnGUI()
        {
            this.InitializeStylesIfNeeded();
            this.DoToolbar();
            this.ShowDrawInstructions();
        }

        private static void OnInspectedViewChanged()
        {
            if (s_ActiveInspector != null)
            {
                s_ActiveInspector.Repaint();
            }
        }

        private void OnSelectedInstructionChanged()
        {
            if (this.m_ListViewState.row >= 0)
            {
                if (this.m_Instruction == null)
                {
                    this.m_Instruction = new GUIInstruction();
                }
                if (this.m_CachedinstructionInfo == null)
                {
                    this.m_CachedinstructionInfo = new CachedInstructionInfo();
                }
                this.m_Instruction.rect = GUIViewDebuggerHelper.GetRectFromInstruction(this.m_ListViewState.row);
                this.m_Instruction.usedGUIStyle = GUIViewDebuggerHelper.GetStyleFromInstruction(this.m_ListViewState.row);
                this.m_Instruction.usedGUIContent = GUIViewDebuggerHelper.GetContentFromInstruction(this.m_ListViewState.row);
                this.m_Instruction.stackframes = GUIViewDebuggerHelper.GetManagedStackTrace(this.m_ListViewState.row);
                this.m_CachedinstructionInfo.styleContainer.inspectedStyle = this.m_Instruction.usedGUIStyle;
                this.m_CachedinstructionInfo.styleContainerSerializedObject = null;
                this.m_CachedinstructionInfo.styleSerializedProperty = null;
                this.GetSelectedStyleProperty(out this.m_CachedinstructionInfo.styleContainerSerializedObject, out this.m_CachedinstructionInfo.styleSerializedProperty);
                this.HighlightInstruction(this.m_Inspected, this.m_Instruction.rect, this.m_Instruction.usedGUIStyle);
            }
            else
            {
                this.m_Instruction = null;
                this.m_CachedinstructionInfo = null;
                if (this.m_InstructionOverlayWindow != null)
                {
                    this.m_InstructionOverlayWindow.Close();
                }
            }
        }

        private void OnShowOverlayChanged()
        {
            if (!this.m_ShowOverlay)
            {
                if (this.m_InstructionOverlayWindow != null)
                {
                    this.m_InstructionOverlayWindow.Close();
                }
            }
            else if ((this.m_Inspected != null) && (this.m_Instruction != null))
            {
                this.HighlightInstruction(this.m_Inspected, this.m_Instruction.rect, this.m_Instruction.usedGUIStyle);
            }
        }

        private void OnWindowSelected(object userdata, string[] options, int selected)
        {
            GUIView view;
            selected--;
            if (selected >= 0)
            {
                List<GUIView> list = (List<GUIView>) userdata;
                view = list[selected];
            }
            else
            {
                view = null;
            }
            if (this.m_Inspected != view)
            {
                if (this.m_InstructionOverlayWindow != null)
                {
                    this.m_InstructionOverlayWindow.Close();
                }
                this.m_Inspected = view;
                if (this.m_Inspected != null)
                {
                    GUIViewDebuggerHelper.DebugWindow(this.m_Inspected);
                    this.m_Inspected.Repaint();
                }
                this.m_ListViewState.row = -1;
                this.m_ListViewState.selectionChanged = true;
                this.m_Instruction = null;
            }
            base.Repaint();
        }

        private void ShowDrawInstructions()
        {
            if (this.m_Inspected != null)
            {
                this.m_ListViewState.totalRows = GUIViewDebuggerHelper.GetInstructionCount();
                if (this.m_QueuedPointInspection)
                {
                    this.m_ListViewState.row = this.FindInstructionUnderPoint(this.m_PointToInspect);
                    this.m_ListViewState.selectionChanged = true;
                    this.m_QueuedPointInspection = false;
                    this.m_Instruction.Reset();
                }
                SplitterGUILayout.BeginHorizontalSplit(this.m_InstructionListDetailSplitter, new GUILayoutOption[0]);
                this.DrawInstructionList();
                EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
                if (this.m_ListViewState.selectionChanged)
                {
                    this.OnSelectedInstructionChanged();
                }
                this.DrawSelectedInstructionDetails();
                EditorGUILayout.EndVertical();
                SplitterGUILayout.EndHorizontalSplit();
            }
        }

        private void ShowInstructionInExternalEditor(int row)
        {
            StackFrame[] managedStackTrace = GUIViewDebuggerHelper.GetManagedStackTrace(row);
            int interestingFrameIndex = this.GetInterestingFrameIndex(managedStackTrace);
            StackFrame frame = managedStackTrace[interestingFrameIndex];
            InternalEditorUtility.OpenFileAtLineExternal(frame.sourceFile, (int) frame.lineNumber);
        }

        [Serializable]
        private class CachedInstructionInfo
        {
            public readonly GUIStyleHolder styleContainer = ScriptableObject.CreateInstance<GUIStyleHolder>();
            public SerializedObject styleContainerSerializedObject;
            public SerializedProperty styleSerializedProperty;
        }

        private class GUIInstruction
        {
            public Rect rect;
            public StackFrame[] stackframes;
            public GUIContent usedGUIContent = GUIContent.none;
            public GUIStyle usedGUIStyle = GUIStyle.none;

            public void Reset()
            {
                this.rect = new Rect();
                this.usedGUIStyle = GUIStyle.none;
                this.usedGUIContent = GUIContent.none;
            }
        }

        private class Styles
        {
            public readonly GUIStyle boxStyle = new GUIStyle("CN Box");
            public readonly GUIStyle centeredText = new GUIStyle("PR Label");
            public readonly GUIStyle listBackgroundStyle = new GUIStyle("CN Box");
            public readonly GUIStyle listItem = new GUIStyle("PR Label");
            public readonly GUIStyle listItemBackground = new GUIStyle("CN EntryBackOdd");
            public readonly GUIStyle stackframeStyle = new GUIStyle(EditorStyles.label);
            public readonly GUIStyle stacktraceBackground = new GUIStyle("CN Box");

            public Styles()
            {
                this.stackframeStyle.margin = new RectOffset(0, 0, 0, 0);
                this.stackframeStyle.padding = new RectOffset(0, 0, 0, 0);
                this.stacktraceBackground.padding = new RectOffset(5, 5, 5, 5);
                this.centeredText.alignment = TextAnchor.MiddleCenter;
                this.centeredText.stretchHeight = true;
                this.centeredText.stretchWidth = true;
            }
        }
    }
}

