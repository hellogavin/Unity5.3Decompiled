namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class GradientPicker : EditorWindow
    {
        private const int k_DefaultNumSteps = 0;
        private GUIView m_DelegateView;
        private Gradient m_Gradient;
        private GradientEditor m_GradientEditor;
        private PresetLibraryEditor<GradientPresetLibrary> m_GradientLibraryEditor;
        [SerializeField]
        private PresetLibraryEditorState m_GradientLibraryEditorState;
        private static GradientPicker s_GradientPicker;

        private GradientPicker()
        {
            base.hideFlags = HideFlags.DontSave;
        }

        public static void CloseWindow()
        {
            if (s_GradientPicker != null)
            {
                s_GradientPicker.Close();
                GUIUtility.ExitGUI();
            }
        }

        private void Init(Gradient newGradient)
        {
            this.m_Gradient = newGradient;
            if (this.m_GradientEditor != null)
            {
                this.m_GradientEditor.Init(newGradient, 0);
            }
            base.Repaint();
        }

        private void InitIfNeeded()
        {
            if (this.m_GradientEditor == null)
            {
                this.m_GradientEditor = new GradientEditor();
                this.m_GradientEditor.Init(this.m_Gradient, 0);
            }
            if (this.m_GradientLibraryEditorState == null)
            {
                this.m_GradientLibraryEditorState = new PresetLibraryEditorState(presetsEditorPrefID);
                this.m_GradientLibraryEditorState.TransferEditorPrefsState(true);
            }
            if (this.m_GradientLibraryEditor == null)
            {
                ScriptableObjectSaveLoadHelper<GradientPresetLibrary> helper = new ScriptableObjectSaveLoadHelper<GradientPresetLibrary>("gradients", SaveType.Text);
                this.m_GradientLibraryEditor = new PresetLibraryEditor<GradientPresetLibrary>(helper, this.m_GradientLibraryEditorState, new Action<int, object>(this.PresetClickedCallback));
                this.m_GradientLibraryEditor.showHeader = true;
                this.m_GradientLibraryEditor.minMaxPreviewHeight = new Vector2(14f, 14f);
            }
        }

        public void OnDestroy()
        {
            this.m_GradientLibraryEditor.UnloadUsedLibraries();
        }

        public void OnDisable()
        {
            if (this.m_GradientLibraryEditorState != null)
            {
                this.m_GradientLibraryEditorState.TransferEditorPrefsState(false);
            }
            if (this.m_GradientEditor != null)
            {
                this.m_GradientEditor.Clear();
            }
            s_GradientPicker = null;
        }

        public void OnEnable()
        {
        }

        public void OnGUI()
        {
            if (this.m_Gradient != null)
            {
                this.InitIfNeeded();
                float num = Mathf.Min(base.position.height, 120f);
                float num2 = 10f;
                float height = (base.position.height - num) - num2;
                Rect position = new Rect(10f, 10f, base.position.width - 20f, num - 20f);
                Rect rect = new Rect(0f, num + num2, base.position.width, height);
                EditorGUI.DrawRect(new Rect(rect.x, rect.y - 1f, rect.width, 1f), new Color(0f, 0f, 0f, 0.3f));
                EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 1f), new Color(1f, 1f, 1f, 0.1f));
                EditorGUI.BeginChangeCheck();
                this.m_GradientEditor.OnGUI(position);
                if (EditorGUI.EndChangeCheck())
                {
                    this.gradientChanged = true;
                }
                this.m_GradientLibraryEditor.OnGUI(rect, this.m_Gradient);
                if (this.gradientChanged)
                {
                    this.gradientChanged = false;
                    this.SendEvent(true);
                }
            }
        }

        private void OnPlayModeStateChanged()
        {
            base.Close();
        }

        private void PresetClickedCallback(int clickCount, object presetObject)
        {
            Gradient gradient = presetObject as Gradient;
            if (gradient == null)
            {
                Debug.LogError("Incorrect object passed " + presetObject);
            }
            SetCurrentGradient(gradient);
            this.gradientChanged = true;
        }

        public static void RepaintWindow()
        {
            if (s_GradientPicker != null)
            {
                s_GradientPicker.Repaint();
            }
        }

        private void SendEvent(bool exitGUI)
        {
            if (this.m_DelegateView != null)
            {
                Event e = EditorGUIUtility.CommandEvent("GradientPickerChanged");
                base.Repaint();
                this.m_DelegateView.SendEvent(e);
                if (exitGUI)
                {
                    GUIUtility.ExitGUI();
                }
            }
        }

        public static void SetCurrentGradient(Gradient gradient)
        {
            if (s_GradientPicker != null)
            {
                s_GradientPicker.SetGradientData(gradient);
                GUI.changed = true;
            }
        }

        private void SetGradientData(Gradient gradient)
        {
            this.m_Gradient.colorKeys = gradient.colorKeys;
            this.m_Gradient.alphaKeys = gradient.alphaKeys;
            this.Init(this.m_Gradient);
        }

        public static void Show(Gradient newGradient)
        {
            GUIView current = GUIView.current;
            if (s_GradientPicker == null)
            {
                s_GradientPicker = (GradientPicker) EditorWindow.GetWindow(typeof(GradientPicker), true, "Gradient Editor", false);
                Vector2 vector = new Vector2(360f, 224f);
                Vector2 vector2 = new Vector2(1900f, 3000f);
                s_GradientPicker.minSize = vector;
                s_GradientPicker.maxSize = vector2;
                s_GradientPicker.wantsMouseMove = true;
                s_GradientPicker.ShowAuxWindow();
            }
            else
            {
                s_GradientPicker.Repaint();
            }
            s_GradientPicker.m_DelegateView = current;
            s_GradientPicker.Init(newGradient);
        }

        public string currentPresetLibrary
        {
            get
            {
                this.InitIfNeeded();
                return this.m_GradientLibraryEditor.currentLibraryWithoutExtension;
            }
            set
            {
                this.InitIfNeeded();
                this.m_GradientLibraryEditor.currentLibraryWithoutExtension = value;
            }
        }

        public static Gradient gradient
        {
            get
            {
                if (s_GradientPicker != null)
                {
                    return s_GradientPicker.m_Gradient;
                }
                return null;
            }
        }

        private bool gradientChanged { get; set; }

        public static GradientPicker instance
        {
            get
            {
                if (s_GradientPicker == null)
                {
                    Debug.LogError("Gradient Picker not initalized, did you call Show first?");
                }
                return s_GradientPicker;
            }
        }

        public static string presetsEditorPrefID
        {
            get
            {
                return "Gradient";
            }
        }

        public static bool visible
        {
            get
            {
                return (s_GradientPicker != null);
            }
        }
    }
}

