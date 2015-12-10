namespace UnityEditor
{
    using System;
    using UnityEngine;
    using UnityEngine.Scripting;

    [RequiredByNativeCode]
    public sealed class AnimationClipSettings
    {
        public AnimationClip additiveReferencePoseClip;
        public float additiveReferencePoseTime;
        public float cycleOffset;
        public bool hasAdditiveReferencePose;
        public bool heightFromFeet;
        public bool keepOriginalOrientation;
        public bool keepOriginalPositionXZ;
        public bool keepOriginalPositionY;
        public float level;
        public bool loopBlend;
        public bool loopBlendOrientation;
        public bool loopBlendPositionXZ;
        public bool loopBlendPositionY;
        public bool loopTime;
        public bool mirror;
        public float orientationOffsetY;
        public float startTime;
        public float stopTime;
    }
}

