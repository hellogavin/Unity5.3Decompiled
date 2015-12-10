namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor.VersionControl;
    using UnityEngine;

    [CustomEditor(typeof(MonoManager))]
    internal class ScriptExecutionOrderInspector : Editor
    {
        private const int kIntFieldWidth = 50;
        private const int kListElementHeight = 0x15;
        private const int kOrderRangeMax = 0x7d00;
        private const int kOrderRangeMin = -32000;
        private const string kOrderValuesEditorPrefString = "ScriptExecutionOrderShowOrderValues";
        private const int kPreferredSpacing = 100;
        private int[] kRoundingAmounts = new int[] { 0x3e8, 500, 100, 50, 10, 5, 1 };
        private int[] m_AllOrders;
        private MonoScript[] m_AllScripts;
        private List<MonoScript> m_CustomTimeScripts;
        private List<MonoScript> m_DefaultTimeScripts;
        private bool m_DirtyOrders;
        private MonoScript m_Edited;
        private Vector2 m_Scroll = Vector2.zero;
        public static Styles m_Styles;
        private static int s_DropFieldHash = "DropField".GetHashCode();
        private static MonoScript sDummyScript;

        private void AddScriptToCustomOrder(MonoScript script)
        {
            if (IsValidScript(script) && !this.m_CustomTimeScripts.Contains(script))
            {
                int order = this.RoundByAmount(this.GetExecutionOrderAtIndex(this.m_CustomTimeScripts.Count - 1) + 100, 100);
                this.SetExecutionOrder(script, order);
                this.m_CustomTimeScripts.Add(script);
                this.m_DefaultTimeScripts.Remove(script);
            }
        }

        private void Apply()
        {
            List<int> list = new List<int>();
            List<MonoScript> list2 = new List<MonoScript>();
            for (int i = 0; i < this.m_AllScripts.Length; i++)
            {
                if (MonoImporter.GetExecutionOrder(this.m_AllScripts[i]) != this.m_AllOrders[i])
                {
                    list.Add(i);
                    list2.Add(this.m_AllScripts[i]);
                }
            }
            bool success = true;
            if (Provider.enabled)
            {
                Task task = Provider.Checkout(list2.ToArray(), CheckoutMode.Meta);
                task.Wait();
                success = task.success;
            }
            if (success)
            {
                foreach (int num2 in list)
                {
                    MonoImporter.SetExecutionOrder(this.m_AllScripts[num2], this.m_AllOrders[num2]);
                }
                this.PopulateScriptArray();
            }
            else
            {
                Debug.LogError("Could not checkout scrips in version control for changing script execution order");
            }
        }

        private void ApplyRevertGUI()
        {
            EditorGUILayout.Space();
            bool enabled = GUI.enabled;
            GUI.enabled = this.m_DirtyOrders;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Revert", new GUILayoutOption[0]))
            {
                this.Revert();
            }
            if (GUILayout.Button("Apply", new GUILayoutOption[0]))
            {
                this.Apply();
            }
            GUILayout.EndHorizontal();
            GUI.enabled = enabled;
        }

        public void DrawElement(Rect r, object obj, bool dragging)
        {
            MonoScript script = obj as MonoScript;
            if (Event.current.type == EventType.Repaint)
            {
                m_Styles.elementBackground.Draw(r, false, false, false, false);
                m_Styles.draggingHandle.Draw(this.GetDraggingHandleRect(r), false, false, false, false);
            }
            GUI.Label(this.GetButtonLabelRect(r), script.GetClass().FullName);
            int executionOrder = this.GetExecutionOrder(script);
            string s = EditorGUI.DelayedTextFieldInternal(this.GetFieldRect(r), executionOrder.ToString(), "0123456789-", EditorStyles.textField);
            int result = executionOrder;
            if (int.TryParse(s, out result) && (result != executionOrder))
            {
                this.SetExecutionOrder(script, result);
                this.m_Edited = script;
            }
            if (GUI.Button(this.GetAddRemoveButtonRect(r), m_Styles.iconToolbarMinus, m_Styles.removeButton))
            {
                this.SetExecutionOrder(script, 0);
                this.m_Edited = script;
            }
        }

        private Rect GetAddRemoveButtonRect(Rect r)
        {
            Vector2 minusButtonSize = this.GetMinusButtonSize();
            return new Rect((r.xMax - minusButtonSize.x) - 5f, r.y + 1f, minusButtonSize.x, minusButtonSize.y);
        }

        private int GetAverageRoundedAwayFromZero(int a, int b)
        {
            if (((a + b) % 2) == 0)
            {
                return ((a + b) / 2);
            }
            return (((a + b) + Math.Sign((int) (a + b))) / 2);
        }

        private int GetBestPushDirectionForOrderValue(int order)
        {
            int num = (int) Mathf.Sign((float) order);
            if ((order >= -16000) && (order <= 0x3e80))
            {
                return num;
            }
            return -num;
        }

        private Rect GetButtonLabelRect(Rect r)
        {
            return new Rect(r.x + 20f, r.y + 1f, (((r.width - this.GetMinusButtonSize().x) - 10f) - 20f) - 55f, r.height);
        }

        private Rect GetDraggingHandleRect(Rect r)
        {
            return new Rect(r.x + 5f, r.y + 7f, 10f, r.height - 14f);
        }

        private int GetExecutionOrder(MonoScript script)
        {
            int index = Array.IndexOf<MonoScript>(this.m_AllScripts, script);
            if (index >= 0)
            {
                return this.m_AllOrders[index];
            }
            return 0;
        }

        private int GetExecutionOrderAtIndex(int idx)
        {
            return this.GetExecutionOrder(this.m_CustomTimeScripts[idx]);
        }

        private Rect GetFieldRect(Rect r)
        {
            return new Rect(((r.xMax - 50f) - this.GetMinusButtonSize().x) - 10f, r.y + 2f, 50f, r.height - 5f);
        }

        private Vector2 GetMinusButtonSize()
        {
            return m_Styles.removeButton.CalcSize(m_Styles.iconToolbarMinus);
        }

        private static bool IsValidScript(MonoScript script)
        {
            if (script == null)
            {
                return false;
            }
            if (script.GetClass() == null)
            {
                return false;
            }
            bool flag = typeof(MonoBehaviour).IsAssignableFrom(script.GetClass());
            bool flag2 = typeof(ScriptableObject).IsAssignableFrom(script.GetClass());
            if (!flag && !flag2)
            {
                return false;
            }
            if (AssetDatabase.GetAssetPath(script).IndexOf("Assets/") != 0)
            {
                return false;
            }
            return true;
        }

        private void MenuSelection(object userData, string[] options, int selected)
        {
            this.AddScriptToCustomOrder(this.m_DefaultTimeScripts[selected]);
        }

        private static Object MonoScriptValidatorCallback(Object[] references, Type objType, SerializedProperty property)
        {
            foreach (Object obj2 in references)
            {
                MonoScript script = obj2 as MonoScript;
                if ((script != null) && IsValidScript(script))
                {
                    return script;
                }
            }
            return null;
        }

        private void OnDestroy()
        {
            if (this.m_DirtyOrders && EditorUtility.DisplayDialog("Unapplied execution order", "Unapplied script execution order", "Apply", "Revert"))
            {
                this.Apply();
            }
        }

        public void OnEnable()
        {
            if (sDummyScript == null)
            {
                sDummyScript = new MonoScript();
            }
            if ((this.m_AllScripts == null) || !this.m_DirtyOrders)
            {
                this.PopulateScriptArray();
            }
        }

        public override void OnInspectorGUI()
        {
            if (m_Styles == null)
            {
                m_Styles = new Styles();
            }
            if (this.m_Edited != null)
            {
                this.UpdateOrder(this.m_Edited);
                this.m_Edited = null;
            }
            EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins, new GUILayoutOption[0]);
            GUILayout.Label(m_Styles.helpText, EditorStyles.helpBox, new GUILayoutOption[0]);
            EditorGUILayout.Space();
            Rect position = EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
            int id = GUIUtility.GetControlID(s_DropFieldHash, FocusType.Passive, position);
            MonoScript script = EditorGUI.DoDropField(position, id, typeof(MonoScript), new EditorGUI.ObjectFieldValidator(ScriptExecutionOrderInspector.MonoScriptValidatorCallback), false, m_Styles.dropField) as MonoScript;
            if (script != null)
            {
                this.AddScriptToCustomOrder(script);
            }
            EditorGUILayout.BeginVertical(m_Styles.boxBackground, new GUILayoutOption[0]);
            this.m_Scroll = EditorGUILayout.BeginVerticalScrollView(this.m_Scroll, new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            int indexOfChangedItem = DragReorderGUI.DragReorder(GUILayoutUtility.GetRect((float) 10f, (float) (0x15 * this.m_CustomTimeScripts.Count), options), 0x15, this.m_CustomTimeScripts, new DragReorderGUI.DrawElementDelegate(this.DrawElement));
            if (indexOfChangedItem >= 0)
            {
                this.SetExecutionOrderAtIndexAccordingToNeighbors(indexOfChangedItem, 0);
                this.UpdateOrder(this.m_CustomTimeScripts[indexOfChangedItem]);
                this.SetExecutionOrderAtIndexAccordingToNeighbors(indexOfChangedItem, 0);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            GUILayout.BeginHorizontal(m_Styles.toolbar, new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUIContent iconToolbarPlus = m_Styles.iconToolbarPlus;
            Rect rect = GUILayoutUtility.GetRect(iconToolbarPlus, m_Styles.toolbarDropDown);
            if (EditorGUI.ButtonMouseDown(rect, iconToolbarPlus, FocusType.Native, m_Styles.toolbarDropDown))
            {
                this.ShowScriptPopup(rect);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            this.ApplyRevertGUI();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
        }

        private void PopulateScriptArray()
        {
            this.m_AllScripts = MonoImporter.GetAllRuntimeMonoScripts();
            this.m_AllOrders = new int[this.m_AllScripts.Length];
            this.m_CustomTimeScripts = new List<MonoScript>();
            this.m_DefaultTimeScripts = new List<MonoScript>();
            for (int i = 0; i < this.m_AllScripts.Length; i++)
            {
                MonoScript script = this.m_AllScripts[i];
                this.m_AllOrders[i] = MonoImporter.GetExecutionOrder(script);
                if (IsValidScript(script))
                {
                    if (this.GetExecutionOrder(script) == 0)
                    {
                        this.m_DefaultTimeScripts.Add(script);
                    }
                    else
                    {
                        this.m_CustomTimeScripts.Add(script);
                    }
                }
            }
            this.m_CustomTimeScripts.Add(sDummyScript);
            this.m_CustomTimeScripts.Add(sDummyScript);
            this.m_CustomTimeScripts.Sort(new SortMonoScriptExecutionOrder(this));
            this.m_DefaultTimeScripts.Sort(new SortMonoScriptNameOrder());
            this.m_Edited = null;
            this.m_DirtyOrders = false;
        }

        private void PushAwayToAvoidConflicts(int startIndex, int pushDirection)
        {
            for (int i = startIndex; (i >= 0) && (i < this.m_CustomTimeScripts.Count); i += pushDirection)
            {
                if (((this.GetExecutionOrderAtIndex(i) - this.GetExecutionOrderAtIndex(i - pushDirection)) * pushDirection) >= 1)
                {
                    break;
                }
                this.SetExecutionOrderAtIndexAccordingToNeighbors(i, pushDirection);
            }
        }

        private void Revert()
        {
            this.PopulateScriptArray();
        }

        private int RoundBasedOnContext(int val, int lowerBound, int upperBound)
        {
            int num = Mathf.Max(0, (upperBound - lowerBound) / 6);
            lowerBound += num;
            upperBound -= num;
            for (int i = 0; i < this.kRoundingAmounts.Length; i++)
            {
                int num3 = this.RoundByAmount(val, this.kRoundingAmounts[i]);
                if ((num3 > lowerBound) && (num3 < upperBound))
                {
                    return num3;
                }
            }
            return val;
        }

        private int RoundByAmount(int val, int rounding)
        {
            return (Mathf.RoundToInt(((float) val) / ((float) rounding)) * rounding);
        }

        private void SetExecutionOrder(MonoScript script, int order)
        {
            int index = Array.IndexOf<MonoScript>(this.m_AllScripts, script);
            if (index >= 0)
            {
                this.m_AllOrders[index] = Mathf.Clamp(order, -32000, 0x7d00);
                this.m_DirtyOrders = true;
            }
        }

        private void SetExecutionOrderAtIndex(int idx, int order)
        {
            this.SetExecutionOrder(this.m_CustomTimeScripts[idx], order);
        }

        private void SetExecutionOrderAtIndexAccordingToNeighbors(int indexOfChangedItem, int pushDirection)
        {
            if ((indexOfChangedItem >= 0) && (indexOfChangedItem < this.m_CustomTimeScripts.Count))
            {
                if (indexOfChangedItem == 0)
                {
                    this.SetExecutionOrderAtIndex(indexOfChangedItem, this.RoundByAmount(this.GetExecutionOrderAtIndex(indexOfChangedItem + 1) - 100, 100));
                }
                else if (indexOfChangedItem == (this.m_CustomTimeScripts.Count - 1))
                {
                    this.SetExecutionOrderAtIndex(indexOfChangedItem, this.RoundByAmount(this.GetExecutionOrderAtIndex(indexOfChangedItem - 1) + 100, 100));
                }
                else
                {
                    int executionOrderAtIndex = this.GetExecutionOrderAtIndex(indexOfChangedItem - 1);
                    int upperBound = this.GetExecutionOrderAtIndex(indexOfChangedItem + 1);
                    int order = this.RoundBasedOnContext(this.GetAverageRoundedAwayFromZero(executionOrderAtIndex, upperBound), executionOrderAtIndex, upperBound);
                    if (order != 0)
                    {
                        if (pushDirection == 0)
                        {
                            pushDirection = this.GetBestPushDirectionForOrderValue(order);
                        }
                        if (pushDirection > 0)
                        {
                            order = Mathf.Max(order, executionOrderAtIndex + 1);
                        }
                        else
                        {
                            order = Mathf.Min(order, upperBound - 1);
                        }
                    }
                    this.SetExecutionOrderAtIndex(indexOfChangedItem, order);
                }
            }
        }

        private void ShowScriptPopup(Rect r)
        {
            int count = this.m_DefaultTimeScripts.Count;
            string[] options = new string[count];
            bool[] enabled = new bool[count];
            for (int i = 0; i < count; i++)
            {
                options[i] = this.m_DefaultTimeScripts[i].GetClass().FullName;
                enabled[i] = true;
            }
            EditorUtility.DisplayCustomMenu(r, options, enabled, null, new EditorUtility.SelectMenuItemFunction(this.MenuSelection), null);
        }

        private void UpdateOrder(MonoScript changedScript)
        {
            this.m_CustomTimeScripts.Remove(changedScript);
            int executionOrder = this.GetExecutionOrder(changedScript);
            if (executionOrder == 0)
            {
                this.m_DefaultTimeScripts.Add(changedScript);
                this.m_DefaultTimeScripts.Sort(new SortMonoScriptNameOrder());
            }
            else
            {
                int index = -1;
                for (int i = 0; i < this.m_CustomTimeScripts.Count; i++)
                {
                    if (this.GetExecutionOrderAtIndex(i) == executionOrder)
                    {
                        index = i;
                        break;
                    }
                }
                if (index == -1)
                {
                    this.m_CustomTimeScripts.Add(changedScript);
                    this.m_CustomTimeScripts.Sort(new SortMonoScriptExecutionOrder(this));
                }
                else
                {
                    int bestPushDirectionForOrderValue = this.GetBestPushDirectionForOrderValue(executionOrder);
                    if (bestPushDirectionForOrderValue == 1)
                    {
                        this.m_CustomTimeScripts.Insert(index, changedScript);
                        index++;
                    }
                    else
                    {
                        this.m_CustomTimeScripts.Insert(index + 1, changedScript);
                    }
                    this.PushAwayToAvoidConflicts(index, bestPushDirectionForOrderValue);
                }
            }
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }

        private class DragReorderGUI
        {
            private static int s_DragReorderGUIHash = "DragReorderGUI".GetHashCode();
            private static int s_ReorderingDraggedElement;
            private static int[] s_ReorderingGoals;
            private static float[] s_ReorderingPositions;

            public static int DragReorder(Rect position, int elementHeight, List<MonoScript> elements, DrawElementDelegate drawElementDelegate)
            {
                Rect rect2;
                int controlID = GUIUtility.GetControlID(s_DragReorderGUIHash, FocusType.Passive);
                Rect r = position;
                r.height = elementHeight;
                int index = 0;
                if ((GUIUtility.hotControl == controlID) && (Event.current.type == EventType.Repaint))
                {
                    for (int i = 0; i < elements.Count; i++)
                    {
                        if (i != s_ReorderingDraggedElement)
                        {
                            if (IsDefaultTimeElement(elements[i]))
                            {
                                index = i;
                                i++;
                            }
                            else
                            {
                                r.y = position.y + (s_ReorderingPositions[i] * elementHeight);
                                drawElementDelegate(r, elements[i], false);
                            }
                        }
                    }
                    rect2 = new Rect(r.x, position.y + (s_ReorderingPositions[index] * elementHeight), r.width, ((s_ReorderingPositions[index + 1] - s_ReorderingPositions[index]) + 1f) * elementHeight);
                }
                else
                {
                    for (int j = 0; j < elements.Count; j++)
                    {
                        r.y = position.y + (j * elementHeight);
                        if (IsDefaultTimeElement(elements[j]))
                        {
                            index = j;
                            j++;
                        }
                        else
                        {
                            drawElementDelegate(r, elements[j], false);
                        }
                    }
                    rect2 = new Rect(r.x, position.y + (index * elementHeight), r.width, (float) (elementHeight * 2));
                }
                GUI.Label(rect2, ScriptExecutionOrderInspector.m_Styles.defaultTimeContent, ScriptExecutionOrderInspector.m_Styles.defaultTime);
                bool flag = rect2.height > (elementHeight * 2.5f);
                if (GUIUtility.hotControl == controlID)
                {
                    if (flag)
                    {
                        GUI.color = new Color(1f, 1f, 1f, 0.5f);
                    }
                    r.y = position.y + (s_ReorderingPositions[s_ReorderingDraggedElement] * elementHeight);
                    drawElementDelegate(r, elements[s_ReorderingDraggedElement], true);
                    GUI.color = Color.white;
                }
                int num5 = -1;
                switch (Event.current.GetTypeForControl(controlID))
                {
                    case EventType.MouseDown:
                        if (position.Contains(Event.current.mousePosition))
                        {
                            GUIUtility.keyboardControl = 0;
                            EditorGUI.EndEditingActiveTextField();
                            s_ReorderingDraggedElement = Mathf.FloorToInt((Event.current.mousePosition.y - position.y) / ((float) elementHeight));
                            if (IsDefaultTimeElement(elements[s_ReorderingDraggedElement]))
                            {
                                return num5;
                            }
                            s_ReorderingPositions = new float[elements.Count];
                            s_ReorderingGoals = new int[elements.Count];
                            for (int k = 0; k < elements.Count; k++)
                            {
                                s_ReorderingGoals[k] = k;
                                s_ReorderingPositions[k] = k;
                            }
                            GUIUtility.hotControl = controlID;
                            Event.current.Use();
                        }
                        return num5;

                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == controlID)
                        {
                            if (s_ReorderingGoals[s_ReorderingDraggedElement] != s_ReorderingDraggedElement)
                            {
                                List<MonoScript> list = new List<MonoScript>(elements);
                                for (int m = 0; m < elements.Count; m++)
                                {
                                    elements[s_ReorderingGoals[m]] = list[m];
                                }
                                num5 = s_ReorderingGoals[s_ReorderingDraggedElement];
                            }
                            s_ReorderingGoals = null;
                            s_ReorderingPositions = null;
                            s_ReorderingDraggedElement = -1;
                            GUIUtility.hotControl = 0;
                            Event.current.Use();
                            return num5;
                        }
                        return num5;

                    case EventType.MouseMove:
                    case EventType.KeyDown:
                    case EventType.KeyUp:
                    case EventType.ScrollWheel:
                        return num5;

                    case EventType.MouseDrag:
                        if (GUIUtility.hotControl == controlID)
                        {
                            s_ReorderingPositions[s_ReorderingDraggedElement] = ((Event.current.mousePosition.y - position.y) / ((float) elementHeight)) - 0.5f;
                            s_ReorderingPositions[s_ReorderingDraggedElement] = Mathf.Clamp(s_ReorderingPositions[s_ReorderingDraggedElement], 0f, (float) (elements.Count - 1));
                            int num7 = Mathf.RoundToInt(s_ReorderingPositions[s_ReorderingDraggedElement]);
                            if (num7 != s_ReorderingGoals[s_ReorderingDraggedElement])
                            {
                                for (int n = 0; n < elements.Count; n++)
                                {
                                    s_ReorderingGoals[n] = n;
                                }
                                int num9 = (num7 <= s_ReorderingDraggedElement) ? 1 : -1;
                                for (int num10 = s_ReorderingDraggedElement; num10 != num7; num10 -= num9)
                                {
                                    s_ReorderingGoals[num10 - num9] = num10;
                                }
                                s_ReorderingGoals[s_ReorderingDraggedElement] = num7;
                            }
                            Event.current.Use();
                            return num5;
                        }
                        return num5;

                    case EventType.Repaint:
                        if (GUIUtility.hotControl == controlID)
                        {
                            for (int num12 = 0; num12 < elements.Count; num12++)
                            {
                                if (num12 != s_ReorderingDraggedElement)
                                {
                                    s_ReorderingPositions[num12] = Mathf.MoveTowards(s_ReorderingPositions[num12], (float) s_ReorderingGoals[num12], 0.075f);
                                }
                            }
                            GUIView.current.Repaint();
                        }
                        return num5;
                }
                return num5;
            }

            private static bool IsDefaultTimeElement(MonoScript element)
            {
                return (element.name == string.Empty);
            }

            public delegate void DrawElementDelegate(Rect r, object obj, bool dragging);
        }

        public class SortMonoScriptExecutionOrder : IComparer<MonoScript>
        {
            private ScriptExecutionOrderInspector inspector;

            public SortMonoScriptExecutionOrder(ScriptExecutionOrderInspector inspector)
            {
                this.inspector = inspector;
            }

            public int Compare(MonoScript x, MonoScript y)
            {
                if ((x == null) || (y == null))
                {
                    return -1;
                }
                int executionOrder = this.inspector.GetExecutionOrder(x);
                int num2 = this.inspector.GetExecutionOrder(y);
                if (executionOrder == num2)
                {
                    return x.name.CompareTo(y.name);
                }
                return executionOrder.CompareTo(num2);
            }
        }

        public class SortMonoScriptNameOrder : IComparer<MonoScript>
        {
            public int Compare(MonoScript x, MonoScript y)
            {
                if ((x != null) && (y != null))
                {
                    return x.name.CompareTo(y.name);
                }
                return -1;
            }
        }

        public class Styles
        {
            public GUIStyle boxBackground = "TE NodeBackground";
            public GUIStyle defaultTime = new GUIStyle(EditorStyles.inspectorBig);
            public GUIContent defaultTimeContent = EditorGUIUtility.TextContent("Default Time|All scripts not in the custom order are executed at the default time.");
            public GUIStyle draggingHandle = "WindowBottomResize";
            public GUIStyle dropField = new GUIStyle(EditorStyles.objectFieldThumb);
            public GUIStyle elementBackground = new GUIStyle("OL Box");
            public GUIContent helpText = EditorGUIUtility.TextContent("Add scripts to the custom order and drag them to reorder.\n\nScripts in the custom order can execute before or after the default time and are executed from top to bottom. All other scripts execute at the default time in the order they are loaded.\n\n(Changing the order of a script may modify the meta data for more than one script.)");
            public GUIContent iconToolbarMinus = EditorGUIUtility.IconContent("Toolbar Minus", "Remove script from custom order");
            public GUIContent iconToolbarPlus = EditorGUIUtility.IconContent("Toolbar Plus", "Add script to custom order");
            public GUIStyle removeButton = "InvisibleButton";
            public GUIStyle toolbar = "TE Toolbar";
            public GUIStyle toolbarDropDown = "TE ToolbarDropDown";

            public Styles()
            {
                this.boxBackground.margin = new RectOffset();
                this.boxBackground.padding = new RectOffset(1, 1, 1, 0);
                this.elementBackground.overflow = new RectOffset(1, 1, 1, 0);
                this.defaultTime.alignment = TextAnchor.MiddleCenter;
                this.defaultTime.overflow = new RectOffset(0, 0, 1, 0);
                this.dropField.overflow = new RectOffset(2, 2, 2, 2);
                this.dropField.normal.background = null;
                this.dropField.hover.background = null;
                this.dropField.active.background = null;
                this.dropField.focused.background = null;
            }
        }
    }
}

