namespace UnityEngine
{
    using System;

    public enum AnimatorCullingMode
    {
        AlwaysAnimate = 0,
        [Obsolete("Enum member AnimatorCullingMode.BasedOnRenderers has been deprecated. Use AnimatorCullingMode.CullUpdateTransforms instead (UnityUpgradable) -> CullUpdateTransforms", true)]
        BasedOnRenderers = 1,
        CullCompletely = 2,
        CullUpdateTransforms = 1
    }
}

