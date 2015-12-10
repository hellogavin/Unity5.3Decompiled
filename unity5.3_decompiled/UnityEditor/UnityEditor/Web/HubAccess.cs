namespace UnityEditor.Web
{
    using System;
    using UnityEditor;
    using UnityEditor.Connect;

    [InitializeOnLoad]
    internal class HubAccess : CloudServiceAccess
    {
        private const string kServiceDisplayName = "Services";
        public const string kServiceName = "Hub";
        private const string kServiceUrl = "https://public-cdn.cloud.unity3d.com/editor/5.3/production/cloud/hub";

        static HubAccess()
        {
            UnityConnectServiceData cloudService = new UnityConnectServiceData("Hub", "https://public-cdn.cloud.unity3d.com/editor/5.3/production/cloud/hub", new HubAccess(), "unity/project/cloud/hub");
            UnityConnectServiceCollection.instance.AddService(cloudService);
        }

        public void EnableCloudService(string name, bool enabled)
        {
            UnityConnectServiceCollection.instance.EnableService(name, enabled);
        }

        public override string GetServiceDisplayName()
        {
            return "Services";
        }

        public override string GetServiceName()
        {
            return "Hub";
        }

        public UnityConnectServiceCollection.ServiceInfo[] GetServices()
        {
            return UnityConnectServiceCollection.instance.GetAllServiceInfos();
        }

        [MenuItem("Window/Services %0", false, 0x7cf)]
        private static void ShowMyWindow()
        {
            UnityConnectServiceCollection.instance.ShowService("Hub", true);
        }

        public void ShowService(string name)
        {
            UnityConnectServiceCollection.instance.ShowService(name, true);
        }
    }
}

