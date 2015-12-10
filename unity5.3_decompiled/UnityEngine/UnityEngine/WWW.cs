namespace UnityEngine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;

    [UsedByNativeCode]
    public sealed class WWW : IDisposable
    {
        private static readonly char[] forbiddenCharacters = new char[] { 
            '\0', '\x0001', '\x0002', '\x0003', '\x0004', '\x0005', '\x0006', '\a', '\b', '\t', '\n', '\v', '\f', '\r', '\x000e', '\x000f', 
            '\x0010', '\x0011', '\x0012', '\x0013', '\x0014', '\x0015', '\x0016', '\x0017', '\x0018', '\x0019', '\x001a', '\x001b', '\x001c', '\x001d', '\x001e', '\x001f', 
            '\x007f', '\0'
         };
        private static readonly char[] forbiddenCharactersForNames = new char[] { ' ' };
        private static readonly string[] forbiddenHeaderKeys = new string[] { 
            "Accept-Charset", "Accept-Encoding", "Access-Control-Request-Headers", "Access-Control-Request-Method", "Connection", "Content-Length", "Cookie", "Cookie2", "Date", "DNT", "Expect", "Host", "Keep-Alive", "Origin", "Referer", "TE", 
            "Trailer", "Transfer-Encoding", "Upgrade", "User-Agent", "Via", "X-Unity-Version"
         };
        internal IntPtr m_Ptr;

        public WWW(string url)
        {
            this.InitWWW(url, null, null);
        }

        public WWW(string url, WWWForm form)
        {
            string[] headers = FlattenedHeadersFrom(form.headers);
            if (this.enforceWebSecurityRestrictions())
            {
                CheckSecurityOnHeaders(headers);
            }
            this.InitWWW(url, form.data, headers);
        }

        public WWW(string url, byte[] postData)
        {
            this.InitWWW(url, postData, null);
        }

        public WWW(string url, byte[] postData, Dictionary<string, string> headers)
        {
            string[] strArray = FlattenedHeadersFrom(headers);
            if (this.enforceWebSecurityRestrictions())
            {
                CheckSecurityOnHeaders(strArray);
            }
            this.InitWWW(url, postData, strArray);
        }

        [Obsolete("This overload is deprecated. Use UnityEngine.WWW.WWW(string, byte[], System.Collections.Generic.Dictionary<string, string>) instead.", true)]
        public WWW(string url, byte[] postData, Hashtable headers)
        {
            Debug.LogError("This overload is deprecated. Use UnityEngine.WWW.WWW(string, byte[], System.Collections.Generic.Dictionary<string, string>) instead");
        }

        internal WWW(string url, Hash128 hash, uint crc)
        {
            INTERNAL_CALL_WWW(this, url, ref hash, crc);
        }

        private static void CheckSecurityOnHeaders(string[] headers)
        {
            bool flag = Application.GetBuildUnityVersion() >= Application.GetNumericUnityVersion("4.3.0b1");
            for (int i = 0; i < headers.Length; i += 2)
            {
                foreach (string str in forbiddenHeaderKeys)
                {
                    if (string.Equals(headers[i], str, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (flag)
                        {
                            throw new ArgumentException("Cannot overwrite header: " + headers[i]);
                        }
                        Debug.LogError("Illegal header overwrite, this will fail in 4.3 and above: " + headers[i]);
                    }
                }
                if (headers[i].StartsWith("Sec-") || headers[i].StartsWith("Proxy-"))
                {
                    if (flag)
                    {
                        throw new ArgumentException("Cannot overwrite header: " + headers[i]);
                    }
                    Debug.LogError("Illegal header overwrite, this will fail in 4.3 and above: " + headers[i]);
                }
                if (((headers[i].IndexOfAny(forbiddenCharacters) > -1) || (headers[i].IndexOfAny(forbiddenCharactersForNames) > -1)) || (headers[i + 1].IndexOfAny(forbiddenCharacters) > -1))
                {
                    if (flag)
                    {
                        throw new ArgumentException("Cannot include control characters in a HTTP header, either as key or value.");
                    }
                    Debug.LogError("Illegal control characters in header, this will fail in 4.3 and above");
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void DestroyWWW(bool cancel);
        public void Dispose()
        {
            this.DestroyWWW(true);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern bool enforceWebSecurityRestrictions();
        [ExcludeFromDocs]
        public static string EscapeURL(string s)
        {
            Encoding e = Encoding.UTF8;
            return EscapeURL(s, e);
        }

        public static string EscapeURL(string s, [DefaultValue("System.Text.Encoding.UTF8")] Encoding e)
        {
            if (s == null)
            {
                return null;
            }
            if (s == string.Empty)
            {
                return string.Empty;
            }
            if (e == null)
            {
                return null;
            }
            return WWWTranscoder.URLEncode(s, e);
        }

        ~WWW()
        {
            this.DestroyWWW(false);
        }

        private static string[] FlattenedHeadersFrom(Dictionary<string, string> headers)
        {
            if (headers == null)
            {
                return null;
            }
            string[] strArray = new string[headers.Count * 2];
            int num = 0;
            foreach (KeyValuePair<string, string> pair in headers)
            {
                strArray[num++] = pair.Key.ToString();
                strArray[num++] = pair.Value.ToString();
            }
            return strArray;
        }

        public AudioClip GetAudioClip(bool threeD)
        {
            return this.GetAudioClip(threeD, false);
        }

        public AudioClip GetAudioClip(bool threeD, bool stream)
        {
            return this.GetAudioClip(threeD, stream, AudioType.UNKNOWN);
        }

        public AudioClip GetAudioClip(bool threeD, bool stream, AudioType audioType)
        {
            return this.GetAudioClipInternal(threeD, stream, false, audioType);
        }

        public AudioClip GetAudioClipCompressed()
        {
            return this.GetAudioClipCompressed(true);
        }

        public AudioClip GetAudioClipCompressed(bool threeD)
        {
            return this.GetAudioClipCompressed(threeD, AudioType.UNKNOWN);
        }

        public AudioClip GetAudioClipCompressed(bool threeD, AudioType audioType)
        {
            return this.GetAudioClipInternal(threeD, false, true, audioType);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern AudioClip GetAudioClipInternal(bool threeD, bool stream, bool compressed, AudioType audioType);
        private Encoding GetTextEncoder()
        {
            string str = null;
            if (this.responseHeaders.TryGetValue("CONTENT-TYPE", out str))
            {
                int index = str.IndexOf("charset", StringComparison.OrdinalIgnoreCase);
                if (index > -1)
                {
                    int num2 = str.IndexOf('=', index);
                    if (num2 > -1)
                    {
                        char[] trimChars = new char[] { '\'', '"' };
                        string name = str.Substring(num2 + 1).Trim().Trim(trimChars).Trim();
                        int length = name.IndexOf(';');
                        if (length > -1)
                        {
                            name = name.Substring(0, length);
                        }
                        try
                        {
                            return Encoding.GetEncoding(name);
                        }
                        catch (Exception)
                        {
                            Debug.Log("Unsupported encoding: '" + name + "'");
                        }
                    }
                }
            }
            return Encoding.UTF8;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern Texture2D GetTexture(bool markNonReadable);
        [Obsolete("All blocking WWW functions have been deprecated, please use one of the asynchronous functions instead.", true)]
        public static Texture2D GetTextureFromURL(string url)
        {
            return new WWW(url).texture;
        }

        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("All blocking WWW functions have been deprecated, please use one of the asynchronous functions instead.", true), WrapperlessIcall]
        public static extern string GetURL(string url);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitWWW(string url, byte[] postData, string[] iHeaders);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_WWW(WWW self, string url, ref Hash128 hash, uint crc);
        [ExcludeFromDocs]
        public static WWW LoadFromCacheOrDownload(string url, int version)
        {
            uint crc = 0;
            return LoadFromCacheOrDownload(url, version, crc);
        }

        [ExcludeFromDocs]
        public static WWW LoadFromCacheOrDownload(string url, Hash128 hash)
        {
            uint crc = 0;
            return LoadFromCacheOrDownload(url, hash, crc);
        }

        public static WWW LoadFromCacheOrDownload(string url, int version, [DefaultValue("0")] uint crc)
        {
            Hash128 hash = new Hash128(0, 0, 0, (uint) version);
            return LoadFromCacheOrDownload(url, hash, crc);
        }

        public static WWW LoadFromCacheOrDownload(string url, Hash128 hash, [DefaultValue("0")] uint crc)
        {
            return new WWW(url, hash, crc);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void LoadImageIntoTexture(Texture2D tex);
        [Obsolete("LoadUnityWeb is no longer supported. Please use javascript to reload the web player on a different url instead", true)]
        public void LoadUnityWeb()
        {
        }

        internal static Dictionary<string, string> ParseHTTPHeaderString(string input)
        {
            if (input == null)
            {
                throw new ArgumentException("input was null to ParseHTTPHeaderString");
            }
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            StringReader reader = new StringReader(input);
            int num = 0;
            while (true)
            {
                string str = reader.ReadLine();
                if (str == null)
                {
                    return dictionary;
                }
                if ((num++ == 0) && str.StartsWith("HTTP"))
                {
                    dictionary["STATUS"] = str;
                }
                else
                {
                    int index = str.IndexOf(": ");
                    if (index != -1)
                    {
                        string str2 = str.Substring(0, index).ToUpper();
                        string str3 = str.Substring(index + 2);
                        dictionary[str2] = str3;
                    }
                }
            }
        }

        [ExcludeFromDocs]
        public static string UnEscapeURL(string s)
        {
            Encoding e = Encoding.UTF8;
            return UnEscapeURL(s, e);
        }

        public static string UnEscapeURL(string s, [DefaultValue("System.Text.Encoding.UTF8")] Encoding e)
        {
            if (s == null)
            {
                return null;
            }
            if ((s.IndexOf('%') == -1) && (s.IndexOf('+') == -1))
            {
                return s;
            }
            return WWWTranscoder.URLDecode(s, e);
        }

        public AssetBundle assetBundle { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public AudioClip audioClip
        {
            get
            {
                return this.GetAudioClip(true);
            }
        }

        public byte[] bytes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int bytesDownloaded { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("Please use WWW.text instead")]
        public string data
        {
            get
            {
                return this.text;
            }
        }

        internal static Encoding DefaultEncoding
        {
            get
            {
                return Encoding.ASCII;
            }
        }

        public string error { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isDone { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public MovieTexture movie { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("Property WWW.oggVorbis has been deprecated. Use WWW.audioClip instead (UnityUpgradable).", true), EditorBrowsable(EditorBrowsableState.Never)]
        public AudioClip oggVorbis
        {
            get
            {
                return null;
            }
        }

        public float progress { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Dictionary<string, string> responseHeaders
        {
            get
            {
                if (!this.isDone)
                {
                    throw new UnityException("WWW is not finished downloading yet");
                }
                return ParseHTTPHeaderString(this.responseHeadersString);
            }
        }

        private string responseHeadersString { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int size { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string text
        {
            get
            {
                if (!this.isDone)
                {
                    throw new UnityException("WWW is not ready downloading yet");
                }
                byte[] bytes = this.bytes;
                return this.GetTextEncoder().GetString(bytes, 0, bytes.Length);
            }
        }

        public Texture2D texture
        {
            get
            {
                return this.GetTexture(false);
            }
        }

        public Texture2D textureNonReadable
        {
            get
            {
                return this.GetTexture(true);
            }
        }

        public ThreadPriority threadPriority { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float uploadProgress { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string url { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

