namespace UnityEditor
{
    using System;
    using UnityEngine;

    public sealed class EditorStyles
    {
        private GUIStyle m_AssetLabel;
        private GUIStyle m_AssetLabelIcon;
        private GUIStyle m_AssetLabelPartial;
        internal Font m_BoldFont;
        private GUIStyle m_BoldLabel;
        private GUIStyle m_CenteredGreyMiniLabel;
        private GUIStyle m_ColorField;
        private GUIStyle m_ColorPickerBox;
        private GUIStyle m_DropDownList;
        private GUIStyle m_Foldout;
        private GUIStyle m_FoldoutPreDrop;
        private GUIStyle m_FoldoutSelected;
        private GUIStyle m_HelpBox;
        private GUIStyle m_InspectorBig;
        private GUIStyle m_InspectorDefaultMargins;
        private GUIStyle m_InspectorFullWidthMargins;
        private GUIStyle m_InspectorTitlebar;
        private GUIStyle m_InspectorTitlebarText;
        private Vector2 m_KnobSize = new Vector2(40f, 40f);
        internal GUIStyle m_Label;
        private GUIStyle m_LargeLabel;
        private GUIStyle m_LayerMaskField;
        internal Font m_MiniBoldFont;
        private GUIStyle m_MiniBoldLabel;
        private GUIStyle m_MiniButton;
        private GUIStyle m_MiniButtonLeft;
        private GUIStyle m_MiniButtonMid;
        private GUIStyle m_MiniButtonRight;
        internal Font m_MiniFont;
        private Vector2 m_MiniKnobSize = new Vector2(29f, 29f);
        private GUIStyle m_MiniLabel;
        private GUIStyle m_MiniTextField;
        private GUIStyle m_MinMaxHorizontalSliderThumb;
        private GUIStyle m_NotificationBackground;
        private GUIStyle m_NotificationText;
        private GUIStyle m_NumberField;
        private GUIStyle m_ObjectField;
        private GUIStyle m_ObjectFieldMiniThumb;
        private GUIStyle m_ObjectFieldThumb;
        private GUIStyle m_Popup;
        private GUIStyle m_ProgressBarBack;
        private GUIStyle m_ProgressBarBar;
        private GUIStyle m_ProgressBarText;
        private GUIStyle m_RadioButton;
        private GUIStyle m_SearchField;
        private GUIStyle m_SearchFieldCancelButton;
        private GUIStyle m_SearchFieldCancelButtonEmpty;
        private GUIStyle m_SelectionRect;
        internal Font m_StandardFont;
        internal GUIStyle m_TextArea;
        internal GUIStyle m_TextField;
        private GUIStyle m_TextFieldDropDown;
        private GUIStyle m_TextFieldDropDownText;
        private GUIStyle m_Toggle;
        private GUIStyle m_ToggleGroup;
        private GUIStyle m_ToggleMixed;
        private GUIStyle m_Toolbar;
        private GUIStyle m_ToolbarButton;
        private GUIStyle m_ToolbarDropDown;
        private GUIStyle m_ToolbarPopup;
        private GUIStyle m_ToolbarSearchField;
        private GUIStyle m_ToolbarSearchFieldCancelButton;
        private GUIStyle m_ToolbarSearchFieldCancelButtonEmpty;
        private GUIStyle m_ToolbarSearchFieldPopup;
        private GUIStyle m_ToolbarTextField;
        private GUIStyle m_Tooltip;
        private GUIStyle m_WhiteBoldLabel;
        private GUIStyle m_WhiteLabel;
        private GUIStyle m_WhiteLargeLabel;
        private GUIStyle m_WhiteMiniLabel;
        private GUIStyle m_WordWrappedLabel;
        private GUIStyle m_WordWrappedMiniLabel;
        private static EditorStyles[] s_CachedStyles = new EditorStyles[2];
        internal static EditorStyles s_Current;

        private GUIStyle GetStyle(string styleName)
        {
            GUIStyle error = GUI.skin.FindStyle(styleName);
            if (error == null)
            {
                error = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle(styleName);
            }
            if (error == null)
            {
                Debug.LogError("Missing built-in guistyle " + styleName);
                error = GUISkin.error;
            }
            return error;
        }

