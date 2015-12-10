namespace UnityEngine
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    public sealed class ParticleSystem : Component
    {
        [CompilerGenerated]
        private static IteratorDelegate <>f__am$cache0;
        [CompilerGenerated]
        private static IteratorDelegate <>f__am$cache1;
        [CompilerGenerated]
        private static IteratorDelegate <>f__am$cache2;
        [CompilerGenerated]
        private static IteratorDelegate <>f__am$cache3;
        [CompilerGenerated]
        private static IteratorDelegate <>f__am$cache4;

        [ExcludeFromDocs]
        public void Clear()
        {
            bool withChildren = true;
            this.Clear(withChildren);
        }

        public void Clear([DefaultValue("true")] bool withChildren)
        {
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = ps => Internal_Clear(ps);
            }
            this.IterateParticleSystems(withChildren, <>f__am$cache3);
        }

        public void Emit(int count)
        {
            INTERNAL_CALL_Emit(this, count);
        }

        [Obsolete("Emit with a single particle structure is deprecated. Pass a ParticleSystem.EmitParams parameter instead, which allows you to override some/all of the emission properties")]
        public void Emit(Particle particle)
        {
            this.Internal_EmitOld(ref particle);
        }

        public void Emit(EmitParams emitParams, int count)
        {
            this.Internal_Emit(ref emitParams, count);
        }

        [Obsolete("Emit with specific parameters is deprecated. Pass a ParticleSystem.EmitParams parameter instead, which allows you to override some/all of the emission properties")]
        public void Emit(Vector3 position, Vector3 velocity, float size, float lifetime, Color32 color)
        {
            Particle particle = new Particle {
                position = position,
                velocity = velocity,
                lifetime = lifetime,
                startLifetime = lifetime,
                startSize = size,
                rotation3D = Vector3.zero,
                angularVelocity3D = Vector3.zero,
                startColor = color,
                randomSeed = 5
            };
            this.Internal_EmitOld(ref particle);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int GetParticles(Particle[] particles);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Emit(ParticleSystem self, int count);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool Internal_Clear(ParticleSystem self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_Emit(ref EmitParams emitParams, int count);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_EmitOld(ref Particle particle);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_startColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_startRotation3D(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool Internal_IsAlive(ParticleSystem self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool Internal_Pause(ParticleSystem self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool Internal_Play(ParticleSystem self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_startColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_startRotation3D(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool Internal_Simulate(ParticleSystem self, float t, bool restart);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool Internal_Stop(ParticleSystem self);
        [ExcludeFromDocs]
        public bool IsAlive()
        {
            bool withChildren = true;
            return this.IsAlive(withChildren);
        }

        public bool IsAlive([DefaultValue("true")] bool withChildren)
        {
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = ps => Internal_IsAlive(ps);
            }
            return this.IterateParticleSystems(withChildren, <>f__am$cache4);
        }

        internal bool IterateParticleSystems(bool recurse, IteratorDelegate func)
        {
            bool flag = func(this);
            if (recurse)
            {
                flag |= IterateParticleSystemsRecursive(base.transform, func);
            }
            return flag;
        }

        private static bool IterateParticleSystemsRecursive(Transform transform, IteratorDelegate func)
        {
            bool flag = false;
            IEnumerator enumerator = transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    ParticleSystem component = current.gameObject.GetComponent<ParticleSystem>();
                    if (component != null)
                    {
                        flag = func(component);
                        if (flag)
                        {
                            return flag;
                        }
                        IterateParticleSystemsRecursive(current, func);
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            return flag;
        }

        [ExcludeFromDocs]
        public void Pause()
        {
            bool withChildren = true;
            this.Pause(withChildren);
        }

        public void Pause([DefaultValue("true")] bool withChildren)
        {
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = ps => Internal_Pause(ps);
            }
            this.IterateParticleSystems(withChildren, <>f__am$cache2);
        }

        [ExcludeFromDocs]
        public void Play()
        {
            bool withChildren = true;
            this.Play(withChildren);
        }

        public void Play([DefaultValue("true")] bool withChildren)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = ps => Internal_Play(ps);
            }
            this.IterateParticleSystems(withChildren, <>f__am$cache0);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetParticles(Particle[] particles, int size);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void SetupDefaultType(int type);
        [ExcludeFromDocs]
        public void Simulate(float t)
        {
            bool restart = true;
            bool withChildren = true;
            this.Simulate(t, withChildren, restart);
        }

        [ExcludeFromDocs]
        public void Simulate(float t, bool withChildren)
        {
            bool restart = true;
            this.Simulate(t, withChildren, restart);
        }

        public void Simulate(float t, [DefaultValue("true")] bool withChildren, [DefaultValue("true")] bool restart)
        {
            <Simulate>c__AnonStorey1 storey = new <Simulate>c__AnonStorey1 {
                t = t,
                restart = restart
            };
            this.IterateParticleSystems(withChildren, new IteratorDelegate(storey.<>m__0));
        }

        [ExcludeFromDocs]
        public void Stop()
        {
            bool withChildren = true;
            this.Stop(withChildren);
        }

        public void Stop([DefaultValue("true")] bool withChildren)
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = ps => Internal_Stop(ps);
            }
            this.IterateParticleSystems(withChildren, <>f__am$cache1);
        }

        public CollisionModule collision
        {
            get
            {
                return new CollisionModule(this);
            }
        }

        public ColorBySpeedModule colorBySpeed
        {
            get
            {
                return new ColorBySpeedModule(this);
            }
        }

        public ColorOverLifetimeModule colorOverLifetime
        {
            get
            {
                return new ColorOverLifetimeModule(this);
            }
        }

        public float duration { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public EmissionModule emission
        {
            get
            {
                return new EmissionModule(this);
            }
        }

        [Obsolete("emissionRate property is deprecated. Use emission.rate instead.")]
        public float emissionRate { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("enableEmission property is deprecated. Use emission.enable instead.")]
        public bool enableEmission { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public ExternalForcesModule externalForces
        {
            get
            {
                return new ExternalForcesModule(this);
            }
        }

        public ForceOverLifetimeModule forceOverLifetime
        {
            get
            {
                return new ForceOverLifetimeModule(this);
            }
        }

        public float gravityModifier { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public InheritVelocityModule inheritVelocity
        {
            get
            {
                return new InheritVelocityModule(this);
            }
        }

        public bool isPaused { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isPlaying { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isStopped { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public LimitVelocityOverLifetimeModule limitVelocityOverLifetime
        {
            get
            {
                return new LimitVelocityOverLifetimeModule(this);
            }
        }

        public bool loop { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int maxParticles { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int particleCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float playbackSpeed { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool playOnAwake { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public uint randomSeed { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public RotationBySpeedModule rotationBySpeed
        {
            get
            {
                return new RotationBySpeedModule(this);
            }
        }

        public RotationOverLifetimeModule rotationOverLifetime
        {
            get
            {
                return new RotationOverLifetimeModule(this);
            }
        }

        [Obsolete("safeCollisionEventSize has been deprecated. Use GetSafeCollisionEventSize() instead (UnityUpgradable) -> ParticlePhysicsExtensions.GetSafeCollisionEventSize(UnityEngine.ParticleSystem)", false)]
        public int safeCollisionEventSize
        {
            get
            {
                return ParticleSystemExtensionsImpl.GetSafeCollisionEventSize(this);
            }
        }

        public ParticleSystemScalingMode scalingMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public ShapeModule shape
        {
            get
            {
                return new ShapeModule(this);
            }
        }

        public ParticleSystemSimulationSpace simulationSpace { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public SizeBySpeedModule sizeBySpeed
        {
            get
            {
                return new SizeBySpeedModule(this);
            }
        }

        public SizeOverLifetimeModule sizeOverLifetime
        {
            get
            {
                return new SizeOverLifetimeModule(this);
            }
        }

        public Color startColor
        {
            get
            {
                Color color;
                this.INTERNAL_get_startColor(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_startColor(ref value);
            }
        }

        public float startDelay { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float startLifetime { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float startRotation { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 startRotation3D
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_startRotation3D(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_startRotation3D(ref value);
            }
        }

        public float startSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float startSpeed { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public SubEmittersModule subEmitters
        {
            get
            {
                return new SubEmittersModule(this);
            }
        }

        public TextureSheetAnimationModule textureSheetAnimation
        {
            get
            {
                return new TextureSheetAnimationModule(this);
            }
        }

        public float time { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public VelocityOverLifetimeModule velocityOverLifetime
        {
            get
            {
                return new VelocityOverLifetimeModule(this);
            }
        }

        [CompilerGenerated]
        private sealed class <Simulate>c__AnonStorey1
        {
            internal bool restart;
            internal float t;

            internal bool <>m__0(ParticleSystem ps)
            {
                return ParticleSystem.Internal_Simulate(ps, this.t, this.restart);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Burst
        {
            private float m_Time;
            private short m_MinCount;
            private short m_MaxCount;
            public Burst(float _time, short _count)
            {
                this.m_Time = _time;
                this.m_MinCount = _count;
                this.m_MaxCount = _count;
            }

            public Burst(float _time, short _minCount, short _maxCount)
            {
                this.m_Time = _time;
                this.m_MinCount = _minCount;
                this.m_MaxCount = _maxCount;
            }

            public float time
            {
                get
                {
                    return this.m_Time;
                }
                set
                {
                    this.m_Time = value;
                }
            }
            public short minCount
            {
                get
                {
                    return this.m_MinCount;
                }
                set
                {
                    this.m_MinCount = value;
                }
            }
            public short maxCount
            {
                get
                {
                    return this.m_MaxCount;
                }
                set
                {
                    this.m_MaxCount = value;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, Size=1), Obsolete("ParticleSystem.CollisionEvent has been deprecated. Use ParticleCollisionEvent instead (UnityUpgradable) -> ParticleCollisionEvent", true)]
        public struct CollisionEvent
        {
            public Vector3 intersection
            {
                get
                {
                    return new Vector3();
                }
            }
            public Vector3 normal
            {
                get
                {
                    return new Vector3();
                }
            }
            public Vector3 velocity
            {
                get
                {
                    return new Vector3();
                }
            }
            public Collider collider
            {
                get
                {
                    return null;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CollisionModule
        {
            private ParticleSystem m_ParticleSystem;
            internal CollisionModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            public bool enabled
            {
                get
                {
                    return GetEnabled(this.m_ParticleSystem);
                }
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            public ParticleSystemCollisionType type
            {
                get
                {
                    return (ParticleSystemCollisionType) GetType(this.m_ParticleSystem);
                }
                set
                {
                    SetType(this.m_ParticleSystem, (int) value);
                }
            }
            public ParticleSystemCollisionMode mode
            {
                get
                {
                    return (ParticleSystemCollisionMode) GetMode(this.m_ParticleSystem);
                }
                set
                {
                    SetMode(this.m_ParticleSystem, (int) value);
                }
            }
            public ParticleSystem.MinMaxCurve dampen
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetDampen(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetDampen(this.m_ParticleSystem, ref value);
                }
            }
            public ParticleSystem.MinMaxCurve bounce
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetBounce(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetBounce(this.m_ParticleSystem, ref value);
                }
            }
            public ParticleSystem.MinMaxCurve lifetimeLoss
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetEnergyLoss(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetEnergyLoss(this.m_ParticleSystem, ref value);
                }
            }
            public float minKillSpeed
            {
                get
                {
                    return GetMinKillSpeed(this.m_ParticleSystem);
                }
                set
                {
                    SetMinKillSpeed(this.m_ParticleSystem, value);
                }
            }
            public LayerMask collidesWith
            {
                get
                {
                    return GetCollidesWith(this.m_ParticleSystem);
                }
                set
                {
                    SetCollidesWith(this.m_ParticleSystem, (int) value);
                }
            }
            public bool enableDynamicColliders
            {
                get
                {
                    return GetEnableDynamicColliders(this.m_ParticleSystem);
                }
                set
                {
                    SetEnableDynamicColliders(this.m_ParticleSystem, value);
                }
            }
            public bool enableInteriorCollisions
            {
                get
                {
                    return GetEnableInteriorCollisions(this.m_ParticleSystem);
                }
                set
                {
                    SetEnableInteriorCollisions(this.m_ParticleSystem, value);
                }
            }
            public int maxCollisionShapes
            {
                get
                {
                    return GetMaxCollisionShapes(this.m_ParticleSystem);
                }
                set
                {
                    SetMaxCollisionShapes(this.m_ParticleSystem, value);
                }
            }
            public ParticleSystemCollisionQuality quality
            {
                get
                {
                    return (ParticleSystemCollisionQuality) GetQuality(this.m_ParticleSystem);
                }
                set
                {
                    SetQuality(this.m_ParticleSystem, (int) value);
                }
            }
            public float voxelSize
            {
                get
                {
                    return GetVoxelSize(this.m_ParticleSystem);
                }
                set
                {
                    SetVoxelSize(this.m_ParticleSystem, value);
                }
            }
            public float radiusScale
            {
                get
                {
                    return GetRadiusScale(this.m_ParticleSystem);
                }
                set
                {
                    SetRadiusScale(this.m_ParticleSystem, value);
                }
            }
            public bool sendCollisionMessages
            {
                get
                {
                    return GetUsesCollisionMessages(this.m_ParticleSystem);
                }
                set
                {
                    SetUsesCollisionMessages(this.m_ParticleSystem, value);
                }
            }
            public void SetPlane(int index, Transform transform)
            {
                SetPlane(this.m_ParticleSystem, index, transform);
            }

            public Transform GetPlane(int index)
            {
                return GetPlane(this.m_ParticleSystem, index);
            }

            public int maxPlaneCount
            {
                get
                {
                    return GetMaxPlaneCount(this.m_ParticleSystem);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetType(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern int GetType(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetMode(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern int GetMode(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetDampen(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetDampen(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetBounce(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetBounce(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetEnergyLoss(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetEnergyLoss(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetMinKillSpeed(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern float GetMinKillSpeed(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetCollidesWith(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern int GetCollidesWith(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetEnableDynamicColliders(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetEnableDynamicColliders(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetEnableInteriorCollisions(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetEnableInteriorCollisions(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetMaxCollisionShapes(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern int GetMaxCollisionShapes(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetQuality(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern int GetQuality(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetVoxelSize(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern float GetVoxelSize(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetRadiusScale(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern float GetRadiusScale(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetUsesCollisionMessages(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetUsesCollisionMessages(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetPlane(ParticleSystem system, int index, Transform transform);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern Transform GetPlane(ParticleSystem system, int index);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern int GetMaxPlaneCount(ParticleSystem system);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ColorBySpeedModule
        {
            private ParticleSystem m_ParticleSystem;
            internal ColorBySpeedModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            public bool enabled
            {
                get
                {
                    return GetEnabled(this.m_ParticleSystem);
                }
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            public ParticleSystem.MinMaxGradient color
            {
                get
                {
                    ParticleSystem.MinMaxGradient gradient = new ParticleSystem.MinMaxGradient();
                    GetColor(this.m_ParticleSystem, ref gradient);
                    return gradient;
                }
                set
                {
                    SetColor(this.m_ParticleSystem, ref value);
                }
            }
            public Vector2 range
            {
                get
                {
                    return GetRange(this.m_ParticleSystem);
                }
                set
                {
                    SetRange(this.m_ParticleSystem, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);
            private static void SetRange(ParticleSystem system, Vector2 value)
            {
                INTERNAL_CALL_SetRange(system, ref value);
            }

            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void INTERNAL_CALL_SetRange(ParticleSystem system, ref Vector2 value);
            private static Vector2 GetRange(ParticleSystem system)
            {
                Vector2 vector;
                INTERNAL_CALL_GetRange(system, out vector);
                return vector;
            }

            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void INTERNAL_CALL_GetRange(ParticleSystem system, out Vector2 value);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ColorOverLifetimeModule
        {
            private ParticleSystem m_ParticleSystem;
            internal ColorOverLifetimeModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            public bool enabled
            {
                get
                {
                    return GetEnabled(this.m_ParticleSystem);
                }
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            public ParticleSystem.MinMaxGradient color
            {
                get
                {
                    ParticleSystem.MinMaxGradient gradient = new ParticleSystem.MinMaxGradient();
                    GetColor(this.m_ParticleSystem, ref gradient);
                    return gradient;
                }
                set
                {
                    SetColor(this.m_ParticleSystem, ref value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetColor(ParticleSystem system, ref ParticleSystem.MinMaxGradient gradient);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct EmissionModule
        {
            private ParticleSystem m_ParticleSystem;
            internal EmissionModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            public bool enabled
            {
                get
                {
                    return GetEnabled(this.m_ParticleSystem);
                }
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            public ParticleSystem.MinMaxCurve rate
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetRate(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetRate(this.m_ParticleSystem, ref value);
                }
            }
            public ParticleSystemEmissionType type
            {
                get
                {
                    return (ParticleSystemEmissionType) GetType(this.m_ParticleSystem);
                }
                set
                {
                    SetType(this.m_ParticleSystem, (int) value);
                }
            }
            public void SetBursts(ParticleSystem.Burst[] bursts)
            {
                SetBursts(this.m_ParticleSystem, bursts, bursts.Length);
            }

            public void SetBursts(ParticleSystem.Burst[] bursts, int size)
            {
                SetBursts(this.m_ParticleSystem, bursts, size);
            }

            public int GetBursts(ParticleSystem.Burst[] bursts)
            {
                return GetBursts(this.m_ParticleSystem, bursts);
            }

            public int burstCount
            {
                get
                {
                    return GetBurstCount(this.m_ParticleSystem);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetType(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern int GetType(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern int GetBurstCount(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetRate(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetRate(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetBursts(ParticleSystem system, ParticleSystem.Burst[] bursts, int size);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern int GetBursts(ParticleSystem system, ParticleSystem.Burst[] bursts);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct EmitParams
        {
            internal ParticleSystem.Particle m_Particle;
            internal bool m_PositionSet;
            internal bool m_VelocitySet;
            internal bool m_AxisOfRotationSet;
            internal bool m_RotationSet;
            internal bool m_AngularVelocitySet;
            internal bool m_StartSizeSet;
            internal bool m_StartColorSet;
            internal bool m_RandomSeedSet;
            internal bool m_StartLifetimeSet;
            public Vector3 position
            {
                get
                {
                    return this.m_Particle.position;
                }
                set
                {
                    this.m_Particle.position = value;
                    this.m_PositionSet = true;
                }
            }
            public Vector3 velocity
            {
                get
                {
                    return this.m_Particle.velocity;
                }
                set
                {
                    this.m_Particle.velocity = value;
                    this.m_VelocitySet = true;
                }
            }
            public float startLifetime
            {
                get
                {
                    return this.m_Particle.startLifetime;
                }
                set
                {
                    this.m_Particle.startLifetime = value;
                    this.m_StartLifetimeSet = true;
                }
            }
            public float startSize
            {
                get
                {
                    return this.m_Particle.startSize;
                }
                set
                {
                    this.m_Particle.startSize = value;
                    this.m_StartSizeSet = true;
                }
            }
            public Vector3 axisOfRotation
            {
                get
                {
                    return this.m_Particle.axisOfRotation;
                }
                set
                {
                    this.m_Particle.axisOfRotation = value;
                    this.m_AxisOfRotationSet = true;
                }
            }
            public float rotation
            {
                get
                {
                    return this.m_Particle.rotation;
                }
                set
                {
                    this.m_Particle.rotation = value;
                    this.m_RotationSet = true;
                }
            }
            public Vector3 rotation3D
            {
                get
                {
                    return this.m_Particle.rotation3D;
                }
                set
                {
                    this.m_Particle.rotation3D = value;
                    this.m_RotationSet = true;
                }
            }
            public float angularVelocity
            {
                get
                {
                    return this.m_Particle.angularVelocity;
                }
                set
                {
                    this.m_Particle.angularVelocity = value;
                    this.m_AngularVelocitySet = true;
                }
            }
            public Vector3 angularVelocity3D
            {
                get
                {
                    return this.m_Particle.angularVelocity3D;
                }
                set
                {
                    this.m_Particle.angularVelocity3D = value;
                    this.m_AngularVelocitySet = true;
                }
            }
            public Color32 startColor
            {
                get
                {
                    return this.m_Particle.startColor;
                }
                set
                {
                    this.m_Particle.startColor = value;
                    this.m_StartColorSet = true;
                }
            }
            public uint randomSeed
            {
                get
                {
                    return this.m_Particle.randomSeed;
                }
                set
                {
                    this.m_Particle.randomSeed = value;
                    this.m_RandomSeedSet = true;
                }
            }
            public void ResetPosition()
            {
                this.m_PositionSet = false;
            }

            public void ResetVelocity()
            {
                this.m_VelocitySet = false;
            }

            public void ResetAxisOfRotation()
            {
                this.m_AxisOfRotationSet = false;
            }

            public void ResetRotation()
            {
                this.m_RotationSet = false;
            }

            public void ResetAngularVelocity()
            {
                this.m_AngularVelocitySet = false;
            }

            public void ResetStartSize()
            {
                this.m_StartSizeSet = false;
            }

            public void ResetStartColor()
            {
                this.m_StartColorSet = false;
            }

            public void ResetRandomSeed()
            {
                this.m_RandomSeedSet = false;
            }

            public void ResetStartLifetime()
            {
                this.m_StartLifetimeSet = false;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ExternalForcesModule
        {
            private ParticleSystem m_ParticleSystem;
            internal ExternalForcesModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            public bool enabled
            {
                get
                {
                    return GetEnabled(this.m_ParticleSystem);
                }
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            public float multiplier
            {
                get
                {
                    return GetMultiplier(this.m_ParticleSystem);
                }
                set
                {
                    SetMultiplier(this.m_ParticleSystem, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetMultiplier(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern float GetMultiplier(ParticleSystem system);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ForceOverLifetimeModule
        {
            private ParticleSystem m_ParticleSystem;
            internal ForceOverLifetimeModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            public bool enabled
            {
                get
                {
                    return GetEnabled(this.m_ParticleSystem);
                }
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            public ParticleSystem.MinMaxCurve x
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetX(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetX(this.m_ParticleSystem, ref value);
                }
            }
            public ParticleSystem.MinMaxCurve y
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetY(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetY(this.m_ParticleSystem, ref value);
                }
            }
            public ParticleSystem.MinMaxCurve z
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetZ(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetZ(this.m_ParticleSystem, ref value);
                }
            }
            public ParticleSystemSimulationSpace space
            {
                get
                {
                    return (!GetWorldSpace(this.m_ParticleSystem) ? ParticleSystemSimulationSpace.Local : ParticleSystemSimulationSpace.World);
                }
                set
                {
                    SetWorldSpace(this.m_ParticleSystem, value == ParticleSystemSimulationSpace.World);
                }
            }
            public bool randomized
            {
                get
                {
                    return GetRandomized(this.m_ParticleSystem);
                }
                set
                {
                    SetRandomized(this.m_ParticleSystem, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetWorldSpace(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetWorldSpace(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetRandomized(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetRandomized(ParticleSystem system);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct InheritVelocityModule
        {
            private ParticleSystem m_ParticleSystem;
            internal InheritVelocityModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            public bool enabled
            {
                get
                {
                    return GetEnabled(this.m_ParticleSystem);
                }
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            public ParticleSystemInheritVelocityMode mode
            {
                get
                {
                    return (ParticleSystemInheritVelocityMode) GetMode(this.m_ParticleSystem);
                }
                set
                {
                    SetMode(this.m_ParticleSystem, (int) value);
                }
            }
            public ParticleSystem.MinMaxCurve curve
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetCurve(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetCurve(this.m_ParticleSystem, ref value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetMode(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern int GetMode(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetCurve(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetCurve(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
        }

        internal delegate bool IteratorDelegate(ParticleSystem ps);

        [StructLayout(LayoutKind.Sequential)]
        public struct LimitVelocityOverLifetimeModule
        {
            private ParticleSystem m_ParticleSystem;
            internal LimitVelocityOverLifetimeModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            public bool enabled
            {
                get
                {
                    return GetEnabled(this.m_ParticleSystem);
                }
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            public ParticleSystem.MinMaxCurve limitX
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetX(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetX(this.m_ParticleSystem, ref value);
                }
            }
            public ParticleSystem.MinMaxCurve limitY
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetY(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetY(this.m_ParticleSystem, ref value);
                }
            }
            public ParticleSystem.MinMaxCurve limitZ
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetZ(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetZ(this.m_ParticleSystem, ref value);
                }
            }
            public ParticleSystem.MinMaxCurve limit
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetMagnitude(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetMagnitude(this.m_ParticleSystem, ref value);
                }
            }
            public float dampen
            {
                get
                {
                    return GetDampen(this.m_ParticleSystem);
                }
                set
                {
                    SetDampen(this.m_ParticleSystem, value);
                }
            }
            public bool separateAxes
            {
                get
                {
                    return GetSeparateAxes(this.m_ParticleSystem);
                }
                set
                {
                    SetSeparateAxes(this.m_ParticleSystem, value);
                }
            }
            public ParticleSystemSimulationSpace space
            {
                get
                {
                    return (!GetWorldSpace(this.m_ParticleSystem) ? ParticleSystemSimulationSpace.Local : ParticleSystemSimulationSpace.World);
                }
                set
                {
                    SetWorldSpace(this.m_ParticleSystem, value == ParticleSystemSimulationSpace.World);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetMagnitude(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetMagnitude(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetDampen(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern float GetDampen(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetSeparateAxes(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetSeparateAxes(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetWorldSpace(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetWorldSpace(ParticleSystem system);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MinMaxCurve
        {
            private ParticleSystemCurveMode m_Mode;
            private float m_CurveScalar;
            private AnimationCurve m_CurveMin;
            private AnimationCurve m_CurveMax;
            private float m_ConstantMin;
            private float m_ConstantMax;
            public MinMaxCurve(float constant)
            {
                this.m_Mode = ParticleSystemCurveMode.Constant;
                this.m_CurveScalar = 0f;
                this.m_CurveMin = null;
                this.m_CurveMax = null;
                this.m_ConstantMin = 0f;
                this.m_ConstantMax = constant;
            }

            public MinMaxCurve(float scalar, AnimationCurve curve)
            {
                this.m_Mode = ParticleSystemCurveMode.Curve;
                this.m_CurveScalar = scalar;
                this.m_CurveMin = null;
                this.m_CurveMax = curve;
                this.m_ConstantMin = 0f;
                this.m_ConstantMax = 0f;
            }

            public MinMaxCurve(float scalar, AnimationCurve min, AnimationCurve max)
            {
                this.m_Mode = ParticleSystemCurveMode.TwoCurves;
                this.m_CurveScalar = scalar;
                this.m_CurveMin = min;
                this.m_CurveMax = max;
                this.m_ConstantMin = 0f;
                this.m_ConstantMax = 0f;
            }

            public MinMaxCurve(float min, float max)
            {
                this.m_Mode = ParticleSystemCurveMode.TwoConstants;
                this.m_CurveScalar = 0f;
                this.m_CurveMin = null;
                this.m_CurveMax = null;
                this.m_ConstantMin = min;
                this.m_ConstantMax = max;
            }

            public ParticleSystemCurveMode mode
            {
                get
                {
                    return this.m_Mode;
                }
                set
                {
                    this.m_Mode = value;
                }
            }
            public float curveScalar
            {
                get
                {
                    return this.m_CurveScalar;
                }
                set
                {
                    this.m_CurveScalar = value;
                }
            }
            public AnimationCurve curveMax
            {
                get
                {
                    return this.m_CurveMax;
                }
                set
                {
                    this.m_CurveMax = value;
                }
            }
            public AnimationCurve curveMin
            {
                get
                {
                    return this.m_CurveMin;
                }
                set
                {
                    this.m_CurveMin = value;
                }
            }
            public float constantMax
            {
                get
                {
                    return this.m_ConstantMax;
                }
                set
                {
                    this.m_ConstantMax = value;
                }
            }
            public float constantMin
            {
                get
                {
                    return this.m_ConstantMin;
                }
                set
                {
                    this.m_ConstantMin = value;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MinMaxGradient
        {
            private ParticleSystemGradientMode m_Mode;
            private Gradient m_GradientMin;
            private Gradient m_GradientMax;
            private Color m_ColorMin;
            private Color m_ColorMax;
            public MinMaxGradient(Color color)
            {
                this.m_Mode = ParticleSystemGradientMode.Color;
                this.m_GradientMin = null;
                this.m_GradientMax = null;
                this.m_ColorMin = Color.black;
                this.m_ColorMax = color;
            }

            public MinMaxGradient(Gradient gradient)
            {
                this.m_Mode = ParticleSystemGradientMode.Gradient;
                this.m_GradientMin = null;
                this.m_GradientMax = gradient;
                this.m_ColorMin = Color.black;
                this.m_ColorMax = Color.black;
            }

            public MinMaxGradient(Color min, Color max)
            {
                this.m_Mode = ParticleSystemGradientMode.TwoColors;
                this.m_GradientMin = null;
                this.m_GradientMax = null;
                this.m_ColorMin = min;
                this.m_ColorMax = max;
            }

            public MinMaxGradient(Gradient min, Gradient max)
            {
                this.m_Mode = ParticleSystemGradientMode.TwoGradients;
                this.m_GradientMin = min;
                this.m_GradientMax = max;
                this.m_ColorMin = Color.black;
                this.m_ColorMax = Color.black;
            }

            public ParticleSystemGradientMode mode
            {
                get
                {
                    return this.m_Mode;
                }
                set
                {
                    this.m_Mode = value;
                }
            }
            public Gradient gradientMax
            {
                get
                {
                    return this.m_GradientMax;
                }
                set
                {
                    this.m_GradientMax = value;
                }
            }
            public Gradient gradientMin
            {
                get
                {
                    return this.m_GradientMin;
                }
                set
                {
                    this.m_GradientMin = value;
                }
            }
            public Color colorMax
            {
                get
                {
                    return this.m_ColorMax;
                }
                set
                {
                    this.m_ColorMax = value;
                }
            }
            public Color colorMin
            {
                get
                {
                    return this.m_ColorMin;
                }
                set
                {
                    this.m_ColorMin = value;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Particle
        {
            private Vector3 m_Position;
            private Vector3 m_Velocity;
            private Vector3 m_AnimatedVelocity;
            private Vector3 m_InitialVelocity;
            private Vector3 m_AxisOfRotation;
            private Vector3 m_Rotation;
            private Vector3 m_AngularVelocity;
            private float m_StartSize;
            private Color32 m_StartColor;
            private uint m_RandomSeed;
            private float m_Lifetime;
            private float m_StartLifetime;
            private float m_EmitAccumulator0;
            private float m_EmitAccumulator1;
            public Vector3 position
            {
                get
                {
                    return this.m_Position;
                }
                set
                {
                    this.m_Position = value;
                }
            }
            public Vector3 velocity
            {
                get
                {
                    return this.m_Velocity;
                }
                set
                {
                    this.m_Velocity = value;
                }
            }
            public float lifetime
            {
                get
                {
                    return this.m_Lifetime;
                }
                set
                {
                    this.m_Lifetime = value;
                }
            }
            public float startLifetime
            {
                get
                {
                    return this.m_StartLifetime;
                }
                set
                {
                    this.m_StartLifetime = value;
                }
            }
            public float startSize
            {
                get
                {
                    return this.m_StartSize;
                }
                set
                {
                    this.m_StartSize = value;
                }
            }
            public Vector3 axisOfRotation
            {
                get
                {
                    return this.m_AxisOfRotation;
                }
                set
                {
                    this.m_AxisOfRotation = value;
                }
            }
            public float rotation
            {
                get
                {
                    return (this.m_Rotation.z * 57.29578f);
                }
                set
                {
                    this.m_Rotation = new Vector3(0f, 0f, value * 0.01745329f);
                }
            }
            public Vector3 rotation3D
            {
                get
                {
                    return (Vector3) (this.m_Rotation * 57.29578f);
                }
                set
                {
                    this.m_Rotation = (Vector3) (value * 0.01745329f);
                }
            }
            public float angularVelocity
            {
                get
                {
                    return (this.m_AngularVelocity.z * 57.29578f);
                }
                set
                {
                    this.m_AngularVelocity.z = value * 0.01745329f;
                }
            }
            public Vector3 angularVelocity3D
            {
                get
                {
                    return (Vector3) (this.m_AngularVelocity * 57.29578f);
                }
                set
                {
                    this.m_AngularVelocity = (Vector3) (value * 0.01745329f);
                }
            }
            public Color32 startColor
            {
                get
                {
                    return this.m_StartColor;
                }
                set
                {
                    this.m_StartColor = value;
                }
            }
            [Obsolete("randomValue property is deprecated. Use randomSeed instead to control random behavior of particles.")]
            public float randomValue
            {
                get
                {
                    return BitConverter.ToSingle(BitConverter.GetBytes(this.m_RandomSeed), 0);
                }
                set
                {
                    this.m_RandomSeed = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
                }
            }
            public uint randomSeed
            {
                get
                {
                    return this.m_RandomSeed;
                }
                set
                {
                    this.m_RandomSeed = value;
                }
            }
            public float GetCurrentSize(ParticleSystem system)
            {
                return GetCurrentSize(system, ref this);
            }

            public Color32 GetCurrentColor(ParticleSystem system)
            {
                return GetCurrentColor(system, ref this);
            }

            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern float GetCurrentSize(ParticleSystem system, ref ParticleSystem.Particle particle);
            private static Color32 GetCurrentColor(ParticleSystem system, ref ParticleSystem.Particle particle)
            {
                Color32 color;
                INTERNAL_CALL_GetCurrentColor(system, ref particle, out color);
                return color;
            }

            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void INTERNAL_CALL_GetCurrentColor(ParticleSystem system, ref ParticleSystem.Particle particle, out Color32 value);
            [Obsolete("size property is deprecated. Use startSize or GetCurrentSize() instead.")]
            public float size
            {
                get
                {
                    return this.m_StartSize;
                }
                set
                {
                    this.m_StartSize = value;
                }
            }
            [Obsolete("color property is deprecated. Use startColor or GetCurrentColor() instead.")]
            public Color32 color
            {
                get
                {
                    return this.m_StartColor;
                }
                set
                {
                    this.m_StartColor = value;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RotationBySpeedModule
        {
            private ParticleSystem m_ParticleSystem;
            internal RotationBySpeedModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            public bool enabled
            {
                get
                {
                    return GetEnabled(this.m_ParticleSystem);
                }
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            public ParticleSystem.MinMaxCurve x
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetX(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetX(this.m_ParticleSystem, ref value);
                }
            }
            public ParticleSystem.MinMaxCurve y
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetY(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetY(this.m_ParticleSystem, ref value);
                }
            }
            public ParticleSystem.MinMaxCurve z
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetZ(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetZ(this.m_ParticleSystem, ref value);
                }
            }
            public bool separateAxes
            {
                get
                {
                    return GetSeparateAxes(this.m_ParticleSystem);
                }
                set
                {
                    SetSeparateAxes(this.m_ParticleSystem, value);
                }
            }
            public Vector2 range
            {
                get
                {
                    return GetRange(this.m_ParticleSystem);
                }
                set
                {
                    SetRange(this.m_ParticleSystem, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetSeparateAxes(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetSeparateAxes(ParticleSystem system);
            private static void SetRange(ParticleSystem system, Vector2 value)
            {
                INTERNAL_CALL_SetRange(system, ref value);
            }

            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void INTERNAL_CALL_SetRange(ParticleSystem system, ref Vector2 value);
            private static Vector2 GetRange(ParticleSystem system)
            {
                Vector2 vector;
                INTERNAL_CALL_GetRange(system, out vector);
                return vector;
            }

            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void INTERNAL_CALL_GetRange(ParticleSystem system, out Vector2 value);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RotationOverLifetimeModule
        {
            private ParticleSystem m_ParticleSystem;
            internal RotationOverLifetimeModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            public bool enabled
            {
                get
                {
                    return GetEnabled(this.m_ParticleSystem);
                }
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            public ParticleSystem.MinMaxCurve x
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetX(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetX(this.m_ParticleSystem, ref value);
                }
            }
            public ParticleSystem.MinMaxCurve y
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetY(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetY(this.m_ParticleSystem, ref value);
                }
            }
            public ParticleSystem.MinMaxCurve z
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetZ(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetZ(this.m_ParticleSystem, ref value);
                }
            }
            public bool separateAxes
            {
                get
                {
                    return GetSeparateAxes(this.m_ParticleSystem);
                }
                set
                {
                    SetSeparateAxes(this.m_ParticleSystem, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetSeparateAxes(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetSeparateAxes(ParticleSystem system);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ShapeModule
        {
            private ParticleSystem m_ParticleSystem;
            internal ShapeModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            public bool enabled
            {
                get
                {
                    return GetEnabled(this.m_ParticleSystem);
                }
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            public ParticleSystemShapeType shapeType
            {
                get
                {
                    return (ParticleSystemShapeType) GetShapeType(this.m_ParticleSystem);
                }
                set
                {
                    SetShapeType(this.m_ParticleSystem, (int) value);
                }
            }
            public bool randomDirection
            {
                get
                {
                    return GetRandomDirection(this.m_ParticleSystem);
                }
                set
                {
                    SetRandomDirection(this.m_ParticleSystem, value);
                }
            }
            public float radius
            {
                get
                {
                    return GetRadius(this.m_ParticleSystem);
                }
                set
                {
                    SetRadius(this.m_ParticleSystem, value);
                }
            }
            public float angle
            {
                get
                {
                    return GetAngle(this.m_ParticleSystem);
                }
                set
                {
                    SetAngle(this.m_ParticleSystem, value);
                }
            }
            public float length
            {
                get
                {
                    return GetLength(this.m_ParticleSystem);
                }
                set
                {
                    SetLength(this.m_ParticleSystem, value);
                }
            }
            public Vector3 box
            {
                get
                {
                    return GetBox(this.m_ParticleSystem);
                }
                set
                {
                    SetBox(this.m_ParticleSystem, value);
                }
            }
            public ParticleSystemMeshShapeType meshShapeType
            {
                get
                {
                    return (ParticleSystemMeshShapeType) GetMeshShapeType(this.m_ParticleSystem);
                }
                set
                {
                    SetMeshShapeType(this.m_ParticleSystem, (int) value);
                }
            }
            public Mesh mesh
            {
                get
                {
                    return GetMesh(this.m_ParticleSystem);
                }
                set
                {
                    SetMesh(this.m_ParticleSystem, value);
                }
            }
            public MeshRenderer meshRenderer
            {
                get
                {
                    return GetMeshRenderer(this.m_ParticleSystem);
                }
                set
                {
                    SetMeshRenderer(this.m_ParticleSystem, value);
                }
            }
            public SkinnedMeshRenderer skinnedMeshRenderer
            {
                get
                {
                    return GetSkinnedMeshRenderer(this.m_ParticleSystem);
                }
                set
                {
                    SetSkinnedMeshRenderer(this.m_ParticleSystem, value);
                }
            }
            public bool useMeshMaterialIndex
            {
                get
                {
                    return GetUseMeshMaterialIndex(this.m_ParticleSystem);
                }
                set
                {
                    SetUseMeshMaterialIndex(this.m_ParticleSystem, value);
                }
            }
            public int meshMaterialIndex
            {
                get
                {
                    return GetMeshMaterialIndex(this.m_ParticleSystem);
                }
                set
                {
                    SetMeshMaterialIndex(this.m_ParticleSystem, value);
                }
            }
            public bool useMeshColors
            {
                get
                {
                    return GetUseMeshColors(this.m_ParticleSystem);
                }
                set
                {
                    SetUseMeshColors(this.m_ParticleSystem, value);
                }
            }
            public float normalOffset
            {
                get
                {
                    return GetNormalOffset(this.m_ParticleSystem);
                }
                set
                {
                    SetNormalOffset(this.m_ParticleSystem, value);
                }
            }
            public float arc
            {
                get
                {
                    return GetArc(this.m_ParticleSystem);
                }
                set
                {
                    SetArc(this.m_ParticleSystem, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetShapeType(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern int GetShapeType(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetRandomDirection(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetRandomDirection(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetRadius(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern float GetRadius(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetAngle(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern float GetAngle(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetLength(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern float GetLength(ParticleSystem system);
            private static void SetBox(ParticleSystem system, Vector3 value)
            {
                INTERNAL_CALL_SetBox(system, ref value);
            }

            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void INTERNAL_CALL_SetBox(ParticleSystem system, ref Vector3 value);
            private static Vector3 GetBox(ParticleSystem system)
            {
                Vector3 vector;
                INTERNAL_CALL_GetBox(system, out vector);
                return vector;
            }

            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void INTERNAL_CALL_GetBox(ParticleSystem system, out Vector3 value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetMeshShapeType(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern int GetMeshShapeType(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetMesh(ParticleSystem system, Mesh value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern Mesh GetMesh(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetMeshRenderer(ParticleSystem system, MeshRenderer value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern MeshRenderer GetMeshRenderer(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetSkinnedMeshRenderer(ParticleSystem system, SkinnedMeshRenderer value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern SkinnedMeshRenderer GetSkinnedMeshRenderer(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetUseMeshMaterialIndex(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetUseMeshMaterialIndex(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetMeshMaterialIndex(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern int GetMeshMaterialIndex(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetUseMeshColors(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetUseMeshColors(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetNormalOffset(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern float GetNormalOffset(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetArc(ParticleSystem system, float value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern float GetArc(ParticleSystem system);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SizeBySpeedModule
        {
            private ParticleSystem m_ParticleSystem;
            internal SizeBySpeedModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            public bool enabled
            {
                get
                {
                    return GetEnabled(this.m_ParticleSystem);
                }
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            public ParticleSystem.MinMaxCurve size
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetSize(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetSize(this.m_ParticleSystem, ref value);
                }
            }
            public Vector2 range
            {
                get
                {
                    return GetRange(this.m_ParticleSystem);
                }
                set
                {
                    SetRange(this.m_ParticleSystem, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetSize(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetSize(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            private static void SetRange(ParticleSystem system, Vector2 value)
            {
                INTERNAL_CALL_SetRange(system, ref value);
            }

            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void INTERNAL_CALL_SetRange(ParticleSystem system, ref Vector2 value);
            private static Vector2 GetRange(ParticleSystem system)
            {
                Vector2 vector;
                INTERNAL_CALL_GetRange(system, out vector);
                return vector;
            }

            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void INTERNAL_CALL_GetRange(ParticleSystem system, out Vector2 value);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SizeOverLifetimeModule
        {
            private ParticleSystem m_ParticleSystem;
            internal SizeOverLifetimeModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            public bool enabled
            {
                get
                {
                    return GetEnabled(this.m_ParticleSystem);
                }
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            public ParticleSystem.MinMaxCurve size
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetSize(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetSize(this.m_ParticleSystem, ref value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetSize(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetSize(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SubEmittersModule
        {
            private ParticleSystem m_ParticleSystem;
            internal SubEmittersModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            public bool enabled
            {
                get
                {
                    return GetEnabled(this.m_ParticleSystem);
                }
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            public ParticleSystem birth0
            {
                get
                {
                    return GetBirth(this.m_ParticleSystem, 0);
                }
                set
                {
                    SetBirth(this.m_ParticleSystem, 0, value);
                }
            }
            public ParticleSystem birth1
            {
                get
                {
                    return GetBirth(this.m_ParticleSystem, 1);
                }
                set
                {
                    SetBirth(this.m_ParticleSystem, 1, value);
                }
            }
            public ParticleSystem collision0
            {
                get
                {
                    return GetCollision(this.m_ParticleSystem, 0);
                }
                set
                {
                    SetCollision(this.m_ParticleSystem, 0, value);
                }
            }
            public ParticleSystem collision1
            {
                get
                {
                    return GetCollision(this.m_ParticleSystem, 1);
                }
                set
                {
                    SetCollision(this.m_ParticleSystem, 1, value);
                }
            }
            public ParticleSystem death0
            {
                get
                {
                    return GetDeath(this.m_ParticleSystem, 0);
                }
                set
                {
                    SetDeath(this.m_ParticleSystem, 0, value);
                }
            }
            public ParticleSystem death1
            {
                get
                {
                    return GetDeath(this.m_ParticleSystem, 1);
                }
                set
                {
                    SetDeath(this.m_ParticleSystem, 1, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetBirth(ParticleSystem system, int index, ParticleSystem value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern ParticleSystem GetBirth(ParticleSystem system, int index);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetCollision(ParticleSystem system, int index, ParticleSystem value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern ParticleSystem GetCollision(ParticleSystem system, int index);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetDeath(ParticleSystem system, int index, ParticleSystem value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern ParticleSystem GetDeath(ParticleSystem system, int index);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TextureSheetAnimationModule
        {
            private ParticleSystem m_ParticleSystem;
            internal TextureSheetAnimationModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            public bool enabled
            {
                get
                {
                    return GetEnabled(this.m_ParticleSystem);
                }
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            public int numTilesX
            {
                get
                {
                    return GetNumTilesX(this.m_ParticleSystem);
                }
                set
                {
                    SetNumTilesX(this.m_ParticleSystem, value);
                }
            }
            public int numTilesY
            {
                get
                {
                    return GetNumTilesY(this.m_ParticleSystem);
                }
                set
                {
                    SetNumTilesY(this.m_ParticleSystem, value);
                }
            }
            public ParticleSystemAnimationType animation
            {
                get
                {
                    return (ParticleSystemAnimationType) GetAnimationType(this.m_ParticleSystem);
                }
                set
                {
                    SetAnimationType(this.m_ParticleSystem, (int) value);
                }
            }
            public bool useRandomRow
            {
                get
                {
                    return GetUseRandomRow(this.m_ParticleSystem);
                }
                set
                {
                    SetUseRandomRow(this.m_ParticleSystem, value);
                }
            }
            public ParticleSystem.MinMaxCurve frameOverTime
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetFrameOverTime(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetFrameOverTime(this.m_ParticleSystem, ref value);
                }
            }
            public int cycleCount
            {
                get
                {
                    return GetCycleCount(this.m_ParticleSystem);
                }
                set
                {
                    SetCycleCount(this.m_ParticleSystem, value);
                }
            }
            public int rowIndex
            {
                get
                {
                    return GetRowIndex(this.m_ParticleSystem);
                }
                set
                {
                    SetRowIndex(this.m_ParticleSystem, value);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetNumTilesX(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern int GetNumTilesX(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetNumTilesY(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern int GetNumTilesY(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetAnimationType(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern int GetAnimationType(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetUseRandomRow(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetUseRandomRow(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetFrameOverTime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetFrameOverTime(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetCycleCount(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern int GetCycleCount(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetRowIndex(ParticleSystem system, int value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern int GetRowIndex(ParticleSystem system);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct VelocityOverLifetimeModule
        {
            private ParticleSystem m_ParticleSystem;
            internal VelocityOverLifetimeModule(ParticleSystem particleSystem)
            {
                this.m_ParticleSystem = particleSystem;
            }

            public bool enabled
            {
                get
                {
                    return GetEnabled(this.m_ParticleSystem);
                }
                set
                {
                    SetEnabled(this.m_ParticleSystem, value);
                }
            }
            public ParticleSystem.MinMaxCurve x
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetX(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetX(this.m_ParticleSystem, ref value);
                }
            }
            public ParticleSystem.MinMaxCurve y
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetY(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetY(this.m_ParticleSystem, ref value);
                }
            }
            public ParticleSystem.MinMaxCurve z
            {
                get
                {
                    ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve();
                    GetZ(this.m_ParticleSystem, ref curve);
                    return curve;
                }
                set
                {
                    SetZ(this.m_ParticleSystem, ref value);
                }
            }
            public ParticleSystemSimulationSpace space
            {
                get
                {
                    return (!GetWorldSpace(this.m_ParticleSystem) ? ParticleSystemSimulationSpace.Local : ParticleSystemSimulationSpace.World);
                }
                set
                {
                    SetWorldSpace(this.m_ParticleSystem, value == ParticleSystemSimulationSpace.World);
                }
            }
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetEnabled(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetEnabled(ParticleSystem system);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetX(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetY(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void GetZ(ParticleSystem system, ref ParticleSystem.MinMaxCurve curve);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern void SetWorldSpace(ParticleSystem system, bool value);
            [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
            private static extern bool GetWorldSpace(ParticleSystem system);
        }
    }
}

