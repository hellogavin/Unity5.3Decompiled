namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class SketchUpImporter : ModelImporter
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern SketchUpImportCamera GetDefaultCamera();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern SketchUpNodeInfo[] GetNodes();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern SketchUpImportScene[] GetScenes();

        public double latitude { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public double longitude { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public double northCorrection { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

