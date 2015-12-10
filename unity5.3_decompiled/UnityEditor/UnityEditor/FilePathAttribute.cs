namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    [AttributeUsage(AttributeTargets.Class)]
    internal class FilePathAttribute : Attribute
    {
        public FilePathAttribute(string relativePath, Location location)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                Debug.LogError("Invalid relative path! (its null or empty)");
            }
            else
            {
                if (relativePath[0] == '/')
                {
                    relativePath = relativePath.Substring(1);
                }
                if (location == Location.PreferencesFolder)
                {
                    this.filepath = InternalEditorUtility.unityPreferencesFolder + "/" + relativePath;
                }
                else
                {
                    this.filepath = relativePath;
                }
            }
        }

        public string filepath { get; set; }

        public enum Location
        {
            PreferencesFolder,
            ProjectFolder
        }
    }
}

