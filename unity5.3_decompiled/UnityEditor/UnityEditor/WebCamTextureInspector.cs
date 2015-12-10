namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(WebCamTexture))]
    internal class WebCamTextureInspector : Editor
    {
        private Vector2 m_Pos;
        private static GUIContent[] s_PlayIcons = new GUIContent[2];

        public override string GetInfoString()
        {
            Texture target = this.target as Texture;
            string str = target.width.ToString() + "x" + target.height.ToString();
            TextureFormat textureFormat = TextureUtil.GetTextureFormat(target);
            return (str + "  " + TextureUtil.GetTextureFormatString(textureFormat));
        }

        public override bool HasPreviewGUI()
        {
            return (this.target != null);
        }

        private static void Init()
        {
            s_PlayIcons[0] = EditorGUIUtility.IconContent("preAudioPlayOff");
            s_PlayIcons[1] = EditorGUIUtility.IconContent("preAudioPlayOn");
        }

        public void OnDisable()
        {
            WebCamTexture target = this.target as WebCamTexture;
            if (!Application.isPlaying && (target != null))
            {
                target.Stop();
            }
        }

        public override void OnInspectorGUI()
        {
            WebCamTexture target = this.target as WebCamTexture;
            EditorGUILayout.LabelField("Requested FPS", target.requestedFPS.ToString(), new GUILayoutOption[0]);
            EditorGUILayout.LabelField("Requested Width", target.requestedWidth.ToString(), new GUILayoutOption[0]);
            EditorGUILayout.LabelField("Requested Height", target.requestedHeight.ToString(), new GUILayoutOption[0]);
            EditorGUILayout.LabelField("Device Name", target.deviceName, new GUILayoutOption[0]);
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (Event.current.type == EventType.Repaint)
            {
                background.Draw(r, false, false, false, false);
            }
            WebCamTexture target = this.target as WebCamTexture;
            float num = Mathf.Min(Mathf.Min((float) (r.width / ((float) target.width)), (float) (r.height / ((float) target.height))), 1f);
            Rect viewRect = new Rect(r.x, r.y, target.width * num, target.height * num);
            PreviewGUI.BeginScrollView(r, this.m_Pos, viewRect, "PreHorizontalScrollbar", "PreHorizontalScrollbarThumb");
            GUI.DrawTexture(viewRect, target, ScaleMode.StretchToFill, false);
            this.m_Pos = PreviewGUI.EndScrollView();
            if (target.isPlaying)
            {
                GUIView.current.Repaint();
            }
            if (Application.isPlaying)
            {
                if (target.isPlaying)
                {
                    EditorGUI.DropShadowLabel(new Rect(r.x, r.y + 10f, r.width, 20f), "Can't pause preview when in play mode");
                }
                else
                {
                    EditorGUI.DropShadowLabel(new Rect(r.x, r.y + 10f, r.width, 20f), "Can't start preview when in play mode");
                }
            }
        }

        public override void OnPreviewSettings()
        {
            Init();
            GUI.enabled = !Application.isPlaying;
            WebCamTexture target = this.target as WebCamTexture;
            bool flag = PreviewGUI.CycleButton(!target.isPlaying ? 0 : 1, s_PlayIcons) != 0;
            if (flag != target.isPlaying)
            {
                if (flag)
                {
                    target.Stop();
                    target.Play();
                }
                else
                {
                    target.Pause();
                }
            }
            GUI.enabled = true;
        }
    }
}

