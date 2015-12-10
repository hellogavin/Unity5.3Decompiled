namespace UnityEngine
{
    using System;

    internal static class ClassLibraryInitializer
    {
        private static void Init()
        {
            UnityLogWriter.Init();
        }
    }
}

