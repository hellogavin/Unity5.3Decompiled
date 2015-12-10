namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditorInternal;
    using UnityEngine;

    internal static class PresetLibraryLocations
    {
        private static string ConvertToUnitySeperators(string path)
        {
            return path.Replace('\\', '/');
        }

        public static List<string> GetAvailableFilesWithExtensionOnTheHDD(PresetFileLocation fileLocation, string fileExtensionWithoutDot)
        {
            List<string> filesWithExentionFromFolders = GetFilesWithExentionFromFolders(GetDirectoryPaths(fileLocation), fileExtensionWithoutDot);
            for (int i = 0; i < filesWithExentionFromFolders.Count; i++)
            {
                filesWithExentionFromFolders[i] = ConvertToUnitySeperators(filesWithExentionFromFolders[i]);
            }
            return filesWithExentionFromFolders;
        }

        public static string GetCurveLibraryExtension(bool normalized)
        {
            if (normalized)
            {
                return "curvesNormalized";
            }
            return "curves";
        }

        public static string GetDefaultFilePathForFileLocation(PresetFileLocation fileLocation)
        {
            switch (fileLocation)
            {
                case PresetFileLocation.PreferencesFolder:
                    return (InternalEditorUtility.unityPreferencesFolder + "/Presets/");

                case PresetFileLocation.ProjectFolder:
                    return "Assets/Editor/";
            }
            Debug.LogError("Enum not handled!");
            return string.Empty;
        }

        private static List<string> GetDirectoryPaths(PresetFileLocation fileLocation)
        {
            List<string> list = new List<string>();
            PresetFileLocation location = fileLocation;
            if (location != PresetFileLocation.PreferencesFolder)
            {
                if (location == PresetFileLocation.ProjectFolder)
                {
                    string[] collection = Directory.GetDirectories("Assets/", "Editor", SearchOption.AllDirectories);
                    list.AddRange(collection);
                    return list;
                }
                Debug.LogError("Enum not handled!");
                return list;
            }
            list.Add(GetDefaultFilePathForFileLocation(PresetFileLocation.PreferencesFolder));
            return list;
        }

        public static PresetFileLocation GetFileLocationFromPath(string path)
        {
            if (path.Contains(InternalEditorUtility.unityPreferencesFolder))
            {
                return PresetFileLocation.PreferencesFolder;
            }
            if (!path.Contains("Assets/"))
            {
                Debug.LogError("Could not determine preset file location type " + path);
            }
            return PresetFileLocation.ProjectFolder;
        }

        private static List<string> GetFilesWithExentionFromFolders(List<string> folderPaths, string fileExtensionWithoutDot)
        {
            List<string> list = new List<string>();
            foreach (string str in folderPaths)
            {
                string[] files = Directory.GetFiles(str, "*." + fileExtensionWithoutDot);
                list.AddRange(files);
            }
            return list;
        }

        public static string GetParticleCurveLibraryExtension(bool singleCurve, bool signedRange)
        {
            string str = "particle";
            if (singleCurve)
            {
                str = str + "Curves";
            }
            else
            {
                str = str + "DoubleCurves";
            }
            if (signedRange)
            {
                return (str + "Signed");
            }
            return (str + string.Empty);
        }

        public static string defaultLibraryLocation
        {
            get
            {
                return GetDefaultFilePathForFileLocation(PresetFileLocation.PreferencesFolder);
            }
        }

        public static string defaultLibraryName
        {
            get
            {
                return "Default";
            }
        }

        public static string defaultPresetLibraryPath
        {
            get
            {
                return Path.Combine(defaultLibraryLocation, defaultLibraryName);
            }
        }
    }
}

