namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    [Serializable]
    internal class ASConfigWindow
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map10;
        private static ASMainWindow.Constants constants;
        private const int listLenghts = 20;
        private ASMainWindow parentWin;
        private string password = string.Empty;
        private PListConfig plc;
        private string port = string.Empty;
        private string projectName = string.Empty;
        private string[] projectsList;
        private ListViewState projectsLv = new ListViewState(0);
        private bool resetKeyboardControl;
        private string serverAddress = string.Empty;
        private string[] serversList;
        private ListViewState serversLv = new ListViewState(0);
        private string userName = string.Empty;

        public ASConfigWindow(ASMainWindow parent)
        {
            this.parentWin = parent;
            this.LoadConfig();
        }

        private void ClearConfig()
        {
            if (EditorUtility.DisplayDialog("Clear Configuration", "Are you sure you want to disconnect from Asset Server project and clear all configuration values?", "Clear", "Cancel"))
            {
                this.plc = new PListConfig("Library/ServerPreferences.plist");
                this.plc.Clear();
                this.plc.Save();
                this.LoadConfig();
                this.projectsLv.totalRows = 0;
                ASEditorBackend.InitializeMaintBinding();
                this.resetKeyboardControl = true;
            }
        }

        private void DoConfigGUI()
        {
            bool enabled = GUI.enabled;
            bool changed = GUI.changed;
            GUI.changed = false;
            bool flag3 = false;
            bool flag4 = false;
            Event current = Event.current;
            if (current.type == EventType.KeyDown)
            {
                bool flag5;
                if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    flag5 = (((current.character == '\n') || (current.character == '\x0003')) || (current.keyCode == KeyCode.Return)) || (current.keyCode == KeyCode.KeypadEnter);
                }
                else
                {
                    if ((current.keyCode == KeyCode.Return) || (current.keyCode == KeyCode.KeypadEnter))
                    {
                        current.Use();
                    }
                    flag5 = (current.character == '\n') || (current.character == '\x0003');
                }
                if (flag5)
                {
                    string nameOfFocusedControl = GUI.GetNameOfFocusedControl();
                    if (nameOfFocusedControl != null)
                    {
                        int num;
                        if (<>f__switch$map10 == null)
                        {
                            Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
                            dictionary.Add("password", 0);
                            dictionary.Add("project", 1);
                            <>f__switch$map10 = dictionary;
                        }
                        if (<>f__switch$map10.TryGetValue(nameOfFocusedControl, out num))
                        {
                            if (num == 0)
                            {
                                flag3 = true;
                                goto Label_0141;
                            }
                            if (num == 1)
                            {
                                flag4 = true;
                                goto Label_0141;
                            }
                        }
                    }
                    current.Use();
                }
            }
        Label_0141:
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            this.serverAddress = EditorGUILayout.TextField("Server", this.serverAddress, new GUILayoutOption[0]);
            this.ServersPopup();
            GUILayout.EndHorizontal();
            if (GUI.changed)
            {
                this.GetUserAndPassword();
            }
            GUI.changed |= changed;
            this.userName = EditorGUILayout.TextField("User Name", this.userName, new GUILayoutOption[0]);
            GUI.SetNextControlName("password");
            this.password = EditorGUILayout.PasswordField("Password", this.password, new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUI.enabled = (((this.userName != string.Empty) && (this.password != string.Empty)) && (this.serverAddress != string.Empty)) && enabled;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(100f) };
            if (GUILayout.Button("Show Projects", options) || (flag3 && GUI.enabled))
            {
                this.DoShowProjects();
                this.projectName = string.Empty;
                EditorGUI.FocusTextInControl("project");
            }
            bool flag6 = GUI.enabled;
            GUI.enabled = enabled;
            if (GUILayout.Button("Clear Configuration", new GUILayoutOption[0]))
            {
                this.ClearConfig();
            }
            GUI.enabled = flag6;
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
            changed = GUI.changed;
            GUI.changed = false;
            GUI.SetNextControlName("project");
            this.projectName = EditorGUILayout.TextField("Project Name", this.projectName, new GUILayoutOption[0]);
            GUI.changed |= changed;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUI.enabled = (((this.userName != string.Empty) && (this.password != string.Empty)) && ((this.serverAddress != string.Empty) && (this.projectName != string.Empty))) && enabled;
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MinWidth(100f) };
            if (GUILayout.Button("Connect", constants.bigButton, optionArray2) || (flag4 && GUI.enabled))
            {
                this.DoConnect();
            }
            GUI.enabled = enabled;
            GUILayout.EndHorizontal();
        }

        private void DoConnect()
        {
            AssetServer.RemoveMaintErrorsFromConsole();
            int result = 0x29ed;
            string serverAddress = this.serverAddress;
            if (serverAddress.IndexOf(":") > 0)
            {
                int.TryParse(serverAddress.Substring(serverAddress.IndexOf(":") + 1), out result);
                serverAddress = serverAddress.Substring(0, serverAddress.IndexOf(":"));
            }
            this.port = result.ToString();
            string str2 = AssetServer.GetDatabaseName(serverAddress, this.userName, this.password, this.port, this.projectName);
            this.GetDefaultPListConfig();
            this.plc["Maint Server"] = serverAddress;
            this.plc["Maint UserName"] = this.userName;
            this.plc["Maint database name"] = str2;
            this.plc["Maint port number"] = this.port;
            this.plc["Maint project name"] = this.projectName;
            this.plc.Save();
            if (ArrayUtility.Contains<string>(this.serversList, this.serverAddress))
            {
                ArrayUtility.Remove<string>(ref this.serversList, this.serverAddress);
            }
            ArrayUtility.Insert<string>(ref this.serversList, 0, this.serverAddress);
            ASEditorBackend.AddUser(this.serverAddress, this.userName);
            ASEditorBackend.SetPassword(this.serverAddress, this.userName, this.password);
            InternalEditorUtility.SaveEditorSettingsList("ASServer", this.serversList, 20);
            if (str2 != string.Empty)
            {
                ASEditorBackend.InitializeMaintBinding();
                this.parentWin.Reinit();
                GUIUtility.ExitGUI();
            }
            else
            {
                this.parentWin.NeedsSetup = true;
                this.parentWin.Repaint();
            }
        }

        public bool DoGUI()
        {
            if (constants == null)
            {
                constants = new ASMainWindow.Constants();
            }
            if (this.resetKeyboardControl)
            {
                this.resetKeyboardControl = false;
                GUIUtility.keyboardControl = 0;
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.BeginVertical(constants.groupBox, new GUILayoutOption[0]);
            GUILayout.Box("Server Connection", constants.title, new GUILayoutOption[0]);
            GUILayout.BeginVertical(constants.contentBox, new GUILayoutOption[0]);
            this.DoConfigGUI();
            if (AssetServer.GetAssetServerError() != string.Empty)
            {
                GUILayout.Space(10f);
                GUILayout.Label(AssetServer.GetAssetServerError(), constants.errorLabel, new GUILayoutOption[0]);
                GUILayout.Space(10f);
            }
            GUILayout.EndVertical();
            GUILayout.EndVertical();
            this.DoProjectsGUI();
            GUILayout.EndHorizontal();
            return true;
        }

        private void DoProjectsGUI()
        {
            GUILayout.BeginVertical(constants.groupBox, new GUILayoutOption[0]);
            GUILayout.Label("Projects on Server", constants.title, new GUILayoutOption[0]);
            IEnumerator enumerator = ListViewGUILayout.ListView(this.projectsLv, constants.background, new GUILayoutOption[0]).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ListViewElement current = (ListViewElement) enumerator.Current;
                    if ((current.row == this.projectsLv.row) && (Event.current.type == EventType.Repaint))
                    {
                        constants.entrySelected.Draw(current.position, false, false, false, false);
                    }
                    GUILayout.Label(this.projectsList[current.row], constants.element, new GUILayoutOption[0]);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            if (this.projectsLv.selectionChanged)
            {
                this.projectName = this.projectsList[this.projectsLv.row];
            }
            GUILayout.EndVertical();
        }

        private void DoShowProjects()
        {
            int result = 0x29ed;
            string serverAddress = this.serverAddress;
            if (serverAddress.IndexOf(":") > 0)
            {
                int.TryParse(serverAddress.Substring(serverAddress.IndexOf(":") + 1), out result);
                serverAddress = serverAddress.Substring(0, serverAddress.IndexOf(":"));
            }
            AssetServer.AdminSetCredentials(serverAddress, result, this.userName, this.password);
            MaintDatabaseRecord[] recordArray = AssetServer.AdminRefreshDatabases();
            if (recordArray != null)
            {
                this.projectsList = new string[recordArray.Length];
                for (int i = 0; i < recordArray.Length; i++)
                {
                    this.projectsList[i] = recordArray[i].name;
                }
                this.projectsLv.totalRows = recordArray.Length;
                this.GetDefaultPListConfig();
                this.plc["Maint Server"] = serverAddress;
                this.plc["Maint UserName"] = this.userName;
                this.plc["Maint port number"] = this.port;
                this.plc.Save();
                ASEditorBackend.SetPassword(serverAddress, this.userName, this.password);
                ASEditorBackend.AddUser(this.serverAddress, this.userName);
            }
            else
            {
                this.projectsLv.totalRows = 0;
            }
        }

        private void GetDefaultPListConfig()
        {
            this.plc = new PListConfig("Library/ServerPreferences.plist");
            this.plc["Maint Server"] = string.Empty;
            this.plc["Maint UserName"] = string.Empty;
            this.plc["Maint database name"] = string.Empty;
            this.plc["Maint port number"] = string.Empty;
            this.plc["Maint project name"] = string.Empty;
            this.plc["Maint Password"] = string.Empty;
            if (this.plc["Maint settings type"] == string.Empty)
            {
                this.plc["Maint settings type"] = "manual";
            }
            if (this.plc["Maint Timeout"] == string.Empty)
            {
                this.plc["Maint Timeout"] = "5";
            }
            if (this.plc["Maint Connection Settings"] == string.Empty)
            {
                this.plc["Maint Connection Settings"] = string.Empty;
            }
        }

        private void GetUserAndPassword()
        {
            string user = ASEditorBackend.GetUser(this.serverAddress);
            if (user != string.Empty)
            {
                this.userName = user;
            }
            user = ASEditorBackend.GetPassword(this.serverAddress, user);
            if (user != string.Empty)
            {
                this.password = user;
            }
        }

        private void LoadConfig()
        {
            PListConfig config = new PListConfig("Library/ServerPreferences.plist");
            this.serverAddress = config["Maint Server"];
            this.userName = config["Maint UserName"];
            this.port = config["Maint port number"];
            this.projectName = config["Maint project name"];
            this.password = ASEditorBackend.GetPassword(this.serverAddress, this.userName);
            if (this.port != string.Empty)
            {
                int num = 0x29ed;
                if (this.port != num.ToString())
                {
                    this.serverAddress = this.serverAddress + ":" + this.port;
                }
            }
            this.serversList = InternalEditorUtility.GetEditorSettingsList("ASServer", 20);
            this.serversLv.totalRows = this.serversList.Length;
            if (ArrayUtility.Contains<string>(this.serversList, this.serverAddress))
            {
                this.serversLv.row = ArrayUtility.IndexOf<string>(this.serversList, this.serverAddress);
            }
        }

        private void ServersPopup()
        {
            if (this.serversList.Length > 0)
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth(18f) };
                int index = EditorGUILayout.Popup(-1, this.serversList, constants.dropDown, options);
                if (index >= 0)
                {
                    GUIUtility.keyboardControl = 0;
                    GUIUtility.hotControl = 0;
                    this.resetKeyboardControl = true;
                    this.serverAddress = this.serversList[index];
                    this.parentWin.Repaint();
                }
            }
        }
    }
}

