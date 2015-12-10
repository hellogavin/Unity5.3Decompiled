namespace UnityEditor.Sprites
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class PackerJob
    {
        internal PackerJob()
        {
        }

        public void AddAtlas(string atlasName, AtlasSettings settings)
        {
            this.AddAtlas_Internal(atlasName, ref settings);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void AddAtlas_Internal(string atlasName, ref AtlasSettings settings);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void AssignToAtlas(string atlasName, Sprite sprite, SpritePackingMode packingMode, SpritePackingRotation packingRotation);
    }
}

