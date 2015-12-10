namespace UnityEditor
{
    using System;

    [CanEditMultipleObjects, CustomEditor(typeof(ModelImporter))]
    internal class ModelImporterEditor : AssetImporterTabbedEditor
    {
        public override bool HasPreviewGUI()
        {
            return (base.HasPreviewGUI() && (base.targets.Length < 2));
        }

        internal override void OnEnable()
        {
            if (base.m_SubEditorTypes == null)
            {
                base.m_SubEditorTypes = new Type[] { typeof(ModelImporterModelEditor), typeof(ModelImporterRigEditor), typeof(ModelImporterClipEditor) };
                base.m_SubEditorNames = new string[] { "Model", "Rig", "Animations" };
            }
            base.OnEnable();
        }

        internal override bool showImportedObject
        {
            get
            {
                return (base.activeEditor is ModelImporterModelEditor);
            }
        }

        protected override bool useAssetDrawPreview
        {
            get
            {
                return false;
            }
        }
    }
}