        private void InitSharedStyles()
        {
            this.m_ColorPickerBox = this.GetStyle("ColorPickerBox");
            this.m_InspectorBig = this.GetStyle("In BigTitle");
            this.m_MiniLabel = this.GetStyle("miniLabel");
            this.m_LargeLabel = this.GetStyle("LargeLabel");
            this.m_BoldLabel = this.GetStyle("BoldLabel");
            this.m_MiniBoldLabel = this.GetStyle("MiniBoldLabel");
            this.m_WordWrappedLabel = this.GetStyle("WordWrappedLabel");
            this.m_WordWrappedMiniLabel = this.GetStyle("WordWrappedMiniLabel");
            this.m_WhiteLabel = this.GetStyle("WhiteLabel");
            this.m_WhiteMiniLabel = this.GetStyle("WhiteMiniLabel");
            this.m_WhiteLargeLabel = this.GetStyle("WhiteLargeLabel");
            this.m_WhiteBoldLabel = this.GetStyle("WhiteBoldLabel");
            this.m_MiniTextField = this.GetStyle("MiniTextField");
            this.m_RadioButton = this.GetStyle("Radio");
            this.m_MiniButton = this.GetStyle("miniButton");
            this.m_MiniButtonLeft = this.GetStyle("miniButtonLeft");
            this.m_MiniButtonMid = this.GetStyle("miniButtonMid");
            this.m_MiniButtonRight = this.GetStyle("miniButtonRight");
            this.m_Toolbar = this.GetStyle("toolbar");
            this.m_ToolbarButton = this.GetStyle("toolbarbutton");
            this.m_ToolbarPopup = this.GetStyle("toolbarPopup");
            this.m_ToolbarDropDown = this.GetStyle("toolbarDropDown");
            this.m_ToolbarTextField = this.GetStyle("toolbarTextField");
            this.m_ToolbarSearchField = this.GetStyle("ToolbarSeachTextField");
            this.m_ToolbarSearchFieldPopup = this.GetStyle("ToolbarSeachTextFieldPopup");
            this.m_ToolbarSearchFieldCancelButton = this.GetStyle("ToolbarSeachCancelButton");
            this.m_ToolbarSearchFieldCancelButtonEmpty = this.GetStyle("ToolbarSeachCancelButtonEmpty");
            this.m_SearchField = this.GetStyle("SearchTextField");
            this.m_SearchFieldCancelButton = this.GetStyle("SearchCancelButton");
            this.m_SearchFieldCancelButtonEmpty = this.GetStyle("SearchCancelButtonEmpty");
            this.m_HelpBox = this.GetStyle("HelpBox");
            this.m_AssetLabel = this.GetStyle("AssetLabel");
            this.m_AssetLabelPartial = this.GetStyle("AssetLabel Partial");
            this.m_AssetLabelIcon = this.GetStyle("AssetLabel Icon");
            this.m_SelectionRect = this.GetStyle("selectionRect");
            this.m_MinMaxHorizontalSliderThumb = this.GetStyle("MinMaxHorizontalSliderThumb");
            this.m_DropDownList = this.GetStyle("DropDownButton");
            this.m_BoldFont = this.GetStyle("BoldLabel").font;
            this.m_StandardFont = this.GetStyle("Label").font;
            this.m_MiniFont = this.GetStyle("MiniLabel").font;
            this.m_MiniBoldFont = this.GetStyle("MiniBoldLabel").font;
            this.m_ProgressBarBack = this.GetStyle("ProgressBarBack");
            this.m_ProgressBarBar = this.GetStyle("ProgressBarBar");
            this.m_ProgressBarText = this.GetStyle("ProgressBarText");
            this.m_FoldoutPreDrop = this.GetStyle("FoldoutPreDrop");
            this.m_InspectorTitlebar = this.GetStyle("IN Title");
            this.m_InspectorTitlebarText = this.GetStyle("IN TitleText");
            this.m_ToggleGroup = this.GetStyle("BoldToggle");
            this.m_Tooltip = this.GetStyle("Tooltip");
            this.m_NotificationText = this.GetStyle("NotificationText");
            this.m_NotificationBackground = this.GetStyle("NotificationBackground");
            this.m_Popup = this.m_LayerMaskField = this.GetStyle("MiniPopup");
            this.m_TextField = this.m_NumberField = this.GetStyle("textField");
            this.m_Label = this.GetStyle("ControlLabel");
            this.m_ObjectField = this.GetStyle("ObjectField");
            this.m_ObjectFieldThumb = this.GetStyle("ObjectFieldThumb");
            this.m_ObjectFieldMiniThumb = this.GetStyle("ObjectFieldMiniThumb");
            this.m_Toggle = this.GetStyle("Toggle");
            this.m_ToggleMixed = this.GetStyle("ToggleMixed");
            this.m_ColorField = this.GetStyle("ColorField");
            this.m_Foldout = this.GetStyle("Foldout");
            this.m_FoldoutSelected = GUIStyle.none;
            this.m_TextFieldDropDown = this.GetStyle("TextFieldDropDown");
            this.m_TextFieldDropDownText = this.GetStyle("TextFieldDropDownText");
            this.m_TextArea = new GUIStyle(this.m_TextField);
            this.m_TextArea.wordWrap = true;
            this.m_InspectorDefaultMargins = new GUIStyle();
            this.m_InspectorDefaultMargins.padding = new RectOffset(14, 4, 0, 0);
            this.m_InspectorFullWidthMargins = new GUIStyle();
            this.m_InspectorFullWidthMargins.padding = new RectOffset(5, 4, 0, 0);
            this.m_CenteredGreyMiniLabel = new GUIStyle(this.m_MiniLabel);
            this.m_CenteredGreyMiniLabel.alignment = TextAnchor.MiddleCenter;
            this.m_CenteredGreyMiniLabel.normal.textColor = Color.grey;
        }

