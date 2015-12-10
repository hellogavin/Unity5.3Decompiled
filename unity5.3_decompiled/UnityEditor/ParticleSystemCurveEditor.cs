using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

internal class ParticleSystemCurveEditor
{
    public const float k_PresetsHeight = 30f;
    private List<CurveData> m_AddedCurves;
    private List<Color> m_AvailableColors;
    private Color[] m_Colors;
    private CurveEditor m_CurveEditor;
    private static CurveEditorSettings m_CurveEditorSettings = new CurveEditorSettings();
    private DoubleCurvePresetsContentsForPopupWindow m_DoubleCurvePresets;
    private int m_LastTopMostCurveID = -1;
    internal static Styles s_Styles;

    private void Add(CurveData cd)
    {
        this.m_CurveEditor.SelectNone();
        this.m_AddedCurves.Add(cd);
        this.ContentChanged();
        SessionState.SetVector3(cd.m_UniqueName, new Vector3(cd.m_Color.r, cd.m_Color.g, cd.m_Color.b));
        this.UpdateRangeBasedOnShownCurves();
    }

    public void AddCurve(CurveData curveData)
    {
        this.Add(curveData);
    }

    public void AddCurveDataIfNeeded(string curveName, CurveData curveData)
    {
        Vector3 vector = SessionState.GetVector3(curveName, Vector3.zero);
        if (vector != Vector3.zero)
        {
            Color color = new Color(vector.x, vector.y, vector.z);
            curveData.m_Color = color;
            this.AddCurve(curveData);
            for (int i = 0; i < this.m_AvailableColors.Count; i++)
            {
                if (SameColor(this.m_AvailableColors[i], color))
                {
                    this.m_AvailableColors.RemoveAt(i);
                    break;
                }
            }
        }
    }

    private void ContentChanged()
    {
        this.m_CurveEditor.animationCurves = this.CreateCurveWrapperArray();
        m_CurveEditorSettings.showAxisLabels = this.m_CurveEditor.animationCurves.Length > 0;
    }

    private CurveWrapper CreateCurveWrapper(SerializedProperty curve, int id, int regionId, Color color, bool signedRange, CurveWrapper.GetAxisScalarsCallback getAxisScalarsCallback, CurveWrapper.SetAxisScalarsCallback setAxisScalarsCallback)
    {
        float end = 1f;
        CurveWrapper wrapper = new CurveWrapper {
            id = id,
            regionId = regionId,
            color = color,
            renderer = new NormalCurveRenderer(curve.animationCurveValue)
        };
        wrapper.renderer.SetCustomRange(0f, end);
        wrapper.vRangeMin = !signedRange ? 0f : -1f;
        wrapper.getAxisUiScalarsCallback = getAxisScalarsCallback;
        wrapper.setAxisUiScalarsCallback = setAxisScalarsCallback;
        return wrapper;
    }

    private CurveWrapper[] CreateCurveWrapperArray()
    {
        List<CurveWrapper> list = new List<CurveWrapper>();
        int num = 0;
        for (int i = 0; i < this.m_AddedCurves.Count; i++)
        {
            CurveData data = this.m_AddedCurves[i];
            if (data.m_Visible)
            {
                int regionId = -1;
                if (data.IsRegion())
                {
                    regionId = ++num;
                }
                if (data.m_Max != null)
                {
                    list.Add(this.CreateCurveWrapper(data.m_Max, data.m_MaxId, regionId, data.m_Color, data.m_SignedRange, data.m_GetAxisScalarsCallback, data.m_SetAxisScalarsCallback));
                }
                if (data.m_Min != null)
                {
                    list.Add(this.CreateCurveWrapper(data.m_Min, data.m_MinId, regionId, data.m_Color, data.m_SignedRange, data.m_GetAxisScalarsCallback, data.m_SetAxisScalarsCallback));
                }
            }
        }
        return list.ToArray();
    }

    private DoubleCurve CreateDoubleCurveFromTopMostCurve()
    {
        int num;
        if (this.m_CurveEditor.GetTopMostCurveID(out num))
        {
            for (int i = 0; i < this.m_AddedCurves.Count; i++)
            {
                CurveData data = this.m_AddedCurves[i];
                if ((data.m_MaxId == num) || (data.m_MinId == num))
                {
                    AnimationCurve maxCurve = null;
                    AnimationCurve minCurve = null;
                    if (data.m_Min != null)
                    {
                        minCurve = data.m_Min.animationCurveValue;
                    }
                    if (data.m_Max != null)
                    {
                        maxCurve = data.m_Max.animationCurveValue;
                    }
                    return new DoubleCurve(minCurve, maxCurve, data.m_SignedRange);
                }
            }
        }
        return null;
    }

