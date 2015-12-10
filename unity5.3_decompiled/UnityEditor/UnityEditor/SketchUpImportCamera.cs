namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct SketchUpImportCamera
    {
        public Vector3 position;
        public Vector3 lookAt;
        public Vector3 up;
        public float fieldOfView;
        public float aspectRatio;
        public float orthoSize;
        public bool isPerspective;
    }
}

