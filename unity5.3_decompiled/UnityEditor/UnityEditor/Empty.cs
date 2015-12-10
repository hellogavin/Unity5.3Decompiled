namespace UnityEditor
{
    using System;

    internal class Empty
    {
        internal static bool OnOpenAsset(int instanceID, int line)
        {
            return false;
        }

        internal static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
        }
    }
}

