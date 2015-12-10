namespace UnityEditor.Connect
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;

    internal class UnityConnectPrefs
    {
        public const int kCustomEnv = 3;
        public static string[] kEnvironmentFamilies = new string[] { "Production", "Staging", "Dev", "Custom" };
        public const int kProductionEnv = 0;
        public const string kSvcCustomPortPref = "CloudPanelCustomPort";
        public const string kSvcCustomUrlPref = "CloudPanelCustomUrl";
        public const string kSvcEnvPref = "CloudPanelServer";
        protected static Dictionary<string, CloudPanelPref> m_CloudPanelPref = new Dictionary<string, CloudPanelPref>();

        public static string FixUrl(string url, string serviceName)
        {
            string str = url;
            int serviceEnv = GetServiceEnv(serviceName);
            if (serviceEnv != 0)
            {
                if (str.StartsWith("http://") || str.StartsWith("https://"))
                {
                    if (serviceEnv == 3)
                    {
                        string str2 = EditorPrefs.GetString(ServicePrefKey("CloudPanelCustomUrl", serviceName));
                        int @int = EditorPrefs.GetInt(ServicePrefKey("CloudPanelCustomPort", serviceName));
                        return (str2 + ":" + @int);
                    }
                    return str.ToLower().Replace("/" + kEnvironmentFamilies[0].ToLower() + "/", "/" + kEnvironmentFamilies[serviceEnv].ToLower() + "/");
                }
                if (str.StartsWith("file://"))
                {
                    str = str.Substring(7);
                    if (serviceEnv == 3)
                    {
                        string str3 = EditorPrefs.GetString(ServicePrefKey("CloudPanelCustomUrl", serviceName));
                        int num3 = EditorPrefs.GetInt(ServicePrefKey("CloudPanelCustomPort", serviceName));
                        str = str3 + ":" + num3;
                    }
                    return str;
                }
                if ((!str.StartsWith("file://") && !str.StartsWith("http://")) && !str.StartsWith("https://"))
                {
                    return ("http://" + str);
                }
            }
            return str;
        }

        protected static CloudPanelPref GetPanelPref(string serviceName)
        {
            if (m_CloudPanelPref.ContainsKey(serviceName))
            {
                return m_CloudPanelPref[serviceName];
            }
            CloudPanelPref pref = new CloudPanelPref(serviceName);
            m_CloudPanelPref.Add(serviceName, pref);
            return pref;
        }

        public static int GetServiceEnv(string serviceName)
        {
            if (Unsupported.IsDeveloperBuild() || UnityConnect.preferencesEnabled)
            {
                return EditorPrefs.GetInt(ServicePrefKey("CloudPanelServer", serviceName));
            }
            for (int i = 0; i < kEnvironmentFamilies.Length; i++)
            {
                string str = kEnvironmentFamilies[i];
                if (str.Equals(UnityConnect.instance.configuration, StringComparison.InvariantCultureIgnoreCase))
                {
                    return i;
                }
            }
            return 0;
        }

        public static string ServicePrefKey(string baseKey, string serviceName)
        {
            return (baseKey + "/" + serviceName);
        }

        public static void ShowPanelPrefUI()
        {
            List<string> allServiceNames = UnityConnectServiceCollection.instance.GetAllServiceNames();
            bool flag = false;
            foreach (string str in allServiceNames)
            {
                CloudPanelPref panelPref = GetPanelPref(str);
                int result = EditorGUILayout.Popup(str, panelPref.m_CloudPanelServer, kEnvironmentFamilies, new GUILayoutOption[0]);
                if (result != panelPref.m_CloudPanelServer)
                {
                    panelPref.m_CloudPanelServer = result;
                    flag = true;
                }
                if (panelPref.m_CloudPanelServer == 3)
                {
                    EditorGUI.indentLevel++;
                    string str2 = EditorGUILayout.TextField("Custom server URL", panelPref.m_CloudPanelCustomUrl, new GUILayoutOption[0]);
                    if (str2 != panelPref.m_CloudPanelCustomUrl)
                    {
                        panelPref.m_CloudPanelCustomUrl = str2;
                        flag = true;
                    }
                    int.TryParse(EditorGUILayout.TextField("Custom server port", panelPref.m_CloudPanelCustomPort.ToString(), new GUILayoutOption[0]), out result);
                    if (result != panelPref.m_CloudPanelCustomPort)
                    {
                        panelPref.m_CloudPanelCustomPort = result;
                        flag = true;
                    }
                    EditorGUI.indentLevel--;
                }
            }
            if (flag)
            {
                UnityConnectServiceCollection.instance.ReloadServices();
            }
        }

        public static void StorePanelPrefs()
        {
            if (Unsupported.IsDeveloperBuild() || UnityConnect.preferencesEnabled)
            {
                foreach (KeyValuePair<string, CloudPanelPref> pair in m_CloudPanelPref)
                {
                    pair.Value.StoreCloudServicePref();
                }
            }
        }

        protected class CloudPanelPref
        {
            public int m_CloudPanelCustomPort;
            public string m_CloudPanelCustomUrl;
            public int m_CloudPanelServer;
            public string m_ServiceName;

            public CloudPanelPref(string serviceName)
            {
                this.m_ServiceName = serviceName;
                this.m_CloudPanelServer = UnityConnectPrefs.GetServiceEnv(this.m_ServiceName);
                this.m_CloudPanelCustomUrl = EditorPrefs.GetString(UnityConnectPrefs.ServicePrefKey("CloudPanelCustomUrl", this.m_ServiceName));
                this.m_CloudPanelCustomPort = EditorPrefs.GetInt(UnityConnectPrefs.ServicePrefKey("CloudPanelCustomPort", this.m_ServiceName));
            }

            public void StoreCloudServicePref()
            {
                EditorPrefs.SetInt(UnityConnectPrefs.ServicePrefKey("CloudPanelServer", this.m_ServiceName), this.m_CloudPanelServer);
                EditorPrefs.SetString(UnityConnectPrefs.ServicePrefKey("CloudPanelCustomUrl", this.m_ServiceName), this.m_CloudPanelCustomUrl);
                EditorPrefs.SetInt(UnityConnectPrefs.ServicePrefKey("CloudPanelCustomPort", this.m_ServiceName), this.m_CloudPanelCustomPort);
            }
        }
    }
}

