namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Security;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography.X509Certificates;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AssetStoreClient
    {
        [CompilerGenerated]
        private static RemoteCertificateValidationCallback <>f__am$cache4;
        [CompilerGenerated]
        private static Predicate<string> <>f__am$cache5;
        private const string kUnauthSessionID = "26c4202eb475d02864b40827dfff11a14657aa41";
        private static string s_AssetStoreSearchUrl;
        private static string s_AssetStoreUrl;
        private static string sLoginErrorMessage;
        private static LoginState sLoginState;

        static AssetStoreClient()
        {
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = new RemoteCertificateValidationCallback(AssetStoreClient.<AssetStoreClient>m__8C);
            }
            ServicePointManager.ServerCertificateValidationCallback = <>f__am$cache4;
        }

        [CompilerGenerated]
        private static bool <AssetStoreClient>m__8C(object, X509Certificate, X509Chain, SslPolicyErrors)
        {
            return true;
        }

        private static string APISearchUrl(string path)
        {
            return string.Format("{0}/public-api{2}.json?{1}", AssetStoreSearchUrl, VersionParams, path);
        }

        private static string APIUrl(string path)
        {
            return string.Format("{0}/api{2}.json?{1}", AssetStoreUrl, VersionParams, path);
        }

        internal static AsyncHTTPClient AssetsInfo(List<AssetStoreAsset> assets, AssetStoreResultBase<AssetStoreAssetsInfo>.Callback callback)
        {
            <AssetsInfo>c__AnonStorey52 storey = new <AssetsInfo>c__AnonStorey52();
            string url = APIUrl("/assets/list");
            foreach (AssetStoreAsset asset in assets)
            {
                url = url + "&id=" + asset.id.ToString();
            }
            storey.r = new AssetStoreAssetsInfo(callback, assets);
            return CreateJSONRequest(url, new DoneCallback(storey.<>m__91));
        }

        internal static AsyncHTTPClient BuildPackage(AssetStoreAsset asset, AssetStoreResultBase<BuildPackageResult>.Callback callback)
        {
            <BuildPackage>c__AnonStorey54 storey = new <BuildPackage>c__AnonStorey54();
            string url = APIUrl("/content/download/" + asset.packageID.ToString());
            storey.r = new BuildPackageResult(asset, callback);
            return CreateJSONRequest(url, new DoneCallback(storey.<>m__93));
        }

        private static AsyncHTTPClient CreateJSONRequest(string url, DoneCallback callback)
        {
            AsyncHTTPClient client = new AsyncHTTPClient(url);
            client.header["X-Unity-Session"] = ActiveOrUnauthSessionID + GetToken();
            client.doneCallback = WrapJsonCallback(callback);
            client.Begin();
            return client;
        }

        private static AsyncHTTPClient CreateJSONRequestDelete(string url, DoneCallback callback)
        {
            AsyncHTTPClient client = new AsyncHTTPClient(url, "DELETE");
            client.header["X-Unity-Session"] = ActiveOrUnauthSessionID + GetToken();
            client.doneCallback = WrapJsonCallback(callback);
            client.Begin();
            return client;
        }

        private static AsyncHTTPClient CreateJSONRequestPost(string url, Dictionary<string, string> param, DoneCallback callback)
        {
            AsyncHTTPClient client = new AsyncHTTPClient(url);
            client.header["X-Unity-Session"] = ActiveOrUnauthSessionID + GetToken();
            client.postDictionary = param;
            client.doneCallback = WrapJsonCallback(callback);
            client.Begin();
            return client;
        }

        private static AsyncHTTPClient CreateJSONRequestPost(string url, string postData, DoneCallback callback)
        {
            AsyncHTTPClient client = new AsyncHTTPClient(url);
            client.header["X-Unity-Session"] = ActiveOrUnauthSessionID + GetToken();
            client.postData = postData;
            client.doneCallback = WrapJsonCallback(callback);
            client.Begin();
            return client;
        }

        internal static AsyncHTTPClient DirectPurchase(int packageID, string password, AssetStoreResultBase<PurchaseResult>.Callback callback)
        {
            <DirectPurchase>c__AnonStorey53 storey = new <DirectPurchase>c__AnonStorey53();
            string url = APIUrl(string.Format("/purchase/direct/{0}", packageID.ToString()));
            storey.r = new PurchaseResult(callback);
            Dictionary<string, string> param = new Dictionary<string, string>();
            param["password"] = password;
            return CreateJSONRequestPost(url, param, new DoneCallback(storey.<>m__92));
        }

        private static string GetToken()
        {
            return InternalEditorUtility.GetAuthToken();
        }

        public static bool LoggedIn()
        {
            return !string.IsNullOrEmpty(ActiveSessionID);
        }

        public static bool LoggedOut()
        {
            return string.IsNullOrEmpty(ActiveSessionID);
        }

        public static bool LoginError()
        {
            return (sLoginState == LoginState.LOGIN_ERROR);
        }

        public static bool LoginInProgress()
        {
            return (sLoginState == LoginState.IN_PROGRESS);
        }

        internal static void LoginWithCredentials(string username, string password, bool rememberMe, DoneLoginCallback callback)
        {
            if (sLoginState == LoginState.IN_PROGRESS)
            {
                Debug.LogError("Tried to login with credentials while already in progress of logging in");
            }
            else
            {
                sLoginState = LoginState.IN_PROGRESS;
                RememberSession = rememberMe;
                string str = AssetStoreUrl + "/login?skip_terms=1";
                sLoginErrorMessage = null;
                AsyncHTTPClient client = new AsyncHTTPClient(str.Replace("http://", "https://")) {
                    postData = "user=" + username + "&pass=" + password
                };
                client.header["X-Unity-Session"] = "26c4202eb475d02864b40827dfff11a14657aa41" + GetToken();
                client.doneCallback = WrapLoginCallback(callback);
                client.Begin();
            }
        }

        internal static void LoginWithRememberedSession(DoneLoginCallback callback)
        {
            if (sLoginState == LoginState.IN_PROGRESS)
            {
                Debug.LogError("Tried to login with remembered session while already in progress of logging in");
            }
            else
            {
                sLoginState = LoginState.IN_PROGRESS;
                if (!RememberSession)
                {
                    SavedSessionID = string.Empty;
                }
                string str = AssetStoreUrl + "/login?skip_terms=1&reuse_session=" + SavedSessionID;
                sLoginErrorMessage = null;
                AsyncHTTPClient client = new AsyncHTTPClient(str);
                client.header["X-Unity-Session"] = "26c4202eb475d02864b40827dfff11a14657aa41" + GetToken();
                client.doneCallback = WrapLoginCallback(callback);
                client.Begin();
            }
        }

        public static void Logout()
        {
            ActiveSessionID = string.Empty;
            SavedSessionID = string.Empty;
            sLoginState = LoginState.LOGGED_OUT;
        }

        private static AssetStoreResponse ParseContent(AsyncHTTPClient job)
        {
            string str2;
            string str3;
            AssetStoreResponse response = new AssetStoreResponse {
                job = job,
                dict = null,
                ok = false
            };
            AsyncHTTPClient.State state = job.state;
            string text = job.text;
            if (!AsyncHTTPClient.IsSuccess(state))
            {
                Console.WriteLine(text);
                return response;
            }
            response.dict = ParseJSON(text, out str2, out str3);
            if (str2 == "error")
            {
                Debug.LogError("Request error (" + str2 + "): " + str3);
                return response;
            }
            response.ok = true;
            return response;
        }

        private static Dictionary<string, JSONValue> ParseJSON(string content, out string status, out string message)
        {
            JSONValue value2;
            Dictionary<string, JSONValue> dictionary;
            message = null;
            status = null;
            try
            {
                value2 = new JSONParser(content).Parse();
            }
            catch (JSONParseException exception)
            {
                Debug.Log("Error parsing server reply: " + content);
                Debug.Log(exception.Message);
                return null;
            }
            try
            {
                dictionary = value2.AsDict(true);
                if (dictionary == null)
                {
                    Debug.Log("Error parsing server message: " + content);
                    return null;
                }
                if (dictionary.ContainsKey("result"))
                {
                    JSONValue value3 = dictionary["result"];
                    if (value3.IsDict())
                    {
                        dictionary = dictionary["result"].AsDict(true);
                    }
                }
                if (dictionary.ContainsKey("message"))
                {
                    message = dictionary["message"].AsString(true);
                }
                if (dictionary.ContainsKey("status"))
                {
                    status = dictionary["status"].AsString(true);
                    return dictionary;
                }
                if (dictionary.ContainsKey("error"))
                {
                    status = dictionary["error"].AsString(true);
                    if (status == string.Empty)
                    {
                        status = "ok";
                    }
                    return dictionary;
                }
                status = "ok";
                return dictionary;
            }
            catch (JSONTypeException exception2)
            {
                Debug.Log("Error parsing server reply. " + content);
                Debug.Log(exception2.Message);
                return null;
            }
            return dictionary;
        }

        internal static AsyncHTTPClient SearchAssets(string searchString, string[] requiredClassNames, string[] assetLabels, List<SearchCount> counts, AssetStoreResultBase<AssetStoreSearchResults>.Callback callback)
        {
            <SearchAssets>c__AnonStorey51 storey = new <SearchAssets>c__AnonStorey51();
            string str = string.Empty;
            string str2 = string.Empty;
            string str3 = string.Empty;
            string str4 = string.Empty;
            foreach (SearchCount count in counts)
            {
                str = str + str4 + count.offset;
                str2 = str2 + str4 + count.limit;
                str3 = str3 + str4 + count.name;
                str4 = ",";
            }
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = a => a.Equals("MonoScript", StringComparison.OrdinalIgnoreCase);
            }
            if (Array.Exists<string>(requiredClassNames, <>f__am$cache5))
            {
                Array.Resize<string>(ref requiredClassNames, requiredClassNames.Length + 1);
                requiredClassNames[requiredClassNames.Length - 1] = "Script";
            }
            object[] args = new object[] { APISearchUrl("/search/assets"), Uri.EscapeDataString(searchString), Uri.EscapeDataString(string.Join(",", requiredClassNames)), Uri.EscapeDataString(string.Join(",", assetLabels)), str, str2, str3 };
            string url = string.Format("{0}&q={1}&c={2}&l={3}&O={4}&N={5}&G={6}", args);
            storey.r = new AssetStoreSearchResults(callback);
            return CreateJSONRequest(url, new DoneCallback(storey.<>m__90));
        }

        private static AsyncHTTPClient.DoneCallback WrapJsonCallback(DoneCallback callback)
        {
            <WrapJsonCallback>c__AnonStorey50 storey = new <WrapJsonCallback>c__AnonStorey50 {
                callback = callback
            };
            return new AsyncHTTPClient.DoneCallback(storey.<>m__8E);
        }

        private static AsyncHTTPClient.DoneCallback WrapLoginCallback(DoneLoginCallback callback)
        {
            <WrapLoginCallback>c__AnonStorey4F storeyf = new <WrapLoginCallback>c__AnonStorey4F {
                callback = callback
            };
            return new AsyncHTTPClient.DoneCallback(storeyf.<>m__8D);
        }

        private static string ActiveOrUnauthSessionID
        {
            get
            {
                string activeSessionID = ActiveSessionID;
                if (activeSessionID == string.Empty)
                {
                    return "26c4202eb475d02864b40827dfff11a14657aa41";
                }
                return activeSessionID;
            }
        }

        internal static string ActiveSessionID
        {
            get
            {
                if (AssetStoreContext.SessionHasString("kharma.active_sessionid"))
                {
                    return AssetStoreContext.SessionGetString("kharma.active_sessionid");
                }
                return string.Empty;
            }
            set
            {
                AssetStoreContext.SessionSetString("kharma.active_sessionid", value);
            }
        }

        private static string AssetStoreSearchUrl
        {
            get
            {
                if (s_AssetStoreSearchUrl == null)
                {
                    s_AssetStoreSearchUrl = AssetStoreUtils.GetAssetStoreSearchUrl();
                }
                return s_AssetStoreSearchUrl;
            }
        }

        private static string AssetStoreUrl
        {
            get
            {
                if (s_AssetStoreUrl == null)
                {
                    s_AssetStoreUrl = AssetStoreUtils.GetAssetStoreUrl();
                }
                return s_AssetStoreUrl;
            }
        }

        public static bool HasActiveSessionID
        {
            get
            {
                return !string.IsNullOrEmpty(ActiveSessionID);
            }
        }

        public static bool HasSavedSessionID
        {
            get
            {
                return !string.IsNullOrEmpty(SavedSessionID);
            }
        }

        public static string LoginErrorMessage
        {
            get
            {
                return sLoginErrorMessage;
            }
        }

        public static bool RememberSession
        {
            get
            {
                return (EditorPrefs.GetString("kharma.remember_session") == "1");
            }
            set
            {
                EditorPrefs.SetString("kharma.remember_session", !value ? "0" : "1");
            }
        }

        private static string SavedSessionID
        {
            get
            {
                if (RememberSession)
                {
                    return EditorPrefs.GetString("kharma.sessionid", string.Empty);
                }
                return string.Empty;
            }
            set
            {
                EditorPrefs.SetString("kharma.sessionid", value);
            }
        }

        private static string VersionParams
        {
            get
            {
                return ("unityversion=" + Uri.EscapeDataString(Application.unityVersion) + "&skip_terms=1");
            }
        }

        [CompilerGenerated]
        private sealed class <AssetsInfo>c__AnonStorey52
        {
            internal AssetStoreAssetsInfo r;

            internal void <>m__91(AssetStoreResponse ar)
            {
                this.r.Parse(ar);
            }
        }

        [CompilerGenerated]
        private sealed class <BuildPackage>c__AnonStorey54
        {
            internal BuildPackageResult r;

            internal void <>m__93(AssetStoreResponse ar)
            {
                this.r.Parse(ar);
            }
        }

        [CompilerGenerated]
        private sealed class <DirectPurchase>c__AnonStorey53
        {
            internal PurchaseResult r;

            internal void <>m__92(AssetStoreResponse ar)
            {
                this.r.Parse(ar);
            }
        }

        [CompilerGenerated]
        private sealed class <SearchAssets>c__AnonStorey51
        {
            internal AssetStoreSearchResults r;

            internal void <>m__90(AssetStoreResponse ar)
            {
                this.r.Parse(ar);
            }
        }

        [CompilerGenerated]
        private sealed class <WrapJsonCallback>c__AnonStorey50
        {
            internal AssetStoreClient.DoneCallback callback;

            internal void <>m__8E(AsyncHTTPClient job)
            {
                if (job.IsDone())
                {
                    try
                    {
                        AssetStoreResponse response = AssetStoreClient.ParseContent(job);
                        this.callback(response);
                    }
                    catch (Exception exception)
                    {
                        Debug.Log("Uncaught exception in async net callback: " + exception.Message);
                        Debug.Log(exception.StackTrace);
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <WrapLoginCallback>c__AnonStorey4F
        {
            internal AssetStoreClient.DoneLoginCallback callback;

            internal void <>m__8D(AsyncHTTPClient job)
            {
                string text = job.text;
                if (!job.IsSuccess())
                {
                    AssetStoreClient.sLoginState = AssetStoreClient.LoginState.LOGIN_ERROR;
                    AssetStoreClient.sLoginErrorMessage = ((job.responseCode < 200) || (job.responseCode >= 300)) ? "Failed to login - please retry" : text;
                }
                else if (text.StartsWith("<!DOCTYPE"))
                {
                    AssetStoreClient.sLoginState = AssetStoreClient.LoginState.LOGIN_ERROR;
                    AssetStoreClient.sLoginErrorMessage = "Failed to login";
                }
                else
                {
                    AssetStoreClient.sLoginState = AssetStoreClient.LoginState.LOGGED_IN;
                    if (text.Contains("@"))
                    {
                        AssetStoreClient.ActiveSessionID = AssetStoreClient.SavedSessionID;
                    }
                    else
                    {
                        AssetStoreClient.ActiveSessionID = text;
                    }
                    if (AssetStoreClient.RememberSession)
                    {
                        AssetStoreClient.SavedSessionID = AssetStoreClient.ActiveSessionID;
                    }
                }
                this.callback(AssetStoreClient.sLoginErrorMessage);
            }
        }

        public delegate void DoneCallback(AssetStoreResponse response);

        public delegate void DoneLoginCallback(string errorMessage);

        internal enum LoginState
        {
            LOGGED_OUT,
            IN_PROGRESS,
            LOGGED_IN,
            LOGIN_ERROR
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SearchCount
        {
            public string name;
            public int offset;
            public int limit;
        }
    }
}

