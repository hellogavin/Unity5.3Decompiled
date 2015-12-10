namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class SpriteRenderer : Renderer
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern Sprite GetSprite_INTERNAL();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_color(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_color(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetSprite_INTERNAL(Sprite sprite);

        public Color color
        {
            get
            {
                Color color;
                this.INTERNAL_get_color(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_color(ref value);
            }
        }

        public bool flipX { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool flipY { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Sprite sprite
        {
            get
            {
                return this.GetSprite_INTERNAL();
            }
            set
            {
                this.SetSprite_INTERNAL(value);
            }
        }
    }
}

