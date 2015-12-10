using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

internal class PostProcessWebPlayer
{
    private static Dictionary<BuildOptions, string> optionNames;

    static PostProcessWebPlayer()
    {
        Dictionary<BuildOptions, string> dictionary = new Dictionary<BuildOptions, string>();
        dictionary.Add(BuildOptions.AllowDebugging, "enableDebugging");
        optionNames = dictionary;
    }

    private static string GeneratePlayerParamsString(BuildOptions options)
    {
        <GeneratePlayerParamsString>c__AnonStorey69 storey = new <GeneratePlayerParamsString>c__AnonStorey69 {
            options = options
        };
        return string.Format("{{ {0} }}", string.Join(",", optionNames.Select<KeyValuePair<BuildOptions, string>, string>(new Func<KeyValuePair<BuildOptions, string>, string>(storey.<>m__E7)).ToArray<string>()));
    }

    public static void PostProcess(BuildOptions options, string installPath, string downloadWebplayerUrl, int width, int height)
    {
        string str = FileUtil.UnityGetFileName(installPath);
        string str2 = installPath;
        string path = "Temp/BuildingWebplayerTemplate";
        FileUtil.DeleteFileOrDirectory(path);
        if ((PlayerSettings.webPlayerTemplate == null) || !PlayerSettings.webPlayerTemplate.Contains(":"))
        {
            Debug.LogError("Invalid WebPlayer template selection! Select a template in player settings.");
        }
        else
        {
            string dataPath;
            char[] separator = new char[] { ':' };
            string[] strArray = PlayerSettings.webPlayerTemplate.Split(separator);
            if (strArray[0].Equals("PROJECT"))
            {
                dataPath = Application.dataPath;
            }
            else
            {
                dataPath = Path.Combine(EditorApplication.applicationContentsPath, "Resources");
            }
            dataPath = Path.Combine(Path.Combine(dataPath, "WebPlayerTemplates"), strArray[1]);
            if (!Directory.Exists(dataPath))
            {
                Debug.LogError("Invalid WebPlayer template path! Select a template in player settings.");
            }
            else if (Directory.GetFiles(dataPath, "index.*").Length < 1)
            {
                Debug.LogError("Invalid WebPlayer template selection! Select a template in player settings.");
            }
            else
            {
                FileUtil.CopyDirectoryRecursive(dataPath, path);
                string str5 = Directory.GetFiles(path, "index.*")[0];
                string to = Path.Combine(path, str + Path.GetExtension(str5));
                FileUtil.MoveFileOrDirectory(str5, to);
                string[] files = Directory.GetFiles(path, "thumbnail.*");
                if (files.Length > 0)
                {
                    FileUtil.DeleteFileOrDirectory(files[0]);
                }
                bool flag = (options & BuildOptions.WebPlayerOfflineDeployment) != BuildOptions.CompressTextures;
                string str8 = !flag ? (downloadWebplayerUrl + "/3.0/uo/UnityObject2.js") : "UnityObject2.js";
                string str9 = string.Format("<script type='text/javascript' src='{0}'></script>", !flag ? "https://ssl-webplayer.unity3d.com/download_webplayer-3.x/3.0/uo/jquery.min.js" : "jquery.min.js");
                List<string> list = new List<string> {
                    "%UNITY_UNITYOBJECT_DEPENDENCIES%",
                    str9,
                    "%UNITY_UNITYOBJECT_URL%",
                    str8,
                    "%UNITY_WIDTH%",
                    width.ToString(),
                    "%UNITY_HEIGHT%",
                    height.ToString(),
                    "%UNITY_PLAYER_PARAMS%",
                    GeneratePlayerParamsString(options),
                    "%UNITY_WEB_NAME%",
                    PlayerSettings.productName,
                    "%UNITY_WEB_PATH%",
                    str + ".unity3d"
                };
                if (InternalEditorUtility.IsUnityBeta())
                {
                    list.Add("%UNITY_BETA_WARNING%");
                    list.Add("\r\n\t\t<p style=\"color: #c00; font-size: small; font-style: italic;\">Built with beta version of Unity. Will only work on your computer!</p>");
                    list.Add("%UNITY_SET_BASE_DOWNLOAD_URL%");
                    list.Add(",baseDownloadUrl: \"" + downloadWebplayerUrl + "/\"");
                }
                else
                {
                    list.Add("%UNITY_BETA_WARNING%");
                    list.Add(string.Empty);
                    list.Add("%UNITY_SET_BASE_DOWNLOAD_URL%");
                    list.Add(string.Empty);
                }
                foreach (string str10 in PlayerSettings.templateCustomKeys)
                {
                    list.Add("%UNITY_CUSTOM_" + str10.ToUpper() + "%");
                    list.Add(PlayerSettings.GetTemplateCustomValue(str10));
                }
                FileUtil.ReplaceText(to, list.ToArray());
                if (flag)
                {
                    string str11 = Path.Combine(path, "UnityObject2.js");
                    FileUtil.DeleteFileOrDirectory(str11);
                    FileUtil.UnityFileCopy(EditorApplication.applicationContentsPath + "/Resources/UnityObject2.js", str11);
                    str11 = Path.Combine(path, "jquery.min.js");
                    FileUtil.DeleteFileOrDirectory(str11);
                    FileUtil.UnityFileCopy(EditorApplication.applicationContentsPath + "/Resources/jquery.min.js", str11);
                }
                FileUtil.CopyDirectoryRecursive(path, installPath, true);
                string str12 = Path.Combine(str2, str + ".unity3d");
                FileUtil.DeleteFileOrDirectory(str12);
                FileUtil.MoveFileOrDirectory("Temp/unitystream.unity3d", str12);
                if (Directory.Exists("Assets/StreamingAssets"))
                {
                    FileUtil.CopyDirectoryRecursiveForPostprocess("Assets/StreamingAssets", Path.Combine(str2, "StreamingAssets"), true);
                }
            }
        }
    }

    [CompilerGenerated]
    private sealed class <GeneratePlayerParamsString>c__AnonStorey69
    {
        internal BuildOptions options;

        internal string <>m__E7(KeyValuePair<BuildOptions, string> pair)
        {
            return string.Format("{0}:\"{1}\"", pair.Value, (((BuildOptions) pair.Key) != (this.options & ((BuildOptions) pair.Key))) ? 0 : 1);
        }
    }
}

