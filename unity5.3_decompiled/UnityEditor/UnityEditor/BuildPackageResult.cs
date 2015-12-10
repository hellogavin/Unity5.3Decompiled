namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditorInternal;
    using UnityEngine;

    internal class BuildPackageResult : AssetStoreResultBase<BuildPackageResult>
    {
        internal AssetStoreAsset asset;
        internal int packageID;

        internal BuildPackageResult(AssetStoreAsset asset, AssetStoreResultBase<BuildPackageResult>.Callback c) : base(c)
        {
            this.asset = asset;
            this.packageID = -1;
        }

        protected override void Parse(Dictionary<string, JSONValue> dict)
        {
            dict = dict["download"].AsDict();
            this.packageID = int.Parse(dict["id"].AsString());
            if (this.packageID != this.asset.packageID)
            {
                Debug.LogError("Got asset store server build result from mismatching package");
            }
            else
            {
                this.asset.previewInfo.packageUrl = !dict.ContainsKey("url") ? string.Empty : dict["url"].AsString(true);
                this.asset.previewInfo.encryptionKey = !dict.ContainsKey("key") ? string.Empty : dict["key"].AsString(true);
                JSONValue value6 = dict["progress"];
                this.asset.previewInfo.buildProgress = (!value6.IsFloat() ? float.Parse(dict["progress"].AsString(true)) : dict["progress"].AsFloat(true)) / 100f;
            }
        }
    }
}

