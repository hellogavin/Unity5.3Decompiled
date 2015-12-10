namespace UnityEditor
{
    using System;
    using UnityEditorInternal;
    using UnityEngine;

    internal class EyeDropper : GUIView
    {
        private const int kDummyWindowSize = 0x2710;
        private const int kPixelSize = 10;
        private GUIView m_DelegateView;
        private bool m_Focused;
        private Texture2D m_Preview;
        private static EyeDropper s_Instance;
        internal static Color s_LastPickedColor;
        private static Vector2 s_PickCoordinates = Vector2.zero;
        private static Styles styles;

        private EyeDropper()
        {
            s_Instance = this;
        }

        public static void DrawPreview(Rect position)
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (styles == null)
                {
                    styles = new Styles();
                }
                Texture2D preview = get.m_Preview;
                int width = (int) Mathf.Ceil(position.width / 10f);
                int height = (int) Mathf.Ceil(position.height / 10f);
                if (preview == null)
                {
                    get.m_Preview = preview = ColorPicker.MakeTexture(width, height);
                    preview.filterMode = FilterMode.Point;
                }
                if ((preview.width != width) || (preview.height != height))
                {
                    preview.Resize(width, height);
                }
                Vector2 vector = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                Vector2 pixelPos = vector - new Vector2((float) (width / 2), (float) (height / 2));
                preview.SetPixels(InternalEditorUtility.ReadScreenPixel(pixelPos, width, height), 0);
                preview.Apply(true);
                Graphics.DrawTexture(position, preview);
                float num3 = position.width / ((float) width);
                GUIStyle eyeDropperVerticalLine = styles.eyeDropperVerticalLine;
                for (float i = position.x; i < position.xMax; i += num3)
                {
                    Rect rect = new Rect(Mathf.Round(i), position.y, num3, position.height);
                    eyeDropperVerticalLine.Draw(rect, false, false, false, false);
                }
                float num5 = position.height / ((float) height);
                eyeDropperVerticalLine = styles.eyeDropperHorizontalLine;
                for (float j = position.y; j < position.yMax; j += num5)
                {
                    Rect rect2 = new Rect(position.x, Mathf.Floor(j), position.width, num5);
                    eyeDropperVerticalLine.Draw(rect2, false, false, false, false);
                }
                Rect rect3 = new Rect(((vector.x - pixelPos.x) * num3) + position.x, ((vector.y - pixelPos.y) * num5) + position.y, num3, num5);
                styles.eyeDropperPickedPixel.Draw(rect3, false, false, false, false);
            }
        }

        public static Color GetLastPickedColor()
        {
            return s_LastPickedColor;
        }

        public static Color GetPickedColor()
        {
            return InternalEditorUtility.ReadScreenPixel(s_PickCoordinates, 1, 1)[0];
        }

        public void OnDestroy()
        {
            if (this.m_Preview != null)
            {
                Object.DestroyImmediate(this.m_Preview);
            }
            if (!this.m_Focused)
            {
                this.SendEvent("EyeDropperCancelled", false);
            }
        }

        protected override bool OnFocus()
        {
            this.m_Focused = true;
            return base.OnFocus();
        }

        private void OnGUI()
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (Event.current.button == 0)
                    {
                        s_PickCoordinates = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                        base.window.Close();
                        s_LastPickedColor = GetPickedColor();
                        this.SendEvent("EyeDropperClicked", true);
                    }
                    break;

                case EventType.MouseMove:
                    s_PickCoordinates = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                    base.StealMouseCapture();
                    this.SendEvent("EyeDropperUpdate", true);
                    break;

                case EventType.KeyDown:
                    if (Event.current.keyCode == KeyCode.Escape)
                    {
                        base.window.Close();
                        this.SendEvent("EyeDropperCancelled", true);
                    }
                    break;
            }
        }

        private void SendEvent(string eventName, bool exitGUI)
        {
            if (this.m_DelegateView != null)
            {
                Event e = EditorGUIUtility.CommandEvent(eventName);
                this.m_DelegateView.SendEvent(e);
                if (exitGUI)
                {
                    GUIUtility.ExitGUI();
                }
            }
        }

        private void Show(GUIView sourceView)
        {
            this.m_DelegateView = sourceView;
            ContainerWindow window = ScriptableObject.CreateInstance<ContainerWindow>();
            window.m_DontSaveToLayout = true;
            window.title = "EyeDropper";
            window.hideFlags = HideFlags.DontSave;
            window.mainView = this;
            window.Show(ShowMode.PopupMenu, true, false);
            base.AddToAuxWindowList();
            window.SetInvisible();
            base.SetMinMaxSizes(new Vector2(0f, 0f), new Vector2(10000f, 10000f));
            window.position = new Rect(-5000f, -5000f, 10000f, 10000f);
            base.wantsMouseMove = true;
            base.StealMouseCapture();
        }

        public static void Start(GUIView viewToUpdate)
        {
            get.Show(viewToUpdate);
        }

        private static EyeDropper get
        {
            get
            {
                if (s_Instance == null)
                {
                    ScriptableObject.CreateInstance<EyeDropper>();
                }
                return s_Instance;
            }
        }

        private class Styles
        {
            public GUIStyle eyeDropperHorizontalLine = "EyeDropperHorizontalLine";
            public GUIStyle eyeDropperPickedPixel = "EyeDropperPickedPixel";
            public GUIStyle eyeDropperVerticalLine = "EyeDropperVerticalLine";
        }
    }
}

