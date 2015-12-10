namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class ParticleEmitter : Component
    {
        internal ParticleEmitter()
        {
        }

        public void ClearParticles()
        {
            INTERNAL_CALL_ClearParticles(this);
        }

        public void Emit()
        {
            this.Emit2((int) Random.Range(this.minEmission, this.maxEmission));
        }

        public void Emit(int count)
        {
            this.Emit2(count);
        }

        public void Emit(Vector3 pos, Vector3 velocity, float size, float energy, Color color)
        {
            InternalEmitParticleArguments args = new InternalEmitParticleArguments {
                pos = pos,
                velocity = velocity,
                size = size,
                energy = energy,
                color = color,
                rotation = 0f,
                angularVelocity = 0f
            };
            this.Emit3(ref args);
        }

        public void Emit(Vector3 pos, Vector3 velocity, float size, float energy, Color color, float rotation, float angularVelocity)
        {
            InternalEmitParticleArguments args = new InternalEmitParticleArguments {
                pos = pos,
                velocity = velocity,
                size = size,
                energy = energy,
                color = color,
                rotation = rotation,
                angularVelocity = angularVelocity
            };
            this.Emit3(ref args);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Emit2(int count);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Emit3(ref InternalEmitParticleArguments args);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_ClearParticles(ParticleEmitter self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_localVelocity(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_rndVelocity(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_worldVelocity(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_localVelocity(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_rndVelocity(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_worldVelocity(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Simulate(float deltaTime);

        public float angularVelocity { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool emit { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float emitterVelocityScale { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool enabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 localVelocity
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_localVelocity(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_localVelocity(ref value);
            }
        }

        public float maxEmission { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float maxEnergy { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float maxSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float minEmission { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float minEnergy { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float minSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int particleCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Particle[] particles { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float rndAngularVelocity { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool rndRotation { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 rndVelocity
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_rndVelocity(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_rndVelocity(ref value);
            }
        }

        public bool useWorldSpace { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 worldVelocity
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_worldVelocity(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_worldVelocity(ref value);
            }
        }
    }
}

