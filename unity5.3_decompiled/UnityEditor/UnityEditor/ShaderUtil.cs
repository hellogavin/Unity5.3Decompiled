namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public sealed class ShaderUtil
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool AddNewShaderToCollection(Shader shader, ShaderVariantCollection collection);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void ApplyMaterialPropertyBlockToMaterialProperty(MaterialPropertyBlock propertyBlock, MaterialProperty materialProperty);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void ApplyMaterialPropertyToMaterialPropertyBlock(MaterialProperty materialProperty, int propertyMask, MaterialPropertyBlock propertyBlock);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void ApplyProperty(MaterialProperty prop, int propertyMask, string undoName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void CalculateFogStrippingFromCurrentScene();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void CalculateLightmapStrippingFromCurrentScene();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void ClearCurrentShaderVariantCollection();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Shader CreateShaderAsset(string source);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool DoesIgnoreProjector(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void FetchCachedErrors(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetAvailableShaderCompilerPlatforms();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetComboCount(Shader s, bool usedBySceneOnly);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetComputeShaderErrorCount(ComputeShader s);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern ShaderError[] GetComputeShaderErrors(ComputeShader s);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetCurrentShaderVariantCollectionShaderCount();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetCurrentShaderVariantCollectionVariantCount();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetDependency(Shader s, string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetLOD(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern MaterialProperty[] GetMaterialProperties(Object[] mats);
        internal static MaterialProperty GetMaterialProperty(Object[] mats, int propertyIndex)
        {
            return GetMaterialProperty_Index(mats, propertyIndex);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern MaterialProperty GetMaterialProperty(Object[] mats, string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern MaterialProperty GetMaterialProperty_Index(Object[] mats, int propertyIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetPropertyCount(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetPropertyDescription(Shader s, int propertyIdx);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetPropertyName(Shader s, int propertyIdx);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern ShaderPropertyType GetPropertyType(Shader s, int propertyIdx);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern float GetRangeLimits(Shader s, int propertyIdx, int defminmax);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetRenderQueue(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetShaderErrorCount(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern ShaderError[] GetShaderErrors(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string[] GetShaderPropertyAttributes(Shader s, string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void GetShaderVariantEntries(Shader shader, ShaderVariantCollection skipAlreadyInCollection, out int[] types, out string[] keywords);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern ShaderPropertyTexDim GetTexDim(Shader s, int propertyIdx);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetTextureBindingIndex(Shader s, int texturePropertyID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetTextureDimension(Texture t);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool HasFixedFunctionShaders(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool HasShaderSnippets(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool HasShadowCasterPass(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool HasSurfaceShaders(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool HasTangentChannel(Shader s);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_get_rawScissorRect(out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_get_rawViewportRect(out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_set_rawScissorRect(ref Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_set_rawViewportRect(ref Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsShaderPropertyHidden(Shader s, int propertyIdx);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void OpenCompiledComputeShader(ComputeShader shader, bool allVariantsAndPlatforms);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void OpenCompiledShader(Shader shader, int mode, int customPlatformsMask, bool includeAllVariants);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void OpenGeneratedFixedFunctionShader(Shader shader);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void OpenParsedSurfaceShader(Shader shader);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void OpenShaderCombinations(Shader shader, bool usedBySceneOnly);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void OpenShaderSnippets(Shader shader);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void RecreateGfxDevice();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void RecreateSkinnedMeshResources();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void ReloadAllShaders();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SaveCurrentShaderVariantCollection(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void UpdateShaderAsset(Shader shader, string source);

        internal static bool hardwareSupportsFullNPOT { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool hardwareSupportsRectRenderTexture { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static Rect rawScissorRect
        {
            get
            {
                Rect rect;
                INTERNAL_get_rawScissorRect(out rect);
                return rect;
            }
            set
            {
                INTERNAL_set_rawScissorRect(ref value);
            }
        }

        internal static Rect rawViewportRect
        {
            get
            {
                Rect rect;
                INTERNAL_get_rawViewportRect(out rect);
                return rect;
            }
            set
            {
                INTERNAL_set_rawViewportRect(ref value);
            }
        }

        internal enum ShaderCompilerPlatformType
        {
            OpenGL,
            D3D9,
            Xbox360,
            PS3,
            D3D11,
            OpenGLES20,
            OpenGLES20Desktop,
            Flash,
            D3D11_9x,
            OpenGLES30,
            PSVita,
            PS4,
            XboxOne,
            PSM,
            Metal,
            OpenGLCore,
            N3DS,
            WiiU,
            Count
        }

        public enum ShaderPropertyTexDim
        {
            TexDim2D = 2,
            TexDim3D = 3,
            TexDimAny = 5,
            TexDimCUBE = 4,
            TexDimDeprecated1D = 1,
            TexDimNone = 0,
            TexDimRECT = 5,
            TexDimUnknown = -1
        }

        public enum ShaderPropertyType
        {
            Color,
            Vector,
            Float,
            Range,
            TexEnv
        }
    }
}

