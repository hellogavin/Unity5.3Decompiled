namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class AnimationWindowCurve : IComparable<AnimationWindowCurve>
    {
        [CompilerGenerated]
        private static Comparison<AnimationWindowKeyframe> <>f__am$cache3;
        private EditorCurveBinding m_Binding;
        public List<AnimationWindowKeyframe> m_Keyframes;
        public Type m_ValueType;
        public const float timeEpsilon = 1E-05f;

        public AnimationWindowCurve(AnimationClip clip, EditorCurveBinding binding, Type valueType)
        {
            binding = RotationCurveInterpolation.RemapAnimationBindingForRotationCurves(binding, clip);
            this.m_Binding = binding;
            this.m_ValueType = valueType;
            this.LoadKeyframes(clip);
        }

        public void AddKeyframe(AnimationWindowKeyframe key, AnimationKeyTime keyTime)
        {
            this.RemoveKeyframe(keyTime);
            this.m_Keyframes.Add(key);
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = (a, b) => a.time.CompareTo(b.time);
            }
            this.m_Keyframes.Sort(<>f__am$cache3);
        }

        public int CompareTo(AnimationWindowCurve obj)
        {
            bool flag = this.path.Equals(obj.path);
            if (!flag && (this.depth != obj.depth))
            {
                return ((this.depth >= obj.depth) ? 1 : -1);
            }
            bool flag2 = ((this.type == typeof(Transform)) && (obj.type == typeof(Transform))) && flag;
            bool flag3 = ((this.type == typeof(Transform)) || (obj.type == typeof(Transform))) && flag;
            if (flag2)
            {
                string nicePropertyGroupDisplayName = AnimationWindowUtility.GetNicePropertyGroupDisplayName(typeof(Transform), AnimationWindowUtility.GetPropertyGroupName(this.propertyName));
                string str2 = AnimationWindowUtility.GetNicePropertyGroupDisplayName(typeof(Transform), AnimationWindowUtility.GetPropertyGroupName(obj.propertyName));
                if (nicePropertyGroupDisplayName.Contains("Position") && str2.Contains("Rotation"))
                {
                    return -1;
                }
                if (nicePropertyGroupDisplayName.Contains("Rotation") && str2.Contains("Position"))
                {
                    return 1;
                }
            }
            else
            {
                if (flag3)
                {
                    if (this.type == typeof(Transform))
                    {
                        return -1;
                    }
                    return 1;
                }
                if ((this.path == obj.path) && (obj.type == this.type))
                {
                    int componentIndex = AnimationWindowUtility.GetComponentIndex(obj.propertyName);
                    int num2 = AnimationWindowUtility.GetComponentIndex(this.propertyName);
                    if (((componentIndex != -1) && (num2 != -1)) && (this.propertyName.Substring(0, this.propertyName.Length - 2) == obj.propertyName.Substring(0, obj.propertyName.Length - 2)))
                    {
                        return (num2 - componentIndex);
                    }
                }
            }
            return (this.path + this.type + this.propertyName).CompareTo(obj.path + obj.type + obj.propertyName);
        }

        public AnimationWindowKeyframe FindKeyAtTime(AnimationKeyTime keyTime)
        {
            int keyframeIndex = this.GetKeyframeIndex(keyTime);
            if (keyframeIndex == -1)
            {
                return null;
            }
            return this.m_Keyframes[keyframeIndex];
        }

        public override int GetHashCode()
        {
            return this.m_Binding.GetHashCode();
        }

        public int GetKeyframeIndex(AnimationKeyTime time)
        {
            for (int i = 0; i < this.m_Keyframes.Count; i++)
            {
                if (time.ContainsTime(this.m_Keyframes[i].time))
                {
                    return i;
                }
            }
            return -1;
        }

        public bool HasKeyframe(AnimationKeyTime time)
        {
            return (this.GetKeyframeIndex(time) != -1);
        }

        public void LoadKeyframes(AnimationClip clip)
        {
            this.m_Keyframes = new List<AnimationWindowKeyframe>();
            if (!this.m_Binding.isPPtrCurve)
            {
                AnimationCurve editorCurve = AnimationUtility.GetEditorCurve(clip, this.binding);
                for (int i = 0; (editorCurve != null) && (i < editorCurve.length); i++)
                {
                    this.m_Keyframes.Add(new AnimationWindowKeyframe(this, editorCurve[i]));
                }
            }
            else
            {
                ObjectReferenceKeyframe[] objectReferenceCurve = AnimationUtility.GetObjectReferenceCurve(clip, this.binding);
                for (int j = 0; (objectReferenceCurve != null) && (j < objectReferenceCurve.Length); j++)
                {
                    this.m_Keyframes.Add(new AnimationWindowKeyframe(this, objectReferenceCurve[j]));
                }
            }
        }

        public void RemoveKeyframe(AnimationKeyTime time)
        {
            for (int i = this.m_Keyframes.Count - 1; i >= 0; i--)
            {
                if (time.ContainsTime(this.m_Keyframes[i].time))
                {
                    this.m_Keyframes.RemoveAt(i);
                }
            }
        }

        public void RemoveKeysAtRange(float startTime, float endTime)
        {
            for (int i = this.m_Keyframes.Count - 1; i >= 0; i--)
            {
                if (Mathf.Approximately(endTime, this.m_Keyframes[i].time) || ((this.m_Keyframes[i].time > startTime) && (this.m_Keyframes[i].time < endTime)))
                {
                    this.m_Keyframes.RemoveAt(i);
                }
            }
        }

        public AnimationCurve ToAnimationCurve()
        {
            int count = this.m_Keyframes.Count;
            AnimationCurve curve = new AnimationCurve();
            List<Keyframe> list = new List<Keyframe>();
            float minValue = float.MinValue;
            for (int i = 0; i < count; i++)
            {
                if (Mathf.Abs((float) (this.m_Keyframes[i].time - minValue)) > 1E-05f)
                {
                    Keyframe item = new Keyframe(this.m_Keyframes[i].time, (float) this.m_Keyframes[i].value, this.m_Keyframes[i].m_InTangent, this.m_Keyframes[i].m_OutTangent) {
                        tangentMode = this.m_Keyframes[i].m_TangentMode
                    };
                    list.Add(item);
                    minValue = this.m_Keyframes[i].time;
                }
            }
            curve.keys = list.ToArray();
            return curve;
        }

        public ObjectReferenceKeyframe[] ToObjectCurve()
        {
            int count = this.m_Keyframes.Count;
            List<ObjectReferenceKeyframe> list = new List<ObjectReferenceKeyframe>();
            float minValue = float.MinValue;
            for (int i = 0; i < count; i++)
            {
                if (Mathf.Abs((float) (this.m_Keyframes[i].time - minValue)) > 1E-05f)
                {
                    ObjectReferenceKeyframe item = new ObjectReferenceKeyframe {
                        time = this.m_Keyframes[i].time,
                        value = (Object) this.m_Keyframes[i].value
                    };
                    minValue = item.time;
                    list.Add(item);
                }
            }
            return list.ToArray();
        }

        public EditorCurveBinding binding
        {
            get
            {
                return this.m_Binding;
            }
        }

        public int depth
        {
            get
            {
                return ((this.path.Length <= 0) ? 0 : this.path.Split(new char[] { '/' }).Length);
            }
        }

        public bool isPPtrCurve
        {
            get
            {
                return this.m_Binding.isPPtrCurve;
            }
        }

        public int length
        {
            get
            {
                return this.m_Keyframes.Count;
            }
        }

        public string path
        {
            get
            {
                return this.m_Binding.path;
            }
        }

        public string propertyName
        {
            get
            {
                return this.m_Binding.propertyName;
            }
        }

        public Type type
        {
            get
            {
                return this.m_Binding.type;
            }
        }
    }
}