        internal static void UpdateSkinCache()
        {
            UpdateSkinCache(EditorGUIUtility.skinIndex);
        }

        internal static void UpdateSkinCache(int skinIndex)
        {
            if (GUIUtility.s_SkinMode != 0)
            {
                if (s_CachedStyles[skinIndex] == null)
                {
                    s_CachedStyles[skinIndex] = new EditorStyles();
                    s_CachedStyles[skinIndex].InitSharedStyles();
                }
                s_Current = s_CachedStyles[skinIndex];
                EditorGUIUtility.s_FontIsBold = -1;
                EditorGUIUtility.SetBoldDefaultFont(false);
            }
        }

        internal static GUIStyle assetLabel
        {
            get
            {
                return s_Current.m_AssetLabel;
            }
        }

        internal static GUIStyle assetLabelIcon
        {
            get
            {
                return s_Current.m_AssetLabelIcon;
            }
        }

        internal static GUIStyle assetLabelPartial
        {
            get
            {
                return s_Current.m_AssetLabelPartial;
            }
        }

        public static Font boldFont
        {
            get
            {
                return s_Current.m_BoldFont;
            }
        }

        public static GUIStyle boldLabel
        {
            get
            {
                return s_Current.m_BoldLabel;
            }
        }

        public static GUIStyle centeredGreyMiniLabel
        {
            get
            {
                return s_Current.m_CenteredGreyMiniLabel;
            }
        }

        public static GUIStyle colorField
        {
            get
            {
                return s_Current.m_ColorField;
            }
        }

        internal static GUIStyle colorPickerBox
        {
            get
            {
                return s_Current.m_ColorPickerBox;
            }
        }

        internal static GUIStyle dropDownList
        {
            get
            {
                return s_Current.m_DropDownList;
            }
        }

        public static GUIStyle foldout
        {
            get
            {
                return s_Current.m_Foldout;
            }
        }

        public static GUIStyle foldoutPreDrop
        {
            get
            {
                return s_Current.m_FoldoutPreDrop;
            }
        }

