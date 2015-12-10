namespace UnityEditor
{
    using System;
    using UnityEditorInternal;
    using UnityEngine;

    internal class WelcomeScreen : EditorWindow
    {
        private const string kAssetStoreURL = "home/?ref=http%3a%2f%2fUnityEditor.unity3d.com%2fWelcomeScreen";
        private const float kItemHeight = 55f;
        private const string kUnityAnswersURL = "http://answers.unity3d.com/";
        private const string kUnityBasicsHelp = "file:///unity/Manual/UnityBasics.html";
        private const string kUnityForumURL = "http://forum.unity3d.com/";
        private const string kVideoTutURL = "http://unity3d.com/learn/tutorials/modules/";
        private static bool s_ShowAtStartup;
        private static int s_ShowCount;
        private static Styles styles;

        private static void DoShowWelcomeScreen(string how)
        {
            s_ShowCount++;
            EditorPrefs.SetInt("WelcomeScreenShowCount", s_ShowCount);
            Analytics.Track(string.Format("/WelcomeScreen/Show/{0}/{1}", how, s_ShowCount));
            EditorWindow.GetWindowWithRect<WelcomeScreen>(new Rect(0f, 0f, 570f, 440f), true, "Welcome To Unity");
        }

        private static void LoadLogos()
        {
            if (styles == null)
            {
                s_ShowAtStartup = EditorPrefs.GetInt("ShowWelcomeAtStartup4", 1) != 0;
                s_ShowCount = EditorPrefs.GetInt("WelcomeScreenShowCount", 0);
                styles = new Styles();
            }
        }

        public void OnGUI()
        {
            LoadLogos();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUI.Box(new Rect(13f, 8f, (float) styles.unityLogo.image.width, (float) styles.unityLogo.image.height), styles.unityLogo, GUIStyle.none);
            GUILayout.Space(15f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(120f);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Label(styles.mainHeader, new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(300f) };
            GUILayout.Label(styles.mainText, "WordWrappedLabel", options);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(8f);
            this.ShowEntry(styles.videoTutLogo, "http://unity3d.com/learn/tutorials/modules/", styles.videoTutHeader, styles.videoTutText, "VideoTutorials");
            this.ShowEntry(styles.unityBasicsLogo, "file:///unity/Manual/UnityBasics.html", styles.unityBasicsHeader, styles.unityBasicsText, "UnityBasics");
            this.ShowEntry(styles.unityAnswersLogo, "http://answers.unity3d.com/", styles.unityAnswersHeader, styles.unityAnswersText, "UnityAnswers");
            this.ShowEntry(styles.unityForumLogo, "http://forum.unity3d.com/", styles.unityForumHeader, styles.unityForumText, "UnityForum");
            this.ShowEntry(styles.assetStoreLogo, "home/?ref=http%3a%2f%2fUnityEditor.unity3d.com%2fWelcomeScreen", styles.assetStoreHeader, styles.assetStoreText, "AssetStore");
            GUILayout.FlexibleSpace();
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Height(20f) };
            GUILayout.BeginHorizontal(optionArray2);
            GUILayout.FlexibleSpace();
            GUI.changed = false;
            s_ShowAtStartup = GUILayout.Toggle(s_ShowAtStartup, styles.showAtStartupText, new GUILayoutOption[0]);
            if (GUI.changed)
            {
                EditorPrefs.SetInt("ShowWelcomeAtStartup4", !s_ShowAtStartup ? 0 : 1);
                if (s_ShowAtStartup)
                {
                    Analytics.Track(string.Format("/WelcomeScreen/EnableAtStartup/{0}", s_ShowCount));
                }
                else
                {
                    Analytics.Track(string.Format("/WelcomeScreen/DisableAtStartup/{0}", s_ShowCount));
                }
                s_ShowCount = 0;
                EditorPrefs.SetInt("WelcomeScreenShowCount", 0);
            }
            GUILayout.Space(10f);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void ShowEntry(GUIContent logo, string url, GUIContent header, GUIContent text, string analyticsAction)
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(55f) };
            GUILayout.BeginHorizontal(options);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(120f) };
            GUILayout.BeginHorizontal(optionArray2);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(logo, GUIStyle.none, new GUILayoutOption[0]))
            {
                this.ShowHelpPageOrBrowseURL(url, analyticsAction);
            }
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
            GUILayout.Space(10f);
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            if (GUILayout.Button(header, "HeaderLabel", new GUILayoutOption[0]))
            {
                this.ShowHelpPageOrBrowseURL(url, analyticsAction);
            }
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(400f) };
            GUILayout.Label(text, "WordWrappedLabel", optionArray3);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void ShowHelpPageOrBrowseURL(string url, string analyticsAction)
        {
            Analytics.Track(string.Format("/WelcomeScreen/OpenURL/{0}/{1}", analyticsAction, s_ShowCount));
            if (url.StartsWith("file"))
            {
                Help.ShowHelpPage(url);
            }
            else if (url.StartsWith("home/"))
            {
                AssetStore.Open(url);
                GUIUtility.ExitGUI();
            }
            else
            {
                Help.BrowseURL(url);
            }
        }

        private static void ShowWelcomeScreen()
        {
            DoShowWelcomeScreen("Manual");
        }

        private static void ShowWelcomeScreenAtStartup()
        {
            LoadLogos();
            if (s_ShowAtStartup)
            {
                DoShowWelcomeScreen("Startup");
            }
        }

        private class Styles
        {
            public GUIContent assetStoreHeader = EditorGUIUtility.TextContent("Unity Asset Store");
            public GUIContent assetStoreLogo = EditorGUIUtility.IconContent("WelcomeScreen.AssetStoreLogo");
            public GUIContent assetStoreText = EditorGUIUtility.TextContent("The Asset Store is the place to find art assets, game code and extensions directly inside the Unity editor. It's like having a complete supermarket under your desk.");
            public GUIContent mainHeader = EditorGUIUtility.IconContent("WelcomeScreen.MainHeader");
            public GUIContent mainText = EditorGUIUtility.TextContent("As you dive into Unity, may we suggest a few hints to get you off to a good start?");
            public GUIContent showAtStartupText = EditorGUIUtility.TextContent("Show at Startup");
            public GUIContent unityAnswersHeader = EditorGUIUtility.TextContent("Unity Answers");
            public GUIContent unityAnswersLogo = EditorGUIUtility.IconContent("WelcomeScreen.UnityAnswersLogo");
            public GUIContent unityAnswersText = EditorGUIUtility.TextContent("Have a question about how to use Unity? Check our answers site for precise how-to knowledge.");
            public GUIContent unityBasicsHeader = EditorGUIUtility.TextContent("Unity Basics");
            public GUIContent unityBasicsLogo = EditorGUIUtility.IconContent("WelcomeScreen.UnityBasicsLogo");
            public GUIContent unityBasicsText = EditorGUIUtility.TextContent("Take a look at our manual for a quick startup guide.");
            public GUIContent unityForumHeader = EditorGUIUtility.TextContent("Unity Forum");
            public GUIContent unityForumLogo = EditorGUIUtility.IconContent("WelcomeScreen.UnityForumLogo");
            public GUIContent unityForumText = EditorGUIUtility.TextContent("Meet the other Unity users here - the friendliest people in the industry.");
            public GUIContent unityLogo = EditorGUIUtility.IconContent("UnityLogo");
            public GUIContent videoTutHeader = EditorGUIUtility.TextContent("Video Tutorials");
            public GUIContent videoTutLogo = EditorGUIUtility.IconContent("WelcomeScreen.VideoTutLogo");
            public GUIContent videoTutText = EditorGUIUtility.TextContent("We have collected some videos to make you productive immediately.");
        }
    }
}

