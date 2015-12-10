namespace UnityEditor.Sprites
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class DefaultPackerPolicy : IPackerPolicy
    {
        [CompilerGenerated]
        private static Func<Object, Sprite> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Sprite, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<Entry, string> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<Entry, Entry> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<Entry, AtlasSettings> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<Entry, Entry> <>f__am$cache5;
        private const uint kDefaultPaddingPower = 3;

        private SpritePackingMode GetPackingMode(string packingTag, SpriteMeshType meshType)
        {
            if ((meshType == SpriteMeshType.Tight) && (this.IsTagPrefixed(packingTag) == this.AllowTightWhenTagged))
            {
                return SpritePackingMode.Tight;
            }
            return SpritePackingMode.Rectangle;
        }

        public virtual int GetVersion()
        {
            return 1;
        }

        protected bool IsTagPrefixed(string packingTag)
        {
            packingTag = packingTag.Trim();
            if (packingTag.Length < this.TagPrefix.Length)
            {
                return false;
            }
            return (packingTag.Substring(0, this.TagPrefix.Length) == this.TagPrefix);
        }

        public void OnGroupAtlases(BuildTarget target, PackerJob job, int[] textureImporterInstanceIDs)
        {
            List<Entry> source = new List<Entry>();
            foreach (int num in textureImporterInstanceIDs)
            {
                TextureFormat format;
                ColorSpace space;
                int num3;
                TextureImporter assetToUnload = EditorUtility.InstanceIDToObject(num) as TextureImporter;
                assetToUnload.ReadTextureImportInstructions(target, out format, out space, out num3);
                TextureImporterSettings dest = new TextureImporterSettings();
                assetToUnload.ReadTextureSettings(dest);
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = x => x as Sprite;
                }
                if (<>f__am$cache1 == null)
                {
                    <>f__am$cache1 = x => x != null;
                }
                foreach (Sprite sprite in AssetDatabase.LoadAllAssetRepresentationsAtPath(assetToUnload.assetPath).Select<Object, Sprite>(<>f__am$cache0).Where<Sprite>(<>f__am$cache1).ToArray<Sprite>())
                {
                    Entry item = new Entry {
                        sprite = sprite
                    };
                    item.settings.format = format;
                    item.settings.colorSpace = space;
                    item.settings.compressionQuality = !TextureUtil.IsCompressedTextureFormat(format) ? 0 : num3;
                    item.settings.filterMode = !Enum.IsDefined(typeof(FilterMode), assetToUnload.filterMode) ? FilterMode.Bilinear : assetToUnload.filterMode;
                    item.settings.maxWidth = 0x800;
                    item.settings.maxHeight = 0x800;
                    item.settings.generateMipMaps = assetToUnload.mipmapEnabled;
                    item.settings.enableRotation = this.AllowRotationFlipping;
                    item.settings.allowsAlphaSplitting = assetToUnload.GetAllowsAlphaSplitting();
                    if (assetToUnload.mipmapEnabled)
                    {
                        item.settings.paddingPower = 3;
                    }
                    else
                    {
                        item.settings.paddingPower = (uint) EditorSettings.spritePackerPaddingPower;
                    }
                    item.atlasName = this.ParseAtlasName(assetToUnload.spritePackingTag);
                    item.packingMode = this.GetPackingMode(assetToUnload.spritePackingTag, dest.spriteMeshType);
                    item.anisoLevel = assetToUnload.anisoLevel;
                    source.Add(item);
                }
                Resources.UnloadAsset(assetToUnload);
            }
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = e => e.atlasName;
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = e => e;
            }
            IEnumerator<IGrouping<string, Entry>> enumerator = source.GroupBy<Entry, string, Entry>(<>f__am$cache2, <>f__am$cache3).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    IGrouping<string, Entry> current = enumerator.Current;
                    int num5 = 0;
                    if (<>f__am$cache4 == null)
                    {
                        <>f__am$cache4 = t => t.settings;
                    }
                    if (<>f__am$cache5 == null)
                    {
                        <>f__am$cache5 = t => t;
                    }
                    IEnumerable<IGrouping<AtlasSettings, Entry>> enumerable2 = current.GroupBy<Entry, AtlasSettings, Entry>(<>f__am$cache4, <>f__am$cache5);
                    IEnumerator<IGrouping<AtlasSettings, Entry>> enumerator2 = enumerable2.GetEnumerator();
                    try
                    {
                        while (enumerator2.MoveNext())
                        {
                            IGrouping<AtlasSettings, Entry> grouping2 = enumerator2.Current;
                            string key = current.Key;
                            if (enumerable2.Count<IGrouping<AtlasSettings, Entry>>() > 1)
                            {
                                key = key + string.Format(" (Group {0})", num5);
                            }
                            AtlasSettings settings = grouping2.Key;
                            settings.anisoLevel = 1;
                            if (settings.generateMipMaps)
                            {
                                IEnumerator<Entry> enumerator3 = grouping2.GetEnumerator();
                                try
                                {
                                    while (enumerator3.MoveNext())
                                    {
                                        Entry entry2 = enumerator3.Current;
                                        if (entry2.anisoLevel > settings.anisoLevel)
                                        {
                                            settings.anisoLevel = entry2.anisoLevel;
                                        }
                                    }
                                }
                                finally
                                {
                                    if (enumerator3 == null)
                                    {
                                    }
                                    enumerator3.Dispose();
                                }
                            }
                            job.AddAtlas(key, settings);
                            IEnumerator<Entry> enumerator4 = grouping2.GetEnumerator();
                            try
                            {
                                while (enumerator4.MoveNext())
                                {
                                    Entry entry3 = enumerator4.Current;
                                    job.AssignToAtlas(key, entry3.sprite, entry3.packingMode, SpritePackingRotation.None);
                                }
                            }
                            finally
                            {
                                if (enumerator4 == null)
                                {
                                }
                                enumerator4.Dispose();
                            }
                            num5++;
                        }
                        continue;
                    }
                    finally
                    {
                        if (enumerator2 == null)
                        {
                        }
                        enumerator2.Dispose();
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
        }

        private string ParseAtlasName(string packingTag)
        {
            string str = packingTag.Trim();
            if (this.IsTagPrefixed(str))
            {
                str = str.Substring(this.TagPrefix.Length).Trim();
            }
            return ((str.Length != 0) ? str : "(unnamed)");
        }

        protected virtual bool AllowRotationFlipping
        {
            get
            {
                return false;
            }
        }

        protected virtual bool AllowTightWhenTagged
        {
            get
            {
                return true;
            }
        }

        protected virtual string TagPrefix
        {
            get
            {
                return "[TIGHT]";
            }
        }

        protected class Entry
        {
            public int anisoLevel;
            public string atlasName;
            public SpritePackingMode packingMode;
            public AtlasSettings settings;
            public Sprite sprite;
        }
    }
}

