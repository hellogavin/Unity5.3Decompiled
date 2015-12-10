namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    internal class GUIView : View
    {
        private int m_DepthBufferBits;
        private int m_AntiAlias;
        private bool m_WantsMouseMove;
        private bool m_AutoRepaintOnSceneChange;
        private bool m_BackgroundValid;
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void SetTitle(string title);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_Init(int depthBits, int antiAlias);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_Recreate(int depthBits, int antiAlias);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_Close();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool Internal_SendEvent(Event e);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void AddToAuxWindowList();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void RemoveFromAuxWindowList();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        protected extern void Internal_SetAsActiveWindow();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_SetWantsMouseMove(bool wantIt);
        public void SetInternalGameViewRect(Rect rect)
        {
            INTERNAL_CALL_SetInternalGameViewRect(this, ref rect);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetInternalGameViewRect(GUIView self, ref Rect rect);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetAsStartView();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ClearStartView();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_SetAutoRepaint(bool doit);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_SetWindow(ContainerWindow win);
        private void Internal_SetPosition(Rect windowPosition)
        {
            INTERNAL_CALL_Internal_SetPosition(this, ref windowPosition);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_SetPosition(GUIView self, ref Rect windowPosition);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Focus();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Repaint();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void RepaintImmediately();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void CaptureRenderDoc();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void MakeVistaDWMHappyDance();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void StealMouseCapture();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void ClearKeyboardControl();
        public static GUIView current { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public static GUIView focusedView { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public static GUIView mouseOverView { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public bool hasFocus { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        internal bool mouseRayInvisible { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        internal void GrabPixels(RenderTexture rd, Rect rect)
        {
            INTERNAL_CALL_GrabPixels(this, rd, ref rect);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GrabPixels(GUIView self, RenderTexture rd, ref Rect rect);
        internal bool SendEvent(Event e)
        {
            int num = SavedGUIState.Internal_GetGUIDepth();
            bool flag = false;
            if (num > 0)
            {
                SavedGUIState state = SavedGUIState.Create();
                flag = this.Internal_SendEvent(e);
                state.ApplyAndForget();
                return flag;
            }
            return this.Internal_SendEvent(e);
        }

        protected override void SetWindow(ContainerWindow win)
        {
            base.SetWindow(win);
            this.Internal_Init(this.m_DepthBufferBits, this.m_AntiAlias);
            if (win != null)
            {
                this.Internal_SetWindow(win);
            }
            this.Internal_SetAutoRepaint(this.m_AutoRepaintOnSceneChange);
            this.Internal_SetPosition(base.windowPosition);
            this.Internal_SetWantsMouseMove(this.m_WantsMouseMove);
            this.m_BackgroundValid = false;
        }

        internal void RecreateContext()
        {
            this.Internal_Recreate(this.m_DepthBufferBits, this.m_AntiAlias);
            this.m_BackgroundValid = false;
        }

        public bool wantsMouseMove
        {
            get
            {
                return this.m_WantsMouseMove;
            }
            set
            {
                this.m_WantsMouseMove = value;
                this.Internal_SetWantsMouseMove(this.m_WantsMouseMove);
            }
        }
        internal bool backgroundValid
        {
            get
            {
                return this.m_BackgroundValid;
            }
            set
            {
                this.m_BackgroundValid = value;
            }
        }
        public bool autoRepaintOnSceneChange
        {
            get
            {
                return this.m_AutoRepaintOnSceneChange;
            }
            set
            {
                this.m_AutoRepaintOnSceneChange = value;
                this.Internal_SetAutoRepaint(this.m_AutoRepaintOnSceneChange);
            }
        }
        public int depthBufferBits
        {
            get
            {
                return this.m_DepthBufferBits;
            }
            set
            {
                this.m_DepthBufferBits = value;
            }
        }
        public int antiAlias
        {
            get
            {
                return this.m_AntiAlias;
            }
            set
            {
                this.m_AntiAlias = value;
            }
        }
        protected override void SetPosition(Rect newPos)
        {
            Rect windowPosition = base.windowPosition;
            base.SetPosition(newPos);
            if (windowPosition == base.windowPosition)
            {
                this.Internal_SetPosition(base.windowPosition);
            }
            else
            {
                this.Repaint();
                this.Internal_SetPosition(base.windowPosition);
                this.m_BackgroundValid = false;
            }
        }

        public void OnDestroy()
        {
            this.Internal_Close();
            base.OnDestroy();
        }

        internal void DoWindowDecorationStart()
        {
            if (base.window != null)
            {
                base.window.HandleWindowDecorationStart(base.windowPosition);
            }
        }

        internal void DoWindowDecorationEnd()
        {
            if (base.window != null)
            {
                base.window.HandleWindowDecorationEnd(base.windowPosition);
            }
        }
    }
}