    private void DoLabelForTopMostCurve(Rect rect)
    {
        int num;
        if ((this.m_CurveEditor.IsDraggingCurveOrRegion() || (this.m_CurveEditor.selectedCurves.Count <= 1)) && this.m_CurveEditor.GetTopMostCurveID(out num))
        {
            for (int i = 0; i < this.m_AddedCurves.Count; i++)
            {
                if ((this.m_AddedCurves[i].m_MaxId == num) || (this.m_AddedCurves[i].m_MinId == num))
                {
                    s_Styles.yAxisHeader.normal.textColor = this.m_AddedCurves[i].m_Color;
                    GUI.Label(rect, this.m_AddedCurves[i].m_DisplayName, s_Styles.yAxisHeader);
                    return;
                }
            }
        }
    }

    private void DoOptimizeCurveButton(Rect rect)
    {
        if (!this.m_CurveEditor.IsDraggingCurveOrRegion())
        {
            Rect position = new Rect((rect.xMax - 10f) - 14f, rect.y + ((rect.height - 14f) * 0.5f), 14f, 14f);
            int num2 = 0;
            List<CurveSelection> selectedCurves = this.m_CurveEditor.selectedCurves;
            if (selectedCurves.Count > 0)
            {
                for (int i = 0; i < selectedCurves.Count; i++)
                {
                    CurveWrapper curveWrapper = selectedCurves[i].curveWrapper;
                    num2 += !AnimationUtility.IsValidPolynomialCurve(curveWrapper.curve) ? 0 : 1;
                }
                if ((selectedCurves.Count != num2) && GUI.Button(position, s_Styles.optimizeCurveText, s_Styles.plus))
                {
                    for (int j = 0; j < selectedCurves.Count; j++)
                    {
                        CurveWrapper wrapper2 = selectedCurves[j].curveWrapper;
                        if (!AnimationUtility.IsValidPolynomialCurve(wrapper2.curve))
                        {
                            AnimationUtility.ConstrainToPolynomialCurve(wrapper2.curve);
                            wrapper2.changed = true;
                        }
                    }
                    this.m_CurveEditor.SelectNone();
                }
            }
            else
            {
                int num5;
                if (this.m_CurveEditor.GetTopMostCurveID(out num5))
                {
                    CurveWrapper wrapper3 = this.m_CurveEditor.getCurveWrapperById(num5);
                    if (!AnimationUtility.IsValidPolynomialCurve(wrapper3.curve) && GUI.Button(position, s_Styles.optimizeCurveText, s_Styles.plus))
                    {
                        AnimationUtility.ConstrainToPolynomialCurve(wrapper3.curve);
                        wrapper3.changed = true;
                    }
                }
            }
        }
    }

    private void DoRemoveSelectedButton(Rect rect)
    {
        if (this.m_CurveEditor.animationCurves.Length != 0)
        {
            float width = 14f;
            Rect position = new Rect(((rect.x + rect.width) - width) - 10f, rect.y + ((rect.height - width) * 0.5f), width, width);
            if (GUI.Button(position, s_Styles.removeCurveText, s_Styles.minus))
            {
                if (this.m_CurveEditor.selectedCurves.Count > 0)
                {
                    this.RemoveSelected();
                }
                else
                {
                    this.RemoveTopMost();
                }
            }
        }
    }

    private int FindIndex(SerializedProperty prop)
    {
        return this.FindIndex(null, prop);
    }

    private int FindIndex(SerializedProperty min, SerializedProperty max)
    {
        if (max != null)
        {
            if (min == null)
            {
                for (int i = 0; i < this.m_AddedCurves.Count; i++)
                {
                    if (this.m_AddedCurves[i].m_Max == max)
                    {
                        return i;
                    }
                }
            }
            else
            {
                for (int j = 0; j < this.m_AddedCurves.Count; j++)
                {
                    if ((this.m_AddedCurves[j].m_Max == max) && (this.m_AddedCurves[j].m_Min == min))
                    {
                        return j;
                    }
                }
            }
        }
        return -1;
    }

