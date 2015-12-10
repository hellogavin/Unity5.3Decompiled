namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Rendering;

    [StructLayout(LayoutKind.Sequential)]
    public struct RenderBuffer
    {
        internal int m_RenderTextureInstanceID;
        internal IntPtr m_BufferPtr;
        internal void SetLoadAction(RenderBufferLoadAction action)
        {
            RenderBufferHelper.SetLoadAction(out this, (int) action);
        }

        internal void SetStoreAction(RenderBufferStoreAction action)
        {
            RenderBufferHelper.SetStoreAction(out this, (int) action);
        }

        internal RenderBufferLoadAction loadAction
        {
            get
            {
                return (RenderBufferLoadAction) RenderBufferHelper.GetLoadAction(out this);
            }
            set
            {
                this.SetLoadAction(value);
            }
        }
        internal RenderBufferStoreAction storeAction
        {
            get
            {
                return (RenderBufferStoreAction) RenderBufferHelper.GetStoreAction(out this);
            }
            set
            {
                this.SetStoreAction(value);
            }
        }
        public IntPtr GetNativeRenderBufferPtr()
        {
            return this.m_BufferPtr;
        }
    }
}

