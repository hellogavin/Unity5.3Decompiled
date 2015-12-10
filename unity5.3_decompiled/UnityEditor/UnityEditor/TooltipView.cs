namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class TooltipView : GUIView
    {
        private Rect m_hoverRect;
        private Vector2 m_optimalSize;
        private GUIStyle m_Style;
        private GUIContent m_tooltip = new GUIContent();
        private ContainerWindow m_tooltipContainer;
        private const float MAX_WIDTH = 300f;
        private static TooltipView s_guiView;

        public static void Close()
        {
            if (s_guiView != null)
            {
                s_guiView.m_tooltipContainer.Close();
            }
        }

        private void OnDisable()
        {
            s_guiView = null;
        }

        private void OnEnable()
        {
            s_guiView = this;
        }

        private void OnGUI()
        {
            if (this.m_tooltipContainer != null)
            {
                GUI.Box(new Rect(0f, 0f, this.m_optimalSize.x, this.m_optimalSize.y), this.m_tooltip, this.m_Style);
            }
        }

        public static void SetAlpha(float percent)
        {
            if (s_guiView != null)
            {
                s_guiView.m_tooltipContainer.SetAlpha(percent);
            }
        }

        private void Setup(string tooltip, Rect rect)
        {
            this.m_hoverRect = rect;
            this.m_tooltip.text = tooltip;
            this.m_Style = EditorStyles.tooltip;
            this.m_Style.wordWrap = false;
            this.m_optimalSize = this.m_Style.CalcSize(this.m_tooltip);
            if (this.m_optimalSize.x > 300f)
            {
                this.m_Style.wordWrap = true;
                this.m_optimalSize.x = 300f;
                this.m_optimalSize.y = this.m_Style.CalcHeight(this.m_tooltip, 300f);
            }
            float x = Mathf.Floor((this.m_hoverRect.x + (this.m_hoverRect.width / 2f)) - (this.m_optimalSize.x / 2f));
            this.m_tooltipContainer.position = new Rect(x, Mathf.Floor((this.m_hoverRect.y + this.m_hoverRect.height) + 10f), this.m_optimalSize.x, this.m_optimalSize.y);
            base.position = new Rect(0f, 0f, this.m_optimalSize.x, this.m_optimalSize.y);
            this.m_tooltipContainer.ShowPopup();
            this.m_tooltipContainer.SetAlpha(1f);
            s_guiView.mouseRayInvisible = true;
            base.RepaintImmediately();
        }

        public static void Show(string tooltip, Rect rect)
        {
            if (s_guiView == null)
            {
                s_guiView = ScriptableObject.CreateInstance<TooltipView>();
                s_guiView.m_tooltipContainer = ScriptableObject.CreateInstance<ContainerWindow>();
                s_guiView.m_tooltipContainer.m_DontSaveToLayout = true;
                s_guiView.m_tooltipContainer.mainView = s_guiView;
                s_guiView.m_tooltipContainer.SetMinMaxSizes(new Vector2(10f, 10f), new Vector2(2000f, 2000f));
            }
            if ((s_guiView.m_tooltip.text != tooltip) || (rect != s_guiView.m_hoverRect))
            {
                s_guiView.Setup(tooltip, rect);
            }
        }
    }
}

