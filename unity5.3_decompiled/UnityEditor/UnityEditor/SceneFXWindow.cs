namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class SceneFXWindow : PopupWindowContent
    {
        private const float kFrameWidth = 1f;
        private readonly SceneView m_SceneView;
        private static SceneFXWindow s_SceneFXWindow;
        private static Styles s_Styles;

        public SceneFXWindow(SceneView sceneView)
        {
            this.m_SceneView = sceneView;
        }

        private void Draw(Rect rect)
        {
            <Draw>c__AnonStorey49 storey = new <Draw>c__AnonStorey49();
            if ((this.m_SceneView != null) && (this.m_SceneView.m_SceneViewState != null))
            {
                Rect rect2 = new Rect(1f, 1f, rect.width - 2f, 16f);
                storey.state = this.m_SceneView.m_SceneViewState;
                this.DrawListElement(rect2, "Skybox", storey.state.showSkybox, new Action<bool>(storey.<>m__84));
                rect2.y += 16f;
                this.DrawListElement(rect2, "Fog", storey.state.showFog, new Action<bool>(storey.<>m__85));
                rect2.y += 16f;
                this.DrawListElement(rect2, "Flares", storey.state.showFlares, new Action<bool>(storey.<>m__86));
                rect2.y += 16f;
                this.DrawListElement(rect2, "Animated Materials", storey.state.showMaterialUpdate, new Action<bool>(storey.<>m__87));
                rect2.y += 16f;
            }
        }

        private void DrawListElement(Rect rect, string toggleName, bool value, Action<bool> setValue)
        {
            EditorGUI.BeginChangeCheck();
            bool flag = GUI.Toggle(rect, value, EditorGUIUtility.TempContent(toggleName), s_Styles.menuItem);
            if (EditorGUI.EndChangeCheck())
            {
                setValue(flag);
                this.m_SceneView.Repaint();
            }
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(160f, 66f);
        }

        public override void OnGUI(Rect rect)
        {
            if (((this.m_SceneView != null) && (this.m_SceneView.m_SceneViewState != null)) && (Event.current.type != EventType.Layout))
            {
                if (s_Styles == null)
                {
                    s_Styles = new Styles();
                }
                this.Draw(rect);
                if (Event.current.type == EventType.MouseMove)
                {
                    Event.current.Use();
                }
                if ((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Escape))
                {
                    base.editorWindow.Close();
                    GUIUtility.ExitGUI();
                }
            }
        }

        [CompilerGenerated]
        private sealed class <Draw>c__AnonStorey49
        {
            internal SceneView.SceneViewState state;

            internal void <>m__84(bool value)
            {
                this.state.showSkybox = value;
            }

            internal void <>m__85(bool value)
            {
                this.state.showFog = value;
            }

            internal void <>m__86(bool value)
            {
                this.state.showFlares = value;
            }

            internal void <>m__87(bool value)
            {
                this.state.showMaterialUpdate = value;
            }
        }

        private class Styles
        {
            public readonly GUIStyle menuItem = "MenuItem";
        }
    }
}

