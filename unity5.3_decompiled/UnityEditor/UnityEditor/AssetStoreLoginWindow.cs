namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AssetStoreLoginWindow : EditorWindow
    {
        private const float kBaseHeight = 110f;
        private LoginCallback m_LoginCallback;
        private string m_LoginReason;
        private string m_LoginRemoteMessage;
        private string m_Password = string.Empty;
        private string m_Username = string.Empty;
        private static GUIContent s_AssetStoreLogo;
        private static Styles styles;

        private void DoLogin()
        {
            this.m_LoginRemoteMessage = null;
            if (AssetStoreClient.HasActiveSessionID)
            {
                AssetStoreClient.Logout();
            }
            AssetStoreClient.LoginWithCredentials(this.m_Username, this.m_Password, AssetStoreClient.RememberSession, delegate (string errorMessage) {
                this.m_LoginRemoteMessage = errorMessage;
                if (errorMessage == null)
                {
                    base.Close();
                }
                else
                {
                    base.Repaint();
                }
            });
        }

        private static void LoadLogos()
        {
            if (s_AssetStoreLogo == null)
            {
                s_AssetStoreLogo = new GUIContent(string.Empty);
            }
        }

        public static void Login(string loginReason, LoginCallback callback)
        {
            <Login>c__AnonStorey56 storey = new <Login>c__AnonStorey56 {
                callback = callback,
                loginReason = loginReason
            };
            if (AssetStoreClient.HasActiveSessionID)
            {
                AssetStoreClient.Logout();
            }
            if (!AssetStoreClient.RememberSession || !AssetStoreClient.HasSavedSessionID)
            {
                ShowAssetStoreLoginWindow(storey.loginReason, storey.callback);
            }
            else
            {
                AssetStoreClient.LoginWithRememberedSession(new AssetStoreClient.DoneLoginCallback(storey.<>m__97));
            }
        }

        public static void Logout()
        {
            AssetStoreClient.Logout();
        }

        public void OnDisable()
        {
            if (this.m_LoginCallback != null)
            {
                this.m_LoginCallback(this.m_LoginRemoteMessage);
            }
            this.m_LoginCallback = null;
            this.m_Password = null;
        }

        public void OnGUI()
        {
            if (styles == null)
            {
                styles = new Styles();
            }
            LoadLogos();
            if (AssetStoreClient.LoginInProgress() || AssetStoreClient.LoggedIn())
            {
                GUI.enabled = false;
            }
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(5f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            GUILayout.Label(s_AssetStoreLogo, GUIStyle.none, options);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(6f);
            GUILayout.Label(this.m_LoginReason, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
            Rect lastRect = GUILayoutUtility.GetLastRect();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(6f);
            Rect rect2 = new Rect(0f, 0f, 0f, 0f);
            if (this.m_LoginRemoteMessage != null)
            {
                Color color = GUI.color;
                GUI.color = Color.red;
                GUILayout.Label(this.m_LoginRemoteMessage, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
                GUI.color = color;
                rect2 = GUILayoutUtility.GetLastRect();
            }
            float height = (lastRect.height + rect2.height) + 110f;
            if ((Event.current.type == EventType.Repaint) && (height != base.position.height))
            {
                base.position = new Rect(base.position.x, base.position.y, base.position.width, height);
                base.Repaint();
            }
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUI.SetNextControlName("username");
            this.m_Username = EditorGUILayout.TextField("Username", this.m_Username, new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            this.m_Password = EditorGUILayout.PasswordField("Password", this.m_Password, optionArray2);
            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            if (GUILayout.Button(new GUIContent("Forgot?", "Reset your password"), styles.link, optionArray3))
            {
                Application.OpenURL("https://accounts.unity3d.com/password/new");
            }
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
            GUILayout.EndHorizontal();
            bool rememberSession = AssetStoreClient.RememberSession;
            bool flag2 = EditorGUILayout.Toggle("Remember me", rememberSession, new GUILayoutOption[0]);
            if (flag2 != rememberSession)
            {
                AssetStoreClient.RememberSession = flag2;
            }
            GUILayout.EndVertical();
            GUILayout.Space(5f);
            GUILayout.EndHorizontal();
            GUILayout.Space(8f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button("Create account", new GUILayoutOption[0]))
            {
                AssetStore.Open("createuser/");
                this.m_LoginRemoteMessage = "Cancelled - create user";
                base.Close();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Cancel", new GUILayoutOption[0]))
            {
                this.m_LoginRemoteMessage = "Cancelled";
                base.Close();
            }
            GUILayout.Space(5f);
            if (GUILayout.Button("Login", new GUILayoutOption[0]))
            {
                this.DoLogin();
                base.Repaint();
            }
            GUILayout.Space(5f);
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
            GUILayout.EndVertical();
            if (Event.current.Equals(Event.KeyboardEvent("return")))
            {
                this.DoLogin();
                base.Repaint();
            }
            if (this.m_Username == string.Empty)
            {
                EditorGUI.FocusTextInControl("username");
            }
        }

        public static void ShowAssetStoreLoginWindow(string loginReason, LoginCallback callback)
        {
            AssetStoreLoginWindow window = EditorWindow.GetWindowWithRect<AssetStoreLoginWindow>(new Rect(100f, 100f, 360f, 140f), true, "Login to Asset Store");
            window.position = new Rect(100f, 100f, window.position.width, window.position.height);
            window.m_Parent.window.m_DontSaveToLayout = true;
            window.m_Password = string.Empty;
            window.m_LoginCallback = callback;
            window.m_LoginReason = loginReason;
            window.m_LoginRemoteMessage = null;
            Analytics.Track("/AssetStore/Login");
        }

        public static bool IsLoggedIn
        {
            get
            {
                return AssetStoreClient.HasActiveSessionID;
            }
        }

        [CompilerGenerated]
        private sealed class <Login>c__AnonStorey56
        {
            internal AssetStoreLoginWindow.LoginCallback callback;
            internal string loginReason;

            internal void <>m__97(string errorMessage)
            {
                if (string.IsNullOrEmpty(errorMessage))
                {
                    this.callback(errorMessage);
                }
                else
                {
                    AssetStoreLoginWindow.ShowAssetStoreLoginWindow(this.loginReason, this.callback);
                }
            }
        }

        public delegate void LoginCallback(string errorMessage);

        private class Styles
        {
            public GUIStyle link = new GUIStyle(EditorStyles.miniLabel);

            public Styles()
            {
                this.link.normal.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
            }
        }
    }
}

