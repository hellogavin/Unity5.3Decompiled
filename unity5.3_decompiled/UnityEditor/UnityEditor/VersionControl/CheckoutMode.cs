namespace UnityEditor.VersionControl
{
    using System;

    [Flags]
    public enum CheckoutMode
    {
        Asset = 1,
        Both = 3,
        Exact = 4,
        Meta = 2
    }
}

