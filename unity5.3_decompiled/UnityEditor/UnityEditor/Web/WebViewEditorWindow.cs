namespace UnityEditor.Web
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Timers;
    using UnityEditor;
    using UnityEngine;

    internal class WebViewEditorWindow : EditorWindow, ISerializationCallbackReceiver, IHasCustomMenu
    {
        private const int k_RepaintTimerDelay = 30;
        protected object m_GlobalObject = null;
        [SerializeField]
        protected string m_GlobalObjectTypeName;
        [SerializeField]
        protected string m_InitialOpenURL;
        private Timer m_PostLoadTimer;
        [SerializeField]
        private List<WebView> m_RegisteredViewInstances = new List<WebView>();
        private Dictionary<string, WebView> m_RegisteredViews = new Dictionary<string, WebView>();
        [SerializeField]
        private List<string> m_RegisteredViewURLs = new List<string>();
        private int m_RepeatedShow;
        private bool m_SyncingFocus;
        internal WebScriptObject scriptObject;
        internal WebView webView;

        protected WebViewEditorWindow()
        {
            Resolution currentResolution = Screen.currentResolution;
            int num = (currentResolution.width < 0x400) ? currentResolution.width : 0x400;
            int num2 = (currentResolution.height < 0x380) ? (currentResolution.height - 0x60) : 800;
            int num3 = (currentResolution.width - num) / 2;
            int num4 = (currentResolution.height - num2) / 2;
            base.position = new Rect((float) num3, (float) num4, (float) num, (float) num2);
            this.m_RepeatedShow = 0;
        }

        public void About()
        {
            if (this.webView != null)
            {
                this.webView.LoadURL("chrome://version");
            }
        }

        public virtual void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Reload"), false, new GenericMenu.MenuFunction(this.Reload));
            if (Unsupported.IsDeveloperBuild())
            {
                menu.AddItem(new GUIContent("About"), false, new GenericMenu.MenuFunction(this.About));
            }
        }

        public static WebViewEditorWindow Create<T>(string title, string sourcesPath, int minWidth, int minHeight, int maxWidth, int maxHeight) where T: new()
        {
            WebViewEditorWindow window = ScriptableObject.CreateInstance<WebViewEditorWindow>();
            window.titleContent = new GUIContent(title);
            window.minSize = new Vector2((float) minWidth, (float) minHeight);
            window.maxSize = new Vector2((float) maxWidth, (float) maxHeight);
            window.m_InitialOpenURL = sourcesPath;
            window.m_GlobalObjectTypeName = typeof(T).FullName;
            window.Init();
            window.Show();
            return window;
        }

        public static WebViewEditorWindow CreateBase(string title, string sourcesPath, int minWidth, int minHeight, int maxWidth, int maxHeight)
        {
            WebViewEditorWindow window = EditorWindow.GetWindow<WebViewEditorWindow>(title);
            window.minSize = new Vector2((float) minWidth, (float) minHeight);
            window.maxSize = new Vector2((float) maxWidth, (float) maxHeight);
            window.m_InitialOpenURL = sourcesPath;
            window.m_GlobalObjectTypeName = null;
            window.Init();
            window.Show();
            return window;
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

        private void DoPostLoadTask()
        {
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.DoPostLoadTask));
            base.RepaintImmediately();
        }

        private bool FindWebView(string webViewUrl, out WebView webView)
        {
            webView = null;
            string key = MakeUrlKey(webViewUrl);
            return this.m_RegisteredViews.TryGetValue(key, out webView);
        }

        public WebView GetWebViewFromURL(string url)
        {
            string str = MakeUrlKey(url);
            return this.m_RegisteredViews[str];
        }

        public void Init()
        {
            if ((this.m_GlobalObject == null) && !string.IsNullOrEmpty(this.m_GlobalObjectTypeName))
            {
                Type type = Type.GetType(this.m_GlobalObjectTypeName);
                if (type != null)
                {
                    this.m_GlobalObject = Activator.CreateInstance(type);
                    JSProxyMgr.GetInstance().AddGlobalObject(this.m_GlobalObject.GetType().Name, this.m_GlobalObject);
                }
            }
        }

        private void InitWebView(Rect webViewRect)
        {
            if (this.webView == null)
            {
                int x = (int) webViewRect.x;
                int y = (int) webViewRect.y;
                int width = (int) webViewRect.width;
                int height = (int) webViewRect.height;
                this.webView = ScriptableObject.CreateInstance<WebView>();
                this.RegisterWebviewUrl(this.m_InitialOpenURL, this.webView);
                this.webView.InitWebView(base.m_Parent, x, y, width, height, false);
                this.webView.hideFlags = HideFlags.HideAndDontSave;
                this.SetFocus(base.hasFocus);
            }
            this.webView.SetDelegateObject(this);
            if (this.m_InitialOpenURL.StartsWith("http"))
            {
                this.webView.LoadURL(this.m_InitialOpenURL);
                this.m_PostLoadTimer = new Timer(30.0);
                this.m_PostLoadTimer.Elapsed += new ElapsedEventHandler(this.RaisePostLoadCondition);
                this.m_PostLoadTimer.Enabled = true;
            }
            else if (this.m_InitialOpenURL.StartsWith("file"))
            {
                this.webView.LoadFile(this.m_InitialOpenURL);
            }
            else
            {
                string path = Path.Combine(Uri.EscapeUriString(Path.Combine(EditorApplication.applicationContentsPath, "Resources")), this.m_InitialOpenURL);
                this.webView.LoadFile(path);
            }
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

        protected void LoadPage()
        {
            if (this.webView != null)
            {
                WebView view;
                if (!this.FindWebView(this.m_InitialOpenURL, out view) || (view == null))
                {
                    this.NotifyVisibility(false);
                    this.webView.SetHostView(null);
                    this.webView = null;
                    Rect webViewRect = GUIClip.Unclip(new Rect(0f, 0f, base.position.width, base.position.height));
                    this.InitWebView(webViewRect);
                    this.NotifyVisibility(true);
                }
                else if (view != this.webView)
                {
                    this.NotifyVisibility(false);
                    view.SetHostView(base.m_Parent);
                    this.webView.SetHostView(null);
                    this.webView = view;
                    this.NotifyVisibility(true);
                    this.webView.Show();
                }
            }
        }

        public void Logout()
        {
        }

        private static string MakeUrlKey(string webViewUrl)
        {
            string str;
            int index = webViewUrl.IndexOf("#");
            if (index != -1)
            {
                str = webViewUrl.Substring(0, index);
            }
            else
            {
                str = webViewUrl;
            }
            index = str.LastIndexOf("/");
            if (index == (str.Length - 1))
            {
                return str.Substring(0, index);
            }
            return str;
        }

        protected void NotifyVisibility(bool visible)
        {
            if (this.webView != null)
            {
                string scriptCode = "document.dispatchEvent(new CustomEvent('showWebView',{ detail: { visible:";
                scriptCode = scriptCode + (!visible ? "false" : "true") + "}, bubbles: true, cancelable: false }));";
                this.webView.ExecuteJavascript(scriptCode);
            }
        }

        public void OnAfterDeserialize()
        {
            this.m_RegisteredViews = new Dictionary<string, WebView>();
            for (int i = 0; i != Math.Min(this.m_RegisteredViewURLs.Count, this.m_RegisteredViewInstances.Count); i++)
            {
                this.m_RegisteredViews.Add(this.m_RegisteredViewURLs[i], this.m_RegisteredViewInstances[i]);
            }
        }

        public void OnBatchMode()
        {
            Rect webViewRect = GUIClip.Unclip(new Rect(0f, 0f, base.position.width, base.position.height));
            if ((this.m_InitialOpenURL != null) && (this.webView == null))
            {
                this.InitWebView(webViewRect);
            }
        }

        public void OnBecameInvisible()
        {
            if (this.webView != null)
            {
                this.webView.SetHostView(null);
            }
        }

        public void OnBeforeSerialize()
        {
            this.m_RegisteredViewURLs = new List<string>();
            this.m_RegisteredViewInstances = new List<WebView>();
            foreach (KeyValuePair<string, WebView> pair in this.m_RegisteredViews)
            {
                this.m_RegisteredViewURLs.Add(pair.Key);
                this.m_RegisteredViewInstances.Add(pair.Value);
            }
        }

        public void OnDestroy()
        {
            if (this.webView != null)
            {
                Object.DestroyImmediate(this.webView);
            }
            this.m_GlobalObject = null;
            foreach (WebView view in this.m_RegisteredViews.Values)
            {
                if (view != null)
                {
                    Object.DestroyImmediate(view);
                }
            }
            this.m_RegisteredViews.Clear();
            this.m_RegisteredViewURLs.Clear();
            this.m_RegisteredViewInstances.Clear();
        }

        public void OnEnable()
        {
            this.Init();
        }

        public void OnFocus()
        {
            this.SetFocus(true);
        }

        public void OnGUI()
        {
            Rect webViewRect = GUIClip.Unclip(new Rect(0f, 0f, base.position.width, base.position.height));
            if (this.m_RepeatedShow-- > 0)
            {
                this.Refresh();
            }
            if (this.m_InitialOpenURL != null)
            {
                if (this.webView == null)
                {
                    this.InitWebView(webViewRect);
                }
                if (Event.current.type == EventType.Layout)
                {
                    this.webView.SetSizeAndPosition((int) webViewRect.x, (int) webViewRect.y, (int) webViewRect.width, (int) webViewRect.height);
                }
            }
        }

        public void OnInitScripting()
        {
            this.SetScriptObject();
        }

        public void OnLoadError(string url)
        {
            if (this.webView == null)
            {
            }
        }

        public void OnLostFocus()
        {
            this.SetFocus(false);
        }

        private void RaisePostLoadCondition(object obj, ElapsedEventArgs args)
        {
            this.m_PostLoadTimer.Stop();
            this.m_PostLoadTimer = null;
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.DoPostLoadTask));
        }

        public void Refresh()
        {
            if (this.webView != null)
            {
                this.webView.Hide();
                this.webView.Show();
            }
        }

        private void RegisterWebviewUrl(string webViewUrl, WebView view)
        {
            string str = MakeUrlKey(webViewUrl);
            this.m_RegisteredViews[str] = view;
        }

        public void Reload()
        {
            if (this.webView != null)
            {
                this.webView.Reload();
            }
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
                        if (Application.platform != RuntimePlatform.WindowsEditor)
                        {
                            this.m_RepeatedShow = 15;
                        }
                        else
                        {
                            this.webView.Show();
                        }
                    }
                    this.webView.SetApplicationFocus(((base.m_Parent != null) && base.m_Parent.hasFocus) && base.hasFocus);
                    this.webView.SetFocus(value);
                }
                this.m_SyncingFocus = false;
            }
        }

        private void SetScriptObject()
        {
            if (this.webView != null)
            {
                this.CreateScriptObject();
                this.webView.DefineScriptObject("window.webScriptObject", this.scriptObject);
            }
        }

        public void ToggleMaximize()
        {
            base.maximized = !base.maximized;
            this.Refresh();
            this.SetFocus(true);
        }

        protected void UnregisterWebviewUrl(string webViewUrl)
        {
            string str = MakeUrlKey(webViewUrl);
            this.m_RegisteredViews[str] = null;
        }

        public string initialOpenUrl
        {
            get
            {
                return this.m_InitialOpenURL;
            }
            set
            {
                this.m_InitialOpenURL = value;
            }
        }
    }
}

