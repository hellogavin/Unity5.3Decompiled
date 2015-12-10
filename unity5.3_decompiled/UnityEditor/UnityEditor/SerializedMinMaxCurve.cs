namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal class SerializedMinMaxCurve
    {
        public bool m_AllowConstant;
        public bool m_AllowCurves;
        public bool m_AllowRandom;
        public float m_DefaultCurveScalar;
        public GUIContent m_DisplayName;
        public float m_MaxAllowedScalar;
        public ModuleUI m_Module;
        private string m_Name;
        public float m_RemapValue;
        private bool m_SignedRange;
        public SerializedProperty maxCurve;
        public SerializedProperty maxCurveFirstKeyValue;
        public SerializedProperty minCurve;
        public SerializedProperty minCurveFirstKeyValue;
        private SerializedProperty minMaxState;
        public SerializedProperty scalar;

        public SerializedMinMaxCurve(ModuleUI m, GUIContent displayName)
        {
            this.m_MaxAllowedScalar = float.PositiveInfinity;
            this.Init(m, displayName, "curve", false, false);
        }

        public SerializedMinMaxCurve(ModuleUI m, GUIContent displayName, bool signedRange)
        {
            this.m_MaxAllowedScalar = float.PositiveInfinity;
            this.Init(m, displayName, "curve", signedRange, false);
        }

        public SerializedMinMaxCurve(ModuleUI m, GUIContent displayName, string name)
        {
            this.m_MaxAllowedScalar = float.PositiveInfinity;
            this.Init(m, displayName, name, false, false);
        }

        public SerializedMinMaxCurve(ModuleUI m, GUIContent displayName, string name, bool signedRange)
        {
            this.m_MaxAllowedScalar = float.PositiveInfinity;
            this.Init(m, displayName, name, signedRange, false);
        }

        public SerializedMinMaxCurve(ModuleUI m, GUIContent displayName, string name, bool signedRange, bool useProp0)
        {
            this.m_MaxAllowedScalar = float.PositiveInfinity;
            this.Init(m, displayName, name, signedRange, useProp0);
        }

        private float ClampValueToMaxAllowed(float val)
        {
            if (Mathf.Abs(val) > this.m_MaxAllowedScalar)
            {
                return (this.m_MaxAllowedScalar * Mathf.Sign(val));
            }
            return val;
        }

        public ParticleSystemCurveEditor.CurveData CreateCurveData(Color color)
        {
            return new ParticleSystemCurveEditor.CurveData(this.GetUniqueCurveName(), this.m_DisplayName, this.GetMinCurve(), this.maxCurve, color, this.m_SignedRange, new CurveWrapper.GetAxisScalarsCallback(this.GetAxisScalars), new CurveWrapper.SetAxisScalarsCallback(this.SetAxisScalars), this.m_Module.foldout);
        }

        private float GetAverageKeyValue(Keyframe[] keyFrames)
        {
            float num = 0f;
            foreach (Keyframe keyframe in keyFrames)
            {
                num += keyframe.value;
            }
            return (num / ((float) keyFrames.Length));
        }

        public Vector2 GetAxisScalars()
        {
            return new Vector2(this.m_Module.GetXAxisScalar(), this.scalar.floatValue * this.m_RemapValue);
        }

        private float GetMaxKeyValue(Keyframe[] keyFrames)
        {
            float negativeInfinity = float.NegativeInfinity;
            float positiveInfinity = float.PositiveInfinity;
            foreach (Keyframe keyframe in keyFrames)
            {
                if (keyframe.value > negativeInfinity)
                {
                    negativeInfinity = keyframe.value;
                }
                if (keyframe.value < positiveInfinity)
                {
                    positiveInfinity = keyframe.value;
                }
            }
            if (Mathf.Abs(positiveInfinity) > negativeInfinity)
            {
                return positiveInfinity;
            }
            return negativeInfinity;
        }

        private SerializedProperty GetMinCurve()
        {
            return ((this.state != MinMaxCurveState.k_TwoCurves) ? null : this.minCurve);
        }

        private float GetNormalizedValueFromScalar()
        {
            if (this.scalar.floatValue < 0f)
            {
                return -1f;
            }
            if (this.scalar.floatValue > 0f)
            {
                return 1f;
            }
            return 0f;
        }

        public string GetUniqueCurveName()
        {
            return SerializedModule.Concat(this.m_Module.GetUniqueModuleName(), this.m_Name);
        }

        private void Init(ModuleUI m, GUIContent displayName, string uniqueName, bool signedRange, bool useProp0)
        {
            this.m_Module = m;
            this.m_DisplayName = displayName;
            this.m_Name = uniqueName;
            this.m_SignedRange = signedRange;
            this.m_RemapValue = 1f;
            this.m_DefaultCurveScalar = 1f;
            this.m_AllowConstant = true;
            this.m_AllowRandom = true;
            this.m_AllowCurves = true;
            this.scalar = !useProp0 ? m.GetProperty(this.m_Name, "scalar") : m.GetProperty0(this.m_Name, "scalar");
            this.maxCurve = !useProp0 ? m.GetProperty(this.m_Name, "maxCurve") : m.GetProperty0(this.m_Name, "maxCurve");
            this.maxCurveFirstKeyValue = this.maxCurve.FindPropertyRelative("m_Curve.Array.data[0].value");
            this.minCurve = !useProp0 ? m.GetProperty(this.m_Name, "minCurve") : m.GetProperty0(this.m_Name, "minCurve");
            this.minCurveFirstKeyValue = this.minCurve.FindPropertyRelative("m_Curve.Array.data[0].value");
            this.minMaxState = !useProp0 ? m.GetProperty(this.m_Name, "minMaxState") : m.GetProperty0(this.m_Name, "minMaxState");
            if (((this.state == MinMaxCurveState.k_Curve) || (this.state == MinMaxCurveState.k_TwoCurves)) && this.m_Module.m_ParticleSystemUI.m_ParticleEffectUI.IsParticleSystemUIVisible(this.m_Module.m_ParticleSystemUI))
            {
                m.GetParticleSystemCurveEditor().AddCurveDataIfNeeded(this.GetUniqueCurveName(), this.CreateCurveData(Color.black));
            }
            m.AddToModuleCurves(this.maxCurve);
        }

        private void InitDoubleCurves(MinMaxCurveState oldState)
        {
            switch (oldState)
            {
                case MinMaxCurveState.k_Scalar:
                    this.SetCurveConstant(this.maxCurve, this.GetNormalizedValueFromScalar());
                    break;
            }
            this.SetCurveRequirements();
        }

        private void InitDoubleScalars(MinMaxCurveState oldState)
        {
            this.minConstant = this.GetAverageKeyValue(this.minCurve.animationCurveValue.keys) * this.scalar.floatValue;
            switch (oldState)
            {
                case MinMaxCurveState.k_Scalar:
                    this.maxConstant = this.scalar.floatValue;
                    break;

                case MinMaxCurveState.k_Curve:
                case MinMaxCurveState.k_TwoCurves:
                    this.maxConstant = this.GetAverageKeyValue(this.maxCurve.animationCurveValue.keys) * this.scalar.floatValue;
                    break;

                default:
                    Debug.LogError("Enum not handled!");
                    break;
            }
            this.SetCurveRequirements();
        }

        private void InitSingleCurve(MinMaxCurveState oldState)
        {
            switch (oldState)
            {
                case MinMaxCurveState.k_Scalar:
                    this.SetCurveConstant(this.maxCurve, this.GetNormalizedValueFromScalar());
                    break;
            }
            this.SetCurveRequirements();
        }

        private void InitSingleScalar(MinMaxCurveState oldState)
        {
            switch (oldState)
            {
                case MinMaxCurveState.k_Curve:
                case MinMaxCurveState.k_TwoCurves:
                case MinMaxCurveState.k_TwoScalars:
                {
                    float maxKeyValue = this.GetMaxKeyValue(this.maxCurve.animationCurveValue.keys);
                    this.scalar.floatValue *= maxKeyValue;
                    break;
                }
            }
            this.SetCurveConstant(this.maxCurve, 1f);
        }

        private bool IsCurveConstant(Keyframe[] keyFrames, out float constantValue)
        {
            if (keyFrames.Length == 0)
            {
                constantValue = 0f;
                return false;
            }
            constantValue = keyFrames[0].value;
            for (int i = 1; i < keyFrames.Length; i++)
            {
                if (Mathf.Abs((float) (constantValue - keyFrames[i].value)) > 1E-05f)
                {
                    return false;
                }
            }
            return true;
        }

        public bool OnCurveAreaMouseDown(int button, Rect drawRect, Rect curveRanges)
        {
            if (button == 0)
            {
                this.ToggleCurveInEditor();
                return true;
            }
            if (button == 1)
            {
                AnimationCurveContextMenu.Show(drawRect, this.maxCurve, this.GetMinCurve(), this.scalar, curveRanges, this.m_Module.GetParticleSystemCurveEditor());
                return true;
            }
            return false;
        }

        public void RemoveCurveFromEditor()
        {
            ParticleSystemCurveEditor particleSystemCurveEditor = this.m_Module.GetParticleSystemCurveEditor();
            if (particleSystemCurveEditor.IsAdded(this.GetMinCurve(), this.maxCurve))
            {
                particleSystemCurveEditor.RemoveCurve(this.GetMinCurve(), this.maxCurve);
            }
        }

        public void SetAxisScalars(Vector2 axisScalars)
        {
            float num = (this.m_RemapValue != 0f) ? this.m_RemapValue : 1f;
            this.scalar.floatValue = axisScalars.y / num;
        }

        private void SetCurveConstant(SerializedProperty curve, float value)
        {
            Keyframe[] keys = new Keyframe[] { new Keyframe(0f, value) };
            curve.animationCurveValue = new AnimationCurve(keys);
        }

        private void SetCurveRequirements()
        {
            this.scalar.floatValue = Mathf.Abs(this.scalar.floatValue);
            if (this.scalar.floatValue == 0f)
            {
                this.scalar.floatValue = this.m_DefaultCurveScalar;
            }
        }

        private void SetMinMaxState(MinMaxCurveState newState)
        {
            if (newState != this.state)
            {
                MinMaxCurveState oldState = this.state;
                ParticleSystemCurveEditor particleSystemCurveEditor = this.m_Module.GetParticleSystemCurveEditor();
                if (particleSystemCurveEditor.IsAdded(this.GetMinCurve(), this.maxCurve))
                {
                    particleSystemCurveEditor.RemoveCurve(this.GetMinCurve(), this.maxCurve);
                }
                switch (newState)
                {
                    case MinMaxCurveState.k_Scalar:
                        this.InitSingleScalar(oldState);
                        break;

                    case MinMaxCurveState.k_Curve:
                        this.InitSingleCurve(oldState);
                        break;

                    case MinMaxCurveState.k_TwoCurves:
                        this.InitDoubleCurves(oldState);
                        break;

                    case MinMaxCurveState.k_TwoScalars:
                        this.InitDoubleScalars(oldState);
                        break;
                }
                this.minMaxState.intValue = (int) newState;
                switch (newState)
                {
                    case MinMaxCurveState.k_Scalar:
                    case MinMaxCurveState.k_TwoScalars:
                        break;

                    case MinMaxCurveState.k_Curve:
                    case MinMaxCurveState.k_TwoCurves:
                        particleSystemCurveEditor.AddCurve(this.CreateCurveData(particleSystemCurveEditor.GetAvailableColor()));
                        break;

                    default:
                        Debug.LogError("Unhandled enum value");
                        break;
                }
                AnimationCurvePreviewCache.ClearCache();
            }
        }

        private void SetNormalizedConstant(SerializedProperty curve, float totalValue)
        {
            float num = Mathf.Max(this.scalar.floatValue, 0.0001f);
            float num2 = totalValue / num;
            this.SetCurveConstant(curve, num2);
        }

        public void SetScalarAndNormalizedConstants(float newScalar, float totalMin, float totalMax)
        {
            this.scalar.floatValue = newScalar;
            this.SetNormalizedConstant(this.minCurve, totalMin);
            this.SetNormalizedConstant(this.maxCurve, totalMax);
        }

        public bool SupportsProcedural()
        {
            bool flag = AnimationUtility.CurveSupportsProcedural(this.maxCurve.animationCurveValue);
            if ((this.state != MinMaxCurveState.k_TwoCurves) && (this.state != MinMaxCurveState.k_TwoScalars))
            {
                return flag;
            }
            return (flag && AnimationUtility.CurveSupportsProcedural(this.minCurve.animationCurveValue));
        }

        public void ToggleCurveInEditor()
        {
            ParticleSystemCurveEditor particleSystemCurveEditor = this.m_Module.GetParticleSystemCurveEditor();
            if (particleSystemCurveEditor.IsAdded(this.GetMinCurve(), this.maxCurve))
            {
                particleSystemCurveEditor.RemoveCurve(this.GetMinCurve(), this.maxCurve);
            }
            else
            {
                particleSystemCurveEditor.AddCurve(this.CreateCurveData(particleSystemCurveEditor.GetAvailableColor()));
            }
        }

        public float maxConstant
        {
            get
            {
                return (this.maxCurveFirstKeyValue.floatValue * this.scalar.floatValue);
            }
            set
            {
                value = this.ClampValueToMaxAllowed(value);
                if (!this.signedRange)
                {
                    value = Mathf.Max(value, 0f);
                }
                float minConstant = this.minConstant;
                float num2 = Mathf.Abs(minConstant);
                float num3 = Mathf.Abs(value);
                float newScalar = (num3 <= num2) ? num2 : num3;
                if (newScalar != this.scalar.floatValue)
                {
                    this.SetScalarAndNormalizedConstants(newScalar, minConstant, value);
                }
                else
                {
                    this.SetNormalizedConstant(this.maxCurve, value);
                }
            }
        }

        public float minConstant
        {
            get
            {
                return (this.minCurveFirstKeyValue.floatValue * this.scalar.floatValue);
            }
            set
            {
                value = this.ClampValueToMaxAllowed(value);
                if (!this.signedRange)
                {
                    value = Mathf.Max(value, 0f);
                }
                float maxConstant = this.maxConstant;
                float num2 = Mathf.Abs(value);
                float num3 = Mathf.Abs(maxConstant);
                float newScalar = (num3 <= num2) ? num2 : num3;
                if (newScalar != this.scalar.floatValue)
                {
                    this.SetScalarAndNormalizedConstants(newScalar, value, maxConstant);
                }
                else
                {
                    this.SetNormalizedConstant(this.minCurve, value);
                }
            }
        }

        public bool signedRange
        {
            get
            {
                return this.m_SignedRange;
            }
        }

        public MinMaxCurveState state
        {
            get
            {
                return (MinMaxCurveState) this.minMaxState.intValue;
            }
            set
            {
                this.SetMinMaxState(value);
            }
        }
    }
}

