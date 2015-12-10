using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

internal class GenerateIconsWithMipLevels
{
    private static string k_IconSourceFolder = "Assets/MipLevels For Icons/";
    private static string k_IconTargetFolder = "Assets/Editor Default Resources/Icons/Generated";

    private static void Blit(Texture2D source, Texture2D dest, int mipLevel)
    {
        Color32[] colors = source.GetPixels32();
        for (int i = 0; i < colors.Length; i++)
        {
            Color32 color = colors[i];
            if (color.a >= 3)
            {
                color.a = (byte) (color.a - 3);
            }
            colors[i] = color;
        }
        dest.SetPixels32(colors, mipLevel);
    }

    private static bool BlitMip(Texture2D iconWithMips, string mipFile, int mipLevel)
    {
        Texture2D source = GetTexture2D(mipFile);
        if (source != null)
        {
            Blit(source, iconWithMips, mipLevel);
            return true;
        }
        Debug.Log("Mip file NOT found: " + mipFile);
        return false;
    }

    private static Texture2D CreateIconWithMipLevels(InputData inputData, string baseName, List<string> assetPathsOfAllIcons)
    {
        <CreateIconWithMipLevels>c__AnonStorey36 storey = new <CreateIconWithMipLevels>c__AnonStorey36 {
            baseName = baseName,
            inputData = inputData
        };
        List<string> list = assetPathsOfAllIcons.FindAll(new Predicate<string>(storey.<>m__4D));
        List<Texture2D> list2 = new List<Texture2D>();
        foreach (string str in list)
        {
            Texture2D item = GetTexture2D(str);
            if (item != null)
            {
                list2.Add(item);
            }
            else
            {
                Debug.LogError("Mip not found " + str);
            }
        }
        int num = 0x1869f;
        int width = 0;
        foreach (Texture2D textured2 in list2)
        {
            int num3 = textured2.width;
            if (num3 > width)
            {
                width = num3;
            }
            if (num3 < num)
            {
                num = num3;
            }
        }
        if (width == 0)
        {
            return null;
        }
        Texture2D iconWithMips = new Texture2D(width, width, TextureFormat.ARGB32, true, true);
        if (BlitMip(iconWithMips, storey.inputData.GetMipFileName(storey.baseName, width), 0))
        {
            iconWithMips.Apply(true);
        }
        else
        {
            return iconWithMips;
        }
        int mipResolution = width;
        for (int i = 1; i < iconWithMips.mipmapCount; i++)
        {
            mipResolution /= 2;
            if (mipResolution < num)
            {
                break;
            }
            BlitMip(iconWithMips, storey.inputData.GetMipFileName(storey.baseName, mipResolution), i);
        }
        iconWithMips.Apply(false, true);
        return iconWithMips;
    }

    private static void DeleteFile(string file)
    {
        if (AssetDatabase.GetMainAssetInstanceID(file) != 0)
        {
            Debug.Log("Deleted unused file: " + file);
            AssetDatabase.DeleteAsset(file);
        }
    }

    private static void EnsureFolderIsCreated(string targetFolder)
    {
        if (AssetDatabase.GetMainAssetInstanceID(targetFolder) == 0)
        {
            Debug.Log("Created target folder " + targetFolder);
            AssetDatabase.CreateFolder(Path.GetDirectoryName(targetFolder), Path.GetFileName(targetFolder));
        }
    }

    private static void EnsureFolderIsCreatedRecursively(string targetFolder)
    {
        if (AssetDatabase.GetMainAssetInstanceID(targetFolder) == 0)
        {
            EnsureFolderIsCreatedRecursively(Path.GetDirectoryName(targetFolder));
            Debug.Log("Created target folder " + targetFolder);
            AssetDatabase.CreateFolder(Path.GetDirectoryName(targetFolder), Path.GetFileName(targetFolder));
        }
    }

    public static void GenerateAllIconsWithMipLevels()
    {
        InputData inputData = new InputData {
            sourceFolder = k_IconSourceFolder,
            targetFolder = k_IconTargetFolder,
            mipIdentifier = "@",
            mipFileExtension = "png"
        };
        if (AssetDatabase.GetMainAssetInstanceID(inputData.targetFolder) != 0)
        {
            AssetDatabase.DeleteAsset(inputData.targetFolder);
            AssetDatabase.Refresh();
        }
        EnsureFolderIsCreated(inputData.targetFolder);
        float realtimeSinceStartup = Time.realtimeSinceStartup;
        GenerateIconsWithMips(inputData);
        Debug.Log(string.Format("Generated {0} icons with mip levels in {1} seconds", inputData.generatedFileNames.Count, Time.realtimeSinceStartup - realtimeSinceStartup));
        RemoveUnusedFiles(inputData.generatedFileNames);
        AssetDatabase.Refresh();
        InternalEditorUtility.RepaintAllViews();
    }

    private static void GenerateIcon(InputData inputData, string baseName, List<string> assetPathsOfAllIcons)
    {
        string path = inputData.targetFolder + "/" + baseName + " Icon.asset";
        EnsureFolderIsCreatedRecursively(Path.GetDirectoryName(path));
        Texture2D asset = CreateIconWithMipLevels(inputData, baseName, assetPathsOfAllIcons);
        if (asset == null)
        {
            Debug.Log("CreateIconWithMipLevels failed");
        }
        else
        {
            asset.name = baseName + " Icon.png";
            AssetDatabase.CreateAsset(asset, path);
            inputData.generatedFileNames.Add(path);
        }
    }

