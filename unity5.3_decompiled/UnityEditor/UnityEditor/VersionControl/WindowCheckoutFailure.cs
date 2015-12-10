namespace UnityEditor.VersionControl
{
    using System;
    using UnityEditor;
    using UnityEditorInternal.VersionControl;
    using UnityEngine;

    internal class WindowCheckoutFailure : EditorWindow
    {
        private AssetList assetList = new AssetList();
        private ListControl checkoutFailureList = new ListControl();
        private ListControl checkoutSuccessList = new ListControl();

        private void DoOpen(AssetList assets, bool alreadyOpen)
        {
            if (alreadyOpen)
            {
                foreach (Asset asset in assets)
                {
                    bool flag = false;
                    int count = this.assetList.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (this.assetList[i].path == asset.path)
                        {
                            flag = true;
                            this.assetList[i] = asset;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        this.assetList.Add(asset);
                    }
                }
            }
            else
            {
                this.assetList.AddRange(assets);
            }
            this.RefreshList();
        }

        private static WindowCheckoutFailure GetWindow()
        {
            return EditorWindow.GetWindow<WindowCheckoutFailure>(true, "Version Control Check Out Failed");
        }

        public void OnEnable()
        {
            base.position = new Rect(100f, 100f, 700f, 230f);
            base.minSize = new Vector2(700f, 230f);
            this.checkoutSuccessList.ReadOnly = true;
            this.checkoutFailureList.ReadOnly = true;
        }

        public void OnGUI()
        {
            float height = (base.position.height - 122f) / 2f;
            GUILayout.Label("Some files could not be checked out:", EditorStyles.boldLabel, new GUILayoutOption[0]);
            Rect screenRect = new Rect(6f, 40f, base.position.width - 12f, height);
            GUILayout.BeginArea(screenRect);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) };
            GUILayout.Box(string.Empty, options);
            GUILayout.EndArea();
            this.checkoutFailureList.OnGUI(new Rect(screenRect.x + 2f, screenRect.y + 2f, screenRect.width - 4f, screenRect.height - 4f), true);
            GUILayout.Space(20f + height);
            GUILayout.Label("The following files were successfully checked out:", EditorStyles.boldLabel, new GUILayoutOption[0]);
            Rect rect2 = new Rect(6f, (40f + height) + 40f, base.position.width - 12f, height);
            GUILayout.BeginArea(rect2);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true) };
            GUILayout.Box(string.Empty, optionArray2);
            GUILayout.EndArea();
            this.checkoutSuccessList.OnGUI(new Rect(rect2.x + 2f, rect2.y + 2f, rect2.width - 4f, rect2.height - 4f), true);
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorUserSettings.showFailedCheckout = !GUILayout.Toggle(!EditorUserSettings.showFailedCheckout, "Don't show this window again.", new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            bool enabled = GUI.enabled;
            GUI.enabled = this.checkoutFailureList.Size > 0;
            if (GUILayout.Button("Retry Check Out", new GUILayoutOption[0]))
            {
                Provider.Checkout(this.assetList, CheckoutMode.Exact);
            }
            GUI.enabled = this.checkoutSuccessList.Size > 0;
            if (GUILayout.Button("Revert Unchanged", new GUILayoutOption[0]))
            {
                Provider.Revert(this.assetList, RevertMode.Unchanged).SetCompletionAction(CompletionAction.UpdatePendingWindow);
                Provider.Status(this.assetList);
                base.Close();
            }
            GUI.enabled = enabled;
            if (GUILayout.Button("OK", new GUILayoutOption[0]))
            {
                base.Close();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(12f);
        }

        public static void OpenIfCheckoutFailed(AssetList assets)
        {
            Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(WindowCheckoutFailure));
            WindowCheckoutFailure failure = (objArray.Length <= 0) ? null : (objArray[0] as WindowCheckoutFailure);
            bool alreadyOpen = failure != null;
            bool flag2 = alreadyOpen;
            if (!flag2)
            {
                foreach (Asset asset in assets)
                {
                    if (!asset.IsState(Asset.States.CheckedOutLocal))
                    {
                        flag2 = true;
                        break;
                    }
                }
            }
            if (flag2)
            {
                GetWindow().DoOpen(assets, alreadyOpen);
            }
        }

        private void RefreshList()
        {
            this.checkoutSuccessList.Clear();
            this.checkoutFailureList.Clear();
            foreach (Asset asset in this.assetList)
            {
                if (asset.IsState(Asset.States.CheckedOutLocal))
                {
                    this.checkoutSuccessList.Add(null, asset.prettyPath, asset);
                }
                else
                {
                    this.checkoutFailureList.Add(null, asset.prettyPath, asset);
                }
            }
            this.checkoutSuccessList.Refresh();
            this.checkoutFailureList.Refresh();
            base.Repaint();
        }
    }
}

