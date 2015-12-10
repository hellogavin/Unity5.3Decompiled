namespace UnityEngine.Experimental.Networking
{
    using System;

    public interface IMultipartFormSection
    {
        string contentType { get; }

        string fileName { get; }

        byte[] sectionData { get; }

        string sectionName { get; }
    }
}

