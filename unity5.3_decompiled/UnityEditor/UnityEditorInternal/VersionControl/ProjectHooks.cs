namespace UnityEditorInternal.VersionControl
{
    using System;
    using UnityEditor.VersionControl;
    using UnityEngine;

    internal class ProjectHooks
    {
        public static Rect GetOverlayRect(Rect drawRect)
        {
            return Overlay.GetOverlayRect(drawRect);
        }

        public static void OnProjectWindowItem(string guid, Rect drawRect)
        {
            if (Provider.isActive)
            {
                Asset assetByGUID = Provider.GetAssetByGUID(guid);
                if (assetByGUID != null)
                {
                    char[] trimChars = new char[] { '/' };
                    Asset assetByPath = Provider.GetAssetByPath(assetByGUID.path.Trim(trimChars) + ".meta");
                    Overlay.DrawOverlay(assetByGUID, assetByPath, drawRect);
                }
            }
        }
    }
}

