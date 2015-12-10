namespace UnityEngine.Experimental.Networking
{
    using System;
    using System.Text;

    public class MultipartFormFileSection : IMultipartFormSection
    {
        private string content;
        private byte[] data;
        private string file;
        private string name;

        public MultipartFormFileSection(byte[] data) : this(null, data, null, null)
        {
        }

        public MultipartFormFileSection(string fileName, byte[] data) : this(null, data, fileName, null)
        {
        }

        public MultipartFormFileSection(string data, string fileName) : this(data, null, fileName)
        {
        }

        public MultipartFormFileSection(string data, Encoding dataEncoding, string fileName) : this(null, data, dataEncoding, fileName)
        {
        }

        public MultipartFormFileSection(string name, byte[] data, string fileName, string contentType)
        {
            if ((data == null) || (data.Length < 1))
            {
                throw new ArgumentException("Cannot create a multipart form file section without body data");
            }
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "file.dat";
            }
            if (string.IsNullOrEmpty(contentType))
            {
                contentType = "application/octet-stream";
            }
            this.Init(name, data, fileName, contentType);
        }

        public MultipartFormFileSection(string name, string data, Encoding dataEncoding, string fileName)
        {
            if ((data == null) || (data.Length < 1))
            {
                throw new ArgumentException("Cannot create a multipart form file section without body data");
            }
            if (dataEncoding == null)
            {
                dataEncoding = Encoding.UTF8;
            }
            byte[] bytes = dataEncoding.GetBytes(data);
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "file.txt";
            }
            if (string.IsNullOrEmpty(this.content))
            {
                this.content = "text/plain; charset=" + dataEncoding.WebName;
            }
            this.Init(name, bytes, fileName, this.content);
        }

        private void Init(string name, byte[] data, string fileName, string contentType)
        {
            this.name = name;
            this.data = data;
            this.file = fileName;
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
                return this.file;
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

