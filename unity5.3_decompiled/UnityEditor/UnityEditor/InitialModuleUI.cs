namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class InitialModuleUI : ModuleUI
    {
        public SerializedMinMaxGradient m_Color;
        public SerializedProperty m_GravityModifier;
        public SerializedProperty m_LengthInSec;
        public SerializedMinMaxCurve m_LifeTime;
        public SerializedProperty m_Looping;
        public SerializedProperty m_MaxNumParticles;
        public SerializedProperty m_PlayOnAwake;
        public SerializedProperty m_Prewarm;
        public SerializedProperty m_RandomizeRotationDirection;
        public SerializedProperty m_Rotation3D;
        public SerializedMinMaxCurve m_RotationX;
        public SerializedMinMaxCurve m_RotationY;
        public SerializedMinMaxCurve m_RotationZ;
        public SerializedProperty m_ScalingMode;
        public SerializedProperty m_SimulationSpace;
        public SerializedMinMaxCurve m_Size;
        public SerializedMinMaxCurve m_Speed;
        public SerializedMinMaxCurve m_StartDelay;
        private static Texts s_Texts;

        public InitialModuleUI(ParticleSystemUI owner, SerializedObject o, string displayName) : base(owner, o, "InitialModule", displayName, ModuleUI.VisibilityState.VisibleAndFoldedOut)
        {
            this.Init();
        }

        public override float GetXAxisScalar()
        {
            return base.m_ParticleSystemUI.GetEmitterDuration();
        }

        protected override void Init()
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            if (this.m_LengthInSec == null)
            {
                this.m_LengthInSec = base.GetProperty0("lengthInSec");
                this.m_Looping = base.GetProperty0("looping");
                this.m_Prewarm = base.GetProperty0("prewarm");
                this.m_StartDelay = new SerializedMinMaxCurve(this, s_Texts.startDelay, "startDelay", false, true);
                this.m_StartDelay.m_AllowCurves = false;
                this.m_PlayOnAwake = base.GetProperty0("playOnAwake");
                this.m_SimulationSpace = base.GetProperty0("moveWithTransform");
                this.m_ScalingMode = base.GetProperty0("scalingMode");
                this.m_LifeTime = new SerializedMinMaxCurve(this, s_Texts.lifetime, "startLifetime");
                this.m_Speed = new SerializedMinMaxCurve(this, s_Texts.speed, "startSpeed", ModuleUI.kUseSignedRange);
                this.m_Color = new SerializedMinMaxGradient(this, "startColor");
                this.m_Size = new SerializedMinMaxCurve(this, s_Texts.size, "startSize");
                this.m_Rotation3D = base.GetProperty("rotation3D");
                this.m_RotationX = new SerializedMinMaxCurve(this, s_Texts.x, "startRotationX", ModuleUI.kUseSignedRange);
                this.m_RotationY = new SerializedMinMaxCurve(this, s_Texts.y, "startRotationY", ModuleUI.kUseSignedRange);
                this.m_RotationZ = new SerializedMinMaxCurve(this, s_Texts.z, "startRotation", ModuleUI.kUseSignedRange);
                this.m_RotationX.m_RemapValue = 57.29578f;
                this.m_RotationY.m_RemapValue = 57.29578f;
                this.m_RotationZ.m_RemapValue = 57.29578f;
                this.m_RotationX.m_DefaultCurveScalar = 3.141593f;
                this.m_RotationY.m_DefaultCurveScalar = 3.141593f;
                this.m_RotationZ.m_DefaultCurveScalar = 3.141593f;
                this.m_RandomizeRotationDirection = base.GetProperty("randomizeRotationDirection");
                this.m_GravityModifier = base.GetProperty("gravityModifier");
                this.m_MaxNumParticles = base.GetProperty("maxNumParticles");
            }
        }

        public override void OnInspectorGUI(ParticleSystem s)
        {
            if (s_Texts == null)
            {
                s_Texts = new Texts();
            }
            ModuleUI.GUIFloat(s_Texts.duration, this.m_LengthInSec, "f2");
            this.m_LengthInSec.floatValue = Mathf.Min(100000f, Mathf.Max(0f, this.m_LengthInSec.floatValue));
            bool boolValue = this.m_Looping.boolValue;
            ModuleUI.GUIToggle(s_Texts.looping, this.m_Looping);
            if ((this.m_Looping.boolValue && !boolValue) && (s.time >= this.m_LengthInSec.floatValue))
            {
                s.time = 0f;
            }
            EditorGUI.BeginDisabledGroup(!this.m_Looping.boolValue);
            ModuleUI.GUIToggle(s_Texts.prewarm, this.m_Prewarm);
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(this.m_Prewarm.boolValue && this.m_Looping.boolValue);
            ModuleUI.GUIMinMaxCurve(s_Texts.startDelay, this.m_StartDelay);
            EditorGUI.EndDisabledGroup();
            ModuleUI.GUIMinMaxCurve(s_Texts.lifetime, this.m_LifeTime);
            ModuleUI.GUIMinMaxCurve(s_Texts.speed, this.m_Speed);
            ModuleUI.GUIMinMaxCurve(s_Texts.size, this.m_Size);
            EditorGUI.BeginChangeCheck();
            bool flag2 = ModuleUI.GUIToggle(s_Texts.rotation3D, this.m_Rotation3D);
            if (EditorGUI.EndChangeCheck())
            {
                if (flag2)
                {
                    this.m_RotationZ.RemoveCurveFromEditor();
                }
                else
                {
                    this.m_RotationX.RemoveCurveFromEditor();
                    this.m_RotationY.RemoveCurveFromEditor();
                    this.m_RotationZ.RemoveCurveFromEditor();
                }
            }
            if (flag2)
            {
                this.m_RotationZ.m_DisplayName = s_Texts.z;
                base.GUITripleMinMaxCurve(GUIContent.none, s_Texts.x, this.m_RotationX, s_Texts.y, this.m_RotationY, s_Texts.z, this.m_RotationZ, null);
            }
            else
            {
                this.m_RotationZ.m_DisplayName = s_Texts.rotation;
                ModuleUI.GUIMinMaxCurve(s_Texts.rotation, this.m_RotationZ);
            }
            ModuleUI.GUIFloat(s_Texts.randomizeRotationDirection, this.m_RandomizeRotationDirection);
            base.GUIMinMaxGradient(s_Texts.color, this.m_Color);
            ModuleUI.GUIFloat(s_Texts.gravity, this.m_GravityModifier);
            ModuleUI.GUIBoolAsPopup(s_Texts.simulationSpace, this.m_SimulationSpace, s_Texts.simulationSpaces);
            string[] options = new string[] { "Hierarchy", "Local", "Shape" };
            ModuleUI.GUIPopup(s_Texts.scalingMode, this.m_ScalingMode, options);
            bool flag3 = this.m_PlayOnAwake.boolValue;
            bool newPlayOnAwake = ModuleUI.GUIToggle(s_Texts.autoplay, this.m_PlayOnAwake);
            if (flag3 != newPlayOnAwake)
            {
                base.m_ParticleSystemUI.m_ParticleEffectUI.PlayOnAwakeChanged(newPlayOnAwake);
            }
            ModuleUI.GUIInt(s_Texts.maxParticles, this.m_MaxNumParticles);
        }

        public override void UpdateCullingSupportedString(ref string text)
        {
            if (!this.m_SimulationSpace.boolValue)
            {
                text = text + "\n\tWorld space simulation is used.";
            }
        }

        private class Texts
        {
            public GUIContent autoplay = new GUIContent("Play On Awake*", "If enabled, the system will start playing automatically. Note that this setting is shared between all particle systems in the current particle effect.");
            public GUIContent color = new GUIContent("Start Color", "The start color of particles.");
            public GUIContent duration = new GUIContent("Duration", "The length of time the Particle System is emitting particles, if the system is looping, this indicates the length of one cycle.");
            public GUIContent gravity = new GUIContent("Gravity Modifier", "Scales the gravity defined in Physics Manager");
            public GUIContent lifetime = new GUIContent("Start Lifetime", "Start lifetime in seconds, particle will die when its lifetime reaches 0.");
            public GUIContent looping = new GUIContent("Looping", "If true, the emission cycle will repeat after the duration.");
            public GUIContent maxParticles = new GUIContent("Max Particles", "The number of particles in the system will be limited by this number. Emission will be temporarily halted if this is reached.");
            public GUIContent prewarm = new GUIContent("Prewarm", "When played a prewarmed system will be in a state as if it had emitted one loop cycle. Can only be used if the system is looping.");
            public GUIContent randomizeRotationDirection = new GUIContent("Randomize Rotation Direction", "Cause some particles to spin in the opposite direction. (Set between 0 and 1, where a higher value causes more to flip)");
            public GUIContent rotation = new GUIContent("Start Rotation", "The start rotation of particles in degrees.");
            public GUIContent rotation3D = new GUIContent("3D Start Rotation", "If enabled, you can control the rotation separately for each axis.");
            public GUIContent scalingMode = new GUIContent("Scaling Mode", "Should we use the combined scale from our entire hierarchy, just this particle node, or just apply scale to the shape module?");
            public GUIContent simulationSpace = new GUIContent("Simulation Space", "Makes particle positions simulate in worldspace or local space. In local space they stay relative to the Transform.");
            public string[] simulationSpaces = new string[] { "World", "Local" };
            public GUIContent size = new GUIContent("Start Size", "The start size of particles.");
            public GUIContent speed = new GUIContent("Start Speed", "The start speed of particles, applied in the starting direction.");
            public GUIContent startDelay = new GUIContent("Start Delay", "Delay in seconds that this Particle System will wait before emitting particles. Cannot be used together with a prewarmed looping system.");
            public GUIContent x = new GUIContent("X");
            public GUIContent y = new GUIContent("Y");
            public GUIContent z = new GUIContent("Z");
        }
    }
}

