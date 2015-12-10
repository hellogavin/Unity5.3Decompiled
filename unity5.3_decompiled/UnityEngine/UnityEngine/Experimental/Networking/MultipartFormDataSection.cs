namespace UnityEngine.Experimental.Networking
{
    using System;
    using System.Text;

    public class MultipartFormDataSection : IMultipartFormSection
    {
        private string content;
        private byte[] data;
        private string name;

        public MultipartFormDataSection(byte[] data) : this(null, data)
        {
        }

        public MultipartFormDataSection(string data) : this(null, data)
        {
        }

        public MultipartFormDataSection(string name, byte[] data) : this(name, data, null)
        {
        }

        public MultipartFormDataSection(string name, string data) : this(name, data, "text/plain")
        {
        }

        public MultipartFormDataSection(string name, byte[] data, string contentType)
        {
            if ((data == null) || (data.Length < 1))
            {
                throw new ArgumentException("Cannot create a multipart form data section without body data");
            }
            this.name = name;
            this.data = data;
            this.content = contentType;
        }

        public MultipartFormDataSection(string name, string data, string contentType) : this(name, data, Encoding.UTF8, contentType)
        {
        }

        public MultipartFormDataSection(string name, string data, Encoding encoding, string contentType)
        {
            if ((data == null) || (data.Length < 1))
            {
                throw new ArgumentException("Cannot create a multipart form data section without body data");
            }
            byte[] bytes = encoding.GetBytes(data);
            this.name = name;
            this.data = bytes;
            if ((contentType != null) && !contentType.Contains("encoding="))
            {
                contentType = contentType.Trim() + "; encoding=" + encoding.WebName;
            }
            this.content = contentType;
        }

        public string contentType
        {
            get
            {
                return this.content;
            }
        }

        public string fileName
        {
            get
            {
                return null;
            }
        }

        public byte[] sectionData
        {
            get
            {
                return this.data;
            }
        }

        public string sectionName
        {
            get
            {
                return this.name;
            }
        }
    }
}