        internal static GUIStyle foldoutSelected
        {
            get
            {
                return s_Current.m_FoldoutSelected;
            }
        }

        public static GUIStyle helpBox
        {
            get
            {
                return s_Current.m_HelpBox;
            }
        }

        internal static GUIStyle inspectorBig
        {
            get
            {
                return s_Current.m_InspectorBig;
            }
        }

        public static GUIStyle inspectorDefaultMargins
        {
            get
            {
                return s_Current.m_InspectorDefaultMargins;
            }
        }

        public static GUIStyle inspectorFullWidthMargins
        {
            get
            {
                return s_Current.m_InspectorFullWidthMargins;
            }
        }

        internal static GUIStyle inspectorTitlebar
        {
            get
            {
                return s_Current.m_InspectorTitlebar;
            }
        }

        internal static GUIStyle inspectorTitlebarText
        {
            get
            {
                return s_Current.m_InspectorTitlebarText;
            }
        }

        internal static Vector2 knobSize
        {
            get
            {
                return s_Current.m_KnobSize;
            }
        }

        public static GUIStyle label
        {
            get
            {
                return s_Current.m_Label;
            }
        }

        public static GUIStyle largeLabel
        {
            get
            {
                return s_Current.m_LargeLabel;
            }
        }

        public static GUIStyle layerMaskField
        {
            get
            {
                return s_Current.m_LayerMaskField;
            }
        }

        public static Font miniBoldFont
        {
            get
            {
                return s_Current.m_MiniBoldFont;
            }
        }

        public static GUIStyle miniBoldLabel
        {
            get
            {
                return s_Current.m_MiniBoldLabel;
            }
        }

        public static GUIStyle miniButton
        {
            get
            {
                return s_Current.m_MiniButton;
            }
        }

        public static GUIStyle miniButtonLeft
        {
            get
            {
                return s_Current.m_MiniButtonLeft;
            }
        }

        public static GUIStyle miniButtonMid
        {
            get
            {
                return s_Current.m_MiniButtonMid;
            }
        }

        public static GUIStyle miniButtonRight
        {
            get
            {
                return s_Current.m_MiniButtonRight;
            }
        }

        public static Font miniFont
        {
            get
            {
                return s_Current.m_MiniFont;
            }
        }

        internal static Vector2 miniKnobSize
        {
            get
            {
                return s_Current.m_MiniKnobSize;
            }
        }

        public static GUIStyle miniLabel
        {
            get
            {
                return s_Current.m_MiniLabel;
            }
        }

        public static GUIStyle miniTextField
        {
            get
            {
                return s_Current.m_MiniTextField;
            }
        }

        internal static GUIStyle minMaxHorizontalSliderThumb
        {
            get
            {
                return s_Current.m_MinMaxHorizontalSliderThumb;
            }
        }

        internal static GUIStyle notificationBackground
        {
            get
            {
                return s_Current.m_NotificationBackground;
            }
        }

        internal static GUIStyle notificationText
        {
            get
            {
                return s_Current.m_NotificationText;
            }
        }

        public static GUIStyle numberField
        {
            get
            {
                return s_Current.m_NumberField;
            }
        }

        public static GUIStyle objectField
        {
            get
            {
                return s_Current.m_ObjectField;
            }
        }

        public static GUIStyle objectFieldMiniThumb
        {
            get
            {
                return s_Current.m_ObjectFieldMiniThumb;
            }
        }

        public static GUIStyle objectFieldThumb
        {
            get
            {
                return s_Current.m_ObjectFieldThumb;
            }
        }

        public static GUIStyle popup
        {
            get
            {
                return s_Current.m_Popup;
            }
        }

        internal static GUIStyle progressBarBack
        {
            get
            {
                return s_Current.m_ProgressBarBack;
            }
        }

        internal static GUIStyle progressBarBar
        {
            get
            {
                return s_Current.m_ProgressBarBar;
            }
        }

        internal static GUIStyle progressBarText
        {
            get
            {
                return s_Current.m_ProgressBarText;
            }
        }

