namespace UnityEditor.Modules
{
    using System;

    internal interface IUserAssembliesValidator
    {
        void Cleanup();
        void Validate(string[] userAssemblies);

        bool canRunInBackground { get; }
    }
}

