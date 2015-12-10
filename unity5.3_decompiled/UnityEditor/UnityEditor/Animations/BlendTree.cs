namespace UnityEditor.Animations
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    public sealed class BlendTree : Motion
    {
        public BlendTree()
        {
            Internal_Create(this);
        }

        public void AddChild(Motion motion)
        {
            this.AddChild(motion, Vector2.zero, 0f);
        }

        public void AddChild(Motion motion, float threshold)
        {
            this.AddChild(motion, Vector2.zero, threshold);
        }

        public void AddChild(Motion motion, Vector2 position)
        {
            this.AddChild(motion, position, 0f);
        }

        internal void AddChild(Motion motion, Vector2 position, float threshold)
        {
            Undo.RecordObject(this, "Added BlendTree Child");
            ChildMotion[] children = this.children;
            ChildMotion item = new ChildMotion {
                timeScale = 1f,
                motion = motion,
                position = position,
                threshold = threshold,
                directBlendParameter = "Blend"
            };
            ArrayUtility.Add<ChildMotion>(ref children, item);
            this.children = children;
        }

        public BlendTree CreateBlendTreeChild(float threshold)
        {
            return this.CreateBlendTreeChild(Vector2.zero, threshold);
        }

        public BlendTree CreateBlendTreeChild(Vector2 position)
        {
            return this.CreateBlendTreeChild(position, 0f);
        }

        internal BlendTree CreateBlendTreeChild(Vector2 position, float threshold)
        {
            Undo.RecordObject(this, "Created BlendTree Child");
            BlendTree objectToAdd = new BlendTree {
                name = "BlendTree",
                hideFlags = HideFlags.HideInHierarchy
            };
            if (AssetDatabase.GetAssetPath(this) != string.Empty)
            {
                AssetDatabase.AddObjectToAsset(objectToAdd, AssetDatabase.GetAssetPath(this));
            }
            this.AddChild(objectToAdd, position, threshold);
            return objectToAdd;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern AnimationClip[] GetAnimationClipsFlattened();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern int GetChildCount();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern Motion GetChildMotion(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern string GetDirectBlendTreeParameter(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern float GetInputBlendValue(string blendValueName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern string GetRecursiveBlendParameter(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern float GetRecursiveBlendParameterMax(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern float GetRecursiveBlendParameterMin(int index);
        internal bool HasChild(BlendTree childTree, bool recursive)
        {
            foreach (ChildMotion motion in this.children)
            {
                if (motion.motion == childTree)
                {
                    return true;
                }
                if ((recursive && (motion.motion is BlendTree)) && (motion.motion as BlendTree).HasChild(childTree, true))
                {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_Create(BlendTree mono);
        public void RemoveChild(int index)
        {
            Undo.RecordObject(this, "Remove Child");
            ChildMotion[] children = this.children;
            ArrayUtility.RemoveAt<ChildMotion>(ref children, index);
            this.children = children;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void SetDirectBlendTreeParameter(int index, string parameter);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void SetInputBlendValue(string blendValueName, float value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void SortChildren();

        public string blendParameter { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public string blendParameterY { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public BlendTreeType blendType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public ChildMotion[] children { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float maxThreshold { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float minThreshold { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal int recursiveBlendParameterCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool useAutomaticThresholds { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

