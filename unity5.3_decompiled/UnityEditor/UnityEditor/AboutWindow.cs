namespace UnityEditor
{
    using System;
    using UnityEditor.Modules;
    using UnityEditor.VisualStudioIntegration;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AboutWindow : EditorWindow
    {
        private readonly string kCreditsNames = string.Join(", ", AboutWindowNames.names);
        private const string kSpecialThanksNames = "Thanks to Forest 'Yoggy' Johnson, Graham McAllister, David Janik-Jones, Raimund Schumacher, Alan J. Dickins and Emil 'Humus' Persson";
        private int m_InternalCodeProgress;
        private double m_LastScrollUpdate;
        private bool m_ShowDetailedVersion;
        private float m_TextInitialYPos = 120f;
        private float m_TextYPos = 120f;
        private float m_TotalCreditsHeight = float.PositiveInfinity;
        private static GUIContent s_AgeiaLogo;
        private static GUIContent s_Header;
        private static GUIContent s_MonoLogo;

        private string FormatExtensionVersionString()
        {
            string target = EditorUserBuildSettings.selectedBuildTargetGroup.ToString();
            string extensionVersion = ModuleManager.GetExtensionVersion(target);
            if (!string.IsNullOrEmpty(extensionVersion))
            {
                string[] textArray1 = new string[] { " [", target, " extension: ", extensionVersion, "]" };
                return string.Concat(textArray1);
            }
            return string.Empty;
        }

        private void ListenForSecretCodes()
        {
            if (((Event.current.type == EventType.KeyDown) && (Event.current.character != '\0')) && this.SecretCodeHasBeenTyped("internal", ref this.m_InternalCodeProgress))
            {
                bool flag = !EditorPrefs.GetBool("InternalMode", false);
                EditorPrefs.SetBool("InternalMode", flag);
                base.ShowNotification(new GUIContent("Internal Mode " + (!flag ? "Off" : "On")));
                InternalEditorUtility.RequestScriptReload();
            }
        }

        private static void LoadLogos()
        {
            if (s_MonoLogo == null)
            {
                s_MonoLogo = EditorGUIUtility.IconContent("MonoLogo");
                s_AgeiaLogo = EditorGUIUtility.IconContent("AgeiaLogo");
                s_Header = EditorGUIUtility.IconContent("AboutWindow.MainHeader");
            }
        }

        public void OnDisable()
        {
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateScroll));
        }

        public void OnEnable()
        {
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.UpdateScroll));
            this.m_LastScrollUpdate = EditorApplication.timeSinceStartup;
        }

        public void OnGUI()
        {
            LoadLogos();
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(5f);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayout.Label(s_Header, GUIStyle.none, new GUILayoutOption[0]);
            this.ListenForSecretCodes();
            string str = string.Empty;
            if (InternalEditorUtility.HasFreeLicense())
            {
                str = " Personal";
            }
            if (InternalEditorUtility.HasEduLicense())
            {
                str = " Edu";
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(52f);
            string str2 = this.FormatExtensionVersionString();
            this.m_ShowDetailedVersion |= Event.current.alt;
            if (this.m_ShowDetailedVersion)
            {
                int unityVersionDate = InternalEditorUtility.GetUnityVersionDate();
                DateTime time = new DateTime(0x7b2, 1, 1, 0, 0, 0, 0);
                string unityBuildBranch = InternalEditorUtility.GetUnityBuildBranch();
                string str4 = string.Empty;
                if (unityBuildBranch.Length > 0)
                {
                    str4 = "Branch: " + unityBuildBranch;
                }
                object[] args = new object[] { InternalEditorUtility.GetFullUnityVersion(), str, str2, time.AddSeconds((double) unityVersionDate), str4 };
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(400f), GUILayout.Height(42f) };
                EditorGUILayout.SelectableLabel(string.Format("Version {0}{1}{2}\n{3:r}\n{4}", args), options);
                this.m_TextInitialYPos = 108f;
            }
            else
            {
                GUILayout.Label(string.Format("Version {0}{1}{2}", Application.unityVersion, str, str2), new GUILayoutOption[0]);
            }
            if (Event.current.type != EventType.ValidateCommand)
            {
                GUILayout.EndHorizontal();
                GUILayout.Space(4f);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                GUI.BeginGroup(GUILayoutUtility.GetRect(10f, this.m_TextInitialYPos));
                float width = base.position.width - 10f;
                float height = EditorStyles.wordWrappedLabel.CalcHeight(GUIContent.Temp(this.kCreditsNames), width);
                Rect position = new Rect(5f, this.m_TextYPos, width, height);
                GUI.Label(position, this.kCreditsNames, EditorStyles.wordWrappedLabel);
                float num4 = EditorStyles.wordWrappedMiniLabel.CalcHeight(GUIContent.Temp("Thanks to Forest 'Yoggy' Johnson, Graham McAllister, David Janik-Jones, Raimund Schumacher, Alan J. Dickins and Emil 'Humus' Persson"), width);
                Rect rect2 = new Rect(5f, this.m_TextYPos + height, width, num4);
                GUI.Label(rect2, "Thanks to Forest 'Yoggy' Johnson, Graham McAllister, David Janik-Jones, Raimund Schumacher, Alan J. Dickins and Emil 'Humus' Persson", EditorStyles.wordWrappedMiniLabel);
                GUI.EndGroup();
                this.m_TotalCreditsHeight = height + num4;
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Label(s_MonoLogo, new GUILayoutOption[0]);
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(200f) };
                GUILayout.Label("Scripting powered by The Mono Project.\n\n(c) 2011 Novell, Inc.", "MiniLabel", optionArray2);
                GUILayout.Label(s_AgeiaLogo, new GUILayoutOption[0]);
                GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(200f) };
                GUILayout.Label("Physics powered by PhysX.\n\n(c) 2011 NVIDIA Corporation.", "MiniLabel", optionArray3);
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Space(5f);
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                string aboutWindowLabel = UnityVSSupport.GetAboutWindowLabel();
                if (aboutWindowLabel.Length > 0)
                {
                    GUILayout.Label(aboutWindowLabel, "MiniLabel", new GUILayoutOption[0]);
                }
                GUILayout.Label(InternalEditorUtility.GetUnityCopyright(), "MiniLabel", new GUILayoutOption[0]);
                GUILayout.EndVertical();
                GUILayout.Space(10f);
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                GUILayout.Label(InternalEditorUtility.GetLicenseInfo(), "AboutWindowLicenseLabel", new GUILayoutOption[0]);
                GUILayout.EndVertical();
                GUILayout.Space(5f);
                GUILayout.EndHorizontal();
                GUILayout.Space(5f);
            }
        }

        private bool SecretCodeHasBeenTyped(string code, ref int characterProgress)
        {
            if (((characterProgress < 0) || (characterProgress >= code.Length)) || (code[characterProgress] != Event.current.character))
            {
                characterProgress = 0;
            }
            if (code[characterProgress] == Event.current.character)
            {
                characterProgress++;
                if (characterProgress >= code.Length)
                {
                    characterProgress = 0;
                    return true;
                }
            }
            return false;
        }

        private static void ShowAboutWindow()
        {
            AboutWindow window = EditorWindow.GetWindowWithRect<AboutWindow>(new Rect(100f, 100f, 570f, 340f), true, "About Unity");
            window.position = new Rect(100f, 100f, 570f, 340f);
            window.m_Parent.window.m_DontSaveToLayout = true;
        }

        public void UpdateScroll()
        {
            double num = EditorApplication.timeSinceStartup - this.m_LastScrollUpdate;
            this.m_TextYPos -= 40f * ((float) num);
            if (this.m_TextYPos < -this.m_TotalCreditsHeight)
            {
                this.m_TextYPos = this.m_TextInitialYPos;
            }
            base.Repaint();
            this.m_LastScrollUpdate = EditorApplication.timeSinceStartup;
        }
    }
}

