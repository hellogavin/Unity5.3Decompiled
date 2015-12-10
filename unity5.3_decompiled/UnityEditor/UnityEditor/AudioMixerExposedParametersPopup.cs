namespace UnityEditor
{
    using System;
    using UnityEditor.Audio;
    using UnityEngine;

    internal class AudioMixerExposedParametersPopup : PopupWindowContent
    {
        private static GUIContent m_ButtonContent = new GUIContent(string.Empty, "Audio Mixer parameters can be exposed to scripting. Select an Audio Mixer Group, right click one of its properties in the Inspector and select 'Expose ..'.");
        private readonly AudioMixerExposedParameterView m_ExposedParametersView = new AudioMixerExposedParameterView(new ReorderableListWithRenameAndScrollView.State());
        private static int m_LastNumExposedParams = -1;

        private AudioMixerExposedParametersPopup(AudioMixerController controller)
        {
            this.m_ExposedParametersView.OnMixerControllerChanged(controller);
        }

        private static GUIContent GetButtonContent(AudioMixerController controller)
        {
            if (controller.numExposedParameters != m_LastNumExposedParams)
            {
                m_ButtonContent.text = string.Format("Exposed Parameters ({0})", controller.numExposedParameters);
                m_LastNumExposedParams = controller.numExposedParameters;
            }
            return m_ButtonContent;
        }

        public override Vector2 GetWindowSize()
        {
            Vector2 vector = this.m_ExposedParametersView.CalcSize();
            vector.x = Math.Max(vector.x, 125f);
            vector.y = Math.Max(vector.y, 23f);
            return vector;
        }

        public override void OnGUI(Rect rect)
        {
            this.m_ExposedParametersView.OnEvent();
            this.m_ExposedParametersView.OnGUI(rect);
        }

        internal static void Popup(AudioMixerController controller, GUIStyle style, params GUILayoutOption[] options)
        {
            GUIContent buttonContent = GetButtonContent(controller);
            Rect position = GUILayoutUtility.GetRect(buttonContent, style, options);
            if (EditorGUI.ButtonMouseDown(position, buttonContent, FocusType.Passive, style))
            {
                PopupWindow.Show(position, new AudioMixerExposedParametersPopup(controller));
            }
        }
    }
}

