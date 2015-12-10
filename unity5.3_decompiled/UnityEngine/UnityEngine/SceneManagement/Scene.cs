namespace UnityEngine.SceneManagement
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct Scene
    {
        private const int kInvalidUnitySceneHandle = 0;
        private int m_Handle;
        internal int handle
        {
            get
            {
                return this.m_Handle;
            }
        }
        public bool IsValid()
        {
            return (this.m_Handle != 0);
        }

        public string path
        {
            get
            {
                return GetPathInternal(this.handle);
            }
        }
        public string name
        {
            get
            {
                return GetNameInternal(this.handle);
            }
        }
        public bool isLoaded
        {
            get
            {
                return GetIsLoadedInternal(this.handle);
            }
        }
        public int buildIndex
        {
            get
            {
                return GetBuildIndexInternal(this.handle);
            }
        }
        public bool isDirty
        {
            get
            {
                return GetIsDirtyInternal(this.handle);
            }
        }
        public int rootCount
        {
            get
            {
                return GetRootCountInternal(this.handle);
            }
        }
        public override int GetHashCode()
        {
            return this.m_Handle;
        }

        public override bool Equals(object other)
        {
            if (!(other is Scene))
            {
                return false;
            }
            Scene scene = (Scene) other;
            return (this.handle == scene.handle);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string GetPathInternal(int sceneHandle);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string GetNameInternal(int sceneHandle);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool GetIsLoadedInternal(int sceneHandle);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool GetIsDirtyInternal(int sceneHandle);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int GetBuildIndexInternal(int sceneHandle);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int GetRootCountInternal(int sceneHandle);
        public static bool operator ==(Scene lhs, Scene rhs)
        {
            return (lhs.handle == rhs.handle);
        }

        public static bool operator !=(Scene lhs, Scene rhs)
        {
            return (lhs.handle != rhs.handle);
        }
    }
}

