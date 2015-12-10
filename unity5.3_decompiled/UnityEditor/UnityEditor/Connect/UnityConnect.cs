namespace UnityEditor.Connect
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Web;
    using UnityEngine;

    [InitializeOnLoad]
    internal sealed class UnityConnect
    {
        private static readonly UnityConnect s_Instance = new UnityConnect();

        public event ProjectStateChangedDelegate ProjectStateChanged;

        public event StateChangedDelegate StateChanged;

        static UnityConnect()
        {
            JSProxyMgr.GetInstance().AddGlobalObject("unity/connect", s_Instance);
        }

        private UnityConnect()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void BindProject(string projectGUID, string projectName, string organizationId);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ClearCache();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ClearErrors();
        public bool DisplayDialog(string title, string message, string okBtn, string cancelBtn)
        {
            return EditorUtility.DisplayDialog(title, message, okBtn, cancelBtn);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetAccessToken();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetAPIVersion();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetConfigurationURL(CloudConfigUrl config);
        public string GetConfigurationUrlByIndex(int index)
        {
            if (index == 0)
            {
                return this.GetConfigurationURL(CloudConfigUrl.CloudCore);
            }
            if (index == 1)
            {
                return this.GetConfigurationURL(CloudConfigUrl.CloudCollab);
            }
            if (index == 2)
            {
                return this.GetConfigurationURL(CloudConfigUrl.CloudWebauth);
            }
            if (index == 3)
            {
                return this.GetConfigurationURL(CloudConfigUrl.CloudLogin);
            }
            return string.Empty;
        }

        public ConnectInfo GetConnectInfo()
        {
            return this.connectInfo;
        }

        public string GetCoreConfigurationUrl()
        {
            return this.GetConfigurationURL(CloudConfigUrl.CloudCore);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetEnvironment();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int GetOrganizationForeignKey();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetOrganizationId();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetOrganizationName();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetProjectGUID();
        public ProjectInfo GetProjectInfo()
        {
            return this.projectInfo;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetProjectName();
        public UserInfo GetUserInfo()
        {
            return this.userInfo;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetUserName();
        public void GoToHub(string page)
        {
            UnityConnectServiceCollection.instance.ShowService("Hub", page, true);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Logout();
        private static void OnProjectStateChanged()
        {
            ProjectStateChangedDelegate projectStateChanged = instance.ProjectStateChanged;
            if (projectStateChanged != null)
            {
                projectStateChanged(instance.projectInfo);
            }
        }

        private static void OnStateChanged()
        {
            StateChangedDelegate stateChanged = instance.StateChanged;
            if (stateChanged != null)
            {
                stateChanged(instance.connectInfo);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void OpenAuthorizedURLInWebBrowser(string url);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void RefreshProject();
        public bool SetCOPPACompliance(int compliance)
        {
            return this.SetCOPPACompliance((COPPACompliance) compliance);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool SetCOPPACompliance(COPPACompliance compliance);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ShowLogin();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void UnbindProject();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void UnhandledError(string request, int responseCode, string response);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void WorkOffline(bool rememberDecision);

        public bool canBuildWithUPID { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string configuration { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public ConnectInfo connectInfo { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static UnityConnect instance
        {
            get
            {
                return s_Instance;
            }
        }

        public int lastErrorCode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string lastErrorMessage { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool loggedIn { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool online { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool preferencesEnabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public ProjectInfo projectInfo { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool projectValid { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool skipMissingUPID { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public UserInfo userInfo { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool workingOffline { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

