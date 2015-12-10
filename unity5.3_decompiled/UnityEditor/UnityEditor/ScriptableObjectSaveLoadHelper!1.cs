namespace UnityEditor
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class ScriptableObjectSaveLoadHelper<T> where T: ScriptableObject
    {
        public ScriptableObjectSaveLoadHelper(string fileExtensionWithoutDot, SaveType saveType)
        {
            this.saveType = saveType;
            char[] trimChars = new char[] { '.' };
            this.fileExtensionWithoutDot = fileExtensionWithoutDot.TrimStart(trimChars);
        }

        private string AppendFileExtensionIfNeeded(string path)
        {
            if (!Path.HasExtension(path) && !string.IsNullOrEmpty(this.fileExtensionWithoutDot))
            {
                return (path + "." + this.fileExtensionWithoutDot);
            }
            return path;
        }

        public T Create()
        {
            return ScriptableObject.CreateInstance<T>();
        }

        public T Load(string filePath)
        {
            filePath = this.AppendFileExtensionIfNeeded(filePath);
            if (!string.IsNullOrEmpty(filePath))
            {
                Object[] objArray = InternalEditorUtility.LoadSerializedFileAndForget(filePath);
                if ((objArray != null) && (objArray.Length > 0))
                {
                    return (objArray[0] as T);
                }
            }
            return null;
        }

        public void Save(T t, string filePath)
        {
            if (t == null)
            {
                Debug.LogError("Cannot save scriptableObject: its null!");
            }
            else if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogError("Invalid path: '" + filePath + "'");
            }
            else
            {
                string directoryName = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                filePath = this.AppendFileExtensionIfNeeded(filePath);
                T[] localArray1 = new T[] { t };
                InternalEditorUtility.SaveToSerializedFileAndForget(localArray1, filePath, this.saveType == SaveType.Text);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", this.fileExtensionWithoutDot, this.saveType);
        }

        public string fileExtensionWithoutDot { get; private set; }

        private SaveType saveType { get; set; }
    }
}

