namespace UnityEditor
{
    using System;
    using System.Collections;
    using UnityEditorInternal;
    using UnityEngine;

    [Serializable]
    internal class ASServerAdminWindow
    {
        private static ASMainWindow.Constants constants;
        private Action currAction;
        private MaintDatabaseRecord[] databases;
        private bool isConnected;
        private const int listLenghts = 20;
        private ListViewState lv = new ListViewState(0);
        private ListViewState lv2 = new ListViewState(0);
        private SplitterState lvSplit = new SplitterState(new float[] { 5f, 50f, 50f, 150f }, new int[] { 20, 70, 70, 100 }, null);
        private string nEmail = string.Empty;
        private string nFullName = string.Empty;
        private string nPassword1 = string.Empty;
        private string nPassword2 = string.Empty;
        private string nProjectName = string.Empty;
        private string nTemplateProjectName = string.Empty;
        private string nUserName = string.Empty;
        private ASMainWindow parentWin;
        private string password = string.Empty;
        private bool projectSelected;
        private bool resetKeyboardControl;
        private string server = string.Empty;
        private string[] servers;
        private bool splittersOk;
        private string user = string.Empty;
        private MaintUserRecord[] users;
        private bool userSelected;

        public ASServerAdminWindow(ASMainWindow parentWin)
        {
            this.parentWin = parentWin;
            this.servers = InternalEditorUtility.GetEditorSettingsList("ASServer", 20);
            this.server = EditorPrefs.GetString("ASAdminServer");
            this.user = "admin";
        }

