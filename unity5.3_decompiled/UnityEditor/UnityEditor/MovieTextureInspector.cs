namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(MovieTexture)), CanEditMultipleObjects]
    internal class MovieTextureInspector : TextureInspector
    {
        private static GUIContent[] s_PlayIcons = new GUIContent[2];

        public override string GetInfoString()
        {
            string infoString = base.GetInfoString();
            MovieTexture target = this.target as MovieTexture;
            if (!target.isReadyToPlay)
            {
                infoString = infoString + "/nNot ready to play yet.";
            }
            return infoString;
        }

        private static void Init()
        {
            s_PlayIcons[0] = EditorGUIUtility.IconContent("preAudioPlayOff");
            s_PlayIcons[1] = EditorGUIUtility.IconContent("preAudioPlayOn");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            MovieTexture target = this.target as MovieTexture;
            if (!Application.isPlaying && (target != null))
            {
                AudioClip audioClip = target.audioClip;
                target.Stop();
                if (audioClip != null)
                {
                    AudioUtil.StopClip(audioClip);
                }
            }
        }

        protected override void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (Event.current.type == EventType.Repaint)
            {
                background.Draw(r, false, false, false, false);
            }
            MovieTexture target = this.target as MovieTexture;
            float num = Mathf.Min(Mathf.Min((float) (r.width / ((float) target.width)), (float) (r.height / ((float) target.height))), 1f);
            Rect viewRect = new Rect(r.x, r.y, target.width * num, target.height * num);
            PreviewGUI.BeginScrollView(r, base.m_Pos, viewRect, "PreHorizontalScrollbar", "PreHorizontalScrollbarThumb");
            EditorGUI.DrawPreviewTexture(viewRect, target, null, ScaleMode.StretchToFill);
            base.m_Pos = PreviewGUI.EndScrollView();
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
            EditorGUI.BeginDisabledGroup(Application.isPlaying || (base.targets.Length > 1));
            MovieTexture target = this.target as MovieTexture;
            AudioClip audioClip = target.audioClip;
            bool flag = PreviewGUI.CycleButton(!target.isPlaying ? 0 : 1, s_PlayIcons) != 0;
            if (flag != target.isPlaying)
            {
                if (flag)
                {
                    target.Stop();
                    target.Play();
                    if (audioClip != null)
                    {
                        AudioUtil.PlayClip(audioClip);
                    }
                }
                else
                {
                    target.Pause();
                    if (audioClip != null)
                    {
                        AudioUtil.PauseClip(audioClip);
                    }
                }
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}

