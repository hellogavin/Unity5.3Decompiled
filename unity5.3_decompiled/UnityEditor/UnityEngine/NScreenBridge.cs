namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class NScreenBridge : Object
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern NScreenBridge();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Texture2D GetScreenTexture();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitServer(int id);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ResetInput();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetInput(int x, int y, int button, int key, int type);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetResolution(int x, int y);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Shutdown();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void StartWatchdogForPid(int pid);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Update();
    }
}

