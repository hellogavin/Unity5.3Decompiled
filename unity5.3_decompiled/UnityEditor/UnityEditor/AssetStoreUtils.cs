namespace UnityEditor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using UnityEngine;
    using UnityEngine.Internal;

    internal sealed class AssetStoreUtils
    {
        private const string kAssetStoreUrl = "https://shawarma.unity3d.com";

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string CheckDownload(string id, string url, string[] destination, string key);
        public static void DecryptFile(string inputFile, string outputFile, string keyIV)
        {
            byte[] array = new byte[0x20];
            byte[] buffer2 = new byte[0x10];
            HexStringToByteArray(keyIV, array, 0);
            HexStringToByteArray(keyIV, buffer2, 0x40);
            EditorUtility.DisplayProgressBar("Decrypting", "Decrypting package", 0f);
            FileStream stream = File.Open(inputFile, FileMode.Open);
            FileStream stream2 = File.Open(outputFile, FileMode.CreateNew);
            long length = stream.Length;
            long num2 = 0L;
            AesManaged managed = new AesManaged {
                Key = array,
                IV = buffer2
            };
            CryptoStream stream3 = new CryptoStream(stream, managed.CreateDecryptor(managed.Key, managed.IV), CryptoStreamMode.Read);
            try
            {
                int num3;
                byte[] buffer = new byte[0xa000];
                while ((num3 = stream3.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream2.Write(buffer, 0, num3);
                    num2 += num3;
                    if (EditorUtility.DisplayCancelableProgressBar("Decrypting", "Decrypting package", ((float) num2) / ((float) length)))
                    {
                        throw new Exception("User cancelled decryption");
                    }
                }
            }
            finally
            {
                stream3.Close();
                stream.Close();
                stream2.Close();
                EditorUtility.ClearProgressBar();
            }
        }

        [ExcludeFromDocs]
        public static void Download(string id, string url, string[] destination, string key, string jsonData, bool resumeOK)
        {
            DownloadDoneCallback doneCallback = null;
            Download(id, url, destination, key, jsonData, resumeOK, doneCallback);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Download(string id, string url, string[] destination, string key, string jsonData, bool resumeOK, [DefaultValue("null")] DownloadDoneCallback doneCallback);
        public static string GetAssetStoreSearchUrl()
        {
            return GetAssetStoreUrl().Replace("https", "http");
        }

        public static string GetAssetStoreUrl()
        {
            return "https://shawarma.unity3d.com";
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetLoaderPath();
        public static string GetOfflinePath()
        {
            return Uri.EscapeUriString(EditorApplication.applicationContentsPath + "/Resources/offline.html");
        }

        private static void HexStringToByteArray(string hex, byte[] array, int offset)
        {
            if ((offset + (array.Length * 2)) > hex.Length)
            {
                throw new ArgumentException("Hex string too short");
            }
            for (int i = 0; i < array.Length; i++)
            {
                string s = hex.Substring((i * 2) + offset, 2);
                array[i] = byte.Parse(s, NumberStyles.HexNumber);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RegisterDownloadDelegate(ScriptableObject d);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void UnRegisterDownloadDelegate(ScriptableObject d);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void UpdatePreloading();

        public delegate void DownloadDoneCallback(string package_id, string message, int bytes, int total);
    }
}

