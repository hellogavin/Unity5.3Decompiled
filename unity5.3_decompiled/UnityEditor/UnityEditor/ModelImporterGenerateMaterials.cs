namespace UnityEditor
{
    using System;

    [Obsolete("Use ModelImporterMaterialName, ModelImporter.materialName and ModelImporter.importMaterials instead")]
    public enum ModelImporterGenerateMaterials
    {
        [Obsolete("Use ModelImporter.importMaterials=false instead")]
        None = 0,
        [Obsolete("Use ModelImporter.importMaterials=true and ModelImporter.materialName=ModelImporterMaterialName.BasedOnModelNameAndMaterialName instead")]
        PerSourceMaterial = 2,
        [Obsolete("Use ModelImporter.importMaterials=true and ModelImporter.materialName=ModelImporterMaterialName.BasedOnTextureName instead")]
        PerTexture = 1
    }
}

