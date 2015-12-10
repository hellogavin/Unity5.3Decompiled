namespace UnityEditor
{
    using System;
    using System.IO;
    using UnityEditorInternal;
    using UnityEngine;

    internal class ScreenShots
    {
        public static Color kToolbarBorderColor = new Color(0.54f, 0.54f, 0.54f, 1f);
        public static Color kWindowBorderColor = new Color(0.51f, 0.51f, 0.51f, 1f);
        public static bool s_TakeComponentScreenshot = false;

        private static string GetGUIViewName(GUIView view)
        {
            HostView view2 = view as HostView;
            if (view2 != null)
            {
                return view2.actualView.GetType().Name;
            }
            return "Window";
        }

        private static GUIView GetMouseOverView()
        {
            GUIView mouseOverView = GUIView.mouseOverView;
            if (mouseOverView == null)
            {
                EditorApplication.Beep();
                Debug.LogWarning("Could not take screenshot.");
            }
            return mouseOverView;
        }

        private static string GetUniquePathForName(string name)
        {
            string path = string.Format("{0}/../../{1}.png", Application.dataPath, name);
            for (int i = 0; File.Exists(path); i++)
            {
                path = string.Format("{0}/../../{1}{2:000}.png", Application.dataPath, name, i);
            }
            return path;
        }

        public static void SaveScreenShot(Rect r, string name)
        {
            SaveScreenShot((int) r.width, (int) r.height, InternalEditorUtility.ReadScreenPixel(new Vector2(r.x, r.y), (int) r.width, (int) r.height), name);
        }

        private static string SaveScreenShot(int width, int height, Color[] pixels, string name)
        {
            Texture2D textured = new Texture2D(width, height);
            textured.SetPixels(pixels, 0);
            textured.Apply(true);
            byte[] bytes = textured.EncodeToPNG();
            Object.DestroyImmediate(textured, true);
            string uniquePathForName = GetUniquePathForName(name);
            File.WriteAllBytes(uniquePathForName, bytes);
            Debug.Log(string.Format("Saved screenshot at {0}", uniquePathForName));
            return uniquePathForName;
        }

        public static string SaveScreenShotWithBorder(Rect r, Color borderColor, string name)
        {
            int width = (int) r.width;
            int height = (int) r.height;
            Color[] colorArray = InternalEditorUtility.ReadScreenPixel(new Vector2(r.x, r.y), width, height);
            Color[] pixels = new Color[(width + 2) * (height + 2)];
            for (int i = 0; i < width; i++)
            {
                for (int m = 0; m < height; m++)
                {
                    pixels[(i + 1) + ((width + 2) * (m + 1))] = colorArray[i + (width * m)];
                }
            }
            for (int j = 0; j < (width + 2); j++)
            {
                pixels[j] = borderColor;
                pixels[j + ((width + 2) * (height + 1))] = borderColor;
            }
            for (int k = 0; k < (height + 2); k++)
            {
                pixels[k * (width + 2)] = borderColor;
                pixels[(k * (width + 2)) + (width + 1)] = borderColor;
            }
            return SaveScreenShot((int) (r.width + 2f), (int) (r.height + 2f), pixels, name);
        }

        [MenuItem("Window/Screenshot/Snap Game View Content", false, 0x3e8, true)]
        public static void ScreenGameViewContent()
        {
            string uniquePathForName = GetUniquePathForName("ContentExample");
            Application.CaptureScreenshot(uniquePathForName);
            Debug.Log(string.Format("Saved screenshot at {0}", uniquePathForName));
        }

        [MenuItem("Window/Screenshot/Snap View %&j", false, 0x3e8, true)]
        public static void Screenshot()
        {
            GUIView mouseOverView = GetMouseOverView();
            if (mouseOverView != null)
            {
                string gUIViewName = GetGUIViewName(mouseOverView);
                Rect screenPosition = mouseOverView.screenPosition;
                screenPosition.y--;
                screenPosition.height += 2f;
                SaveScreenShot(screenPosition, gUIViewName);
            }
        }

        [MenuItem("Window/Screenshot/Snap Component", false, 0x3e8, true)]
        public static void ScreenShotComponent()
        {
            s_TakeComponentScreenshot = true;
        }

        public static void ScreenShotComponent(Rect contentRect, Object target)
        {
            s_TakeComponentScreenshot = false;
            contentRect.yMax += 2f;
            contentRect.xMin++;
            SaveScreenShotWithBorder(contentRect, kWindowBorderColor, target.GetType().Name + "Inspector");
        }

        [MenuItem("Window/Screenshot/Snap View Extended Right %&k", false, 0x3e8, true)]
        public static void ScreenshotExtendedRight()
        {
            GUIView mouseOverView = GetMouseOverView();
            if (mouseOverView != null)
            {
                string name = GetGUIViewName(mouseOverView) + "Extended";
                MainWindow window = Resources.FindObjectsOfTypeAll(typeof(MainWindow))[0] as MainWindow;
                Rect screenPosition = mouseOverView.screenPosition;
                screenPosition.xMax = window.window.position.xMax;
                screenPosition.y--;
                screenPosition.height += 2f;
                SaveScreenShot(screenPosition, name);
            }
        }

        [MenuItem("Window/Screenshot/Snap View Toolbar", false, 0x3e8, true)]
        public static void ScreenshotToolbar()
        {
            GUIView mouseOverView = GetMouseOverView();
            if (mouseOverView != null)
            {
                string name = GetGUIViewName(mouseOverView) + "Toolbar";
                Rect screenPosition = mouseOverView.screenPosition;
                screenPosition.y += 19f;
                screenPosition.height = 16f;
                screenPosition.width -= 2f;
                SaveScreenShotWithBorder(screenPosition, kToolbarBorderColor, name);
            }
        }

        [MenuItem("Window/Screenshot/Set Window Size %&l", false, 0x3e8, true)]
        public static void SetMainWindowSize()
        {
            MainWindow window = Resources.FindObjectsOfTypeAll(typeof(MainWindow))[0] as MainWindow;
            window.window.position = new Rect(0f, 0f, 1024f, 768f);
        }

        [MenuItem("Window/Screenshot/Set Window Size Small", false, 0x3e8, true)]
        public static void SetMainWindowSizeSmall()
        {
            MainWindow window = Resources.FindObjectsOfTypeAll(typeof(MainWindow))[0] as MainWindow;
            window.window.position = new Rect(0f, 0f, 762f, 600f);
        }

        [MenuItem("Window/Screenshot/Toggle DeveloperBuild", false, 0x3e8, true)]
        public static void ToggleFakeNonDeveloperBuild()
        {
            Unsupported.fakeNonDeveloperBuild = !Unsupported.fakeNonDeveloperBuild;
            InternalEditorUtility.RequestScriptReload();
            InternalEditorUtility.RepaintAllViews();
        }
    }
}

