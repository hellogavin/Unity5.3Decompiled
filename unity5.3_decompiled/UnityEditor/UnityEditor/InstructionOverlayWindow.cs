namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class InstructionOverlayWindow : EditorWindow
    {
        private GUIView m_InspectedGUIView;
        private Rect m_InstructionRect;
        private GUIStyle m_InstructionStyle;
        private RenderTexture m_RenderTexture;
        [NonSerialized]
        private bool m_RenderTextureNeedsRefresh;
        private static Styles s_Styles;

        private void DoRefreshRenderTexture()
        {
            if (this.m_RenderTexture == null)
            {
                int width = Mathf.CeilToInt(this.m_InstructionRect.width);
                this.m_RenderTexture = new RenderTexture(width, Mathf.CeilToInt(this.m_InstructionRect.height), 0x18);
                this.m_RenderTexture.Create();
            }
            else if ((this.m_RenderTexture.width != this.m_InstructionRect.width) || (this.m_RenderTexture.height != this.m_InstructionRect.height))
            {
                this.m_RenderTexture.Release();
                this.m_RenderTexture.width = Mathf.CeilToInt(this.m_InstructionRect.width);
                this.m_RenderTexture.height = Mathf.CeilToInt(this.m_InstructionRect.height);
                this.m_RenderTexture.Create();
            }
            this.m_RenderTextureNeedsRefresh = false;
            base.Repaint();
        }

        private void OnFocus()
        {
            EditorWindow.GetWindow<GUIViewDebuggerWindow>();
        }

        private void OnGUI()
        {
            Color color = new Color(0.76f, 0.87f, 0.71f);
            Color color2 = new Color(0.62f, 0.77f, 0.9f);
            Rect position = new Rect(0f, 0f, this.m_InstructionRect.width, this.m_InstructionRect.height);
            GUI.backgroundColor = color;
            GUI.Box(position, GUIContent.none, this.styles.solidColor);
            Rect rect2 = this.m_InstructionStyle.padding.Remove(position);
            GUI.backgroundColor = color2;
            GUI.Box(rect2, GUIContent.none, this.styles.solidColor);
        }

        public void SetTransparent(float d)
        {
            base.m_Parent.window.SetAlpha(d);
            base.m_Parent.window.SetInvisible();
        }

        public void Show(GUIView view, Rect instructionRect, GUIStyle style)
        {
            Rect rect;
            base.minSize = Vector2.zero;
            this.m_InstructionStyle = style;
            this.m_InspectedGUIView = view;
            this.m_InstructionRect = instructionRect;
            rect = new Rect(instructionRect) {
                x = rect.x + this.m_InspectedGUIView.screenPosition.x,
                y = rect.y + this.m_InspectedGUIView.screenPosition.y
            };
            base.position = rect;
            this.m_RenderTextureNeedsRefresh = true;
            base.ShowWithMode(ShowMode.NoShadow);
            base.m_Parent.window.m_DontSaveToLayout = true;
            base.Repaint();
        }

        private void Start()
        {
            base.minSize = Vector2.zero;
            base.m_Parent.window.m_DontSaveToLayout = true;
        }

        private void Update()
        {
            if (this.m_RenderTextureNeedsRefresh)
            {
                this.DoRefreshRenderTexture();
            }
        }

        private Styles styles
        {
            get
            {
                if (s_Styles == null)
                {
                    s_Styles = new Styles();
                }
                return s_Styles;
            }
        }

        private class Styles
        {
            public GUIStyle solidColor = new GUIStyle();

            public Styles()
            {
                this.solidColor.normal.background = EditorGUIUtility.whiteTexture;
            }
        }
    }
}

