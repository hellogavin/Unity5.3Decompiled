namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class AudioFilterGUI
    {
        private EditorGUI.VUMeter.SmoothingData[] dataOut;

        public void DrawAudioFilterGUI(MonoBehaviour behaviour)
        {
            int customFilterChannelCount = AudioUtil.GetCustomFilterChannelCount(behaviour);
            if (customFilterChannelCount > 0)
            {
                if (this.dataOut == null)
                {
                    this.dataOut = new EditorGUI.VUMeter.SmoothingData[customFilterChannelCount];
                }
                double num2 = ((double) AudioUtil.GetCustomFilterProcessTime(behaviour)) / 1000000.0;
                float r = ((float) num2) / ((((float) AudioSettings.outputSampleRate) / 1024f) / ((float) customFilterChannelCount));
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Space(13f);
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                EditorGUILayout.Space();
                for (int i = 0; i < customFilterChannelCount; i++)
                {
                    GUILayoutOption[] optionArray1 = new GUILayoutOption[] { GUILayout.MinWidth(50f), GUILayout.Height(5f) };
                    EditorGUILayout.VUMeterHorizontal(AudioUtil.GetCustomFilterMaxOut(behaviour, i), ref this.dataOut[i], optionArray1);
                }
                GUILayout.EndVertical();
                Color color = GUI.color;
                GUI.color = new Color(r, 1f - r, 0f, 1f);
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(40f), GUILayout.Height(20f) };
                GUILayout.Box(string.Format("{0:00.00}ms", num2), options);
                GUI.color = color;
                GUILayout.EndHorizontal();
                EditorGUILayout.Space();
                GUIView.current.Repaint();
            }
        }
    }
}

