namespace UnityEditor
{
    using System;

    internal interface IPlayHead
    {
        float currentTime { set; }

        bool playing { get; }
    }
}

