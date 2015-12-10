namespace UnityEditor.Web
{
    using System;
    using UnityEditor;
    using UnityEditor.Connect;
    using UnityEngine;

    [InitializeOnLoad]
    internal class UnetAccess : CloudServiceAccess
    {
        private const string kMultiplayerNetworkingIdKey = "CloudNetworkingId";
        private const string kServiceDisplayName = "Multiplayer";
        private const string kServiceName = "UNet";
        private const string kServiceUrl = "https://public-cdn.cloud.unity3d.com/editor/5.3/production/cloud/unet";

        static UnetAccess()
        {
            UnityConnectServiceData cloudService = new UnityConnectServiceData("UNet", "https://public-cdn.cloud.unity3d.com/editor/5.3/production/cloud/unet", new UnetAccess(), "unity/project/cloud/networking");
            UnityConnectServiceCollection.instance.AddService(cloudService);
        }

        public override string GetServiceDisplayName()
        {
            return "Multiplayer";
        }

        public override string GetServiceName()
        {
            return "UNet";
        }

        public void SetMultiplayerId(int id)
        {
            PlayerSettings.InitializePropertyInt("CloudNetworkingId", id);
            PlayerPrefs.SetString("CloudNetworkingId", id.ToString());
            PlayerPrefs.Save();
        }
    }
}

