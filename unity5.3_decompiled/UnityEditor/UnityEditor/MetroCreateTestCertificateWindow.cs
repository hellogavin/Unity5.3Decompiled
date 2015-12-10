namespace UnityEditor
{
    using System;
    using System.IO;
    using UnityEngine;

    internal class MetroCreateTestCertificateWindow : EditorWindow
    {
        private string confirm;
        private string focus;
        private static readonly GUILayoutOption kButtonWidth = GUILayout.Width(110f);
        private const string kConfirmId = "confirm";
        private static readonly GUILayoutOption kLabelWidth = GUILayout.Width(110f);
        private const char kPasswordChar = '●';
        private const string kPasswordId = "password";
        private const string kPublisherId = "publisher";
        private const float kSpace = 5f;
        private GUIContent message;
        private GUIStyle messageStyle;
        private string password;
        private string path;
        private string publisher;

        public void OnGUI()
        {
            Event current = Event.current;
            bool flag = false;
            bool flag2 = false;
            if (current.type == EventType.KeyDown)
            {
                flag = current.keyCode == KeyCode.Escape;
                flag2 = (current.keyCode == KeyCode.Return) || (current.keyCode == KeyCode.KeypadEnter);
            }
            using (HorizontalLayout.DoLayout())
            {
                GUILayout.Space(10f);
                using (VerticalLayout.DoLayout())
                {
                    GUILayout.FlexibleSpace();
                    using (HorizontalLayout.DoLayout())
                    {
                        GUILayoutOption[] options = new GUILayoutOption[] { kLabelWidth };
                        GUILayout.Label(EditorGUIUtility.TextContent("Publisher|Publisher of the package."), options);
                        GUI.SetNextControlName("publisher");
                        this.publisher = GUILayout.TextField(this.publisher, new GUILayoutOption[0]);
                    }
                    GUILayout.Space(5f);
                    using (HorizontalLayout.DoLayout())
                    {
                        GUILayoutOption[] optionArray2 = new GUILayoutOption[] { kLabelWidth };
                        GUILayout.Label(EditorGUIUtility.TextContent("Password|Certificate password."), optionArray2);
                        GUI.SetNextControlName("password");
                        this.password = GUILayout.PasswordField(this.password, '●', new GUILayoutOption[0]);
                    }
                    GUILayout.Space(5f);
                    using (HorizontalLayout.DoLayout())
                    {
                        GUILayoutOption[] optionArray3 = new GUILayoutOption[] { kLabelWidth };
                        GUILayout.Label(EditorGUIUtility.TextContent("Confirm password|Re-enter certificate password."), optionArray3);
                        GUI.SetNextControlName("confirm");
                        this.confirm = GUILayout.PasswordField(this.confirm, '●', new GUILayoutOption[0]);
                    }
                    GUILayout.Space(10f);
                    using (HorizontalLayout.DoLayout())
                    {
                        GUILayout.Label(this.message, this.messageStyle, new GUILayoutOption[0]);
                        GUILayout.FlexibleSpace();
                        GUILayoutOption[] optionArray4 = new GUILayoutOption[] { kButtonWidth };
                        if (GUILayout.Button(EditorGUIUtility.TextContent("Create"), optionArray4) || flag2)
                        {
                            this.message = GUIContent.none;
                            if (string.IsNullOrEmpty(this.publisher))
                            {
                                this.message = EditorGUIUtility.TextContent("Publisher must be specified.");
                                this.focus = "publisher";
                            }
                            else if (this.password != this.confirm)
                            {
                                if (string.IsNullOrEmpty(this.confirm))
                                {
                                    this.message = EditorGUIUtility.TextContent("Confirm the password.");
                                    this.focus = "confirm";
                                }
                                else
                                {
                                    this.message = EditorGUIUtility.TextContent("Passwords do not match.");
                                    this.password = string.Empty;
                                    this.confirm = this.password;
                                    this.focus = "password";
                                }
                            }
                            else
                            {
                                try
                                {
                                    EditorUtility.WSACreateTestCertificate(this.path, this.publisher, this.password, true);
                                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                                    if (!PlayerSettings.WSA.SetCertificate(FileUtil.GetProjectRelativePath(this.path), this.password))
                                    {
                                        this.message = EditorGUIUtility.TextContent("Invalid password.");
                                    }
                                    flag = true;
                                }
                                catch (UnityException exception)
                                {
                                    Debug.LogError(exception.Message);
                                }
                            }
                        }
                    }
                    GUILayout.FlexibleSpace();
                }
                GUILayout.Space(10f);
            }
            if (flag)
            {
                base.Close();
            }
            else if (this.focus != null)
            {
                EditorGUI.FocusTextInControl(this.focus);
                this.focus = null;
            }
        }

        public static void Show(string publisher)
        {
            MetroCreateTestCertificateWindow[] windowArray = (MetroCreateTestCertificateWindow[]) Resources.FindObjectsOfTypeAll(typeof(MetroCreateTestCertificateWindow));
            MetroCreateTestCertificateWindow window = (windowArray.Length <= 0) ? ScriptableObject.CreateInstance<MetroCreateTestCertificateWindow>() : windowArray[0];
            window.path = Path.Combine(Application.dataPath, "WSATestCertificate.pfx").Replace('\\', '/');
            window.publisher = publisher;
            window.password = string.Empty;
            window.confirm = window.password;
            window.message = !File.Exists(window.path) ? GUIContent.none : EditorGUIUtility.TextContent("Current file will be overwritten.");
            window.messageStyle = new GUIStyle(GUI.skin.label);
            window.messageStyle.fontStyle = FontStyle.Italic;
            window.focus = "publisher";
            if (windowArray.Length > 0)
            {
                window.Focus();
            }
            else
            {
                window.titleContent = EditorGUIUtility.TextContent("Create Test Certificate for Windows Store");
                window.position = new Rect(100f, 100f, 350f, 140f);
                window.minSize = new Vector2(window.position.width, window.position.height);
                window.maxSize = window.minSize;
                window.ShowUtility();
            }
        }
    }
}

