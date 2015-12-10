namespace UnityEditor
{
    using System;
    using System.Reflection;
    using UnityEngine;

    internal class ASEditorBackend
    {
        public static ASMainWindow asMainWin;
        public const string kConnectionSettings = "Maint Connection Settings";
        public const string kDatabaseName = "Maint database name";
        public const int kDefaultServerPort = 0x29ed;
        public const string kPassword = "Maint Password";
        public const string kPortNumber = "Maint port number";
        public const string kProjectName = "Maint project name";
        public const string kServer = "Maint Server";
        public const string kServerSettingsFile = "Library/ServerPreferences.plist";
        public const string kSettingsType = "Maint settings type";
        public const string kTimeout = "Maint Timeout";
        public const string kUserName = "Maint UserName";
        private static string s_TestingConflictResClass;
        private static string s_TestingConflictResFunction;

        internal static void AddUser(string server, string user)
        {
            EditorPrefs.SetString("ASUser::" + server, user);
        }

        public static void CBCommitFinished(int actionResult)
        {
            if (ASWin.asCommitWin != null)
            {
                ASWin.asCommitWin.CommitFinished(actionResult != 0);
            }
        }

        public static void CBDoDiscardChanges(int actionResult)
        {
            ASWin.DoDiscardChanges(actionResult != 0);
        }

        public static void CBInitHistoryPage(int actionResult)
        {
            ASWin.InitHistoryPage(actionResult != 0);
        }

        public static void CBInitOverviewPage(int actionResult)
        {
            ASWin.InitOverviewPage(actionResult != 0);
        }

        public static void CBInitUpdatePage(int actionResult)
        {
            ASWin.InitUpdatePage(actionResult != 0);
        }

        public static void CBOverviewsCommitFinished(int actionResult)
        {
            if (ASWin != null)
            {
                ASWin.CommitFinished(actionResult != 0);
            }
        }

        public static void CBReinitASMainWindow()
        {
            ASWin.Reinit();
        }

        public static void CBReinitCommitWindow(int actionResult)
        {
            if (ASWin.asCommitWin != null)
            {
                ASWin.asCommitWin.Reinit(actionResult != 0);
            }
        }

        public static void CBReinitOnSuccess(int actionResult)
        {
            if (actionResult != 0)
            {
                ASWin.Reinit();
            }
            else
            {
                ASWin.Repaint();
            }
        }

        public static void CommitItemsChanged()
        {
            if ((asMainWin != null) || ((asMainWin == null) && (Resources.FindObjectsOfTypeAll(typeof(ASMainWindow)).Length != 0)))
            {
                ASWin.CommitItemsChanged();
            }
        }

        public static void DoAS()
        {
            if (!ASWin.Error)
            {
                ASWin.Show();
                ASWin.Focus();
            }
        }

        internal static string GetPassword(string server, string user)
        {
            return EditorPrefs.GetString("ASPassword::" + server + "::" + user, string.Empty);
        }

        internal static string GetUser(string server)
        {
            return EditorPrefs.GetString("ASUser::" + server, string.Empty);
        }

        public static bool InitializeMaintBinding()
        {
            int num;
            PListConfig config = new PListConfig("Library/ServerPreferences.plist");
            string user = config["Maint UserName"];
            string str2 = config["Maint Server"];
            string str3 = config["Maint project name"];
            string str4 = config["Maint database name"];
            string str5 = config["Maint port number"];
            if (!int.TryParse(config["Maint Timeout"], out num))
            {
                num = 5;
            }
            if (((str2.Length == 0) || (str3.Length == 0)) || ((str4.Length == 0) || (user.Length == 0)))
            {
                AssetServer.SetProjectName(string.Empty);
                return false;
            }
            AssetServer.SetProjectName(string.Format("{0} @ {1}", str3, str2));
            string[] textArray1 = new string[] { "host='", str2, "' user='", user, "' password='", GetPassword(str2, user), "' dbname='", str4, "' port='", str5, "' sslmode=disable ", config["Maint Connection Settings"] };
            string connectionString = string.Concat(textArray1);
            AssetServer.Initialize(user, connectionString, num);
            return true;
        }

        internal static void SetPassword(string server, string user, string password)
        {
            EditorPrefs.SetString("ASPassword::" + server + "::" + user, password);
        }

        public static bool SettingsAreValid()
        {
            PListConfig config = new PListConfig("Library/ServerPreferences.plist");
            string str = config["Maint UserName"];
            string str2 = config["Maint Server"];
            string str3 = config["Maint database name"];
            string str4 = config["Maint Timeout"];
            string str5 = config["Maint port number"];
            return ((((str.Length != 0) && (str2.Length != 0)) && ((str3.Length != 0) && (str4.Length != 0))) && (str5.Length != 0));
        }

        public static bool SettingsIfNeeded()
        {
            return InitializeMaintBinding();
        }

        public static void ShowASConflictResolutionsWindow(string[] conflicting)
        {
            ASWin.ShowConflictResolutions(conflicting);
        }

        private static void Testing_DummyCallback(bool success)
        {
            object[] prms = new object[] { success };
            Testing_Invoke(AssetServer.GetAndRemoveString("s_TestingClass"), AssetServer.GetAndRemoveString("s_TestingFunction"), prms);
        }

        public static void Testing_DummyConflictResolutionFunction(string[] conflicting)
        {
            object[] prms = new object[] { conflicting };
            Testing_Invoke(s_TestingConflictResClass, s_TestingConflictResFunction, prms);
        }

        public static string[] Testing_GetAllDatabaseNames()
        {
            MaintDatabaseRecord[] recordArray = AssetServer.AdminRefreshDatabases();
            string[] strArray = new string[recordArray.Length];
            for (int i = 0; i < recordArray.Length; i++)
            {
                strArray[i] = recordArray[i].name;
            }
            return strArray;
        }

        private static void Testing_Invoke(string klass, string method, params object[] prms)
        {
            try
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if ((assembly.GetName().Name != "UnityEditor") && (assembly.GetName().Name != "UnityEngine"))
                    {
                        foreach (Type type in AssemblyHelper.GetTypesFromAssembly(assembly))
                        {
                            if (type.Name == klass)
                            {
                                type.InvokeMember(method, BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, null, prms);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                if ((exception.InnerException != null) && (exception.InnerException.GetType() == typeof(ExitGUIException)))
                {
                    throw exception;
                }
                object[] objArray1 = new object[] { exception };
                Testing_Invoke(AssetServer.GetString("s_ExceptionHandlerClass"), AssetServer.GetString("s_ExceptionHandlerFunction"), objArray1);
            }
        }

        public static void Testing_SetActionFinishedCallback(string klass, string name)
        {
            AssetServer.SaveString("s_TestingClass", klass);
            AssetServer.SaveString("s_TestingFunction", name);
            AssetServer.SetAfterActionFinishedCallback("ASEditorBackend", "Testing_DummyCallback");
        }

        public static void Testing_SetActiveDatabase(string host, int port, string projectName, string dbName, string user, string pwd)
        {
            PListConfig config = new PListConfig("Library/ServerPreferences.plist");
            config["Maint Server"] = host;
            config["Maint UserName"] = user;
            config["Maint database name"] = dbName;
            config["Maint port number"] = port.ToString();
            config["Maint project name"] = projectName;
            config["Maint Password"] = string.Empty;
            config["Maint settings type"] = "manual";
            config["Maint Timeout"] = "5";
            config["Maint Connection Settings"] = string.Empty;
            config.Save();
        }

        public static void Testing_SetConflictResolutionFunction(string klass, string fn)
        {
            s_TestingConflictResClass = klass;
            s_TestingConflictResFunction = fn;
        }

        private static void Testing_SetExceptionHandler(string exceptionHandlerClass, string exceptionHandlerFunction)
        {
            AssetServer.SaveString("s_ExceptionHandlerClass", exceptionHandlerClass);
            AssetServer.SaveString("s_ExceptionHandlerFunction", exceptionHandlerFunction);
        }

        public static bool Testing_SetupDatabase(string host, int port, string adminUser, string adminPwd, string user, string pwd, string projectName)
        {
            AssetServer.AdminSetCredentials(host, port, adminUser, adminPwd);
            MaintDatabaseRecord[] recordArray = AssetServer.AdminRefreshDatabases();
            if (recordArray == null)
            {
                return false;
            }
            foreach (MaintDatabaseRecord record in recordArray)
            {
                if (record.name == projectName)
                {
                    AssetServer.AdminDeleteDB(projectName);
                }
            }
            if (AssetServer.AdminCreateDB(projectName) == 0)
            {
                return false;
            }
            string databaseName = AssetServer.GetDatabaseName(host, adminUser, adminPwd, port.ToString(), projectName);
            if (!AssetServer.AdminSetUserEnabled(databaseName, user, user, string.Empty, 1))
            {
                return false;
            }
            Testing_SetActiveDatabase(host, port, projectName, databaseName, user, pwd);
            return true;
        }

        public static ASMainWindow ASWin
        {
            get
            {
                return ((asMainWin == null) ? EditorWindow.GetWindowDontShow<ASMainWindow>() : asMainWin);
            }
        }
    }
}

