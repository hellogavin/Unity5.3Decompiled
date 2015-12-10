namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class MetroCertificatePasswordWindow : EditorWindow
    {
        private string focus;
        private static readonly GUILayoutOption kButtonWidth = GUILayout.Width(110f);
        private static readonly GUILayoutOption kLabelWidth = GUILayout.Width(110f);
        private const char kPasswordChar = '●';
        private const string kPasswordId = "password";
        private const float kSpace = 5f;
        private GUIContent message;
        private GUIStyle messageStyle;
        private string password;
        private string path;

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
                        GUILayout.Label(EditorGUIUtility.TextContent("Password|Certificate password."), options);
                        GUI.SetNextControlName("password");
                        this.password = GUILayout.PasswordField(this.password, '●', new GUILayoutOption[0]);
                    }
                    GUILayout.Space(10f);
                    using (HorizontalLayout.DoLayout())
                    {
                        GUILayout.Label(this.message, this.messageStyle, new GUILayoutOption[0]);
                        GUILayout.FlexibleSpace();
                        GUILayoutOption[] optionArray2 = new GUILayoutOption[] { kButtonWidth };
                        if (GUILayout.Button(EditorGUIUtility.TextContent("Ok"), optionArray2) || flag2)
                        {
                            this.message = GUIContent.none;
                            try
                            {
                                if (PlayerSettings.WSA.SetCertificate(this.path, this.password))
                                {
                                    flag = true;
                                }
                                else
                                {
                                    this.message = EditorGUIUtility.TextContent("Invalid password.");
                                }
                            }
                            catch (UnityException exception)
                            {
                                Debug.LogError(exception.Message);
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

        public static void Show(string path)
        {
            MetroCertificatePasswordWindow[] windowArray = (MetroCertificatePasswordWindow[]) Resources.FindObjectsOfTypeAll(typeof(MetroCertificatePasswordWindow));
            MetroCertificatePasswordWindow window = (windowArray.Length <= 0) ? ScriptableObject.CreateInstance<MetroCertificatePasswordWindow>() : windowArray[0];
            window.path = path;
            window.password = string.Empty;
            window.message = GUIContent.none;
            window.messageStyle = new GUIStyle(GUI.skin.label);
            window.messageStyle.fontStyle = FontStyle.Italic;
            window.focus = "password";
            if (windowArray.Length > 0)
            {
                window.Focus();
            }
            else
            {
                window.titleContent = EditorGUIUtility.TextContent("Enter Windows Store Certificate Password");
                window.position = new Rect(100f, 100f, 350f, 90f);
                window.minSize = new Vector2(window.position.width, window.position.height);
                window.maxSize = window.minSize;
                window.ShowUtility();
            }
        }
    }
}

