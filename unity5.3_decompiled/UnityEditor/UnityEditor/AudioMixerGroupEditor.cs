namespace UnityEditor
{
    using System;
    using UnityEditor.Audio;
    using UnityEngine;

    [CustomEditor(typeof(AudioMixerGroupController))]
    internal class AudioMixerGroupEditor : Editor
    {
        public static readonly string kPrefKeyForShowCpuUsage = "AudioMixerShowCPU";
        private AudioMixerEffectView m_EffectView;
        private readonly TickTimerHelper m_Ticker = new TickTimerHelper(0.05);

        [MenuItem("CONTEXT/AudioMixerGroupController/Copy all effect settings to all snapshots")]
        private static void CopyAllEffectToSnapshots(MenuCommand command)
        {
            AudioMixerGroupController context = command.context as AudioMixerGroupController;
            AudioMixerController controller = context.controller;
            if (controller != null)
            {
                Undo.RecordObject(controller, "Copy all effect settings to all snapshots");
                controller.CopyAllSettingsToAllSnapshots(context, controller.TargetSnapshot);
            }
        }

        internal override void DrawHeaderHelpAndSettingsGUI(Rect r)
        {
            if (this.m_EffectView != null)
            {
                AudioMixerGroupController target = this.target as AudioMixerGroupController;
                base.DrawHeaderHelpAndSettingsGUI(r);
                Rect position = new Rect(r.x + 44f, r.yMax - 20f, r.width - 50f, 15f);
                GUI.Label(position, GUIContent.Temp(target.controller.name), EditorStyles.miniLabel);
            }
        }

        private void OnDisable()
        {
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
        }

        private void OnEnable()
        {
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
        }

        public override void OnInspectorGUI()
        {
            AudioMixerDrawUtils.InitStyles();
            if (this.m_EffectView == null)
            {
                this.m_EffectView = new AudioMixerEffectView();
            }
            AudioMixerGroupController target = this.target as AudioMixerGroupController;
            this.m_EffectView.OnGUI(target);
        }

        [MenuItem("CONTEXT/AudioMixerGroupController/Toggle CPU usage display (only available on first editor instance)")]
        private static void ShowCPUUsage(MenuCommand command)
        {
            bool @bool = EditorPrefs.GetBool(kPrefKeyForShowCpuUsage, false);
            EditorPrefs.SetBool(kPrefKeyForShowCpuUsage, !@bool);
        }

        public void Update()
        {
            if (EditorApplication.isPlaying && this.m_Ticker.DoTick())
            {
                base.Repaint();
            }
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }
    }
}

