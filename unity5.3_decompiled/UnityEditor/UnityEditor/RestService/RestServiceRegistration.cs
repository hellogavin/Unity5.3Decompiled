namespace UnityEditor.RestService
{
    using System;
    using UnityEditor;

    [InitializeOnLoad]
    internal class RestServiceRegistration
    {
        static RestServiceRegistration()
        {
            OpenDocumentsRestHandler.Register();
            ProjectStateRestHandler.Register();
            AssetRestHandler.Register();
            PairingRestHandler.Register();
            PlayModeRestHandler.Register();
        }
    }
}

