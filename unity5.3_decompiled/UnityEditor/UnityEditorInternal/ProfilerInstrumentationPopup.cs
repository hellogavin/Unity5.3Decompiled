namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class ProfilerInstrumentationPopup : PopupWindowContent
    {
        private const int kAutoInstrumentButtonHeight = 20;
        private const int kAutoInstrumentButtonsHeight = 20;
        private const string kAutoInstrumentSettingKey = "ProfilerAutoInstrumentedAssemblyTypes";
        private PopupList.ListElement m_AllCheckbox;
        private InstrumentedAssemblyTypes m_AutoInstrumentedAssemblyTypes;
        private PopupList m_FunctionsList;
        private InputData m_FunctionsListInputData;
        private bool m_ShowAllCheckbox;
        private bool m_ShowAutoInstrumemtationParams;
        private static GUIContent s_AutoInstrumentScriptsContent = new GUIContent("Auto instrument " + InstrumentedAssemblyTypes.Script.ToString() + " assemblies");
        private static Dictionary<string, int> s_InstrumentableFunctions;
        private static ProfilerInstrumentationPopup s_PendingPopup;

        public ProfilerInstrumentationPopup(Dictionary<string, int> functions, bool showAllCheckbox, bool showAutoInstrumemtationParams)
        {
            this.m_ShowAutoInstrumemtationParams = showAutoInstrumemtationParams;
            this.m_ShowAllCheckbox = showAllCheckbox;
            this.m_AutoInstrumentedAssemblyTypes = (InstrumentedAssemblyTypes) SessionState.GetInt("ProfilerAutoInstrumentedAssemblyTypes", 0);
            this.m_FunctionsListInputData = new InputData();
            this.m_FunctionsListInputData.m_CloseOnSelection = false;
            this.m_FunctionsListInputData.m_AllowCustom = true;
            this.m_FunctionsListInputData.m_MaxCount = 0;
            this.m_FunctionsListInputData.m_EnableAutoCompletion = false;
            this.m_FunctionsListInputData.m_SortAlphabetically = true;
            this.m_FunctionsListInputData.m_OnSelectCallback = new PopupList.OnSelectCallback(this.ProfilerInstrumentationPopupCallback);
            this.SetFunctions(functions);
            this.m_FunctionsList = new PopupList(this.m_FunctionsListInputData);
        }

        public static bool FunctionHasInstrumentationPopup(string funcName)
        {
            return ((s_InstrumentableFunctions != null) && s_InstrumentableFunctions.ContainsKey(funcName));
        }

        public override Vector2 GetWindowSize()
        {
            Vector2 windowSize = this.m_FunctionsList.GetWindowSize();
            windowSize.x = 450f;
            if (this.m_ShowAutoInstrumemtationParams)
            {
                windowSize.y += 20f;
            }
            return windowSize;
        }

        public override void OnClose()
        {
            this.m_FunctionsList.OnClose();
        }

        public override void OnGUI(Rect rect)
        {
            Rect source = new Rect(rect);
            if (this.m_ShowAutoInstrumemtationParams)
            {
                Rect position = new Rect(source) {
                    height = 20f
                };
                InstrumentedAssemblyTypes none = InstrumentedAssemblyTypes.None;
                if (GUI.Toggle(position, (this.m_AutoInstrumentedAssemblyTypes & InstrumentedAssemblyTypes.Script) != InstrumentedAssemblyTypes.None, s_AutoInstrumentScriptsContent))
                {
                    none |= InstrumentedAssemblyTypes.Script;
                }
                if (none != this.m_AutoInstrumentedAssemblyTypes)
                {
                    this.m_AutoInstrumentedAssemblyTypes = none;
                    ProfilerDriver.SetAutoInstrumentedAssemblies(this.m_AutoInstrumentedAssemblyTypes);
                    SessionState.SetInt("ProfilerAutoInstrumentedAssemblyTypes", (int) this.m_AutoInstrumentedAssemblyTypes);
                }
                source.y += 20f;
                source.height -= 20f;
            }
            this.m_FunctionsList.OnGUI(source);
        }

        private void ProfilerInstrumentationPopupCallback(PopupList.ListElement element)
        {
            if (element == this.m_AllCheckbox)
            {
                element.selected = !element.selected;
                foreach (PopupList.ListElement element2 in this.m_FunctionsListInputData.m_ListElements)
                {
                    if (element.selected)
                    {
                        ProfilerDriver.BeginInstrumentFunction(element2.text);
                    }
                    else
                    {
                        ProfilerDriver.EndInstrumentFunction(element2.text);
                    }
                    element2.selected = element.selected;
                }
            }
            else
            {
                element.selected = !element.selected;
                if (element.selected)
                {
                    ProfilerDriver.BeginInstrumentFunction(element.text);
                }
                else
                {
                    ProfilerDriver.EndInstrumentFunction(element.text);
                }
            }
            this.UpdateAllCheckbox();
        }

        private static void SetFunctionNamesFromUnity(bool allFunction, string[] functionNames, int[] isInstrumentedFlags)
        {
            Dictionary<string, int> functions = new Dictionary<string, int>(functionNames.Length);
            for (int i = 0; i < functionNames.Length; i++)
            {
                functions.Add(functionNames[i], isInstrumentedFlags[i]);
            }
            if (allFunction)
            {
                s_InstrumentableFunctions = functions;
            }
            if (s_PendingPopup != null)
            {
                s_PendingPopup.SetFunctions(functions);
                s_PendingPopup = null;
            }
        }

        private void SetFunctions(Dictionary<string, int> functions)
        {
            this.m_FunctionsListInputData.m_ListElements.Clear();
            if (functions == null)
            {
                this.m_FunctionsListInputData.NewOrMatchingElement("Querying instrumentable functions...").enabled = false;
            }
            else if (functions.Count == 0)
            {
                this.m_FunctionsListInputData.NewOrMatchingElement("No instrumentable child functions found").enabled = false;
            }
            else
            {
                this.m_FunctionsListInputData.m_MaxCount = Mathf.Clamp(functions.Count + 1, 0, 30);
                if (this.m_ShowAllCheckbox)
                {
                    this.m_AllCheckbox = new PopupList.ListElement(" All", false, float.MaxValue);
                    this.m_FunctionsListInputData.m_ListElements.Add(this.m_AllCheckbox);
                }
                foreach (KeyValuePair<string, int> pair in functions)
                {
                    PopupList.ListElement item = new PopupList.ListElement(pair.Key, pair.Value != 0);
                    item.ResetScore();
                    this.m_FunctionsListInputData.m_ListElements.Add(item);
                }
                if (this.m_ShowAllCheckbox)
                {
                    this.UpdateAllCheckbox();
                }
            }
        }

        public static void Show(Rect r)
        {
            ProfilerInstrumentationPopup windowContent = new ProfilerInstrumentationPopup(s_InstrumentableFunctions, false, true);
            if (s_InstrumentableFunctions == null)
            {
                s_PendingPopup = windowContent;
                ProfilerDriver.QueryInstrumentableFunctions();
            }
            else
            {
                s_PendingPopup = null;
            }
            PopupWindow.Show(r, windowContent);
        }

        public static void Show(Rect r, string funcName)
        {
            ProfilerInstrumentationPopup windowContent = new ProfilerInstrumentationPopup(null, true, false);
            s_PendingPopup = windowContent;
            ProfilerDriver.QueryFunctionCallees(funcName);
            PopupWindow.Show(r, windowContent);
        }

        public void UpdateAllCheckbox()
        {
            if (this.m_AllCheckbox != null)
            {
                bool flag = false;
                bool flag2 = true;
                foreach (PopupList.ListElement element in this.m_FunctionsListInputData.m_ListElements)
                {
                    if (element != this.m_AllCheckbox)
                    {
                        if (element.selected)
                        {
                            flag = true;
                        }
                        else
                        {
                            flag2 = false;
                        }
                    }
                }
                this.m_AllCheckbox.selected = flag2;
                this.m_AllCheckbox.partiallySelected = flag && !flag2;
            }
        }

        public static void UpdateInstrumentableFunctions()
        {
            ProfilerDriver.QueryInstrumentableFunctions();
        }

        public static bool InstrumentationEnabled
        {
            get
            {
                return false;
            }
        }

        private class InputData : PopupList.InputData
        {
            public override IEnumerable<PopupList.ListElement> BuildQuery(string prefix)
            {
                <BuildQuery>c__AnonStoreyA3 ya = new <BuildQuery>c__AnonStoreyA3 {
                    prefix = prefix
                };
                if (ya.prefix == string.Empty)
                {
                    return base.m_ListElements;
                }
                return base.m_ListElements.Where<PopupList.ListElement>(new Func<PopupList.ListElement, bool>(ya.<>m__1E9));
            }

            [CompilerGenerated]
            private sealed class <BuildQuery>c__AnonStoreyA3
            {
                internal string prefix;

                internal bool <>m__1E9(PopupList.ListElement element)
                {
                    return element.m_Content.text.Contains(this.prefix);
                }
            }
        }
    }
}

