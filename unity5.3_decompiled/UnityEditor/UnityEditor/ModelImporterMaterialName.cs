namespace UnityEditor
{
    using System;

    public enum ModelImporterMaterialName
    {
        BasedOnMaterialName = 1,
        BasedOnModelNameAndMaterialName = 2,
        BasedOnTextureName = 0,
        [Obsolete("You should use ModelImporterMaterialName.BasedOnTextureName instead, because it it less complicated and behaves in more consistent way.")]
        BasedOnTextureName_Or_ModelNameAndMaterialName = 3
    }
}

