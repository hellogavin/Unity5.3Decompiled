namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class ParticleRenderer : Renderer
    {
        [Obsolete("animatedTextureCount has been replaced by uvAnimationXTile and uvAnimationYTile.")]
        public int animatedTextureCount
        {
            get
            {
                return this.uvAnimationXTile;
            }
            set
            {
                this.uvAnimationXTile = value;
            }
        }

        public float cameraVelocityScale { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("This function has been removed.", true)]
        public AnimationCurve heightCurve
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public float lengthScale { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float maxParticleSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float maxPartileSize
        {
            get
            {
                return this.maxParticleSize;
            }
            set
            {
                this.maxParticleSize = value;
            }
        }

        public ParticleRenderMode particleRenderMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("This function has been removed.", true)]
        public AnimationCurve rotationCurve
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public float uvAnimationCycles { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int uvAnimationXTile { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int uvAnimationYTile { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Rect[] uvTiles { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float velocityScale { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("This function has been removed.", true)]
        public AnimationCurve widthCurve
        {
            get
            {
                return null;
            }
            set
            {
            }
        }
    }
}

