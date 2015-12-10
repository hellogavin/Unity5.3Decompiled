namespace UnityEditor
{
    using System;
    using UnityEngine;
    using UnityEngine.Internal;

    public class AssetPostprocessor
    {
        private string m_PathName;

        public virtual int GetPostprocessOrder()
        {
            return 0;
        }

        public virtual uint GetVersion()
        {
            return 0;
        }

        [ExcludeFromDocs]
        public void LogError(string warning)
        {
            Object context = null;
            this.LogError(warning, context);
        }

        public void LogError(string warning, [DefaultValue("null")] Object context)
        {
            Debug.LogError(warning, context);
        }

        [ExcludeFromDocs]
        public void LogWarning(string warning)
        {
            Object context = null;
            this.LogWarning(warning, context);
        }

        public void LogWarning(string warning, [DefaultValue("null")] Object context)
        {
            Debug.LogWarning(warning, context);
        }

        public AssetImporter assetImporter
        {
            get
            {
                return AssetImporter.GetAtPath(this.assetPath);
            }
        }

        public string assetPath
        {
            get
            {
                return this.m_PathName;
            }
            set
            {
                this.m_PathName = value;
            }
        }

        [Obsolete("To set or get the preview, call EditorUtility.SetAssetPreview or AssetPreview.GetAssetPreview instead", true)]
        public Texture2D preview
        {
            get
            {
                return null;
            }
            set
            {
            }
        }
    }
}

