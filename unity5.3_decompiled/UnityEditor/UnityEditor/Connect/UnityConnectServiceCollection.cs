namespace UnityEditor.Connect
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Web;
    using UnityEditorInternal;
    using UnityEngine;

    internal class UnityConnectServiceCollection
    {
        [CompilerGenerated]
        private static Func<UnityConnectEditorWindow, bool> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<UnityConnectServiceData, string> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<KeyValuePair<string, UnityConnectServiceData>, ServiceInfo> <>f__am$cache7;
        private const string kDrawerContainerTitle = "Services";
        private string m_CurrentPageName = string.Empty;
        private string m_CurrentServiceName = string.Empty;
        private readonly Dictionary<string, UnityConnectServiceData> m_Services = new Dictionary<string, UnityConnectServiceData>();
        private static UnityConnectServiceCollection s_UnityConnectEditor;
        private static UnityConnectEditorWindow s_UnityConnectEditorWindow;

        private UnityConnectServiceCollection()
        {
            UnityConnect.instance.StateChanged += new StateChangedDelegate(this.InstanceStateChanged);
        }

        public bool AddService(UnityConnectServiceData cloudService)
        {
            if (this.m_Services.ContainsKey(cloudService.serviceName))
            {
                return false;
            }
            this.m_Services[cloudService.serviceName] = cloudService;
            return true;
        }

        public void EnableService(string name, bool enabled)
        {
            if (this.m_Services.ContainsKey(name))
            {
                this.m_Services[name].EnableService(enabled);
            }
        }

        private void EnsureDrawerIsVisible(bool forceFocus)
        {
            if ((s_UnityConnectEditorWindow == null) || !s_UnityConnectEditorWindow.UrlsMatch(this.GetAllServiceUrls()))
            {
                string title = "Services";
                int serviceEnv = UnityConnectPrefs.GetServiceEnv(this.m_CurrentServiceName);
                if (serviceEnv != 0)
                {
                    title = title + " [" + UnityConnectPrefs.kEnvironmentFamilies[serviceEnv] + "]";
                }
                s_UnityConnectEditorWindow = UnityConnectEditorWindow.Create(title, this.GetAllServiceUrls());
                s_UnityConnectEditorWindow.ErrorUrl = this.m_Services["ErrorHub"].serviceUrl;
                s_UnityConnectEditorWindow.minSize = new Vector2(275f, 50f);
            }
            string serviceUrl = this.m_Services[this.m_CurrentServiceName].serviceUrl;
            if (this.m_CurrentPageName.Length > 0)
            {
                serviceUrl = serviceUrl + "/#/" + this.m_CurrentPageName;
            }
            s_UnityConnectEditorWindow.currentUrl = serviceUrl;
            s_UnityConnectEditorWindow.Show();
            if (InternalEditorUtility.isApplicationActive && forceFocus)
            {
                s_UnityConnectEditorWindow.Focus();
            }
        }

        private string GetActualServiceName(string desiredServiceName, ConnectInfo state)
        {
            if (!state.online)
            {
                return "ErrorHub";
            }
            if (!state.ready)
            {
                return "Hub";
            }
            if (state.maintenance)
            {
                return "ErrorHub";
            }
            if (((desiredServiceName != "Hub") && state.online) && !state.loggedIn)
            {
                return "Hub";
            }
            if ((desiredServiceName == "ErrorHub") && state.online)
            {
                return "Hub";
            }
            if (string.IsNullOrEmpty(desiredServiceName))
            {
                return "Hub";
            }
            return desiredServiceName;
        }

        public ServiceInfo[] GetAllServiceInfos()
        {
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = item => new ServiceInfo(item.Value.serviceName, item.Value.serviceUrl, item.Value.serviceJsGlobalObjectName, item.Value.serviceJsGlobalObject.IsServiceEnabled());
            }
            return this.m_Services.Select<KeyValuePair<string, UnityConnectServiceData>, ServiceInfo>(<>f__am$cache7).ToArray<ServiceInfo>();
        }

        public List<string> GetAllServiceNames()
        {
            return this.m_Services.Keys.ToList<string>();
        }

        public List<string> GetAllServiceUrls()
        {
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = unityConnectData => unityConnectData.serviceUrl;
            }
            return this.m_Services.Values.Select<UnityConnectServiceData, string>(<>f__am$cache6).ToList<string>();
        }

        public UnityConnectServiceData GetServiceFromUrl(string searchUrl)
        {
            <GetServiceFromUrl>c__AnonStoreyAD yad = new <GetServiceFromUrl>c__AnonStoreyAD {
                searchUrl = searchUrl
            };
            return this.m_Services.FirstOrDefault<KeyValuePair<string, UnityConnectServiceData>>(new Func<KeyValuePair<string, UnityConnectServiceData>, bool>(yad.<>m__215)).Value;
        }

        public string GetUrlForService(string serviceName)
        {
            return (!this.m_Services.ContainsKey(serviceName) ? string.Empty : this.m_Services[serviceName].serviceUrl);
        }

        public WebView GetWebViewFromServiceName(string serviceName)
        {
            if ((s_UnityConnectEditorWindow == null) || !s_UnityConnectEditorWindow.UrlsMatch(this.GetAllServiceUrls()))
            {
                return null;
            }
            if (!this.m_Services.ContainsKey(serviceName))
            {
                return null;
            }
            ConnectInfo connectInfo = UnityConnect.instance.connectInfo;
            string actualServiceName = this.GetActualServiceName(serviceName, connectInfo);
            string serviceUrl = this.m_Services[actualServiceName].serviceUrl;
            return s_UnityConnectEditorWindow.GetWebViewFromURL(serviceUrl);
        }

        private void Init()
        {
            JSProxyMgr.GetInstance().AddGlobalObject("UnityConnectEditor", this);
        }

        protected void InstanceStateChanged(ConnectInfo state)
        {
            if (this.isDrawerOpen && state.ready)
            {
                string actualServiceName = this.GetActualServiceName(this.m_CurrentServiceName, state);
                if ((actualServiceName != this.m_CurrentServiceName) || ((s_UnityConnectEditorWindow != null) && (this.m_Services[actualServiceName].serviceUrl != s_UnityConnectEditorWindow.currentUrl)))
                {
                    bool forceFocus = ((s_UnityConnectEditorWindow != null) && (s_UnityConnectEditorWindow.webView != null)) && s_UnityConnectEditorWindow.webView.HasApplicationFocus();
                    this.ShowService(actualServiceName, forceFocus);
                }
            }
        }

        public void ReloadServices()
        {
            if (s_UnityConnectEditorWindow != null)
            {
                s_UnityConnectEditorWindow.Close();
                s_UnityConnectEditorWindow = null;
            }
            UnityConnect.instance.ClearCache();
        }

        public bool ServiceExist(string serviceName)
        {
            return this.m_Services.ContainsKey(serviceName);
        }

        public bool ShowService(string serviceName, bool forceFocus)
        {
            return this.ShowService(serviceName, string.Empty, forceFocus);
        }

        public bool ShowService(string serviceName, string atPage, bool forceFocus)
        {
            if (!this.m_Services.ContainsKey(serviceName))
            {
                return false;
            }
            ConnectInfo connectInfo = UnityConnect.instance.connectInfo;
            this.m_CurrentServiceName = this.GetActualServiceName(serviceName, connectInfo);
            this.m_CurrentPageName = atPage;
            this.EnsureDrawerIsVisible(forceFocus);
            return true;
        }

        public static UnityConnectServiceCollection instance
        {
            get
            {
                if (s_UnityConnectEditor == null)
                {
                    s_UnityConnectEditor = new UnityConnectServiceCollection();
                    s_UnityConnectEditor.Init();
                }
                return s_UnityConnectEditor;
            }
        }

        private bool isDrawerOpen
        {
            get
            {
                UnityConnectEditorWindow[] source = Resources.FindObjectsOfTypeAll(typeof(UnityConnectEditorWindow)) as UnityConnectEditorWindow[];
                if (source == null)
                {
                }
                return ((<>f__am$cache5 == null) && source.Any<UnityConnectEditorWindow>(<>f__am$cache5));
            }
        }

        [CompilerGenerated]
        private sealed class <GetServiceFromUrl>c__AnonStoreyAD
        {
            internal string searchUrl;

            internal bool <>m__215(KeyValuePair<string, UnityConnectServiceData> kvp)
            {
                return (kvp.Value.serviceUrl == this.searchUrl);
            }
        }

        public class ServiceInfo
        {
            public bool enabled;
            public string name;
            public string unityPath;
            public string url;

            public ServiceInfo(string name, string url, string unityPath, bool enabled)
            {
                this.name = name;
                this.url = url;
                this.unityPath = unityPath;
                this.enabled = enabled;
            }
        }
    }
}