    public Color GetAvailableColor()
    {
        if (this.m_AvailableColors.Count == 0)
        {
            this.m_AvailableColors = new List<Color>(this.m_Colors);
        }
        int index = this.m_AvailableColors.Count - 1;
        Color color = this.m_AvailableColors[index];
        this.m_AvailableColors.RemoveAt(index);
        return color;
    }

    public Color GetCurveColor(SerializedProperty max)
    {
        int num = this.FindIndex(max);
        if ((num >= 0) && (num < this.m_AddedCurves.Count))
        {
            return this.m_AddedCurves[num].m_Color;
        }
        return new Color(0.8f, 0.8f, 0.8f, 0.7f);
    }

    public void Init()
    {
        if (this.m_AddedCurves == null)
        {
            this.m_AddedCurves = new List<CurveData>();
            this.m_Colors = new Color[] { new Color(1f, 0.6196079f, 0.1294118f), new Color(0.8745098f, 0.2117647f, 0.5803922f), new Color(0f, 0.6862745f, 1f), new Color(1f, 0.9215686f, 0f), new Color(0.1960784f, 1f, 0.2666667f), new Color(0.9803922f, 0f, 0f) };
            this.m_AvailableColors = new List<Color>(this.m_Colors);
            m_CurveEditorSettings.useFocusColors = true;
            m_CurveEditorSettings.showAxisLabels = false;
            m_CurveEditorSettings.hRangeMin = 0f;
            m_CurveEditorSettings.vRangeMin = 0f;
            m_CurveEditorSettings.vRangeMax = 1f;
            m_CurveEditorSettings.hRangeMax = 1f;
            m_CurveEditorSettings.vSlider = false;
            m_CurveEditorSettings.hSlider = false;
            m_CurveEditorSettings.wrapColor = new Color(0f, 0f, 0f, 0f);
            m_CurveEditorSettings.hTickLabelOffset = 5f;
            m_CurveEditorSettings.allowDraggingCurvesAndRegions = true;
            m_CurveEditorSettings.allowDeleteLastKeyInCurve = false;
            TickStyle style = new TickStyle {
                color = new Color(0f, 0f, 0f, 0.2f),
                distLabel = 30,
                stubs = false,
                centerLabel = true
            };
            m_CurveEditorSettings.hTickStyle = style;
            TickStyle style2 = new TickStyle {
                color = new Color(0f, 0f, 0f, 0.2f),
                distLabel = 20,
                stubs = false,
                centerLabel = true
            };
            m_CurveEditorSettings.vTickStyle = style2;
            this.m_CurveEditor = new CurveEditor(new Rect(0f, 0f, 1000f, 100f), this.CreateCurveWrapperArray(), false);
            this.m_CurveEditor.settings = m_CurveEditorSettings;
            this.m_CurveEditor.leftmargin = 40f;
            float num = 25f;
            this.m_CurveEditor.bottommargin = num;
            this.m_CurveEditor.topmargin = num;
            this.m_CurveEditor.rightmargin = num;
            this.m_CurveEditor.SetShownHRangeInsideMargins(m_CurveEditorSettings.hRangeMin, m_CurveEditorSettings.hRangeMax);
            this.m_CurveEditor.SetShownVRangeInsideMargins(m_CurveEditorSettings.vRangeMin, m_CurveEditorSettings.hRangeMax);
            this.m_CurveEditor.ignoreScrollWheelUntilClicked = false;
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
        }
    }

    private void InitDoubleCurvePresets()
    {
        int num;
        if (this.m_CurveEditor.GetTopMostCurveID(out num) && ((this.m_DoubleCurvePresets == null) || (this.m_LastTopMostCurveID != num)))
        {
            this.m_LastTopMostCurveID = num;
            Action<DoubleCurve> presetSelectedCallback = delegate (DoubleCurve presetDoubleCurve) {
                this.SetTopMostCurve(presetDoubleCurve);
                InternalEditorUtility.RepaintAllViews();
            };
            DoubleCurve doubleCurveToSave = this.CreateDoubleCurveFromTopMostCurve();
            this.m_DoubleCurvePresets = new DoubleCurvePresetsContentsForPopupWindow(doubleCurveToSave, presetSelectedCallback);
            this.m_DoubleCurvePresets.InitIfNeeded();
        }
    }

    public bool IsAdded(SerializedProperty max)
    {
        return (this.FindIndex(null, max) != -1);
    }

    public bool IsAdded(SerializedProperty min, SerializedProperty max)
    {
        return (this.FindIndex(min, max) != -1);
    }

