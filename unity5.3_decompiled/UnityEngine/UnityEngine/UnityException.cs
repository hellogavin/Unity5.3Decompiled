namespace UnityEngine
{
    using System;
    using System.Runtime.Serialization;
    using UnityEngine.Scripting;

    [Serializable, RequiredByNativeCode]
    public class UnityException : SystemException
    {
        private const int Result = -2147467261;
        private string unityStackTrace;

        public UnityException() : base("A Unity Runtime error occurred!")
        {
            base.HResult = -2147467261;
        }

        public UnityException(string message) : base(message)
        {
            base.HResult = -2147467261;
        }

        protected UnityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public UnityException(string message, Exception innerException) : base(message, innerException)
        {
            base.HResult = -2147467261;
        }
    }
}

