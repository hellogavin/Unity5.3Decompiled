namespace UnityEditor.VersionControl
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    public sealed class Provider
    {
        public static Task Add(Asset asset, bool recursive)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_Add(assets, recursive);
        }

        public static Task Add(AssetList assets, bool recursive)
        {
            return Internal_Add(assets.ToArray(), recursive);
        }

        public static bool AddIsValid(AssetList assets)
        {
            return Internal_AddIsValid(assets.ToArray());
        }

        internal static Asset CacheStatus(string assetPath)
        {
            return Internal_CacheStatus(assetPath);
        }

        public static Task ChangeSetDescription(ChangeSet changeset)
        {
            return Internal_ChangeSetDescription(changeset);
        }

        public static Task ChangeSetMove(Asset asset, string changesetID)
        {
            ChangeSet target = new ChangeSet(string.Empty, changesetID);
            Asset[] assets = new Asset[] { asset };
            return Internal_ChangeSetMove(assets, target);
        }

        public static Task ChangeSetMove(Asset asset, ChangeSet changeset)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_ChangeSetMove(assets, changeset);
        }

        public static Task ChangeSetMove(AssetList assets, string changesetID)
        {
            ChangeSet target = new ChangeSet(string.Empty, changesetID);
            return Internal_ChangeSetMove(assets.ToArray(), target);
        }

        public static Task ChangeSetMove(AssetList assets, ChangeSet changeset)
        {
            return Internal_ChangeSetMove(assets.ToArray(), changeset);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Task ChangeSets();
        public static Task ChangeSetStatus(string changesetID)
        {
            ChangeSet changeset = new ChangeSet(string.Empty, changesetID);
            return Internal_ChangeSetStatus(changeset);
        }

        public static Task ChangeSetStatus(ChangeSet changeset)
        {
            return Internal_ChangeSetStatus(changeset);
        }

        public static Task Checkout(string asset, CheckoutMode mode)
        {
            string[] assets = new string[] { asset };
            return Internal_CheckoutStrings(assets, mode);
        }

        public static Task Checkout(Asset asset, CheckoutMode mode)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_Checkout(assets, mode);
        }

        public static Task Checkout(AssetList assets, CheckoutMode mode)
        {
            return Internal_Checkout(assets.ToArray(), mode);
        }

        public static Task Checkout(string[] assets, CheckoutMode mode)
        {
            return Internal_CheckoutStrings(assets, mode);
        }

        public static Task Checkout(Object[] assets, CheckoutMode mode)
        {
            AssetList list = new AssetList();
            foreach (Object obj2 in assets)
            {
                Asset assetByPath = GetAssetByPath(AssetDatabase.GetAssetPath(obj2));
                list.Add(assetByPath);
            }
            return Internal_Checkout(list.ToArray(), mode);
        }

        public static Task Checkout(Object asset, CheckoutMode mode)
        {
            Asset assetByPath = GetAssetByPath(AssetDatabase.GetAssetPath(asset));
            Asset[] assets = new Asset[] { assetByPath };
            return Internal_Checkout(assets, mode);
        }

        public static bool CheckoutIsValid(Asset asset)
        {
            return CheckoutIsValid(asset, CheckoutMode.Exact);
        }

        public static bool CheckoutIsValid(AssetList assets)
        {
            return CheckoutIsValid(assets, CheckoutMode.Exact);
        }

        public static bool CheckoutIsValid(Asset asset, CheckoutMode mode)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_CheckoutIsValid(assets, mode);
        }

        public static bool CheckoutIsValid(AssetList assets, CheckoutMode mode)
        {
            return Internal_CheckoutIsValid(assets.ToArray(), mode);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ClearCache();
        public static Task Delete(string assetProjectPath)
        {
            return Internal_DeleteAtProjectPath(assetProjectPath);
        }

        public static Task Delete(Asset asset)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_Delete(assets);
        }

        public static Task Delete(AssetList assets)
        {
            return Internal_Delete(assets.ToArray());
        }

        public static Task DeleteChangeSets(UnityEditor.VersionControl.ChangeSets changesets)
        {
            return Internal_DeleteChangeSets(changesets.ToArray());
        }

        public static bool DeleteChangeSetsIsValid(UnityEditor.VersionControl.ChangeSets changesets)
        {
            return Internal_DeleteChangeSetsIsValid(changesets.ToArray());
        }

        public static Task DiffHead(AssetList assets, bool includingMetaFiles)
        {
            return Internal_DiffHead(assets.ToArray(), includingMetaFiles);
        }

        public static bool DiffIsValid(AssetList assets)
        {
            return Internal_DiffIsValid(assets.ToArray());
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GenerateID();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern ConfigField[] GetActiveConfigFields();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Plugin GetActivePlugin();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Asset GetAssetByGUID(string guid);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Asset GetAssetByPath(string unityPath);
        public static AssetList GetAssetListFromSelection()
        {
            AssetList list = new AssetList();
            foreach (Asset asset in Internal_GetAssetArrayFromSelection())
            {
                list.Add(asset);
            }
            return list;
        }

        internal static Rect GetAtlasRectForState(int state)
        {
            Rect rect;
            INTERNAL_CALL_GetAtlasRectForState(state, out rect);
            return rect;
        }

        public static Task GetLatest(Asset asset)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_GetLatest(assets);
        }

        public static Task GetLatest(AssetList assets)
        {
            return Internal_GetLatest(assets.ToArray());
        }

        public static bool GetLatestIsValid(Asset asset)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_GetLatestIsValid(assets);
        }

        public static bool GetLatestIsValid(AssetList assets)
        {
            return Internal_GetLatestIsValid(assets.ToArray());
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Task Incoming();
        public static Task IncomingChangeSetAssets(string changesetID)
        {
            ChangeSet changeset = new ChangeSet(string.Empty, changesetID);
            return Internal_IncomingChangeSetAssets(changeset);
        }

        public static Task IncomingChangeSetAssets(ChangeSet changeset)
        {
            return Internal_IncomingChangeSetAssets(changeset);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_Add(Asset[] assets, bool recursive);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool Internal_AddIsValid(Asset[] assets);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Asset Internal_CacheStatus(string assetPath);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetAtlasRectForState(int state, out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_ChangeSetDescription(ChangeSet changeset);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_ChangeSetMove(Asset[] assets, ChangeSet target);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_ChangeSetStatus(ChangeSet changeset);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_Checkout(Asset[] assets, CheckoutMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool Internal_CheckoutIsValid(Asset[] assets, CheckoutMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_CheckoutStrings(string[] assets, CheckoutMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_Delete(Asset[] assets);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_DeleteAtProjectPath(string assetProjectPath);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_DeleteChangeSets(ChangeSet[] changesets);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool Internal_DeleteChangeSetsIsValid(ChangeSet[] changes);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_DiffHead(Asset[] assets, bool includingMetaFiles);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool Internal_DiffIsValid(Asset[] assets);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Asset[] Internal_GetAssetArrayFromSelection();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_GetLatest(Asset[] assets);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool Internal_GetLatestIsValid(Asset[] assets);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_IncomingChangeSetAssets(ChangeSet changeset);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_Lock(Asset[] assets, bool locked);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool Internal_LockIsValid(Asset[] assets);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_Merge(Asset[] assets, MergeMethod method);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_MoveAsStrings(string from, string to);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool Internal_PromptAndCheckoutIfNeeded(string[] assets, string promptIfCheckoutIsNeeded);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_Resolve(Asset[] assets, ResolveMethod resolveMethod);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool Internal_ResolveIsValid(Asset[] assets);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_Revert(Asset[] assets, RevertMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_RevertChangeSets(ChangeSet[] changesets, RevertMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool Internal_RevertIsValid(Asset[] assets, RevertMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_SetFileMode(Asset[] assets, FileMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_SetFileModeStrings(string[] assets, FileMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_Status(Asset[] assets, bool recursively);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_StatusAbsolutePath(string assetPath);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_StatusStrings(string[] assetsProjectPaths, bool recursively);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Task Internal_Submit(ChangeSet changeset, Asset[] assets, string description, bool saveOnly);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool Internal_SubmitIsValid(ChangeSet changeset, Asset[] assets);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool Internal_UnlockIsValid(Asset[] assets);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void InvalidateCache();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool IsCustomCommandEnabled(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsOpenForEdit(Asset asset);
        public static Task Lock(Asset asset, bool locked)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_Lock(assets, locked);
        }

        public static Task Lock(AssetList assets, bool locked)
        {
            return Internal_Lock(assets.ToArray(), locked);
        }

        public static bool LockIsValid(Asset asset)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_LockIsValid(assets);
        }

        public static bool LockIsValid(AssetList assets)
        {
            return Internal_LockIsValid(assets.ToArray());
        }

        public static Task Merge(AssetList assets, MergeMethod method)
        {
            return Internal_Merge(assets.ToArray(), method);
        }

        public static Task Move(string from, string to)
        {
            return Internal_MoveAsStrings(from, to);
        }

        internal static bool PromptAndCheckoutIfNeeded(string[] assets, string promptIfCheckoutIsNeeded)
        {
            return Internal_PromptAndCheckoutIfNeeded(assets, promptIfCheckoutIsNeeded);
        }

        public static Task Resolve(AssetList assets, ResolveMethod resolveMethod)
        {
            return Internal_Resolve(assets.ToArray(), resolveMethod);
        }

        public static bool ResolveIsValid(AssetList assets)
        {
            return Internal_ResolveIsValid(assets.ToArray());
        }

        public static Task Revert(Asset asset, RevertMode mode)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_Revert(assets, mode);
        }

        public static Task Revert(AssetList assets, RevertMode mode)
        {
            return Internal_Revert(assets.ToArray(), mode);
        }

        internal static Task RevertChangeSets(UnityEditor.VersionControl.ChangeSets changesets, RevertMode mode)
        {
            return Internal_RevertChangeSets(changesets.ToArray(), mode);
        }

        public static bool RevertIsValid(Asset asset, RevertMode mode)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_RevertIsValid(assets, mode);
        }

        public static bool RevertIsValid(AssetList assets, RevertMode mode)
        {
            return Internal_RevertIsValid(assets.ToArray(), mode);
        }

        internal static Task SetFileMode(AssetList assets, FileMode mode)
        {
            return Internal_SetFileMode(assets.ToArray(), mode);
        }

        internal static Task SetFileMode(string[] assets, FileMode mode)
        {
            return Internal_SetFileModeStrings(assets, mode);
        }

        public static Task Status(Asset asset)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_Status(assets, true);
        }

        public static Task Status(AssetList assets)
        {
            return Internal_Status(assets.ToArray(), true);
        }

        public static Task Status(string[] assets)
        {
            return Internal_StatusStrings(assets, true);
        }

        public static Task Status(string asset)
        {
            string[] assetsProjectPaths = new string[] { asset };
            return Internal_StatusStrings(assetsProjectPaths, true);
        }

        public static Task Status(string[] assets, bool recursively)
        {
            return Internal_StatusStrings(assets, recursively);
        }

        public static Task Status(string asset, bool recursively)
        {
            string[] assetsProjectPaths = new string[] { asset };
            return Internal_StatusStrings(assetsProjectPaths, recursively);
        }

        public static Task Status(Asset asset, bool recursively)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_Status(assets, recursively);
        }

        public static Task Status(AssetList assets, bool recursively)
        {
            return Internal_Status(assets.ToArray(), recursively);
        }

        public static Task Submit(ChangeSet changeset, AssetList list, string description, bool saveOnly)
        {
            return Internal_Submit(changeset, (list == null) ? null : list.ToArray(), description, saveOnly);
        }

        public static bool SubmitIsValid(ChangeSet changeset, AssetList assets)
        {
            return Internal_SubmitIsValid(changeset, (assets == null) ? null : assets.ToArray());
        }

        public static bool UnlockIsValid(Asset asset)
        {
            Asset[] assets = new Asset[] { asset };
            return Internal_UnlockIsValid(assets);
        }

        public static bool UnlockIsValid(AssetList assets)
        {
            return Internal_UnlockIsValid(assets.ToArray());
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Task UpdateSettings();

        public static Task activeTask { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static CustomCommand[] customCommands { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool enabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool hasChangelistSupport { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool hasCheckoutSupport { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isActive { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isVersioningFolders { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string offlineReason { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static OnlineState onlineState { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static Texture2D overlayAtlas { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool requiresNetwork { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

