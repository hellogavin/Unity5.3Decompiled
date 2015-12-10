namespace UnityEditor
{
    using System;
    using UnityEditorInternal;
    using UnityEngine;

    internal abstract class AssetImporterInspector : Editor
    {
        private Editor m_AssetEditor;
        private ulong m_AssetTimeStamp;
        private bool m_MightHaveModified;

        protected AssetImporterInspector()
        {
        }

        internal virtual void Apply()
        {
            base.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        internal void ApplyAndImport()
        {
            this.Apply();
            this.m_MightHaveModified = false;
            ImportAssets(this.GetAssetPaths());
            this.ResetValues();
        }

        protected bool ApplyButton()
        {
            return this.ApplyButton("Apply");
        }

        protected bool ApplyButton(string buttonText)
        {
            if (GUILayout.Button(buttonText, new GUILayoutOption[0]))
            {
                this.ApplyAndImport();
                return true;
            }
            return false;
        }

        protected void ApplyRevertGUI()
        {
            this.m_MightHaveModified = true;
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            bool flag = false;
            flag = this.ApplyRevertGUIButtons();
            if (this.AssetWasUpdated() && (Event.current.type != EventType.Layout))
            {
                IPreviewable preview = this.preview;
                if (preview != null)
                {
                    preview.ReloadPreviewInstances();
                }
                this.ResetTimeStamp();
                this.ResetValues();
                base.Repaint();
            }
            GUILayout.EndHorizontal();
            if (flag)
            {
                GUIUtility.ExitGUI();
            }
        }

        protected virtual bool ApplyRevertGUIButtons()
        {
            EditorGUI.BeginDisabledGroup(!this.HasModified());
            this.RevertButton();
            bool flag = this.ApplyButton();
            EditorGUI.EndDisabledGroup();
            return flag;
        }

        internal bool AssetWasUpdated()
        {
            AssetImporter target = this.target as AssetImporter;
            if (this.m_AssetTimeStamp == 0)
            {
                this.ResetTimeStamp();
            }
            return ((target != null) && (this.m_AssetTimeStamp != target.assetTimeStamp));
        }

        internal virtual void Awake()
        {
            this.ResetTimeStamp();
            this.ResetValues();
        }

        private string[] GetAssetPaths()
        {
            Object[] targets = base.targets;
            string[] strArray = new string[targets.Length];
            for (int i = 0; i < targets.Length; i++)
            {
                AssetImporter importer = targets[i] as AssetImporter;
                strArray[i] = importer.assetPath;
            }
            return strArray;
        }

        internal override SerializedObject GetSerializedObjectInternal()
        {
            if (base.m_SerializedObject == null)
            {
                base.m_SerializedObject = SerializedObject.LoadFromCache(base.GetInstanceID());
            }
            if (base.m_SerializedObject == null)
            {
                base.m_SerializedObject = new SerializedObject(base.targets);
            }
            return base.m_SerializedObject;
        }

        internal virtual bool HasModified()
        {
            return base.serializedObject.hasModifiedProperties;
        }

        private static void ImportAssets(string[] paths)
        {
            foreach (string str in paths)
            {
                AssetDatabase.WriteImportSettingsIfDirty(str);
            }
            try
            {
                AssetDatabase.StartAssetEditing();
                foreach (string str2 in paths)
                {
                    AssetDatabase.ImportAsset(str2);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        public virtual void OnDisable()
        {
            AssetImporter target = this.target as AssetImporter;
            if (((Unsupported.IsDestroyScriptableObject(this) && this.m_MightHaveModified) && ((target != null) && !InternalEditorUtility.ignoreInspectorChanges)) && (this.HasModified() && !this.AssetWasUpdated()))
            {
                string message = "Unapplied import settings for '" + target.assetPath + "'";
                if (base.targets.Length > 1)
                {
                    message = "Unapplied import settings for '" + base.targets.Length + "' files";
                }
                if (EditorUtility.DisplayDialog("Unapplied import settings", message, "Apply", "Revert"))
                {
                    this.Apply();
                    this.m_MightHaveModified = false;
                    ImportAssets(this.GetAssetPaths());
                }
            }
            if ((base.m_SerializedObject != null) && base.m_SerializedObject.hasModifiedProperties)
            {
                base.m_SerializedObject.Cache(base.GetInstanceID());
                base.m_SerializedObject = null;
            }
        }

        internal override void OnHeaderIconGUI(Rect iconRect)
        {
            this.assetEditor.OnHeaderIconGUI(iconRect);
        }

        internal void ResetTimeStamp()
        {
            AssetImporter target = this.target as AssetImporter;
            if (target != null)
            {
                this.m_AssetTimeStamp = target.assetTimeStamp;
            }
        }

        internal virtual void ResetValues()
        {
            base.serializedObject.SetIsDifferentCacheDirty();
            base.serializedObject.Update();
        }

        protected void RevertButton()
        {
            this.RevertButton("Revert");
        }

        protected void RevertButton(string buttonText)
        {
            if (GUILayout.Button(buttonText, new GUILayoutOption[0]))
            {
                this.m_MightHaveModified = false;
                this.ResetTimeStamp();
                this.ResetValues();
                if (this.HasModified())
                {
                    Debug.LogError("Importer reports modified values after reset.");
                }
            }
        }

        internal virtual Editor assetEditor
        {
            get
            {
                return this.m_AssetEditor;
            }
            set
            {
                this.m_AssetEditor = value;
            }
        }

        internal override IPreviewable preview
        {
            get
            {
                if (this.useAssetDrawPreview && (this.assetEditor != null))
                {
                    return this.assetEditor;
                }
                return base.preview;
            }
        }

        internal override int referenceTargetIndex
        {
            get
            {
                return base.referenceTargetIndex;
            }
            set
            {
                base.referenceTargetIndex = value;
                this.assetEditor.referenceTargetIndex = value;
            }
        }

        internal virtual bool showImportedObject
        {
            get
            {
                return true;
            }
        }

        internal override string targetTitle
        {
            get
            {
                return (this.assetEditor.targetTitle + " Import Settings");
            }
        }

        protected virtual bool useAssetDrawPreview
        {
            get
            {
                return true;
            }
        }
    }
}

