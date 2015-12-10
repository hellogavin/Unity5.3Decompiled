namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(AssetStoreAssetInspector))]
    internal class AssetStoreAssetInspector : Editor
    {
        private int lastAssetID;
        internal static PaymentAvailability m_PaymentAvailability;
        private EditorWrapper m_PreviewEditor;
        private Object m_PreviewObject;
        private bool packageInfoShown = true;
        private Vector2 pos;
        internal static string s_PaymentMethodCard = string.Empty;
        internal static string s_PaymentMethodExpire = string.Empty;
        internal static string s_PriceText = string.Empty;
        internal static string s_PurchaseMessage = string.Empty;
        private static AssetStoreAssetInspector s_SharedAssetStoreAssetInspector;
        private static GUIContent[] sStatusWheel;
        private static Styles styles;

        public override string GetInfoString()
        {
            EditorWrapper previewEditor = this.previewEditor;
            AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
            if (firstAsset == null)
            {
                return "No item selected";
            }
            if ((previewEditor != null) && firstAsset.HasLivePreview)
            {
                return previewEditor.GetInfoString();
            }
            return string.Empty;
        }

        public override GUIContent GetPreviewTitle()
        {
            return GUIContent.Temp("Asset Store Preview");
        }

        public override bool HasPreviewGUI()
        {
            return ((this.target != null) && (AssetStoreAssetSelection.Count != 0));
        }

        private void ImportPackage(AssetStoreAsset asset)
        {
            if (paymentAvailability == PaymentAvailability.AnonymousUser)
            {
                this.LoginAndImport(asset);
            }
            else
            {
                AssetStoreInstaBuyWindow.ShowAssetStoreInstaBuyWindowBuilding(asset);
            }
        }

        private void InitiateBuySelected()
        {
            this.InitiateBuySelected(true);
        }

        private void InitiateBuySelected(bool firstAttempt)
        {
            AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
            if (firstAsset == null)
            {
                EditorUtility.DisplayDialog("No asset selected", "Please select asset before buying a package", "ok");
            }
            else if (paymentAvailability == PaymentAvailability.AnonymousUser)
            {
                if (AssetStoreClient.LoggedIn())
                {
                    AssetStoreAssetSelection.RefreshFromServer(() => this.InitiateBuySelected(false));
                }
                else if (firstAttempt)
                {
                    this.LoginAndInitiateBuySelected();
                }
            }
            else if (paymentAvailability == PaymentAvailability.ServiceDisabled)
            {
                if (firstAsset.previewInfo != null)
                {
                    AssetStore.Open(string.Format("content/{0}/directpurchase", firstAsset.packageID));
                }
            }
            else if (paymentAvailability == PaymentAvailability.BasketNotEmpty)
            {
                if (firstAsset.previewInfo != null)
                {
                    if (firstAttempt)
                    {
                        AssetStoreAssetSelection.RefreshFromServer(() => this.InitiateBuySelected(false));
                    }
                    else
                    {
                        AssetStore.Open(string.Format("content/{0}/basketpurchase", firstAsset.packageID));
                    }
                }
            }
            else
            {
                AssetStoreInstaBuyWindow.ShowAssetStoreInstaBuyWindow(firstAsset, s_PurchaseMessage, s_PaymentMethodCard, s_PaymentMethodExpire, s_PriceText);
            }
        }

        private static string intToSizeString(int inValue)
        {
            if (inValue < 0)
            {
                return "unknown";
            }
            float num = inValue;
            string[] strArray = new string[] { "TB", "GB", "MB", "KB", "Bytes" };
            int index = strArray.Length - 1;
            while ((num > 1000f) && (index >= 0))
            {
                num /= 1000f;
                index--;
            }
            if (index < 0)
            {
                return "<error>";
            }
            return string.Format("{0:#.##} {1}", num, strArray[index]);
        }

        private void LoginAndImport(AssetStoreAsset asset)
        {
            <LoginAndImport>c__AnonStorey7D storeyd = new <LoginAndImport>c__AnonStorey7D {
                asset = asset
            };
            AssetStoreLoginWindow.Login("Please login to the Asset Store in order to get download information for the package.", new AssetStoreLoginWindow.LoginCallback(storeyd.<>m__136));
        }

        private void LoginAndInitiateBuySelected()
        {
            AssetStoreLoginWindow.Login("Please login to the Asset Store in order to get payment information about the package.", delegate (string errorMessage) {
                if (errorMessage == null)
                {
                    AssetStoreAssetSelection.RefreshFromServer(() => this.InitiateBuySelected(false));
                }
            });
        }

        public void OnDisable()
        {
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
            if (this.m_PreviewEditor != null)
            {
                this.m_PreviewEditor.Dispose();
                this.m_PreviewEditor = null;
            }
            if (this.m_PreviewObject != null)
            {
                this.m_PreviewObject = null;
            }
            AssetStoreUtils.UnRegisterDownloadDelegate(this);
        }

        public void OnDownloadProgress(string id, string message, int bytes, int total)
        {
            AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
            if (firstAsset != null)
            {
                AssetStoreAsset.PreviewInfo previewInfo = firstAsset.previewInfo;
                if ((previewInfo != null) && (firstAsset.packageID.ToString() == id))
                {
                    if (((message == "downloading") || (message == "connecting")) && !OfflineNoticeEnabled)
                    {
                        previewInfo.downloadProgress = ((float) bytes) / ((float) total);
                    }
                    else
                    {
                        previewInfo.downloadProgress = -1f;
                    }
                    base.Repaint();
                }
            }
        }

        public void OnEnable()
        {
            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
            AssetStoreUtils.RegisterDownloadDelegate(this);
        }

        public override void OnInspectorGUI()
        {
            if (styles == null)
            {
                s_SharedAssetStoreAssetInspector = this;
                styles = new Styles();
            }
            AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
            AssetStoreAsset.PreviewInfo previewInfo = null;
            if (firstAsset != null)
            {
                previewInfo = firstAsset.previewInfo;
            }
            if (firstAsset != null)
            {
                this.target.name = string.Format("Asset Store: {0}", firstAsset.name);
            }
            else
            {
                this.target.name = "Asset Store";
            }
            EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
            bool enabled = GUI.enabled;
            GUI.enabled = (firstAsset != null) && (firstAsset.packageID != 0);
            if (OfflineNoticeEnabled)
            {
                Color color = GUI.color;
                GUI.color = Color.yellow;
                GUILayout.Label("Network is offline", new GUILayoutOption[0]);
                GUI.color = color;
            }
            if (firstAsset != null)
            {
                string name = (firstAsset.className != null) ? firstAsset.className.Split(new char[] { ' ' }, 2)[0] : string.Empty;
                bool flag2 = firstAsset.id == -firstAsset.packageID;
                if (flag2)
                {
                    name = "Package";
                }
                if (firstAsset.HasLivePreview)
                {
                    name = firstAsset.Preview.GetType().Name;
                }
                EditorGUILayout.LabelField("Type", name, new GUILayoutOption[0]);
                if (flag2)
                {
                    this.packageInfoShown = true;
                }
                else
                {
                    EditorGUILayout.Separator();
                    this.packageInfoShown = EditorGUILayout.Foldout(this.packageInfoShown, "Part of package");
                }
                if (this.packageInfoShown)
                {
                    EditorGUILayout.LabelField("Name", (previewInfo != null) ? previewInfo.packageName : "-", new GUILayoutOption[0]);
                    EditorGUILayout.LabelField("Version", (previewInfo != null) ? previewInfo.packageVersion : "-", new GUILayoutOption[0]);
                    string str2 = (previewInfo != null) ? (((firstAsset.price == null) || !(firstAsset.price != string.Empty)) ? "free" : firstAsset.price) : "-";
                    EditorGUILayout.LabelField("Price", str2, new GUILayoutOption[0]);
                    string str3 = ((previewInfo == null) || (previewInfo.packageRating < 0)) ? "-" : (previewInfo.packageRating.ToString() + " of 5");
                    EditorGUILayout.LabelField("Rating", str3, new GUILayoutOption[0]);
                    EditorGUILayout.LabelField("Size", (previewInfo != null) ? intToSizeString(previewInfo.packageSize) : "-", new GUILayoutOption[0]);
                    string str4 = ((previewInfo == null) || (previewInfo.packageAssetCount < 0)) ? "-" : previewInfo.packageAssetCount.ToString();
                    EditorGUILayout.LabelField("Asset count", str4, new GUILayoutOption[0]);
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    EditorGUILayout.PrefixLabel("Web page");
                    bool flag3 = ((previewInfo != null) && (previewInfo.packageShortUrl != null)) && (previewInfo.packageShortUrl != string.Empty);
                    bool flag4 = GUI.enabled;
                    GUI.enabled = flag3;
                    if (GUILayout.Button(!flag3 ? EditorGUIUtility.TempContent("-") : new GUIContent(previewInfo.packageShortUrl, "View in browser"), styles.link, new GUILayoutOption[0]))
                    {
                        Application.OpenURL(previewInfo.packageShortUrl);
                    }
                    if (GUI.enabled)
                    {
                        EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
                    }
                    GUI.enabled = flag4;
                    GUILayout.EndHorizontal();
                    EditorGUILayout.LabelField("Publisher", (previewInfo != null) ? previewInfo.publisherName : "-", new GUILayoutOption[0]);
                }
                if (firstAsset.id != 0)
                {
                    string str5;
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.FlexibleSpace();
                    if ((previewInfo != null) && previewInfo.isDownloadable)
                    {
                        str5 = "Import package";
                    }
                    else
                    {
                        str5 = "Buy for " + firstAsset.price;
                    }
                    bool flag5 = GUI.enabled;
                    bool flag6 = (previewInfo != null) && (previewInfo.buildProgress >= 0f);
                    bool flag7 = (previewInfo != null) && (previewInfo.downloadProgress >= 0f);
                    if ((flag6 || flag7) || (previewInfo == null))
                    {
                        str5 = string.Empty;
                        GUI.enabled = false;
                    }
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(40f), GUILayout.Width(120f) };
                    if (GUILayout.Button(str5, options))
                    {
                        if (previewInfo.isDownloadable)
                        {
                            this.ImportPackage(firstAsset);
                        }
                        else
                        {
                            this.InitiateBuySelected();
                        }
                        GUIUtility.ExitGUI();
                    }
                    if (Event.current.type == EventType.Repaint)
                    {
                        Rect lastRect = GUILayoutUtility.GetLastRect();
                        lastRect.height -= 4f;
                        float width = lastRect.width;
                        lastRect.width = lastRect.height;
                        lastRect.y += 2f;
                        lastRect.x += 2f;
                        if (flag6 || flag7)
                        {
                            lastRect.width = (width - lastRect.height) - 4f;
                            lastRect.x += lastRect.height;
                            EditorGUI.ProgressBar(lastRect, !flag7 ? previewInfo.buildProgress : previewInfo.downloadProgress, !flag7 ? "Building" : "Downloading");
                        }
                    }
                    GUI.enabled = flag5;
                    GUILayout.Space(4f);
                    GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Height(40f), GUILayout.Width(120f) };
                    if (GUILayout.Button("Open Asset Store", optionArray2))
                    {
                        OpenItemInAssetStore(firstAsset);
                        GUIUtility.ExitGUI();
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
                GUILayout.FlexibleSpace();
            }
            EditorWrapper previewEditor = this.previewEditor;
            if (((previewEditor != null) && (firstAsset != null)) && firstAsset.HasLivePreview)
            {
                previewEditor.OnAssetStoreInspectorGUI();
            }
            GUI.enabled = enabled;
            EditorGUILayout.EndVertical();
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            EditorWrapper previewEditor = this.previewEditor;
            if (previewEditor != null)
            {
                previewEditor.OnInteractivePreviewGUI(r, background);
            }
            AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
            if (((firstAsset != null) && !firstAsset.HasLivePreview) && !string.IsNullOrEmpty(firstAsset.dynamicPreviewURL))
            {
                GUIContent statusWheel = StatusWheel;
                r.y += (r.height - statusWheel.image.height) / 2f;
                r.x += (r.width - statusWheel.image.width) / 2f;
                GUI.Label(r, StatusWheel);
                base.Repaint();
            }
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (this.m_PreviewObject != null)
            {
                EditorWrapper previewEditor = this.previewEditor;
                if ((previewEditor != null) && (this.m_PreviewObject is AnimationClip))
                {
                    previewEditor.OnPreviewGUI(r, background);
                }
                else
                {
                    this.OnInteractivePreviewGUI(r, background);
                }
            }
        }

        public override void OnPreviewSettings()
        {
            AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
            if (firstAsset != null)
            {
                EditorWrapper previewEditor = this.previewEditor;
                if ((previewEditor != null) && firstAsset.HasLivePreview)
                {
                    previewEditor.OnPreviewSettings();
                }
            }
        }

        public static void OpenItemInAssetStore(AssetStoreAsset activeAsset)
        {
            if (activeAsset.id != 0)
            {
                AssetStore.Open(string.Format("content/{0}?assetID={1}", activeAsset.packageID, activeAsset.id));
                Analytics.Track(string.Format("/AssetStore/ViewInStore/{0}/{1}", activeAsset.packageID, activeAsset.id));
            }
        }

        public void Update()
        {
            AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
            bool flag = ((firstAsset != null) && (firstAsset.previewInfo != null)) && ((firstAsset.previewInfo.buildProgress >= 0f) || (firstAsset.previewInfo.downloadProgress >= 0f));
            if ((((firstAsset == null) && (this.lastAssetID != 0)) || ((firstAsset != null) && (this.lastAssetID != firstAsset.id))) || flag)
            {
                this.lastAssetID = (firstAsset != null) ? firstAsset.id : 0;
                base.Repaint();
            }
            if ((firstAsset != null) && (firstAsset.previewBundle != null))
            {
                firstAsset.previewBundle.Unload(false);
                firstAsset.previewBundle = null;
                base.Repaint();
            }
        }

        public static AssetStoreAssetInspector Instance
        {
            get
            {
                if (s_SharedAssetStoreAssetInspector == null)
                {
                    s_SharedAssetStoreAssetInspector = ScriptableObject.CreateInstance<AssetStoreAssetInspector>();
                    s_SharedAssetStoreAssetInspector.hideFlags = HideFlags.HideAndDontSave;
                }
                return s_SharedAssetStoreAssetInspector;
            }
        }

        public static bool OfflineNoticeEnabled
        {
            [CompilerGenerated]
            get
            {
                return <OfflineNoticeEnabled>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                <OfflineNoticeEnabled>k__BackingField = value;
            }
        }

        internal static PaymentAvailability paymentAvailability
        {
            get
            {
                if (AssetStoreClient.LoggedOut())
                {
                    m_PaymentAvailability = PaymentAvailability.AnonymousUser;
                }
                return m_PaymentAvailability;
            }
            set
            {
                if (AssetStoreClient.LoggedOut())
                {
                    m_PaymentAvailability = PaymentAvailability.AnonymousUser;
                }
                else
                {
                    m_PaymentAvailability = value;
                }
            }
        }

        private EditorWrapper previewEditor
        {
            get
            {
                AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
                if (firstAsset == null)
                {
                    return null;
                }
                Object preview = firstAsset.Preview;
                if (preview == null)
                {
                    return null;
                }
                if (preview != this.m_PreviewObject)
                {
                    this.m_PreviewObject = preview;
                    if (this.m_PreviewEditor != null)
                    {
                        this.m_PreviewEditor.Dispose();
                    }
                    this.m_PreviewEditor = EditorWrapper.Make(this.m_PreviewObject, EditorFeatures.PreviewGUI);
                }
                return this.m_PreviewEditor;
            }
        }

        private static GUIContent StatusWheel
        {
            get
            {
                if (sStatusWheel == null)
                {
                    sStatusWheel = new GUIContent[12];
                    for (int i = 0; i < 12; i++)
                    {
                        sStatusWheel[i] = new GUIContent { image = EditorGUIUtility.LoadIcon("WaitSpin" + i.ToString("00")) };
                    }
                }
                int index = (int) Mathf.Repeat(Time.realtimeSinceStartup * 10f, 11.99f);
                return sStatusWheel[index];
            }
        }

        [CompilerGenerated]
        private sealed class <LoginAndImport>c__AnonStorey7D
        {
            internal AssetStoreAsset asset;

            internal void <>m__136(string errorMessage)
            {
                if (errorMessage == null)
                {
                    AssetStoreAssetSelection.RefreshFromServer(() => AssetStoreInstaBuyWindow.ShowAssetStoreInstaBuyWindowBuilding(this.asset));
                }
            }

            internal void <>m__138()
            {
                AssetStoreInstaBuyWindow.ShowAssetStoreInstaBuyWindowBuilding(this.asset);
            }
        }

        internal enum PaymentAvailability
        {
            BasketNotEmpty,
            ServiceDisabled,
            AnonymousUser,
            Ok
        }

        private class Styles
        {
            public GUIContent assetStoreLogo = EditorGUIUtility.IconContent("WelcomeScreen.AssetStoreLogo");
            public GUIStyle link = new GUIStyle(EditorStyles.label);

            public Styles()
            {
                this.link.normal.textColor = new Color(0.26f, 0.51f, 0.75f, 1f);
            }
        }
    }
}

