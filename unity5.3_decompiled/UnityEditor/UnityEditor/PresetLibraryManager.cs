namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class PresetLibraryManager : ScriptableSingleton<PresetLibraryManager>
    {
        private List<LibraryCache> m_LibraryCaches = new List<LibraryCache>();
        private static string s_LastError;

        public T CreateLibrary<T>(ScriptableObjectSaveLoadHelper<T> helper, string presetLibraryPathWithoutExtension) where T: ScriptableObject
        {
            string libaryNameFromPath = this.GetLibaryNameFromPath(presetLibraryPathWithoutExtension);
            if (!InternalEditorUtility.IsValidFileName(libaryNameFromPath))
            {
                string displayStringOfInvalidCharsOfFileName = InternalEditorUtility.GetDisplayStringOfInvalidCharsOfFileName(libaryNameFromPath);
                if (displayStringOfInvalidCharsOfFileName.Length > 0)
                {
                    s_LastError = string.Format("A library filename cannot contain the following character{0}:  {1}", (displayStringOfInvalidCharsOfFileName.Length <= 1) ? string.Empty : "s", displayStringOfInvalidCharsOfFileName);
                }
                else
                {
                    s_LastError = "Invalid filename";
                }
                return null;
            }
            if (this.GetLibrary<T>(helper, presetLibraryPathWithoutExtension) != null)
            {
                s_LastError = "Library '" + libaryNameFromPath + "' already exists! Ensure a unique name.";
                return null;
            }
            T item = helper.Create();
            item.hideFlags = this.libraryHideFlag;
            LibraryCache presetLibraryCache = this.GetPresetLibraryCache(helper.fileExtensionWithoutDot);
            presetLibraryCache.loadedLibraries.Add(item);
            presetLibraryCache.loadedLibraryIDs.Add(presetLibraryPathWithoutExtension);
            s_LastError = null;
            return item;
        }

        public void GetAvailableLibraries<T>(ScriptableObjectSaveLoadHelper<T> helper, out List<string> preferencesLibs, out List<string> projectLibs) where T: ScriptableObject
        {
            preferencesLibs = PresetLibraryLocations.GetAvailableFilesWithExtensionOnTheHDD(PresetFileLocation.PreferencesFolder, helper.fileExtensionWithoutDot);
            projectLibs = PresetLibraryLocations.GetAvailableFilesWithExtensionOnTheHDD(PresetFileLocation.ProjectFolder, helper.fileExtensionWithoutDot);
        }

        public string GetLastError()
        {
            string str = s_LastError;
            s_LastError = null;
            return str;
        }

        private string GetLibaryNameFromPath(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }

        public T GetLibrary<T>(ScriptableObjectSaveLoadHelper<T> helper, string presetLibraryPathWithoutExtension) where T: ScriptableObject
        {
            LibraryCache presetLibraryCache = this.GetPresetLibraryCache(helper.fileExtensionWithoutDot);
            for (int i = 0; i < presetLibraryCache.loadedLibraryIDs.Count; i++)
            {
                if (presetLibraryCache.loadedLibraryIDs[i] == presetLibraryPathWithoutExtension)
                {
                    if (presetLibraryCache.loadedLibraries[i] != null)
                    {
                        return (presetLibraryCache.loadedLibraries[i] as T);
                    }
                    presetLibraryCache.loadedLibraries.RemoveAt(i);
                    presetLibraryCache.loadedLibraryIDs.RemoveAt(i);
                    Debug.LogError("Invalid library detected: Reload " + presetLibraryCache.loadedLibraryIDs[i] + " from HDD");
                    break;
                }
            }
            T item = helper.Load(presetLibraryPathWithoutExtension);
            if (item != null)
            {
                item.hideFlags = this.libraryHideFlag;
                presetLibraryCache.loadedLibraries.Add(item);
                presetLibraryCache.loadedLibraryIDs.Add(presetLibraryPathWithoutExtension);
                return item;
            }
            return null;
        }

        private LibraryCache GetPresetLibraryCache(string identifier)
        {
            foreach (LibraryCache cache in this.m_LibraryCaches)
            {
                if (cache.identifier == identifier)
                {
                    return cache;
                }
            }
            LibraryCache item = new LibraryCache(identifier);
            this.m_LibraryCaches.Add(item);
            return item;
        }

        public void SaveLibrary<T>(ScriptableObjectSaveLoadHelper<T> helper, T library, string presetLibraryPathWithoutExtension) where T: ScriptableObject
        {
            bool flag = File.Exists(presetLibraryPathWithoutExtension + "." + helper.fileExtensionWithoutDot);
            helper.Save(library, presetLibraryPathWithoutExtension);
            if (!flag)
            {
                AssetDatabase.Refresh();
            }
        }

        public void UnloadAllLibrariesFor<T>(ScriptableObjectSaveLoadHelper<T> helper) where T: ScriptableObject
        {
            for (int i = 0; i < this.m_LibraryCaches.Count; i++)
            {
                if (this.m_LibraryCaches[i].identifier == helper.fileExtensionWithoutDot)
                {
                    this.m_LibraryCaches[i].UnloadScriptableObjects();
                    this.m_LibraryCaches.RemoveAt(i);
                    break;
                }
            }
        }

        private HideFlags libraryHideFlag
        {
            get
            {
                return HideFlags.DontSave;
            }
        }

        private class LibraryCache
        {
            private string m_Identifier;
            private List<ScriptableObject> m_LoadedLibraries = new List<ScriptableObject>();
            private List<string> m_LoadedLibraryIDs = new List<string>();

            public LibraryCache(string identifier)
            {
                this.m_Identifier = identifier;
            }

            public void UnloadScriptableObjects()
            {
                foreach (ScriptableObject obj2 in this.m_LoadedLibraries)
                {
                    Object.DestroyImmediate(obj2);
                }
                this.m_LoadedLibraries.Clear();
                this.m_LoadedLibraryIDs.Clear();
            }

            public string identifier
            {
                get
                {
                    return this.m_Identifier;
                }
            }

            public List<ScriptableObject> loadedLibraries
            {
                get
                {
                    return this.m_LoadedLibraries;
                }
            }

            public List<string> loadedLibraryIDs
            {
                get
                {
                    return this.m_LoadedLibraryIDs;
                }
            }
        }
    }
}