        private void ActionBox()
        {
            bool enabled = GUI.enabled;
            switch (this.currAction)
            {
                case Action.Main:
                    if (!this.isConnected)
                    {
                        GUI.enabled = false;
                    }
                    if (this.WordWrappedLabelButton("Want to create a new project?", "Create"))
                    {
                        this.nProjectName = string.Empty;
                        this.nTemplateProjectName = string.Empty;
                        this.currAction = Action.CreateProject;
                    }
                    if (this.WordWrappedLabelButton("Want to create a new user?", "New User"))
                    {
                        this.nPassword1 = this.nPassword2 = string.Empty;
                        this.nFullName = this.nUserName = this.nEmail = string.Empty;
                        this.currAction = Action.CreateUser;
                    }
                    GUI.enabled = (this.isConnected && this.userSelected) && enabled;
                    if (this.WordWrappedLabelButton("Need to change user password?", "Set Password"))
                    {
                        this.nPassword1 = this.nPassword2 = string.Empty;
                        this.currAction = Action.SetPassword;
                    }
                    if (this.WordWrappedLabelButton("Need to change user information?", "Edit"))
                    {
                        this.nFullName = this.users[this.lv2.row].fullName;
                        this.nEmail = this.users[this.lv2.row].email;
                        this.currAction = Action.ModifyUser;
                    }
                    GUI.enabled = (this.isConnected && this.projectSelected) && enabled;
                    if (this.WordWrappedLabelButton("Duplicate selected project", "Copy Project"))
                    {
                        this.nProjectName = string.Empty;
                        this.nTemplateProjectName = this.databases[this.lv.row].name;
                        this.currAction = Action.CreateProject;
                    }
                    if ((this.WordWrappedLabelButton("Delete selected project", "Delete Project") && EditorUtility.DisplayDialog("Delete project", "Are you sure you want to delete project " + this.databases[this.lv.row].name + "? This operation cannot be undone!", "Delete", "Cancel")) && (AssetServer.AdminDeleteDB(this.databases[this.lv.row].name) != 0))
                    {
                        this.DoRefreshDatabases();
                        GUIUtility.ExitGUI();
                    }
                    GUI.enabled = (this.isConnected && this.userSelected) && enabled;
                    if ((this.WordWrappedLabelButton("Delete selected user", "Delete User") && EditorUtility.DisplayDialog("Delete user", "Are you sure you want to delete user " + this.users[this.lv2.row].userName + "? This operation cannot be undone!", "Delete", "Cancel")) && (AssetServer.AdminDeleteUser(this.users[this.lv2.row].userName) != 0))
                    {
                        if (this.lv.row > -1)
                        {
                            this.DoGetUsers();
                        }
                        GUIUtility.ExitGUI();
                    }
                    GUI.enabled = enabled;
                    break;

                case Action.CreateUser:
                    this.nFullName = EditorGUILayout.TextField("Full Name:", this.nFullName, new GUILayoutOption[0]);
                    this.nEmail = EditorGUILayout.TextField("Email Address:", this.nEmail, new GUILayoutOption[0]);
                    GUILayout.Space(5f);
                    this.nUserName = EditorGUILayout.TextField("User Name:", this.nUserName, new GUILayoutOption[0]);
                    GUILayout.Space(5f);
                    this.nPassword1 = EditorGUILayout.PasswordField("Password:", this.nPassword1, new GUILayoutOption[0]);
                    this.nPassword2 = EditorGUILayout.PasswordField("Repeat Password:", this.nPassword2, new GUILayoutOption[0]);
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.FlexibleSpace();
                    GUI.enabled = this.CanPerformCurrentAction() && enabled;
                    if (GUILayout.Button("Create User", constants.smallButton, new GUILayoutOption[0]))
                    {
                        this.PerformCurrentAction();
                    }
                    GUI.enabled = enabled;
                    if (GUILayout.Button("Cancel", constants.smallButton, new GUILayoutOption[0]))
                    {
                        this.currAction = Action.Main;
                    }
                    GUILayout.EndHorizontal();
                    break;

                case Action.SetPassword:
                    GUILayout.Label("Setting password for user: " + this.users[this.lv2.row].userName, constants.title, new GUILayoutOption[0]);
                    GUILayout.Space(5f);
                    this.nPassword1 = EditorGUILayout.PasswordField("Password:", this.nPassword1, new GUILayoutOption[0]);
                    this.nPassword2 = EditorGUILayout.PasswordField("Repeat Password:", this.nPassword2, new GUILayoutOption[0]);
                    GUILayout.Space(5f);
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.FlexibleSpace();
                    GUI.enabled = this.CanPerformCurrentAction() && enabled;
                    if (GUILayout.Button("Change Password", constants.smallButton, new GUILayoutOption[0]))
                    {
                        this.PerformCurrentAction();
                    }
                    GUI.enabled = enabled;
                    if (GUILayout.Button("Cancel", constants.smallButton, new GUILayoutOption[0]))
                    {
                        this.currAction = Action.Main;
                    }
                    GUILayout.EndHorizontal();
                    break;

                case Action.CreateProject:
                    this.nProjectName = EditorGUILayout.TextField("Project Name:", this.nProjectName, new GUILayoutOption[0]);
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.FlexibleSpace();
                    GUI.enabled = this.CanPerformCurrentAction() && enabled;
                    if (GUILayout.Button(!(this.nTemplateProjectName == string.Empty) ? ("Copy " + this.nTemplateProjectName) : "Create Project", constants.smallButton, new GUILayoutOption[0]))
                    {
                        this.PerformCurrentAction();
                    }
                    GUI.enabled = enabled;
                    if (GUILayout.Button("Cancel", constants.smallButton, new GUILayoutOption[0]))
                    {
                        this.currAction = Action.Main;
                    }
                    GUILayout.EndHorizontal();
                    break;

                case Action.ModifyUser:
                    this.nFullName = EditorGUILayout.TextField("Full Name:", this.nFullName, new GUILayoutOption[0]);
                    this.nEmail = EditorGUILayout.TextField("Email Address:", this.nEmail, new GUILayoutOption[0]);
                    GUILayout.Space(5f);
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.FlexibleSpace();
                    GUI.enabled = this.CanPerformCurrentAction() && enabled;
                    if (GUILayout.Button("Change", constants.smallButton, new GUILayoutOption[0]))
                    {
                        this.PerformCurrentAction();
                    }
                    GUI.enabled = enabled;
                    if (GUILayout.Button("Cancel", constants.smallButton, new GUILayoutOption[0]))
                    {
                        this.currAction = Action.Main;
                    }
                    GUILayout.EndHorizontal();
                    break;
            }
        }

        private bool CanPerformCurrentAction()
        {
            bool flag;
            switch (this.currAction)
            {
                case Action.Main:
                    return ((this.server != string.Empty) && (this.user != string.Empty));

                case Action.CreateUser:
                    flag = true;
                    for (int i = 0; i < this.nUserName.Length; i++)
                    {
                        char ch = this.nUserName[i];
                        if ((((ch < 'a') || (ch > 'z')) && ((ch < 'A') || (ch > 'Z'))) && (((ch < '0') || (ch > '9')) && ((ch != '-') && (ch != '_'))))
                        {
                            flag = false;
                            break;
                        }
                    }
                    break;

                case Action.SetPassword:
                    return ((this.nPassword1 != string.Empty) && (this.nPassword1 == this.nPassword2));

                case Action.CreateProject:
                    return (this.nProjectName != string.Empty);

                case Action.ModifyUser:
                    return (this.nFullName != string.Empty);

                default:
                    return false;
            }
            return ((((this.nFullName != string.Empty) && (this.nUserName != string.Empty)) && ((this.nPassword1 != string.Empty) && (this.nPassword1 == this.nPassword2))) && flag);
        }

