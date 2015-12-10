namespace UnityEditor.RestService
{
    using System;

    internal enum HttpStatusCode
    {
        Accepted = 0xca,
        BadRequest = 400,
        Created = 0xc9,
        Forbidden = 0x193,
        InternalServerError = 500,
        MethodNotAllowed = 0x195,
        NoContent = 0xcc,
        NotFound = 0x194,
        NotImplemented = 0x1f5,
        Ok = 200
    }
}

