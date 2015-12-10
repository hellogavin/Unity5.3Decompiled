namespace UnityEditor.RestService
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEditorInternal;
    using UnityEngine;

    internal class PairingRestHandler : Handler
    {
        protected override JSONValue HandlePost(Request request, JSONValue payload)
        {
            ScriptEditorSettings.ServerURL = payload["url"].AsString();
            ScriptEditorSettings.Name = !payload.ContainsKey("name") ? null : payload["name"].AsString();
            ScriptEditorSettings.ProcessId = !payload.ContainsKey("processid") ? -1 : ((int) payload["processid"].AsFloat());
            object[] objArray1 = new object[6];
            objArray1[0] = "[Pair] Name: ";
            if (ScriptEditorSettings.Name == null)
            {
            }
            objArray1[1] = "<null>";
            objArray1[2] = " ServerURL ";
            objArray1[3] = ScriptEditorSettings.ServerURL;
            objArray1[4] = " Process id: ";
            objArray1[5] = ScriptEditorSettings.ProcessId;
            Logger.Log(string.Concat(objArray1));
            JSONValue value2 = new JSONValue();
            value2["unityprocessid"] = Process.GetCurrentProcess().Id;
            value2["unityproject"] = Application.dataPath;
            return value2;
        }

        private static bool IsScriptEditorRunning()
        {
            if (ScriptEditorSettings.ProcessId < 0)
            {
                return false;
            }
            try
            {
                return !Process.GetProcessById(ScriptEditorSettings.ProcessId).HasExited;
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return false;
            }
        }

        [OnOpenAsset]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            if (ScriptEditorSettings.ServerURL != null)
            {
                string str = Path.GetFullPath(Application.dataPath + "/../" + AssetDatabase.GetAssetPath(instanceID)).Replace('\\', '/');
                string str2 = str.ToLower();
                if ((!str2.EndsWith(".cs") && !str2.EndsWith(".js")) && !str2.EndsWith(".boo"))
                {
                    return false;
                }
                if (IsScriptEditorRunning())
                {
                    object[] objArray1 = new object[] { "{ \"file\" : \"", str, "\", \"line\" : ", line, " }" };
                    if (RestRequest.Send("/openfile", string.Concat(objArray1), 0x1388))
                    {
                        return true;
                    }
                }
                ScriptEditorSettings.ServerURL = null;
                ScriptEditorSettings.Name = null;
                ScriptEditorSettings.ProcessId = -1;
            }
            return false;
        }

        internal static void Register()
        {
            Router.RegisterHandler("/unity/pair", new PairingRestHandler());
        }
    }
}

