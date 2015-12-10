namespace UnityEditor
{
    using System;
    using System.IO;
    using UnityEditorInternal;
    using UnityEngine;

    public class ScriptableSingleton<T> : ScriptableObject where T: ScriptableObject
    {
        private static T s_Instance;

        protected ScriptableSingleton()
        {
            if (ScriptableSingleton<T>.s_Instance != null)
            {
                Debug.LogError("ScriptableSingleton already exists. Did you query the singleton in a constructor?");
            }
            else
            {
                object obj2 = this;
                ScriptableSingleton<T>.s_Instance = obj2 as T;
            }
        }

        private static void CreateAndLoad()
        {
            string filePath = ScriptableSingleton<T>.GetFilePath();
            if (!string.IsNullOrEmpty(filePath))
            {
                InternalEditorUtility.LoadSerializedFileAndForget(filePath);
            }
            if (ScriptableSingleton<T>.s_Instance == null)
            {
                ScriptableObject.CreateInstance<T>().hideFlags = HideFlags.HideAndDontSave;
            }
        }

        private static string GetFilePath()
        {
            Type type = typeof(T);
            foreach (object obj2 in type.GetCustomAttributes(true))
            {
                if (obj2 is FilePathAttribute)
                {
                    FilePathAttribute attribute = obj2 as FilePathAttribute;
                    return attribute.filepath;
                }
            }
            return null;
        }

        protected virtual void Save(bool saveAsText)
        {
            if (ScriptableSingleton<T>.s_Instance == null)
            {
                Debug.Log("Cannot save ScriptableSingleton: no instance!");
            }
            else
            {
                string filePath = ScriptableSingleton<T>.GetFilePath();
                if (!string.IsNullOrEmpty(filePath))
                {
                    string directoryName = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                    T[] localArray1 = new T[] { ScriptableSingleton<T>.s_Instance };
                    InternalEditorUtility.SaveToSerializedFileAndForget(localArray1, filePath, saveAsText);
                }
            }
        }

        public static T instance
        {
            get
            {
                if (ScriptableSingleton<T>.s_Instance == null)
                {
                    ScriptableSingleton<T>.CreateAndLoad();
                }
                return ScriptableSingleton<T>.s_Instance;
            }
        }
    }
}

