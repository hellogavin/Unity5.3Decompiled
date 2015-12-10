namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Text;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Scripting;

    [EditorWindowTitle(title="Console", useTypeNameAsIconName=true)]
    internal class ConsoleWindow : EditorWindow, IHasCustomMenu
    {
        internal static Texture2D iconError;
        internal static Texture2D iconErrorMono;
        internal static Texture2D iconErrorSmall;
        internal static Texture2D iconInfo;
        internal static Texture2D iconInfoMono;
        internal static Texture2D iconInfoSmall;
        internal static Texture2D iconWarn;
        internal static Texture2D iconWarnMono;
        internal static Texture2D iconWarnSmall;
        private int m_ActiveInstanceID;
        private string m_ActiveText = string.Empty;
        private bool m_DevBuild;
        private ListViewState m_ListView;
        private const int m_RowHeight = 0x20;
        private Vector2 m_TextScroll = Vector2.zero;
        private static ConsoleWindow ms_ConsoleWindow;
        private static bool ms_LoadedIcons;
        private int ms_LVHeight;
        private SplitterState spl;

        public ConsoleWindow()
        {
            float[] relativeSizes = new float[] { 70f, 30f };
            int[] minSizes = new int[] { 0x20, 0x20 };
            this.spl = new SplitterState(relativeSizes, minSizes, null);
            base.position = new Rect(200f, 200f, 800f, 400f);
            this.m_ListView = new ListViewState(0, 0x20);
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                menu.AddItem(new GUIContent("Open Player Log"), false, new GenericMenu.MenuFunction(InternalEditorUtility.OpenPlayerConsole));
            }
            menu.AddItem(new GUIContent("Open Editor Log"), false, new GenericMenu.MenuFunction(InternalEditorUtility.OpenEditorConsole));
            IEnumerator enumerator = Enum.GetValues(typeof(StackTraceLogType)).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    StackTraceLogType current = (StackTraceLogType) ((int) enumerator.Current);
                    menu.AddItem(new GUIContent("Stack Trace Logging/" + current), Application.stackTraceLogType == current, new GenericMenu.MenuFunction2(this.ToggleLogStackTraces), current);
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
        }

        private static string ContextString(LogEntry entry)
        {
            StringBuilder builder = new StringBuilder();
            if (HasMode(entry.mode, Mode.Error))
            {
                builder.Append("Error ");
            }
            else if (HasMode(entry.mode, Mode.Log))
            {
                builder.Append("Log ");
            }
            else
            {
                builder.Append("Assert ");
            }
            builder.Append("in file: ");
            builder.Append(entry.file);
            builder.Append(" at line: ");
            builder.Append(entry.line);
            if (entry.errorNum != 0)
            {
                builder.Append(" and errorNum: ");
                builder.Append(entry.errorNum);
            }
            return builder.ToString();
        }

        public void DoLogChanged()
        {
            ms_ConsoleWindow.Repaint();
        }

        private static string GetFirstLine(string s)
        {
            int index = s.IndexOf("\n");
            return ((index == -1) ? s : s.Substring(0, index));
        }

        private static string GetFirstTwoLines(string s)
        {
            int index = s.IndexOf("\n");
            if (index != -1)
            {
                index = s.IndexOf("\n", (int) (index + 1));
                if (index != -1)
                {
                    return s.Substring(0, index);
                }
            }
            return s;
        }

        internal static Texture2D GetIconForErrorMode(int mode, bool large)
        {
            if (HasMode(mode, Mode.ScriptingAssertion | Mode.GraphCompileError | Mode.ScriptCompileError | Mode.ScriptingError | Mode.AssetImportError | Mode.Fatal | Mode.Assert | Mode.Error))
            {
                return (!large ? iconErrorSmall : iconError);
            }
            if (HasMode(mode, Mode.ScriptCompileWarning | Mode.ScriptingWarning | Mode.AssetImportWarning))
            {
                return (!large ? iconWarnSmall : iconWarn);
            }
            if (HasMode(mode, Mode.ScriptingLog | Mode.Log))
            {
                return (!large ? iconInfoSmall : iconInfo);
            }
            return null;
        }

        internal static GUIStyle GetStatusStyleForErrorMode(int mode)
        {
            if (HasMode(mode, Mode.ScriptingAssertion | Mode.GraphCompileError | Mode.ScriptCompileError | Mode.ScriptingError | Mode.AssetImportError | Mode.Fatal | Mode.Assert | Mode.Error))
            {
                return Constants.StatusError;
            }
            if (HasMode(mode, Mode.ScriptCompileWarning | Mode.ScriptingWarning | Mode.AssetImportWarning))
            {
                return Constants.StatusWarn;
            }
            return Constants.StatusLog;
        }

        internal static GUIStyle GetStyleForErrorMode(int mode)
        {
            if (HasMode(mode, Mode.ScriptingAssertion | Mode.GraphCompileError | Mode.ScriptCompileError | Mode.ScriptingError | Mode.AssetImportError | Mode.Fatal | Mode.Assert | Mode.Error))
            {
                return Constants.ErrorStyle;
            }
            if (HasMode(mode, Mode.ScriptCompileWarning | Mode.ScriptingWarning | Mode.AssetImportWarning))
            {
                return Constants.WarningStyle;
            }
            return Constants.LogStyle;
        }

        private bool HasFlag(ConsoleFlags flags)
        {
            return ((LogEntries.consoleFlags & flags) != 0);
        }

        private static bool HasMode(int mode, Mode modeToCheck)
        {
            return ((mode & modeToCheck) != 0);
        }

        internal static void LoadIcons()
        {
            if (!ms_LoadedIcons)
            {
                ms_LoadedIcons = true;
                iconInfo = EditorGUIUtility.LoadIcon("console.infoicon");
                iconWarn = EditorGUIUtility.LoadIcon("console.warnicon");
                iconError = EditorGUIUtility.LoadIcon("console.erroricon");
                iconInfoSmall = EditorGUIUtility.LoadIcon("console.infoicon.sml");
                iconWarnSmall = EditorGUIUtility.LoadIcon("console.warnicon.sml");
                iconErrorSmall = EditorGUIUtility.LoadIcon("console.erroricon.sml");
                iconInfoMono = EditorGUIUtility.LoadIcon("console.infoicon.sml");
                iconWarnMono = EditorGUIUtility.LoadIcon("console.warnicon.inactive.sml");
                iconErrorMono = EditorGUIUtility.LoadIcon("console.erroricon.inactive.sml");
                Constants.Init();
            }
        }

        [RequiredByNativeCode]
        public static void LogChanged()
        {
            if (ms_ConsoleWindow != null)
            {
                ms_ConsoleWindow.DoLogChanged();
            }
        }

        private void OnDisable()
        {
            if (ms_ConsoleWindow == this)
            {
                ms_ConsoleWindow = null;
            }
        }

        private void OnEnable()
        {
            base.titleContent = base.GetLocalizedTitleContent();
            ms_ConsoleWindow = this;
            this.m_DevBuild = Unsupported.IsDeveloperBuild();
        }

        private void OnGUI()
        {
            Event current = Event.current;
            LoadIcons();
            GUILayout.BeginHorizontal(Constants.Toolbar, new GUILayoutOption[0]);
            if (GUILayout.Button("Clear", Constants.MiniButton, new GUILayoutOption[0]))
            {
                LogEntries.Clear();
                GUIUtility.keyboardControl = 0;
            }
            int count = LogEntries.GetCount();
            if ((this.m_ListView.totalRows != count) && (this.m_ListView.scrollPos.y >= ((this.m_ListView.rowHeight * this.m_ListView.totalRows) - this.ms_LVHeight)))
            {
                this.m_ListView.scrollPos.y = (count * 0x20) - this.ms_LVHeight;
            }
            EditorGUILayout.Space();
            bool flag = this.HasFlag(ConsoleFlags.Collapse);
            this.SetFlag(ConsoleFlags.Collapse, GUILayout.Toggle(flag, "Collapse", Constants.MiniButtonLeft, new GUILayoutOption[0]));
            if (flag != this.HasFlag(ConsoleFlags.Collapse))
            {
                this.m_ListView.row = -1;
                this.m_ListView.scrollPos.y = LogEntries.GetCount() * 0x20;
            }
            this.SetFlag(ConsoleFlags.ClearOnPlay, GUILayout.Toggle(this.HasFlag(ConsoleFlags.ClearOnPlay), "Clear on Play", Constants.MiniButtonMiddle, new GUILayoutOption[0]));
            this.SetFlag(ConsoleFlags.ErrorPause, GUILayout.Toggle(this.HasFlag(ConsoleFlags.ErrorPause), "Error Pause", Constants.MiniButtonRight, new GUILayoutOption[0]));
            EditorGUILayout.Space();
            if (this.m_DevBuild)
            {
                GUILayout.FlexibleSpace();
                this.SetFlag(ConsoleFlags.StopForAssert, GUILayout.Toggle(this.HasFlag(ConsoleFlags.StopForAssert), "Stop for Assert", Constants.MiniButtonLeft, new GUILayoutOption[0]));
                this.SetFlag(ConsoleFlags.StopForError, GUILayout.Toggle(this.HasFlag(ConsoleFlags.StopForError), "Stop for Error", Constants.MiniButtonRight, new GUILayoutOption[0]));
            }
            GUILayout.FlexibleSpace();
            int errorCount = 0;
            int warningCount = 0;
            int logCount = 0;
            LogEntries.GetCountsByType(ref errorCount, ref warningCount, ref logCount);
            bool val = GUILayout.Toggle(this.HasFlag(ConsoleFlags.LogLevelLog), new GUIContent((logCount > 0x3e7) ? "999+" : logCount.ToString(), (logCount <= 0) ? iconInfoMono : iconInfoSmall), Constants.MiniButtonRight, new GUILayoutOption[0]);
            bool flag4 = GUILayout.Toggle(this.HasFlag(ConsoleFlags.LogLevelWarning), new GUIContent((warningCount > 0x3e7) ? "999+" : warningCount.ToString(), (warningCount <= 0) ? iconWarnMono : iconWarnSmall), Constants.MiniButtonMiddle, new GUILayoutOption[0]);
            bool flag5 = GUILayout.Toggle(this.HasFlag(ConsoleFlags.LogLevelError), new GUIContent((errorCount > 0x3e7) ? "999+" : errorCount.ToString(), (errorCount <= 0) ? iconErrorMono : iconErrorSmall), Constants.MiniButtonLeft, new GUILayoutOption[0]);
            this.SetFlag(ConsoleFlags.LogLevelLog, val);
            this.SetFlag(ConsoleFlags.LogLevelWarning, flag4);
            this.SetFlag(ConsoleFlags.LogLevelError, flag5);
            GUILayout.EndHorizontal();
            this.m_ListView.totalRows = LogEntries.StartGettingEntries();
            SplitterGUILayout.BeginVerticalSplit(this.spl, new GUILayoutOption[0]);
            EditorGUIUtility.SetIconSize(new Vector2(32f, 32f));
            GUIContent content = new GUIContent();
            int controlID = GUIUtility.GetControlID(FocusType.Native);
            try
            {
                bool flag6 = false;
                bool flag7 = this.HasFlag(ConsoleFlags.Collapse);
                IEnumerator enumerator = ListViewGUI.ListView(this.m_ListView, Constants.Box, new GUILayoutOption[0]).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        ListViewElement element = (ListViewElement) enumerator.Current;
                        if (((current.type == EventType.MouseDown) && (current.button == 0)) && element.position.Contains(current.mousePosition))
                        {
                            if (current.clickCount == 2)
                            {
                                LogEntries.RowGotDoubleClicked(this.m_ListView.row);
                            }
                            flag6 = true;
                        }
                        if (current.type == EventType.Repaint)
                        {
                            int mask = 0;
                            string outString = null;
                            LogEntries.GetFirstTwoLinesEntryTextAndModeInternal(element.row, ref mask, ref outString);
                            (((element.row % 2) != 0) ? Constants.EvenBackground : Constants.OddBackground).Draw(element.position, false, false, this.m_ListView.row == element.row, false);
                            content.text = outString;
                            GetStyleForErrorMode(mask).Draw(element.position, content, controlID, this.m_ListView.row == element.row);
                            if (flag7)
                            {
                                Rect position = element.position;
                                content.text = LogEntries.GetEntryCount(element.row).ToString(CultureInfo.InvariantCulture);
                                Vector2 vector = Constants.CountBadge.CalcSize(content);
                                position.xMin = position.xMax - vector.x;
                                position.yMin += ((position.yMax - position.yMin) - vector.y) * 0.5f;
                                position.x -= 5f;
                                GUI.Label(position, content, Constants.CountBadge);
                            }
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
                if (flag6 && (this.m_ListView.scrollPos.y >= ((this.m_ListView.rowHeight * this.m_ListView.totalRows) - this.ms_LVHeight)))
                {
                    this.m_ListView.scrollPos.y = ((this.m_ListView.rowHeight * this.m_ListView.totalRows) - this.ms_LVHeight) - 1;
                }
                if (((this.m_ListView.totalRows == 0) || (this.m_ListView.row >= this.m_ListView.totalRows)) || (this.m_ListView.row < 0))
                {
                    if (this.m_ActiveText.Length != 0)
                    {
                        this.SetActiveEntry(null);
                    }
                }
                else
                {
                    LogEntry outputEntry = new LogEntry();
                    LogEntries.GetEntryInternal(this.m_ListView.row, outputEntry);
                    this.SetActiveEntry(outputEntry);
                    LogEntries.GetEntryInternal(this.m_ListView.row, outputEntry);
                    if (this.m_ListView.selectionChanged || !this.m_ActiveText.Equals(outputEntry.condition))
                    {
                        this.SetActiveEntry(outputEntry);
                    }
                }
                if (((GUIUtility.keyboardControl == this.m_ListView.ID) && (current.type == EventType.KeyDown)) && ((current.keyCode == KeyCode.Return) && (this.m_ListView.row != 0)))
                {
                    LogEntries.RowGotDoubleClicked(this.m_ListView.row);
                    Event.current.Use();
                }
                if ((current.type != EventType.Layout) && (ListViewGUI.ilvState.rectHeight != 1))
                {
                    this.ms_LVHeight = ListViewGUI.ilvState.rectHeight;
                }
            }
            finally
            {
                LogEntries.EndGettingEntries();
                EditorGUIUtility.SetIconSize(Vector2.zero);
            }
            this.m_TextScroll = GUILayout.BeginScrollView(this.m_TextScroll, Constants.Box);
            float minHeight = Constants.MessageStyle.CalcHeight(GUIContent.Temp(this.m_ActiveText), base.position.width);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.MinHeight(minHeight) };
            EditorGUILayout.SelectableLabel(this.m_ActiveText, Constants.MessageStyle, options);
            GUILayout.EndScrollView();
            SplitterGUILayout.EndVerticalSplit();
            if (((current.type == EventType.ValidateCommand) || (current.type == EventType.ExecuteCommand)) && ((current.commandName == "Copy") && (this.m_ActiveText != string.Empty)))
            {
                if (current.type == EventType.ExecuteCommand)
                {
                    EditorGUIUtility.systemCopyBuffer = this.m_ActiveText;
                }
                current.Use();
            }
        }

        private void SetActiveEntry(LogEntry entry)
        {
            if (entry != null)
            {
                this.m_ActiveText = entry.condition;
                if (this.m_ActiveInstanceID != entry.instanceID)
                {
                    this.m_ActiveInstanceID = entry.instanceID;
                    if (entry.instanceID != 0)
                    {
                        EditorGUIUtility.PingObject(entry.instanceID);
                    }
                }
            }
            else
            {
                this.m_ActiveText = string.Empty;
                this.m_ActiveInstanceID = 0;
                this.m_ListView.row = -1;
            }
        }

        private void SetFlag(ConsoleFlags flags, bool val)
        {
            LogEntries.SetConsoleFlag((int) flags, val);
        }

        private static void ShowConsoleRow(int row)
        {
            ShowConsoleWindow(false);
            if (ms_ConsoleWindow != null)
            {
                ms_ConsoleWindow.m_ListView.row = row;
                ms_ConsoleWindow.m_ListView.selectionChanged = true;
                ms_ConsoleWindow.Repaint();
            }
        }

        public static void ShowConsoleWindow(bool immediate)
        {
            if (ms_ConsoleWindow == null)
            {
                ms_ConsoleWindow = ScriptableObject.CreateInstance<ConsoleWindow>();
                ms_ConsoleWindow.Show(immediate);
                ms_ConsoleWindow.Focus();
            }
            else
            {
                ms_ConsoleWindow.Show(immediate);
                ms_ConsoleWindow.Focus();
            }
        }

        private static void ShowConsoleWindowImmediate()
        {
            ShowConsoleWindow(true);
        }

        public void ToggleLogStackTraces(object userData)
        {
            Application.stackTraceLogType = (StackTraceLogType) ((int) userData);
        }

        private enum ConsoleFlags
        {
            Autoscroll = 0x40,
            ClearOnPlay = 2,
            Collapse = 1,
            ErrorPause = 4,
            LogLevelError = 0x200,
            LogLevelLog = 0x80,
            LogLevelWarning = 0x100,
            StopForAssert = 0x10,
            StopForError = 0x20,
            Verbose = 8
        }

        internal class Constants
        {
            public static GUIStyle Box;
            public static GUIStyle Button;
            public static GUIStyle CountBadge;
            public static GUIStyle ErrorStyle;
            public static GUIStyle EvenBackground;
            public static GUIStyle LogStyle;
            public static GUIStyle MessageStyle;
            public static GUIStyle MiniButton;
            public static GUIStyle MiniButtonLeft;
            public static GUIStyle MiniButtonMiddle;
            public static GUIStyle MiniButtonRight;
            public static bool ms_Loaded;
            public static GUIStyle OddBackground;
            public static GUIStyle StatusError;
            public static GUIStyle StatusLog;
            public static GUIStyle StatusWarn;
            public static GUIStyle Toolbar;
            public static GUIStyle WarningStyle;

            public static void Init()
            {
                if (!ms_Loaded)
                {
                    ms_Loaded = true;
                    Box = "CN Box";
                    Button = "Button";
                    MiniButton = "ToolbarButton";
                    MiniButtonLeft = "ToolbarButton";
                    MiniButtonMiddle = "ToolbarButton";
                    MiniButtonRight = "ToolbarButton";
                    Toolbar = "Toolbar";
                    LogStyle = "CN EntryInfo";
                    WarningStyle = "CN EntryWarn";
                    ErrorStyle = "CN EntryError";
                    EvenBackground = "CN EntryBackEven";
                    OddBackground = "CN EntryBackodd";
                    MessageStyle = "CN Message";
                    StatusError = "CN StatusError";
                    StatusWarn = "CN StatusWarn";
                    StatusLog = "CN StatusInfo";
                    CountBadge = "CN CountBadge";
                }
            }
        }

        private enum Mode
        {
            Assert = 2,
            AssetImportError = 0x40,
            AssetImportWarning = 0x80,
            DisplayPreviousErrorInStatusBar = 0x10000,
            DontExtractStacktrace = 0x40000,
            DontPreprocessCondition = 0x20,
            Error = 1,
            Fatal = 0x10,
            GraphCompileError = 0x100000,
            Log = 4,
            MayIgnoreLineNumber = 0x4000,
            ReportBug = 0x8000,
            ScriptCompileError = 0x800,
            ScriptCompileWarning = 0x1000,
            ScriptingAssertion = 0x200000,
            ScriptingError = 0x100,
            ScriptingException = 0x20000,
            ScriptingLog = 0x400,
            ScriptingWarning = 0x200,
            ShouldClearOnPlay = 0x80000,
            StickyError = 0x2000
        }
    }
}

