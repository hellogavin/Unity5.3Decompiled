namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Internal;

    public sealed class SubstanceImporter : AssetImporter
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool CanShaderPropertyHostProceduralOutput(string name, ProceduralOutputType substanceType);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void ClearPlatformTextureSettings(string materialName, string platform);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string CloneMaterial(ProceduralMaterial material);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void DestroyMaterial(ProceduralMaterial material);
        [ExcludeFromDocs]
        internal void ExportBitmaps(ProceduralMaterial material)
        {
            bool alphaRemap = false;
            this.ExportBitmaps(material, alphaRemap);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void ExportBitmaps(ProceduralMaterial material, [DefaultValue("false")] bool alphaRemap);
        public int GetAnimationUpdateRate(ProceduralMaterial material)
        {
            return this.GetMaterialInformation(material).animationUpdateRate;
        }

        public bool GetGenerateAllOutputs(ProceduralMaterial material)
        {
            return this.GetMaterialInformation(material).generateAllOutputs;
        }

        public bool GetGenerateMipMaps(ProceduralMaterial material)
        {
            return this.GetMaterialInformation(material).generateMipMaps;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int GetMaterialCount();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern ProceduralMaterialInformation GetMaterialInformation(ProceduralMaterial material);
        public Vector2 GetMaterialOffset(ProceduralMaterial material)
        {
            return this.GetMaterialInformation(material).offset;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern ProceduralMaterial[] GetMaterials();
        public Vector2 GetMaterialScale(ProceduralMaterial material)
        {
            return this.GetMaterialInformation(material).scale;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool GetPlatformTextureSettings(string materialName, string platform, out int maxTextureWidth, out int maxTextureHeight, out int textureFormat, out int loadBehavior);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string[] GetPrototypeNames();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern ProceduralOutputType GetTextureAlphaSource(ProceduralMaterial material, string textureName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string InstantiateMaterial(string prototypeName);
        internal static bool IsProceduralTextureSlot(Material material, Texture tex, string name)
        {
            return ((((material is ProceduralMaterial) && (tex is ProceduralTexture)) && CanShaderPropertyHostProceduralOutput(name, (tex as ProceduralTexture).GetProceduralOutputType())) && IsSubstanceParented(tex as ProceduralTexture, material as ProceduralMaterial));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool IsSubstanceParented(ProceduralTexture texture, ProceduralMaterial material);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void OnShaderModified(ProceduralMaterial material);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void OnTextureInformationsChanged(ProceduralTexture texture);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool RenameMaterial(ProceduralMaterial material, string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ResetMaterial(ProceduralMaterial material);
        public void SetAnimationUpdateRate(ProceduralMaterial material, int animation_update_rate)
        {
            ProceduralMaterialInformation materialInformation = this.GetMaterialInformation(material);
            materialInformation.animationUpdateRate = animation_update_rate;
            this.SetMaterialInformation(material, materialInformation);
        }

        public void SetGenerateAllOutputs(ProceduralMaterial material, bool generated)
        {
            ProceduralMaterialInformation materialInformation = this.GetMaterialInformation(material);
            materialInformation.generateAllOutputs = generated;
            this.SetMaterialInformation(material, materialInformation);
        }

        public void SetGenerateMipMaps(ProceduralMaterial material, bool mode)
        {
            ProceduralMaterialInformation materialInformation = this.GetMaterialInformation(material);
            materialInformation.generateMipMaps = mode;
            this.SetMaterialInformation(material, materialInformation);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetMaterialInformation(ProceduralMaterial material, ProceduralMaterialInformation information);
        public void SetMaterialOffset(ProceduralMaterial material, Vector2 offset)
        {
            ProceduralMaterialInformation materialInformation = this.GetMaterialInformation(material);
            materialInformation.offset = offset;
            this.SetMaterialInformation(material, materialInformation);
        }

        public void SetMaterialScale(ProceduralMaterial material, Vector2 scale)
        {
            ProceduralMaterialInformation materialInformation = this.GetMaterialInformation(material);
            materialInformation.scale = scale;
            this.SetMaterialInformation(material, materialInformation);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetPlatformTextureSettings(ProceduralMaterial material, string platform, int maxTextureWidth, int maxTextureHeight, int textureFormat, int loadBehavior);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetTextureAlphaSource(ProceduralMaterial material, string textureName, ProceduralOutputType alphaSource);
    }
}