    public void OnDestroy()
    {
        this.m_DoubleCurvePresets.GetPresetLibraryEditor().UnloadUsedLibraries();
    }

    public void OnDisable()
    {
        this.m_CurveEditor.OnDisable();
        Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
    }

    public void OnGUI(Rect rect)
    {
        this.Init();
        if (s_Styles == null)
        {
            s_Styles = new Styles();
        }
        Rect position = new Rect(rect.x, rect.y, rect.width, rect.height - 30f);
        Rect rect3 = new Rect(rect.x, rect.y + position.height, rect.width, 30f);
        GUI.Label(position, GUIContent.none, s_Styles.curveEditorBackground);
        this.m_CurveEditor.rect = position;
        this.m_CurveEditor.OnGUI();
        foreach (CurveWrapper wrapper in this.m_CurveEditor.animationCurves)
        {
            if ((wrapper.getAxisUiScalarsCallback != null) && (wrapper.setAxisUiScalarsCallback != null))
            {
                Vector2 newAxisScalars = wrapper.getAxisUiScalarsCallback();
                if (newAxisScalars.y > 1000000f)
                {
                    newAxisScalars.y = 1000000f;
                    wrapper.setAxisUiScalarsCallback(newAxisScalars);
                }
            }
        }
        this.DoLabelForTopMostCurve(new Rect(rect.x + 4f, rect.y, rect.width, 20f));
        this.DoRemoveSelectedButton(new Rect(position.x, position.y, position.width, 24f));
        this.DoOptimizeCurveButton(rect3);
        rect3.x += 30f;
        rect3.width -= 60f;
        this.PresetCurveButtons(rect3, rect);
        this.SaveChangedCurves();
    }

    private void PresetCurveButtons(Rect position, Rect curveEditorRect)
    {
        if (this.m_CurveEditor.animationCurves.Length != 0)
        {
            this.InitDoubleCurvePresets();
            if (this.m_DoubleCurvePresets != null)
            {
                DoubleCurvePresetLibrary currentLib = this.m_DoubleCurvePresets.GetPresetLibraryEditor().GetCurrentLib();
                int a = (currentLib == null) ? 0 : currentLib.Count();
                int num3 = Mathf.Min(a, 9);
                float width = 30f;
                float height = 15f;
                float num6 = 10f;
                float num7 = (num3 * width) + ((num3 - 1) * num6);
                float num8 = (position.width - num7) * 0.5f;
                float y = (position.height - height) * 0.5f;
                float x = 3f;
                if (num8 > 0f)
                {
                    x = num8;
                }
                this.PresetDropDown(new Rect((x - 20f) + position.x, y + position.y, 16f, 16f));
                GUI.BeginGroup(position);
                Color.white.a *= 0.6f;
                for (int i = 0; i < num3; i++)
                {
                    if (i > 0)
                    {
                        x += num6;
                    }
                    Rect rect = new Rect(x, y, width, height);
                    s_Styles.presetTooltip.tooltip = currentLib.GetName(i);
                    if (GUI.Button(rect, s_Styles.presetTooltip, GUIStyle.none))
                    {
                        DoubleCurve preset = currentLib.GetPreset(i) as DoubleCurve;
                        if (preset != null)
                        {
                            this.SetTopMostCurve(preset);
                            this.m_CurveEditor.ClearSelection();
                        }
                    }
                    if (Event.current.type == EventType.Repaint)
                    {
                        currentLib.Draw(rect, i);
                    }
                    x += width;
                }
                GUI.EndGroup();
            }
        }
    }

    private void PresetDropDown(Rect rect)
    {
        if (EditorGUI.ButtonMouseDown(rect, EditorGUI.GUIContents.titleSettingsIcon, FocusType.Native, EditorStyles.inspectorTitlebarText) && (this.CreateDoubleCurveFromTopMostCurve() != null))
        {
            this.InitDoubleCurvePresets();
            if (this.m_DoubleCurvePresets != null)
            {
                this.m_DoubleCurvePresets.doubleCurveToSave = this.CreateDoubleCurveFromTopMostCurve();
                PopupWindow.Show(rect, this.m_DoubleCurvePresets);
            }
        }
    }

    public void Refresh()
    {
        this.ContentChanged();
        AnimationCurvePreviewCache.ClearCache();
    }

