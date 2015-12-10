namespace UnityEditor.Web
{
    using System;
    using UnityEditor;
    using UnityEditor.Connect;

    internal abstract class CloudServiceAccess
    {
        private const string kServiceEnabled = "ServiceEnabled";

        protected CloudServiceAccess()
        {
        }

        public virtual void EnableService(bool enabled)
        {
            this.SetServiceConfig("ServiceEnabled", enabled.ToString());
        }

        protected string GetSafeServiceName()
        {
            return this.GetServiceName().Replace(' ', '_');
        }

        public string GetServiceConfig(string key)
        {
            string name = this.GetSafeServiceName() + "_" + key;
            string str2 = string.Empty;
            if (PlayerSettings.GetPropertyOptionalString(name, ref str2))
            {
                return str2;
            }
            return string.Empty;
        }

        public virtual string GetServiceDisplayName()
        {
            return this.GetServiceName();
        }

        public abstract string GetServiceName();
        protected WebView GetWebView()
        {
            return UnityConnectServiceCollection.instance.GetWebViewFromServiceName(this.GetServiceName());
        }

        public void GoBackToHub()
        {
            UnityConnectServiceCollection.instance.ShowService("Hub", true);
        }

        public virtual bool IsServiceEnabled()
        {
            bool flag;
            bool.TryParse(this.GetServiceConfig("ServiceEnabled"), out flag);
            return flag;
        }

        public void SetServiceConfig(string key, string value)
        {
            string name = this.GetSafeServiceName() + "_" + key;
            string str2 = string.Empty;
            if (!PlayerSettings.GetPropertyOptionalString(name, ref str2))
            {
                PlayerSettings.InitializePropertyString(name, value);
            }
            else
            {
                PlayerSettings.SetPropertyString(name, value);
            }
        }
    }
}

