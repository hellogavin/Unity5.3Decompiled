namespace UnityEditor
{
    using System;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;

    internal class PaneDragTab : GUIView
    {
        public GUIContent content;
        private const float kTopThumbnailOffset = 10f;
        private bool m_DidResizeOnLastLayout;
        [SerializeField]
        private ContainerWindow m_InFrontOfWindow;
        private AnimBool m_PaneVisible = new AnimBool();
        [SerializeField]
        private bool m_Shadow;
        private float m_StartAlpha = 1f;
        private Rect m_StartRect;
        private float m_StartTime;
        private AnimBool m_TabVisible = new AnimBool();
        private float m_TargetAlpha = 1f;
        [SerializeField]
        private Rect m_TargetRect;
        private Texture2D m_Thumbnail;
        [SerializeField]
        private Vector2 m_ThumbnailSize = new Vector2(80f, 60f);
        private DropInfo.Type m_Type = ~DropInfo.Type.Tab;
        [SerializeField]
        internal ContainerWindow m_Window;
        private static PaneDragTab s_Get;
        [SerializeField]
        private static GUIStyle s_PaneStyle;
        [SerializeField]
        private static GUIStyle s_TabStyle;

        private float CalcFade()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return 1f;
            }
            return Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(5f * (Time.realtimeSinceStartup - this.m_StartTime)));
        }

        public void Close()
        {
            if (this.m_Thumbnail != null)
            {
                Object.DestroyImmediate(this.m_Thumbnail);
            }
            if (this.m_Window != null)
            {
                this.m_Window.Close();
            }
            Object.DestroyImmediate(this, true);
            s_Get = null;
        }

        private Rect GetInterpolatedRect(float fade)
        {
            float x = Mathf.Lerp(this.m_StartRect.x, this.m_TargetRect.x, fade);
            float y = Mathf.Lerp(this.m_StartRect.y, this.m_TargetRect.y, fade);
            float width = Mathf.Lerp(this.m_StartRect.width, this.m_TargetRect.width, fade);
            return new Rect(x, y, width, Mathf.Lerp(this.m_StartRect.height, this.m_TargetRect.height, fade));
        }

        public void GrabThumbnail()
        {
            if (this.m_Thumbnail != null)
            {
                Object.DestroyImmediate(this.m_Thumbnail);
            }
            this.m_Thumbnail = new Texture2D(Screen.width, Screen.height);
            this.m_Thumbnail.ReadPixels(new Rect(0f, 0f, (float) Screen.width, (float) Screen.height), 0, 0);
            this.m_Thumbnail.Apply();
            float num2 = this.m_Thumbnail.width * this.m_Thumbnail.height;
            this.m_ThumbnailSize = (Vector2) (new Vector2((float) this.m_Thumbnail.width, (float) this.m_Thumbnail.height) * Mathf.Sqrt(Mathf.Clamp01(50000f / num2)));
        }

        public void OnDisable()
        {
            this.m_PaneVisible.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            this.m_TabVisible.valueChanged.RemoveListener(new UnityAction(this.Repaint));
        }

        public void OnEnable()
        {
            this.m_PaneVisible.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_TabVisible.valueChanged.AddListener(new UnityAction(this.Repaint));
        }

        private void OnGUI()
        {
            float fade = this.CalcFade();
            if (s_PaneStyle == null)
            {
                s_PaneStyle = "dragtabdropwindow";
                s_TabStyle = "dragtab";
            }
            if (Event.current.type == EventType.Layout)
            {
                this.m_DidResizeOnLastLayout = !this.m_DidResizeOnLastLayout;
                if (!this.m_DidResizeOnLastLayout)
                {
                    this.SetWindowPos(this.GetInterpolatedRect(fade));
                    if (Application.platform == RuntimePlatform.OSXEditor)
                    {
                        this.m_Window.SetAlpha(Mathf.Lerp(this.m_StartAlpha, this.m_TargetAlpha, fade));
                    }
                    return;
                }
            }
            if (Event.current.type == EventType.Repaint)
            {
                Color color = GUI.color;
                GUI.color = new Color(1f, 1f, 1f, 1f);
                if (this.m_Thumbnail != null)
                {
                    GUI.DrawTexture(new Rect(0f, 0f, base.position.width, base.position.height), this.m_Thumbnail, ScaleMode.StretchToFill, false);
                }
                if (this.m_TabVisible.faded != 0f)
                {
                    GUI.color = new Color(1f, 1f, 1f, this.m_TabVisible.faded);
                    s_TabStyle.Draw(new Rect(0f, 0f, base.position.width, base.position.height), this.content, false, false, true, true);
                }
                if (this.m_PaneVisible.faded != 0f)
                {
                    GUI.color = new Color(1f, 1f, 1f, this.m_PaneVisible.faded);
                    s_PaneStyle.Draw(new Rect(0f, 0f, base.position.width, base.position.height), this.content, false, false, true, true);
                }
                GUI.color = color;
            }
            if (Application.platform != RuntimePlatform.WindowsEditor)
            {
                base.Repaint();
            }
        }

        public void SetDropInfo(DropInfo di, Vector2 mouseScreenPos, ContainerWindow inFrontOf)
        {
            if ((this.m_Type != di.type) || ((di.type == DropInfo.Type.Pane) && (di.rect != this.m_TargetRect)))
            {
                this.m_Type = di.type;
                this.m_StartRect = this.GetInterpolatedRect(this.CalcFade());
                this.m_StartTime = Time.realtimeSinceStartup;
                switch (di.type)
                {
                    case DropInfo.Type.Tab:
                    case DropInfo.Type.Pane:
                        this.m_TargetAlpha = 1f;
                        break;

                    case DropInfo.Type.Window:
                        this.m_TargetAlpha = 0.6f;
                        break;
                }
            }
            switch (di.type)
            {
                case DropInfo.Type.Tab:
                case DropInfo.Type.Pane:
                    this.m_TargetRect = di.rect;
                    break;

                case DropInfo.Type.Window:
                    this.m_TargetRect = new Rect(mouseScreenPos.x - (this.m_ThumbnailSize.x / 2f), mouseScreenPos.y - (this.m_ThumbnailSize.y / 2f), this.m_ThumbnailSize.x, this.m_ThumbnailSize.y);
                    break;
            }
            this.m_PaneVisible.target = di.type == DropInfo.Type.Pane;
            this.m_TabVisible.target = di.type == DropInfo.Type.Tab;
            this.m_TargetRect.x = Mathf.Round(this.m_TargetRect.x);
            this.m_TargetRect.y = Mathf.Round(this.m_TargetRect.y);
            this.m_TargetRect.width = Mathf.Round(this.m_TargetRect.width);
            this.m_TargetRect.height = Mathf.Round(this.m_TargetRect.height);
            this.m_InFrontOfWindow = inFrontOf;
            this.m_Window.MoveInFrontOf(this.m_InFrontOfWindow);
            this.SetWindowPos(this.GetInterpolatedRect(this.CalcFade()));
            base.Repaint();
        }

        private void SetWindowPos(Rect screenPosition)
        {
            this.m_Window.position = screenPosition;
        }

        public void Show(Rect pixelPos, Vector2 mouseScreenPosition)
        {
            if (this.m_Window == null)
            {
                this.m_Window = ScriptableObject.CreateInstance<ContainerWindow>();
                this.m_Window.m_DontSaveToLayout = true;
                base.SetMinMaxSizes(Vector2.zero, new Vector2(10000f, 10000f));
                this.SetWindowPos(pixelPos);
                this.m_Window.mainView = this;
            }
            else
            {
                this.SetWindowPos(pixelPos);
            }
            this.m_Window.Show(ShowMode.NoShadow, true, false);
            this.m_TargetRect = pixelPos;
        }

        public static PaneDragTab get
        {
            get
            {
                if (s_Get == null)
                {
                    Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(PaneDragTab));
                    if (objArray.Length != 0)
                    {
                        s_Get = (PaneDragTab) objArray[0];
                    }
                    if (s_Get != null)
                    {
                        return s_Get;
                    }
                    s_Get = ScriptableObject.CreateInstance<PaneDragTab>();
                }
                return s_Get;
            }
        }
    }
}

