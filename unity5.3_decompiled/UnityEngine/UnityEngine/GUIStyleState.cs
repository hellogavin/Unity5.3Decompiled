namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public sealed class GUIStyleState
    {
        [NonSerialized]
        internal IntPtr m_Ptr;
        private readonly GUIStyle m_SourceStyle;
        [NonSerialized]
        private Texture2D m_Background;
        public GUIStyleState()
        {
            this.Init();
        }

        private GUIStyleState(GUIStyle sourceStyle, IntPtr source)
        {
            this.m_SourceStyle = sourceStyle;
            this.m_Ptr = source;
        }

        internal static GUIStyleState ProduceGUIStyleStateFromDeserialization(GUIStyle sourceStyle, IntPtr source)
        {
            GUIStyleState state;
            return new GUIStyleState(sourceStyle, source) { m_Background = state.GetBackgroundInternalFromDeserialization() };
        }

        internal static GUIStyleState GetGUIStyleState(GUIStyle sourceStyle, IntPtr source)
        {
            GUIStyleState state;
            return new GUIStyleState(sourceStyle, source) { m_Background = state.GetBackgroundInternal() };
        }

        ~GUIStyleState()
        {
            if (this.m_SourceStyle == null)
            {
                this.Cleanup();
            }
        }

        public Texture2D background
        {
            get
            {
                return this.GetBackgroundInternal();
            }
            set
            {
                this.SetBackgroundInternal(value);
                this.m_Background = value;
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Init();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Cleanup();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetBackgroundInternal(Texture2D value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern Texture2D GetBackgroundInternalFromDeserialization();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern Texture2D GetBackgroundInternal();
        public Color textColor
        {
            get
            {
                Color color;
                this.INTERNAL_get_textColor(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_textColor(ref value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_textColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_textColor(ref Color value);
    }
}