    private bool Remove(int index)
    {
        if ((index >= 0) && (index < this.m_AddedCurves.Count))
        {
            Color item = this.m_AddedCurves[index].m_Color;
            this.m_AvailableColors.Add(item);
            SessionState.EraseVector3(this.m_AddedCurves[index].m_UniqueName);
            this.m_AddedCurves.RemoveAt(index);
            if (this.m_AddedCurves.Count == 0)
            {
                this.m_AvailableColors = new List<Color>(this.m_Colors);
            }
            return true;
        }
        Debug.Log("Invalid index in ParticleSystemCurveEditor::Remove");
        return false;
    }

    private void RemoveAll()
    {
        bool flag = false;
        while (this.m_AddedCurves.Count > 0)
        {
            flag |= this.Remove(0);
        }
        if (flag)
        {
            this.ContentChanged();
            this.UpdateRangeBasedOnShownCurves();
        }
    }

    public void RemoveCurve(SerializedProperty max)
    {
        this.RemoveCurve(null, max);
    }

    public void RemoveCurve(SerializedProperty min, SerializedProperty max)
    {
        if (this.Remove(this.FindIndex(min, max)))
        {
            this.ContentChanged();
            this.UpdateRangeBasedOnShownCurves();
        }
    }

    private void RemoveSelected()
    {
        bool flag = false;
        List<CurveSelection> selectedCurves = this.m_CurveEditor.selectedCurves;
        for (int i = 0; i < selectedCurves.Count; i++)
        {
            int curveID = selectedCurves[i].curveID;
            for (int j = 0; j < this.m_AddedCurves.Count; j++)
            {
                CurveData data = this.m_AddedCurves[j];
                if ((data.m_MaxId == curveID) || (data.m_MinId == curveID))
                {
                    flag |= this.Remove(j);
                    break;
                }
            }
        }
        if (flag)
        {
            this.ContentChanged();
            this.UpdateRangeBasedOnShownCurves();
        }
        this.m_CurveEditor.SelectNone();
    }

    private void RemoveTopMost()
    {
        int num;
        if (this.m_CurveEditor.GetTopMostCurveID(out num))
        {
            for (int i = 0; i < this.m_AddedCurves.Count; i++)
            {
                CurveData data = this.m_AddedCurves[i];
                if ((data.m_MaxId == num) || (data.m_MinId == num))
                {
                    this.Remove(i);
                    this.ContentChanged();
                    this.UpdateRangeBasedOnShownCurves();
                    return;
                }
            }
        }
    }

    private static bool SameColor(Color c1, Color c2)
    {
        return (((Mathf.Abs((float) (c1.r - c2.r)) < 0.01f) && (Mathf.Abs((float) (c1.g - c2.g)) < 0.01f)) && (Mathf.Abs((float) (c1.b - c2.b)) < 0.01f));
    }

    private void SaveChangedCurves()
    {
        CurveWrapper[] animationCurves = this.m_CurveEditor.animationCurves;
        bool flag = false;
        for (int i = 0; i < animationCurves.Length; i++)
        {
            CurveWrapper cw = animationCurves[i];
            if (cw.changed)
            {
                for (int j = 0; j < this.m_AddedCurves.Count; j++)
                {
                    if (this.m_AddedCurves[j].m_MaxId == cw.id)
                    {
                        this.SaveCurve(this.m_AddedCurves[j].m_Max, cw);
                        break;
                    }
                    if (this.m_AddedCurves[j].IsRegion() && (this.m_AddedCurves[j].m_MinId == cw.id))
                    {
                        this.SaveCurve(this.m_AddedCurves[j].m_Min, cw);
                        break;
                    }
                }
                flag = true;
            }
        }
        if (flag)
        {
            AnimationCurvePreviewCache.ClearCache();
            HandleUtility.Repaint();
        }
    }

    private void SaveCurve(SerializedProperty prop, CurveWrapper cw)
    {
        prop.animationCurveValue = cw.curve;
        cw.changed = false;
    }

    private void SetConstantCurve(CurveWrapper cw, float constantValue)
    {
        Keyframe[] keyframeArray = new Keyframe[1];
        keyframeArray[0].time = 0f;
        keyframeArray[0].value = constantValue;
        cw.curve.keys = keyframeArray;
        cw.changed = true;
    }

    private void SetCurve(CurveWrapper cw, AnimationCurve curve)
    {
        Keyframe[] destinationArray = new Keyframe[curve.keys.Length];
        Array.Copy(curve.keys, destinationArray, destinationArray.Length);
        cw.curve.keys = destinationArray;
        cw.changed = true;
    }

