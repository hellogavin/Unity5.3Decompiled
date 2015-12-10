namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class AssetStoreInstaBuyWindow : EditorWindow
    {
        private const double kBuildPollInterval = 2.0;
        private const int kMaxPolls = 150;
        private const int kStandardHeight = 160;
        private AssetStoreAsset m_Asset;
        private int m_BuildAttempts;
        private string m_Message = string.Empty;
        private double m_NextAllowedBuildRequestTime;
        private string m_Password = string.Empty;
        private string m_PaymentMethodCard;
        private string m_PaymentMethodExpire;
        private string m_PriceText;
        private string m_PurchaseMessage;
        private PurchaseStatus m_Purchasing;
        private static GUIContent s_AssetStoreLogo;

        private void BuildPackage()
        {
            AssetStoreAsset.PreviewInfo previewInfo = this.m_Asset.previewInfo;
            if (previewInfo != null)
            {
                if (this.m_BuildAttempts++ > 150)
                {
                    EditorUtility.DisplayDialog("Building timed out", "Timed out during building of package", "Close");
                    base.Close();
                }
                else
                {
                    previewInfo.downloadProgress = -1f;
                    this.m_Purchasing = PurchaseStatus.Building;
                    AssetStoreClient.BuildPackage(this.m_Asset, delegate (BuildPackageResult result) {
                        if (this != null)
                        {
                            if (result.error != null)
                            {
                                Debug.Log(result.error);
                                if (EditorUtility.DisplayDialog("Error building package", "The server was unable to build the package. Please re-import.", "Ok"))
                                {
                                    base.Close();
                                }
                            }
                            else
                            {
                                if (((this.m_Asset == null) || (this.m_Purchasing != PurchaseStatus.Building)) || (result.packageID != this.m_Asset.packageID))
                                {
                                    base.Close();
                                }
                                string packageUrl = result.asset.previewInfo.packageUrl;
                                if ((packageUrl != null) && (packageUrl != string.Empty))
                                {
                                    this.DownloadPackage();
                                }
                                else
                                {
                                    this.m_Purchasing = PurchaseStatus.StartBuild;
                                }
                                base.Repaint();
                            }
                        }
                    });
                }
            }
        }

        private void CompletePurchase()
        {
            this.m_Message = string.Empty;
            string password = this.m_Password;
            this.m_Password = string.Empty;
            this.m_Purchasing = PurchaseStatus.InProgress;
            AssetStoreClient.DirectPurchase(this.m_Asset.packageID, password, delegate (PurchaseResult result) {
                this.m_Purchasing = PurchaseStatus.Init;
                if (result.error != null)
                {
                    this.m_Purchasing = PurchaseStatus.Declined;
                    this.m_Message = "An error occured while completing you purhase.";
                    base.Close();
                }
                string str = null;
                switch (result.status)
                {
                    case PurchaseResult.Status.BasketNotEmpty:
                        this.m_Message = "Something else has been put in our Asset Store basket while doing this purchase.";
                        this.m_Purchasing = PurchaseStatus.Declined;
                        break;

                    case PurchaseResult.Status.ServiceDisabled:
                        this.m_Message = "Single click purchase has been disabled while doing this purchase.";
                        this.m_Purchasing = PurchaseStatus.Declined;
                        break;

                    case PurchaseResult.Status.AnonymousUser:
                        this.m_Message = "You have been logged out from somewhere else while doing this purchase.";
                        this.m_Purchasing = PurchaseStatus.Declined;
                        break;

                    case PurchaseResult.Status.PasswordMissing:
                        this.m_Message = result.message;
                        base.Repaint();
                        break;

                    case PurchaseResult.Status.PasswordWrong:
                        this.m_Message = result.message;
                        base.Repaint();
                        break;

                    case PurchaseResult.Status.PurchaseDeclined:
                        this.m_Purchasing = PurchaseStatus.Declined;
                        if (result.message != null)
                        {
                            this.m_Message = result.message;
                        }
                        base.Repaint();
                        break;

                    case PurchaseResult.Status.Ok:
                        this.m_Purchasing = PurchaseStatus.Complete;
                        if (result.message != null)
                        {
                            this.m_Message = result.message;
                        }
                        base.Repaint();
                        break;
                }
                if (str != null)
                {
                    EditorUtility.DisplayDialog("Purchase failed", str + " This purchase has been cancelled.", "Add this item to basket", "Cancel");
                }
            });
            Analytics.Track(string.Format("/AssetStore/InstaBuy/{0}/{1}", this.m_Asset.packageID, this.m_Asset.id));
        }

        private void DownloadingGUI()
        {
            AssetStoreAsset.PreviewInfo previewInfo = this.m_Asset.previewInfo;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(5f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            GUILayout.Label(s_AssetStoreLogo, GUIStyle.none, options);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            string text = "Importing";
            GUILayout.Label(text, EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUILayout.Label("Package: " + previewInfo.packageName, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
            GUILayout.Label("    ", new GUILayoutOption[0]);
            if (Event.current.type == EventType.Repaint)
            {
                Rect lastRect = GUILayoutUtility.GetLastRect();
                lastRect.height++;
                bool flag = previewInfo.downloadProgress >= 0f;
                EditorGUI.ProgressBar(lastRect, !flag ? previewInfo.buildProgress : previewInfo.downloadProgress, !flag ? "Building" : "Downloading");
            }
            GUILayout.EndVertical();
            GUILayout.Space(5f);
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Abort", new GUILayoutOption[0]))
            {
                base.Close();
            }
            GUILayout.Space(5f);
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
        }

        private void DownloadPackage()
        {
            <DownloadPackage>c__AnonStorey55 storey = new <DownloadPackage>c__AnonStorey55 {
                <>f__this = this,
                item = this.m_Asset.previewInfo
            };
            this.m_Purchasing = PurchaseStatus.Downloading;
            storey.item.downloadProgress = 0f;
            storey.item.buildProgress = -1f;
            AssetStoreContext.Download(this.m_Asset.packageID.ToString(), storey.item.packageUrl, storey.item.encryptionKey, storey.item.packageName, storey.item.publisherName, storey.item.categoryName, new AssetStoreUtils.DownloadDoneCallback(storey.<>m__96));
        }

        private static void LoadLogos()
        {
            if (s_AssetStoreLogo == null)
            {
                s_AssetStoreLogo = EditorGUIUtility.IconContent("WelcomeScreen.AssetStoreLogo");
            }
        }

        public void OnDisable()
        {
            AssetStoreAsset.PreviewInfo info = (this.m_Asset != null) ? this.m_Asset.previewInfo : null;
            if (info != null)
            {
                info.downloadProgress = -1f;
                info.buildProgress = -1f;
            }
            AssetStoreUtils.UnRegisterDownloadDelegate(this);
            this.m_Purchasing = PurchaseStatus.Init;
        }

        public void OnDownloadProgress(string id, string message, int bytes, int total)
        {
            AssetStoreAsset.PreviewInfo info = (this.m_Asset != null) ? this.m_Asset.previewInfo : null;
            if ((info != null) && (this.m_Asset.packageID.ToString() == id))
            {
                if ((message == "downloading") || (message == "connecting"))
                {
                    info.downloadProgress = ((float) bytes) / ((float) total);
                }
                else
                {
                    info.downloadProgress = -1f;
                }
                base.Repaint();
            }
        }

        private void OnEnable()
        {
            AssetStoreUtils.RegisterDownloadDelegate(this);
        }

        public void OnGUI()
        {
            LoadLogos();
            if (this.m_Asset != null)
            {
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                GUILayout.Space(10f);
                switch (this.m_Purchasing)
                {
                    case PurchaseStatus.Init:
                        this.PasswordGUI();
                        break;

                    case PurchaseStatus.InProgress:
                        if (this.m_Purchasing == PurchaseStatus.InProgress)
                        {
                            GUI.enabled = false;
                        }
                        this.PasswordGUI();
                        break;

                    case PurchaseStatus.Declined:
                        this.PurchaseDeclinedGUI();
                        break;

                    case PurchaseStatus.Complete:
                        this.PurchaseSuccessGUI();
                        break;

                    case PurchaseStatus.StartBuild:
                    case PurchaseStatus.Building:
                    case PurchaseStatus.Downloading:
                        this.DownloadingGUI();
                        break;
                }
                GUILayout.EndVertical();
            }
        }

        public void OnInspectorUpdate()
        {
            if ((this.m_Purchasing == PurchaseStatus.StartBuild) && (this.m_NextAllowedBuildRequestTime <= EditorApplication.timeSinceStartup))
            {
                this.m_NextAllowedBuildRequestTime = EditorApplication.timeSinceStartup + 2.0;
                this.BuildPackage();
            }
        }

        private void PasswordGUI()
        {
            AssetStoreAsset.PreviewInfo previewInfo = this.m_Asset.previewInfo;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(5f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            GUILayout.Label(s_AssetStoreLogo, GUIStyle.none, options);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Label("Complete purchase by entering your AssetStore password", EditorStyles.boldLabel, new GUILayoutOption[0]);
            bool flag = (this.m_PurchaseMessage != null) && (this.m_PurchaseMessage != string.Empty);
            bool flag2 = (this.m_Message != null) && (this.m_Message != string.Empty);
            float height = (160 + (!flag ? 0 : 20)) + (!flag2 ? 0 : 20);
            if (height != base.position.height)
            {
                base.position = new Rect(base.position.x, base.position.y, base.position.width, height);
            }
            if (flag)
            {
                GUILayout.Label(this.m_PurchaseMessage, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
            }
            if (flag2)
            {
                Color color = GUI.color;
                GUI.color = Color.red;
                GUILayout.Label(this.m_Message, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
                GUI.color = color;
            }
            GUILayout.Label("Package: " + previewInfo.packageName, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
            GUILayout.Label(string.Format("Credit card: {0} (expires {1})", this.m_PaymentMethodCard, this.m_PaymentMethodExpire), EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
            GUILayout.Space(8f);
            EditorGUILayout.LabelField("Amount", this.m_PriceText, new GUILayoutOption[0]);
            this.m_Password = EditorGUILayout.PasswordField("Password", this.m_Password, new GUILayoutOption[0]);
            GUILayout.EndVertical();
            GUILayout.Space(5f);
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(8f);
            if (GUILayout.Button("Just put to basket...", new GUILayoutOption[0]))
            {
                AssetStore.Open(string.Format("content/{0}/basketpurchase", this.m_Asset.packageID));
                Analytics.Track(string.Format("/AssetStore/PutToBasket/{0}/{1}", this.m_Asset.packageID, this.m_Asset.id));
                this.m_Asset = null;
                base.Close();
                GUIUtility.ExitGUI();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Cancel", new GUILayoutOption[0]))
            {
                Analytics.Track(string.Format("/AssetStore/CancelInstaBuy/{0}/{1}", this.m_Asset.packageID, this.m_Asset.id));
                this.m_Asset = null;
                base.Close();
                GUIUtility.ExitGUI();
            }
            GUILayout.Space(5f);
            if (GUILayout.Button("Complete purchase", new GUILayoutOption[0]))
            {
                this.CompletePurchase();
            }
            GUILayout.Space(5f);
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
        }

        private void PurchaseDeclinedGUI()
        {
            AssetStoreAsset.PreviewInfo previewInfo = this.m_Asset.previewInfo;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(5f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            GUILayout.Label(s_AssetStoreLogo, GUIStyle.none, options);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Label("Purchase declined", EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUILayout.Label("No money has been drawn from you credit card", new GUILayoutOption[0]);
            bool flag = (this.m_Message != null) && (this.m_Message != string.Empty);
            float height = 160 + (!flag ? 0 : 20);
            if (height != base.position.height)
            {
                base.position = new Rect(base.position.x, base.position.y, base.position.width, height);
            }
            if (flag)
            {
                GUILayout.Label(this.m_Message, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
            }
            GUILayout.Label("Package: " + previewInfo.packageName, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
            GUILayout.EndVertical();
            GUILayout.Space(5f);
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(8f);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close", new GUILayoutOption[0]))
            {
                Analytics.Track(string.Format("/AssetStore/DeclinedAbort/{0}/{1}", this.m_Asset.packageID, this.m_Asset.id));
                this.m_Asset = null;
                base.Close();
            }
            GUILayout.Space(5f);
            if (GUILayout.Button("Put to basket", new GUILayoutOption[0]))
            {
                AssetStore.Open(string.Format("content/{0}/basketpurchase", this.m_Asset.packageID));
                Analytics.Track(string.Format("/AssetStore/DeclinedPutToBasket/{0}/{1}", this.m_Asset.packageID, this.m_Asset.id));
                this.m_Asset = null;
                base.Close();
            }
            GUILayout.Space(5f);
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
        }

        private void PurchaseSuccessGUI()
        {
            AssetStoreAsset.PreviewInfo previewInfo = this.m_Asset.previewInfo;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(5f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            GUILayout.Label(s_AssetStoreLogo, GUIStyle.none, options);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Label("Purchase completed succesfully", EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUILayout.Label("You will receive a receipt in your email soon.", new GUILayoutOption[0]);
            bool flag = (this.m_Message != null) && (this.m_Message != string.Empty);
            float height = 160 + (!flag ? 0 : 20);
            if (height != base.position.height)
            {
                base.position = new Rect(base.position.x, base.position.y, base.position.width, height);
            }
            if (flag)
            {
                GUILayout.Label(this.m_Message, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
            }
            GUILayout.Label("Package: " + previewInfo.packageName, EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
            GUILayout.EndVertical();
            GUILayout.Space(5f);
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(8f);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close", new GUILayoutOption[0]))
            {
                Analytics.Track(string.Format("/AssetStore/PurchaseOk/{0}/{1}", this.m_Asset.packageID, this.m_Asset.id));
                this.m_Asset = null;
                base.Close();
            }
            GUILayout.Space(5f);
            if (GUILayout.Button("Import package", new GUILayoutOption[0]))
            {
                Analytics.Track(string.Format("/AssetStore/PurchaseOkImport/{0}/{1}", this.m_Asset.packageID, this.m_Asset.id));
                this.m_BuildAttempts = 1;
                this.m_Asset.previewInfo.buildProgress = 0f;
                this.m_Purchasing = PurchaseStatus.StartBuild;
            }
            GUILayout.Space(5f);
            GUILayout.EndHorizontal();
            GUILayout.Space(5f);
        }

        public static AssetStoreInstaBuyWindow ShowAssetStoreInstaBuyWindow(AssetStoreAsset asset, string purchaseMessage, string paymentMethodCard, string paymentMethodExpire, string priceText)
        {
            AssetStoreInstaBuyWindow window = EditorWindow.GetWindowWithRect<AssetStoreInstaBuyWindow>(new Rect(100f, 100f, 400f, 160f), true, "Buy package from Asset Store");
            if (window.m_Purchasing != PurchaseStatus.Init)
            {
                EditorUtility.DisplayDialog("Download in progress", "There is already a package download in progress. You can only have one download running at a time", "Close");
                return window;
            }
            window.position = new Rect(100f, 100f, 400f, 160f);
            window.m_Parent.window.m_DontSaveToLayout = true;
            window.m_Asset = asset;
            window.m_Password = string.Empty;
            window.m_Message = string.Empty;
            window.m_Purchasing = PurchaseStatus.Init;
            window.m_NextAllowedBuildRequestTime = 0.0;
            window.m_BuildAttempts = 0;
            window.m_PurchaseMessage = purchaseMessage;
            window.m_PaymentMethodCard = paymentMethodCard;
            window.m_PaymentMethodExpire = paymentMethodExpire;
            window.m_PriceText = priceText;
            Analytics.Track(string.Format("/AssetStore/ShowInstaBuy/{0}/{1}", window.m_Asset.packageID, window.m_Asset.id));
            return window;
        }

        public static void ShowAssetStoreInstaBuyWindowBuilding(AssetStoreAsset asset)
        {
            AssetStoreInstaBuyWindow window = ShowAssetStoreInstaBuyWindow(asset, string.Empty, string.Empty, string.Empty, string.Empty);
            if (window.m_Purchasing != PurchaseStatus.Init)
            {
                EditorUtility.DisplayDialog("Download in progress", "There is already a package download in progress. You can only have one download running at a time", "Close");
            }
            else
            {
                window.m_Purchasing = PurchaseStatus.StartBuild;
                window.m_BuildAttempts = 1;
                asset.previewInfo.buildProgress = 0f;
                Analytics.Track(string.Format("/AssetStore/ShowInstaFree/{0}/{1}", window.m_Asset.packageID, window.m_Asset.id));
            }
        }

        [CompilerGenerated]
        private sealed class <DownloadPackage>c__AnonStorey55
        {
            internal AssetStoreInstaBuyWindow <>f__this;
            internal AssetStoreAsset.PreviewInfo item;

            internal void <>m__96(string id, string status, int bytes, int total)
            {
                if (this.<>f__this != null)
                {
                    this.item.downloadProgress = -1f;
                    if (status != "ok")
                    {
                        object[] args = new object[] { this.item.packageName, id };
                        Debug.LogErrorFormat("Error downloading package {0} ({1})", args);
                        Debug.LogError(status);
                        this.<>f__this.Close();
                    }
                    else
                    {
                        if (((this.<>f__this.m_Asset == null) || (this.<>f__this.m_Purchasing != AssetStoreInstaBuyWindow.PurchaseStatus.Downloading)) || (id != this.<>f__this.m_Asset.packageID.ToString()))
                        {
                            this.<>f__this.Close();
                        }
                        if (!AssetStoreContext.OpenPackageInternal(id))
                        {
                            object[] objArray2 = new object[] { this.item.packageName, id };
                            Debug.LogErrorFormat("Error importing package {0} ({1})", objArray2);
                        }
                        this.<>f__this.Close();
                    }
                }
            }
        }

        private enum PurchaseStatus
        {
            Init,
            InProgress,
            Declined,
            Complete,
            StartBuild,
            Building,
            Downloading
        }
    }
}

