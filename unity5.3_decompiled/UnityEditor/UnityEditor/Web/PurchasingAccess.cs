namespace UnityEditor.Web
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography.X509Certificates;
    using UnityEditor;
    using UnityEditor.Connect;
    using UnityEngine;
    using UnityEngine.Connect;

    [InitializeOnLoad]
    internal class PurchasingAccess : CloudServiceAccess
    {
        [CompilerGenerated]
        private static RemoteCertificateValidationCallback <>f__am$cache2;
        private static readonly Uri kPackageUri = new Uri("https://public-cdn.cloud.unity3d.com/UnityEngine.Cloud.Purchasing.unitypackage");
        private const string kServiceDisplayName = "In App Purchasing";
        private const string kServiceName = "Purchasing";
        private const string kServiceUrl = "https://public-cdn.cloud.unity3d.com/editor/5.3/production/cloud/purchasing";
        private bool m_InstallInProgress;

        static PurchasingAccess()
        {
            UnityConnectServiceData cloudService = new UnityConnectServiceData("Purchasing", "https://public-cdn.cloud.unity3d.com/editor/5.3/production/cloud/purchasing", new PurchasingAccess(), "unity/project/cloud/purchasing");
            UnityConnectServiceCollection.instance.AddService(cloudService);
        }

        public override void EnableService(bool enabled)
        {
            UnityPurchasingSettings.enabled = enabled;
        }

        private void ExecuteJSMethod(string name)
        {
            this.ExecuteJSMethod(name, null);
        }

        private void ExecuteJSMethod(string name, string arg)
        {
            string scriptCode = string.Format("UnityPurchasing.{0}({1})", name, (arg != null) ? string.Format("\"{0}\"", arg) : string.Empty);
            WebView webView = base.GetWebView();
            if (webView != null)
            {
                webView.ExecuteJavascript(scriptCode);
            }
        }

        public override string GetServiceDisplayName()
        {
            return "In App Purchasing";
        }

        public override string GetServiceName()
        {
            return "Purchasing";
        }

        public void InstallUnityPackage()
        {
            <InstallUnityPackage>c__AnonStoreyAE yae = new <InstallUnityPackage>c__AnonStoreyAE {
                <>f__this = this
            };
            if (!this.m_InstallInProgress)
            {
                yae.originalCallback = ServicePointManager.ServerCertificateValidationCallback;
                if (Application.platform != RuntimePlatform.OSXEditor)
                {
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = (a, b, c, d) => true;
                    }
                    ServicePointManager.ServerCertificateValidationCallback = <>f__am$cache2;
                }
                this.m_InstallInProgress = true;
                yae.location = FileUtil.GetUniqueTempPathInProject();
                yae.location = Path.ChangeExtension(yae.location, ".unitypackage");
                WebClient client = new WebClient();
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(yae.<>m__219);
                client.DownloadFileAsync(kPackageUri, yae.location);
            }
        }

        public override bool IsServiceEnabled()
        {
            return UnityPurchasingSettings.enabled;
        }

        [CompilerGenerated]
        private sealed class <InstallUnityPackage>c__AnonStoreyAE
        {
            internal PurchasingAccess <>f__this;
            internal string location;
            internal RemoteCertificateValidationCallback originalCallback;

            internal void <>m__219(object sender, AsyncCompletedEventArgs args)
            {
                <InstallUnityPackage>c__AnonStoreyAF yaf;
                yaf = new <InstallUnityPackage>c__AnonStoreyAF {
                    <>f__ref$174 = this,
                    args = args,
                    handler = null,
                    handler = new EditorApplication.CallbackFunction(yaf.<>m__21A)
                };
                EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, yaf.handler);
            }

            private sealed class <InstallUnityPackage>c__AnonStoreyAF
            {
                internal PurchasingAccess.<InstallUnityPackage>c__AnonStoreyAE <>f__ref$174;
                internal AsyncCompletedEventArgs args;
                internal EditorApplication.CallbackFunction handler;

                internal void <>m__21A()
                {
                    ServicePointManager.ServerCertificateValidationCallback = this.<>f__ref$174.originalCallback;
                    EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, this.handler);
                    this.<>f__ref$174.<>f__this.m_InstallInProgress = false;
                    if (this.args.Error != null)
                    {
                        this.<>f__ref$174.<>f__this.ExecuteJSMethod("OnDownloadFailed", this.args.Error.Message);
                    }
                    else
                    {
                        AssetDatabase.ImportPackage(this.<>f__ref$174.location, false);
                        this.<>f__ref$174.<>f__this.ExecuteJSMethod("OnDownloadComplete");
                    }
                }
            }
        }
    }
}

