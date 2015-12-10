namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.Animations;
    using UnityEngine;

    public class ModelImporter : AssetImporter
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern string CalculateBestFittingPreviewGameObject();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern AnimationClip GetPreviewAnimationClipForTake(string takeName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_humanDescription(out HumanDescription value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_humanDescription(ref HumanDescription value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void UpdateSkeletonPose(SkeletonBone[] skeletonBones, SerializedProperty serializedProperty);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void UpdateTransformMask(AvatarMask mask, SerializedProperty serializedProperty);

        public bool addCollider { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public ModelImporterAnimationCompression animationCompression { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float animationPositionError { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float animationRotationError { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float animationScaleError { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public ModelImporterAnimationType animationType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public WrapMode animationWrapMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool bakeIK { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public ModelImporterClipAnimation[] clipAnimations { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public ModelImporterClipAnimation[] defaultClipAnimations { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string[] extraExposedTransformPaths { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float fileScale { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public ModelImporterGenerateAnimations generateAnimations { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("Use importMaterials, materialName and materialSearch instead")]
        public ModelImporterGenerateMaterials generateMaterials { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool generateSecondaryUV { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float globalScale { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public HumanDescription humanDescription
        {
            get
            {
                HumanDescription description;
                this.INTERNAL_get_humanDescription(out description);
                return description;
            }
            set
            {
                this.INTERNAL_set_humanDescription(ref value);
            }
        }

        public ModelImporterHumanoidOversampling humanoidOversampling { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool importAnimation { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool importBlendShapes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public TakeInfo[] importedTakeInfos { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool importMaterials { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public ModelImporterNormals importNormals { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public ModelImporterTangents importTangents { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal bool isAssetOlderOr42 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isBakeIKSupported { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isFileScaleUsed { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isReadable { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool isTangentImportSupported { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isUseFileUnitsSupported { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public ModelImporterMaterialName materialName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public ModelImporterMaterialSearch materialSearch { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public ModelImporterMeshCompression meshCompression { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public string motionNodeName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("normalImportMode is deprecated. Use importNormals instead")]
        public ModelImporterTangentSpaceMode normalImportMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float normalSmoothingAngle { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool optimizeGameObjects { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool optimizeMesh { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("Use animationCompression instead", true)]
        private bool reduceKeyframes
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public string[] referencedClips { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool resampleCurves { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("use resampleCurves instead.")]
        public bool resampleRotations { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float secondaryUVAngleDistortion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float secondaryUVAreaDistortion { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float secondaryUVHardAngle { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float secondaryUVPackMargin { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Avatar sourceAvatar
        {
            get
            {
                return this.sourceAvatarInternal;
            }
            set
            {
                Avatar avatar = value;
                if (value != null)
                {
                    ModelImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(value)) as ModelImporter;
                    if (atPath != null)
                    {
                        this.humanDescription = atPath.humanDescription;
                    }
                    else
                    {
                        Debug.LogError("Avatar must be from a ModelImporter, otherwise use ModelImporter.humanDescription");
                        avatar = null;
                    }
                }
                this.sourceAvatarInternal = avatar;
            }
        }

        internal Avatar sourceAvatarInternal { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("splitAnimations has been deprecated please use clipAnimations instead.", true)]
        public bool splitAnimations
        {
            get
            {
                return (this.clipAnimations.Length != 0);
            }
            set
            {
            }
        }

        [Obsolete("Please use tangentImportMode instead")]
        public bool splitTangentsAcrossSeams
        {
            get
            {
                return (this.importTangents == ModelImporterTangents.CalculateLegacyWithSplitTangents);
            }
            set
            {
                if ((this.importTangents == ModelImporterTangents.CalculateLegacyWithSplitTangents) && !value)
                {
                    this.importTangents = ModelImporterTangents.CalculateLegacy;
                }
                else if ((this.importTangents == ModelImporterTangents.CalculateLegacy) && value)
                {
                    this.importTangents = ModelImporterTangents.CalculateLegacyWithSplitTangents;
                }
            }
        }

        public bool swapUVChannels { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("tangentImportMode is deprecated. Use importTangents instead")]
        public ModelImporterTangentSpaceMode tangentImportMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public string[] transformPaths { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool useFileUnits { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

