namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal static class AssetStoreAssetSelection
    {
        internal static Dictionary<int, AssetStoreAsset> s_SelectedAssets;

        public static void AddAsset(AssetStoreAsset searchResult, Texture2D placeholderPreviewImage)
        {
            <AddAsset>c__AnonStorey4B storeyb = new <AddAsset>c__AnonStorey4B {
                searchResult = searchResult
            };
            if (placeholderPreviewImage != null)
            {
                storeyb.searchResult.previewImage = ScaleImage(placeholderPreviewImage, 0x100, 0x100);
            }
            storeyb.searchResult.previewInfo = null;
            storeyb.searchResult.previewBundleRequest = null;
            if (!string.IsNullOrEmpty(storeyb.searchResult.dynamicPreviewURL) && (storeyb.searchResult.previewBundle == null))
            {
                <AddAsset>c__AnonStorey4A storeya = new <AddAsset>c__AnonStorey4A {
                    <>f__ref$75 = storeyb
                };
                storeyb.searchResult.disposed = false;
                storeya.client = new AsyncHTTPClient(storeyb.searchResult.dynamicPreviewURL);
                storeya.client.doneCallback = new AsyncHTTPClient.DoneCallback(storeya.<>m__88);
                storeya.client.Begin();
            }
            else if (!string.IsNullOrEmpty(storeyb.searchResult.staticPreviewURL))
            {
                DownloadStaticPreview(storeyb.searchResult);
            }
            AddAssetInternal(storeyb.searchResult);
            RefreshFromServer(null);
        }

        internal static void AddAssetInternal(AssetStoreAsset searchResult)
        {
            if (s_SelectedAssets == null)
            {
                s_SelectedAssets = new Dictionary<int, AssetStoreAsset>();
            }
            s_SelectedAssets[searchResult.id] = searchResult;
        }

        public static void Clear()
        {
            if (s_SelectedAssets != null)
            {
                foreach (KeyValuePair<int, AssetStoreAsset> pair in s_SelectedAssets)
                {
                    pair.Value.Dispose();
                }
                s_SelectedAssets.Clear();
            }
        }

        public static bool ContainsAsset(int id)
        {
            return ((s_SelectedAssets != null) && s_SelectedAssets.ContainsKey(id));
        }

        private static void DownloadStaticPreview(AssetStoreAsset searchResult)
        {
            <DownloadStaticPreview>c__AnonStorey4D storeyd;
            storeyd = new <DownloadStaticPreview>c__AnonStorey4D {
                searchResult = searchResult,
                client = new AsyncHTTPClient(storeyd.searchResult.staticPreviewURL)
            };
            storeyd.client.doneCallback = new AsyncHTTPClient.DoneCallback(storeyd.<>m__89);
            storeyd.client.Begin();
        }

        public static AssetStoreAsset GetFirstAsset()
        {
            if (s_SelectedAssets == null)
            {
                return null;
            }
            Dictionary<int, AssetStoreAsset>.Enumerator enumerator = s_SelectedAssets.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return null;
            }
            KeyValuePair<int, AssetStoreAsset> current = enumerator.Current;
            return current.Value;
        }

        public static void RefreshFromServer(AssetsRefreshed callback)
        {
            <RefreshFromServer>c__AnonStorey4E storeye = new <RefreshFromServer>c__AnonStorey4E {
                callback = callback
            };
            if (s_SelectedAssets.Count != 0)
            {
                List<AssetStoreAsset> assets = new List<AssetStoreAsset>();
                foreach (KeyValuePair<int, AssetStoreAsset> pair in s_SelectedAssets)
                {
                    assets.Add(pair.Value);
                }
                AssetStoreClient.AssetsInfo(assets, new AssetStoreResultBase<AssetStoreAssetsInfo>.Callback(storeye.<>m__8A));
            }
        }

        private static Texture2D ScaleImage(Texture2D source, int w, int h)
        {
            if ((source.width % 4) != 0)
            {
                return null;
            }
            Texture2D textured = new Texture2D(w, h, TextureFormat.RGB24, false, true);
            Color[] pixels = textured.GetPixels(0);
            double num = 1.0 / ((double) w);
            double num2 = 1.0 / ((double) h);
            double num3 = 0.0;
            double num4 = 0.0;
            int index = 0;
            for (int i = 0; i < h; i++)
            {
                int num7 = 0;
                while (num7 < w)
                {
                    pixels[index] = source.GetPixelBilinear((float) num3, (float) num4);
                    num3 += num;
                    num7++;
                    index++;
                }
                num3 = 0.0;
                num4 += num2;
            }
            textured.SetPixels(pixels, 0);
            textured.Apply();
            return textured;
        }

        public static int Count
        {
            get
            {
                return ((s_SelectedAssets != null) ? s_SelectedAssets.Count : 0);
            }
        }

        public static bool Empty
        {
            get
            {
                return ((s_SelectedAssets != null) ? (s_SelectedAssets.Count == 0) : true);
            }
        }

        [CompilerGenerated]
        private sealed class <AddAsset>c__AnonStorey4A
        {
            internal AssetStoreAssetSelection.<AddAsset>c__AnonStorey4B <>f__ref$75;
            internal AsyncHTTPClient client;

            internal void <>m__88(AsyncHTTPClient c)
            {
                if (!this.client.IsSuccess())
                {
                    Console.WriteLine("Error downloading dynamic preview: " + this.client.text);
                    this.<>f__ref$75.searchResult.dynamicPreviewURL = null;
                    AssetStoreAssetSelection.DownloadStaticPreview(this.<>f__ref$75.searchResult);
                }
                else
                {
                    AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
                    if ((!this.<>f__ref$75.searchResult.disposed && (firstAsset != null)) && (this.<>f__ref$75.searchResult.id == firstAsset.id))
                    {
                        try
                        {
                            <AddAsset>c__AnonStorey4C storeyc = new <AddAsset>c__AnonStorey4C {
                                <>f__ref$75 = this.<>f__ref$75,
                                cr = AssetBundle.LoadFromMemoryAsync(c.bytes)
                            };
                            storeyc.cr.DisableCompatibilityChecks();
                            this.<>f__ref$75.searchResult.previewBundleRequest = storeyc.cr;
                            storeyc.callback = null;
                            storeyc.startTime = EditorApplication.timeSinceStartup;
                            storeyc.callback = new EditorApplication.CallbackFunction(storeyc.<>m__8B);
                            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.update, storeyc.callback);
                        }
                        catch (Exception exception)
                        {
                            Console.Write(exception.Message);
                            Debug.Log(exception.Message);
                        }
                    }
                }
            }

            private sealed class <AddAsset>c__AnonStorey4C
            {
                internal AssetStoreAssetSelection.<AddAsset>c__AnonStorey4B <>f__ref$75;
                internal EditorApplication.CallbackFunction callback;
                internal AssetBundleCreateRequest cr;
                internal double startTime;

                internal void <>m__8B()
                {
                    AssetStoreUtils.UpdatePreloading();
                    if (!this.cr.isDone)
                    {
                        if ((EditorApplication.timeSinceStartup - this.startTime) > 10.0)
                        {
                            EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, this.callback);
                            if (this.<>f__ref$75.searchResult.dynamicPreviewURL == null)
                            {
                            }
                            Console.WriteLine("Timed out fetch live preview bundle " + "<n/a>");
                        }
                    }
                    else
                    {
                        EditorApplication.update = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.update, this.callback);
                        AssetStoreAsset firstAsset = AssetStoreAssetSelection.GetFirstAsset();
                        if ((!this.<>f__ref$75.searchResult.disposed && (firstAsset != null)) && (this.<>f__ref$75.searchResult.id == firstAsset.id))
                        {
                            this.<>f__ref$75.searchResult.previewBundle = this.cr.assetBundle;
                            if ((this.cr.assetBundle == null) || (this.cr.assetBundle.mainAsset == null))
                            {
                                this.<>f__ref$75.searchResult.dynamicPreviewURL = null;
                                AssetStoreAssetSelection.DownloadStaticPreview(this.<>f__ref$75.searchResult);
                            }
                            else
                            {
                                this.<>f__ref$75.searchResult.previewAsset = this.<>f__ref$75.searchResult.previewBundle.mainAsset;
                            }
                        }
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <AddAsset>c__AnonStorey4B
        {
            internal AssetStoreAsset searchResult;
        }

        [CompilerGenerated]
        private sealed class <DownloadStaticPreview>c__AnonStorey4D
        {
            internal AsyncHTTPClient client;
            internal AssetStoreAsset searchResult;

            internal void <>m__89(AsyncHTTPClient c)
            {
                if (!this.client.IsSuccess())
                {
                    Console.WriteLine("Error downloading static preview: " + this.client.text);
                }
                else
                {
                    Texture2D texture = c.texture;
                    Texture2D outimage = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false, true);
                    AssetStorePreviewManager.ScaleImage(outimage.width, outimage.height, texture, outimage, null);
                    this.searchResult.previewImage = outimage;
                    Object.DestroyImmediate(texture);
                    AssetStoreAssetInspector.Instance.Repaint();
                }
            }
        }

        [CompilerGenerated]
        private sealed class <RefreshFromServer>c__AnonStorey4E
        {
            internal AssetStoreAssetSelection.AssetsRefreshed callback;

            internal void <>m__8A(AssetStoreAssetsInfo results)
            {
                AssetStoreAssetInspector.paymentAvailability = AssetStoreAssetInspector.PaymentAvailability.ServiceDisabled;
                if ((results.error != null) && (results.error != string.Empty))
                {
                    Console.WriteLine("Error performing Asset Store Info search: " + results.error);
                    AssetStoreAssetInspector.OfflineNoticeEnabled = true;
                    if (this.callback != null)
                    {
                        this.callback();
                    }
                }
                else
                {
                    AssetStoreAssetInspector.OfflineNoticeEnabled = false;
                    if (results.status == AssetStoreAssetsInfo.Status.Ok)
                    {
                        AssetStoreAssetInspector.paymentAvailability = AssetStoreAssetInspector.PaymentAvailability.Ok;
                    }
                    else if (results.status == AssetStoreAssetsInfo.Status.BasketNotEmpty)
                    {
                        AssetStoreAssetInspector.paymentAvailability = AssetStoreAssetInspector.PaymentAvailability.BasketNotEmpty;
                    }
                    else if (results.status == AssetStoreAssetsInfo.Status.AnonymousUser)
                    {
                        AssetStoreAssetInspector.paymentAvailability = AssetStoreAssetInspector.PaymentAvailability.AnonymousUser;
                    }
                    AssetStoreAssetInspector.s_PurchaseMessage = results.message;
                    AssetStoreAssetInspector.s_PaymentMethodCard = results.paymentMethodCard;
                    AssetStoreAssetInspector.s_PaymentMethodExpire = results.paymentMethodExpire;
                    AssetStoreAssetInspector.s_PriceText = results.priceText;
                    AssetStoreAssetInspector.Instance.Repaint();
                    if (this.callback != null)
                    {
                        this.callback();
                    }
                }
            }
        }

        public delegate void AssetsRefreshed();
    }
}

