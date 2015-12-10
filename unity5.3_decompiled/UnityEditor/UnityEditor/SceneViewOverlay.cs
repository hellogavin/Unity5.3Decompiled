namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class SceneViewOverlay
    {
        private float k_WindowPadding = 9f;
        private SceneView m_SceneView;
        private Rect m_WindowRect = new Rect(0f, 0f, 0f, 0f);
        private static List<OverlayWindow> m_Windows;

        public SceneViewOverlay(SceneView sceneView)
        {
            this.m_SceneView = sceneView;
            m_Windows = new List<OverlayWindow>();
        }

        public void Begin()
        {
            if (this.m_SceneView.m_ShowSceneViewWindows)
            {
                if (Event.current.type == EventType.Layout)
                {
                    m_Windows.Clear();
                }
                this.m_SceneView.BeginWindows();
            }
        }

        private void EatMouseInput(Rect position)
        {
            SceneView.AddCursorRect(position, MouseCursor.Arrow);
            int controlID = GUIUtility.GetControlID("SceneViewOverlay".GetHashCode(), FocusType.Native, position);
            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (position.Contains(Event.current.mousePosition))
                    {
                        GUIUtility.hotControl = controlID;
                        Event.current.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        Event.current.Use();
                    }
                    break;

                case EventType.ScrollWheel:
                    if (position.Contains(Event.current.mousePosition))
                    {
                        Event.current.Use();
                    }
                    break;
            }
        }

        public void End()
        {
            if (this.m_SceneView.m_ShowSceneViewWindows)
            {
                m_Windows.Sort();
                if (m_Windows.Count > 0)
                {
                    this.m_WindowRect.x = 0f;
                    this.m_WindowRect.y = 0f;
                    this.m_WindowRect.width = this.m_SceneView.position.width;
                    this.m_WindowRect.height = this.m_SceneView.position.height;
                    this.m_WindowRect = GUILayout.Window("SceneViewOverlay".GetHashCode(), this.m_WindowRect, new GUI.WindowFunction(this.WindowTrampoline), string.Empty, "SceneViewOverlayTransparentBackground", new GUILayoutOption[0]);
                }
                this.m_SceneView.EndWindows();
            }
        }

        public static void Window(GUIContent title, WindowFunction sceneViewFunc, int order, WindowDisplayOption option)
        {
            Window(title, sceneViewFunc, order, null, option);
        }

        public static void Window(GUIContent title, WindowFunction sceneViewFunc, int order, Object target, WindowDisplayOption option)
        {
            if (Event.current.type == EventType.Layout)
            {
                foreach (OverlayWindow window in m_Windows)
                {
                    if ((((option == WindowDisplayOption.OneWindowPerTarget) && (window.m_Target == target)) && (target != null)) || ((option == WindowDisplayOption.OneWindowPerTitle) && ((window.m_Title == title) || (window.m_Title.text == title.text))))
                    {
                        return;
                    }
                }
                OverlayWindow item = new OverlayWindow {
                    m_Title = title,
                    m_SceneViewFunc = sceneViewFunc,
                    m_PrimaryOrder = order,
                    m_SecondaryOrder = m_Windows.Count,
                    m_Target = target
                };
                m_Windows.Add(item);
            }
        }

        private void WindowTrampoline(int id)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            float num = -this.k_WindowPadding;
            foreach (OverlayWindow window in m_Windows)
            {
                GUILayout.Space(this.k_WindowPadding + num);
                num = 0f;
                EditorGUIUtility.ResetGUIState();
                GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
                EditorStyles.UpdateSkinCache(1);
                GUILayout.BeginVertical(window.m_Title, GUI.skin.window, new GUILayoutOption[0]);
                window.m_SceneViewFunc(window.m_Target, this.m_SceneView);
                GUILayout.EndVertical();
            }
            EditorStyles.UpdateSkinCache();
            GUILayout.EndVertical();
            Rect lastRect = GUILayoutUtility.GetLastRect();
            this.EatMouseInput(lastRect);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        public enum Ordering
        {
            Camera = -100,
            Cloth = 0,
            Lightmapping = 200,
            NavMesh = 300,
            OcclusionCulling = 100,
            ParticleEffect = 400
        }

        private class OverlayWindow : IComparable<SceneViewOverlay.OverlayWindow>
        {
            public int m_PrimaryOrder;
            public SceneViewOverlay.WindowFunction m_SceneViewFunc;
            public int m_SecondaryOrder;
            public Object m_Target;
            public GUIContent m_Title;

            public int CompareTo(SceneViewOverlay.OverlayWindow other)
            {
                int num = other.m_PrimaryOrder.CompareTo(this.m_PrimaryOrder);
                if (num == 0)
                {
                    num = other.m_SecondaryOrder.CompareTo(this.m_SecondaryOrder);
                }
                return num;
            }
        }

        public enum WindowDisplayOption
        {
            MultipleWindowsPerTarget,
            OneWindowPerTarget,
            OneWindowPerTitle
        }

        public delegate void WindowFunction(Object target, SceneView sceneView);
    }
}

