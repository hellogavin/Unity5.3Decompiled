namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal sealed class WebView : ScriptableObject
    {
        [SerializeField]
        private MonoReloadableIntPtr WebViewWindow;
        public void OnDestroy()
        {
            this.DestroyWebView();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void DestroyWebView();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InitWebView(GUIView host, int x, int y, int width, int height, bool showResizeHandle);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ExecuteJavascript(string scriptCode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void LoadURL(string url);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void LoadFile(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool DefineScriptObject(string path, ScriptableObject obj);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetDelegateObject(ScriptableObject value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetHostView(GUIView view);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetSizeAndPosition(int x, int y, int width, int height);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetFocus(bool value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool HasApplicationFocus();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetApplicationFocus(bool applicationFocus);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Show();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Hide();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Back();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Forward();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SendOnEvent(string jsonStr);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Reload();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void AllowRightClickMenu(bool allowRightClickMenu);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ShowDevTools();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ToggleMaximize();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void OnDomainReload();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool IntPtrIsNull();
        public static implicit operator bool(WebView exists)
        {
            return ((exists != null) && !exists.IntPtrIsNull());
        }
    }
}

