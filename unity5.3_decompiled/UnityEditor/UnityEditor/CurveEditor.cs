namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable]
    internal class CurveEditor : TimeArea, CurveUpdater
    {
        [CompilerGenerated]
        private static Func<CurveWrapper, int> <>f__am$cache26;
        [CompilerGenerated]
        private static Func<CurveWrapper, int> <>f__am$cache27;
        [CompilerGenerated]
        private static Func<CurveSelection, float> <>f__am$cache29;
        [CompilerGenerated]
        private static Func<CurveSelection, float> <>f__am$cache2A;
        [CompilerGenerated]
        private static Func<CurveSelection, float> <>f__am$cache2B;
        [CompilerGenerated]
        private static Func<CurveSelection, float> <>f__am$cache2C;
        [CompilerGenerated]
        private static Func<CurveSelection, float> <>f__am$cache2D;
        [CompilerGenerated]
        private static Func<CurveSelection, float> <>f__am$cache2E;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$mapC;
        public CallbackFunction curvesUpdated;
        private string focusedPointField;
        public float invSnap;
        private const float kCurveTimeEpsilon = 1E-05f;
        private const float kExactPickDistSqr = 16f;
        private const float kMaxPickDistSqr = 100f;
        private const string kPointTimeFieldName = "pointTimeField";
        private const string kPointValueFieldName = "pointValueField";
        private CurveSelection lastSelected;
        private CurveWrapper[] m_AnimationCurves;
        private string m_AxisLabelFormat;
        private AxisLock m_AxisLock;
        private Bounds m_Bounds;
        private List<SavedCurve> m_CurveBackups;
        internal Bounds m_DefaultBounds;
        private Vector2 m_DraggedCoord;
        private CurveWrapper[] m_DraggingCurveOrRegion;
        private CurveWrapper m_DraggingKey;
        private List<int> m_DrawOrder;
        private CurveMenuManager m_MenuManager;
        private Vector2 m_MoveCoord;
        internal IPlayHead m_PlayHead;
        private Vector2 m_PreviousDrawPointCenter;
        private CurveSelection m_SelectedTangentPoint;
        private List<CurveSelection> m_Selection;
        private Color m_TangentColor;
        internal Styles ms_Styles;
        private Vector2 pointEditingFieldPosition;
        private List<CurveSelection> preCurveDragSelection;
        private Vector2 s_EndMouseDragPosition;
        private PickMode s_PickMode;
        private List<CurveSelection> s_SelectionBackup;
        private static int s_SelectKeyHash = "SelectKeys".GetHashCode();
        private float s_StartClickedTime;
        private Vector2 s_StartKeyDragPosition;
        private Vector2 s_StartMouseDragPosition;
        private static int s_TangentControlIDHash = "s_TangentControlIDHash".GetHashCode();
        private bool s_TimeRangeSelectionActive;
        private float s_TimeRangeSelectionEnd;
        private float s_TimeRangeSelectionStart;
        private bool timeWasEdited;
        private bool valueWasEdited;

        public CurveEditor(Rect rect, CurveWrapper[] curves, bool minimalGUI) : base(minimalGUI)
        {
            this.m_DrawOrder = new List<int>();
            this.m_DefaultBounds = new Bounds(new Vector3(0.5f, 0.5f, 0f), new Vector3(1f, 1f, 0f));
            this.m_TangentColor = new Color(1f, 1f, 1f, 0.5f);
            this.m_Selection = new List<CurveSelection>();
            this.m_Bounds = new Bounds(Vector3.zero, Vector3.zero);
            this.m_AxisLabelFormat = "n1";
            base.rect = rect;
            this.animationCurves = curves;
            float[] tickModulos = new float[] { 
                1E-07f, 5E-07f, 1E-06f, 5E-06f, 1E-05f, 5E-05f, 0.0001f, 0.0005f, 0.001f, 0.005f, 0.01f, 0.05f, 0.1f, 0.5f, 1f, 5f, 
                10f, 50f, 100f, 500f, 1000f, 5000f, 10000f, 50000f, 100000f, 500000f, 1000000f, 5000000f, 1E+07f
             };
            base.hTicks = new TickHandler();
            base.hTicks.SetTickModulos(tickModulos);
            base.vTicks = new TickHandler();
            base.vTicks.SetTickModulos(tickModulos);
            base.margin = 40f;
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
        }

        private CurveSelection AddKeyAtPosition(CurveWrapper cw, Vector2 position)
        {
            float num;
            position.x = this.SnapTime(position.x);
            if (this.invSnap != 0f)
            {
                num = 0.5f / this.invSnap;
            }
            else
            {
                num = 0.0001f;
            }
            if (CurveUtility.HaveKeysInRange(cw.curve, position.x - num, position.x + num))
            {
                return null;
            }
            float inTangent = 0f;
            Keyframe key = new Keyframe(position.x, this.SnapValue(position.y), inTangent, inTangent);
            return this.AddKeyframeAndSelect(key, cw);
        }

        private CurveSelection AddKeyAtTime(CurveWrapper cw, float time)
        {
            float num;
            time = this.SnapTime(time);
            if (this.invSnap != 0f)
            {
                num = 0.5f / this.invSnap;
            }
            else
            {
                num = 0.0001f;
            }
            if (CurveUtility.HaveKeysInRange(cw.curve, time - num, time + num))
            {
                return null;
            }
            float inTangent = cw.renderer.EvaluateCurveDeltaSlow(time);
            float num3 = this.ClampVerticalValue(this.SnapValue(cw.renderer.EvaluateCurveSlow(time)), cw.id);
            Keyframe key = new Keyframe(time, num3, inTangent, inTangent);
            return this.AddKeyframeAndSelect(key, cw);
        }

        private CurveSelection AddKeyframeAndSelect(Keyframe key, CurveWrapper cw)
        {
            int keyIndex = cw.curve.AddKey(key);
            CurveUtility.SetKeyModeFromContext(cw.curve, keyIndex);
            CurveUtility.UpdateTangentsFromModeSurrounding(cw.curve, keyIndex);
            CurveSelection selection = new CurveSelection(cw.id, this, keyIndex);
            cw.selected = CurveWrapper.SelectionMode.Selected;
            cw.changed = true;
            this.activeTime = key.time;
            return selection;
        }

        internal void AddSelection(CurveSelection curveSelection)
        {
            this.m_Selection.Add(curveSelection);
            this.lastSelected = curveSelection;
        }

        protected override void ApplySettings()
        {
            base.ApplySettings();
            this.RecalculateBounds();
        }

        public void BeginTimeRangeSelection(float time, bool addToSelection)
        {
            if (this.s_TimeRangeSelectionActive)
            {
                Debug.LogError("BeginTimeRangeSelection can only be called once");
            }
            else
            {
                this.s_TimeRangeSelectionActive = true;
                this.s_TimeRangeSelectionStart = this.s_TimeRangeSelectionEnd = time;
                if (addToSelection)
                {
                    this.s_SelectionBackup = new List<CurveSelection>(this.selectedCurves);
                }
                else
                {
                    this.s_SelectionBackup = new List<CurveSelection>();
                }
            }
        }

        public void CancelTimeRangeSelection()
        {
            if (!this.s_TimeRangeSelectionActive)
            {
                Debug.LogError("CancelTimeRangeSelection can only be called after BeginTimeRangeSelection");
            }
            else
            {
                this.selectedCurves = this.s_SelectionBackup;
                this.s_TimeRangeSelectionActive = false;
            }
        }

        private float ClampVerticalValue(float value, int curveID)
        {
            value = Mathf.Clamp(value, base.vRangeMin, base.vRangeMax);
            CurveWrapper curveFromID = this.GetCurveFromID(curveID);
            if (curveFromID != null)
            {
                value = Mathf.Clamp(value, curveFromID.vRangeMin, curveFromID.vRangeMax);
            }
            return value;
        }

        internal void ClearSelection()
        {
            this.m_Selection.Clear();
            this.lastSelected = null;
        }

        private void CreateKeyFromClick(object obj)
        {
            List<int> curveIds = this.CreateKeyFromClick((Vector2) obj);
            this.UpdateCurves(curveIds, "Add Key");
        }

        private List<int> CreateKeyFromClick(Vector2 position)
        {
            Vector2 vector;
            List<int> list = new List<int>();
            int index = this.OnlyOneEditableCurve();
            if (index >= 0)
            {
                float x = position.x;
                CurveWrapper wrapper = this.m_AnimationCurves[index];
                if (((wrapper.curve.keys.Length == 0) || (x < wrapper.curve.keys[0].time)) || (x > wrapper.curve.keys[wrapper.curve.keys.Length - 1].time))
                {
                    this.CreateKeyFromClick(index, position);
                    list.Add(wrapper.id);
                    return list;
                }
            }
            int curveAtPosition = this.GetCurveAtPosition(position, out vector);
            this.CreateKeyFromClick(curveAtPosition, vector.x);
            if (curveAtPosition >= 0)
            {
                list.Add(this.m_AnimationCurves[curveAtPosition].id);
            }
            return list;
        }

        private void CreateKeyFromClick(int curveIndex, float time)
        {
            time = Mathf.Clamp(time, base.settings.hRangeMin, base.settings.hRangeMax);
            if (curveIndex >= 0)
            {
                CurveSelection curveSelection = null;
                CurveWrapper cw = this.m_AnimationCurves[curveIndex];
                if (cw.groupId == -1)
                {
                    curveSelection = this.AddKeyAtTime(cw, time);
                }
                else
                {
                    foreach (CurveWrapper wrapper2 in this.m_AnimationCurves)
                    {
                        if (wrapper2.groupId == cw.groupId)
                        {
                            CurveSelection selection2 = this.AddKeyAtTime(wrapper2, time);
                            if (wrapper2.id == cw.id)
                            {
                                curveSelection = selection2;
                            }
                        }
                    }
                }
                if (curveSelection != null)
                {
                    this.ClearSelection();
                    this.AddSelection(curveSelection);
                    this.RecalcSecondarySelection();
                }
                else
                {
                    this.SelectNone();
                }
            }
        }

        private void CreateKeyFromClick(int curveIndex, Vector2 position)
        {
            position.x = Mathf.Clamp(position.x, base.settings.hRangeMin, base.settings.hRangeMax);
            if (curveIndex >= 0)
            {
                CurveSelection curveSelection = null;
                CurveWrapper cw = this.m_AnimationCurves[curveIndex];
                if (cw.groupId == -1)
                {
                    curveSelection = this.AddKeyAtPosition(cw, position);
                }
                else
                {
                    foreach (CurveWrapper wrapper2 in this.m_AnimationCurves)
                    {
                        if (wrapper2.groupId == cw.groupId)
                        {
                            if (wrapper2.id == cw.id)
                            {
                                curveSelection = this.AddKeyAtPosition(wrapper2, position);
                            }
                            else
                            {
                                this.AddKeyAtTime(wrapper2, position.x);
                            }
                        }
                    }
                }
                if (curveSelection != null)
                {
                    this.ClearSelection();
                    this.AddSelection(curveSelection);
                    this.RecalcSecondarySelection();
                }
                else
                {
                    this.SelectNone();
                }
            }
        }

        private static List<Vector3> CreateRegion(CurveWrapper minCurve, CurveWrapper maxCurve, float deltaTime)
        {
            List<Vector3> list = new List<Vector3>();
            List<float> list2 = new List<float>();
            float item = deltaTime;
            while (item <= 1f)
            {
                list2.Add(item);
                item += deltaTime;
            }
            if (item != 1f)
            {
                list2.Add(1f);
            }
            foreach (Keyframe keyframe in maxCurve.curve.keys)
            {
                if ((keyframe.time > 0f) && (keyframe.time < 1f))
                {
                    list2.Add(keyframe.time - 0.0001f);
                    list2.Add(keyframe.time);
                    list2.Add(keyframe.time + 0.0001f);
                }
            }
            foreach (Keyframe keyframe2 in minCurve.curve.keys)
            {
                if ((keyframe2.time > 0f) && (keyframe2.time < 1f))
                {
                    list2.Add(keyframe2.time - 0.0001f);
                    list2.Add(keyframe2.time);
                    list2.Add(keyframe2.time + 0.0001f);
                }
            }
            list2.Sort();
            Vector3 vector = new Vector3(0f, maxCurve.renderer.EvaluateCurveSlow(0f), 0f);
            Vector3 vector2 = new Vector3(0f, minCurve.renderer.EvaluateCurveSlow(0f), 0f);
            for (int i = 0; i < list2.Count; i++)
            {
                float x = list2[i];
                Vector3 vector3 = new Vector3(x, maxCurve.renderer.EvaluateCurveSlow(x), 0f);
                Vector3 vector4 = new Vector3(x, minCurve.renderer.EvaluateCurveSlow(x), 0f);
                if ((vector.y >= vector2.y) && (vector3.y >= vector4.y))
                {
                    list.Add(vector);
                    list.Add(vector4);
                    list.Add(vector2);
                    list.Add(vector);
                    list.Add(vector3);
                    list.Add(vector4);
                }
                else if ((vector.y <= vector2.y) && (vector3.y <= vector4.y))
                {
                    list.Add(vector2);
                    list.Add(vector3);
                    list.Add(vector);
                    list.Add(vector2);
                    list.Add(vector4);
                    list.Add(vector3);
                }
                else
                {
                    Vector2 zero = Vector2.zero;
                    if (Mathf.LineIntersection(vector, vector3, vector2, vector4, ref zero))
                    {
                        list.Add(vector);
                        list.Add((Vector3) zero);
                        list.Add(vector2);
                        list.Add(vector3);
                        list.Add((Vector3) zero);
                        list.Add(vector4);
                    }
                    else
                    {
                        Debug.Log("Error: No intersection found! There should be one...");
                    }
                }
                vector = vector3;
                vector2 = vector4;
            }
            return list;
        }

        public void CurveGUI()
        {
            this.InitStyles();
            GUI.BeginGroup(base.drawRect);
            this.Init();
            GUIUtility.GetControlID(s_SelectKeyHash, FocusType.Passive);
            Color white = Color.white;
            GUI.backgroundColor = white;
            GUI.contentColor = white;
            Color color = GUI.color;
            Event current = Event.current;
            if (current.type != EventType.Repaint)
            {
                this.EditSelectedPoints();
            }
            switch (current.type)
            {
                case EventType.KeyDown:
                    if (((current.keyCode == KeyCode.Backspace) || (current.keyCode == KeyCode.Delete)) && this.hasSelection)
                    {
                        this.DeleteSelectedPoints();
                        current.Use();
                    }
                    break;

                case EventType.Repaint:
                    this.DrawCurves(this.animationCurves);
                    break;

                case EventType.ValidateCommand:
                case EventType.ExecuteCommand:
                {
                    bool flag = current.type == EventType.ExecuteCommand;
                    string commandName = current.commandName;
                    if (commandName != null)
                    {
                        int num;
                        if (<>f__switch$mapC == null)
                        {
                            Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
                            dictionary.Add("Delete", 0);
                            dictionary.Add("FrameSelected", 1);
                            dictionary.Add("SelectAll", 2);
                            <>f__switch$mapC = dictionary;
                        }
                        if (<>f__switch$mapC.TryGetValue(commandName, out num))
                        {
                            switch (num)
                            {
                                case 0:
                                    if (this.hasSelection)
                                    {
                                        if (flag)
                                        {
                                            this.DeleteSelectedPoints();
                                        }
                                        current.Use();
                                    }
                                    break;

                                case 1:
                                    if (flag)
                                    {
                                        this.FrameSelected(true, true);
                                    }
                                    current.Use();
                                    break;

                                case 2:
                                    if (flag)
                                    {
                                        this.SelectAll();
                                    }
                                    current.Use();
                                    break;
                            }
                        }
                    }
                    break;
                }
                case EventType.ContextClick:
                {
                    CurveSelection curveSelection = this.FindNearest();
                    if (curveSelection != null)
                    {
                        List<KeyIdentifier> userData = new List<KeyIdentifier>();
                        bool flag2 = false;
                        foreach (CurveSelection selection2 in this.selectedCurves)
                        {
                            userData.Add(new KeyIdentifier(selection2.curveWrapper.renderer, selection2.curveID, selection2.key));
                            if ((selection2.curveID == curveSelection.curveID) && (selection2.key == curveSelection.key))
                            {
                                flag2 = true;
                            }
                        }
                        if (!flag2)
                        {
                            userData.Clear();
                            userData.Add(new KeyIdentifier(curveSelection.curveWrapper.renderer, curveSelection.curveID, curveSelection.key));
                            this.ClearSelection();
                            this.AddSelection(curveSelection);
                        }
                        this.m_MenuManager = new CurveMenuManager(this);
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent((userData.Count <= 1) ? "Delete Key" : "Delete Keys"), false, new GenericMenu.MenuFunction2(this.DeleteKeys), userData);
                        menu.AddItem(new GUIContent((userData.Count <= 1) ? "Edit Key..." : "Edit Keys..."), false, new GenericMenu.MenuFunction2(this.StartEditingSelectedPointsContext), base.mousePositionInDrawing);
                        menu.AddSeparator(string.Empty);
                        this.m_MenuManager.AddTangentMenuItems(menu, userData);
                        menu.ShowAsContext();
                        Event.current.Use();
                    }
                    break;
                }
            }
            if (current.type == EventType.Repaint)
            {
                this.EditSelectedPoints();
            }
            EditorGUI.BeginChangeCheck();
            GUI.color = color;
            this.DragTangents();
            this.EditAxisLabels();
            this.SelectPoints();
            if (EditorGUI.EndChangeCheck())
            {
                this.RecalcSecondarySelection();
                this.RecalcCurveSelection();
            }
            EditorGUI.BeginChangeCheck();
            Vector2 vector = this.MovePoints();
            if (EditorGUI.EndChangeCheck() && (this.m_DraggingKey != null))
            {
                this.activeTime = vector.x + this.s_StartClickedTime;
                this.m_MoveCoord = vector;
            }
            GUI.color = color;
            GUI.EndGroup();
        }

        private void DeleteKeys(object obj)
        {
            string str;
            List<KeyIdentifier> list = (List<KeyIdentifier>) obj;
            List<int> curveIds = new List<int>();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (base.settings.allowDeleteLastKeyInCurve || (list[i].curve.keys.Length != 1))
                {
                    list[i].curve.RemoveKey(list[i].key);
                    CurveUtility.UpdateTangentsFromMode(list[i].curve);
                    curveIds.Add(list[i].curveId);
                }
            }
            if (list.Count > 1)
            {
                str = "Delete Keys";
            }
            else
            {
                str = "Delete Key";
            }
            this.UpdateCurves(curveIds, str);
            this.SelectNone();
        }

        private void DeleteSelectedPoints()
        {
            for (int i = this.selectedCurves.Count - 1; i >= 0; i--)
            {
                CurveSelection selection = this.selectedCurves[i];
                CurveWrapper curveWrapper = selection.curveWrapper;
                if (base.settings.allowDeleteLastKeyInCurve || (curveWrapper.curve.keys.Length != 1))
                {
                    curveWrapper.curve.RemoveKey(selection.key);
                    CurveUtility.UpdateTangentsFromMode(curveWrapper.curve);
                    curveWrapper.changed = true;
                    GUI.changed = true;
                }
            }
            this.SelectNone();
        }

        private void DragTangents()
        {
            CurveSelection selectedTangentPoint;
            Keyframe keyframe;
            Event current = Event.current;
            int controlID = GUIUtility.GetControlID(s_TangentControlIDHash, FocusType.Passive);
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if ((current.button == 0) && !current.alt)
                    {
                        this.m_SelectedTangentPoint = null;
                        float num2 = 100f;
                        Vector2 mousePosition = Event.current.mousePosition;
                        foreach (CurveSelection selection in this.selectedCurves)
                        {
                            Keyframe key = selection.keyframe;
                            if (CurveUtility.GetKeyTangentMode(key, 0) == TangentMode.Editable)
                            {
                                CurveSelection selection2 = new CurveSelection(selection.curveID, this, selection.key, CurveSelection.SelectionType.InTangent);
                                Vector2 vector5 = base.DrawingToViewTransformPoint(this.GetPosition(selection2)) - mousePosition;
                                float sqrMagnitude = vector5.sqrMagnitude;
                                if (sqrMagnitude <= num2)
                                {
                                    this.m_SelectedTangentPoint = selection2;
                                    num2 = sqrMagnitude;
                                }
                            }
                            if (CurveUtility.GetKeyTangentMode(key, 1) == TangentMode.Editable)
                            {
                                CurveSelection selection3 = new CurveSelection(selection.curveID, this, selection.key, CurveSelection.SelectionType.OutTangent);
                                Vector2 vector6 = base.DrawingToViewTransformPoint(this.GetPosition(selection3)) - mousePosition;
                                float num4 = vector6.sqrMagnitude;
                                if (num4 <= num2)
                                {
                                    this.m_SelectedTangentPoint = selection3;
                                    num2 = num4;
                                }
                            }
                        }
                        if (this.m_SelectedTangentPoint != null)
                        {
                            GUIUtility.hotControl = controlID;
                            current.Use();
                        }
                    }
                    return;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                    }
                    return;

                case EventType.MouseMove:
                    return;

                case EventType.MouseDrag:
                {
                    if (GUIUtility.hotControl != controlID)
                    {
                        return;
                    }
                    Vector2 mousePositionInDrawing = base.mousePositionInDrawing;
                    selectedTangentPoint = this.m_SelectedTangentPoint;
                    keyframe = selectedTangentPoint.keyframe;
                    if (selectedTangentPoint.type != CurveSelection.SelectionType.InTangent)
                    {
                        if (selectedTangentPoint.type == CurveSelection.SelectionType.OutTangent)
                        {
                            Vector2 vector4 = mousePositionInDrawing - new Vector2(keyframe.time, keyframe.value);
                            if (vector4.x > 0.0001f)
                            {
                                keyframe.outTangent = vector4.y / vector4.x;
                            }
                            else
                            {
                                keyframe.outTangent = float.PositiveInfinity;
                            }
                            CurveUtility.SetKeyTangentMode(ref keyframe, 1, TangentMode.Editable);
                            if (!CurveUtility.GetKeyBroken(keyframe))
                            {
                                keyframe.inTangent = keyframe.outTangent;
                                CurveUtility.SetKeyTangentMode(ref keyframe, 0, TangentMode.Editable);
                            }
                        }
                        goto Label_02B2;
                    }
                    Vector2 vector3 = mousePositionInDrawing - new Vector2(keyframe.time, keyframe.value);
                    if (vector3.x >= -0.0001f)
                    {
                        keyframe.inTangent = float.PositiveInfinity;
                        break;
                    }
                    keyframe.inTangent = vector3.y / vector3.x;
                    break;
                }
                default:
                    return;
            }
            CurveUtility.SetKeyTangentMode(ref keyframe, 0, TangentMode.Editable);
            if (!CurveUtility.GetKeyBroken(keyframe))
            {
                keyframe.outTangent = keyframe.inTangent;
                CurveUtility.SetKeyTangentMode(ref keyframe, 1, TangentMode.Editable);
            }
        Label_02B2:
            selectedTangentPoint.key = selectedTangentPoint.curve.MoveKey(selectedTangentPoint.key, keyframe);
            CurveUtility.UpdateTangentsFromModeSurrounding(selectedTangentPoint.curveWrapper.curve, selectedTangentPoint.key);
            selectedTangentPoint.curveWrapper.changed = true;
            GUI.changed = true;
            Event.current.Use();
        }

        private void DrawCurve(CurveWrapper cw, bool hasFocus)
        {
            CurveRenderer renderer = cw.renderer;
            Color a = cw.color;
            if (this.IsDraggingCurve(cw) || (cw.selected == CurveWrapper.SelectionMode.Selected))
            {
                a = Color.Lerp(a, Color.white, 0.3f);
            }
            else if (base.settings.useFocusColors && !hasFocus)
            {
                a = (Color) (a * 0.5f);
                a.a = 0.8f;
            }
            Rect shownArea = base.shownArea;
            renderer.DrawCurve(shownArea.xMin, shownArea.xMax, a, base.drawingToViewMatrix, base.settings.wrapColor);
        }

        private void DrawCurveAndPoints(CurveWrapper cw, List<CurveSelection> selection, bool hasFocus)
        {
            this.DrawCurve(cw, hasFocus);
            this.DrawPointsOnCurve(cw, selection, hasFocus);
        }

        private void DrawCurves(CurveWrapper[] curves)
        {
            if (Event.current.type == EventType.Repaint)
            {
                for (int i = 0; i < this.m_DrawOrder.Count; i++)
                {
                    CurveWrapper wrapper = this.getCurveWrapperById(this.m_DrawOrder[i]);
                    if (((wrapper != null) && !wrapper.hidden) && (wrapper.curve.length != 0))
                    {
                        CurveWrapper wrapper2 = null;
                        if (i < (this.m_DrawOrder.Count - 1))
                        {
                            wrapper2 = this.getCurveWrapperById(this.m_DrawOrder[i + 1]);
                        }
                        if (this.IsRegion(wrapper, wrapper2))
                        {
                            i++;
                            bool hasFocus = this.ShouldCurveHaveFocus(i, wrapper, wrapper2);
                            this.DrawCurvesAndRegion(wrapper, wrapper2, !this.IsRegionCurveSelected(wrapper, wrapper2) ? null : this.selectedCurves, hasFocus);
                        }
                        else
                        {
                            bool flag2 = this.ShouldCurveHaveFocus(i, wrapper, null);
                            this.DrawCurveAndPoints(wrapper, !this.IsCurveSelected(wrapper) ? null : this.selectedCurves, flag2);
                        }
                    }
                }
                if (this.m_DraggingCurveOrRegion == null)
                {
                    HandleUtility.ApplyWireMaterial();
                    GL.Begin(1);
                    GL.Color(this.m_TangentColor * new Color(1f, 1f, 1f, 0.75f));
                    foreach (CurveSelection selection in this.selectedCurves)
                    {
                        if (!selection.semiSelected)
                        {
                            Vector2 position = this.GetPosition(selection);
                            if ((CurveUtility.GetKeyTangentMode(selection.keyframe, 0) == TangentMode.Editable) && (selection.keyframe.time != selection.curve.keys[0].time))
                            {
                                Vector2 lhs = this.GetPosition(new CurveSelection(selection.curveID, this, selection.key, CurveSelection.SelectionType.InTangent));
                                this.DrawLine(lhs, position);
                            }
                            if ((CurveUtility.GetKeyTangentMode(selection.keyframe, 1) == TangentMode.Editable) && (selection.keyframe.time != selection.curve.keys[selection.curve.keys.Length - 1].time))
                            {
                                Vector2 rhs = this.GetPosition(new CurveSelection(selection.curveID, this, selection.key, CurveSelection.SelectionType.OutTangent));
                                this.DrawLine(position, rhs);
                            }
                        }
                    }
                    GL.End();
                    GUI.color = this.m_TangentColor;
                    foreach (CurveSelection selection2 in this.selectedCurves)
                    {
                        if (!selection2.semiSelected)
                        {
                            if ((CurveUtility.GetKeyTangentMode(selection2.keyframe, 0) == TangentMode.Editable) && (selection2.keyframe.time != selection2.curve.keys[0].time))
                            {
                                Vector2 vector4 = this.GetPosition(new CurveSelection(selection2.curveID, this, selection2.key, CurveSelection.SelectionType.InTangent));
                                this.DrawPoint(vector4.x, vector4.y, CurveWrapper.SelectionMode.None);
                            }
                            if ((CurveUtility.GetKeyTangentMode(selection2.keyframe, 1) == TangentMode.Editable) && (selection2.keyframe.time != selection2.curve.keys[selection2.curve.keys.Length - 1].time))
                            {
                                Vector2 vector5 = this.GetPosition(new CurveSelection(selection2.curveID, this, selection2.key, CurveSelection.SelectionType.OutTangent));
                                this.DrawPoint(vector5.x, vector5.y, CurveWrapper.SelectionMode.None);
                            }
                        }
                    }
                    if (this.m_DraggingKey != null)
                    {
                        int num4;
                        float vRangeMin = base.vRangeMin;
                        float vRangeMax = base.vRangeMax;
                        vRangeMin = Mathf.Max(vRangeMin, this.m_DraggingKey.vRangeMin);
                        vRangeMax = Mathf.Min(vRangeMax, this.m_DraggingKey.vRangeMax);
                        Vector2 vector6 = this.m_DraggedCoord + this.m_MoveCoord;
                        vector6.x = Mathf.Clamp(vector6.x, base.hRangeMin, base.hRangeMax);
                        vector6.y = Mathf.Clamp(vector6.y, vRangeMin, vRangeMax);
                        Vector2 vector7 = base.DrawingToViewTransformPoint(vector6);
                        if (this.invSnap != 0f)
                        {
                            num4 = MathUtils.GetNumberOfDecimalsForMinimumDifference((float) (1f / this.invSnap));
                        }
                        else
                        {
                            num4 = MathUtils.GetNumberOfDecimalsForMinimumDifference((float) (base.shownArea.width / base.drawRect.width));
                        }
                        int numberOfDecimalsForMinimumDifference = MathUtils.GetNumberOfDecimalsForMinimumDifference((float) (base.shownArea.height / base.drawRect.height));
                        Vector2 vector8 = (this.m_DraggingKey.getAxisUiScalarsCallback == null) ? Vector2.one : this.m_DraggingKey.getAxisUiScalarsCallback();
                        if (vector8.x >= 0f)
                        {
                            vector6.x *= vector8.x;
                        }
                        if (vector8.y >= 0f)
                        {
                            vector6.y *= vector8.y;
                        }
                        string introduced31 = vector6.x.ToString("N" + num4);
                        GUIContent content = new GUIContent(string.Format("{0}, {1}", introduced31, vector6.y.ToString("N" + numberOfDecimalsForMinimumDifference)));
                        Vector2 vector9 = this.ms_Styles.dragLabel.CalcSize(content);
                        EditorGUI.DoDropShadowLabel(new Rect(vector7.x, vector7.y - vector9.y, vector9.x, vector9.y), content, this.ms_Styles.dragLabel, 0.3f);
                    }
                }
            }
        }

        private void DrawCurvesAndRegion(CurveWrapper cw1, CurveWrapper cw2, List<CurveSelection> selection, bool hasFocus)
        {
            this.DrawRegion(cw1, cw2, hasFocus);
            this.DrawCurveAndPoints(cw1, !this.IsCurveSelected(cw1) ? null : selection, hasFocus);
            this.DrawCurveAndPoints(cw2, !this.IsCurveSelected(cw2) ? null : selection, hasFocus);
        }

        private void DrawLine(Vector2 lhs, Vector2 rhs)
        {
            GL.Vertex(base.DrawingToViewTransformPoint(new Vector3(lhs.x, lhs.y, 0f)));
            GL.Vertex(base.DrawingToViewTransformPoint(new Vector3(rhs.x, rhs.y, 0f)));
        }

        private void DrawPoint(float x, float y, CurveWrapper.SelectionMode selected)
        {
            Vector3 vector = base.DrawingToViewTransformPoint(new Vector3(x, y, 0f));
            float introduced5 = Mathf.Floor(vector.x);
            vector = new Vector3(introduced5, Mathf.Floor(vector.y), 0f);
            Rect position = new Rect(vector.x - 4f, vector.y - 4f, (float) this.ms_Styles.pointIcon.width, (float) this.ms_Styles.pointIcon.height);
            Vector2 center = position.center;
            Vector2 vector3 = center - this.m_PreviousDrawPointCenter;
            if (vector3.magnitude > 8f)
            {
                if (selected == CurveWrapper.SelectionMode.None)
                {
                    GUI.Label(position, this.ms_Styles.pointIcon, GUIStyle.none);
                }
                else
                {
                    GUI.Label(position, this.ms_Styles.pointIconSelected, GUIStyle.none);
                    Color color = GUI.color;
                    GUI.color = Color.white;
                    if (selected == CurveWrapper.SelectionMode.Selected)
                    {
                        GUI.Label(position, this.ms_Styles.pointIconSelectedOverlay, GUIStyle.none);
                    }
                    else
                    {
                        GUI.Label(position, this.ms_Styles.pointIconSemiSelectedOverlay, GUIStyle.none);
                    }
                    GUI.color = color;
                }
                this.m_PreviousDrawPointCenter = center;
            }
        }

        private void DrawPointsOnCurve(CurveWrapper cw, List<CurveSelection> selected, bool hasFocus)
        {
            this.m_PreviousDrawPointCenter = new Vector2(float.MinValue, float.MinValue);
            if (selected == null)
            {
                Color color = cw.color;
                if (base.settings.useFocusColors && !hasFocus)
                {
                    color = (Color) (color * 0.5f);
                }
                GUI.color = color;
                foreach (Keyframe keyframe in cw.curve.keys)
                {
                    this.DrawPoint(keyframe.time, keyframe.value, CurveWrapper.SelectionMode.None);
                }
            }
            else
            {
                GUI.color = Color.Lerp(cw.color, Color.white, 0.2f);
                int num2 = 0;
                while ((num2 < selected.Count) && (selected[num2].curveID != cw.id))
                {
                    num2++;
                }
                int num3 = 0;
                foreach (Keyframe keyframe2 in cw.curve.keys)
                {
                    if (((num2 < selected.Count) && (selected[num2].key == num3)) && (selected[num2].curveID == cw.id))
                    {
                        this.DrawPoint(keyframe2.time, keyframe2.value, !selected[num2].semiSelected ? CurveWrapper.SelectionMode.Selected : CurveWrapper.SelectionMode.SemiSelected);
                        num2++;
                    }
                    else
                    {
                        this.DrawPoint(keyframe2.time, keyframe2.value, CurveWrapper.SelectionMode.None);
                    }
                    num3++;
                }
                GUI.color = Color.white;
            }
        }

        public void DrawRegion(CurveWrapper curve1, CurveWrapper curve2, bool hasFocus)
        {
            float deltaTime = 1f / (base.rect.width / 10f);
            List<Vector3> list = CreateRegion(curve1, curve2, deltaTime);
            Color a = curve1.color;
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = base.drawingToViewMatrix.MultiplyPoint(list[i]);
            }
            if (this.IsDraggingRegion(curve1, curve2))
            {
                a = Color.Lerp(a, Color.black, 0.1f);
                a.a = 0.4f;
            }
            else if (base.settings.useFocusColors && !hasFocus)
            {
                a = (Color) (a * 0.4f);
                a.a = 0.1f;
            }
            else
            {
                a = (Color) (a * 1f);
                a.a = 0.4f;
            }
            Shader.SetGlobalColor("_HandleColor", a);
            HandleUtility.ApplyWireMaterial();
            GL.Begin(4);
            int num3 = list.Count / 3;
            for (int j = 0; j < num3; j++)
            {
                GL.Color(a);
                GL.Vertex(list[j * 3]);
                GL.Vertex(list[(j * 3) + 1]);
                GL.Vertex(list[(j * 3) + 2]);
            }
            GL.End();
        }

        public void DrawTimeRange()
        {
            if (this.s_TimeRangeSelectionActive && (Event.current.type == EventType.Repaint))
            {
                float x = Mathf.Min(this.s_TimeRangeSelectionStart, this.s_TimeRangeSelectionEnd);
                float num2 = Mathf.Max(this.s_TimeRangeSelectionStart, this.s_TimeRangeSelectionEnd);
                float num3 = this.GetGUIPoint(new Vector3(x, 0f, 0f)).x;
                float num4 = this.GetGUIPoint(new Vector3(num2, 0f, 0f)).x;
                GUI.Label(new Rect(num3, -10000f, num4 - num3, 20000f), GUIContent.none, this.ms_Styles.selectionRect);
            }
        }

        private void EditAxisLabels()
        {
            int controlID = GUIUtility.GetControlID(0x1218b72, FocusType.Keyboard);
            List<CurveWrapper> curvesWithSameParameterSpace = new List<CurveWrapper>();
            Vector2 axisUiScalars = this.GetAxisUiScalars(curvesWithSameParameterSpace);
            if (((axisUiScalars.y >= 0f) && (curvesWithSameParameterSpace.Count > 0)) && (curvesWithSameParameterSpace[0].setAxisUiScalarsCallback != null))
            {
                Rect position = new Rect(0f, base.topmargin - 8f, base.leftmargin - 4f, 16f);
                Rect rect2 = position;
                rect2.y -= position.height;
                Event current = Event.current;
                switch (current.GetTypeForControl(controlID))
                {
                    case EventType.MouseDown:
                        if (current.button == 0)
                        {
                            if (rect2.Contains(Event.current.mousePosition) && (GUIUtility.hotControl == 0))
                            {
                                GUIUtility.keyboardControl = 0;
                                GUIUtility.hotControl = controlID;
                                GUI.changed = true;
                                current.Use();
                            }
                            if (!position.Contains(Event.current.mousePosition))
                            {
                                GUIUtility.keyboardControl = 0;
                            }
                        }
                        break;

                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == controlID)
                        {
                            GUIUtility.hotControl = 0;
                        }
                        break;

                    case EventType.MouseDrag:
                        if (GUIUtility.hotControl == controlID)
                        {
                            float num2 = Mathf.Clamp01(Mathf.Max(axisUiScalars.y, Mathf.Pow(Mathf.Abs(axisUiScalars.y), 0.5f)) * 0.01f);
                            axisUiScalars.y += HandleUtility.niceMouseDelta * num2;
                            if (axisUiScalars.y < 0.001f)
                            {
                                axisUiScalars.y = 0.001f;
                            }
                            this.SetAxisUiScalars(axisUiScalars, curvesWithSameParameterSpace);
                            current.Use();
                        }
                        break;

                    case EventType.Repaint:
                        if (GUIUtility.hotControl == 0)
                        {
                            EditorGUIUtility.AddCursorRect(rect2, MouseCursor.SlideArrow);
                        }
                        break;
                }
                string kFloatFieldFormatString = EditorGUI.kFloatFieldFormatString;
                EditorGUI.kFloatFieldFormatString = this.m_AxisLabelFormat;
                float y = EditorGUI.FloatField(position, axisUiScalars.y, this.ms_Styles.axisLabelNumberField);
                if (axisUiScalars.y != y)
                {
                    this.SetAxisUiScalars(new Vector2(axisUiScalars.x, y), curvesWithSameParameterSpace);
                }
                EditorGUI.kFloatFieldFormatString = kFloatFieldFormatString;
            }
        }

        private void EditSelectedPoints()
        {
            Event current = Event.current;
            if (this.editingPoints && !this.hasSelection)
            {
                this.editingPoints = false;
            }
            bool flag = false;
            if (current.type == EventType.KeyDown)
            {
                if ((current.keyCode == KeyCode.KeypadEnter) || (current.keyCode == KeyCode.Return))
                {
                    if (this.hasSelection && !this.editingPoints)
                    {
                        this.StartEditingSelectedPoints();
                        current.Use();
                    }
                    else if (this.editingPoints)
                    {
                        this.FinishEditingPoints();
                        current.Use();
                    }
                }
                else if (current.keyCode == KeyCode.Escape)
                {
                    flag = true;
                }
            }
            if (this.editingPoints)
            {
                Vector2 vector = base.DrawingToViewTransformPoint(this.pointEditingFieldPosition);
                Rect rect = Rect.MinMaxRect(base.leftmargin, base.topmargin, base.rect.width - base.rightmargin, base.rect.height - base.bottommargin);
                vector.x = Mathf.Clamp(vector.x, rect.xMin, rect.xMax - 80f);
                vector.y = Mathf.Clamp(vector.y, rect.yMin, rect.yMax - 36f);
                EditorGUI.BeginChangeCheck();
                GUI.SetNextControlName("pointTimeField");
                if (<>f__am$cache2D == null)
                {
                    <>f__am$cache2D = x => x.keyframe.time;
                }
                float num3 = this.PointFieldForSelection(new Rect(vector.x, vector.y, 80f, 18f), 1, <>f__am$cache2D, "time");
                if (EditorGUI.EndChangeCheck())
                {
                    this.timeWasEdited = true;
                }
                EditorGUI.BeginChangeCheck();
                GUI.SetNextControlName("pointValueField");
                if (<>f__am$cache2E == null)
                {
                    <>f__am$cache2E = x => x.keyframe.value;
                }
                float y = this.PointFieldForSelection(new Rect(vector.x, vector.y + 18f, 80f, 18f), 2, <>f__am$cache2E, "value");
                if (EditorGUI.EndChangeCheck())
                {
                    this.valueWasEdited = true;
                }
                if (this.timeWasEdited || this.valueWasEdited)
                {
                    this.SetSelectedKeyPositions(new Vector2(num3, y), this.timeWasEdited, this.valueWasEdited);
                }
                if (flag)
                {
                    this.FinishEditingPoints();
                }
                if (this.focusedPointField != null)
                {
                    EditorGUI.FocusTextInControl(this.focusedPointField);
                    if (current.type == EventType.Repaint)
                    {
                        this.focusedPointField = null;
                    }
                }
                if ((current.type == EventType.KeyDown) && (current.character == '\t'))
                {
                    this.focusedPointField = !(GUI.GetNameOfFocusedControl() == "pointValueField") ? "pointValueField" : "pointTimeField";
                    current.Use();
                }
                if (current.type == EventType.MouseDown)
                {
                    this.FinishEditingPoints();
                }
            }
        }

        public void EndTimeRangeSelection()
        {
            if (!this.s_TimeRangeSelectionActive)
            {
                Debug.LogError("EndTimeRangeSelection can only be called after BeginTimeRangeSelection");
            }
            else
            {
                this.s_TimeRangeSelectionStart = this.s_TimeRangeSelectionEnd = 0f;
                this.s_TimeRangeSelectionActive = false;
            }
        }

        private CurveSelection FindNearest()
        {
            Vector2 mousePosition = Event.current.mousePosition;
            bool flag = false;
            int curveID = -1;
            int keyIndex = -1;
            float num3 = 100f;
            for (int i = this.m_DrawOrder.Count - 1; i >= 0; i--)
            {
                CurveWrapper wrapper = this.getCurveWrapperById(this.m_DrawOrder[i]);
                if (!wrapper.readOnly && !wrapper.hidden)
                {
                    for (int j = 0; j < wrapper.curve.keys.Length; j++)
                    {
                        Keyframe keyframe = wrapper.curve.keys[j];
                        Vector2 vector2 = this.GetGUIPoint((Vector3) new Vector2(keyframe.time, keyframe.value)) - mousePosition;
                        float sqrMagnitude = vector2.sqrMagnitude;
                        if (sqrMagnitude <= 16f)
                        {
                            return new CurveSelection(wrapper.id, this, j);
                        }
                        if (sqrMagnitude < num3)
                        {
                            flag = true;
                            curveID = wrapper.id;
                            keyIndex = j;
                            num3 = sqrMagnitude;
                        }
                    }
                    if ((i == (this.m_DrawOrder.Count - 1)) && (curveID >= 0))
                    {
                        num3 = 16f;
                    }
                }
            }
            if (flag)
            {
                return new CurveSelection(curveID, this, keyIndex);
            }
            return null;
        }

        private void FinishEditingPoints()
        {
            this.editingPoints = false;
        }

        public void FrameSelected(bool horizontally, bool vertically)
        {
            Bounds drawingBounds = new Bounds();
            if (!this.hasSelection)
            {
                drawingBounds = this.drawingBounds;
                if (drawingBounds.size == Vector3.zero)
                {
                    return;
                }
            }
            else
            {
                drawingBounds = new Bounds((Vector3) new Vector2(this.selectedCurves[0].keyframe.time, this.selectedCurves[0].keyframe.value), (Vector3) Vector2.zero);
                foreach (CurveSelection selection in this.selectedCurves)
                {
                    Keyframe keyframe3 = selection.curve[selection.key];
                    Keyframe keyframe4 = selection.curve[selection.key];
                    drawingBounds.Encapsulate((Vector3) new Vector2(keyframe3.time, keyframe4.value));
                    if ((selection.key - 1) >= 0)
                    {
                        Keyframe keyframe5 = selection.curve[selection.key - 1];
                        Keyframe keyframe6 = selection.curve[selection.key - 1];
                        drawingBounds.Encapsulate((Vector3) new Vector2(keyframe5.time, keyframe6.value));
                    }
                    if ((selection.key + 1) < selection.curve.length)
                    {
                        Keyframe keyframe7 = selection.curve[selection.key + 1];
                        Keyframe keyframe8 = selection.curve[selection.key + 1];
                        drawingBounds.Encapsulate((Vector3) new Vector2(keyframe7.time, keyframe8.value));
                    }
                }
                float x = Mathf.Max(drawingBounds.size.x, 0.1f);
                drawingBounds.size = new Vector3(x, Mathf.Max(drawingBounds.size.y, 0.1f), 0f);
            }
            if (horizontally)
            {
                base.SetShownHRangeInsideMargins(drawingBounds.min.x, drawingBounds.max.x);
            }
            if (vertically)
            {
                base.SetShownVRangeInsideMargins(drawingBounds.min.y, drawingBounds.max.y);
            }
        }

        private Vector2 GetAxisUiScalars(List<CurveWrapper> curvesWithSameParameterSpace)
        {
            if (this.selectedCurves.Count <= 1)
            {
                if (this.m_DrawOrder.Count > 0)
                {
                    CurveWrapper item = this.getCurveWrapperById(this.m_DrawOrder[this.m_DrawOrder.Count - 1]);
                    if (item.getAxisUiScalarsCallback != null)
                    {
                        if (curvesWithSameParameterSpace != null)
                        {
                            curvesWithSameParameterSpace.Add(item);
                        }
                        return item.getAxisUiScalarsCallback();
                    }
                }
                return Vector2.one;
            }
            Vector2 vector = new Vector2(-1f, -1f);
            if (this.selectedCurves.Count > 1)
            {
                bool flag = true;
                bool flag2 = true;
                Vector2 one = Vector2.one;
                for (int i = 0; i < this.selectedCurves.Count; i++)
                {
                    CurveWrapper curveWrapper = this.selectedCurves[i].curveWrapper;
                    if (curveWrapper.getAxisUiScalarsCallback != null)
                    {
                        Vector2 vector3 = curveWrapper.getAxisUiScalarsCallback();
                        if (i == 0)
                        {
                            one = vector3;
                        }
                        else
                        {
                            if (Mathf.Abs((float) (vector3.x - one.x)) > 1E-05f)
                            {
                                flag = false;
                            }
                            if (Mathf.Abs((float) (vector3.y - one.y)) > 1E-05f)
                            {
                                flag2 = false;
                            }
                            one = vector3;
                        }
                        if (curvesWithSameParameterSpace != null)
                        {
                            curvesWithSameParameterSpace.Add(curveWrapper);
                        }
                    }
                }
                if (flag)
                {
                    vector.x = one.x;
                }
                if (flag2)
                {
                    vector.y = one.y;
                }
            }
            return vector;
        }

        private int GetCurveAtPosition(Vector2 position, out Vector2 closestPointOnCurve)
        {
            Vector2 vector = base.DrawingToViewTransformPoint(position);
            int num = (int) Mathf.Sqrt(100f);
            float num2 = 100f;
            int listIndex = -1;
            closestPointOnCurve = Vector3.zero;
            for (int i = this.m_DrawOrder.Count - 1; i >= 0; i--)
            {
                CurveWrapper wrapper = this.getCurveWrapperById(this.m_DrawOrder[i]);
                if (!wrapper.hidden && !wrapper.readOnly)
                {
                    Vector2 vector2;
                    vector2.x = position.x - (((float) num) / base.scale.x);
                    vector2.y = wrapper.renderer.EvaluateCurveSlow(vector2.x);
                    vector2 = base.DrawingToViewTransformPoint(vector2);
                    for (int j = -num; j < num; j++)
                    {
                        Vector2 vector3;
                        vector3.x = position.x + (((float) (j + 1)) / base.scale.x);
                        vector3.y = wrapper.renderer.EvaluateCurveSlow(vector3.x);
                        vector3 = base.DrawingToViewTransformPoint(vector3);
                        float num6 = HandleUtility.DistancePointLine((Vector3) vector, (Vector3) vector2, (Vector3) vector3);
                        num6 *= num6;
                        if (num6 < num2)
                        {
                            num2 = num6;
                            listIndex = wrapper.listIndex;
                            closestPointOnCurve = HandleUtility.ProjectPointLine((Vector3) vector, (Vector3) vector2, (Vector3) vector3);
                        }
                        vector2 = vector3;
                    }
                }
            }
            closestPointOnCurve = base.ViewToDrawingTransformPoint(closestPointOnCurve);
            return listIndex;
        }

        internal CurveWrapper GetCurveFromID(int curveID)
        {
            if (this.m_AnimationCurves != null)
            {
                foreach (CurveWrapper wrapper in this.m_AnimationCurves)
                {
                    if (wrapper.id == curveID)
                    {
                        return wrapper;
                    }
                }
            }
            return null;
        }

        public CurveWrapper getCurveWrapperById(int id)
        {
            foreach (CurveWrapper wrapper in this.m_AnimationCurves)
            {
                if (wrapper.id == id)
                {
                    return wrapper;
                }
            }
            return null;
        }

        private Vector2 GetGUIPoint(Vector3 point)
        {
            return HandleUtility.WorldToGUIPoint(base.DrawingToViewTransformPoint(point));
        }

        private Vector2 GetPosition(CurveSelection selection)
        {
            Keyframe keyframe = selection.keyframe;
            Vector2 vector = new Vector2(keyframe.time, keyframe.value);
            float num = 50f;
            if (selection.type == CurveSelection.SelectionType.InTangent)
            {
                Vector2 vector2 = new Vector2(1f, keyframe.inTangent);
                if (keyframe.inTangent == float.PositiveInfinity)
                {
                    vector2 = new Vector2(0f, -1f);
                }
                vector2 = base.NormalizeInViewSpace(vector2);
                return (vector - ((Vector2) (vector2 * num)));
            }
            if (selection.type != CurveSelection.SelectionType.OutTangent)
            {
                return vector;
            }
            Vector2 vec = new Vector2(1f, keyframe.outTangent);
            if (keyframe.outTangent == float.PositiveInfinity)
            {
                vec = new Vector2(0f, -1f);
            }
            vec = base.NormalizeInViewSpace(vec);
            return (vector + ((Vector2) (vec * num)));
        }

        public bool GetTopMostCurveID(out int curveID)
        {
            if (this.m_DrawOrder.Count > 0)
            {
                curveID = this.m_DrawOrder[this.m_DrawOrder.Count - 1];
                return true;
            }
            curveID = -1;
            return false;
        }

        public void GridGUI()
        {
            GUI.BeginGroup(base.drawRect);
            if (Event.current.type != EventType.Repaint)
            {
                GUI.EndGroup();
            }
            else
            {
                float yMin;
                float num2;
                this.InitStyles();
                Color color = GUI.color;
                Vector2 axisUiScalars = this.GetAxisUiScalars(null);
                Rect shownArea = base.shownArea;
                base.hTicks.SetRanges(shownArea.xMin * axisUiScalars.x, shownArea.xMax * axisUiScalars.x, base.drawRect.xMin, base.drawRect.xMax);
                base.vTicks.SetRanges(shownArea.yMin * axisUiScalars.y, shownArea.yMax * axisUiScalars.y, base.drawRect.yMin, base.drawRect.yMax);
                HandleUtility.ApplyWireMaterial();
                GL.Begin(1);
                base.hTicks.SetTickStrengths((float) base.settings.hTickStyle.distMin, (float) base.settings.hTickStyle.distFull, false);
                if (base.settings.hTickStyle.stubs)
                {
                    yMin = shownArea.yMin;
                    num2 = shownArea.yMin - (40f / base.scale.y);
                }
                else
                {
                    yMin = Mathf.Max(shownArea.yMin, base.vRangeMin);
                    num2 = Mathf.Min(shownArea.yMax, base.vRangeMax);
                }
                for (int i = 0; i < base.hTicks.tickLevels; i++)
                {
                    float strengthOfLevel = base.hTicks.GetStrengthOfLevel(i);
                    if (strengthOfLevel > 0f)
                    {
                        GL.Color((base.settings.hTickStyle.color * new Color(1f, 1f, 1f, strengthOfLevel)) * new Color(1f, 1f, 1f, 0.75f));
                        float[] ticksAtLevel = base.hTicks.GetTicksAtLevel(i, true);
                        for (int k = 0; k < ticksAtLevel.Length; k++)
                        {
                            ticksAtLevel[k] /= axisUiScalars.x;
                            if ((ticksAtLevel[k] > base.hRangeMin) && (ticksAtLevel[k] < base.hRangeMax))
                            {
                                this.DrawLine(new Vector2(ticksAtLevel[k], yMin), new Vector2(ticksAtLevel[k], num2));
                            }
                        }
                    }
                }
                GL.Color((base.settings.hTickStyle.color * new Color(1f, 1f, 1f, 1f)) * new Color(1f, 1f, 1f, 0.75f));
                if (base.hRangeMin != float.NegativeInfinity)
                {
                    this.DrawLine(new Vector2(base.hRangeMin, yMin), new Vector2(base.hRangeMin, num2));
                }
                if (base.hRangeMax != float.PositiveInfinity)
                {
                    this.DrawLine(new Vector2(base.hRangeMax, yMin), new Vector2(base.hRangeMax, num2));
                }
                base.vTicks.SetTickStrengths((float) base.settings.vTickStyle.distMin, (float) base.settings.vTickStyle.distFull, false);
                if (base.settings.vTickStyle.stubs)
                {
                    yMin = shownArea.xMin;
                    num2 = shownArea.xMin + (40f / base.scale.x);
                }
                else
                {
                    yMin = Mathf.Max(shownArea.xMin, base.hRangeMin);
                    num2 = Mathf.Min(shownArea.xMax, base.hRangeMax);
                }
                for (int j = 0; j < base.vTicks.tickLevels; j++)
                {
                    float a = base.vTicks.GetStrengthOfLevel(j);
                    if (a > 0f)
                    {
                        GL.Color((base.settings.vTickStyle.color * new Color(1f, 1f, 1f, a)) * new Color(1f, 1f, 1f, 0.75f));
                        float[] numArray2 = base.vTicks.GetTicksAtLevel(j, true);
                        for (int m = 0; m < numArray2.Length; m++)
                        {
                            numArray2[m] /= axisUiScalars.y;
                            if ((numArray2[m] > base.vRangeMin) && (numArray2[m] < base.vRangeMax))
                            {
                                this.DrawLine(new Vector2(yMin, numArray2[m]), new Vector2(num2, numArray2[m]));
                            }
                        }
                    }
                }
                GL.Color((base.settings.vTickStyle.color * new Color(1f, 1f, 1f, 1f)) * new Color(1f, 1f, 1f, 0.75f));
                if (base.vRangeMin != float.NegativeInfinity)
                {
                    this.DrawLine(new Vector2(yMin, base.vRangeMin), new Vector2(num2, base.vRangeMin));
                }
                if (base.vRangeMax != float.PositiveInfinity)
                {
                    this.DrawLine(new Vector2(yMin, base.vRangeMax), new Vector2(num2, base.vRangeMax));
                }
                GL.End();
                if (base.settings.showAxisLabels)
                {
                    if ((base.settings.hTickStyle.distLabel > 0) && (axisUiScalars.x > 0f))
                    {
                        GUI.color = base.settings.hTickStyle.labelColor;
                        int levelWithMinSeparation = base.hTicks.GetLevelWithMinSeparation((float) base.settings.hTickStyle.distLabel);
                        int numberOfDecimalsForMinimumDifference = MathUtils.GetNumberOfDecimalsForMinimumDifference(base.hTicks.GetPeriodOfLevel(levelWithMinSeparation));
                        float[] numArray3 = base.hTicks.GetTicksAtLevel(levelWithMinSeparation, false);
                        float[] numArray4 = (float[]) numArray3.Clone();
                        float y = Mathf.Floor(base.drawRect.height);
                        for (int n = 0; n < numArray3.Length; n++)
                        {
                            numArray4[n] /= axisUiScalars.x;
                            if ((numArray4[n] >= base.hRangeMin) && (numArray4[n] <= base.hRangeMax))
                            {
                                Rect rect2;
                                TextAnchor upperCenter;
                                Vector2 vector2 = base.DrawingToViewTransformPoint(new Vector2(numArray4[n], 0f));
                                vector2 = new Vector2(Mathf.Floor(vector2.x), y);
                                float num13 = numArray3[n];
                                if (base.settings.hTickStyle.centerLabel)
                                {
                                    upperCenter = TextAnchor.UpperCenter;
                                    rect2 = new Rect(vector2.x, (vector2.y - 16f) - base.settings.hTickLabelOffset, 1f, 16f);
                                }
                                else
                                {
                                    upperCenter = TextAnchor.UpperLeft;
                                    rect2 = new Rect(vector2.x, (vector2.y - 16f) - base.settings.hTickLabelOffset, 50f, 16f);
                                }
                                if (this.ms_Styles.labelTickMarksX.alignment != upperCenter)
                                {
                                    this.ms_Styles.labelTickMarksX.alignment = upperCenter;
                                }
                                GUI.Label(rect2, num13.ToString("n" + numberOfDecimalsForMinimumDifference) + base.settings.hTickStyle.unit, this.ms_Styles.labelTickMarksX);
                            }
                        }
                    }
                    if ((base.settings.vTickStyle.distLabel > 0) && (axisUiScalars.y > 0f))
                    {
                        GUI.color = base.settings.vTickStyle.labelColor;
                        int level = base.vTicks.GetLevelWithMinSeparation((float) base.settings.vTickStyle.distLabel);
                        float[] numArray5 = base.vTicks.GetTicksAtLevel(level, false);
                        float[] numArray6 = (float[]) numArray5.Clone();
                        int num15 = MathUtils.GetNumberOfDecimalsForMinimumDifference(base.vTicks.GetPeriodOfLevel(level));
                        string format = "n" + num15;
                        this.m_AxisLabelFormat = format;
                        float width = 35f;
                        if (!base.settings.vTickStyle.stubs && (numArray5.Length > 1))
                        {
                            float num17 = numArray5[1];
                            float num18 = numArray5[numArray5.Length - 1];
                            string text = num17.ToString(format) + base.settings.vTickStyle.unit;
                            string str3 = num18.ToString(format) + base.settings.vTickStyle.unit;
                            width = Mathf.Max(this.ms_Styles.labelTickMarksY.CalcSize(new GUIContent(text)).x, this.ms_Styles.labelTickMarksY.CalcSize(new GUIContent(str3)).x) + 6f;
                        }
                        for (int num19 = 0; num19 < numArray5.Length; num19++)
                        {
                            numArray6[num19] /= axisUiScalars.y;
                            if ((numArray6[num19] >= base.vRangeMin) && (numArray6[num19] <= base.vRangeMax))
                            {
                                Rect rect3;
                                Vector2 vector3 = base.DrawingToViewTransformPoint(new Vector2(0f, numArray6[num19]));
                                vector3 = new Vector2(vector3.x, Mathf.Floor(vector3.y));
                                float num20 = numArray5[num19];
                                if (base.settings.vTickStyle.centerLabel)
                                {
                                    rect3 = new Rect(0f, vector3.y - 8f, base.leftmargin - 4f, 16f);
                                }
                                else
                                {
                                    rect3 = new Rect(0f, vector3.y - 13f, width, 16f);
                                }
                                GUI.Label(rect3, num20.ToString(format) + base.settings.vTickStyle.unit, this.ms_Styles.labelTickMarksY);
                            }
                        }
                    }
                }
                GUI.color = color;
                GUI.EndGroup();
            }
        }

        private bool HandleCurveAndRegionMoveToFrontOnMouseDown(ref Vector2 timeValue, ref CurveWrapper[] curves)
        {
            Vector2 vector;
            int curveAtPosition = this.GetCurveAtPosition(base.mousePositionInDrawing, out vector);
            if (curveAtPosition >= 0)
            {
                this.MoveCurveToFront(this.m_AnimationCurves[curveAtPosition].id);
                timeValue = base.mousePositionInDrawing;
                CurveWrapper[] wrapperArray1 = new CurveWrapper[] { this.m_AnimationCurves[curveAtPosition] };
                curves = wrapperArray1;
                return true;
            }
            for (int i = this.m_DrawOrder.Count - 1; i >= 0; i--)
            {
                CurveWrapper wrapper = this.getCurveWrapperById(this.m_DrawOrder[i]);
                if (((wrapper != null) && !wrapper.hidden) && (wrapper.curve.length != 0))
                {
                    CurveWrapper wrapper2 = null;
                    if (i > 0)
                    {
                        wrapper2 = this.getCurveWrapperById(this.m_DrawOrder[i - 1]);
                    }
                    if (this.IsRegion(wrapper, wrapper2))
                    {
                        float y = base.mousePositionInDrawing.y;
                        float x = base.mousePositionInDrawing.x;
                        float num5 = wrapper.renderer.EvaluateCurveSlow(x);
                        float num6 = wrapper2.renderer.EvaluateCurveSlow(x);
                        if (num5 > num6)
                        {
                            float num7 = num5;
                            num5 = num6;
                            num6 = num7;
                        }
                        if ((y >= num5) && (y <= num6))
                        {
                            timeValue = base.mousePositionInDrawing;
                            CurveWrapper[] wrapperArray2 = new CurveWrapper[] { wrapper, wrapper2 };
                            curves = wrapperArray2;
                            this.MoveCurveToFront(wrapper.id);
                            return true;
                        }
                        i--;
                    }
                }
            }
            return false;
        }

        private void Init()
        {
            if (((this.selectedCurves != null) && this.hasSelection) && (this.selectedCurves[0].m_Host == null))
            {
                foreach (CurveSelection selection in this.selectedCurves)
                {
                    selection.m_Host = this;
                }
            }
        }

        internal void InitStyles()
        {
            if (this.ms_Styles == null)
            {
                this.ms_Styles = new Styles();
            }
        }

        private bool IsCurveSelected(CurveWrapper cw)
        {
            return ((cw != null) && (cw.selected != CurveWrapper.SelectionMode.None));
        }

        public bool IsDraggingCurve(CurveWrapper cw)
        {
            return (((this.m_DraggingCurveOrRegion != null) && (this.m_DraggingCurveOrRegion.Length == 1)) && (this.m_DraggingCurveOrRegion[0] == cw));
        }

        public bool IsDraggingCurveOrRegion()
        {
            return (this.m_DraggingCurveOrRegion != null);
        }

        public bool IsDraggingRegion(CurveWrapper cw1, CurveWrapper cw2)
        {
            return (((this.m_DraggingCurveOrRegion != null) && (this.m_DraggingCurveOrRegion.Length == 2)) && ((this.m_DraggingCurveOrRegion[0] == cw1) || (this.m_DraggingCurveOrRegion[0] == cw2)));
        }

        private bool IsRegion(CurveWrapper cw1, CurveWrapper cw2)
        {
            return ((((cw1 != null) && (cw2 != null)) && (cw1.regionId >= 0)) && (cw1.regionId == cw2.regionId));
        }

        private bool IsRegionCurveSelected(CurveWrapper cw1, CurveWrapper cw2)
        {
            return (this.IsCurveSelected(cw1) || this.IsCurveSelected(cw2));
        }

        internal void MakeCurveBackups()
        {
            this.m_CurveBackups = new List<SavedCurve>();
            int num = -1;
            SavedCurve item = null;
            for (int i = 0; i < this.selectedCurves.Count; i++)
            {
                CurveSelection selection = this.selectedCurves[i];
                if (selection.curveID != num)
                {
                    item = new SavedCurve();
                    num = item.curveId = selection.curveID;
                    Keyframe[] keys = selection.curve.keys;
                    item.keys = new List<SavedCurve.SavedKeyFrame>(keys.Length);
                    foreach (Keyframe keyframe in keys)
                    {
                        item.keys.Add(new SavedCurve.SavedKeyFrame(keyframe, CurveWrapper.SelectionMode.None));
                    }
                    this.m_CurveBackups.Add(item);
                }
                item.keys[selection.key].selected = !selection.semiSelected ? CurveWrapper.SelectionMode.Selected : CurveWrapper.SelectionMode.SemiSelected;
            }
        }

        private void MoveCurveToFront(int curveID)
        {
            int count = this.m_DrawOrder.Count;
            for (int i = 0; i < count; i++)
            {
                if (this.m_DrawOrder[i] == curveID)
                {
                    int regionId = this.getCurveWrapperById(curveID).regionId;
                    if (regionId < 0)
                    {
                        if (i != (count - 1))
                        {
                            this.m_DrawOrder.RemoveAt(i);
                            this.m_DrawOrder.Add(curveID);
                            this.ValidateCurveList();
                        }
                        return;
                    }
                    int num4 = 0;
                    int item = -1;
                    if ((i - 1) >= 0)
                    {
                        int id = this.m_DrawOrder[i - 1];
                        if (regionId == this.getCurveWrapperById(id).regionId)
                        {
                            item = id;
                            num4 = -1;
                        }
                    }
                    if (((i + 1) < count) && (item < 0))
                    {
                        int num7 = this.m_DrawOrder[i + 1];
                        if (regionId == this.getCurveWrapperById(num7).regionId)
                        {
                            item = num7;
                            num4 = 0;
                        }
                    }
                    if (item >= 0)
                    {
                        this.m_DrawOrder.RemoveRange(i + num4, 2);
                        this.m_DrawOrder.Add(item);
                        this.m_DrawOrder.Add(curveID);
                        this.ValidateCurveList();
                        return;
                    }
                    Debug.LogError("Unhandled region");
                }
            }
        }

        public Vector2 MovePoints()
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            if (!this.hasSelection && !base.settings.allowDraggingCurvesAndRegions)
            {
                return Vector2.zero;
            }
            Event current = Event.current;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (current.button != 0)
                    {
                        goto Label_04A9;
                    }
                    foreach (CurveSelection selection in this.selectedCurves)
                    {
                        if (!selection.curveWrapper.hidden)
                        {
                            Vector2 vector4 = base.DrawingToViewTransformPoint(this.GetPosition(selection)) - current.mousePosition;
                            if (vector4.sqrMagnitude <= 100f)
                            {
                                this.SetupKeyOrCurveDragging(new Vector2(selection.keyframe.time, selection.keyframe.value), selection.curveWrapper, controlID, current.mousePosition);
                                break;
                            }
                        }
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        this.ResetDragging();
                        GUI.changed = true;
                        current.Use();
                    }
                    goto Label_04A9;

                case EventType.MouseDrag:
                {
                    if (GUIUtility.hotControl != controlID)
                    {
                        goto Label_04A9;
                    }
                    Vector2 lhs = current.mousePosition - this.s_StartMouseDragPosition;
                    Vector3 zero = Vector3.zero;
                    if (current.shift && (this.m_AxisLock == AxisLock.None))
                    {
                        float introduced20 = Mathf.Abs(lhs.x);
                        this.m_AxisLock = (introduced20 <= Mathf.Abs(lhs.y)) ? AxisLock.Y : AxisLock.X;
                    }
                    if (this.m_DraggingCurveOrRegion != null)
                    {
                        lhs.x = 0f;
                        zero = (Vector3) base.ViewToDrawingTransformVector(lhs);
                        float introduced21 = this.SnapValue(zero.y + this.s_StartKeyDragPosition.y);
                        zero.y = introduced21 - this.s_StartKeyDragPosition.y;
                    }
                    else
                    {
                        switch (this.m_AxisLock)
                        {
                            case AxisLock.None:
                            {
                                zero = (Vector3) base.ViewToDrawingTransformVector(lhs);
                                float introduced22 = this.SnapTime(zero.x + this.s_StartKeyDragPosition.x);
                                zero.x = introduced22 - this.s_StartKeyDragPosition.x;
                                float introduced23 = this.SnapValue(zero.y + this.s_StartKeyDragPosition.y);
                                zero.y = introduced23 - this.s_StartKeyDragPosition.y;
                                break;
                            }
                            case AxisLock.X:
                            {
                                lhs.y = 0f;
                                zero = (Vector3) base.ViewToDrawingTransformVector(lhs);
                                float introduced24 = this.SnapTime(zero.x + this.s_StartKeyDragPosition.x);
                                zero.x = introduced24 - this.s_StartKeyDragPosition.x;
                                break;
                            }
                            case AxisLock.Y:
                            {
                                lhs.x = 0f;
                                zero = (Vector3) base.ViewToDrawingTransformVector(lhs);
                                float introduced25 = this.SnapValue(zero.y + this.s_StartKeyDragPosition.y);
                                zero.y = introduced25 - this.s_StartKeyDragPosition.y;
                                break;
                            }
                        }
                    }
                    this.TranslateSelectedKeys(zero);
                    GUI.changed = true;
                    current.Use();
                    return zero;
                }
                case EventType.KeyDown:
                    if ((GUIUtility.hotControl == controlID) && (current.keyCode == KeyCode.Escape))
                    {
                        this.TranslateSelectedKeys(Vector2.zero);
                        this.ResetDragging();
                        GUI.changed = true;
                        current.Use();
                    }
                    goto Label_04A9;

                case EventType.Repaint:
                    if (this.m_DraggingCurveOrRegion != null)
                    {
                        EditorGUIUtility.AddCursorRect(new Rect(current.mousePosition.x - 10f, current.mousePosition.y - 10f, 20f, 20f), MouseCursor.ResizeVertical);
                    }
                    goto Label_04A9;

                default:
                    goto Label_04A9;
            }
            if (base.settings.allowDraggingCurvesAndRegions && (this.m_DraggingKey == null))
            {
                Vector2 timeValue = Vector2.zero;
                CurveWrapper[] curves = null;
                if (this.HandleCurveAndRegionMoveToFrontOnMouseDown(ref timeValue, ref curves))
                {
                    List<CurveSelection> list = new List<CurveSelection>();
                    foreach (CurveWrapper wrapper in curves)
                    {
                        for (int i = 0; i < wrapper.curve.keys.Length; i++)
                        {
                            list.Add(new CurveSelection(wrapper.id, this, i));
                        }
                        this.MoveCurveToFront(wrapper.id);
                    }
                    this.preCurveDragSelection = this.selectedCurves;
                    this.selectedCurves = list;
                    this.SetupKeyOrCurveDragging(timeValue, curves[0], controlID, current.mousePosition);
                    this.m_DraggingCurveOrRegion = curves;
                }
            }
        Label_04A9:
            return Vector2.zero;
        }

        public void OnDisable()
        {
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
        }

        public void OnGUI()
        {
            base.BeginViewGUI();
            this.GridGUI();
            this.CurveGUI();
            base.EndViewGUI();
        }

        private int OnlyOneEditableCurve()
        {
            int num = -1;
            int num2 = 0;
            for (int i = 0; i < this.m_AnimationCurves.Length; i++)
            {
                CurveWrapper wrapper = this.m_AnimationCurves[i];
                if (!wrapper.hidden && !wrapper.readOnly)
                {
                    num2++;
                    num = i;
                }
            }
            if (num2 == 1)
            {
                return num;
            }
            return -1;
        }

        private float PointFieldForSelection(Rect rect, int customID, Func<CurveSelection, float> memberGetter, string label)
        {
            <PointFieldForSelection>c__AnonStorey47 storey = new <PointFieldForSelection>c__AnonStorey47 {
                memberGetter = memberGetter,
                <>f__this = this
            };
            float num = 0f;
            if (this.selectedCurves.All<CurveSelection>(new Func<CurveSelection, bool>(storey.<>m__7D)))
            {
                num = storey.memberGetter(this.selectedCurves[0]);
            }
            else
            {
                EditorGUI.showMixedValue = true;
            }
            Rect position = rect;
            position.x -= position.width;
            GUIStyle style = GUI.skin.label;
            style.alignment = TextAnchor.UpperRight;
            int id = GUIUtility.GetControlID(customID, FocusType.Keyboard, rect);
            Color color = GUI.color;
            GUI.color = Color.white;
            GUI.Label(position, label, style);
            float num3 = EditorGUI.DoFloatField(EditorGUI.s_RecycledEditor, rect, new Rect(0f, 0f, 0f, 0f), id, num, EditorGUI.kFloatFieldFormatString, EditorStyles.numberField, false);
            GUI.color = color;
            EditorGUI.showMixedValue = false;
            return num3;
        }

        private void RecalcCurveSelection()
        {
            foreach (CurveWrapper wrapper in this.m_AnimationCurves)
            {
                wrapper.selected = CurveWrapper.SelectionMode.None;
            }
            foreach (CurveSelection selection in this.selectedCurves)
            {
                selection.curveWrapper.selected = !selection.semiSelected ? CurveWrapper.SelectionMode.Selected : CurveWrapper.SelectionMode.SemiSelected;
            }
        }

        private void RecalcSecondarySelection()
        {
            List<CurveSelection> list = new List<CurveSelection>(this.m_Selection.Count);
            foreach (CurveSelection selection in this.m_Selection)
            {
                CurveWrapper curveWrapper = selection.curveWrapper;
                int groupId = selection.curveWrapper.groupId;
                if ((groupId != -1) && !selection.semiSelected)
                {
                    list.Add(selection);
                    foreach (CurveWrapper wrapper2 in this.m_AnimationCurves)
                    {
                        if ((wrapper2.groupId == groupId) && (wrapper2 != curveWrapper))
                        {
                            CurveSelection item = new CurveSelection(wrapper2.id, this, selection.key) {
                                semiSelected = true
                            };
                            list.Add(item);
                        }
                    }
                }
                else
                {
                    list.Add(selection);
                }
            }
            list.Sort();
            int num3 = 0;
            while (num3 < (list.Count - 1))
            {
                CurveSelection selection3 = list[num3];
                CurveSelection selection4 = list[num3 + 1];
                if ((selection3.curveID == selection4.curveID) && (selection3.key == selection4.key))
                {
                    if (!selection3.semiSelected || !selection4.semiSelected)
                    {
                        selection3.semiSelected = false;
                    }
                    list.RemoveAt(num3 + 1);
                }
                else
                {
                    num3++;
                }
            }
            this.m_Selection = list;
        }

        public void RecalculateBounds()
        {
            this.m_Bounds = this.m_DefaultBounds;
            if ((this.animationCurves != null) && (((base.hRangeMin == float.NegativeInfinity) || (base.hRangeMax == float.PositiveInfinity)) || ((base.vRangeMin == float.NegativeInfinity) || (base.vRangeMax == float.PositiveInfinity))))
            {
                bool flag = false;
                foreach (CurveWrapper wrapper in this.animationCurves)
                {
                    if (!wrapper.hidden && (wrapper.curve.length != 0))
                    {
                        if (!flag)
                        {
                            this.m_Bounds = wrapper.renderer.GetBounds();
                            flag = true;
                        }
                        else
                        {
                            this.m_Bounds.Encapsulate(wrapper.renderer.GetBounds());
                        }
                    }
                }
            }
            if (base.hRangeMin != float.NegativeInfinity)
            {
                this.m_Bounds.min = new Vector3(base.hRangeMin, this.m_Bounds.min.y, this.m_Bounds.min.z);
            }
            if (base.hRangeMax != float.PositiveInfinity)
            {
                this.m_Bounds.max = new Vector3(base.hRangeMax, this.m_Bounds.max.y, this.m_Bounds.max.z);
            }
            if (base.vRangeMin != float.NegativeInfinity)
            {
                this.m_Bounds.min = new Vector3(this.m_Bounds.min.x, base.vRangeMin, this.m_Bounds.min.z);
            }
            if (base.vRangeMax != float.PositiveInfinity)
            {
                this.m_Bounds.max = new Vector3(this.m_Bounds.max.y, base.vRangeMax, this.m_Bounds.max.z);
            }
            float x = Mathf.Max(this.m_Bounds.size.x, 0.1f);
            this.m_Bounds.size = new Vector3(x, Mathf.Max(this.m_Bounds.size.y, 0.1f), 0f);
        }

        internal void RemoveSelection(CurveSelection curveSelection)
        {
            this.m_Selection.Remove(curveSelection);
            this.lastSelected = null;
        }

        private void ResetDragging()
        {
            if (this.m_DraggingCurveOrRegion != null)
            {
                this.selectedCurves = this.preCurveDragSelection;
                this.preCurveDragSelection = null;
            }
            GUIUtility.hotControl = 0;
            this.m_DraggingKey = null;
            this.m_DraggingCurveOrRegion = null;
            this.m_MoveCoord = Vector2.zero;
            this.m_AxisLock = AxisLock.None;
        }

        public void SelectAll()
        {
            int capacity = 0;
            foreach (CurveWrapper wrapper in this.m_AnimationCurves)
            {
                if (!wrapper.hidden)
                {
                    capacity += wrapper.curve.length;
                }
            }
            List<CurveSelection> list = new List<CurveSelection>(capacity);
            foreach (CurveWrapper wrapper2 in this.m_AnimationCurves)
            {
                wrapper2.selected = CurveWrapper.SelectionMode.Selected;
                for (int i = 0; i < wrapper2.curve.length; i++)
                {
                    list.Add(new CurveSelection(wrapper2.id, this, i));
                }
            }
            this.selectedCurves = list;
        }

        public void SelectNone()
        {
            this.ClearSelection();
            foreach (CurveWrapper wrapper in this.m_AnimationCurves)
            {
                wrapper.selected = CurveWrapper.SelectionMode.None;
            }
        }

        private void SelectPoints()
        {
            int controlID = GUIUtility.GetControlID(0xdb218, FocusType.Passive);
            Event current = Event.current;
            bool shift = current.shift;
            bool actionKey = EditorGUI.actionKey;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                {
                    if ((current.clickCount != 2) || (current.button != 0))
                    {
                        if (current.button == 0)
                        {
                            CurveSelection item = this.FindNearest();
                            if ((item == null) || item.semiSelected)
                            {
                                Vector2 zero = Vector2.zero;
                                CurveWrapper[] curves = null;
                                bool flag3 = this.HandleCurveAndRegionMoveToFrontOnMouseDown(ref zero, ref curves);
                                if ((!shift && !actionKey) && !flag3)
                                {
                                    this.SelectNone();
                                }
                                GUIUtility.hotControl = controlID;
                                this.s_EndMouseDragPosition = this.s_StartMouseDragPosition = current.mousePosition;
                                this.s_PickMode = PickMode.Click;
                            }
                            else
                            {
                                this.MoveCurveToFront(item.curveID);
                                this.activeTime = item.keyframe.time;
                                this.s_StartKeyDragPosition = new Vector2(item.keyframe.time, item.keyframe.value);
                                if (shift)
                                {
                                    if ((this.lastSelected == null) || (item.curveID != this.lastSelected.curveID))
                                    {
                                        if (!this.selectedCurves.Contains(item))
                                        {
                                            this.AddSelection(item);
                                        }
                                    }
                                    else
                                    {
                                        <SelectPoints>c__AnonStorey45 storey3 = new <SelectPoints>c__AnonStorey45 {
                                            rangeCurveID = item.curveID
                                        };
                                        int num3 = Mathf.Min(this.lastSelected.key, item.key);
                                        int num4 = Mathf.Max(this.lastSelected.key, item.key);
                                        <SelectPoints>c__AnonStorey46 storey4 = new <SelectPoints>c__AnonStorey46 {
                                            <>f__ref$69 = storey3,
                                            keyIndex = num3
                                        };
                                        while (storey4.keyIndex <= num4)
                                        {
                                            if (!this.selectedCurves.Any<CurveSelection>(new Func<CurveSelection, bool>(storey4.<>m__76)))
                                            {
                                                CurveSelection curveSelection = new CurveSelection(storey3.rangeCurveID, this, storey4.keyIndex);
                                                this.AddSelection(curveSelection);
                                            }
                                            storey4.keyIndex++;
                                        }
                                    }
                                    Event.current.Use();
                                }
                                else if (actionKey)
                                {
                                    if (!this.selectedCurves.Contains(item))
                                    {
                                        this.AddSelection(item);
                                    }
                                    else
                                    {
                                        this.RemoveSelection(item);
                                    }
                                    Event.current.Use();
                                }
                                else if (!this.selectedCurves.Contains(item))
                                {
                                    this.ClearSelection();
                                    this.AddSelection(item);
                                }
                            }
                            GUI.changed = true;
                            HandleUtility.Repaint();
                        }
                        break;
                    }
                    <SelectPoints>c__AnonStorey43 storey = new <SelectPoints>c__AnonStorey43 {
                        selectedPoint = this.FindNearest()
                    };
                    if (storey.selectedPoint == null)
                    {
                        this.CreateKeyFromClick(base.mousePositionInDrawing);
                    }
                    else
                    {
                        if (!shift)
                        {
                            this.ClearSelection();
                        }
                        <SelectPoints>c__AnonStorey44 storey2 = new <SelectPoints>c__AnonStorey44 {
                            <>f__ref$67 = storey,
                            keyIndex = 0
                        };
                        while (storey2.keyIndex < storey.selectedPoint.curve.keys.Length)
                        {
                            if (!this.selectedCurves.Any<CurveSelection>(new Func<CurveSelection, bool>(storey2.<>m__75)))
                            {
                                CurveSelection selection = new CurveSelection(storey.selectedPoint.curveID, this, storey2.keyIndex);
                                this.AddSelection(selection);
                            }
                            storey2.keyIndex++;
                        }
                    }
                    current.Use();
                    break;
                }
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        this.s_PickMode = PickMode.None;
                        Event.current.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        this.s_EndMouseDragPosition = current.mousePosition;
                        if (this.s_PickMode != PickMode.Click)
                        {
                            Rect rect2 = EditorGUIExt.FromToRect(this.s_StartMouseDragPosition, current.mousePosition);
                            List<CurveSelection> list = new List<CurveSelection>(this.s_SelectionBackup);
                            foreach (CurveWrapper wrapper in this.m_AnimationCurves)
                            {
                                if (!wrapper.readOnly && !wrapper.hidden)
                                {
                                    int keyIndex = 0;
                                    foreach (Keyframe keyframe in wrapper.curve.keys)
                                    {
                                        if (rect2.Contains(this.GetGUIPoint((Vector3) new Vector2(keyframe.time, keyframe.value))))
                                        {
                                            list.Add(new CurveSelection(wrapper.id, this, keyIndex));
                                            this.MoveCurveToFront(wrapper.id);
                                        }
                                        keyIndex++;
                                    }
                                }
                            }
                            this.selectedCurves = list;
                            GUI.changed = true;
                        }
                        else
                        {
                            this.s_PickMode = PickMode.Marquee;
                            if (!shift)
                            {
                                this.s_SelectionBackup = new List<CurveSelection>();
                            }
                            else
                            {
                                this.s_SelectionBackup = new List<CurveSelection>(this.selectedCurves);
                            }
                        }
                        current.Use();
                    }
                    break;

                case EventType.Layout:
                    HandleUtility.AddDefaultControl(controlID);
                    break;

                case EventType.ContextClick:
                {
                    Vector2 vector;
                    Rect drawRect = base.drawRect;
                    float num8 = 0f;
                    drawRect.y = num8;
                    drawRect.x = num8;
                    if (drawRect.Contains(Event.current.mousePosition) && (this.GetCurveAtPosition(base.mousePositionInDrawing, out vector) >= 0))
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Add Key"), false, new GenericMenu.MenuFunction2(this.CreateKeyFromClick), base.mousePositionInDrawing);
                        menu.ShowAsContext();
                        Event.current.Use();
                    }
                    break;
                }
            }
            if (this.s_PickMode == PickMode.Marquee)
            {
                GUI.Label(EditorGUIExt.FromToRect(this.s_StartMouseDragPosition, this.s_EndMouseDragPosition), GUIContent.none, this.ms_Styles.selectionRect);
            }
        }

        private void SetAxisUiScalars(Vector2 newScalars, List<CurveWrapper> curvesInSameSpace)
        {
            foreach (CurveWrapper wrapper in curvesInSameSpace)
            {
                Vector2 newAxisScalars = wrapper.getAxisUiScalarsCallback();
                if (newScalars.x >= 0f)
                {
                    newAxisScalars.x = newScalars.x;
                }
                if (newScalars.y >= 0f)
                {
                    newAxisScalars.y = newScalars.y;
                }
                if (wrapper.setAxisUiScalarsCallback != null)
                {
                    wrapper.setAxisUiScalarsCallback(newAxisScalars);
                }
            }
        }

        private void SetSelectedKeyPositions(Vector2 newPosition, bool updateTime, bool updateValue)
        {
            <SetSelectedKeyPositions>c__AnonStorey42 storey = new <SetSelectedKeyPositions>c__AnonStorey42 {
                updateTime = updateTime,
                newPosition = newPosition,
                updateValue = updateValue,
                <>f__this = this
            };
            this.UpdateCurvesFromPoints(new Action<SavedCurve.SavedKeyFrame, SavedCurve>(storey.<>m__74));
        }

        private void SetupKeyOrCurveDragging(Vector2 timeValue, CurveWrapper cw, int id, Vector2 mousePos)
        {
            this.m_DraggedCoord = timeValue;
            this.m_DraggingKey = cw;
            GUIUtility.hotControl = id;
            this.MakeCurveBackups();
            this.activeTime = timeValue.x;
            this.s_StartMouseDragPosition = mousePos;
            this.s_StartClickedTime = timeValue.x;
        }

        private bool ShouldCurveHaveFocus(int indexIntoDrawOrder, CurveWrapper cw1, CurveWrapper cw2)
        {
            bool flag = false;
            if (indexIntoDrawOrder == (this.m_DrawOrder.Count - 1))
            {
                return true;
            }
            if (this.hasSelection)
            {
                flag = this.IsCurveSelected(cw1) || this.IsCurveSelected(cw2);
            }
            return flag;
        }

        private float SnapTime(float t)
        {
            if (EditorGUI.actionKey)
            {
                int levelWithMinSeparation = base.hTicks.GetLevelWithMinSeparation(5f);
                float periodOfLevel = base.hTicks.GetPeriodOfLevel(levelWithMinSeparation);
                t = Mathf.Round(t / periodOfLevel) * periodOfLevel;
                return t;
            }
            if (this.invSnap != 0f)
            {
                t = Mathf.Round(t * this.invSnap) / this.invSnap;
            }
            return t;
        }

        private float SnapValue(float v)
        {
            if (EditorGUI.actionKey)
            {
                int levelWithMinSeparation = base.vTicks.GetLevelWithMinSeparation(5f);
                float periodOfLevel = base.vTicks.GetPeriodOfLevel(levelWithMinSeparation);
                v = Mathf.Round(v / periodOfLevel) * periodOfLevel;
            }
            return v;
        }

        private void StartEditingSelectedPoints()
        {
            if (<>f__am$cache29 == null)
            {
                <>f__am$cache29 = x => x.keyframe.time;
            }
            float num = this.selectedCurves.Min<CurveSelection>(<>f__am$cache29);
            if (<>f__am$cache2A == null)
            {
                <>f__am$cache2A = x => x.keyframe.time;
            }
            float num2 = this.selectedCurves.Max<CurveSelection>(<>f__am$cache2A);
            if (<>f__am$cache2B == null)
            {
                <>f__am$cache2B = x => x.keyframe.value;
            }
            float num3 = this.selectedCurves.Min<CurveSelection>(<>f__am$cache2B);
            if (<>f__am$cache2C == null)
            {
                <>f__am$cache2C = x => x.keyframe.value;
            }
            float num4 = this.selectedCurves.Max<CurveSelection>(<>f__am$cache2C);
            Vector2 fieldPosition = (Vector2) (new Vector2(num + num2, num3 + num4) * 0.5f);
            this.StartEditingSelectedPoints(fieldPosition);
        }

        private void StartEditingSelectedPoints(Vector2 fieldPosition)
        {
            this.pointEditingFieldPosition = fieldPosition;
            this.focusedPointField = "pointValueField";
            this.timeWasEdited = this.valueWasEdited = false;
            this.MakeCurveBackups();
            this.editingPoints = true;
        }

        private void StartEditingSelectedPointsContext(object fieldPosition)
        {
            this.StartEditingSelectedPoints((Vector2) fieldPosition);
        }

        private void SyncDrawOrder()
        {
            if (this.m_DrawOrder.Count == 0)
            {
                if (<>f__am$cache26 == null)
                {
                    <>f__am$cache26 = cw => cw.id;
                }
                this.m_DrawOrder = this.m_AnimationCurves.Select<CurveWrapper, int>(<>f__am$cache26).ToList<int>();
            }
            else
            {
                List<int> list = new List<int>(this.m_AnimationCurves.Length);
                for (int i = 0; i < this.m_DrawOrder.Count; i++)
                {
                    int item = this.m_DrawOrder[i];
                    for (int j = 0; j < this.m_AnimationCurves.Length; j++)
                    {
                        if (this.m_AnimationCurves[j].id == item)
                        {
                            list.Add(item);
                            break;
                        }
                    }
                }
                this.m_DrawOrder = list;
                if (this.m_DrawOrder.Count != this.m_AnimationCurves.Length)
                {
                    for (int k = 0; k < this.m_AnimationCurves.Length; k++)
                    {
                        int id = this.m_AnimationCurves[k].id;
                        bool flag = false;
                        for (int m = 0; m < this.m_DrawOrder.Count; m++)
                        {
                            if (this.m_DrawOrder[m] == id)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            this.m_DrawOrder.Add(id);
                        }
                    }
                    if (this.m_DrawOrder.Count != this.m_AnimationCurves.Length)
                    {
                        if (<>f__am$cache27 == null)
                        {
                            <>f__am$cache27 = cw => cw.id;
                        }
                        this.m_DrawOrder = this.m_AnimationCurves.Select<CurveWrapper, int>(<>f__am$cache27).ToList<int>();
                    }
                }
            }
        }

        private void SyncSelection()
        {
            this.Init();
            List<CurveSelection> list = new List<CurveSelection>(this.m_Selection.Count);
            foreach (CurveSelection selection in this.m_Selection)
            {
                CurveWrapper curveWrapper = selection.curveWrapper;
                if ((curveWrapper != null) && (!curveWrapper.hidden || (curveWrapper.groupId != -1)))
                {
                    curveWrapper.selected = CurveWrapper.SelectionMode.Selected;
                    list.Add(selection);
                }
            }
            this.m_Selection = list;
            this.RecalculateBounds();
        }

        public void TimeRangeSelectTo(float time)
        {
            if (!this.s_TimeRangeSelectionActive)
            {
                Debug.LogError("TimeRangeSelectTo can only be called after BeginTimeRangeSelection");
            }
            else
            {
                this.s_TimeRangeSelectionEnd = time;
                List<CurveSelection> list = new List<CurveSelection>(this.s_SelectionBackup);
                float num = Mathf.Min(this.s_TimeRangeSelectionStart, this.s_TimeRangeSelectionEnd);
                float num2 = Mathf.Max(this.s_TimeRangeSelectionStart, this.s_TimeRangeSelectionEnd);
                foreach (CurveWrapper wrapper in this.m_AnimationCurves)
                {
                    if (!wrapper.readOnly && !wrapper.hidden)
                    {
                        int keyIndex = 0;
                        foreach (Keyframe keyframe in wrapper.curve.keys)
                        {
                            if ((keyframe.time >= num) && (keyframe.time < num2))
                            {
                                list.Add(new CurveSelection(wrapper.id, this, keyIndex));
                            }
                            keyIndex++;
                        }
                    }
                }
                this.selectedCurves = list;
                this.RecalcSecondarySelection();
                this.RecalcCurveSelection();
            }
        }

        private void TranslateSelectedKeys(Vector2 movement)
        {
            <TranslateSelectedKeys>c__AnonStorey41 storey = new <TranslateSelectedKeys>c__AnonStorey41 {
                movement = movement,
                <>f__this = this
            };
            this.UpdateCurvesFromPoints(new Action<SavedCurve.SavedKeyFrame, SavedCurve>(storey.<>m__73));
        }

        private void UndoRedoPerformed()
        {
            this.SelectNone();
        }

        public void UpdateCurves(List<int> curveIds, string undoText)
        {
            foreach (int num in curveIds)
            {
                this.GetCurveFromID(num).changed = true;
            }
            if (this.curvesUpdated != null)
            {
                this.curvesUpdated();
            }
        }

        public void UpdateCurves(List<ChangedCurve> curve, string undoText)
        {
        }

        private void UpdateCurvesFromPoints(Action<SavedCurve.SavedKeyFrame, SavedCurve> action)
        {
            List<CurveSelection> list = new List<CurveSelection>();
            foreach (SavedCurve curve in this.m_CurveBackups)
            {
                List<SavedCurve.SavedKeyFrame> list2 = new List<SavedCurve.SavedKeyFrame>(curve.keys.Count);
                for (int i = 0; i != curve.keys.Count; i++)
                {
                    SavedCurve.SavedKeyFrame frame = curve.keys[i];
                    if (frame.selected != CurveWrapper.SelectionMode.None)
                    {
                        frame = new SavedCurve.SavedKeyFrame(frame.key, frame.selected);
                        action(frame, curve);
                        for (int k = list2.Count - 1; k >= 0; k--)
                        {
                            if (Mathf.Abs((float) (list2[k].key.time - frame.key.time)) < 1E-05f)
                            {
                                list2.RemoveAt(k);
                            }
                        }
                    }
                    list2.Add(new SavedCurve.SavedKeyFrame(frame.key, frame.selected));
                }
                list2.Sort();
                Keyframe[] keyframeArray = new Keyframe[list2.Count];
                for (int j = 0; j < list2.Count; j++)
                {
                    SavedCurve.SavedKeyFrame frame2 = list2[j];
                    keyframeArray[j] = frame2.key;
                    if (frame2.selected != CurveWrapper.SelectionMode.None)
                    {
                        CurveSelection item = new CurveSelection(curve.curveId, this, j);
                        if (frame2.selected == CurveWrapper.SelectionMode.SemiSelected)
                        {
                            item.semiSelected = true;
                        }
                        list.Add(item);
                    }
                }
                this.selectedCurves = list;
                CurveWrapper curveFromID = this.GetCurveFromID(curve.curveId);
                curveFromID.curve.keys = keyframeArray;
                curveFromID.changed = true;
            }
            this.UpdateTangentsFromSelection();
        }

        private void UpdateTangentsFromSelection()
        {
            foreach (CurveSelection selection in this.selectedCurves)
            {
                CurveUtility.UpdateTangentsFromModeSurrounding(selection.curveWrapper.curve, selection.key);
            }
        }

        private void ValidateCurveList()
        {
            for (int i = 0; i < this.m_AnimationCurves.Length; i++)
            {
                CurveWrapper wrapper = this.m_AnimationCurves[i];
                int regionId = wrapper.regionId;
                if (regionId >= 0)
                {
                    if (i == (this.m_AnimationCurves.Length - 1))
                    {
                        Debug.LogError("Region has only one curve last! Regions should be added as two curves after each other with same regionId");
                        return;
                    }
                    CurveWrapper wrapper2 = this.m_AnimationCurves[++i];
                    int num3 = wrapper2.regionId;
                    if (regionId != num3)
                    {
                        Debug.LogError(string.Concat(new object[] { "Regions should be added as two curves after each other with same regionId: ", regionId, " != ", num3 }));
                        return;
                    }
                }
            }
            if (this.m_DrawOrder.Count != this.m_AnimationCurves.Length)
            {
                Debug.LogError(string.Concat(new object[] { "DrawOrder and AnimationCurves mismatch: DrawOrder ", this.m_DrawOrder.Count, ", AnimationCurves: ", this.m_AnimationCurves.Length }));
            }
            else
            {
                int count = this.m_DrawOrder.Count;
                for (int j = 0; j < count; j++)
                {
                    int id = this.m_DrawOrder[j];
                    int num7 = this.getCurveWrapperById(id).regionId;
                    if (num7 >= 0)
                    {
                        if (j == (count - 1))
                        {
                            Debug.LogError("Region has only one curve last! Regions should be added as two curves after each other with same regionId");
                            return;
                        }
                        int num8 = this.m_DrawOrder[++j];
                        int num9 = this.getCurveWrapperById(num8).regionId;
                        if (num7 != num9)
                        {
                            Debug.LogError(string.Concat(new object[] { "DrawOrder: Regions not added correctly after each other. RegionIds: ", num7, " , ", num9 }));
                            return;
                        }
                    }
                }
            }
        }

        public float activeTime
        {
            set
            {
                if ((this.m_PlayHead != null) && !this.m_PlayHead.playing)
                {
                    this.m_PlayHead.currentTime = value;
                }
            }
        }

        public CurveWrapper[] animationCurves
        {
            get
            {
                if (this.m_AnimationCurves == null)
                {
                    this.m_AnimationCurves = new CurveWrapper[0];
                }
                return this.m_AnimationCurves;
            }
            set
            {
                if (this.m_AnimationCurves == null)
                {
                    this.m_AnimationCurves = new CurveWrapper[0];
                }
                this.m_AnimationCurves = value;
                for (int i = 0; i < this.m_AnimationCurves.Length; i++)
                {
                    this.m_AnimationCurves[i].listIndex = i;
                }
                this.SyncDrawOrder();
                this.SyncSelection();
                this.ValidateCurveList();
            }
        }

        public override Bounds drawingBounds
        {
            get
            {
                return this.m_Bounds;
            }
        }

        private bool editingPoints { get; set; }

        public bool hasSelection
        {
            get
            {
                return (this.m_Selection.Count != 0);
            }
        }

        internal List<CurveSelection> selectedCurves
        {
            get
            {
                return this.m_Selection;
            }
            set
            {
                this.m_Selection = value;
                this.lastSelected = null;
            }
        }

        public Color tangentColor
        {
            get
            {
                return this.m_TangentColor;
            }
            set
            {
                this.m_TangentColor = value;
            }
        }

        [CompilerGenerated]
        private sealed class <PointFieldForSelection>c__AnonStorey47
        {
            internal CurveEditor <>f__this;
            internal Func<CurveSelection, float> memberGetter;

            internal bool <>m__7D(CurveSelection x)
            {
                return (this.memberGetter(x) == this.memberGetter(this.<>f__this.selectedCurves[0]));
            }
        }

        [CompilerGenerated]
        private sealed class <SelectPoints>c__AnonStorey43
        {
            internal CurveSelection selectedPoint;
        }

        [CompilerGenerated]
        private sealed class <SelectPoints>c__AnonStorey44
        {
            internal CurveEditor.<SelectPoints>c__AnonStorey43 <>f__ref$67;
            internal int keyIndex;

            internal bool <>m__75(CurveSelection x)
            {
                return ((x.curveID == this.<>f__ref$67.selectedPoint.curveID) && (x.key == this.keyIndex));
            }
        }

        [CompilerGenerated]
        private sealed class <SelectPoints>c__AnonStorey45
        {
            internal int rangeCurveID;
        }

        [CompilerGenerated]
        private sealed class <SelectPoints>c__AnonStorey46
        {
            internal CurveEditor.<SelectPoints>c__AnonStorey45 <>f__ref$69;
            internal int keyIndex;

            internal bool <>m__76(CurveSelection x)
            {
                return ((x.curveID == this.<>f__ref$69.rangeCurveID) && (x.key == this.keyIndex));
            }
        }

        [CompilerGenerated]
        private sealed class <SetSelectedKeyPositions>c__AnonStorey42
        {
            internal CurveEditor <>f__this;
            internal Vector2 newPosition;
            internal bool updateTime;
            internal bool updateValue;

            internal void <>m__74(CurveEditor.SavedCurve.SavedKeyFrame keyframe, CurveEditor.SavedCurve curve)
            {
                if (this.updateTime)
                {
                    keyframe.key.time = Mathf.Max(this.newPosition.x, 0f);
                }
                if (this.updateValue)
                {
                    keyframe.key.value = this.<>f__this.ClampVerticalValue(this.newPosition.y, curve.curveId);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <TranslateSelectedKeys>c__AnonStorey41
        {
            internal CurveEditor <>f__this;
            internal Vector2 movement;

            internal void <>m__73(CurveEditor.SavedCurve.SavedKeyFrame keyframe, CurveEditor.SavedCurve curve)
            {
                keyframe.key.time = Mathf.Clamp(keyframe.key.time + this.movement.x, this.<>f__this.hRangeMin, this.<>f__this.hRangeMax);
                if (keyframe.selected == CurveWrapper.SelectionMode.Selected)
                {
                    keyframe.key.value = this.<>f__this.ClampVerticalValue(keyframe.key.value + this.movement.y, curve.curveId);
                }
            }
        }

        private enum AxisLock
        {
            None,
            X,
            Y
        }

        public delegate void CallbackFunction();

        [StructLayout(LayoutKind.Sequential)]
        private struct KeyFrameCopy
        {
            public float time;
            public float value;
            public float inTangent;
            public float outTangent;
            public int idx;
            public int selectionIdx;
            public KeyFrameCopy(int idx, int selectionIdx, Keyframe source)
            {
                this.idx = idx;
                this.selectionIdx = selectionIdx;
                this.time = source.time;
                this.value = source.value;
                this.inTangent = source.inTangent;
                this.outTangent = source.outTangent;
            }
        }

        internal enum PickMode
        {
            None,
            Click,
            Marquee
        }

        private class SavedCurve
        {
            public int curveId;
            public List<SavedKeyFrame> keys;

            public class SavedKeyFrame : IComparable
            {
                public Keyframe key;
                public CurveWrapper.SelectionMode selected;

                public SavedKeyFrame(Keyframe key, CurveWrapper.SelectionMode selected)
                {
                    this.key = key;
                    this.selected = selected;
                }

                public int CompareTo(object _other)
                {
                    CurveEditor.SavedCurve.SavedKeyFrame frame = (CurveEditor.SavedCurve.SavedKeyFrame) _other;
                    float num = this.key.time - frame.key.time;
                    return ((num >= 0f) ? ((num <= 0f) ? 0 : 1) : -1);
                }
            }
        }

        internal class Styles
        {
            public GUIStyle axisLabelNumberField = new GUIStyle(EditorStyles.miniTextField);
            public GUIStyle dragLabel = "ProfilerBadge";
            public GUIStyle labelTickMarksX;
            public GUIStyle labelTickMarksY = "CurveEditorLabelTickMarks";
            public GUIStyle none = new GUIStyle();
            public Texture2D pointIcon = EditorGUIUtility.LoadIcon("curvekeyframe");
            public Texture2D pointIconSelected = EditorGUIUtility.LoadIcon("curvekeyframeselected");
            public Texture2D pointIconSelectedOverlay = EditorGUIUtility.LoadIcon("curvekeyframeselectedoverlay");
            public Texture2D pointIconSemiSelectedOverlay = EditorGUIUtility.LoadIcon("curvekeyframesemiselectedoverlay");
            public GUIStyle selectionRect = "SelectionRect";

            public Styles()
            {
                this.axisLabelNumberField.alignment = TextAnchor.UpperRight;
                this.labelTickMarksY.contentOffset = Vector2.zero;
                this.labelTickMarksX = new GUIStyle(this.labelTickMarksY);
                this.labelTickMarksX.clipping = TextClipping.Overflow;
            }
        }
    }
}

