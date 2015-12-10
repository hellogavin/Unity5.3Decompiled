namespace UnityEditor
{
    using System;

    [Flags]
    public enum VertexChannelCompressionFlags
    {
        kColor = 4,
        kNormal = 2,
        kPosition = 1,
        kTangent = 0x80,
        kUV0 = 8,
        kUV1 = 0x10,
        kUV2 = 0x20,
        kUV3 = 0x40
    }
}