        public static GUIStyle radioButton
        {
            get
            {
                return s_Current.m_RadioButton;
            }
        }

        internal static GUIStyle searchField
        {
            get
            {
                return s_Current.m_SearchField;
            }
        }

        internal static GUIStyle searchFieldCancelButton
        {
            get
            {
                return s_Current.m_SearchFieldCancelButton;
            }
        }

        internal static GUIStyle searchFieldCancelButtonEmpty
        {
            get
            {
                return s_Current.m_SearchFieldCancelButtonEmpty;
            }
        }

        internal static GUIStyle selectionRect
        {
            get
            {
                return s_Current.m_SelectionRect;
            }
        }

        public static Font standardFont
        {
            get
            {
                return s_Current.m_StandardFont;
            }
        }

        [Obsolete("structHeadingLabel is deprecated, use EditorStyles.label instead.")]
        public static GUIStyle structHeadingLabel
        {
            get
            {
                return s_Current.m_Label;
            }
        }

        public static GUIStyle textArea
        {
            get
            {
                return s_Current.m_TextArea;
            }
        }

        public static GUIStyle textField
        {
            get
            {
                return s_Current.m_TextField;
            }
        }

        internal static GUIStyle textFieldDropDown
        {
            get
            {
                return s_Current.m_TextFieldDropDown;
            }
        }

        internal static GUIStyle textFieldDropDownText
        {
            get
            {
                return s_Current.m_TextFieldDropDownText;
            }
        }

        public static GUIStyle toggle
        {
            get
            {
                return s_Current.m_Toggle;
            }
        }

        public static GUIStyle toggleGroup
        {
            get
            {
                return s_Current.m_ToggleGroup;
            }
        }

        internal static GUIStyle toggleMixed
        {
            get
            {
                return s_Current.m_ToggleMixed;
            }
        }

        public static GUIStyle toolbar
        {
            get
            {
                return s_Current.m_Toolbar;
            }
        }

        public static GUIStyle toolbarButton
        {
            get
            {
                return s_Current.m_ToolbarButton;
            }
        }

        public static GUIStyle toolbarDropDown
        {
            get
            {
                return s_Current.m_ToolbarDropDown;
            }
        }

        public static GUIStyle toolbarPopup
        {
            get
            {
                return s_Current.m_ToolbarPopup;
            }
        }

        internal static GUIStyle toolbarSearchField
        {
            get
            {
                return s_Current.m_ToolbarSearchField;
            }
        }

        internal static GUIStyle toolbarSearchFieldCancelButton
        {
            get
            {
                return s_Current.m_ToolbarSearchFieldCancelButton;
            }
        }

        internal static GUIStyle toolbarSearchFieldCancelButtonEmpty
        {
            get
            {
                return s_Current.m_ToolbarSearchFieldCancelButtonEmpty;
            }
        }

        internal static GUIStyle toolbarSearchFieldPopup
        {
            get
            {
                return s_Current.m_ToolbarSearchFieldPopup;
            }
        }

        public static GUIStyle toolbarTextField
        {
            get
            {
                return s_Current.m_ToolbarTextField;
            }
        }

        internal static GUIStyle tooltip
        {
            get
            {
                return s_Current.m_Tooltip;
            }
        }

        public static GUIStyle whiteBoldLabel
        {
            get
            {
                return s_Current.m_WhiteBoldLabel;
            }
        }

        public static GUIStyle whiteLabel
        {
            get
            {
                return s_Current.m_WhiteLabel;
            }
        }

        public static GUIStyle whiteLargeLabel
        {
            get
            {
                return s_Current.m_WhiteLargeLabel;
            }
        }

        public static GUIStyle whiteMiniLabel
        {
            get
            {
                return s_Current.m_WhiteMiniLabel;
            }
        }

        public static GUIStyle wordWrappedLabel
        {
            get
            {
                return s_Current.m_WordWrappedLabel;
            }
        }

        public static GUIStyle wordWrappedMiniLabel
        {
            get
            {
                return s_Current.m_WordWrappedMiniLabel;
            }
        }
    }
}