    private void SetTopMostCurve(DoubleCurve doubleCurve)
    {
        int num;
        if (this.m_CurveEditor.GetTopMostCurveID(out num))
        {
            for (int i = 0; i < this.m_AddedCurves.Count; i++)
            {
                CurveData data = this.m_AddedCurves[i];
                if ((data.m_MaxId == num) || (data.m_MinId == num))
                {
                    if (doubleCurve.signedRange == data.m_SignedRange)
                    {
                        if (data.m_MaxId > 0)
                        {
                            this.SetCurve(this.m_CurveEditor.GetCurveFromID(data.m_MaxId), doubleCurve.maxCurve);
                        }
                        if (data.m_MinId > 0)
                        {
                            this.SetCurve(this.m_CurveEditor.GetCurveFromID(data.m_MinId), doubleCurve.minCurve);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Cannot assign a curves with different signed range");
                    }
                }
            }
        }
    }

    public void SetVisible(SerializedProperty curveProp, bool visible)
    {
        int num = this.FindIndex(curveProp);
        if (num >= 0)
        {
            this.m_AddedCurves[num].m_Visible = visible;
        }
    }

    private void UndoRedoPerformed()
    {
        this.ContentChanged();
    }

    private void UpdateRangeBasedOnShownCurves()
    {
        bool flag = false;
        for (int i = 0; i < this.m_AddedCurves.Count; i++)
        {
            flag |= this.m_AddedCurves[i].m_SignedRange;
        }
        float num2 = !flag ? 0f : -1f;
        if (num2 != m_CurveEditorSettings.vRangeMin)
        {
            m_CurveEditorSettings.vRangeMin = num2;
            this.m_CurveEditor.settings = m_CurveEditorSettings;
            this.m_CurveEditor.SetShownVRangeInsideMargins(m_CurveEditorSettings.vRangeMin, m_CurveEditorSettings.hRangeMax);
        }
    }

    public class CurveData
    {
        public Color m_Color;
        public GUIContent m_DisplayName;
        public CurveWrapper.GetAxisScalarsCallback m_GetAxisScalarsCallback;
        public SerializedProperty m_Max;
        public int m_MaxId;
        public SerializedProperty m_Min;
        public int m_MinId;
        public CurveWrapper.SetAxisScalarsCallback m_SetAxisScalarsCallback;
        public bool m_SignedRange;
        public string m_UniqueName;
        public bool m_Visible;
        private static int s_IdCounter;

        public CurveData(string name, GUIContent displayName, SerializedProperty min, SerializedProperty max, Color color, bool signedRange, CurveWrapper.GetAxisScalarsCallback getAxisScalars, CurveWrapper.SetAxisScalarsCallback setAxisScalars, bool visible)
        {
            this.m_UniqueName = name;
            this.m_DisplayName = displayName;
            this.m_SignedRange = signedRange;
            this.m_Min = min;
            this.m_Max = max;
            if (this.m_Min != null)
            {
                this.m_MinId = ++s_IdCounter;
            }
            if (this.m_Max != null)
            {
                this.m_MaxId = ++s_IdCounter;
            }
            this.m_Color = color;
            this.m_GetAxisScalarsCallback = getAxisScalars;
            this.m_SetAxisScalarsCallback = setAxisScalars;
            this.m_Visible = visible;
            if ((this.m_Max == null) || (this.m_MaxId == 0))
            {
                Debug.LogError("Max curve should always be valid! (Min curve can be null)");
            }
        }

        public bool IsRegion()
        {
            return (this.m_Min != null);
        }
    }

    internal class Styles
    {
        public GUIStyle curveEditorBackground = "AnimationCurveEditorBackground";
        public GUIContent curveLibraryPopup = new GUIContent(string.Empty, "Open curve library");
        public GUIStyle curveSwatch = "PopupCurveEditorSwatch";
        public GUIStyle curveSwatchArea = "PopupCurveSwatchBackground";
        public GUIStyle minus = "OL Minus";
        public GUIContent optimizeCurveText = new GUIContent(string.Empty, "Click to optimize curve. Optimized curves are defined by having at most 3 keys, with a key at both ends");
        public GUIStyle plus = "OL Plus";
        public GUIContent presetTooltip = new GUIContent();
        public GUIContent removeCurveText = new GUIContent(string.Empty, "Remove selected curve(s)");
        public GUIStyle yAxisHeader = new GUIStyle(ParticleSystemStyles.Get().label);
    }
}

