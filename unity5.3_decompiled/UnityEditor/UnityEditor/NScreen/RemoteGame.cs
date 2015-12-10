namespace UnityEditor.NScreen
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using UnityEditor;
    using UnityEngine;

    internal class RemoteGame : EditorWindow, IHasCustomMenu
    {
        public NScreenBridge bridge;
        public int id;
        private const float kMaxHeight = 720f;
        private const float kMaxWidth = 1280f;
        private int oldHeight;
        private int oldWidth;
        private Process remoteProcess;
        private Rect remoteViewRect;
        public bool shouldBuild;
        public bool shouldExit = true;

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Add Tab/Remote Game"), false, new GenericMenu.MenuFunction(NScreenManager.OpenAnotherWindow));
        }

        internal void DoExitGame()
        {
            if ((this.remoteProcess != null) && !this.remoteProcess.HasExited)
            {
                this.remoteProcess.Kill();
                this.remoteProcess = null;
                base.Repaint();
            }
            if (this.bridge != null)
            {
                this.bridge.Shutdown();
                this.bridge = null;
                this.oldWidth = 0;
                this.oldHeight = 0;
            }
            base.wantsMouseMove = false;
            this.shouldExit = true;
        }

        internal void GameViewAspectWasChanged()
        {
            this.oldWidth = 0;
            this.oldHeight = 0;
        }

        private void HandleExited(object sender, EventArgs e)
        {
            this.shouldExit = true;
        }

        internal bool IsRunning()
        {
            return !this.shouldExit;
        }

        private void OnDestroy()
        {
            this.DoExitGame();
        }

        private void OnEnable()
        {
            base.titleContent = new GUIContent("Remote Game");
        }

        private void OnGUI()
        {
            GUI.color = Color.white;
            GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(160f) };
            EditorGUILayout.GameViewSizePopup(ScriptableSingleton<GameViewSizes>.instance.currentGroupType, ScriptableSingleton<NScreenManager>.instance.SelectedSizeIndex, new Action<int, object>(this.SelectionCallback), EditorStyles.toolbarDropDown, options);
            GUILayout.FlexibleSpace();
            GUI.enabled = !Application.isPlaying;
            bool buildOnPlay = ScriptableSingleton<NScreenManager>.instance.BuildOnPlay;
            ScriptableSingleton<NScreenManager>.instance.BuildOnPlay = GUILayout.Toggle(ScriptableSingleton<NScreenManager>.instance.BuildOnPlay, "Build on Play", EditorStyles.toolbarButton, new GUILayoutOption[0]);
            if (buildOnPlay != ScriptableSingleton<NScreenManager>.instance.BuildOnPlay)
            {
                NScreenManager.RepaintAllGameViews();
            }
            if (GUILayout.Button("Build Now", EditorStyles.toolbarButton, new GUILayoutOption[0]))
            {
                this.shouldBuild = true;
            }
            GUI.enabled = Application.isPlaying;
            GUILayout.EndHorizontal();
            if (!this.shouldExit && (this.bridge != null))
            {
                Texture2D screenTexture = this.bridge.GetScreenTexture();
                if (screenTexture != null)
                {
                    GUI.DrawTexture(this.remoteViewRect, screenTexture);
                }
                if (this == EditorWindow.focusedWindow)
                {
                    this.bridge.SetInput(((int) Event.current.mousePosition.x) - ((int) this.remoteViewRect.x), (((((int) base.position.height) - ((int) Event.current.mousePosition.y)) - ((int) this.remoteViewRect.y)) + this.ToolBarHeight) - ((int) Mathf.Max((float) 0f, (float) (base.position.height - 720f))), Event.current.button, !Event.current.isKey ? -1 : ((int) Event.current.keyCode), (int) Event.current.type);
                }
                else
                {
                    this.bridge.ResetInput();
                }
            }
            else
            {
                GUILayout.Label("Game Stopped", new GUILayoutOption[0]);
            }
        }

        private void SelectionCallback(int indexClicked, object objectSelected)
        {
            if (indexClicked != ScriptableSingleton<NScreenManager>.instance.SelectedSizeIndex)
            {
                ScriptableSingleton<NScreenManager>.instance.SelectedSizeIndex = indexClicked;
                NScreenManager.RepaintAllGameViews();
            }
        }

        internal void StartGame()
        {
            this.DoExitGame();
            base.wantsMouseMove = true;
            this.bridge = new NScreenBridge();
            this.bridge.InitServer(this.id);
            this.bridge.SetResolution((int) base.minSize.x, (int) base.minSize.y);
            this.remoteProcess = new Process();
            this.remoteProcess.EnableRaisingEvents = true;
            this.remoteProcess.Exited += new EventHandler(this.HandleExited);
            this.remoteProcess.StartInfo.FileName = "Temp/NScreen/NScreen.app/Contents/MacOS/NScreen";
            this.remoteProcess.StartInfo.Arguments = "-nscreenid " + this.id;
            this.remoteProcess.StartInfo.UseShellExecute = false;
            try
            {
                this.remoteProcess.Start();
            }
            catch (Win32Exception)
            {
                this.remoteProcess = null;
                this.DoExitGame();
            }
            this.bridge.StartWatchdogForPid(this.remoteProcess.Id);
            this.shouldExit = false;
        }

        internal void StopGame()
        {
            this.shouldExit = true;
        }

        private void Update()
        {
            if (this.shouldExit)
            {
                this.DoExitGame();
            }
            else if (this.bridge != null)
            {
                if ((this.oldWidth != ((int) base.position.width)) || (this.oldHeight != ((int) base.position.height)))
                {
                    int num = (int) Mathf.Clamp(base.position.width, base.minSize.x, 1280f);
                    int num2 = (int) Mathf.Clamp(base.position.height, base.minSize.y, 720f);
                    bool fitsInsideRect = true;
                    this.remoteViewRect = GameViewSizes.GetConstrainedRect(new Rect(0f, 0f, (float) num, (float) num2), ScriptableSingleton<GameViewSizes>.instance.currentGroupType, ScriptableSingleton<NScreenManager>.instance.SelectedSizeIndex, out fitsInsideRect);
                    this.remoteViewRect.y += this.ToolBarHeight;
                    this.remoteViewRect.height -= this.ToolBarHeight;
                    this.bridge.SetResolution((int) this.remoteViewRect.width, (int) this.remoteViewRect.height);
                    this.oldWidth = (int) base.position.width;
                    this.oldHeight = (int) base.position.height;
                }
                this.bridge.Update();
                base.Repaint();
            }
            if (this.shouldBuild)
            {
                this.shouldBuild = false;
                NScreenManager.Build();
            }
        }

        private int ToolBarHeight
        {
            get
            {
                return 0x11;
            }
        }
    }
}

