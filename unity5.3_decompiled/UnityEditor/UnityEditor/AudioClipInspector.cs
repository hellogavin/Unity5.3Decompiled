namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(AudioClip))]
    internal class AudioClipInspector : Editor
    {
        private static bool m_bAutoPlay;
        private static bool m_bLoop = false;
        private static bool m_bPlayFirst;
        private GUIView m_GUI;
        private AudioClip m_PlayingClip;
        private Vector2 m_Position = Vector2.zero;
        private static Rect m_wantedRect;
        private static GUIContent[] s_AutoPlayIcons = new GUIContent[2];
        private static Texture2D s_DefaultIcon;
        private static GUIContent[] s_LoopIcons = new GUIContent[2];
        private static GUIContent[] s_PlayIcons = new GUIContent[2];
        private static GUIStyle s_PreButton;

        private static Texture2D CombineWaveForms(Texture2D[] waveForms)
        {
            if (waveForms.Length == 1)
            {
                return waveForms[0];
            }
            int width = waveForms[0].width;
            int height = 0;
            foreach (Texture2D textured in waveForms)
            {
                height += textured.height;
            }
            Texture2D textured2 = new Texture2D(width, height, TextureFormat.ARGB32, false);
            int num4 = 0;
            foreach (Texture2D textured3 in waveForms)
            {
                num4 += textured3.height;
                textured2.SetPixels(0, height - num4, width, textured3.height, textured3.GetPixels());
                Object.DestroyImmediate(textured3);
            }
            textured2.Apply();
            return textured2;
        }

        public override string GetInfoString()
        {
            AudioClip target = this.target as AudioClip;
            int channelCount = AudioUtil.GetChannelCount(target);
            string str = (channelCount != 1) ? ((channelCount != 2) ? (((channelCount - 1)).ToString() + ".1") : "Stereo") : "Mono";
            AudioCompressionFormat targetPlatformSoundCompressionFormat = AudioUtil.GetTargetPlatformSoundCompressionFormat(target);
            AudioCompressionFormat soundCompressionFormat = AudioUtil.GetSoundCompressionFormat(target);
            string str2 = targetPlatformSoundCompressionFormat.ToString();
            if (targetPlatformSoundCompressionFormat != soundCompressionFormat)
            {
                str2 = str2 + " (" + soundCompressionFormat.ToString() + " in editor)";
            }
            string str3 = str2;
            object[] objArray1 = new object[] { str3, ", ", AudioUtil.GetFrequency(target), " Hz, ", str, ", " };
            str2 = string.Concat(objArray1);
            TimeSpan span = new TimeSpan(0, 0, 0, 0, (int) AudioUtil.GetDuration(target));
            if (((uint) AudioUtil.GetDuration(target)) == uint.MaxValue)
            {
                return (str2 + "Unlimited");
            }
            return (str2 + string.Format("{0:00}:{1:00}.{2:000}", span.Minutes, span.Seconds, span.Milliseconds));
        }

        public override bool HasPreviewGUI()
        {
            return (base.targets != null);
        }

        private static void Init()
        {
            if (s_PreButton == null)
            {
                s_PreButton = "preButton";
                m_bAutoPlay = EditorPrefs.GetBool("AutoPlayAudio", false);
                m_bLoop = false;
                s_AutoPlayIcons[0] = EditorGUIUtility.IconContent("preAudioAutoPlayOff", "Turn Auto Play on");
                s_AutoPlayIcons[1] = EditorGUIUtility.IconContent("preAudioAutoPlayOn", "Turn Auto Play off");
                s_PlayIcons[0] = EditorGUIUtility.IconContent("preAudioPlayOff", "Play");
                s_PlayIcons[1] = EditorGUIUtility.IconContent("preAudioPlayOn", "Stop");
                s_LoopIcons[0] = EditorGUIUtility.IconContent("preAudioLoopOff", "Loop on");
                s_LoopIcons[1] = EditorGUIUtility.IconContent("preAudioLoopOn", "Loop off");
                s_DefaultIcon = EditorGUIUtility.LoadIcon("Profiler.Audio");
            }
        }

        public void OnDisable()
        {
            AudioUtil.StopAllClips();
            AudioUtil.ClearWaveForm(this.target as AudioClip);
            EditorPrefs.SetBool("AutoPlayAudio", m_bAutoPlay);
        }

        public void OnEnable()
        {
            m_bAutoPlay = EditorPrefs.GetBool("AutoPlayAudio", false);
            if (m_bAutoPlay)
            {
                m_bPlayFirst = true;
            }
        }

        public override void OnInspectorGUI()
        {
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (s_DefaultIcon == null)
            {
                Init();
            }
            AudioClip target = this.target as AudioClip;
            Event current = Event.current;
            if (((current.type == EventType.Repaint) || (current.type == EventType.Layout)) || (current.type == EventType.Used))
            {
                if (Event.current.type == EventType.Repaint)
                {
                    background.Draw(r, false, false, false, false);
                }
                int channelCount = AudioUtil.GetChannelCount(target);
                m_wantedRect = new Rect(r.x, r.y, r.width, r.height);
                float num3 = m_wantedRect.width / target.length;
                if (!AudioUtil.HasPreview(target) && (AudioUtil.IsTrackerFile(target) || AudioUtil.IsMovieAudio(target)))
                {
                    float y = (r.height <= 150f) ? ((r.y + (r.height / 2f)) - 25f) : ((r.y + (r.height / 2f)) - 10f);
                    if (r.width > 64f)
                    {
                        if (AudioUtil.IsTrackerFile(target))
                        {
                            EditorGUI.DropShadowLabel(new Rect(r.x, y, r.width, 20f), string.Format("Module file with " + AudioUtil.GetMusicChannelCount(target) + " channels.", new object[0]));
                        }
                        else if (AudioUtil.IsMovieAudio(target))
                        {
                            if (r.width > 450f)
                            {
                                EditorGUI.DropShadowLabel(new Rect(r.x, y, r.width, 20f), "Audio is attached to a movie. To audition the sound, play the movie.");
                            }
                            else
                            {
                                EditorGUI.DropShadowLabel(new Rect(r.x, y, r.width, 20f), "Audio is attached to a movie.");
                                EditorGUI.DropShadowLabel(new Rect(r.x, y + 10f, r.width, 20f), "To audition the sound, play the movie.");
                            }
                        }
                        else
                        {
                            EditorGUI.DropShadowLabel(new Rect(r.x, y, r.width, 20f), "Can not show PCM data for this file");
                        }
                    }
                    if (this.m_PlayingClip == target)
                    {
                        float clipPosition = AudioUtil.GetClipPosition(target);
                        TimeSpan span = new TimeSpan(0, 0, 0, 0, (int) (clipPosition * 1000f));
                        EditorGUI.DropShadowLabel(new Rect(m_wantedRect.x, m_wantedRect.y, m_wantedRect.width, 20f), string.Format("Playing - {0:00}:{1:00}.{2:000}", span.Minutes, span.Seconds, span.Milliseconds));
                    }
                }
                else
                {
                    PreviewGUI.BeginScrollView(m_wantedRect, this.m_Position, m_wantedRect, "PreHorizontalScrollbar", "PreHorizontalScrollbarThumb");
                    Texture2D image = null;
                    if (r.width < 100f)
                    {
                        image = AssetPreview.GetAssetPreview(target);
                    }
                    else
                    {
                        image = AudioUtil.GetWaveFormFast(target, 1, 0, target.samples, r.width, r.height);
                    }
                    if (image == null)
                    {
                        Rect position = new Rect {
                            x = ((m_wantedRect.width - s_DefaultIcon.width) / 2f) + m_wantedRect.x,
                            y = ((m_wantedRect.height - s_DefaultIcon.height) / 2f) + m_wantedRect.y,
                            width = s_DefaultIcon.width,
                            height = s_DefaultIcon.height
                        };
                        GUI.DrawTexture(position, s_DefaultIcon);
                        base.Repaint();
                    }
                    else
                    {
                        GUI.DrawTexture(new Rect(m_wantedRect.x, m_wantedRect.y, m_wantedRect.width, m_wantedRect.height), image);
                    }
                    for (int i = 0; i < channelCount; i++)
                    {
                        if ((channelCount > 1) && (r.width > 64f))
                        {
                            Rect rect2 = new Rect(m_wantedRect.x + 5f, m_wantedRect.y + ((m_wantedRect.height / ((float) channelCount)) * i), 30f, 20f);
                            EditorGUI.DropShadowLabel(rect2, "ch " + ((i + 1)).ToString());
                        }
                    }
                    if (this.m_PlayingClip == target)
                    {
                        float num7 = AudioUtil.GetClipPosition(target);
                        TimeSpan span2 = new TimeSpan(0, 0, 0, 0, (int) (num7 * 1000f));
                        GUI.DrawTexture(new Rect(m_wantedRect.x + ((int) (num3 * num7)), m_wantedRect.y, 2f, m_wantedRect.height), EditorGUIUtility.whiteTexture);
                        if (r.width > 64f)
                        {
                            EditorGUI.DropShadowLabel(new Rect(m_wantedRect.x, m_wantedRect.y, m_wantedRect.width, 20f), string.Format("{0:00}:{1:00}.{2:000}", span2.Minutes, span2.Seconds, span2.Milliseconds));
                        }
                        else
                        {
                            EditorGUI.DropShadowLabel(new Rect(m_wantedRect.x, m_wantedRect.y, m_wantedRect.width, 20f), string.Format("{0:00}:{1:00}", span2.Minutes, span2.Seconds));
                        }
                        if (!AudioUtil.IsClipPlaying(target))
                        {
                            this.m_PlayingClip = null;
                        }
                    }
                    PreviewGUI.EndScrollView();
                }
                if (m_bPlayFirst)
                {
                    AudioUtil.PlayClip(target, 0, m_bLoop);
                    this.m_PlayingClip = target;
                    m_bPlayFirst = false;
                }
                if (this.playing)
                {
                    GUIView.current.Repaint();
                }
            }
            else
            {
                int num = AudioUtil.GetSampleCount(target) / ((int) r.width);
                switch (current.type)
                {
                    case EventType.MouseDown:
                    case EventType.MouseDrag:
                        if (r.Contains(current.mousePosition) && !AudioUtil.IsMovieAudio(target))
                        {
                            if (this.m_PlayingClip != target)
                            {
                                AudioUtil.StopAllClips();
                                AudioUtil.PlayClip(target, 0, m_bLoop);
                                this.m_PlayingClip = target;
                            }
                            AudioUtil.SetClipSamplePosition(target, num * ((int) current.mousePosition.x));
                            current.Use();
                        }
                        break;
                }
            }
        }

        public override void OnPreviewSettings()
        {
            if (s_DefaultIcon == null)
            {
                Init();
            }
            AudioClip target = this.target as AudioClip;
            EditorGUI.BeginDisabledGroup(AudioUtil.IsMovieAudio(target));
            bool disabled = base.targets.Length > 1;
            EditorGUI.BeginDisabledGroup(disabled);
            m_bAutoPlay = !disabled ? m_bAutoPlay : false;
            m_bAutoPlay = PreviewGUI.CycleButton(!m_bAutoPlay ? 0 : 1, s_AutoPlayIcons) != 0;
            EditorGUI.EndDisabledGroup();
            bool bLoop = m_bLoop;
            m_bLoop = PreviewGUI.CycleButton(!m_bLoop ? 0 : 1, s_LoopIcons) != 0;
            if ((bLoop != m_bLoop) && this.playing)
            {
                AudioUtil.LoopClip(target, m_bLoop);
            }
            EditorGUI.BeginDisabledGroup(disabled && !this.playing);
            bool flag3 = PreviewGUI.CycleButton(!this.playing ? 0 : 1, s_PlayIcons) != 0;
            if (flag3 != this.playing)
            {
                if (flag3)
                {
                    AudioUtil.PlayClip(target, 0, m_bLoop);
                    this.m_PlayingClip = target;
                }
                else
                {
                    AudioUtil.StopAllClips();
                    this.m_PlayingClip = null;
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndDisabledGroup();
        }

        public override void ReloadPreviewInstances()
        {
            AudioUtil.ClearWaveForm(this.target as AudioClip);
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            AssetImporter atPath = AssetImporter.GetAtPath(assetPath);
            AudioImporter importer2 = atPath as AudioImporter;
            if (importer2 == null)
            {
                return null;
            }
            AudioClip target = this.target as AudioClip;
            Texture2D[] waveForms = new Texture2D[target.channels];
            for (int i = 0; i < target.channels; i++)
            {
                waveForms[i] = AudioUtil.GetWaveForm(target, atPath, i, (float) width, (float) (height / target.channels));
            }
            return CombineWaveForms(waveForms);
        }

        private bool playing
        {
            get
            {
                return (this.m_PlayingClip != null);
            }
        }
    }
}

