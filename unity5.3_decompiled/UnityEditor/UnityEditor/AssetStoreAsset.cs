namespace UnityEditor
{
    using System;
    using UnityEngine;

    public sealed class AssetStoreAsset
    {
        public string className;
        public string displayName;
        internal bool disposed = false;
        public string dynamicPreviewURL;
        public int id;
        public string name;
        public int packageID;
        internal Object previewAsset;
        internal AssetBundle previewBundle;
        internal AssetBundleCreateRequest previewBundleRequest;
        public Texture2D previewImage;
        internal PreviewInfo previewInfo;
        public string price;
        public string staticPreviewURL;

        public void Dispose()
        {
            if (this.previewImage != null)
            {
                Object.DestroyImmediate(this.previewImage);
                this.previewImage = null;
            }
            if (this.previewBundle != null)
            {
                this.previewBundle.Unload(true);
                this.previewBundle = null;
                this.previewAsset = null;
            }
            this.disposed = true;
        }

        internal string DebugString
        {
            get
            {
                object[] args = new object[7];
                args[0] = this.id;
                if (this.name == null)
                {
                }
                args[1] = "N/A";
                if (this.staticPreviewURL == null)
                {
                }
                args[2] = "N/A";
                if (this.dynamicPreviewURL == null)
                {
                }
                args[3] = "N/A";
                if (this.className == null)
                {
                }
                args[4] = "N/A";
                args[5] = this.price;
                args[6] = this.packageID;
                string str = string.Format("id: {0}\nname: {1}\nstaticPreviewURL: {2}\ndynamicPreviewURL: {3}\nclassName: {4}\nprice: {5}\npackageID: {6}", args);
                if (this.previewInfo == null)
                {
                    return str;
                }
                object[] objArray2 = new object[14];
                if (this.previewInfo.packageName == null)
                {
                }
                objArray2[0] = "N/A";
                if (this.previewInfo.packageShortUrl == null)
                {
                }
                objArray2[1] = "N/A";
                objArray2[2] = this.previewInfo.packageSize;
                if (this.previewInfo.packageVersion == null)
                {
                }
                objArray2[3] = "N/A";
                objArray2[4] = this.previewInfo.packageRating;
                objArray2[5] = this.previewInfo.packageAssetCount;
                objArray2[6] = this.previewInfo.isPurchased;
                objArray2[7] = this.previewInfo.isDownloadable;
                if (this.previewInfo.publisherName == null)
                {
                }
                objArray2[8] = "N/A";
                if (this.previewInfo.encryptionKey == null)
                {
                }
                objArray2[9] = "N/A";
                if (this.previewInfo.packageUrl == null)
                {
                }
                objArray2[10] = "N/A";
                objArray2[11] = this.previewInfo.buildProgress;
                objArray2[12] = this.previewInfo.downloadProgress;
                if (this.previewInfo.categoryName == null)
                {
                }
                objArray2[13] = "N/A";
                return (str + string.Format("previewInfo {{\n    packageName: {0}\n    packageShortUrl: {1}\n    packageSize: {2}\n    packageVersion: {3}\n    packageRating: {4}\n    packageAssetCount: {5}\n    isPurchased: {6}\n    isDownloadable: {7}\n    publisherName: {8}\n    encryptionKey: {9}\n    packageUrl: {10}\n    buildProgress: {11}\n    downloadProgress: {12}\n    categoryName: {13}\n}}", objArray2));
            }
        }

        public bool HasLivePreview
        {
            get
            {
                return (this.previewAsset != null);
            }
        }

        public Object Preview
        {
            get
            {
                if (this.previewAsset != null)
                {
                    return this.previewAsset;
                }
                return this.previewImage;
            }
        }

        internal class PreviewInfo
        {
            public float buildProgress;
            public string categoryName;
            public float downloadProgress;
            public string encryptionKey;
            public bool isDownloadable;
            public bool isPurchased;
            public int packageAssetCount;
            public string packageName;
            public int packageRating;
            public string packageShortUrl;
            public int packageSize;
            public string packageUrl;
            public string packageVersion;
            public string publisherName;
        }
    }
}

