namespace UnityEditorInternal.VersionControl
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditor.VersionControl;
    using UnityEngine;

    public class AssetModificationHook
    {
        private static Asset GetStatusCachedIfPossible(string from)
        {
            Asset asset = Provider.CacheStatus(from);
            if ((asset != null) && !asset.IsState(Asset.States.Updating))
            {
                return asset;
            }
            Provider.Status(from, false).Wait();
            return Provider.CacheStatus(from);
        }

        public static bool IsOpenForEdit(string assetPath, out string message)
        {
            message = string.Empty;
            if (!Provider.enabled)
            {
                return true;
            }
            if (string.IsNullOrEmpty(assetPath))
            {
                return true;
            }
            Asset assetByPath = Provider.GetAssetByPath(assetPath);
            if (assetByPath == null)
            {
                Task task = Provider.Status(assetPath, false);
                task.Wait();
                assetByPath = (task.assetList.Count <= 0) ? null : task.assetList[0];
            }
            if (assetByPath == null)
            {
                return false;
            }
            return Provider.IsOpenForEdit(assetByPath);
        }

        public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions option)
        {
            if (!Provider.enabled)
            {
                return AssetDeleteResult.DidNotDelete;
            }
            Task task = Provider.Delete(assetPath);
            task.SetCompletionAction(CompletionAction.UpdatePendingWindow);
            task.Wait();
            return (!task.success ? AssetDeleteResult.FailedDelete : AssetDeleteResult.DidNotDelete);
        }

        public static AssetMoveResult OnWillMoveAsset(string from, string to)
        {
            if (!Provider.enabled)
            {
                return AssetMoveResult.DidNotMove;
            }
            Asset statusCachedIfPossible = GetStatusCachedIfPossible(from);
            if ((statusCachedIfPossible == null) || !statusCachedIfPossible.IsUnderVersionControl)
            {
                return AssetMoveResult.DidNotMove;
            }
            if (statusCachedIfPossible.IsState(Asset.States.OutOfSync))
            {
                Debug.LogError("Cannot move version controlled file that is not up to date. Please get latest changes from server");
                return AssetMoveResult.FailedMove;
            }
            if (statusCachedIfPossible.IsState(Asset.States.DeletedRemote))
            {
                Debug.LogError("Cannot move version controlled file that is deleted on server. Please get latest changes from server");
                return AssetMoveResult.FailedMove;
            }
            if (statusCachedIfPossible.IsState(Asset.States.CheckedOutRemote))
            {
                Debug.LogError("Cannot move version controlled file that is checked out on server. Please get latest changes from server");
                return AssetMoveResult.FailedMove;
            }
            if (statusCachedIfPossible.IsState(Asset.States.LockedRemote))
            {
                Debug.LogError("Cannot move version controlled file that is locked on server. Please get latest changes from server");
                return AssetMoveResult.FailedMove;
            }
            Task task = Provider.Move(from, to);
            task.Wait();
            return (!task.success ? AssetMoveResult.FailedMove : ((AssetMoveResult) task.resultCode));
        }
    }
}

