namespace UnityEditorInternal.VersionControl
{
    using System;
    using UnityEditor;
    using UnityEditor.VersionControl;

    public class ProjectContextMenu
    {
        private static void CheckOut(MenuCommand cmd)
        {
            Provider.Checkout(Provider.GetAssetListFromSelection(), CheckoutMode.Both);
        }

        private static void CheckOutAsset(MenuCommand cmd)
        {
            Provider.Checkout(Provider.GetAssetListFromSelection(), CheckoutMode.Asset);
        }

        private static bool CheckOutAssetTest(MenuCommand cmd)
        {
            AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
            return (Provider.enabled && Provider.CheckoutIsValid(assetListFromSelection, CheckoutMode.Asset));
        }

        private static void CheckOutBoth(MenuCommand cmd)
        {
            Provider.Checkout(Provider.GetAssetListFromSelection(), CheckoutMode.Both);
        }

        private static bool CheckOutBothTest(MenuCommand cmd)
        {
            AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
            return (Provider.enabled && Provider.CheckoutIsValid(assetListFromSelection, CheckoutMode.Both));
        }

        private static void CheckOutMeta(MenuCommand cmd)
        {
            Provider.Checkout(Provider.GetAssetListFromSelection(), CheckoutMode.Meta);
        }

        private static bool CheckOutMetaTest(MenuCommand cmd)
        {
            AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
            return (Provider.enabled && Provider.CheckoutIsValid(assetListFromSelection, CheckoutMode.Meta));
        }

        private static bool CheckOutTest(MenuCommand cmd)
        {
            AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
            return (Provider.enabled && Provider.CheckoutIsValid(assetListFromSelection, CheckoutMode.Both));
        }

        private static void DiffHead(MenuCommand cmd)
        {
            Provider.DiffHead(Provider.GetAssetListFromSelection(), false);
        }

        private static bool DiffHeadTest(MenuCommand cmd)
        {
            AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
            return (Provider.enabled && Provider.DiffIsValid(assetListFromSelection));
        }

        private static void DiffHeadWithMeta(MenuCommand cmd)
        {
            Provider.DiffHead(Provider.GetAssetListFromSelection(), true);
        }

        private static bool DiffHeadWithMetaTest(MenuCommand cmd)
        {
            AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
            return (Provider.enabled && Provider.DiffIsValid(assetListFromSelection));
        }

        private static void GetLatest(MenuCommand cmd)
        {
            Provider.GetLatest(Provider.GetAssetListFromSelection()).SetCompletionAction(CompletionAction.UpdatePendingWindow);
        }

        private static bool GetLatestTest(MenuCommand cmd)
        {
            AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
            return (Provider.enabled && Provider.GetLatestIsValid(assetListFromSelection));
        }

        private static void Lock(MenuCommand cmd)
        {
            Provider.Lock(Provider.GetAssetListFromSelection(), true).SetCompletionAction(CompletionAction.UpdatePendingWindow);
        }

        private static bool LockTest(MenuCommand cmd)
        {
            AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
            return (Provider.enabled && Provider.LockIsValid(assetListFromSelection));
        }

        private static void MarkAdd(MenuCommand cmd)
        {
            Provider.Add(Provider.GetAssetListFromSelection(), true).SetCompletionAction(CompletionAction.UpdatePendingWindow);
        }

        private static bool MarkAddTest(MenuCommand cmd)
        {
            AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
            return (Provider.enabled && Provider.AddIsValid(assetListFromSelection));
        }

        private static void Resolve(MenuCommand cmd)
        {
            WindowResolve.Open(Provider.GetAssetListFromSelection());
        }

        private static bool ResolveTest(MenuCommand cmd)
        {
            AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
            return (Provider.enabled && Provider.ResolveIsValid(assetListFromSelection));
        }

        private static void Revert(MenuCommand cmd)
        {
            WindowRevert.Open(Provider.GetAssetListFromSelection());
        }

        private static bool RevertTest(MenuCommand cmd)
        {
            AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
            return (Provider.enabled && Provider.RevertIsValid(assetListFromSelection, RevertMode.Normal));
        }

        private static void RevertUnchanged(MenuCommand cmd)
        {
            AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
            Provider.Revert(assetListFromSelection, RevertMode.Unchanged).SetCompletionAction(CompletionAction.UpdatePendingWindow);
            Provider.Status(assetListFromSelection);
        }

        private static bool RevertUnchangedTest(MenuCommand cmd)
        {
            AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
            return (Provider.enabled && Provider.RevertIsValid(assetListFromSelection, RevertMode.Normal));
        }

        private static void Submit(MenuCommand cmd)
        {
            WindowChange.Open(Provider.GetAssetListFromSelection(), true);
        }

        private static bool SubmitTest(MenuCommand cmd)
        {
            AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
            return (Provider.enabled && Provider.SubmitIsValid(null, assetListFromSelection));
        }

        private static void Unlock(MenuCommand cmd)
        {
            Provider.Lock(Provider.GetAssetListFromSelection(), false).SetCompletionAction(CompletionAction.UpdatePendingWindow);
        }

        private static bool UnlockTest(MenuCommand cmd)
        {
            AssetList assetListFromSelection = Provider.GetAssetListFromSelection();
            return (Provider.enabled && Provider.UnlockIsValid(assetListFromSelection));
        }
    }
}

