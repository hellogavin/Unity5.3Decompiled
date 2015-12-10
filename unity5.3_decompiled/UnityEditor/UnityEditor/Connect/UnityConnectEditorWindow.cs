namespace UnityEditor.Connect
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Web;
    using UnityEngine;

    internal class UnityConnectEditorWindow : WebViewEditorWindow
    {
        [CompilerGenerated]
        private static Func<UnityConnectEditorWindow, bool> <>f__am$cache3;
        private bool m_ClearInitialOpenURL = true;
        private List<string> m_ServiceUrls = new List<string>();

        protected UnityConnectEditorWindow()
        {
        }

        public static UnityConnectEditorWindow Create(string title, List<string> serviceUrls)
        {
            UnityConnectEditorWindow[] source = Resources.FindObjectsOfTypeAll(typeof(UnityConnectEditorWindow)) as UnityConnectEditorWindow[];
            if (source != null)
            {
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = win => win != null;
                }
                IEnumerator<UnityConnectEditorWindow> enumerator = source.Where<UnityConnectEditorWindow>(<>f__am$cache3).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        UnityConnectEditorWindow current = enumerator.Current;
                        current.titleContent = new GUIContent(title);
                        return current;
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
            }
            Type[] desiredDockNextTo = new Type[] { typeof(InspectorWindow) };
            UnityConnectEditorWindow window2 = EditorWindow.GetWindow<UnityConnectEditorWindow>(title, desiredDockNextTo);
            window2.m_ClearInitialOpenURL = false;
            window2.initialOpenUrl = serviceUrls[0];
            window2.Init();
            return window2;
        }

        public void OnEnable()
        {
            this.m_ServiceUrls = UnityConnectServiceCollection.instance.GetAllServiceUrls();
            base.OnEnable();
        }

        public void OnGUI()
        {
            if (this.m_ClearInitialOpenURL)
            {
                this.m_ClearInitialOpenURL = false;
                base.m_InitialOpenURL = (this.m_ServiceUrls.Count <= 0) ? null : UnityConnectServiceCollection.instance.GetUrlForService("Hub");
            }
            base.OnGUI();
        }

        public void OnInitScripting()
        {
            base.OnInitScripting();
        }

        public void OnLoadError(string url)
        {
            if (base.webView != null)
            {
                base.webView.LoadFile(EditorApplication.userJavascriptPackagesPath + "unityeditor-cloud-hub/dist/index.html?failure=load_error&reload_url=" + WWW.EscapeURL(url));
                if (url.StartsWith("http://") || url.StartsWith("https://"))
                {
                    base.UnregisterWebviewUrl(url);
                }
            }
        }

        public void ToggleMaximize()
        {
            base.ToggleMaximize();
        }

        public bool UrlsMatch(List<string> referenceUrls)
        {
            <UrlsMatch>c__AnonStoreyAC yac = new <UrlsMatch>c__AnonStoreyAC {
                referenceUrls = referenceUrls
            };
            if (this.m_ServiceUrls.Count != yac.referenceUrls.Count)
            {
                return false;
            }
            return !this.m_ServiceUrls.Where<string>(new Func<string, int, bool>(yac.<>m__213)).Any<string>();
        }

        public string currentUrl
        {
            get
            {
                return base.m_InitialOpenURL;
            }
            set
            {
                if (base.m_InitialOpenURL != value)
                {
                    base.m_InitialOpenURL = value;
                    base.LoadPage();
                }
            }
        }

        public string ErrorUrl { get; set; }

        [CompilerGenerated]
        private sealed class <UrlsMatch>c__AnonStoreyAC
        {
            internal List<string> referenceUrls;

            internal bool <>m__213(string t, int idx)
            {
                return (t != this.referenceUrls[idx]);
            }
        }
    }
}

