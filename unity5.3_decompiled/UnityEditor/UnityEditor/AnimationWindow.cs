namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [EditorWindowTitle(title="Animation", useTypeNameAsIconName=true)]
    internal class AnimationWindow : EditorWindow
    {
        [SerializeField]
        private AnimEditor m_AnimEditor;
        private GUIStyle m_LockButtonStyle;
        private static List<AnimationWindow> s_AnimationWindows = new List<AnimationWindow>();

        public static List<AnimationWindow> GetAllAnimationWindows()
        {
            return s_AnimationWindows;
        }

        public void OnDisable()
        {
            s_AnimationWindows.Remove(this);
            this.m_AnimEditor.OnDisable();
        }

        public void OnEnable()
        {
            if (this.m_AnimEditor == null)
            {
                this.m_AnimEditor = ScriptableObject.CreateInstance(typeof(AnimEditor)) as AnimEditor;
                this.m_AnimEditor.hideFlags = HideFlags.HideAndDontSave;
            }
            s_AnimationWindows.Add(this);
            base.titleContent = base.GetLocalizedTitleContent();
        }

        public void OnGUI()
        {
            this.m_AnimEditor.OnBreadcrumbGUI(this, base.position);
        }

        public void OnSelectionChange()
        {
            this.m_AnimEditor.OnSelectionChange();
        }

        protected virtual void ShowButton(Rect r)
        {
            if (this.m_LockButtonStyle == null)
            {
                this.m_LockButtonStyle = "IN LockButton";
            }
            EditorGUI.BeginDisabledGroup(this.m_AnimEditor.stateDisabled);
            this.m_AnimEditor.locked = GUI.Toggle(r, this.m_AnimEditor.locked, GUIContent.none, this.m_LockButtonStyle);
            EditorGUI.EndDisabledGroup();
        }

        public void Update()
        {
            this.m_AnimEditor.Update();
        }
    }
}

