namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class LicenseManagementWindow : EditorWindow
    {
        private static int buttonWidth = 140;
        private static int height = 350;
        private static int left = 0;
        private static int offsetX = 50;
        private static int offsetY = 0x19;
        private static Rect rectArea = new Rect((float) offsetX, (float) offsetY, (float) (width - (offsetX * 2)), (float) (height - (offsetY * 2)));
        private static int top = 0;
        private static int width = 600;
        private static LicenseManagementWindow win = null;
        private static Rect windowArea;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ActivateNewLicense();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CheckForUpdates();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ManualActivation();
        private void OnGUI()
        {
            GUILayout.BeginArea(rectArea);
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandHeight(true), GUILayout.Width((float) buttonWidth) };
            if (GUILayout.Button("Check for updates", options))
            {
                CheckForUpdates();
            }
            GUI.skin.label.wordWrap = true;
            GUILayout.Label("Checks for updates to the currently installed license. If you have purchased addons you can install them by pressing this button (Internet access required)", new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            GUILayout.Space(15f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandHeight(true), GUILayout.Width((float) buttonWidth) };
            if (GUILayout.Button("Activate new license", optionArray2))
            {
                ActivateNewLicense();
                Window.Close();
            }
            GUILayout.Label("Activate Unity with a different serial number, switch to a free serial or start a trial period if you are eligible for it (Internet access required).", new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            GUILayout.Space(15f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.ExpandHeight(true), GUILayout.Width((float) buttonWidth) };
            if (GUILayout.Button("Return license", optionArray3))
            {
                ReturnLicense();
            }
            GUILayout.Label("Return this license and free an activation for the serial it is using. You can then reuse the activation on another machine (Internet access required).", new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            GUILayout.Space(15f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] optionArray4 = new GUILayoutOption[] { GUILayout.ExpandHeight(true), GUILayout.Width((float) buttonWidth) };
            if (GUILayout.Button("Manual activation", optionArray4))
            {
                ManualActivation();
            }
            GUILayout.Label("Start the manual activation process, you can save this machines license activation request file or deploy a license file you have already activated manually.", new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            GUILayout.Space(15f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] optionArray5 = new GUILayoutOption[] { GUILayout.ExpandHeight(true), GUILayout.Width((float) buttonWidth) };
            if (GUILayout.Button("Unity FAQ", optionArray5))
            {
                Application.OpenURL("http://unity3d.com/unity/faq");
            }
            GUILayout.Label("Open the Unity FAQ web page, where you can find information about Unity's license system.", new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndArea();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ReturnLicense();
        private static void ShowWindow()
        {
            Resolution currentResolution = Screen.currentResolution;
            left = (currentResolution.width - width) / 2;
            top = (currentResolution.height - height) / 2;
            windowArea = new Rect((float) left, (float) top, (float) width, (float) height);
            win = Window;
            win.position = windowArea;
            win.Show();
        }

        private static LicenseManagementWindow Window
        {
            get
            {
                if (win == null)
                {
                    win = EditorWindow.GetWindowWithRect<LicenseManagementWindow>(windowArea, true, "License Management");
                }
                return win;
            }
        }
    }
}

