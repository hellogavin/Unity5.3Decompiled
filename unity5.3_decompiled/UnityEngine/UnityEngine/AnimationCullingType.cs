namespace UnityEngine
{
    using System;

    public enum AnimationCullingType
    {
        AlwaysAnimate = 0,
        [Obsolete("Enum member AnimatorCullingMode.BasedOnClipBounds has been deprecated. Use AnimationCullingType.AlwaysAnimate or AnimationCullingType.BasedOnRenderers instead")]
        BasedOnClipBounds = 2,
        BasedOnRenderers = 1,
        [Obsolete("Enum member AnimatorCullingMode.BasedOnUserBounds has been deprecated. Use AnimationCullingType.AlwaysAnimate or AnimationCullingType.BasedOnRenderers instead")]
        BasedOnUserBounds = 3
    }
}