    private static void GenerateIconsWithMips(InputData inputData)
    {
        List<string> files = GetIconAssetPaths(inputData.sourceFolder, inputData.mipIdentifier, inputData.mipFileExtension);
        if (files.Count == 0)
        {
            Debug.LogWarning("No mip files found for generating icons! Searching in: " + inputData.sourceFolder + ", for files with extension: " + inputData.mipFileExtension);
        }
        foreach (string str in GetBaseNames(inputData, files))
        {
            GenerateIcon(inputData, str, files);
        }
    }

    public static void GenerateSelectedIconsWithMips()
    {
        if (Selection.activeInstanceID == 0)
        {
            Debug.Log("Ensure to select a mip texture..." + Selection.activeInstanceID);
        }
        else
        {
            InputData inputData = new InputData {
                sourceFolder = k_IconSourceFolder,
                targetFolder = k_IconTargetFolder,
                mipIdentifier = "@",
                mipFileExtension = "png"
            };
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            if (assetPath.IndexOf(inputData.sourceFolder) < 0)
            {
                Debug.Log("Selection is not a valid mip texture, it should be located in: " + inputData.sourceFolder);
            }
            else if (assetPath.IndexOf(inputData.mipIdentifier) < 0)
            {
                Debug.Log("Selection does not have a valid mip identifier " + assetPath + "  " + inputData.mipIdentifier);
            }
            else
            {
                float realtimeSinceStartup = Time.realtimeSinceStartup;
                string baseName = assetPath.Replace(inputData.sourceFolder, string.Empty);
                baseName = baseName.Substring(0, baseName.LastIndexOf(inputData.mipIdentifier));
                List<string> assetPathsOfAllIcons = GetIconAssetPaths(inputData.sourceFolder, inputData.mipIdentifier, inputData.mipFileExtension);
                EnsureFolderIsCreated(inputData.targetFolder);
                GenerateIcon(inputData, baseName, assetPathsOfAllIcons);
                Debug.Log(string.Format("Generated {0} icon with mip levels in {1} seconds", baseName, Time.realtimeSinceStartup - realtimeSinceStartup));
                InternalEditorUtility.RepaintAllViews();
            }
        }
    }

    private static string[] GetBaseNames(InputData inputData, List<string> files)
    {
        string[] collection = new string[files.Count];
        int length = inputData.sourceFolder.Length;
        for (int i = 0; i < files.Count; i++)
        {
            collection[i] = files[i].Substring(length, files[i].IndexOf(inputData.mipIdentifier) - length);
        }
        HashSet<string> set = new HashSet<string>(collection);
        collection = new string[set.Count];
        set.CopyTo(collection);
        return collection;
    }

    private static List<string> GetIconAssetPaths(string folderPath, string mustHaveIdentifier, string extension)
    {
        <GetIconAssetPaths>c__AnonStorey37 storey = new <GetIconAssetPaths>c__AnonStorey37 {
            mustHaveIdentifier = mustHaveIdentifier
        };
        string uriString = Path.Combine(Directory.GetCurrentDirectory(), folderPath);
        Uri uri = new Uri(uriString);
        List<string> list = new List<string>(Directory.GetFiles(uriString, "*." + extension, SearchOption.AllDirectories));
        list.RemoveAll(new Predicate<string>(storey.<>m__4E));
        for (int i = 0; i < list.Count; i++)
        {
            Uri uri2 = new Uri(list[i]);
            list[i] = folderPath + uri.MakeRelativeUri(uri2).ToString();
        }
        return list;
    }

    private static Texture2D GetTexture2D(string path)
    {
        return (AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D);
    }

    private static void RemoveUnusedFiles(List<string> generatedFiles)
    {
        for (int i = 0; i < generatedFiles.Count; i++)
        {
            string file = generatedFiles[i].Replace("Icons/Generated", "Icons").Replace(".asset", ".png");
            DeleteFile(file);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
            if (!fileNameWithoutExtension.StartsWith("d_"))
            {
                DeleteFile(file.Replace(fileNameWithoutExtension, "d_" + fileNameWithoutExtension));
            }
        }
        AssetDatabase.Refresh();
    }

    [CompilerGenerated]
    private sealed class <CreateIconWithMipLevels>c__AnonStorey36
    {
        internal string baseName;
        internal GenerateIconsWithMipLevels.InputData inputData;

        internal bool <>m__4D(string o)
        {
            return (o.IndexOf('/' + this.baseName + this.inputData.mipIdentifier) >= 0);
        }
    }

    [CompilerGenerated]
    private sealed class <GetIconAssetPaths>c__AnonStorey37
    {
        internal string mustHaveIdentifier;

        internal bool <>m__4E(string o)
        {
            return (o.IndexOf(this.mustHaveIdentifier) < 0);
        }
    }

    private class InputData
    {
        public List<string> generatedFileNames = new List<string>();
        public string mipFileExtension;
        public string mipIdentifier;
        public string sourceFolder;
        public string targetFolder;

        public string GetMipFileName(string baseName, int mipResolution)
        {
            object[] objArray1 = new object[] { this.sourceFolder, baseName, this.mipIdentifier, mipResolution, ".", this.mipFileExtension };
            return string.Concat(objArray1);
        }
    }
}

