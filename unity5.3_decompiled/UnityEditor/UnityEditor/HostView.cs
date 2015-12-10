namespace UnityEditor
{
    using System;
    using System.Reflection;
    using UnityEngine;

    internal class HostView : GUIView
    {
        internal GUIStyle background;
        internal static PrefColor kPlayModeDarken = new PrefColor("Playmode tint", 0.8f, 0.8f, 0.8f, 1f);
        internal static Color kViewColor = new Color(0.76f, 0.76f, 0.76f, 1f);
        [SerializeField]
        protected EditorWindow m_ActualView;
        [NonSerialized]
        private Rect m_BackgroundClearRect = new Rect(0f, 0f, 0f, 0f);
        [NonSerialized]
        protected readonly RectOffset m_BorderSize = new RectOffset();

        protected virtual void AddDefaultItemsToMenu(GenericMenu menu, EditorWindow view)
        {
        }

        protected void ClearBackground()
        {
            if (Event.current.type == EventType.Repaint)
            {
                EditorWindow actualView = this.actualView;
                if (((actualView == null) || !actualView.dontClearBackground) || (!base.backgroundValid || (base.position != this.m_BackgroundClearRect)))
                {
                    Color color = !EditorGUIUtility.isProSkin ? kViewColor : EditorGUIUtility.kDarkViewBackground;
                    GL.Clear(true, true, !EditorApplication.isPlayingOrWillChangePlaymode ? color : (color * kPlayModeDarken));
                    base.backgroundValid = true;
                    this.m_BackgroundClearRect = base.position;
                }
            }
        }

        protected void DeregisterSelectedPane(bool clearActualView)
        {
            if (this.m_ActualView != null)
            {
                if (this.GetPaneMethod("Update") != null)
                {
                    EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.SendUpdate));
                }
                if (this.GetPaneMethod("ModifierKeysChanged") != null)
                {
                    EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.SendModKeysChanged));
                }
                if (this.m_ActualView.m_FadeoutTime != 0f)
                {
                    EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.m_ActualView.CheckForWindowRepaint));
                }
                if (clearActualView)
                {
                    EditorWindow actualView = this.m_ActualView;
                    this.m_ActualView = null;
                    this.Invoke("OnLostFocus", actualView);
                    this.Invoke("OnBecameInvisible", actualView);
                }
            }
        }

        protected virtual RectOffset GetBorderSize()
        {
            return this.m_BorderSize;
        }

        private MethodInfo GetPaneMethod(string methodName)
        {
            return this.GetPaneMethod(methodName, this.m_ActualView);
        }

        private MethodInfo GetPaneMethod(string methodName, object obj)
        {
            if (obj != null)
            {
                Type baseType = obj.GetType();
                MethodInfo method = null;
                while (baseType != null)
                {
                    method = baseType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (method != null)
                    {
                        return method;
                    }
                    baseType = baseType.BaseType;
                }
            }
            return null;
        }

        protected Type[] GetPaneTypes()
        {
            return new Type[] { typeof(SceneView), typeof(GameView), typeof(InspectorWindow), typeof(SceneHierarchyWindow), typeof(ProjectBrowser), typeof(ProfilerWindow), typeof(AnimationWindow) };
        }

        protected void Invoke(string methodName)
        {
            this.Invoke(methodName, this.m_ActualView);
        }

        protected void Invoke(string methodName, object obj)
        {
            MethodInfo paneMethod = this.GetPaneMethod(methodName, obj);
            if (paneMethod != null)
            {
                paneMethod.Invoke(obj, null);
            }
        }

        public void OnDestroy()
        {
            if (this.m_ActualView != null)
            {
                Object.DestroyImmediate(this.m_ActualView, true);
            }
            base.OnDestroy();
        }

        internal void OnDidOpenScene()
        {
            this.Invoke("OnDidOpenScene");
        }

        private void OnDisable()
        {
            this.DeregisterSelectedPane(false);
        }

        public void OnEnable()
        {
            this.background = null;
            this.RegisterSelectedPane();
        }

        protected override bool OnFocus()
        {
            this.Invoke("OnFocus");
            if (this == null)
            {
                return false;
            }
            base.Repaint();
            return true;
        }

        private void OnGUI()
        {
            EditorGUIUtility.ResetGUIState();
            base.DoWindowDecorationStart();
            if (this.background == null)
            {
                this.background = "hostview";
                this.background.padding.top = 0;
            }
            GUILayout.BeginVertical(this.background, new GUILayoutOption[0]);
            if (this.actualView != null)
            {
                this.actualView.m_Pos = base.screenPosition;
            }
            this.Invoke("OnGUI");
            EditorGUIUtility.ResetGUIState();
            if ((this.m_ActualView.m_FadeoutTime != 0f) && (Event.current.type == EventType.Repaint))
            {
                this.m_ActualView.DrawNotification();
            }
            GUILayout.EndVertical();
            base.DoWindowDecorationEnd();
            EditorGUI.ShowRepaints();
        }

        internal void OnHierarchyChange()
        {
            this.Invoke("OnHierarchyChange");
        }

        internal void OnInspectorUpdate()
        {
            this.Invoke("OnInspectorUpdate");
        }

        private void OnLostFocus()
        {
            this.Invoke("OnLostFocus");
            base.Repaint();
        }

        internal void OnProjectChange()
        {
            this.Invoke("OnProjectChange");
        }

        internal void OnSelectionChange()
        {
            this.Invoke("OnSelectionChange");
        }

        public void PopupGenericMenu(EditorWindow view, Rect pos)
        {
            GenericMenu menu = new GenericMenu();
            IHasCustomMenu menu2 = view as IHasCustomMenu;
            if (menu2 != null)
            {
                menu2.AddItemsToMenu(menu);
            }
            this.AddDefaultItemsToMenu(menu, view);
            menu.DropDown(pos);
            Event.current.Use();
        }

        protected void RegisterSelectedPane()
        {
            if (this.m_ActualView != null)
            {
                this.m_ActualView.m_Parent = this;
                if (this.GetPaneMethod("Update") != null)
                {
                    EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.SendUpdate));
                }
                if (this.GetPaneMethod("ModifierKeysChanged") != null)
                {
                    EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.SendModKeysChanged));
                }
                this.m_ActualView.MakeParentsSettingsMatchMe();
                if (this.m_ActualView.m_FadeoutTime != 0f)
                {
                    EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.m_ActualView.CheckForWindowRepaint));
                }
                try
                {
                    this.Invoke("OnBecameVisible");
                    this.Invoke("OnFocus");
                }
                catch (TargetInvocationException exception)
                {
                    Debug.LogError(exception.InnerException.GetType().Name + ":" + exception.InnerException.Message);
                }
            }
        }

        private void SendModKeysChanged()
        {
            this.Invoke("ModifierKeysChanged");
        }

        private void SendUpdate()
        {
            this.Invoke("Update");
        }

        protected override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            if (this.m_ActualView != null)
            {
                this.m_ActualView.m_Pos = newPos;
                this.m_ActualView.OnResized();
            }
        }

        protected void ShowGenericMenu()
        {
            GUIStyle style = "PaneOptions";
            Rect position = new Rect((base.position.width - style.fixedWidth) - 4f, Mathf.Floor((this.background.margin.top + 20) - style.fixedHeight), style.fixedWidth, style.fixedHeight);
            if (EditorGUI.ButtonMouseDown(position, GUIContent.none, FocusType.Passive, "PaneOptions"))
            {
                this.PopupGenericMenu(this.m_ActualView, position);
            }
            MethodInfo paneMethod = this.GetPaneMethod("ShowButton", this.m_ActualView);
            if (paneMethod != null)
            {
                object[] parameters = new object[] { new Rect((base.position.width - style.fixedWidth) - 20f, Mathf.Floor((float) (this.background.margin.top + 4)), 16f, 16f) };
                paneMethod.Invoke(this.m_ActualView, parameters);
            }
        }

        internal EditorWindow actualView
        {
            get
            {
                return this.m_ActualView;
            }
            set
            {
                if (this.m_ActualView != value)
                {
                    this.DeregisterSelectedPane(true);
                    this.m_ActualView = value;
                    this.RegisterSelectedPane();
                }
            }
        }

        internal RectOffset borderSize
        {
            get
            {
                return this.GetBorderSize();
            }
        }
    }
}

