namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class ColorPicker : EditorWindow
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map17;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map18;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map19;
        private const int kColorBoxHeight = 0xa2;
        private const int kColorBoxSize = 0x20;
        private const int kEyeDropperHeight = 0x5f;
        private const float kFixedWindowWidth = 233f;
        private const float kHDRFieldWidth = 40f;
        private const int kHueRes = 0x40;
        private const float kLDRFieldWidth = 30f;
        private const int kPresetsHeight = 300;
        private const int kSlidersHeight = 0x52;
        [SerializeField]
        private float m_A = 1f;
        [SerializeField]
        private Texture2D m_AlphaTexture;
        [SerializeField]
        private float m_B;
        private Texture2D m_BTexture;
        private float m_BTextureG = -1f;
        private float m_BTextureR = -1f;
        [SerializeField]
        private Color m_Color = Color.black;
        [SerializeField]
        private Texture2D m_ColorBox;
        [SerializeField]
        private ColorBoxMode m_ColorBoxMode = ColorBoxMode.BG_R;
        private string[] m_ColorBoxXAxisLabels = new string[] { "Saturation", "Hue", "Hue", "Blue", "Blue", "Red", string.Empty };
        private string[] m_ColorBoxYAxisLabels = new string[] { "Brightness", "Brightness", "Saturation", "Green", "Red", "Green", string.Empty };
        private string[] m_ColorBoxZAxisLabels = new string[] { "Hue", "Saturation", "Brightness", "Red", "Green", "Blue", string.Empty };
        private PresetLibraryEditor<ColorPresetLibrary> m_ColorLibraryEditor;
        private PresetLibraryEditorState m_ColorLibraryEditorState;
        [SerializeField]
        private Color[] m_Colors;
        [SerializeField]
        private Texture2D m_ColorSlider;
        [NonSerialized]
        private bool m_ColorSliderDirty;
        [SerializeField]
        private float m_ColorSliderSize = 4f;
        [NonSerialized]
        private bool m_ColorSpaceBoxDirty;
        private static readonly ColorPickerHDRConfig m_DefaultHDRConfig = new ColorPickerHDRConfig(0f, 99f, 0.01010101f, 3f);
        [SerializeField]
        private GUIView m_DelegateView;
        [SerializeField]
        private float m_G;
        private Texture2D m_GTexture;
        private float m_GTextureB = -1f;
        private float m_GTextureR = -1f;
        [SerializeField]
        private float m_H;
        [SerializeField]
        private bool m_HDR;
        [SerializeField]
        private ColorPickerHDRConfig m_HDRConfig;
        [SerializeField]
        private HDRValues m_HDRValues = new HDRValues();
        [SerializeField]
        private Texture2D m_HueTexture;
        private float m_HueTextureS = -1f;
        private float m_HueTextureV = -1f;
        [SerializeField]
        private bool m_IsOSColorPicker = EditorPrefs.GetBool("UseOSColorPicker");
        [SerializeField]
        private float m_LastConstant = -1f;
        [SerializeField]
        private Vector2 m_LastConstantValues = new Vector2(-1f, -1f);
        [SerializeField]
        private int m_ModalUndoGroup = -1;
        private float m_OldAlpha = -1f;
        [SerializeField]
        private ColorBoxMode m_OldColorBoxMode;
        [SerializeField]
        private Color m_OriginalColor;
        [SerializeField]
        private float m_R;
        [SerializeField]
        private bool m_resetKeyboardControl;
        [NonSerialized]
        private bool m_RGBHSVSlidersDirty;
        private Texture2D m_RTexture;
        private float m_RTextureB = -1f;
        private float m_RTextureG = -1f;
        [SerializeField]
        private float m_S;
        [SerializeField]
        private Texture2D m_SatTexture;
        private float m_SatTextureH = -1f;
        private float m_SatTextureV = -1f;
        [SerializeField]
        private bool m_ShowAlpha = true;
        [SerializeField]
        private bool m_ShowPresets = true;
        [SerializeField]
        private SliderMode m_SliderMode = SliderMode.HSV;
        [SerializeField]
        private float m_SliderValue;
        [NonSerialized]
        private int m_TextureColorBoxMode = -1;
        [SerializeField]
        private int m_TextureColorSliderMode = -1;
        [SerializeField]
        private ContainerWindow m_TrackingWindow;
        [SerializeField]
        private bool m_UseTonemappingPreview;
        [SerializeField]
        private float m_V;
        [SerializeField]
        private Texture2D m_ValTexture;
        private float m_ValTextureH = -1f;
        private float m_ValTextureS = -1f;
        private static Texture2D s_LeftGradientTexture;
        private static Texture2D s_RightGradientTexture;
        private static ColorPicker s_SharedColorPicker;
        private static int s_Slider2Dhash = "Slider2D".GetHashCode();
        private static Styles styles;

        public ColorPicker()
        {
            base.hideFlags = HideFlags.DontSave;
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.PollOSColorPicker));
            EditorGUIUtility.editingTextField = true;
        }

        private void BrightnessField()
        {
            if (this.m_HDR)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel++;
                Color color = EditorGUILayout.ColorBrightnessField(GUIContent.Temp("Current Brightness"), ColorPicker.color, this.m_HDRConfig.minBrightness, this.m_HDRConfig.maxBrightness, new GUILayoutOption[0]);
                EditorGUI.indentLevel--;
                if (EditorGUI.EndChangeCheck())
                {
                    float maxColorComponent = color.maxColorComponent;
                    if (maxColorComponent > this.m_HDRValues.m_HDRScaleFactor)
                    {
                        this.SetHDRScaleFactor(maxColorComponent);
                    }
                    this.SetNormalizedColor(color.RGBMultiplied((float) (1f / this.m_HDRValues.m_HDRScaleFactor)));
                }
            }
        }

        private static Texture2D CreateGradientTexture(string name, int width, int height, Color leftColor, Color rightColor)
        {
            Texture2D textured = new Texture2D(width, height, TextureFormat.ARGB32, false) {
                name = name,
                hideFlags = HideFlags.HideAndDontSave
            };
            Color[] colors = new Color[width * height];
            for (int i = 0; i < width; i++)
            {
                Color color = Color.Lerp(leftColor, rightColor, ((float) i) / ((float) (width - 1)));
                for (int j = 0; j < height; j++)
                {
                    colors[(j * width) + i] = color;
                }
            }
            textured.SetPixels(colors);
            textured.wrapMode = TextureWrapMode.Clamp;
            textured.Apply();
            return textured;
        }

        private void DoColorSliders()
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            if (GUILayout.Button(styles.sliderCycle, GUIStyle.none, options))
            {
                this.m_SliderMode = (this.m_SliderMode + 1) % ((SliderMode) 2);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(7f);
            switch (this.m_SliderMode)
            {
                case SliderMode.RGB:
                    this.RGBSliders();
                    break;

                case SliderMode.HSV:
                    this.HSVSliders();
                    break;
            }
            if (this.m_ShowAlpha)
            {
                this.m_AlphaTexture = Update1DSlider(this.m_AlphaTexture, 0x20, 0f, 0f, ref this.m_OldAlpha, ref this.m_OldAlpha, 3, false, 1f, -1f, false, this.m_HDRValues.m_TonemappingType);
                float displayScale = !this.m_HDR ? 255f : 1f;
                string formatString = !this.m_HDR ? EditorGUI.kIntFieldFormatString : EditorGUI.kFloatFieldFormatString;
                this.m_A = this.TexturedSlider(this.m_AlphaTexture, "A", this.m_A, 0f, 1f, displayScale, formatString, null);
            }
        }

        private void DoColorSpaceGUI()
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            if (GUILayout.Button(styles.colorCycle, GUIStyle.none, options))
            {
                this.m_OldColorBoxMode = this.m_ColorBoxMode = (this.m_ColorBoxMode + 1) % ColorBoxMode.EyeDropper;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(20f);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Space(7f);
            bool changed = GUI.changed;
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandHeight(false) };
            GUILayout.BeginHorizontal(optionArray2);
            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.MinWidth(64f), GUILayout.MinHeight(64f), GUILayout.MaxWidth(256f), GUILayout.MaxHeight(256f) };
            Rect boxPos = GUILayoutUtility.GetAspectRect(1f, styles.pickerBox, optionArray3);
            EditorGUILayout.Space();
            Rect sliderPos = GUILayoutUtility.GetRect(8f, 32f, 64f, 128f, styles.pickerBox);
            sliderPos.height = boxPos.height;
            GUILayout.EndHorizontal();
            GUI.changed = false;
            switch (this.m_ColorBoxMode)
            {
                case ColorBoxMode.SV_H:
                    this.Slider3D(boxPos, sliderPos, ref this.m_S, ref this.m_V, ref this.m_H, styles.pickerBox, styles.thumb2D, styles.thumbVert);
                    if (GUI.changed)
                    {
                        this.HSVToRGB();
                    }
                    break;

                case ColorBoxMode.HV_S:
                    this.Slider3D(boxPos, sliderPos, ref this.m_H, ref this.m_V, ref this.m_S, styles.pickerBox, styles.thumb2D, styles.thumbVert);
                    if (GUI.changed)
                    {
                        this.HSVToRGB();
                    }
                    break;

                case ColorBoxMode.HS_V:
                    this.Slider3D(boxPos, sliderPos, ref this.m_H, ref this.m_S, ref this.m_V, styles.pickerBox, styles.thumb2D, styles.thumbVert);
                    if (GUI.changed)
                    {
                        this.HSVToRGB();
                    }
                    break;

                case ColorBoxMode.BG_R:
                    this.Slider3D(boxPos, sliderPos, ref this.m_B, ref this.m_G, ref this.m_R, styles.pickerBox, styles.thumb2D, styles.thumbVert);
                    if (GUI.changed)
                    {
                        this.RGBToHSV();
                    }
                    break;

                case ColorBoxMode.BR_G:
                    this.Slider3D(boxPos, sliderPos, ref this.m_B, ref this.m_R, ref this.m_G, styles.pickerBox, styles.thumb2D, styles.thumbVert);
                    if (GUI.changed)
                    {
                        this.RGBToHSV();
                    }
                    break;

                case ColorBoxMode.RG_B:
                    this.Slider3D(boxPos, sliderPos, ref this.m_R, ref this.m_G, ref this.m_B, styles.pickerBox, styles.thumb2D, styles.thumbVert);
                    if (GUI.changed)
                    {
                        this.RGBToHSV();
                    }
                    break;

                case ColorBoxMode.EyeDropper:
                    EyeDropper.DrawPreview(Rect.MinMaxRect(boxPos.x, boxPos.y, sliderPos.xMax, boxPos.yMax));
                    break;
            }
            GUI.changed |= changed;
            GUILayout.Space(5f);
            GUILayout.EndVertical();
            GUILayout.Space(20f);
            GUILayout.EndHorizontal();
        }

        private void DoColorSwatchAndEyedropper()
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(40f), GUILayout.ExpandWidth(false) };
            if (GUILayout.Button(styles.eyeDropper, GUIStyle.none, options))
            {
                EyeDropper.Start(base.m_Parent);
                this.m_ColorBoxMode = ColorBoxMode.EyeDropper;
                GUIUtility.ExitGUI();
            }
            Color color = new Color(this.m_R, this.m_G, this.m_B, this.m_A);
            if (this.m_HDR)
            {
                color = ColorPicker.color;
            }
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            Rect position = GUILayoutUtility.GetRect(20f, 20f, 20f, 20f, styles.colorPickerBox, optionArray2);
            EditorGUIUtility.DrawColorSwatch(position, color, this.m_ShowAlpha, this.m_HDR);
            if (Event.current.type == EventType.Repaint)
            {
                styles.pickerBox.Draw(position, GUIContent.none, false, false, false, false);
            }
            GUILayout.EndHorizontal();
        }

        private void DoHexField(float availableWidth)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            float fieldWidth = EditorGUIUtility.fieldWidth;
            EditorGUIUtility.labelWidth = availableWidth - 85f;
            EditorGUIUtility.fieldWidth = 85f;
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            Color c = EditorGUILayout.HexColorTextField(GUIContent.Temp("Hex Color"), color, this.m_ShowAlpha, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.SetNormalizedColor(c);
                if (this.m_HDR)
                {
                    this.SetHDRScaleFactor(1f);
                }
            }
            EditorGUI.indentLevel--;
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUIUtility.fieldWidth = fieldWidth;
        }

        private void DoPresetsGUI()
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            this.m_ShowPresets = GUILayout.Toggle(this.m_ShowPresets, styles.presetsToggle, styles.foldout, new GUILayoutOption[0]);
            GUILayout.Space(17f);
            GUILayout.EndHorizontal();
            if (this.m_ShowPresets)
            {
                GUILayout.Space(-18f);
                Rect rect = GUILayoutUtility.GetRect(0f, Mathf.Clamp(this.m_ColorLibraryEditor.contentHeight, 20f, 250f));
                this.m_ColorLibraryEditor.OnGUI(rect, color);
            }
        }

        private static Color DoTonemapping(Color col, float exposureAdjustment)
        {
            col.r = PhotographicTonemapping(col.r, exposureAdjustment);
            col.g = PhotographicTonemapping(col.g, exposureAdjustment);
            col.b = PhotographicTonemapping(col.b, exposureAdjustment);
            return col;
        }

        private static void DoTonemapping(Color[] colors, float exposureAdjustment, TonemappingType tonemappingType)
        {
            if (exposureAdjustment >= 0f)
            {
                if (tonemappingType == TonemappingType.Linear)
                {
                    for (int i = 0; i < colors.Length; i++)
                    {
                        colors[i] = colors[i].RGBMultiplied(exposureAdjustment);
                    }
                }
                else
                {
                    for (int j = 0; j < colors.Length; j++)
                    {
                        colors[j] = DoTonemapping(colors[j], exposureAdjustment);
                    }
                }
            }
        }

        private void DrawColorSlider(Rect colorSliderRect, Vector2 constantValues)
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (this.m_ColorBoxMode != this.m_TextureColorSliderMode)
                {
                    int width = 0;
                    int height = 0;
                    width = (int) this.m_ColorSliderSize;
                    if (this.m_ColorBoxMode == ColorBoxMode.SV_H)
                    {
                        height = 0x40;
                    }
                    else
                    {
                        height = (int) this.m_ColorSliderSize;
                    }
                    if (this.m_ColorSlider == null)
                    {
                        this.m_ColorSlider = MakeTexture(width, height);
                    }
                    if ((this.m_ColorSlider.width != width) || (this.m_ColorSlider.height != height))
                    {
                        this.m_ColorSlider.Resize(width, height);
                    }
                }
                if (((this.m_ColorBoxMode != this.m_TextureColorSliderMode) || (constantValues != this.m_LastConstantValues)) || this.m_ColorSliderDirty)
                {
                    float tonemappingExposureAdjusment = this.GetTonemappingExposureAdjusment();
                    float colorScale = this.GetColorScale();
                    Color[] pixels = this.m_ColorSlider.GetPixels(0);
                    int xSize = this.m_ColorSlider.width;
                    int ySize = this.m_ColorSlider.height;
                    switch (this.m_ColorBoxMode)
                    {
                        case ColorBoxMode.SV_H:
                            FillArea(xSize, ySize, pixels, new Color(0f, 1f, 1f, 1f), new Color(0f, 0f, 0f, 0f), new Color(1f, 0f, 0f, 0f));
                            HSVToRGBArray(pixels, 1f);
                            break;

                        case ColorBoxMode.HV_S:
                            FillArea(xSize, ySize, pixels, new Color(this.m_H, 0f, Mathf.Max(this.m_V, 0.3f), 1f), new Color(0f, 0f, 0f, 0f), new Color(0f, 1f, 0f, 0f));
                            HSVToRGBArray(pixels, colorScale);
                            break;

                        case ColorBoxMode.HS_V:
                            FillArea(xSize, ySize, pixels, new Color(this.m_H, this.m_S, 0f, 1f), new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, 1f, 0f));
                            HSVToRGBArray(pixels, colorScale);
                            break;

                        case ColorBoxMode.BG_R:
                            FillArea(xSize, ySize, pixels, new Color(0f, this.m_G * colorScale, this.m_B * colorScale, 1f), new Color(0f, 0f, 0f, 0f), new Color(colorScale, 0f, 0f, 0f));
                            break;

                        case ColorBoxMode.BR_G:
                            FillArea(xSize, ySize, pixels, new Color(this.m_R * colorScale, 0f, this.m_B * colorScale, 1f), new Color(0f, 0f, 0f, 0f), new Color(0f, colorScale, 0f, 0f));
                            break;

                        case ColorBoxMode.RG_B:
                            FillArea(xSize, ySize, pixels, new Color(this.m_R * colorScale, this.m_G * colorScale, 0f, 1f), new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, colorScale, 0f));
                            break;
                    }
                    if (QualitySettings.activeColorSpace == ColorSpace.Linear)
                    {
                        LinearToGammaArray(pixels);
                    }
                    if (this.m_ColorBoxMode != ColorBoxMode.SV_H)
                    {
                        DoTonemapping(pixels, tonemappingExposureAdjusment, this.m_HDRValues.m_TonemappingType);
                    }
                    this.m_ColorSlider.SetPixels(pixels, 0);
                    this.m_ColorSlider.Apply(true);
                }
                Graphics.DrawTexture(colorSliderRect, this.m_ColorSlider, new Rect(0.5f / ((float) this.m_ColorSlider.width), 0.5f / ((float) this.m_ColorSlider.height), 1f - (1f / ((float) this.m_ColorSlider.width)), 1f - (1f / ((float) this.m_ColorSlider.height))), 0, 0, 0, 0, Color.grey);
            }
        }

        private void DrawColorSpaceBox(Rect colorBoxRect, float constantValue)
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (this.m_ColorBoxMode != this.m_TextureColorBoxMode)
                {
                    int width = 0;
                    int height = 0;
                    height = 0x20;
                    if ((this.m_ColorBoxMode == ColorBoxMode.HV_S) || (this.m_ColorBoxMode == ColorBoxMode.HS_V))
                    {
                        width = 0x40;
                    }
                    else
                    {
                        width = 0x20;
                    }
                    if (this.m_ColorBox == null)
                    {
                        this.m_ColorBox = MakeTexture(width, height);
                    }
                    if ((this.m_ColorBox.width != width) || (this.m_ColorBox.height != height))
                    {
                        this.m_ColorBox.Resize(width, height);
                    }
                }
                if (((this.m_ColorBoxMode != this.m_TextureColorBoxMode) || (this.m_LastConstant != constantValue)) || this.m_ColorSpaceBoxDirty)
                {
                    float tonemappingExposureAdjusment = this.GetTonemappingExposureAdjusment();
                    float colorScale = this.GetColorScale();
                    this.m_Colors = this.m_ColorBox.GetPixels(0);
                    int xSize = this.m_ColorBox.width;
                    int ySize = this.m_ColorBox.height;
                    switch (this.m_ColorBoxMode)
                    {
                        case ColorBoxMode.SV_H:
                            FillArea(xSize, ySize, this.m_Colors, new Color(this.m_H, 0f, 0f, 1f), new Color(0f, 1f, 0f, 0f), new Color(0f, 0f, 1f, 0f));
                            HSVToRGBArray(this.m_Colors, colorScale);
                            break;

                        case ColorBoxMode.HV_S:
                            FillArea(xSize, ySize, this.m_Colors, new Color(0f, this.m_S, 0f, 1f), new Color(1f, 0f, 0f, 0f), new Color(0f, 0f, 1f, 0f));
                            HSVToRGBArray(this.m_Colors, colorScale);
                            break;

                        case ColorBoxMode.HS_V:
                            FillArea(xSize, ySize, this.m_Colors, new Color(0f, 0f, this.m_V * colorScale, 1f), new Color(1f, 0f, 0f, 0f), new Color(0f, 1f, 0f, 0f));
                            HSVToRGBArray(this.m_Colors, 1f);
                            break;

                        case ColorBoxMode.BG_R:
                            FillArea(xSize, ySize, this.m_Colors, new Color(this.m_R * colorScale, 0f, 0f, 1f), new Color(0f, 0f, colorScale, 0f), new Color(0f, colorScale, 0f, 0f));
                            break;

                        case ColorBoxMode.BR_G:
                            FillArea(xSize, ySize, this.m_Colors, new Color(0f, this.m_G * colorScale, 0f, 1f), new Color(0f, 0f, colorScale, 0f), new Color(colorScale, 0f, 0f, 0f));
                            break;

                        case ColorBoxMode.RG_B:
                            FillArea(xSize, ySize, this.m_Colors, new Color(0f, 0f, this.m_B * colorScale, 1f), new Color(colorScale, 0f, 0f, 0f), new Color(0f, colorScale, 0f, 0f));
                            break;
                    }
                    if (QualitySettings.activeColorSpace == ColorSpace.Linear)
                    {
                        LinearToGammaArray(this.m_Colors);
                    }
                    DoTonemapping(this.m_Colors, tonemappingExposureAdjusment, this.m_HDRValues.m_TonemappingType);
                    this.m_ColorBox.SetPixels(this.m_Colors, 0);
                    this.m_ColorBox.Apply(true);
                    this.m_LastConstant = constantValue;
                    this.m_TextureColorBoxMode = (int) this.m_ColorBoxMode;
                }
                Graphics.DrawTexture(colorBoxRect, this.m_ColorBox, new Rect(0.5f / ((float) this.m_ColorBox.width), 0.5f / ((float) this.m_ColorBox.height), 1f - (1f / ((float) this.m_ColorBox.width)), 1f - (1f / ((float) this.m_ColorBox.height))), 0, 0, 0, 0, Color.grey);
                DrawLabelOutsideRect(colorBoxRect, this.GetXAxisLabel(this.m_ColorBoxMode), LabelLocation.Bottom);
                DrawLabelOutsideRect(colorBoxRect, this.GetYAxisLabel(this.m_ColorBoxMode), LabelLocation.Left);
            }
        }

        private static void DrawLabelOutsideRect(Rect position, string label, LabelLocation labelLocation)
        {
            Matrix4x4 matrix = GUI.matrix;
            Rect rect = new Rect(position.x, position.y - 18f, position.width, 16f);
            switch (labelLocation)
            {
                case LabelLocation.Bottom:
                    rect = new Rect(position.x, position.yMax, position.width, 16f);
                    break;

                case LabelLocation.Left:
                    GUIUtility.RotateAroundPivot(-90f, position.center);
                    break;

                case LabelLocation.Right:
                    GUIUtility.RotateAroundPivot(90f, position.center);
                    break;
            }
            EditorGUI.BeginDisabledGroup(true);
            GUI.Label(rect, label, styles.label);
            EditorGUI.EndDisabledGroup();
            GUI.matrix = matrix;
        }

        private static float EditableAxisLabel(Rect rect, Rect dragRect, float value, float minValue, float maxValue, GUIStyle style)
        {
            int id = GUIUtility.GetControlID(0x9b10027, FocusType.Keyboard, rect);
            string kFloatFieldFormatString = EditorGUI.kFloatFieldFormatString;
            EditorGUI.kFloatFieldFormatString = (value >= 10f) ? "n0" : "n1";
            float num2 = EditorGUI.DoFloatField(EditorGUI.s_RecycledEditor, rect, dragRect, id, value, EditorGUI.kFloatFieldFormatString, style, true);
            EditorGUI.kFloatFieldFormatString = kFloatFieldFormatString;
            return Mathf.Clamp(num2, minValue, maxValue);
        }

        private static void FillArea(int xSize, int ySize, Color[] retval, Color topLeftColor, Color rightGradient, Color downGradient)
        {
            Color color = new Color(0f, 0f, 0f, 0f);
            Color color2 = new Color(0f, 0f, 0f, 0f);
            if (xSize > 1)
            {
                color = (Color) (rightGradient / ((float) (xSize - 1)));
            }
            if (ySize > 1)
            {
                color2 = (Color) (downGradient / ((float) (ySize - 1)));
            }
            Color color3 = topLeftColor;
            int num = 0;
            for (int i = 0; i < ySize; i++)
            {
                Color color4 = color3;
                for (int j = 0; j < xSize; j++)
                {
                    retval[num++] = color4;
                    color4 += color;
                }
                color3 += color2;
            }
        }

        private float GetColorScale()
        {
            if (this.m_HDR)
            {
                return Mathf.Max(1f, this.m_HDRValues.m_HDRScaleFactor);
            }
            return 1f;
        }

        public static Texture2D GetGradientTextureWithAlpha0To1()
        {
            if (s_RightGradientTexture == null)
            {
            }
            return (s_RightGradientTexture = CreateGradientTexture("ColorPicker_0To1_Gradient", 4, 4, new Color(1f, 1f, 1f, 0f), new Color(1f, 1f, 1f, 1f)));
        }

        public static Texture2D GetGradientTextureWithAlpha1To0()
        {
            if (s_LeftGradientTexture == null)
            {
            }
            return (s_LeftGradientTexture = CreateGradientTexture("ColorPicker_1To0_Gradient", 4, 4, new Color(1f, 1f, 1f, 1f), new Color(1f, 1f, 1f, 0f)));
        }

        private float GetScrollWheelDeltaInRect(Rect rect)
        {
            Event current = Event.current;
            if ((current.type == EventType.ScrollWheel) && rect.Contains(current.mousePosition))
            {
                return current.delta.y;
            }
            return 0f;
        }

        private float GetTonemappingExposureAdjusment()
        {
            return ((!this.m_HDR || !this.m_UseTonemappingPreview) ? -1f : this.m_HDRValues.m_ExposureAdjustment);
        }

        private string GetXAxisLabel(ColorBoxMode colorBoxMode)
        {
            return this.m_ColorBoxXAxisLabels[(int) colorBoxMode];
        }

        private string GetYAxisLabel(ColorBoxMode colorBoxMode)
        {
            return this.m_ColorBoxYAxisLabels[(int) colorBoxMode];
        }

        private string GetZAxisLabel(ColorBoxMode colorBoxMode)
        {
            return this.m_ColorBoxZAxisLabels[(int) colorBoxMode];
        }

        private void HandleCopyPasteEvents()
        {
            string commandName;
            Dictionary<string, int> dictionary;
            int num;
            Event current = Event.current;
            switch (current.type)
            {
                case EventType.ValidateCommand:
                    commandName = current.commandName;
                    if (commandName != null)
                    {
                        if (<>f__switch$map18 == null)
                        {
                            dictionary = new Dictionary<string, int>(2);
                            dictionary.Add("Copy", 0);
                            dictionary.Add("Paste", 0);
                            <>f__switch$map18 = dictionary;
                        }
                        if (<>f__switch$map18.TryGetValue(commandName, out num) && (num == 0))
                        {
                            current.Use();
                        }
                    }
                    break;

                case EventType.ExecuteCommand:
                    commandName = current.commandName;
                    if (commandName != null)
                    {
                        if (<>f__switch$map19 == null)
                        {
                            dictionary = new Dictionary<string, int>(2);
                            dictionary.Add("Copy", 0);
                            dictionary.Add("Paste", 1);
                            <>f__switch$map19 = dictionary;
                        }
                        if (<>f__switch$map19.TryGetValue(commandName, out num))
                        {
                            if (num == 0)
                            {
                                ColorClipboard.SetColor(ColorPicker.color);
                                current.Use();
                            }
                            else
                            {
                                Color color;
                                if ((num == 1) && ColorClipboard.TryGetColor(this.m_HDR, out color))
                                {
                                    if (!this.m_ShowAlpha)
                                    {
                                        color.a = this.m_A;
                                    }
                                    this.SetColor(color);
                                    this.colorChanged = true;
                                    GUI.changed = true;
                                    current.Use();
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private void HSVSliders()
        {
            EditorGUI.BeginChangeCheck();
            float tonemappingExposureAdjusment = this.GetTonemappingExposureAdjusment();
            float colorScale = this.GetColorScale();
            this.m_HueTexture = Update1DSlider(this.m_HueTexture, 0x40, 1f, 1f, ref this.m_HueTextureS, ref this.m_HueTextureV, 0, true, 1f, -1f, this.m_RGBHSVSlidersDirty, this.m_HDRValues.m_TonemappingType);
            this.m_SatTexture = Update1DSlider(this.m_SatTexture, 0x20, this.m_H, Mathf.Max(this.m_V, 0.2f), ref this.m_SatTextureH, ref this.m_SatTextureV, 1, true, colorScale, tonemappingExposureAdjusment, this.m_RGBHSVSlidersDirty, this.m_HDRValues.m_TonemappingType);
            this.m_ValTexture = Update1DSlider(this.m_ValTexture, 0x20, this.m_H, this.m_S, ref this.m_ValTextureH, ref this.m_ValTextureS, 2, true, colorScale, tonemappingExposureAdjusment, this.m_RGBHSVSlidersDirty, this.m_HDRValues.m_TonemappingType);
            this.m_RGBHSVSlidersDirty = false;
            float displayScale = !this.m_HDR ? 255f : colorScale;
            string formatString = !this.m_HDR ? EditorGUI.kIntFieldFormatString : EditorGUI.kFloatFieldFormatString;
            this.m_H = this.TexturedSlider(this.m_HueTexture, "H", this.m_H, 0f, 1f, 359f, EditorGUI.kIntFieldFormatString, null);
            this.m_S = this.TexturedSlider(this.m_SatTexture, "S", this.m_S, 0f, 1f, !this.m_HDR ? 255f : 1f, formatString, null);
            this.m_V = this.TexturedSlider(this.m_ValTexture, "V", this.m_V, 0f, 1f, displayScale, formatString, null);
            if (EditorGUI.EndChangeCheck())
            {
                this.HSVToRGB();
            }
        }

        private void HSVToRGB()
        {
            Color color = Color.HSVToRGB(this.m_H, this.m_S, this.m_V);
            this.m_R = color.r;
            this.m_G = color.g;
            this.m_B = color.b;
        }

        private static void HSVToRGBArray(Color[] colors, float scale)
        {
            int length = colors.Length;
            for (int i = 0; i < length; i++)
            {
                Color color = colors[i];
                Color color2 = Color.HSVToRGB(color.r, color.g, color.b).RGBMultiplied(scale);
                color2.a = color.a;
                colors[i] = color2;
            }
        }

        private void InitIfNeeded()
        {
            if (styles == null)
            {
                styles = new Styles();
            }
            if (this.m_ColorLibraryEditorState == null)
            {
                this.m_ColorLibraryEditorState = new PresetLibraryEditorState(presetsEditorPrefID);
                this.m_ColorLibraryEditorState.TransferEditorPrefsState(true);
            }
            if (this.m_ColorLibraryEditor == null)
            {
                ScriptableObjectSaveLoadHelper<ColorPresetLibrary> helper = new ScriptableObjectSaveLoadHelper<ColorPresetLibrary>("colors", SaveType.Text);
                this.m_ColorLibraryEditor = new PresetLibraryEditor<ColorPresetLibrary>(helper, this.m_ColorLibraryEditorState, new Action<int, object>(this.PresetClickedCallback));
                this.m_ColorLibraryEditor.previewAspect = 1f;
                this.m_ColorLibraryEditor.minMaxPreviewHeight = new Vector2(14f, 14f);
                this.m_ColorLibraryEditor.settingsMenuRightMargin = 2f;
                this.m_ColorLibraryEditor.useOnePixelOverlappedGrid = true;
                this.m_ColorLibraryEditor.alwaysShowScrollAreaHorizontalLines = false;
                this.m_ColorLibraryEditor.marginsForGrid = new RectOffset(0, 0, 0, 0);
                this.m_ColorLibraryEditor.marginsForList = new RectOffset(0, 5, 2, 2);
                this.m_ColorLibraryEditor.InitializeGrid(233f - (styles.background.padding.left + styles.background.padding.right));
            }
        }

        private static void LinearToGammaArray(Color[] colors)
        {
            int length = colors.Length;
            for (int i = 0; i < length; i++)
            {
                Color color = colors[i];
                Color gamma = color.gamma;
                gamma.a = color.a;
                colors[i] = gamma;
            }
        }

        public static Texture2D MakeTexture(int width, int height)
        {
            return new Texture2D(width, height, TextureFormat.ARGB32, false) { hideFlags = HideFlags.HideAndDontSave, wrapMode = TextureWrapMode.Clamp, hideFlags = HideFlags.HideAndDontSave };
        }

        public void OnDestroy()
        {
            Undo.CollapseUndoOperations(this.m_ModalUndoGroup);
            if (this.m_ColorSlider != null)
            {
                Object.DestroyImmediate(this.m_ColorSlider);
            }
            if (this.m_ColorBox != null)
            {
                Object.DestroyImmediate(this.m_ColorBox);
            }
            if (this.m_RTexture != null)
            {
                Object.DestroyImmediate(this.m_RTexture);
            }
            if (this.m_GTexture != null)
            {
                Object.DestroyImmediate(this.m_GTexture);
            }
            if (this.m_BTexture != null)
            {
                Object.DestroyImmediate(this.m_BTexture);
            }
            if (this.m_HueTexture != null)
            {
                Object.DestroyImmediate(this.m_HueTexture);
            }
            if (this.m_SatTexture != null)
            {
                Object.DestroyImmediate(this.m_SatTexture);
            }
            if (this.m_ValTexture != null)
            {
                Object.DestroyImmediate(this.m_ValTexture);
            }
            if (this.m_AlphaTexture != null)
            {
                Object.DestroyImmediate(this.m_AlphaTexture);
            }
            s_SharedColorPicker = null;
            if (this.m_IsOSColorPicker)
            {
                OSColorPicker.Close();
            }
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.PollOSColorPicker));
            if (this.m_ColorLibraryEditorState != null)
            {
                this.m_ColorLibraryEditorState.TransferEditorPrefsState(false);
            }
            if (this.m_ColorLibraryEditor != null)
            {
                this.m_ColorLibraryEditor.UnloadUsedLibraries();
            }
            if (this.m_ColorBoxMode == ColorBoxMode.EyeDropper)
            {
                EditorPrefs.SetInt("CPColorMode", (int) this.m_OldColorBoxMode);
            }
        }

        private void OnDisable()
        {
            EditorPrefs.SetFloat("CPickerExposure", this.m_HDRValues.m_ExposureAdjustment);
            EditorPrefs.SetInt("CPTonePreview", !this.m_UseTonemappingPreview ? 0 : 1);
            EditorPrefs.SetInt("CPSliderMode", (int) this.m_SliderMode);
            EditorPrefs.SetInt("CPColorMode", (int) this.m_ColorBoxMode);
            EditorPrefs.SetInt("CPPresetsShow", !this.m_ShowPresets ? 0 : 1);
            EditorPrefs.SetInt("CPickerHeight", (int) base.position.height);
        }

        private void OnEnable()
        {
            this.m_HDRValues.m_ExposureAdjustment = EditorPrefs.GetFloat("CPickerExposure", 1f);
            this.m_UseTonemappingPreview = EditorPrefs.GetInt("CPTonePreview", 0) != 0;
            this.m_SliderMode = (SliderMode) EditorPrefs.GetInt("CPSliderMode", 0);
            this.m_ColorBoxMode = (ColorBoxMode) EditorPrefs.GetInt("CPColorMode", 0);
            this.m_ShowPresets = EditorPrefs.GetInt("CPPresetsShow", 1) != 0;
        }

        private void OnFloatFieldChanged(float value)
        {
            if (this.m_HDR && (value > this.m_HDRValues.m_HDRScaleFactor))
            {
                this.SetHDRScaleFactor(value);
            }
        }

        private void OnGUI()
        {
            this.InitIfNeeded();
            if (this.m_resetKeyboardControl)
            {
                GUIUtility.keyboardControl = 0;
                this.m_resetKeyboardControl = false;
            }
            if (Event.current.type == EventType.ExecuteCommand)
            {
                string commandName = Event.current.commandName;
                if (commandName != null)
                {
                    int num2;
                    if (<>f__switch$map17 == null)
                    {
                        Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
                        dictionary.Add("EyeDropperUpdate", 0);
                        dictionary.Add("EyeDropperClicked", 1);
                        dictionary.Add("EyeDropperCancelled", 2);
                        <>f__switch$map17 = dictionary;
                    }
                    if (<>f__switch$map17.TryGetValue(commandName, out num2))
                    {
                        switch (num2)
                        {
                            case 0:
                                base.Repaint();
                                break;

                            case 1:
                            {
                                Color lastPickedColor = EyeDropper.GetLastPickedColor();
                                this.m_R = lastPickedColor.r;
                                this.m_G = lastPickedColor.g;
                                this.m_B = lastPickedColor.b;
                                this.RGBToHSV();
                                this.m_ColorBoxMode = this.m_OldColorBoxMode;
                                this.m_Color = new Color(this.m_R, this.m_G, this.m_B, this.m_A);
                                this.SendEvent(true);
                                break;
                            }
                            case 2:
                                base.Repaint();
                                this.m_ColorBoxMode = this.m_OldColorBoxMode;
                                break;
                        }
                    }
                }
            }
            Rect rect = EditorGUILayout.BeginVertical(styles.background, new GUILayoutOption[0]);
            float width = EditorGUILayout.GetControlRect(false, 1f, EditorStyles.numberField, new GUILayoutOption[0]).width;
            EditorGUIUtility.labelWidth = width - this.fieldWidth;
            EditorGUIUtility.fieldWidth = this.fieldWidth;
            EditorGUI.BeginChangeCheck();
            GUILayout.Space(10f);
            this.DoColorSwatchAndEyedropper();
            GUILayout.Space(10f);
            if (this.m_HDR)
            {
                this.TonemappingControls();
                GUILayout.Space(10f);
            }
            this.DoColorSpaceGUI();
            GUILayout.Space(10f);
            if (this.m_HDR)
            {
                GUILayout.Space(5f);
                this.BrightnessField();
                GUILayout.Space(10f);
            }
            this.DoColorSliders();
            GUILayout.Space(5f);
            this.DoHexField(width);
            GUILayout.Space(10f);
            if (EditorGUI.EndChangeCheck())
            {
                this.colorChanged = true;
            }
            this.DoPresetsGUI();
            this.HandleCopyPasteEvents();
            if (this.colorChanged)
            {
                this.colorChanged = false;
                this.m_Color = new Color(this.m_R, this.m_G, this.m_B, this.m_A);
                this.SendEvent(true);
            }
            EditorGUILayout.EndVertical();
            if ((rect.height > 0f) && (Event.current.type == EventType.Repaint))
            {
                this.SetHeight(rect.height);
            }
            if (Event.current.type != EventType.KeyDown)
            {
                goto Label_0321;
            }
            KeyCode keyCode = Event.current.keyCode;
            if (keyCode == KeyCode.Return)
            {
                goto Label_0316;
            }
            if (keyCode != KeyCode.Escape)
            {
                if (keyCode == KeyCode.KeypadEnter)
                {
                    goto Label_0316;
                }
            }
            else
            {
                Undo.RevertAllDownToGroup(this.m_ModalUndoGroup);
                this.m_Color = this.m_OriginalColor;
                this.SendEvent(false);
                base.Close();
                GUIUtility.ExitGUI();
            }
            goto Label_0321;
        Label_0316:
            base.Close();
        Label_0321:
            if (((Event.current.type == EventType.MouseDown) && (Event.current.button != 1)) || (Event.current.type == EventType.ContextClick))
            {
                GUIUtility.keyboardControl = 0;
                base.Repaint();
            }
        }

        private void OnSelectionChange()
        {
            this.m_resetKeyboardControl = true;
            base.Repaint();
        }

        private static float PhotographicTonemapping(float value, float exposureAdjustment)
        {
            return (1f - Mathf.Pow(2f, -exposureAdjustment * value));
        }

        private void PollOSColorPicker()
        {
            if (this.m_IsOSColorPicker)
            {
                if (!OSColorPicker.visible || (Application.platform != RuntimePlatform.OSXEditor))
                {
                    Object.DestroyImmediate(this);
                }
                else
                {
                    Color color = OSColorPicker.color;
                    if (this.m_Color != color)
                    {
                        this.m_Color = color;
                        this.SendEvent(true);
                    }
                }
            }
        }

        private void PresetClickedCallback(int clickCount, object presetObject)
        {
            Color c = (Color) presetObject;
            if (!this.m_HDR && (c.maxColorComponent > 1f))
            {
                c = c.RGBMultiplied((float) (1f / c.maxColorComponent));
            }
            this.SetColor(c);
            this.colorChanged = true;
        }

        private void RemoveFocusFromActiveTextField()
        {
            EditorGUI.EndEditingActiveTextField();
            GUIUtility.keyboardControl = 0;
        }

        private void RGBSliders()
        {
            EditorGUI.BeginChangeCheck();
            float tonemappingExposureAdjusment = this.GetTonemappingExposureAdjusment();
            float colorScale = this.GetColorScale();
            this.m_RTexture = Update1DSlider(this.m_RTexture, 0x20, this.m_G, this.m_B, ref this.m_RTextureG, ref this.m_RTextureB, 0, false, colorScale, tonemappingExposureAdjusment, this.m_RGBHSVSlidersDirty, this.m_HDRValues.m_TonemappingType);
            this.m_GTexture = Update1DSlider(this.m_GTexture, 0x20, this.m_R, this.m_B, ref this.m_GTextureR, ref this.m_GTextureB, 1, false, colorScale, tonemappingExposureAdjusment, this.m_RGBHSVSlidersDirty, this.m_HDRValues.m_TonemappingType);
            this.m_BTexture = Update1DSlider(this.m_BTexture, 0x20, this.m_R, this.m_G, ref this.m_BTextureR, ref this.m_BTextureG, 2, false, colorScale, tonemappingExposureAdjusment, this.m_RGBHSVSlidersDirty, this.m_HDRValues.m_TonemappingType);
            this.m_RGBHSVSlidersDirty = false;
            float displayScale = !this.m_HDR ? 255f : colorScale;
            string formatString = !this.m_HDR ? EditorGUI.kIntFieldFormatString : EditorGUI.kFloatFieldFormatString;
            this.m_R = this.TexturedSlider(this.m_RTexture, "R", this.m_R, 0f, 1f, displayScale, formatString, new Action<float>(this.OnFloatFieldChanged));
            this.m_G = this.TexturedSlider(this.m_GTexture, "G", this.m_G, 0f, 1f, displayScale, formatString, new Action<float>(this.OnFloatFieldChanged));
            this.m_B = this.TexturedSlider(this.m_BTexture, "B", this.m_B, 0f, 1f, displayScale, formatString, new Action<float>(this.OnFloatFieldChanged));
            if (EditorGUI.EndChangeCheck())
            {
                this.RGBToHSV();
            }
        }

        private void RGBToHSV()
        {
            Color.RGBToHSV(new Color(this.m_R, this.m_G, this.m_B, 1f), out this.m_H, out this.m_S, out this.m_V);
        }

        private static void ScaleColors(Color[] colors, float scale)
        {
            int length = colors.Length;
            for (int i = 0; i < length; i++)
            {
                colors[i] = colors[i].RGBMultiplied(scale);
            }
        }

        private void SendEvent(bool exitGUI)
        {
            if (this.m_DelegateView != null)
            {
                Event e = EditorGUIUtility.CommandEvent("ColorPickerChanged");
                if (!this.m_IsOSColorPicker)
                {
                    base.Repaint();
                }
                this.m_DelegateView.SendEvent(e);
                if (!this.m_IsOSColorPicker && exitGUI)
                {
                    GUIUtility.ExitGUI();
                }
            }
        }

        private void SetColor(Color c)
        {
            if (this.m_IsOSColorPicker)
            {
                OSColorPicker.color = c;
            }
            else
            {
                float hDRScaleFactor = this.m_HDRValues.m_HDRScaleFactor;
                if (this.m_HDR)
                {
                    float maxColorComponent = c.maxColorComponent;
                    if (maxColorComponent > 1f)
                    {
                        c = c.RGBMultiplied((float) (1f / maxColorComponent));
                    }
                    this.SetHDRScaleFactor(Mathf.Max(1f, maxColorComponent));
                }
                if ((((this.m_Color.r != c.r) || (this.m_Color.g != c.g)) || ((this.m_Color.b != c.b) || (this.m_Color.a != c.a))) || (hDRScaleFactor != this.m_HDRValues.m_HDRScaleFactor))
                {
                    if (((c.r > 1f) || (c.g > 1f)) || (c.b > 1f))
                    {
                        Debug.LogError(string.Format("Invalid normalized color: {0}, normalize value: {1}", c, this.m_HDRValues.m_HDRScaleFactor));
                    }
                    this.m_resetKeyboardControl = true;
                    this.SetNormalizedColor(c);
                    base.Repaint();
                }
            }
        }

        private void SetHDRScaleFactor(float value)
        {
            if (!this.m_HDR)
            {
                Debug.LogError("HDR scale is being set in LDR mode!");
            }
            if (value < 1f)
            {
                Debug.LogError("SetHDRScaleFactor is below 1, should be >= 1!");
            }
            this.m_HDRValues.m_HDRScaleFactor = Mathf.Clamp(value, 0f, this.m_HDRConfig.maxBrightness);
            this.m_ColorSliderDirty = true;
            this.m_ColorSpaceBoxDirty = true;
            this.m_RGBHSVSlidersDirty = true;
        }

        private void SetHeight(float newHeight)
        {
            if (newHeight != base.position.height)
            {
                base.minSize = new Vector2(233f, newHeight);
                base.maxSize = new Vector2(233f, newHeight);
            }
        }

        private void SetMaxDisplayBrightness(float brightness)
        {
            brightness = Mathf.Clamp(brightness, 1f, this.m_HDRConfig.maxBrightness);
            if (brightness != this.m_HDRValues.m_HDRScaleFactor)
            {
                Color c = color.RGBMultiplied((float) (1f / brightness));
                if (c.maxColorComponent <= 1f)
                {
                    this.SetNormalizedColor(c);
                    this.SetHDRScaleFactor(brightness);
                    base.Repaint();
                }
            }
        }

        private void SetNormalizedColor(Color c)
        {
            if (c.maxColorComponent > 1f)
            {
                Debug.LogError("Setting normalized color with a non-normalized color: " + c);
            }
            this.m_Color = c;
            this.m_R = c.r;
            this.m_G = c.g;
            this.m_B = c.b;
            this.RGBToHSV();
            this.m_A = c.a;
        }

        public static void Show(GUIView viewToUpdate, Color col)
        {
            Show(viewToUpdate, col, true, false, null);
        }

        public static void Show(GUIView viewToUpdate, Color col, bool showAlpha, bool hdr, ColorPickerHDRConfig hdrConfig)
        {
            ColorPicker get = ColorPicker.get;
            get.m_HDR = hdr;
            if (hdrConfig == null)
            {
            }
            get.m_HDRConfig = new ColorPickerHDRConfig(defaultHDRConfig);
            get.m_DelegateView = viewToUpdate;
            get.SetColor(col);
            get.m_OriginalColor = ColorPicker.get.m_Color;
            get.m_ShowAlpha = showAlpha;
            get.m_ModalUndoGroup = Undo.GetCurrentGroup();
            if (get.m_HDR)
            {
                get.m_IsOSColorPicker = false;
            }
            if (get.m_IsOSColorPicker)
            {
                OSColorPicker.Show(showAlpha);
            }
            else
            {
                get.titleContent = !hdr ? EditorGUIUtility.TextContent("Color") : EditorGUIUtility.TextContent("HDR Color");
                float @int = EditorPrefs.GetInt("CPickerHeight", (int) get.position.height);
                get.minSize = new Vector2(233f, @int);
                get.maxSize = new Vector2(233f, @int);
                get.InitIfNeeded();
                get.ShowAuxWindow();
            }
        }

        private Vector2 Slider2D(Rect rect, Vector2 value, Vector2 maxvalue, Vector2 minvalue, GUIStyle backStyle, GUIStyle thumbStyle)
        {
            if (backStyle != null)
            {
                if (thumbStyle == null)
                {
                    return value;
                }
                int controlID = GUIUtility.GetControlID(s_Slider2Dhash, FocusType.Native);
                if (maxvalue.x < minvalue.x)
                {
                    swap(ref maxvalue.x, ref minvalue.x);
                }
                if (maxvalue.y < minvalue.y)
                {
                    swap(ref maxvalue.y, ref minvalue.y);
                }
                float height = (thumbStyle.fixedHeight != 0f) ? thumbStyle.fixedHeight : ((float) thumbStyle.padding.vertical);
                float width = (thumbStyle.fixedWidth != 0f) ? thumbStyle.fixedWidth : ((float) thumbStyle.padding.horizontal);
                Vector2 vector = new Vector2(((rect.width - (backStyle.padding.right + backStyle.padding.left)) - (width * 2f)) / (maxvalue.x - minvalue.x), ((rect.height - (backStyle.padding.top + backStyle.padding.bottom)) - (height * 2f)) / (maxvalue.y - minvalue.y));
                Rect position = new Rect((((rect.x + (value.x * vector.x)) + (width / 2f)) + backStyle.padding.left) - (minvalue.x * vector.x), (((rect.y + (value.y * vector.y)) + (height / 2f)) + backStyle.padding.top) - (minvalue.y * vector.y), width, height);
                Event current = Event.current;
                switch (current.GetTypeForControl(controlID))
                {
                    case EventType.MouseDown:
                        if (rect.Contains(current.mousePosition))
                        {
                            GUIUtility.hotControl = controlID;
                            GUIUtility.keyboardControl = 0;
                            value.x = ((((current.mousePosition.x - rect.x) - width) - backStyle.padding.left) / vector.x) + minvalue.x;
                            value.y = ((((current.mousePosition.y - rect.y) - height) - backStyle.padding.top) / vector.y) + minvalue.y;
                            GUI.changed = true;
                            Event.current.Use();
                        }
                        return value;

                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == controlID)
                        {
                            GUIUtility.hotControl = 0;
                            current.Use();
                        }
                        return value;

                    case EventType.MouseMove:
                    case EventType.KeyDown:
                    case EventType.KeyUp:
                    case EventType.ScrollWheel:
                        return value;

                    case EventType.MouseDrag:
                        if (GUIUtility.hotControl == controlID)
                        {
                            value.x = ((((current.mousePosition.x - rect.x) - width) - backStyle.padding.left) / vector.x) + minvalue.x;
                            value.y = ((((current.mousePosition.y - rect.y) - height) - backStyle.padding.top) / vector.y) + minvalue.y;
                            value.x = Mathf.Clamp(value.x, minvalue.x, maxvalue.x);
                            value.y = Mathf.Clamp(value.y, minvalue.y, maxvalue.y);
                            GUI.changed = true;
                            Event.current.Use();
                            return value;
                        }
                        return value;

                    case EventType.Repaint:
                    {
                        backStyle.Draw(rect, GUIContent.none, controlID);
                        Color color = GUI.color;
                        bool flag = ColorPicker.color.grayscale > 0.5f;
                        if (flag)
                        {
                            GUI.color = Color.black;
                        }
                        thumbStyle.Draw(position, GUIContent.none, controlID);
                        if (flag)
                        {
                            GUI.color = color;
                        }
                        return value;
                    }
                }
            }
            return value;
        }

        private void Slider3D(Rect boxPos, Rect sliderPos, ref float x, ref float y, ref float z, GUIStyle box, GUIStyle thumb2D, GUIStyle thumbHoriz)
        {
            Rect colorBoxRect = boxPos;
            colorBoxRect.x++;
            colorBoxRect.y++;
            colorBoxRect.width -= 2f;
            colorBoxRect.height -= 2f;
            this.DrawColorSpaceBox(colorBoxRect, z);
            Vector2 vector = new Vector2(x, 1f - y);
            vector = this.Slider2D(boxPos, vector, new Vector2(0f, 0f), new Vector2(1f, 1f), box, thumb2D);
            x = vector.x;
            y = 1f - vector.y;
            if (this.m_HDR)
            {
                this.SpecialHDRBrightnessHandling(boxPos, sliderPos);
            }
            Rect colorSliderRect = new Rect(sliderPos.x + 1f, sliderPos.y + 1f, sliderPos.width - 2f, sliderPos.height - 2f);
            this.DrawColorSlider(colorSliderRect, new Vector2(x, y));
            if ((Event.current.type == EventType.MouseDown) && sliderPos.Contains(Event.current.mousePosition))
            {
                this.RemoveFocusFromActiveTextField();
            }
            z = GUI.VerticalSlider(sliderPos, z, 1f, 0f, box, thumbHoriz);
            DrawLabelOutsideRect(new Rect(sliderPos.xMax - sliderPos.height, sliderPos.y, sliderPos.height + 1f, sliderPos.height + 1f), this.GetZAxisLabel(this.m_ColorBoxMode), LabelLocation.Right);
        }

        private void SpecialHDRBrightnessHandling(Rect boxPos, Rect sliderPos)
        {
            if ((this.m_ColorBoxMode == ColorBoxMode.SV_H) || (this.m_ColorBoxMode == ColorBoxMode.HV_S))
            {
                float scrollWheelDeltaInRect = this.GetScrollWheelDeltaInRect(boxPos);
                if (scrollWheelDeltaInRect != 0f)
                {
                    this.SetMaxDisplayBrightness(this.m_HDRValues.m_HDRScaleFactor - (scrollWheelDeltaInRect * 0.05f));
                }
                Rect rect = new Rect(0f, boxPos.y - 7f, boxPos.x - 2f, 14f);
                Rect dragRect = rect;
                dragRect.y += rect.height;
                EditorGUI.BeginChangeCheck();
                float brightness = EditableAxisLabel(rect, dragRect, this.m_HDRValues.m_HDRScaleFactor, 1f, this.m_HDRConfig.maxBrightness, styles.axisLabelNumberField);
                if (EditorGUI.EndChangeCheck())
                {
                    this.SetMaxDisplayBrightness(brightness);
                }
            }
            if (this.m_ColorBoxMode == ColorBoxMode.HS_V)
            {
                Rect rect3 = new Rect(sliderPos.xMax + 2f, sliderPos.y - 7f, (base.position.width - sliderPos.xMax) - 2f, 14f);
                Rect rect4 = rect3;
                rect4.y += rect3.height;
                EditorGUI.BeginChangeCheck();
                float num5 = EditableAxisLabel(rect3, rect4, this.m_HDRValues.m_HDRScaleFactor, 1f, this.m_HDRConfig.maxBrightness, styles.axisLabelNumberField);
                if (EditorGUI.EndChangeCheck())
                {
                    this.SetMaxDisplayBrightness(num5);
                }
            }
        }

        private static void swap(ref float f1, ref float f2)
        {
            float num = f1;
            f1 = f2;
            f2 = num;
        }

        private float TexturedSlider(Texture2D background, string text, float val, float min, float max, float displayScale, string formatString, Action<float> onFloatFieldChanged)
        {
            Rect rect = GUILayoutUtility.GetRect(16f, 16f, GUI.skin.label);
            GUI.Label(new Rect(rect.x, rect.y, 20f, 16f), text);
            rect.x += 14f;
            rect.width -= 20f + this.fieldWidth;
            if (Event.current.type == EventType.Repaint)
            {
                Rect screenRect = new Rect(rect.x + 1f, rect.y + 2f, rect.width - 2f, rect.height - 4f);
                Graphics.DrawTexture(screenRect, background, new Rect(0.5f / ((float) background.width), 0.5f / ((float) background.height), 1f - (1f / ((float) background.width)), 1f - (1f / ((float) background.height))), 0, 0, 0, 0, Color.grey);
            }
            int id = GUIUtility.GetControlID(0xd42b5, EditorGUIUtility.native, base.position);
            EditorGUI.BeginChangeCheck();
            val = GUI.HorizontalSlider(new Rect(rect.x, rect.y + 1f, rect.width, rect.height - 2f), val, min, max, styles.pickerBox, styles.thumbHoriz);
            if (EditorGUI.EndChangeCheck())
            {
                if (EditorGUI.s_RecycledEditor.IsEditingControl(id))
                {
                    EditorGUI.s_RecycledEditor.EndEditing();
                }
                val = (float) Math.Round((double) val, 3);
                GUIUtility.keyboardControl = 0;
            }
            Rect position = new Rect(rect.xMax + 6f, rect.y, this.fieldWidth, 16f);
            EditorGUI.BeginChangeCheck();
            val = EditorGUI.DoFloatField(EditorGUI.s_RecycledEditor, position, new Rect(0f, 0f, 0f, 0f), id, val * displayScale, formatString, EditorStyles.numberField, false);
            if (EditorGUI.EndChangeCheck() && (onFloatFieldChanged != null))
            {
                onFloatFieldChanged(val);
            }
            val = Mathf.Clamp(val / displayScale, min, max);
            GUILayout.Space(3f);
            return val;
        }

        private void TonemappingControls()
        {
            bool flag = false;
            EditorGUI.BeginChangeCheck();
            this.m_UseTonemappingPreview = GUILayout.Toggle(this.m_UseTonemappingPreview, styles.tonemappingToggle, styles.toggle, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                flag = true;
            }
            if (this.m_UseTonemappingPreview)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                float power = (QualitySettings.activeColorSpace != ColorSpace.Linear) ? 2f : 1f;
                this.m_HDRValues.m_ExposureAdjustment = EditorGUILayout.PowerSlider(string.Empty, this.m_HDRValues.m_ExposureAdjustment, this.m_HDRConfig.minExposureValue, this.m_HDRConfig.maxExposureValue, power, new GUILayoutOption[0]);
                if (Event.current.type == EventType.Repaint)
                {
                    GUI.Label(EditorGUILayout.s_LastRect, GUIContent.Temp(string.Empty, "Exposure value"));
                }
                if (EditorGUI.EndChangeCheck())
                {
                    flag = true;
                }
                Rect position = EditorGUILayout.GetControlRect(true, 16f, EditorStyles.numberField, new GUILayoutOption[0]);
                EditorGUI.LabelField(position, GUIContent.Temp("Tonemapped Color"));
                Rect rect2 = new Rect(position.xMax - this.fieldWidth, position.y, this.fieldWidth, position.height);
                EditorGUIUtility.DrawColorSwatch(rect2, DoTonemapping(color, this.m_HDRValues.m_ExposureAdjustment), false, false);
                GUI.Label(rect2, GUIContent.none, styles.colorPickerBox);
                EditorGUI.indentLevel--;
            }
            if (flag)
            {
                this.m_RGBHSVSlidersDirty = true;
                this.m_ColorSpaceBoxDirty = true;
                this.m_ColorSliderDirty = true;
            }
        }

        private static Texture2D Update1DSlider(Texture2D tex, int xSize, float const1, float const2, ref float oldConst1, ref float oldConst2, int idx, bool hsvSpace, float scale, float exposureValue, bool forceUpdate, TonemappingType tonemappingType)
        {
            if (((tex == null) || (const1 != oldConst1)) || ((const2 != oldConst2) || forceUpdate))
            {
                if (tex == null)
                {
                    tex = MakeTexture(xSize, 2);
                }
                Color[] retval = new Color[xSize * 2];
                Color black = Color.black;
                Color rightGradient = Color.black;
                switch (idx)
                {
                    case 0:
                        black = new Color(0f, const1, const2, 1f);
                        rightGradient = new Color(1f, 0f, 0f, 0f);
                        break;

                    case 1:
                        black = new Color(const1, 0f, const2, 1f);
                        rightGradient = new Color(0f, 1f, 0f, 0f);
                        break;

                    case 2:
                        black = new Color(const1, const2, 0f, 1f);
                        rightGradient = new Color(0f, 0f, 1f, 0f);
                        break;

                    case 3:
                        black = new Color(0f, 0f, 0f, 1f);
                        rightGradient = new Color(1f, 1f, 1f, 0f);
                        break;
                }
                FillArea(xSize, 2, retval, black, rightGradient, new Color(0f, 0f, 0f, 0f));
                if (hsvSpace)
                {
                    HSVToRGBArray(retval, scale);
                }
                else
                {
                    ScaleColors(retval, scale);
                }
                DoTonemapping(retval, exposureValue, tonemappingType);
                oldConst1 = const1;
                oldConst2 = const2;
                tex.SetPixels(retval);
                tex.Apply();
            }
            return tex;
        }

        public static Color color
        {
            get
            {
                if (get.m_HDRValues.m_HDRScaleFactor > 1f)
                {
                    return get.m_Color.RGBMultiplied(get.m_HDRValues.m_HDRScaleFactor);
                }
                return get.m_Color;
            }
            set
            {
                get.SetColor(value);
            }
        }

        private bool colorChanged { get; set; }

        public string currentPresetLibrary
        {
            get
            {
                this.InitIfNeeded();
                return this.m_ColorLibraryEditor.currentLibraryWithoutExtension;
            }
            set
            {
                this.InitIfNeeded();
                this.m_ColorLibraryEditor.currentLibraryWithoutExtension = value;
            }
        }

        public static ColorPickerHDRConfig defaultHDRConfig
        {
            get
            {
                return m_DefaultHDRConfig;
            }
        }

        private float fieldWidth
        {
            get
            {
                return (!this.m_HDR ? 30f : 40f);
            }
        }

        public static ColorPicker get
        {
            get
            {
                if (s_SharedColorPicker == null)
                {
                    Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(ColorPicker));
                    if ((objArray != null) && (objArray.Length > 0))
                    {
                        s_SharedColorPicker = (ColorPicker) objArray[0];
                    }
                    if (s_SharedColorPicker == null)
                    {
                        s_SharedColorPicker = ScriptableObject.CreateInstance<ColorPicker>();
                        s_SharedColorPicker.wantsMouseMove = true;
                    }
                }
                return s_SharedColorPicker;
            }
        }

        public static string presetsEditorPrefID
        {
            get
            {
                return "Color";
            }
        }

        public static bool visible
        {
            get
            {
                return (s_SharedColorPicker != null);
            }
        }

        private enum ColorBoxMode
        {
            SV_H,
            HV_S,
            HS_V,
            BG_R,
            BR_G,
            RG_B,
            EyeDropper
        }

        [Serializable]
        private class HDRValues
        {
            [SerializeField]
            public float m_ExposureAdjustment = 1.5f;
            [SerializeField]
            public float m_HDRScaleFactor;
            [NonSerialized]
            public ColorPicker.TonemappingType m_TonemappingType = ColorPicker.TonemappingType.Photographic;
        }

        private enum LabelLocation
        {
            Top,
            Bottom,
            Left,
            Right
        }

        private enum SliderMode
        {
            RGB,
            HSV
        }

        private class Styles
        {
            public GUIStyle axisLabelNumberField = new GUIStyle(EditorStyles.miniTextField);
            public GUIStyle background = new GUIStyle("ColorPickerBackground");
            public GUIContent colorCycle = EditorGUIUtility.IconContent("ColorPicker.CycleColor");
            public GUIStyle colorPickerBox = "ColorPickerBox";
            public GUIContent colorToggle = EditorGUIUtility.TextContent("Colors");
            public GUIContent eyeDropper = EditorGUIUtility.IconContent("EyeDropper.Large", "|Pick a color from the screen.");
            public GUIStyle foldout = new GUIStyle(EditorStyles.foldout);
            public GUIStyle headerLine = "IN Title";
            public GUIStyle label = new GUIStyle(EditorStyles.miniLabel);
            public GUIStyle pickerBox = "ColorPickerBox";
            public GUIContent presetsToggle = new GUIContent("Presets");
            public GUIContent sliderCycle = EditorGUIUtility.IconContent("ColorPicker.CycleSlider");
            public GUIContent sliderToggle = EditorGUIUtility.TextContent("Sliders|The RGB or HSV color sliders.");
            public GUIStyle thumb2D = "ColorPicker2DThumb";
            public GUIStyle thumbHoriz = "ColorPickerHorizThumb";
            public GUIStyle thumbVert = "ColorPickerVertThumb";
            public GUIStyle toggle = new GUIStyle(EditorStyles.toggle);
            public GUIContent tonemappingToggle = new GUIContent("Tonemapped Preview", "When enabled preview colors are tonemapped using Photographic Tonemapping");

            public Styles()
            {
                this.axisLabelNumberField.alignment = TextAnchor.UpperRight;
                this.axisLabelNumberField.normal.background = null;
                this.label.alignment = TextAnchor.LowerCenter;
            }
        }

        internal enum TonemappingType
        {
            Linear,
            Photographic
        }
    }
}

