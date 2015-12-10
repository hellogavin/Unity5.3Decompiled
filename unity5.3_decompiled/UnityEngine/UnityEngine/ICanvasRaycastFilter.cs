namespace UnityEngine
{
    using System;

    public interface ICanvasRaycastFilter
    {
        bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera);
    }
}

