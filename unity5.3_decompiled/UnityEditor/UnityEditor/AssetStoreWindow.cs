namespace UnityEditor
{
    using System;
    using System.Text;
    using UnityEditor.Web;
    using UnityEngine;

    [EditorWindowTitle(title="Asset Store", icon="Asset Store")]
    internal class AssetStoreWindow : EditorWindow, IHasCustomMenu
    {
        private int m_CurrentSkin;
        private bool m_IsDocked;
        private bool m_IsOffline;
        private int m_RepeatedShow;
        private bool m_SyncingFocus;
        internal WebScriptObject scriptObject;
        internal WebView webView;

        public virtual void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Reload"), false, new GenericMenu.MenuFunction(this.Reload));
        }

        private void CreateScriptObject()
        {
            if (this.scriptObject == null)
            {
                this.scriptObject = ScriptableObject.CreateInstance<WebScriptObject>();
                this.scriptObject.hideFlags = HideFlags.HideAndDontSave;
                this.scriptObject.webView = this.webView;
            }
        }

        public static AssetStoreWindow Init()
        {
            Type[] desiredDockNextTo = new Type[] { typeof(SceneView) };
            AssetStoreWindow window = EditorWindow.GetWindow<AssetStoreWindow>(desiredDockNextTo);
            window.SetMinMaxSizes();
            window.Show();
            return window;
        }

        private void InitWebView(Rect webViewRect)
        {
            this.m_CurrentSkin = EditorGUIUtility.skinIndex;
            this.m_IsDocked = base.docked;
            this.m_IsOffline = false;
            if (this.webView == null)
            {
                int x = (int) webViewRect.x;
                int y = (int) webViewRect.y;
                int width = (int) webViewRect.width;
                int height = (int) webViewRect.height;
                this.webView = ScriptableObject.CreateInstance<WebView>();
                this.webView.InitWebView(base.m_Parent, x, y, width, height, false);
                this.webView.hideFlags = HideFlags.HideAndDontSave;
                this.webView.AllowRightClickMenu(true);
                if (base.hasFocus)
                {
                    this.SetFocus(true);
                }
            }
            this.webView.SetDelegateObject(this);
            this.webView.LoadFile(AssetStoreUtils.GetLoaderPath());
        }

        private void InvokeJSMethod(string objectName, string name, params object[] args)
        {
            if (this.webView != null)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(objectName);
                builder.Append('.');
                builder.Append(name);
                builder.Append('(');
                bool flag = true;
                foreach (object obj2 in args)
                {
                    if (!flag)
                    {
                        builder.Append(',');
                    }
                    bool flag2 = obj2 is string;
                    if (flag2)
                    {
                        builder.Append('"');
                    }
                    builder.Append(obj2);
                    if (flag2)
                    {
                        builder.Append('"');
                    }
                    flag = false;
                }
                builder.Append(");");
                this.webView.ExecuteJavascript(builder.ToString());
            }
        }

        public void Logout()
        {
            this.InvokeJSMethod("document.AssetStore.login", "logout", new object[0]);
        }

        public void OnBecameInvisible()
        {
            if (this.webView != null)
            {
                this.webView.SetHostView(null);
            }
        }

        public void OnDestroy()
        {
            Object.DestroyImmediate(this.webView);
        }

        public void OnDisable()
        {
            AssetStoreUtils.UnRegisterDownloadDelegate(this);
        }

        public void OnDownloadProgress(string id, string message, ulong bytes, ulong total)
        {
            object[] args = new object[] { id, message, bytes, total };
            this.InvokeJSMethod("document.AssetStore.pkgs", "OnDownloadProgress", args);
        }

        public void OnEnable()
        {
            this.SetMinMaxSizes();
            base.titleContent = base.GetLocalizedTitleContent();
            AssetStoreUtils.RegisterDownloadDelegate(this);
        }

        public void OnFocus()
        {
            this.SetFocus(true);
        }

        public void OnGUI()
        {
            Rect webViewRect = GUIClip.Unclip(new Rect(0f, 0f, base.position.width, base.position.height));
            if (this.webView == null)
            {
                this.InitWebView(webViewRect);
            }
            if (this.m_RepeatedShow-- > 0)
            {
                this.Refresh();
            }
            if (Event.current.type == EventType.Layout)
            {
                this.webView.SetSizeAndPosition((int) webViewRect.x, (int) webViewRect.y, (int) webViewRect.width, (int) webViewRect.height);
                if (this.m_CurrentSkin != EditorGUIUtility.skinIndex)
                {
                    this.m_CurrentSkin = EditorGUIUtility.skinIndex;
                    this.InvokeJSMethod("document.AssetStore", "refreshSkinIndex", new object[0]);
                }
                this.UpdateDockStatusIfNeeded();
            }
        }

        public void OnInitScripting()
        {
            this.SetScriptObject();
        }

        public void OnLoadError(string url)
        {
            if (this.webView != null)
            {
                if (this.m_IsOffline)
                {
                    object[] args = new object[] { url };
                    Debug.LogErrorFormat("Unexpected error: Failed to load offline Asset Store page (url={0})", args);
                }
                else
                {
                    this.m_IsOffline = true;
                    this.webView.LoadFile(AssetStoreUtils.GetOfflinePath());
                }
            }
        }

        public void OnLostFocus()
        {
            this.SetFocus(false);
        }

        public void OnOpenExternalLink(string url)
        {
            if (url.StartsWith("http://") || url.StartsWith("https://"))
            {
                Application.OpenURL(url);
            }
        }

        public static void OpenURL(string url)
        {
            object[] args = new object[] { url };
            Init().InvokeJSMethod("document.AssetStore", "openURL", args);
            AssetStoreContext.GetInstance().initialOpenURL = url;
        }

        public void Refresh()
        {
            this.webView.Hide();
            this.webView.Show();
        }

        public void Reload()
        {
            this.m_CurrentSkin = EditorGUIUtility.skinIndex;
            this.m_IsDocked = base.docked;
            this.webView.Reload();
        }

        private void SetFocus(bool value)
        {
            if (!this.m_SyncingFocus)
            {
                this.m_SyncingFocus = true;
                if (this.webView != null)
                {
                    if (value)
                    {
                        this.webView.SetHostView(base.m_Parent);
                        this.webView.Show();
                        this.m_RepeatedShow = 5;
                    }
                    this.webView.SetFocus(value);
                }
                this.m_SyncingFocus = false;
            }
        }

        private void SetMinMaxSizes()
        {
            base.minSize = new Vector2(400f, 100f);
            base.maxSize = new Vector2(2048f, 2048f);
        }

        private void SetScriptObject()
        {
            if (this.webView != null)
            {
                this.CreateScriptObject();
                this.webView.DefineScriptObject("window.unityScriptObject", this.scriptObject);
            }
        }

        public void UpdateDockStatusIfNeeded()
        {
            if (this.m_IsDocked != base.docked)
            {
                this.m_IsDocked = base.docked;
                if (this.scriptObject != null)
                {
                    AssetStoreContext.GetInstance().docked = base.docked;
                    this.InvokeJSMethod("document.AssetStore", "updateDockStatus", new object[0]);
                }
            }
        }
    }
}

