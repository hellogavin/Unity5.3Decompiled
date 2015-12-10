namespace UnityEditor.RestService
{
    using System;
    using System.Runtime.CompilerServices;

    internal class RestRequestException : Exception
    {
        public RestRequestException()
        {
        }

        public RestRequestException(UnityEditor.RestService.HttpStatusCode httpStatusCode, string restErrorString) : this(httpStatusCode, restErrorString, null)
        {
        }

        public RestRequestException(UnityEditor.RestService.HttpStatusCode httpStatusCode, string restErrorString, string restErrorDescription)
        {
            this.HttpStatusCode = httpStatusCode;
            this.RestErrorString = restErrorString;
            this.RestErrorDescription = restErrorDescription;
        }

        public UnityEditor.RestService.HttpStatusCode HttpStatusCode { get; set; }

        public string RestErrorDescription { get; set; }

        public string RestErrorString { get; set; }
    }
}

