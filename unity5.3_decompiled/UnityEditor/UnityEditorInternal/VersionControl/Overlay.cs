namespace UnityEditorInternal.VersionControl
{
    using System;
    using UnityEditor;
    using UnityEditor.VersionControl;
    using UnityEngine;

    public class Overlay
    {
        private static Texture2D s_BlueLeftParan;
        private static Texture2D s_BlueRightParan;
        private static Texture2D s_RedLeftParan;
        private static Texture2D s_RedRightParan;

        private static void CreateStaticResources()
        {
            if (s_BlueLeftParan == null)
            {
                s_BlueLeftParan = EditorGUIUtility.LoadIcon("P4_BlueLeftParenthesis");
                s_BlueLeftParan.hideFlags = HideFlags.HideAndDontSave;
                s_BlueRightParan = EditorGUIUtility.LoadIcon("P4_BlueRightParenthesis");
                s_BlueRightParan.hideFlags = HideFlags.HideAndDontSave;
                s_RedLeftParan = EditorGUIUtility.LoadIcon("P4_RedLeftParenthesis");
                s_RedLeftParan.hideFlags = HideFlags.HideAndDontSave;
                s_RedRightParan = EditorGUIUtility.LoadIcon("P4_RedRightParenthesis");
                s_RedRightParan.hideFlags = HideFlags.HideAndDontSave;
            }
        }

        private static void DrawMetaOverlay(Rect iconRect, bool isRemote)
        {
            iconRect.y--;
            if (isRemote)
            {
                iconRect.x -= 5f;
                GUI.DrawTexture(iconRect, s_BlueLeftParan);
                iconRect.x += 8f;
                GUI.DrawTexture(iconRect, s_BlueRightParan);
            }
            else
            {
                iconRect.x -= 5f;
                GUI.DrawTexture(iconRect, s_RedLeftParan);
                iconRect.x += 8f;
                GUI.DrawTexture(iconRect, s_RedRightParan);
            }
        }

        public static void DrawOverlay(Asset asset, Rect itemRect)
        {
            if ((asset != null) && (Event.current.type == EventType.Repaint))
            {
                string externalVersionControl = EditorSettings.externalVersionControl;
                if (((externalVersionControl != ExternalVersionControl.Disabled) && (externalVersionControl != ExternalVersionControl.AutoDetect)) && ((externalVersionControl != ExternalVersionControl.Generic) && (externalVersionControl != ExternalVersionControl.AssetServer)))
                {
                    DrawOverlays(asset, null, itemRect);
                }
            }
        }

        private static void DrawOverlay(Asset.States state, Rect iconRect)
        {
            Rect atlasRectForState = Provider.GetAtlasRectForState((int) state);
            if (atlasRectForState.width != 0f)
            {
                Texture2D overlayAtlas = Provider.overlayAtlas;
                if (overlayAtlas != null)
                {
                    GUI.DrawTextureWithTexCoords(iconRect, overlayAtlas, atlasRectForState);
                }
            }
        }

        public static void DrawOverlay(Asset asset, Asset metaAsset, Rect itemRect)
        {
            if (((asset != null) && (metaAsset != null)) && (Event.current.type == EventType.Repaint))
            {
                string externalVersionControl = EditorSettings.externalVersionControl;
                if (((externalVersionControl != ExternalVersionControl.Disabled) && (externalVersionControl != ExternalVersionControl.AutoDetect)) && ((externalVersionControl != ExternalVersionControl.Generic) && (externalVersionControl != ExternalVersionControl.AssetServer)))
                {
                    DrawOverlays(asset, metaAsset, itemRect);
                }
            }
        }

        private static void DrawOverlays(Asset asset, Asset metaAsset, Rect itemRect)
        {
            CreateStaticResources();
            float width = 16f;
            float num2 = 1f;
            float num3 = 4f;
            float num4 = -4f;
            Rect iconRect = new Rect(itemRect.x - num2, itemRect.y - num3, width, width);
            Rect rect2 = new Rect((itemRect.xMax - width) + num2, itemRect.y - num3, width, width);
            Rect rect3 = new Rect(itemRect.x - num2, (itemRect.yMax - width) + num3, width, width);
            Rect rect4 = new Rect((itemRect.xMax - width) + num4, (itemRect.yMax - width) + num3, width, width);
            Asset.States states = Asset.States.MetaFile | Asset.States.ReadOnly | Asset.States.Synced | Asset.States.Local;
            bool flag = (metaAsset == null) || ((metaAsset.state & states) == states);
            Asset.States states2 = (metaAsset != null) ? (metaAsset.state & (Asset.States.LockedLocal | Asset.States.AddedLocal | Asset.States.DeletedLocal | Asset.States.CheckedOutLocal)) : Asset.States.None;
            Asset.States states3 = (metaAsset != null) ? (metaAsset.state & (Asset.States.LockedRemote | Asset.States.AddedRemote | Asset.States.DeletedRemote | Asset.States.CheckedOutRemote)) : Asset.States.None;
            bool flag2 = asset.isFolder && Provider.isVersioningFolders;
            if (asset.IsState(Asset.States.AddedLocal))
            {
                DrawOverlay(Asset.States.AddedLocal, iconRect);
                if (((metaAsset != null) && ((states2 & Asset.States.AddedLocal) == Asset.States.None)) && !flag)
                {
                    DrawMetaOverlay(iconRect, false);
                }
            }
            else if (asset.IsState(Asset.States.DeletedLocal))
            {
                DrawOverlay(Asset.States.DeletedLocal, iconRect);
                if (((metaAsset != null) && ((states2 & Asset.States.DeletedLocal) == Asset.States.None)) && metaAsset.IsState(Asset.States.Missing | Asset.States.Local))
                {
                    DrawMetaOverlay(iconRect, false);
                }
            }
            else if (asset.IsState(Asset.States.LockedLocal))
            {
                DrawOverlay(Asset.States.LockedLocal, iconRect);
                if (((metaAsset != null) && ((states2 & Asset.States.LockedLocal) == Asset.States.None)) && !flag)
                {
                    DrawMetaOverlay(iconRect, false);
                }
            }
            else if (asset.IsState(Asset.States.CheckedOutLocal))
            {
                DrawOverlay(Asset.States.CheckedOutLocal, iconRect);
                if (((metaAsset != null) && ((states2 & Asset.States.CheckedOutLocal) == Asset.States.None)) && !flag)
                {
                    DrawMetaOverlay(iconRect, false);
                }
            }
            else if ((asset.IsState(Asset.States.Local) && !asset.IsState(Asset.States.OutOfSync)) && !asset.IsState(Asset.States.Synced))
            {
                DrawOverlay(Asset.States.Local, rect3);
                if ((metaAsset != null) && (metaAsset.IsUnderVersionControl || !metaAsset.IsState(Asset.States.Local)))
                {
                    DrawMetaOverlay(rect3, false);
                }
            }
            else if ((metaAsset != null) && metaAsset.IsState(Asset.States.AddedLocal))
            {
                DrawOverlay(Asset.States.AddedLocal, iconRect);
                if (flag2)
                {
                    DrawMetaOverlay(iconRect, false);
                }
            }
            else if ((metaAsset != null) && metaAsset.IsState(Asset.States.DeletedLocal))
            {
                DrawOverlay(Asset.States.DeletedLocal, iconRect);
                if (flag2)
                {
                    DrawMetaOverlay(iconRect, false);
                }
            }
            else if ((metaAsset != null) && metaAsset.IsState(Asset.States.LockedLocal))
            {
                DrawOverlay(Asset.States.LockedLocal, iconRect);
                if (flag2)
                {
                    DrawMetaOverlay(iconRect, false);
                }
            }
            else if ((metaAsset != null) && metaAsset.IsState(Asset.States.CheckedOutLocal))
            {
                DrawOverlay(Asset.States.CheckedOutLocal, iconRect);
                if (flag2)
                {
                    DrawMetaOverlay(iconRect, false);
                }
            }
            else if ((((metaAsset != null) && metaAsset.IsState(Asset.States.Local)) && (!metaAsset.IsState(Asset.States.OutOfSync) && !metaAsset.IsState(Asset.States.Synced))) && (!asset.IsState(Asset.States.Conflicted) && ((metaAsset == null) || !metaAsset.IsState(Asset.States.Conflicted))))
            {
                DrawOverlay(Asset.States.Local, rect3);
                if (flag2)
                {
                    DrawMetaOverlay(rect3, false);
                }
            }
            if (asset.IsState(Asset.States.Conflicted) || ((metaAsset != null) && metaAsset.IsState(Asset.States.Conflicted)))
            {
                DrawOverlay(Asset.States.Conflicted, rect3);
            }
            if (asset.IsState(Asset.States.AddedRemote))
            {
                DrawOverlay(Asset.States.AddedRemote, rect2);
                if ((metaAsset != null) && ((states3 & Asset.States.AddedRemote) == Asset.States.None))
                {
                    DrawMetaOverlay(rect2, true);
                }
            }
            else if (asset.IsState(Asset.States.DeletedRemote))
            {
                DrawOverlay(Asset.States.DeletedRemote, rect2);
                if ((metaAsset != null) && ((states3 & Asset.States.DeletedRemote) == Asset.States.None))
                {
                    DrawMetaOverlay(rect2, true);
                }
            }
            else if (asset.IsState(Asset.States.LockedRemote))
            {
                DrawOverlay(Asset.States.LockedRemote, rect2);
                if ((metaAsset != null) && ((states3 & Asset.States.LockedRemote) == Asset.States.None))
                {
                    DrawMetaOverlay(rect2, true);
                }
            }
            else if (asset.IsState(Asset.States.CheckedOutRemote))
            {
                DrawOverlay(Asset.States.CheckedOutRemote, rect2);
                if ((metaAsset != null) && ((states3 & Asset.States.CheckedOutRemote) == Asset.States.None))
                {
                    DrawMetaOverlay(rect2, true);
                }
            }
            else if ((metaAsset != null) && metaAsset.IsState(Asset.States.AddedRemote))
            {
                DrawOverlay(Asset.States.AddedRemote, rect2);
                if (flag2)
                {
                    DrawMetaOverlay(rect2, true);
                }
            }
            else if ((metaAsset != null) && metaAsset.IsState(Asset.States.DeletedRemote))
            {
                DrawOverlay(Asset.States.DeletedRemote, rect2);
                if (flag2)
                {
                    DrawMetaOverlay(rect2, true);
                }
            }
            else if ((metaAsset != null) && metaAsset.IsState(Asset.States.LockedRemote))
            {
                DrawOverlay(Asset.States.LockedRemote, rect2);
                if (flag2)
                {
                    DrawMetaOverlay(rect2, true);
                }
            }
            else if ((metaAsset != null) && metaAsset.IsState(Asset.States.CheckedOutRemote))
            {
                DrawOverlay(Asset.States.CheckedOutRemote, rect2);
                if (flag2)
                {
                    DrawMetaOverlay(rect2, true);
                }
            }
            if (asset.IsState(Asset.States.OutOfSync) || ((metaAsset != null) && metaAsset.IsState(Asset.States.OutOfSync)))
            {
                DrawOverlay(Asset.States.OutOfSync, rect4);
            }
        }

        public static Rect GetOverlayRect(Rect itemRect)
        {
            if (itemRect.width > itemRect.height)
            {
                itemRect.x += 16f;
                itemRect.width = 20f;
            }
            else
            {
                itemRect.width = 12f;
            }
            itemRect.height = itemRect.width;
            return itemRect;
        }
    }
}

