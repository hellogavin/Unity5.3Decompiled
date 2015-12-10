namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class WindowFocusState : ScriptableObject
    {
        internal bool m_CurrentlyInPlayMode;
        private static WindowFocusState m_Instance;
        internal string m_LastWindowTypeInSameDock = string.Empty;
        internal bool m_WasMaximizedBeforePlay;

        private void OnEnable()
        {
            base.hideFlags = HideFlags.HideAndDontSave;
            m_Instance = this;
        }

        internal static WindowFocusState instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = Object.FindObjectOfType(typeof(WindowFocusState)) as WindowFocusState;
                }
                if (m_Instance == null)
                {
                    m_Instance = ScriptableObject.CreateInstance<WindowFocusState>();
                }
                return m_Instance;
            }
        }
    }
}

