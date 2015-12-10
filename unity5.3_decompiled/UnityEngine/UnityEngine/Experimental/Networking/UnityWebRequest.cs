namespace UnityEngine.Experimental.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public sealed class UnityWebRequest : IDisposable
    {
        public const string kHttpVerbGET = "GET";
        public const string kHttpVerbHEAD = "HEAD";
        public const string kHttpVerbPOST = "POST";
        public const string kHttpVerbPUT = "PUT";
        public const string kHttpVerbCREATE = "CREATE";
        public const string kHttpVerbDELETE = "DELETE";
        [NonSerialized]
        internal IntPtr m_Ptr;
        private static Regex domainRegex = new Regex(@"^\s*\w+(?:\.\w+)+\s*$");
        private static readonly string[] forbiddenHeaderKeys = new string[] { 
            "accept-charset", "accept-encoding", "access-control-request-headers", "access-control-request-method", "connection", "content-length", "cookie", "cookie2", "date", "dnt", "expect", "host", "keep-alive", "origin", "referer", "te", 
            "trailer", "transfer-encoding", "upgrade", "user-agent", "via", "x-unity-version"
         };
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map1;
        public UnityWebRequest()
        {
            this.InternalCreate();
            this.InternalSetDefaults();
        }

        public UnityWebRequest(string url)
        {
            this.InternalCreate();
            this.InternalSetDefaults();
            this.url = url;
        }

        public UnityWebRequest(string url, string method)
        {
            this.InternalCreate();
            this.InternalSetDefaults();
            this.url = url;
            this.method = method;
        }

        public UnityWebRequest(string url, string method, DownloadHandler downloadHandler, UploadHandler uploadHandler)
        {
            this.InternalCreate();
            this.InternalSetDefaults();
            this.url = url;
            this.method = method;
            this.downloadHandler = downloadHandler;
            this.uploadHandler = uploadHandler;
        }

        public static UnityWebRequest Get(string uri)
        {
            return new UnityWebRequest(uri, "GET", new DownloadHandlerBuffer(), null);
        }

        public static UnityWebRequest Delete(string uri)
        {
            return new UnityWebRequest(uri, "DELETE");
        }

        public static UnityWebRequest Head(string uri)
        {
            return new UnityWebRequest(uri, "HEAD");
        }

        public static UnityWebRequest GetTexture(string uri)
        {
            return GetTexture(uri, false);
        }

        public static UnityWebRequest GetTexture(string uri, bool nonReadable)
        {
            return new UnityWebRequest(uri, "GET", new DownloadHandlerTexture(nonReadable), null);
        }

        public static UnityWebRequest GetAssetBundle(string uri)
        {
            return GetAssetBundle(uri, 0);
        }

        public static UnityWebRequest GetAssetBundle(string uri, uint crc)
        {
            return new UnityWebRequest(uri, "GET", new DownloadHandlerAssetBundle(uri, crc), null);
        }

        public static UnityWebRequest GetAssetBundle(string uri, uint version, uint crc)
        {
            return new UnityWebRequest(uri, "GET", new DownloadHandlerAssetBundle(uri, version, crc), null);
        }

        public static UnityWebRequest GetAssetBundle(string uri, Hash128 hash, uint crc)
        {
            return new UnityWebRequest(uri, "GET", new DownloadHandlerAssetBundle(uri, hash, crc), null);
        }

        public static UnityWebRequest Put(string uri, byte[] bodyData)
        {
            return new UnityWebRequest(uri, "PUT", new DownloadHandlerBuffer(), new UploadHandlerRaw(bodyData));
        }

        public static UnityWebRequest Put(string uri, string bodyData)
        {
            return new UnityWebRequest(uri, "PUT", new DownloadHandlerBuffer(), new UploadHandlerRaw(Encoding.UTF8.GetBytes(bodyData)));
        }

        public static UnityWebRequest Post(string uri, string postData)
        {
            UnityWebRequest request = new UnityWebRequest(uri, "POST");
            string s = WWWTranscoder.URLEncode(postData, Encoding.UTF8);
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(s));
            request.uploadHandler.contentType = "application/x-www-form-urlencoded";
            request.downloadHandler = new DownloadHandlerBuffer();
            return request;
        }

        public static UnityWebRequest Post(string uri, WWWForm formData)
        {
            UnityWebRequest request = new UnityWebRequest(uri, "POST") {
                uploadHandler = new UploadHandlerRaw(formData.data),
                downloadHandler = new DownloadHandlerBuffer()
            };
            foreach (KeyValuePair<string, string> pair in formData.headers)
            {
                request.SetRequestHeader(pair.Key, pair.Value);
            }
            return request;
        }

        public static UnityWebRequest Post(string uri, List<IMultipartFormSection> multipartFormSections)
        {
            byte[] boundary = GenerateBoundary();
            return Post(uri, multipartFormSections, boundary);
        }

        public static UnityWebRequest Post(string uri, List<IMultipartFormSection> multipartFormSections, byte[] boundary)
        {
            UnityWebRequest request = new UnityWebRequest(uri, "POST");
            UploadHandler handler = new UploadHandlerRaw(SerializeFormSections(multipartFormSections, boundary)) {
                contentType = "multipart/form-data; boundary=" + Encoding.UTF8.GetString(boundary, 0, boundary.Length)
            };
            request.uploadHandler = handler;
            request.downloadHandler = new DownloadHandlerBuffer();
            return request;
        }

        public static UnityWebRequest Post(string uri, Dictionary<string, string> formFields)
        {
            UnityWebRequest request = new UnityWebRequest(uri, "POST");
            UploadHandler handler = new UploadHandlerRaw(SerializeSimpleForm(formFields)) {
                contentType = "application/x-www-form-urlencoded"
            };
            request.uploadHandler = handler;
            request.downloadHandler = new DownloadHandlerBuffer();
            return request;
        }

        public static byte[] SerializeFormSections(List<IMultipartFormSection> multipartFormSections, byte[] boundary)
        {
            byte[] bytes = Encoding.UTF8.GetBytes("\r\n");
            int capacity = 0;
            foreach (IMultipartFormSection section in multipartFormSections)
            {
                capacity += 0x40 + section.sectionData.Length;
            }
            List<byte> list = new List<byte>(capacity);
            foreach (IMultipartFormSection section2 in multipartFormSections)
            {
                string str = "form-data";
                string sectionName = section2.sectionName;
                string fileName = section2.fileName;
                if (!string.IsNullOrEmpty(fileName))
                {
                    str = "file";
                }
                string s = "Content-Disposition: " + str;
                if (!string.IsNullOrEmpty(sectionName))
                {
                    s = s + "; name=\"" + sectionName + "\"";
                }
                if (!string.IsNullOrEmpty(fileName))
                {
                    s = s + "; filename=\"" + fileName + "\"";
                }
                s = s + "\r\n";
                string contentType = section2.contentType;
                if (!string.IsNullOrEmpty(contentType))
                {
                    s = s + "Content-Type: " + contentType + "\r\n";
                }
                list.AddRange(boundary);
                list.AddRange(bytes);
                list.AddRange(Encoding.UTF8.GetBytes(s));
                list.AddRange(bytes);
                list.AddRange(section2.sectionData);
            }
            list.TrimExcess();
            return list.ToArray();
        }

        public static byte[] GenerateBoundary()
        {
            byte[] buffer = new byte[40];
            for (int i = 0; i < 40; i++)
            {
                int num2 = Random.Range(0x30, 110);
                if (num2 > 0x39)
                {
                    num2 += 7;
                }
                if (num2 > 90)
                {
                    num2 += 6;
                }
                buffer[i] = (byte) num2;
            }
            return buffer;
        }

        public static byte[] SerializeSimpleForm(Dictionary<string, string> formFields)
        {
            string s = string.Empty;
            foreach (KeyValuePair<string, string> pair in formFields)
            {
                if (s.Length > 0)
                {
                    s = s + "&";
                }
                s = s + Uri.EscapeDataString(pair.Key) + "=" + Uri.EscapeDataString(pair.Value);
            }
            return Encoding.UTF8.GetBytes(s);
        }

        public bool disposeDownloadHandlerOnDispose { get; set; }
        public bool disposeUploadHandlerOnDispose { get; set; }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void InternalCreate();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void InternalDestroy();
        private void InternalSetDefaults()
        {
            this.disposeDownloadHandlerOnDispose = true;
            this.disposeUploadHandlerOnDispose = true;
        }

        ~UnityWebRequest()
        {
            this.InternalDestroy();
        }

        public void Dispose()
        {
            if (this.disposeDownloadHandlerOnDispose)
            {
                DownloadHandler downloadHandler = this.downloadHandler;
                if (downloadHandler != null)
                {
                    downloadHandler.Dispose();
                }
            }
            if (this.disposeUploadHandlerOnDispose)
            {
                UploadHandler uploadHandler = this.uploadHandler;
                if (uploadHandler != null)
                {
                    uploadHandler.Dispose();
                }
            }
            this.InternalDestroy();
            GC.SuppressFinalize(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern AsyncOperation InternalBegin();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void InternalAbort();
        public AsyncOperation Send()
        {
            return this.InternalBegin();
        }

        public void Abort()
        {
            this.InternalAbort();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void InternalSetMethod(UnityWebRequestMethod methodType);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void InternalSetCustomMethod(string customMethodName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern int InternalGetMethod();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern string InternalGetCustomMethod();
        public string method
        {
            get
            {
                switch (this.InternalGetMethod())
                {
                    case 0:
                        return "GET";

                    case 1:
                        return "POST";

                    case 2:
                        return "PUT";

                    case 3:
                        return "HEAD";
                }
                return this.InternalGetCustomMethod();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Cannot set a UnityWebRequest's method to an empty or null string");
                }
                string key = value.ToUpper();
                if (key != null)
                {
                    int num;
                    if (<>f__switch$map1 == null)
                    {
                        Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
                        dictionary.Add("GET", 0);
                        dictionary.Add("POST", 1);
                        dictionary.Add("PUT", 2);
                        dictionary.Add("HEAD", 3);
                        <>f__switch$map1 = dictionary;
                    }
                    if (<>f__switch$map1.TryGetValue(key, out num))
                    {
                        switch (num)
                        {
                            case 0:
                                this.InternalSetMethod(UnityWebRequestMethod.Get);
                                return;

                            case 1:
                                this.InternalSetMethod(UnityWebRequestMethod.Post);
                                return;

                            case 2:
                                this.InternalSetMethod(UnityWebRequestMethod.Put);
                                return;

                            case 3:
                                this.InternalSetMethod(UnityWebRequestMethod.Head);
                                return;
                        }
                    }
                }
                this.InternalSetCustomMethod(value.ToUpper());
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern int InternalGetError();
        public string error { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public bool useHttpContinue { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public string url
        {
            get
            {
                return this.InternalGetUrl();
            }
            set
            {
                string input = value;
                string uriString = "http://localhost/";
                Uri baseUri = new Uri(uriString);
                if (input.StartsWith("//"))
                {
                    input = baseUri.Scheme + ":" + input;
                }
                if (input.StartsWith("/"))
                {
                    input = baseUri.Scheme + "://" + baseUri.Host + input;
                }
                if (domainRegex.IsMatch(input))
                {
                    input = baseUri.Scheme + "://" + input;
                }
                Uri uri2 = null;
                try
                {
                    uri2 = new Uri(input);
                }
                catch (FormatException exception)
                {
                    try
                    {
                        uri2 = new Uri(baseUri, input);
                    }
                    catch (FormatException)
                    {
                        throw exception;
                    }
                }
                this.InternalSetUrl(uri2.AbsoluteUri);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern string InternalGetUrl();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void InternalSetUrl(string url);
        public long responseCode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public float uploadProgress { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public bool isModifiable { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public bool isDone { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public bool isError { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public float downloadProgress { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public ulong uploadedBytes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public ulong downloadedBytes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public int redirectLimit { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public bool chunkedTransfer { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetRequestHeader(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void InternalSetRequestHeader(string name, string value);
        public void SetRequestHeader(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Cannot set a Request Header with a null or empty name");
            }
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Cannot set a Request header with a null or empty value");
            }
            if (!IsHeaderNameLegal(name))
            {
                throw new ArgumentException("Cannot set Request Header " + name + " - name contains illegal characters or is not user-overridable");
            }
            if (!IsHeaderValueLegal(value))
            {
                throw new ArgumentException("Cannot set Request Header - value contains illegal characters");
            }
            this.InternalSetRequestHeader(name, value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetResponseHeader(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern string[] InternalGetResponseHeaderKeys();
        public Dictionary<string, string> GetResponseHeaders()
        {
            string[] responseHeaderKeys = this.InternalGetResponseHeaderKeys();
            if (responseHeaderKeys == null)
            {
                return null;
            }
            Dictionary<string, string> dictionary = new Dictionary<string, string>(responseHeaderKeys.Length);
            for (int i = 0; i < responseHeaderKeys.Length; i++)
            {
                string responseHeader = this.GetResponseHeader(responseHeaderKeys[i]);
                dictionary.Add(responseHeaderKeys[i], responseHeader);
            }
            return dictionary;
        }

        public UploadHandler uploadHandler { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public DownloadHandler downloadHandler { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        private static bool ContainsForbiddenCharacters(string s, int firstAllowedCharCode)
        {
            foreach (char ch in s)
            {
                if ((ch < firstAllowedCharCode) || (ch == '\x007f'))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsHeaderNameLegal(string headerName)
        {
            if (string.IsNullOrEmpty(headerName))
            {
                return false;
            }
            headerName = headerName.ToLower();
            if (ContainsForbiddenCharacters(headerName, 0x21))
            {
                return false;
            }
            if (headerName.StartsWith("sec-") || headerName.StartsWith("proxy-"))
            {
                return false;
            }
            foreach (string str in forbiddenHeaderKeys)
            {
                if (string.Equals(headerName, str))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsHeaderValueLegal(string headerValue)
        {
            if (string.IsNullOrEmpty(headerValue))
            {
                return false;
            }
            if (ContainsForbiddenCharacters(headerValue, 0x20))
            {
                return false;
            }
            return true;
        }

        private static string GetErrorDescription(UnityWebRequestError errorCode)
        {
            switch (errorCode)
            {
                case UnityWebRequestError.OK:
                    return "No Error";

                case UnityWebRequestError.SDKError:
                    return "Internal Error With Transport Layer";

                case UnityWebRequestError.UnsupportedProtocol:
                    return "Specified Transport Protocol is Unsupported";

                case UnityWebRequestError.MalformattedUrl:
                    return "URL is Malformatted";

                case UnityWebRequestError.CannotResolveProxy:
                    return "Unable to resolve specified proxy server";

                case UnityWebRequestError.CannotResolveHost:
                    return "Unable to resolve host specified in URL";

                case UnityWebRequestError.CannotConnectToHost:
                    return "Unable to connect to host specified in URL";

                case UnityWebRequestError.AccessDenied:
                    return "Remote server denied access to the specified URL";

                case UnityWebRequestError.GenericHTTPError:
                    return "Unknown/Generic HTTP Error - Check HTTP Error code";

                case UnityWebRequestError.WriteError:
                    return "Error when transmitting request to remote server - transmission terminated prematurely";

                case UnityWebRequestError.ReadError:
                    return "Error when reading response from remote server - transmission terminated prematurely";

                case UnityWebRequestError.OutOfMemory:
                    return "Out of Memory";

                case UnityWebRequestError.Timeout:
                    return "Timeout occurred while waiting for response from remote server";

                case UnityWebRequestError.HTTPPostError:
                    return "Error while transmitting HTTP POST body data";

                case UnityWebRequestError.SSLCannotConnect:
                    return "Unable to connect to SSL server at remote host";

                case UnityWebRequestError.Aborted:
                    return "Request was manually aborted by local code";

                case UnityWebRequestError.TooManyRedirects:
                    return "Redirect limit exceeded";

                case UnityWebRequestError.ReceivedNoData:
                    return "Received an empty response from remote host";

                case UnityWebRequestError.SSLNotSupported:
                    return "SSL connections are not supported on the local machine";

                case UnityWebRequestError.FailedToSendData:
                    return "Failed to transmit body data";

                case UnityWebRequestError.FailedToReceiveData:
                    return "Failed to receive response body data";

                case UnityWebRequestError.SSLCertificateError:
                    return "Failure to authenticate SSL certificate of remote host";

                case UnityWebRequestError.SSLCipherNotAvailable:
                    return "SSL cipher received from remote host is not supported on the local machine";

                case UnityWebRequestError.SSLCACertError:
                    return "Failure to authenticate Certificate Authority of the SSL certificate received from the remote host";

                case UnityWebRequestError.UnrecognizedContentEncoding:
                    return "Remote host returned data with an unrecognized/unparseable content encoding";

                case UnityWebRequestError.LoginFailed:
                    return "HTTP authentication failed";

                case UnityWebRequestError.SSLShutdownFailed:
                    return "Failure while shutting down SSL connection";
            }
            return "Unknown error";
        }
        internal enum UnityWebRequestError
        {
            OK,
            Unknown,
            SDKError,
            UnsupportedProtocol,
            MalformattedUrl,
            CannotResolveProxy,
            CannotResolveHost,
            CannotConnectToHost,
            AccessDenied,
            GenericHTTPError,
            WriteError,
            ReadError,
            OutOfMemory,
            Timeout,
            HTTPPostError,
            SSLCannotConnect,
            Aborted,
            TooManyRedirects,
            ReceivedNoData,
            SSLNotSupported,
            FailedToSendData,
            FailedToReceiveData,
            SSLCertificateError,
            SSLCipherNotAvailable,
            SSLCACertError,
            UnrecognizedContentEncoding,
            LoginFailed,
            SSLShutdownFailed
        }

        internal enum UnityWebRequestMethod
        {
            Get,
            Post,
            Put,
            Head,
            Custom
        }
    }
}

