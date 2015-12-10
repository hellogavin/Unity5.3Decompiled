namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using UnityEditor.Web;
    using UnityEditorInternal;
    using UnityEngine;

    [InitializeOnLoad]
    internal sealed class AssetStoreContext
    {
        internal bool docked;
        internal string initialOpenURL;
        private static Regex s_GeneratedIDRegExp = new Regex(@"^\{(.*)\}$");
        private static AssetStoreContext s_Instance;
        private static Regex s_InvalidPathCharsRegExp = new Regex("[^a-zA-Z0-9() _-]");
        private static Regex s_StandardPackageRegExp = new Regex(@"/Standard Packages/(Character\ Controller|Glass\ Refraction\ \(Pro\ Only\)|Image\ Effects\ \(Pro\ Only\)|Light\ Cookies|Light\ Flares|Particles|Physic\ Materials|Projectors|Scripts|Standard\ Assets\ \(Mobile\)|Skyboxes|Terrain\ Assets|Toon\ Shading|Tree\ Creator|Water\ \(Basic\)|Water\ \(Pro\ Only\))\.unitypackage$", RegexOptions.IgnoreCase);

        static AssetStoreContext()
        {
            GetInstance();
        }

        public void DeleteKey(string key)
        {
            EditorPrefs.DeleteKey(key);
        }

        public void Download(Package package, DownloadInfo downloadInfo)
        {
            Download(downloadInfo.id, downloadInfo.url, downloadInfo.key, package.title, package.publisher.label, package.category.label, null);
        }

        public static void Download(string package_id, string url, string key, string package_name, string publisher_name, string category_name, AssetStoreUtils.DownloadDoneCallback doneCallback)
        {
            string[] destination = PackageStorePath(publisher_name, category_name, package_name, package_id, url);
            JSONValue value2 = JSONParser.SimpleParse(AssetStoreUtils.CheckDownload(package_id, url, destination, key));
            if (value2.Get("in_progress").AsBool(true))
            {
                Debug.Log("Will not download " + package_name + ". Download is already in progress.");
            }
            else
            {
                string str = value2.Get("download.url").AsString(true);
                string str2 = value2.Get("download.key").AsString(true);
                bool resumeOK = (str == url) && (str2 == key);
                JSONValue value3 = new JSONValue();
                value3["url"] = url;
                value3["key"] = key;
                JSONValue value4 = new JSONValue();
                value4["download"] = value3;
                AssetStoreUtils.Download(package_id, url, destination, key, value4.ToString(), resumeOK, doneCallback);
            }
        }

        public string GetAuthToken()
        {
            return InternalEditorUtility.GetAuthToken();
        }

        public bool GetDockedStatus()
        {
            return this.docked;
        }

        public float GetFloat(string key)
        {
            return EditorPrefs.GetFloat(key);
        }

        public string GetInitialOpenURL()
        {
            if (this.initialOpenURL != null)
            {
                string initialOpenURL = this.initialOpenURL;
                this.initialOpenURL = null;
                return initialOpenURL;
            }
            return string.Empty;
        }

        public static AssetStoreContext GetInstance()
        {
            if (s_Instance == null)
            {
                s_Instance = new AssetStoreContext();
                JSProxyMgr.GetInstance().AddGlobalObject("AssetStoreContext", s_Instance);
            }
            return s_Instance;
        }

        public int GetInt(string key)
        {
            return EditorPrefs.GetInt(key);
        }

        public int[] GetLicenseFlags()
        {
            return InternalEditorUtility.GetLicenseFlags();
        }

        public PackageList GetPackageList()
        {
            Dictionary<string, Package> dictionary = new Dictionary<string, Package>();
            foreach (PackageInfo info in PackageInfo.GetPackageList())
            {
                Package package = new Package();
                if (info.jsonInfo == string.Empty)
                {
                    package.title = Path.GetFileNameWithoutExtension(info.packagePath);
                    package.id = info.packagePath;
                    if (this.IsBuiltinStandardAsset(info.packagePath))
                    {
                        LabelAndId id = new LabelAndId {
                            label = "Unity Technologies",
                            id = "1"
                        };
                        package.publisher = id;
                        id = new LabelAndId {
                            label = "Prefab Packages",
                            id = "4"
                        };
                        package.category = id;
                        package.version = "3.5.0.0";
                    }
                }
                else
                {
                    JSONValue json = JSONParser.SimpleParse(info.jsonInfo);
                    if (json.IsNull())
                    {
                        continue;
                    }
                    package.Initialize(json);
                    if (package.id == null)
                    {
                        JSONValue value3 = json.Get("link.id");
                        if (!value3.IsNull())
                        {
                            package.id = value3.AsString();
                        }
                        else
                        {
                            package.id = info.packagePath;
                        }
                    }
                }
                package.local_icon = info.iconURL;
                package.local_path = info.packagePath;
                if (((!dictionary.ContainsKey(package.id) || (dictionary[package.id].version_id == null)) || (dictionary[package.id].version_id == "-1")) || (((package.version_id != null) && (package.version_id != "-1")) && (int.Parse(dictionary[package.id].version_id) <= int.Parse(package.version_id))))
                {
                    dictionary[package.id] = package;
                }
            }
            Package[] packageArray = dictionary.Values.ToArray<Package>();
            return new PackageList { results = packageArray };
        }

        public int GetSkinIndex()
        {
            return EditorGUIUtility.skinIndex;
        }

        public string GetString(string key)
        {
            return EditorPrefs.GetString(key);
        }

        public bool HasKey(string key)
        {
            return EditorPrefs.HasKey(key);
        }

        private bool IsBuiltinStandardAsset(string path)
        {
            return s_StandardPackageRegExp.IsMatch(path);
        }

        public void OpenBrowser(string url)
        {
            Application.OpenURL(url);
        }

        public bool OpenPackage(string id)
        {
            return this.OpenPackage(id, "default");
        }

        public bool OpenPackage(string id, string action)
        {
            return OpenPackageInternal(id);
        }

        public static bool OpenPackageInternal(string id)
        {
            Match match = s_GeneratedIDRegExp.Match(id);
            if (match.Success && File.Exists(match.Groups[1].Value))
            {
                AssetDatabase.ImportPackage(match.Groups[1].Value, true);
                return true;
            }
            foreach (PackageInfo info in PackageInfo.GetPackageList())
            {
                if (info.jsonInfo != string.Empty)
                {
                    JSONValue value2 = JSONParser.SimpleParse(info.jsonInfo);
                    string str = !value2.Get("id").IsNull() ? value2["id"].AsString(true) : null;
                    if (((str != null) && (str == id)) && File.Exists(info.packagePath))
                    {
                        AssetDatabase.ImportPackage(info.packagePath, true);
                        return true;
                    }
                }
            }
            Debug.LogError("Unknown package ID " + id);
            return false;
        }

        public static string[] PackageStorePath(string publisher_name, string category_name, string package_name, string package_id, string url)
        {
            string[] strArray = new string[] { publisher_name, category_name, package_name };
            for (int i = 0; i < 3; i++)
            {
                strArray[i] = s_InvalidPathCharsRegExp.Replace(strArray[i], string.Empty);
            }
            if (strArray[2] == string.Empty)
            {
                strArray[2] = s_InvalidPathCharsRegExp.Replace(package_id, string.Empty);
            }
            if (strArray[2] == string.Empty)
            {
                strArray[2] = s_InvalidPathCharsRegExp.Replace(url, string.Empty);
            }
            return strArray;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string SessionGetString(string key);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool SessionHasString(string key);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SessionRemoveString(string key);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SessionSetString(string key, string value);
        public void SetFloat(string key, float value)
        {
            EditorPrefs.SetFloat(key, value);
        }

        public void SetInt(string key, int value)
        {
            EditorPrefs.SetInt(key, value);
        }

        public void SetString(string key, string value)
        {
            EditorPrefs.SetString(key, value);
        }

        public class DownloadInfo
        {
            public string id;
            public string key;
            public string url;
        }

        public class LabelAndId
        {
            public string id;
            public string label;

            public void Initialize(JSONValue json)
            {
                if (json.ContainsKey("label"))
                {
                    this.label = json["label"].AsString();
                }
                if (json.ContainsKey("id"))
                {
                    this.id = json["id"].AsString();
                }
            }

            public override string ToString()
            {
                return string.Format("{{label={0}, id={1}}}", this.label, this.id);
            }
        }

        public class Link
        {
            public string id;
            public string type;

            public void Initialize(JSONValue json)
            {
                if (json.ContainsKey("type"))
                {
                    this.type = json["type"].AsString();
                }
                if (json.ContainsKey("id"))
                {
                    this.id = json["id"].AsString();
                }
            }

            public override string ToString()
            {
                return string.Format("{{type={0}, id={1}}}", this.type, this.id);
            }
        }

        public class Package
        {
            public AssetStoreContext.LabelAndId category;
            public string description;
            public string id;
            public AssetStoreContext.Link link;
            public string local_icon;
            public string local_path;
            public string pubdate;
            public AssetStoreContext.LabelAndId publisher;
            public string title;
            public string version;
            public string version_id;

            public void Initialize(JSONValue json)
            {
                if (json.ContainsKey("title"))
                {
                    this.title = json["title"].AsString();
                }
                if (json.ContainsKey("id"))
                {
                    this.id = json["id"].AsString();
                }
                if (json.ContainsKey("version"))
                {
                    this.version = json["version"].AsString();
                }
                if (json.ContainsKey("version_id"))
                {
                    this.version_id = json["version_id"].AsString();
                }
                if (json.ContainsKey("local_icon"))
                {
                    this.local_icon = json["local_icon"].AsString();
                }
                if (json.ContainsKey("local_path"))
                {
                    this.local_path = json["local_path"].AsString();
                }
                if (json.ContainsKey("pubdate"))
                {
                    this.pubdate = json["pubdate"].AsString();
                }
                if (json.ContainsKey("description"))
                {
                    this.description = json["description"].AsString();
                }
                if (json.ContainsKey("publisher"))
                {
                    this.publisher = new AssetStoreContext.LabelAndId();
                    this.publisher.Initialize(json["publisher"]);
                }
                if (json.ContainsKey("category"))
                {
                    this.category = new AssetStoreContext.LabelAndId();
                    this.category.Initialize(json["category"]);
                }
                if (json.ContainsKey("link"))
                {
                    this.link = new AssetStoreContext.Link();
                    this.link.Initialize(json["link"]);
                }
            }

            public override string ToString()
            {
                object[] args = new object[] { this.title, this.id, this.publisher, this.category, this.version, this.version_id, this.local_icon, this.local_path, this.pubdate, this.description, this.link };
                return string.Format("{{title={0}, id={1}, publisher={2}, category={3}, pubdate={8}, version={4}, version_id={5}, description={9}, link={10}, local_icon={6}, local_path={7}}}", args);
            }
        }

        public class PackageList
        {
            public AssetStoreContext.Package[] results;
        }
    }
}

