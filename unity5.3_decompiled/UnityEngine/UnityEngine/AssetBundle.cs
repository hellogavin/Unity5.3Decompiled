namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using UnityEngine.Internal;
    using UnityEngineInternal;

    public sealed class AssetBundle : Object
    {
        [Obsolete("This method is deprecated. Use GetAllAssetNames() instead.")]
        public string[] AllAssetNames()
        {
            return this.GetAllAssetNames();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool Contains(string name);
        [Obsolete("Method CreateFromFile has been renamed to LoadFromFile (UnityUpgradable) -> LoadFromFile(*)", true), EditorBrowsable(EditorBrowsableState.Never)]
        public static AssetBundle CreateFromFile(string path)
        {
            return null;
        }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Method CreateFromMemory has been renamed to LoadFromMemoryAsync (UnityUpgradable) -> LoadFromMemoryAsync(*)", true)]
        public static AssetBundleCreateRequest CreateFromMemory(byte[] binary)
        {
            return null;
        }

        [Obsolete("Method CreateFromMemoryImmediate has been renamed to LoadFromMemory (UnityUpgradable) -> LoadFromMemory(*)", true), EditorBrowsable(EditorBrowsableState.Never)]
        public static AssetBundle CreateFromMemoryImmediate(byte[] binary)
        {
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string[] GetAllAssetNames();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string[] GetAllScenePaths();
        [Obsolete("Method Load has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAsset instead and check the documentation for details.", true)]
        public Object Load(string name)
        {
            return null;
        }

        [Obsolete("Method Load has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAsset instead and check the documentation for details.", true)]
        public T Load<T>(string name) where T: Object
        {
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("Method Load has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAsset instead and check the documentation for details.", true), TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
        public extern Object Load(string name, Type type);
        [Obsolete("Method LoadAll has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAllAssets instead and check the documentation for details.", true)]
        public Object[] LoadAll()
        {
            return null;
        }

        [Obsolete("Method LoadAll has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAllAssets instead and check the documentation for details.", true)]
        public T[] LoadAll<T>() where T: Object
        {
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("Method LoadAll has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAllAssets instead and check the documentation for details.", true)]
        public extern Object[] LoadAll(Type type);
        public Object[] LoadAllAssets()
        {
            return this.LoadAllAssets(typeof(Object));
        }

        public T[] LoadAllAssets<T>() where T: Object
        {
            return Resources.ConvertObjects<T>(this.LoadAllAssets(typeof(T)));
        }

        public Object[] LoadAllAssets(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException("The input type cannot be null.");
            }
            return this.LoadAssetWithSubAssets_Internal(string.Empty, type);
        }

        public AssetBundleRequest LoadAllAssetsAsync()
        {
            return this.LoadAllAssetsAsync(typeof(Object));
        }

        public AssetBundleRequest LoadAllAssetsAsync<T>()
        {
            return this.LoadAllAssetsAsync(typeof(T));
        }

        public AssetBundleRequest LoadAllAssetsAsync(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException("The input type cannot be null.");
            }
            return this.LoadAssetWithSubAssetsAsync_Internal(string.Empty, type);
        }

        public Object LoadAsset(string name)
        {
            return this.LoadAsset(name, typeof(Object));
        }

        public T LoadAsset<T>(string name) where T: Object
        {
            return (T) this.LoadAsset(name, typeof(T));
        }

        [TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
        public Object LoadAsset(string name, Type type)
        {
            if (name == null)
            {
                throw new NullReferenceException("The input asset name cannot be null.");
            }
            if (name.Length == 0)
            {
                throw new ArgumentException("The input asset name cannot be empty.");
            }
            if (type == null)
            {
                throw new NullReferenceException("The input type cannot be null.");
            }
            return this.LoadAsset_Internal(name, type);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
        private extern Object LoadAsset_Internal(string name, Type type);
        public AssetBundleRequest LoadAssetAsync(string name)
        {
            return this.LoadAssetAsync(name, typeof(Object));
        }

        public AssetBundleRequest LoadAssetAsync<T>(string name)
        {
            return this.LoadAssetAsync(name, typeof(T));
        }

        public AssetBundleRequest LoadAssetAsync(string name, Type type)
        {
            if (name == null)
            {
                throw new NullReferenceException("The input asset name cannot be null.");
            }
            if (name.Length == 0)
            {
                throw new ArgumentException("The input asset name cannot be empty.");
            }
            if (type == null)
            {
                throw new NullReferenceException("The input type cannot be null.");
            }
            return this.LoadAssetAsync_Internal(name, type);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern AssetBundleRequest LoadAssetAsync_Internal(string name, Type type);
        public Object[] LoadAssetWithSubAssets(string name)
        {
            return this.LoadAssetWithSubAssets(name, typeof(Object));
        }

        public T[] LoadAssetWithSubAssets<T>(string name) where T: Object
        {
            return Resources.ConvertObjects<T>(this.LoadAssetWithSubAssets(name, typeof(T)));
        }

        public Object[] LoadAssetWithSubAssets(string name, Type type)
        {
            if (name == null)
            {
                throw new NullReferenceException("The input asset name cannot be null.");
            }
            if (name.Length == 0)
            {
                throw new ArgumentException("The input asset name cannot be empty.");
            }
            if (type == null)
            {
                throw new NullReferenceException("The input type cannot be null.");
            }
            return this.LoadAssetWithSubAssets_Internal(name, type);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern Object[] LoadAssetWithSubAssets_Internal(string name, Type type);
        public AssetBundleRequest LoadAssetWithSubAssetsAsync(string name)
        {
            return this.LoadAssetWithSubAssetsAsync(name, typeof(Object));
        }

        public AssetBundleRequest LoadAssetWithSubAssetsAsync<T>(string name)
        {
            return this.LoadAssetWithSubAssetsAsync(name, typeof(T));
        }

        public AssetBundleRequest LoadAssetWithSubAssetsAsync(string name, Type type)
        {
            if (name == null)
            {
                throw new NullReferenceException("The input asset name cannot be null.");
            }
            if (name.Length == 0)
            {
                throw new ArgumentException("The input asset name cannot be empty.");
            }
            if (type == null)
            {
                throw new NullReferenceException("The input type cannot be null.");
            }
            return this.LoadAssetWithSubAssetsAsync_Internal(name, type);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern AssetBundleRequest LoadAssetWithSubAssetsAsync_Internal(string name, Type type);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("Method LoadAsync has been deprecated. Script updater cannot update it as the loading behaviour has changed. Please use LoadAssetAsync instead and check the documentation for details.", true)]
        public extern AssetBundleRequest LoadAsync(string name, Type type);
        [ExcludeFromDocs]
        public static AssetBundle LoadFromFile(string path)
        {
            uint crc = 0;
            return LoadFromFile(path, crc);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AssetBundle LoadFromFile(string path, [DefaultValue("0")] uint crc);
        [ExcludeFromDocs]
        public static AssetBundleCreateRequest LoadFromFileAsync(string path)
        {
            uint crc = 0;
            return LoadFromFileAsync(path, crc);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AssetBundleCreateRequest LoadFromFileAsync(string path, [DefaultValue("0")] uint crc);
        [ExcludeFromDocs]
        public static AssetBundle LoadFromMemory(byte[] binary)
        {
            uint crc = 0;
            return LoadFromMemory(binary, crc);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AssetBundle LoadFromMemory(byte[] binary, [DefaultValue("0")] uint crc);
        [ExcludeFromDocs]
        public static AssetBundleCreateRequest LoadFromMemoryAsync(byte[] binary)
        {
            uint crc = 0;
            return LoadFromMemoryAsync(binary, crc);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AssetBundleCreateRequest LoadFromMemoryAsync(byte[] binary, [DefaultValue("0")] uint crc);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Unload(bool unloadAllLoadedObjects);

        public Object mainAsset { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

