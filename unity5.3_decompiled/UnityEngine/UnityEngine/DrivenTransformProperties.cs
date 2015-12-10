namespace UnityEngine
{
    using System;

    [Flags]
    public enum DrivenTransformProperties
    {
        All = -1,
        AnchoredPosition = 6,
        AnchoredPosition3D = 14,
        AnchoredPositionX = 2,
        AnchoredPositionY = 4,
        AnchoredPositionZ = 8,
        AnchorMax = 0xc00,
        AnchorMaxX = 0x400,
        AnchorMaxY = 0x800,
        AnchorMin = 0x300,
        AnchorMinX = 0x100,
        AnchorMinY = 0x200,
        Anchors = 0xf00,
        None = 0,
        Pivot = 0xc000,
        PivotX = 0x4000,
        PivotY = 0x8000,
        Rotation = 0x10,
        Scale = 0xe0,
        ScaleX = 0x20,
        ScaleY = 0x40,
        ScaleZ = 0x80,
        SizeDelta = 0x3000,
        SizeDeltaX = 0x1000,
        SizeDeltaY = 0x2000
    }
}

