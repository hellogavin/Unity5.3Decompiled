namespace UnityEditor
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable]
    internal class CurveEditorWindow : EditorWindow
    {
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache9;
        [SerializeField]
        private GUIView delegateView;
        private const int kPresetsHeight = 0x2e;
        private Color m_Color;
        private AnimationCurve m_Curve;
        private CurveEditor m_CurveEditor;
        private CurvePresetsContentsForPopupWindow m_CurvePresets;
        private GUIContent m_GUIContent = new GUIContent();
        internal static Styles ms_Styles;
        private static CurveEditorWindow s_SharedCurveEditor;
        private static GUIContent s_WrapModeMenuIcon = EditorGUIUtility.IconContent("AnimationWrapModeMenu");

        private static Keyframe[] CopyAndScaleCurveKeys(Keyframe[] orgKeys, Rect rect, bool realToNormalized)
        {
            if (((rect.width == 0f) || (rect.height == 0f)) || (float.IsInfinity(rect.width) || float.IsInfinity(rect.height)))
            {
                Debug.LogError("CopyAndScaleCurve: Invalid scale: " + rect);
                return orgKeys;
            }
            Keyframe[] keyframeArray = new Keyframe[orgKeys.Length];
            if (realToNormalized)
            {
                for (int j = 0; j < keyframeArray.Length; j++)
                {
                    keyframeArray[j].time = (orgKeys[j].time - rect.xMin) / rect.width;
                    keyframeArray[j].value = (orgKeys[j].value - rect.yMin) / rect.height;
                }
                return keyframeArray;
            }
            for (int i = 0; i < keyframeArray.Length; i++)
            {
                keyframeArray[i].time = (orgKeys[i].time * rect.width) + rect.xMin;
                keyframeArray[i].value = (orgKeys[i].value * rect.height) + rect.yMin;
            }
            return keyframeArray;
        }

        private void DoUpdateCurve(bool exitGUI)
        {
            if (((this.m_CurveEditor.animationCurves.Length > 0) && (this.m_CurveEditor.animationCurves[0] != null)) && this.m_CurveEditor.animationCurves[0].changed)
            {
                this.m_CurveEditor.animationCurves[0].changed = false;
                this.SendEvent("CurveChanged", exitGUI);
            }
        }

        private void DoWrapperPopups()
        {
            if (((this.m_Curve != null) && (this.m_Curve.length >= 2)) && (this.m_Curve.preWrapMode != WrapMode.Default))
            {
                Color contentColor = GUI.contentColor;
                GUI.contentColor = Color.white;
                Keyframe key = this.m_Curve.keys[0];
                WrapMode oldWrap = (this.m_Curve == null) ? WrapMode.Default : this.m_Curve.preWrapMode;
                oldWrap = this.WrapModeIconPopup(key, oldWrap, -1.5f);
                if ((this.m_Curve != null) && (oldWrap != this.m_Curve.preWrapMode))
                {
                    this.m_Curve.preWrapMode = oldWrap;
                    this.RefreshShownCurves();
                    this.SendEvent("CurveChanged", true);
                }
                Keyframe keyframe2 = this.m_Curve.keys[this.m_Curve.length - 1];
                WrapMode mode2 = (this.m_Curve == null) ? WrapMode.Default : this.m_Curve.postWrapMode;
                mode2 = this.WrapModeIconPopup(keyframe2, mode2, 0.5f);
                if ((this.m_Curve != null) && (mode2 != this.m_Curve.postWrapMode))
                {
                    this.m_Curve.postWrapMode = mode2;
                    this.RefreshShownCurves();
                    this.SendEvent("CurveChanged", true);
                }
                GUI.contentColor = contentColor;
            }
        }

        internal static Keyframe[] GetConstantKeys(float value)
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, value, 0f, 0f), new Keyframe(1f, value, 0f, 0f) };
            SetSmoothEditable(ref keys);
            return keys;
        }

        private Rect GetCurveEditorRect()
        {
            return new Rect(0f, 0f, base.position.width, base.position.height - 46f);
        }

        private CurveWrapper[] GetCurveWrapperArray()
        {
            if (this.m_Curve == null)
            {
                return new CurveWrapper[0];
            }
            CurveWrapper wrapper = new CurveWrapper {
                id = "Curve".GetHashCode(),
                groupId = -1,
                color = this.m_Color,
                hidden = false,
                readOnly = false,
                renderer = new NormalCurveRenderer(this.m_Curve)
            };
            wrapper.renderer.SetWrap(this.m_Curve.preWrapMode, this.m_Curve.postWrapMode);
            return new CurveWrapper[] { wrapper };
        }

        internal static Keyframe[] GetEaseInKeys()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 0f, 0f, 0f), new Keyframe(1f, 1f, 2f, 2f) };
            SetSmoothEditable(ref keys);
            return keys;
        }

        internal static Keyframe[] GetEaseInMirrorKeys()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 1f, -2f, -2f), new Keyframe(1f, 0f, 0f, 0f) };
            SetSmoothEditable(ref keys);
            return keys;
        }

        internal static Keyframe[] GetEaseInOutKeys()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 0f, 0f, 0f), new Keyframe(1f, 1f, 0f, 0f) };
            SetSmoothEditable(ref keys);
            return keys;
        }

        internal static Keyframe[] GetEaseInOutMirrorKeys()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 1f, 0f, 0f), new Keyframe(1f, 0f, 0f, 0f) };
            SetSmoothEditable(ref keys);
            return keys;
        }

        internal static Keyframe[] GetEaseOutKeys()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 0f, 2f, 2f), new Keyframe(1f, 1f, 0f, 0f) };
            SetSmoothEditable(ref keys);
            return keys;
        }

        internal static Keyframe[] GetEaseOutMirrorKeys()
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 1f, 0f, 0f), new Keyframe(1f, 0f, -2f, -2f) };
            SetSmoothEditable(ref keys);
            return keys;
        }

        internal static Keyframe[] GetLinearKeys()
        {
            Keyframe[] keyframeArray = new Keyframe[] { new Keyframe(0f, 0f, 1f, 1f), new Keyframe(1f, 1f, 1f, 1f) };
            for (int i = 0; i < 2; i++)
            {
                CurveUtility.SetKeyBroken(ref keyframeArray[i], false);
                CurveUtility.SetKeyTangentMode(ref keyframeArray[i], 0, TangentMode.Smooth);
                CurveUtility.SetKeyTangentMode(ref keyframeArray[i], 1, TangentMode.Smooth);
            }
            return keyframeArray;
        }

        internal static Keyframe[] GetLinearMirrorKeys()
        {
            Keyframe[] keyframeArray = new Keyframe[] { new Keyframe(0f, 1f, -1f, -1f), new Keyframe(1f, 0f, -1f, -1f) };
            for (int i = 0; i < 2; i++)
            {
                CurveUtility.SetKeyBroken(ref keyframeArray[i], false);
                CurveUtility.SetKeyTangentMode(ref keyframeArray[i], 0, TangentMode.Smooth);
                CurveUtility.SetKeyTangentMode(ref keyframeArray[i], 1, TangentMode.Smooth);
            }
            return keyframeArray;
        }

        private bool GetNormalizationRect(out Rect normalizationRect)
        {
            normalizationRect = new Rect();
            if (this.m_CurveEditor.settings.hasUnboundedRanges)
            {
                return false;
            }
            normalizationRect = new Rect(this.m_CurveEditor.settings.hRangeMin, this.m_CurveEditor.settings.vRangeMin, this.m_CurveEditor.settings.hRangeMax - this.m_CurveEditor.settings.hRangeMin, this.m_CurveEditor.settings.vRangeMax - this.m_CurveEditor.settings.vRangeMin);
            return true;
        }

        private void Init(CurveEditorSettings settings)
        {
            this.m_CurveEditor = new CurveEditor(this.GetCurveEditorRect(), this.GetCurveWrapperArray(), true);
            this.m_CurveEditor.curvesUpdated = new CurveEditor.CallbackFunction(this.UpdateCurve);
            this.m_CurveEditor.scaleWithWindow = true;
            this.m_CurveEditor.margin = 40f;
            if (settings != null)
            {
                this.m_CurveEditor.settings = settings;
            }
            this.m_CurveEditor.settings.hTickLabelOffset = 10f;
            bool horizontally = true;
            bool vertically = true;
            if ((this.m_CurveEditor.settings.hRangeMin != float.NegativeInfinity) && (this.m_CurveEditor.settings.hRangeMax != float.PositiveInfinity))
            {
                this.m_CurveEditor.SetShownHRangeInsideMargins(this.m_CurveEditor.settings.hRangeMin, this.m_CurveEditor.settings.hRangeMax);
                horizontally = false;
            }
            if ((this.m_CurveEditor.settings.vRangeMin != float.NegativeInfinity) && (this.m_CurveEditor.settings.vRangeMax != float.PositiveInfinity))
            {
                this.m_CurveEditor.SetShownVRangeInsideMargins(this.m_CurveEditor.settings.vRangeMin, this.m_CurveEditor.settings.vRangeMax);
                vertically = false;
            }
            this.m_CurveEditor.FrameSelected(horizontally, vertically);
        }

        private void InitCurvePresets()
        {
            if (this.m_CurvePresets == null)
            {
                Action<AnimationCurve> presetSelectedCallback = delegate (AnimationCurve presetCurve) {
                    Rect rect;
                    this.ValidateCurveLibraryTypeAndScale();
                    if (this.GetNormalizationRect(out rect))
                    {
                        bool realToNormalized = false;
                        this.m_Curve.keys = CopyAndScaleCurveKeys(presetCurve.keys, rect, realToNormalized);
                    }
                    else
                    {
                        this.m_Curve.keys = presetCurve.keys;
                    }
                    this.m_Curve.postWrapMode = presetCurve.postWrapMode;
                    this.m_Curve.preWrapMode = presetCurve.preWrapMode;
                    this.m_CurveEditor.SelectNone();
                    this.RefreshShownCurves();
                    this.SendEvent("CurveChanged", true);
                };
                AnimationCurve animCurve = null;
                this.m_CurvePresets = new CurvePresetsContentsForPopupWindow(animCurve, this.curveLibraryType, presetSelectedCallback);
                this.m_CurvePresets.InitIfNeeded();
            }
        }

        private void OnDestroy()
        {
            this.m_CurvePresets.GetPresetLibraryEditor().UnloadUsedLibraries();
        }

        private void OnDisable()
        {
            this.m_CurveEditor.OnDisable();
            if (s_SharedCurveEditor == this)
            {
                s_SharedCurveEditor = null;
            }
            else if (!this.Equals(s_SharedCurveEditor))
            {
                throw new ApplicationException("s_SharedCurveEditor does not equal this");
            }
        }

        private void OnEnable()
        {
            s_SharedCurveEditor = this;
            this.Init(null);
        }

        private void OnGUI()
        {
            bool flag = Event.current.type == EventType.MouseUp;
            if (this.delegateView == null)
            {
                this.m_Curve = null;
            }
            if (ms_Styles == null)
            {
                ms_Styles = new Styles();
            }
            this.m_CurveEditor.rect = this.GetCurveEditorRect();
            this.m_CurveEditor.hRangeLocked = Event.current.shift;
            this.m_CurveEditor.vRangeLocked = EditorGUI.actionKey;
            GUI.changed = false;
            GUI.Label(this.m_CurveEditor.drawRect, GUIContent.none, ms_Styles.curveEditorBackground);
            this.m_CurveEditor.BeginViewGUI();
            this.m_CurveEditor.GridGUI();
            this.m_CurveEditor.CurveGUI();
            this.DoWrapperPopups();
            this.m_CurveEditor.EndViewGUI();
            GUI.Box(new Rect(0f, base.position.height - 46f, base.position.width, 46f), string.Empty, ms_Styles.curveSwatchArea);
            this.m_Color.a *= 0.6f;
            float y = (base.position.height - 46f) + 10.5f;
            this.InitCurvePresets();
            CurvePresetLibrary currentLib = this.m_CurvePresets.GetPresetLibraryEditor().GetCurrentLib();
            if (currentLib != null)
            {
                for (int i = 0; i < currentLib.Count(); i++)
                {
                    Rect position = new Rect(45f + (45f * i), y, 40f, 25f);
                    this.m_GUIContent.tooltip = currentLib.GetName(i);
                    if (GUI.Button(position, this.m_GUIContent, ms_Styles.curveSwatch))
                    {
                        AnimationCurve preset = currentLib.GetPreset(i) as AnimationCurve;
                        this.m_Curve.keys = preset.keys;
                        this.m_Curve.postWrapMode = preset.postWrapMode;
                        this.m_Curve.preWrapMode = preset.preWrapMode;
                        this.m_CurveEditor.SelectNone();
                        this.SendEvent("CurveChanged", true);
                    }
                    if (Event.current.type == EventType.Repaint)
                    {
                        currentLib.Draw(position, i);
                    }
                    if (position.xMax > (base.position.width - 90f))
                    {
                        break;
                    }
                }
            }
            Rect rect2 = new Rect(25f, y + 5f, 20f, 20f);
            this.PresetDropDown(rect2);
            if ((Event.current.type == EventType.Used) && flag)
            {
                this.DoUpdateCurve(false);
                this.SendEvent("CurveChangeCompleted", true);
            }
            else if ((Event.current.type != EventType.Layout) && (Event.current.type != EventType.Repaint))
            {
                this.DoUpdateCurve(true);
            }
        }

        private void PresetDropDown(Rect rect)
        {
            if (EditorGUI.ButtonMouseDown(rect, EditorGUI.GUIContents.titleSettingsIcon, FocusType.Native, EditorStyles.inspectorTitlebarText) && (this.m_Curve != null))
            {
                if (this.m_CurvePresets == null)
                {
                    Debug.LogError("Curve presets error");
                }
                else
                {
                    Rect rect2;
                    this.ValidateCurveLibraryTypeAndScale();
                    AnimationCurve curve = new AnimationCurve();
                    if (this.GetNormalizationRect(out rect2))
                    {
                        bool realToNormalized = true;
                        curve.keys = CopyAndScaleCurveKeys(this.m_Curve.keys, rect2, realToNormalized);
                    }
                    else
                    {
                        curve = new AnimationCurve(this.m_Curve.keys);
                    }
                    curve.postWrapMode = this.m_Curve.postWrapMode;
                    curve.preWrapMode = this.m_Curve.preWrapMode;
                    this.m_CurvePresets.curveToSaveAsPreset = curve;
                    PopupWindow.Show(rect, this.m_CurvePresets);
                }
            }
        }

        private void RefreshShownCurves()
        {
            this.m_CurveEditor.animationCurves = this.GetCurveWrapperArray();
        }

        private void SendEvent(string eventName, bool exitGUI)
        {
            if (this.delegateView != null)
            {
                Event e = EditorGUIUtility.CommandEvent(eventName);
                base.Repaint();
                this.delegateView.SendEvent(e);
                if (exitGUI)
                {
                    GUIUtility.ExitGUI();
                }
            }
            GUI.changed = true;
        }

        internal static void SetSmoothEditable(ref Keyframe[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                CurveUtility.SetKeyBroken(ref keys[i], false);
                CurveUtility.SetKeyTangentMode(ref keys[i], 0, TangentMode.Editable);
                CurveUtility.SetKeyTangentMode(ref keys[i], 1, TangentMode.Editable);
            }
        }

        public void Show(GUIView viewToUpdate, CurveEditorSettings settings)
        {
            this.delegateView = viewToUpdate;
            this.Init(settings);
            base.ShowAuxWindow();
            base.titleContent = new GUIContent("Curve");
            base.minSize = new Vector2(240f, 286f);
            base.maxSize = new Vector2(10000f, 10000f);
        }

        public void UpdateCurve()
        {
            this.DoUpdateCurve(false);
        }

        private void ValidateCurveLibraryTypeAndScale()
        {
            Rect rect;
            if (this.GetNormalizationRect(out rect))
            {
                if (this.curveLibraryType != CurveLibraryType.NormalizedZeroToOne)
                {
                    Debug.LogError("When having a normalize rect we should be using curve library type: NormalizedZeroToOne (normalizationRect: " + rect + ")");
                }
            }
            else if (this.curveLibraryType != CurveLibraryType.Unbounded)
            {
                Debug.LogError("When NOT having a normalize rect we should be using library type: Unbounded");
            }
        }

        private WrapMode WrapModeIconPopup(Keyframe key, WrapMode oldWrap, float hOffset)
        {
            float width = s_WrapModeMenuIcon.image.width;
            Vector3 lhs = new Vector3(key.time, key.value);
            lhs = this.m_CurveEditor.DrawingToViewTransformPoint(lhs);
            Rect position = new Rect(lhs.x + (width * hOffset), lhs.y, width, width);
            WrapModeFixedCurve curve = (WrapModeFixedCurve) oldWrap;
            Enum[] array = Enum.GetValues(typeof(WrapModeFixedCurve)).Cast<Enum>().ToArray<Enum>();
            if (<>f__am$cache9 == null)
            {
                <>f__am$cache9 = x => ObjectNames.NicifyVariableName(x);
            }
            string[] texts = Enum.GetNames(typeof(WrapModeFixedCurve)).Select<string, string>(<>f__am$cache9).ToArray<string>();
            int index = Array.IndexOf<Enum>(array, curve);
            int controlID = GUIUtility.GetControlID("WrapModeIconPopup".GetHashCode(), EditorGUIUtility.native, position);
            int selectedValueForControl = EditorGUI.PopupCallbackInfo.GetSelectedValueForControl(controlID, index);
            GUIContent[] options = EditorGUIUtility.TempContent(texts);
            Event current = Event.current;
            EventType type = current.type;
            switch (type)
            {
                case EventType.KeyDown:
                    if (current.MainActionKeyForControl(controlID))
                    {
                        if (Application.platform == RuntimePlatform.OSXEditor)
                        {
                            position.y = (position.y - (selectedValueForControl * 0x10)) - 19f;
                        }
                        EditorGUI.PopupCallbackInfo.instance = new EditorGUI.PopupCallbackInfo(controlID);
                        EditorUtility.DisplayCustomMenu(position, options, selectedValueForControl, new EditorUtility.SelectMenuItemFunction(EditorGUI.PopupCallbackInfo.instance.SetEnumValueDelegate), null);
                        current.Use();
                    }
                    break;

                case EventType.Repaint:
                    GUIStyle.none.Draw(position, s_WrapModeMenuIcon, controlID, false);
                    break;

                default:
                    if ((type == EventType.MouseDown) && ((current.button == 0) && position.Contains(current.mousePosition)))
                    {
                        if (Application.platform == RuntimePlatform.OSXEditor)
                        {
                            position.y = (position.y - (selectedValueForControl * 0x10)) - 19f;
                        }
                        EditorGUI.PopupCallbackInfo.instance = new EditorGUI.PopupCallbackInfo(controlID);
                        EditorUtility.DisplayCustomMenu(position, options, selectedValueForControl, new EditorUtility.SelectMenuItemFunction(EditorGUI.PopupCallbackInfo.instance.SetEnumValueDelegate), null);
                        GUIUtility.keyboardControl = controlID;
                        current.Use();
                    }
                    break;
            }
            return (WrapMode) array[selectedValueForControl];
        }

        public static Color color
        {
            get
            {
                return instance.m_Color;
            }
            set
            {
                instance.m_Color = value;
                instance.RefreshShownCurves();
            }
        }

        public string currentPresetLibrary
        {
            get
            {
                this.InitCurvePresets();
                return this.m_CurvePresets.currentPresetLibrary;
            }
            set
            {
                this.InitCurvePresets();
                this.m_CurvePresets.currentPresetLibrary = value;
            }
        }

        public static AnimationCurve curve
        {
            get
            {
                return instance.m_Curve;
            }
            set
            {
                if (value == null)
                {
                    instance.m_Curve = null;
                }
                else
                {
                    instance.m_Curve = value;
                    instance.RefreshShownCurves();
                }
            }
        }

        private CurveLibraryType curveLibraryType
        {
            get
            {
                if (this.m_CurveEditor.settings.hasUnboundedRanges)
                {
                    return CurveLibraryType.Unbounded;
                }
                return CurveLibraryType.NormalizedZeroToOne;
            }
        }

        public static CurveEditorWindow instance
        {
            get
            {
                if (s_SharedCurveEditor == null)
                {
                    s_SharedCurveEditor = ScriptableObject.CreateInstance<CurveEditorWindow>();
                }
                return s_SharedCurveEditor;
            }
        }

        public static bool visible
        {
            get
            {
                return (s_SharedCurveEditor != null);
            }
        }

        internal class Styles
        {
            public GUIStyle curveEditorBackground = "PopupCurveEditorBackground";
            public GUIStyle curveSwatch = "PopupCurveEditorSwatch";
            public GUIStyle curveSwatchArea = "PopupCurveSwatchBackground";
            public GUIStyle curveWrapPopup = "PopupCurveDropdown";
            public GUIStyle miniToolbarButton = "MiniToolbarButtonLeft";
            public GUIStyle miniToolbarPopup = "MiniToolbarPopup";
        }
    }
}

