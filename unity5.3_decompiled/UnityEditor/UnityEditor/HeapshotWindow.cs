namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class HeapshotWindow : EditorWindow
    {
        private int[] connectionGuids;
        private int currentTab;
        private Rect guiRect;
        private const string heapshotExtension = ".heapshot";
        private List<string> heapshotFiles = new List<string>();
        private HeapshotReader heapshotReader;
        private List<HeapshotUIObject> hsAllObjects = new List<HeapshotUIObject>();
        private List<HeapshotUIObject> hsBackTraceObjects = new List<HeapshotUIObject>();
        private List<HeapshotUIObject> hsRoots = new List<HeapshotUIObject>();
        private int itemIndex = -1;
        private string lastOpenedHeapshotFile = string.Empty;
        private string lastOpenedProfiler = string.Empty;
        private Vector2 leftViewScrollPosition = Vector2.zero;
        private static UIStyles ms_Styles;
        private static DelegateReceivedHeapshot onReceivedHeapshot;
        private Vector2 rightViewScrollPosition = Vector2.zero;
        private int selectedHeapshot;
        private int selectedItem = -1;
        private string[] titleNames;
        private SplitterState titleSplit1;
        private SplitterState titleSplit2;
        private SplitterState viewSplit;

        public HeapshotWindow()
        {
            float[] relativeSizes = new float[] { 50f, 50f };
            this.viewSplit = new SplitterState(relativeSizes, null, null);
            this.titleNames = new string[] { "Field Name", "Type", "Pointer", "Size", "References/Referenced" };
            this.titleSplit1 = new SplitterState(new float[] { 30f, 25f, 15f, 15f, 15f }, new int[] { 200, 200, 50, 50, 50 }, null);
            this.titleSplit2 = new SplitterState(new float[] { 30f, 25f, 15f, 15f, 15f }, new int[] { 200, 200, 50, 50, 50 }, null);
            this.selectedHeapshot = -1;
        }

        private void DoActiveProfilerButton(Rect position)
        {
            if (EditorGUI.ButtonMouseDown(position, new GUIContent("Active Profler"), FocusType.Native, EditorStyles.toolbarDropDown))
            {
                int connectedProfiler = ProfilerDriver.connectedProfiler;
                this.connectionGuids = ProfilerDriver.GetAvailableProfilers();
                int length = this.connectionGuids.Length;
                int[] selected = new int[1];
                bool[] enabled = new bool[length];
                string[] options = new string[length];
                for (int i = 0; i < length; i++)
                {
                    int guid = this.connectionGuids[i];
                    bool flag = ProfilerDriver.IsIdentifierConnectable(guid);
                    enabled[i] = flag;
                    string connectionIdentifier = ProfilerDriver.GetConnectionIdentifier(guid);
                    if (!flag)
                    {
                        connectionIdentifier = connectionIdentifier + " (Version mismatch)";
                    }
                    options[i] = connectionIdentifier;
                    if (guid == connectedProfiler)
                    {
                        selected[0] = i;
                    }
                }
                EditorUtility.DisplayCustomMenu(position, options, enabled, selected, new EditorUtility.SelectMenuItemFunction(this.SelectProfilerClick), null);
            }
        }

        private void DoHeapshotObjects(List<HeapshotUIObject> objects, SplitterState splitter, int indent, OnSelect onSelect)
        {
            if (objects != null)
            {
                Event current = Event.current;
                foreach (HeapshotUIObject obj2 in objects)
                {
                    Rect position = new Rect(14f * indent, this.guiRect.y, 14f, this.guiRect.height);
                    Rect[] rectArray = new Rect[this.titleNames.Length];
                    float x = 14f * (indent + 1);
                    for (int i = 0; i < rectArray.Length; i++)
                    {
                        float width = (i != 0) ? ((float) splitter.realSizes[i]) : (splitter.realSizes[i] - x);
                        rectArray[i] = new Rect(x, this.guiRect.y, width, this.guiRect.height);
                        x += width;
                    }
                    if (current.type == EventType.Repaint)
                    {
                        Rect rect2 = new Rect(0f, 16f * this.itemIndex, base.position.width, 16f);
                        (((this.itemIndex & 1) != 0) ? Styles.entryOdd : Styles.entryEven).Draw(rect2, GUIContent.none, false, false, this.itemIndex == this.selectedItem, false);
                    }
                    if (obj2.HasChildren)
                    {
                        GUI.changed = false;
                        bool flag = GUI.Toggle(position, obj2.IsExpanded, GUIContent.none, Styles.foldout);
                        if (GUI.changed)
                        {
                            if (flag)
                            {
                                obj2.Expand();
                            }
                            else
                            {
                                obj2.Collapse();
                            }
                        }
                    }
                    GUI.changed = false;
                    bool flag2 = GUI.Toggle(rectArray[0], this.itemIndex == this.selectedItem, obj2.Name, Styles.numberLabel);
                    if (!obj2.IsDummyObject)
                    {
                        GUI.Toggle(rectArray[1], this.itemIndex == this.selectedItem, obj2.TypeName, Styles.numberLabel);
                        GUI.Toggle(rectArray[2], this.itemIndex == this.selectedItem, "0x" + obj2.Code.ToString("X"), Styles.numberLabel);
                        GUI.Toggle(rectArray[3], this.itemIndex == this.selectedItem, obj2.Size.ToString(), Styles.numberLabel);
                        GUI.Toggle(rectArray[4], this.itemIndex == this.selectedItem, string.Format("{0} / {1}", obj2.ReferenceCount, obj2.InverseReferenceCount), Styles.numberLabel);
                        if ((GUI.changed && flag2) && (onSelect != null))
                        {
                            this.selectedItem = this.itemIndex;
                            onSelect(obj2);
                        }
                    }
                    this.itemIndex++;
                    this.guiRect.y += 16f;
                    this.DoHeapshotObjects(obj2.Children, splitter, indent + 1, onSelect);
                }
            }
        }

        private void DoTitles(SplitterState splitter)
        {
            SplitterGUILayout.BeginHorizontalSplit(splitter, new GUILayoutOption[0]);
            for (int i = 0; i < this.titleNames.Length; i++)
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxHeight(16f) };
                GUILayout.Toggle(false, this.titleNames[i], EditorStyles.toolbarButton, options);
            }
            SplitterGUILayout.EndHorizontalSplit();
        }

        private static void EventHeapShotReceived(string name)
        {
            Debug.Log("Received " + name);
            if (onReceivedHeapshot != null)
            {
                onReceivedHeapshot(name);
            }
        }

        private int GetItemCount(List<HeapshotUIObject> objects)
        {
            int num = 0;
            foreach (HeapshotUIObject obj2 in objects)
            {
                num++;
                if (obj2.IsExpanded)
                {
                    num += this.GetItemCount(obj2.Children);
                }
            }
            return num;
        }

        private static void Init()
        {
            EditorWindow.GetWindow(typeof(HeapshotWindow)).titleContent = EditorGUIUtility.TextContent("Mono heapshot");
        }

        private void OnDisable()
        {
            onReceivedHeapshot = null;
        }

        private void OnEnable()
        {
            onReceivedHeapshot = new DelegateReceivedHeapshot(this.OnReceivedHeapshot);
        }

        private void OnFocus()
        {
            this.SearchForHeapShots();
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(0f, 0f, base.position.width, 20f), "Heapshots are located here: " + Path.Combine(Application.dataPath, "Heapshots"));
            GUI.Label(new Rect(0f, 20f, base.position.width, 20f), "Currently opened: " + this.lastOpenedHeapshotFile);
            GUI.Label(new Rect(100f, 40f, base.position.width, 20f), "Profiling: " + this.lastOpenedProfiler);
            this.DoActiveProfilerButton(new Rect(0f, 40f, 100f, 30f));
            if (GUI.Button(new Rect(0f, 70f, 200f, 20f), "CaptureHeapShot", EditorStyles.toolbarDropDown))
            {
                ProfilerDriver.CaptureHeapshot();
            }
            GUI.changed = false;
            this.selectedHeapshot = EditorGUI.Popup(new Rect(250f, 70f, 500f, 30f), "Click to open -->", this.selectedHeapshot, this.heapshotFiles.ToArray());
            if (GUI.changed && (this.heapshotFiles[this.selectedHeapshot].Length > 0))
            {
                this.OpenHeapshot(this.heapshotFiles[this.selectedHeapshot] + ".heapshot");
            }
            GUILayout.BeginArea(new Rect(0f, 90f, base.position.width, 60f));
            SplitterGUILayout.BeginHorizontalSplit(this.viewSplit, new GUILayoutOption[0]);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            string[] strArray = new string[] { "Roots", "All Objects" };
            for (int i = 0; i < strArray.Length; i++)
            {
                GUILayoutOption[] optionArray1 = new GUILayoutOption[] { GUILayout.MaxHeight(16f) };
                if (GUILayout.Toggle(this.currentTab == i, strArray[i], EditorStyles.toolbarButton, optionArray1))
                {
                    this.currentTab = i;
                }
            }
            GUILayout.EndHorizontal();
            this.DoTitles(this.titleSplit1);
            GUILayout.EndVertical();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxHeight(16f) };
            GUILayout.Label("Back trace references", EditorStyles.toolbarButton, options);
            this.DoTitles(this.titleSplit2);
            GUILayout.EndVertical();
            SplitterGUILayout.EndHorizontalSplit();
            GUILayout.EndArea();
            this.guiRect = new Rect(0f, 130f, (float) this.viewSplit.realSizes[0], 16f);
            float height = this.GetItemCount(this.hsAllObjects) * 16f;
            Rect position = new Rect(this.guiRect.x, this.guiRect.y, this.guiRect.width, base.position.height - this.guiRect.y);
            this.leftViewScrollPosition = GUI.BeginScrollView(position, this.leftViewScrollPosition, new Rect(0f, 0f, position.width - 20f, height));
            this.itemIndex = 0;
            this.guiRect.y = 0f;
            switch (this.currentTab)
            {
                case 0:
                    this.DoHeapshotObjects(this.hsRoots, this.titleSplit1, 0, new OnSelect(this.OnSelectObject));
                    break;

                case 1:
                    this.DoHeapshotObjects(this.hsAllObjects, this.titleSplit1, 0, new OnSelect(this.OnSelectObject));
                    break;
            }
            GUI.EndScrollView();
            this.guiRect = new Rect((float) this.viewSplit.realSizes[0], 130f, (float) this.viewSplit.realSizes[1], 16f);
            float num3 = this.GetItemCount(this.hsBackTraceObjects) * 16f;
            position = new Rect(this.guiRect.x, this.guiRect.y, this.guiRect.width, base.position.height - this.guiRect.y);
            this.rightViewScrollPosition = GUI.BeginScrollView(position, this.rightViewScrollPosition, new Rect(0f, 0f, position.width - 20f, num3));
            if (this.hsBackTraceObjects.Count > 0)
            {
                this.guiRect.y = 0f;
                this.itemIndex = 0;
                this.DoHeapshotObjects(this.hsBackTraceObjects, this.titleSplit2, 0, null);
            }
            GUI.EndScrollView();
        }

        private void OnReceivedHeapshot(string name)
        {
            this.SearchForHeapShots();
            this.OpenHeapshot(name);
        }

        private void OnSelectObject(HeapshotUIObject o)
        {
            this.hsBackTraceObjects.Clear();
            this.hsBackTraceObjects.Add(new HeapshotUIObject(o.Name, o.ObjectInfo, true));
        }

        private void OpenHeapshot(string fileName)
        {
            this.heapshotReader = new HeapshotReader();
            string str = this.HeapshotPath + "/" + fileName;
            if (this.heapshotReader.Open(str))
            {
                this.lastOpenedHeapshotFile = fileName;
                this.RefreshHeapshotUIObjects();
            }
            else
            {
                Debug.LogError("Failed to read " + str);
            }
        }

        private void RefreshHeapshotUIObjects()
        {
            this.hsRoots.Clear();
            this.hsAllObjects.Clear();
            foreach (HeapshotReader.ReferenceInfo info in this.heapshotReader.Roots)
            {
                string name = info.fieldInfo.name;
                this.hsRoots.Add(new HeapshotUIObject(name, info.referencedObject, false));
            }
            SortedDictionary<string, List<HeapshotReader.ObjectInfo>> dictionary = new SortedDictionary<string, List<HeapshotReader.ObjectInfo>>();
            foreach (HeapshotReader.ObjectInfo info2 in this.heapshotReader.Objects)
            {
                if (info2.type == HeapshotReader.ObjectType.Managed)
                {
                    string key = info2.typeInfo.name;
                    if (!dictionary.ContainsKey(key))
                    {
                        dictionary.Add(key, new List<HeapshotReader.ObjectInfo>());
                    }
                    dictionary[key].Add(info2);
                }
            }
            foreach (KeyValuePair<string, List<HeapshotReader.ObjectInfo>> pair in dictionary)
            {
                HeapshotReader.ObjectInfo refObject = new HeapshotReader.ObjectInfo();
                HeapshotReader.FieldInfo field = new HeapshotReader.FieldInfo("(Unknown)");
                foreach (HeapshotReader.ObjectInfo info5 in pair.Value)
                {
                    refObject.references.Add(new HeapshotReader.ReferenceInfo(info5, field));
                }
                HeapshotUIObject item = new HeapshotUIObject(pair.Key + " x " + pair.Value.Count, refObject, false) {
                    IsDummyObject = true
                };
                this.hsAllObjects.Add(item);
            }
        }

        private void SearchForHeapShots()
        {
            this.heapshotFiles.Clear();
            if (Directory.Exists(this.HeapshotPath))
            {
                string[] files = Directory.GetFiles(this.HeapshotPath, "*.heapshot");
                this.selectedHeapshot = -1;
                foreach (string str in files)
                {
                    string item = str.Substring(str.LastIndexOf(@"\") + 1);
                    item = item.Substring(0, item.IndexOf(".heapshot"));
                    this.heapshotFiles.Add(item);
                }
                if (this.heapshotFiles.Count > 0)
                {
                    this.selectedHeapshot = this.heapshotFiles.Count - 1;
                }
            }
        }

        private void SelectProfilerClick(object userData, string[] options, int selected)
        {
            int guid = this.connectionGuids[selected];
            this.lastOpenedProfiler = ProfilerDriver.GetConnectionIdentifier(guid);
            ProfilerDriver.connectedProfiler = guid;
        }

        private string HeapshotPath
        {
            get
            {
                return (Application.dataPath + "/../Heapshots");
            }
        }

        private static UIStyles Styles
        {
            get
            {
                if (ms_Styles == null)
                {
                }
                return (ms_Styles = new UIStyles());
            }
        }

        private delegate void DelegateReceivedHeapshot(string fileName);

        public class HeapshotUIObject
        {
            private List<HeapshotWindow.HeapshotUIObject> children = new List<HeapshotWindow.HeapshotUIObject>();
            private bool inverseReference;
            private bool isDummyObject;
            private string name;
            private UnityEditor.HeapshotReader.ObjectInfo obj;

            public HeapshotUIObject(string name, UnityEditor.HeapshotReader.ObjectInfo refObject, bool inverseReference)
            {
                this.name = name;
                this.obj = refObject;
                this.inverseReference = inverseReference;
            }

            public void Collapse()
            {
                if (this.IsExpanded)
                {
                    this.children.Clear();
                }
            }

            public void Expand()
            {
                if (!this.IsExpanded && this.HasChildren)
                {
                    if (this.inverseReference)
                    {
                        foreach (HeapshotReader.BackReferenceInfo info in this.obj.inverseReferences)
                        {
                            this.children.Add(new HeapshotWindow.HeapshotUIObject(info.fieldInfo.name, info.parentObject, true));
                        }
                    }
                    else
                    {
                        foreach (HeapshotReader.ReferenceInfo info2 in this.obj.references)
                        {
                            this.children.Add(new HeapshotWindow.HeapshotUIObject(info2.fieldInfo.name, info2.referencedObject, false));
                        }
                    }
                }
            }

            public List<HeapshotWindow.HeapshotUIObject> Children
            {
                get
                {
                    if (this.HasChildren && this.IsExpanded)
                    {
                        return this.children;
                    }
                    return null;
                }
            }

            public uint Code
            {
                get
                {
                    return this.obj.code;
                }
            }

            public bool HasChildren
            {
                get
                {
                    return (!this.inverseReference ? (this.obj.references.Count > 0) : (this.obj.inverseReferences.Count > 0));
                }
            }

            public int InverseReferenceCount
            {
                get
                {
                    return (!this.inverseReference ? this.obj.inverseReferences.Count : this.obj.references.Count);
                }
            }

            public bool IsDummyObject
            {
                get
                {
                    return this.isDummyObject;
                }
                set
                {
                    this.isDummyObject = value;
                }
            }

            public bool IsExpanded
            {
                get
                {
                    return (this.HasChildren && (this.children.Count > 0));
                }
            }

            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            public UnityEditor.HeapshotReader.ObjectInfo ObjectInfo
            {
                get
                {
                    return this.obj;
                }
            }

            public int ReferenceCount
            {
                get
                {
                    return (!this.inverseReference ? this.obj.references.Count : this.obj.inverseReferences.Count);
                }
            }

            public uint Size
            {
                get
                {
                    return this.obj.size;
                }
            }

            public string TypeName
            {
                get
                {
                    return this.obj.typeInfo.name;
                }
            }
        }

        private delegate void OnSelect(HeapshotWindow.HeapshotUIObject o);

        internal class UIOptions
        {
            public const float foldoutWidth = 14f;
            public const float height = 16f;
            public const float tabWidth = 50f;
        }

        internal class UIStyles
        {
            public GUIStyle background = "OL Box";
            public GUIStyle entryEven = "OL EntryBackEven";
            public GUIStyle entryOdd = "OL EntryBackOdd";
            public GUIStyle foldout = "IN foldout";
            public GUIStyle header = "OL title";
            public GUIStyle numberLabel = "OL Label";
            public GUIStyle rightHeader = "OL title TextRight";
        }
    }
}