        private void DoConnect()
        {
            string server;
            EditorPrefs.SetString("ASAdminServer", this.server);
            this.userSelected = false;
            this.isConnected = false;
            this.projectSelected = false;
            this.lv.row = -1;
            this.lv2.row = -1;
            this.lv.totalRows = 0;
            this.lv2.totalRows = 0;
            int result = 0x29ed;
            if (this.server.IndexOf(":") > 0)
            {
                int.TryParse(this.server.Substring(this.server.IndexOf(":") + 1), out result);
                server = this.server.Substring(0, this.server.IndexOf(":"));
            }
            else
            {
                server = this.server;
            }
            AssetServer.AdminSetCredentials(server, result, this.user, this.password);
            this.DoRefreshDatabases();
        }

        private void DoGetUsers()
        {
            MaintUserRecord[] recordArray = AssetServer.AdminGetUsers(this.databases[this.lv.row].dbName);
            if (recordArray != null)
            {
                this.users = recordArray;
            }
            else
            {
                this.users = new MaintUserRecord[0];
            }
            this.lv2.totalRows = this.users.Length;
            this.lv2.row = -1;
        }

        public bool DoGUI()
        {
            bool enabled = GUI.enabled;
            if (constants == null)
            {
                constants = new ASMainWindow.Constants();
                constants.toggleSize = constants.toggle.CalcSize(new GUIContent("X"));
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
            Event current = Event.current;
            if (((current.type == EventType.KeyDown) && (current.keyCode == KeyCode.Return)) && this.CanPerformCurrentAction())
            {
                this.PerformCurrentAction();
            }
            if (((current.type == EventType.KeyDown) && (current.keyCode == KeyCode.Escape)) && (this.currAction != Action.Main))
            {
                this.currAction = Action.Main;
                current.Use();
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            this.server = EditorGUILayout.TextField("Server Address:", this.server, new GUILayoutOption[0]);
            this.ServersPopup();
            GUILayout.EndHorizontal();
            this.user = EditorGUILayout.TextField("User Name:", this.user, new GUILayoutOption[0]);
            this.password = EditorGUILayout.PasswordField("Password:", this.password, new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUI.enabled = this.CanPerformCurrentAction() && enabled;
            if (GUILayout.Button("Connect", constants.smallButton, new GUILayoutOption[0]))
            {
                this.PerformCurrentAction();
            }
            GUI.enabled = enabled;
            GUILayout.EndHorizontal();
            if (AssetServer.GetAssetServerError() != string.Empty)
            {
                GUILayout.Label(AssetServer.GetAssetServerError(), constants.errorLabel, new GUILayoutOption[0]);
            }
            GUILayout.EndVertical();
            GUILayout.EndVertical();
            GUILayout.BeginVertical(constants.groupBox, new GUILayoutOption[0]);
            GUILayout.Box("Admin Actions", constants.title, new GUILayoutOption[0]);
            GUILayout.BeginVertical(constants.contentBox, new GUILayoutOption[0]);
            this.ActionBox();
            GUILayout.EndVertical();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.BeginVertical(constants.groupBox, new GUILayoutOption[0]);
            GUILayout.Box("Project", constants.title, new GUILayoutOption[0]);
            IEnumerator enumerator = ListViewGUILayout.ListView(this.lv, constants.background, new GUILayoutOption[0]).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ListViewElement element = (ListViewElement) enumerator.Current;
                    if ((element.row == this.lv.row) && (Event.current.type == EventType.Repaint))
                    {
                        constants.entrySelected.Draw(element.position, false, false, false, false);
                    }
                    GUILayout.Label(this.databases[element.row].name, new GUILayoutOption[0]);
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
            if (this.lv.selectionChanged)
            {
                if (this.lv.row > -1)
                {
                    this.projectSelected = true;
                }
                this.currAction = Action.Main;
                this.DoGetUsers();
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(constants.groupBox, new GUILayoutOption[0]);
            SplitterGUILayout.BeginHorizontalSplit(this.lvSplit, new GUILayoutOption[0]);
            GUILayout.Box(string.Empty, constants.columnHeader, new GUILayoutOption[0]);
            GUILayout.Box("User", constants.columnHeader, new GUILayoutOption[0]);
            GUILayout.Box("Full Name", constants.columnHeader, new GUILayoutOption[0]);
            GUILayout.Box("Email", constants.columnHeader, new GUILayoutOption[0]);
            SplitterGUILayout.EndHorizontalSplit();
            int left = EditorStyles.label.margin.left;
            IEnumerator enumerator2 = ListViewGUILayout.ListView(this.lv2, constants.background, new GUILayoutOption[0]).GetEnumerator();
            try
            {
                while (enumerator2.MoveNext())
                {
                    ListViewElement element2 = (ListViewElement) enumerator2.Current;
                    if ((element2.row == this.lv2.row) && (Event.current.type == EventType.Repaint))
                    {
                        constants.entrySelected.Draw(element2.position, false, false, false, false);
                    }
                    bool flag2 = this.users[element2.row].enabled != 0;
                    bool flag3 = GUI.Toggle(new Rect(element2.position.x + 2f, element2.position.y - 1f, constants.toggleSize.x, constants.toggleSize.y), flag2, string.Empty);
                    GUILayout.Space(constants.toggleSize.x);
                    if ((flag2 != flag3) && AssetServer.AdminSetUserEnabled(this.databases[this.lv.row].dbName, this.users[element2.row].userName, this.users[element2.row].fullName, this.users[element2.row].email, !flag3 ? 0 : 1))
                    {
                        this.users[element2.row].enabled = !flag3 ? 0 : 1;
                    }
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width((float) (this.lvSplit.realSizes[1] - left)) };
                    GUILayout.Label(this.users[element2.row].userName, options);
                    GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width((float) (this.lvSplit.realSizes[2] - left)) };
                    GUILayout.Label(this.users[element2.row].fullName, optionArray2);
                    GUILayout.Label(this.users[element2.row].email, new GUILayoutOption[0]);
                }
            }
            finally
            {
                IDisposable disposable2 = enumerator2 as IDisposable;
                if (disposable2 == null)
                {
                }
                disposable2.Dispose();
            }
            if (this.lv2.selectionChanged)
            {
                if (this.lv2.row > -1)
                {
                    this.userSelected = true;
                }
                if (this.currAction == Action.SetPassword)
                {
                    this.currAction = Action.Main;
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);
            if (!this.splittersOk && (Event.current.type == EventType.Repaint))
            {
                this.splittersOk = true;
                this.parentWin.Repaint();
            }
            return true;
        }

        private void DoRefreshDatabases()
        {
            MaintDatabaseRecord[] recordArray = AssetServer.AdminRefreshDatabases();
            if (recordArray != null)
            {
                this.databases = recordArray;
                this.isConnected = true;
            }
            else
            {
                this.databases = new MaintDatabaseRecord[0];
                this.lv2.totalRows = 0;
            }
            this.lv.row = -1;
            this.lv.totalRows = this.databases.Length;
            this.lv2.totalRows = 0;
            this.users = new MaintUserRecord[0];
        }

        private void PerformCurrentAction()
        {
            switch (this.currAction)
            {
                case Action.Main:
                    this.currAction = Action.Main;
                    this.DoConnect();
                    Event.current.Use();
                    return;

                case Action.CreateUser:
                    AssetServer.AdminCreateUser(this.nUserName, this.nFullName, this.nEmail, this.nPassword1);
                    this.currAction = Action.Main;
                    if (this.lv.row > -1)
                    {
                        this.DoGetUsers();
                    }
                    Event.current.Use();
                    return;

                case Action.SetPassword:
                    AssetServer.AdminChangePassword(this.users[this.lv2.row].userName, this.nPassword1);
                    this.currAction = Action.Main;
                    Event.current.Use();
                    return;

                case Action.CreateProject:
                    if (AssetServer.AdminCreateDB(this.nProjectName, this.nTemplateProjectName) != 0)
                    {
                        this.DoRefreshDatabases();
                        for (int i = 0; i < this.databases.Length; i++)
                        {
                            if (this.databases[i].name == this.nProjectName)
                            {
                                this.lv.row = i;
                                this.DoGetUsers();
                                break;
                            }
                        }
                    }
                    break;

                case Action.ModifyUser:
                    AssetServer.AdminModifyUserInfo(this.databases[this.lv.row].dbName, this.users[this.lv2.row].userName, this.nFullName, this.nEmail);
                    this.currAction = Action.Main;
                    if (this.lv.row > -1)
                    {
                        this.DoGetUsers();
                    }
                    Event.current.Use();
                    return;

                default:
                    return;
            }
            this.currAction = Action.Main;
            Event.current.Use();
        }

        private void ServersPopup()
        {
            if (this.servers.Length > 0)
            {
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth(18f) };
                int index = EditorGUILayout.Popup(-1, this.servers, constants.dropDown, options);
                if (index >= 0)
                {
                    GUIUtility.keyboardControl = 0;
                    GUIUtility.hotControl = 0;
                    this.resetKeyboardControl = true;
                    this.server = this.servers[index];
                    this.parentWin.Repaint();
                }
            }
        }

        private bool WordWrappedLabelButton(string label, string buttonText)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label(label, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(100f) };
            bool flag = GUILayout.Button(buttonText, options);
            GUILayout.EndHorizontal();
            return flag;
        }

        private enum Action
        {
            Main,
            CreateUser,
            SetPassword,
            CreateProject,
            ModifyUser
        }
    }
}

