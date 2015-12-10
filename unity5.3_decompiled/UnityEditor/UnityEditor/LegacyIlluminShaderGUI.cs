namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class LegacyIlluminShaderGUI : ShaderGUI
    {
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            base.OnGUI(materialEditor, props);
            materialEditor.LightmapEmissionProperty(0);
            foreach (Material material in materialEditor.targets)
            {
                material.globalIlluminationFlags &= ~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
            }
        }
    }
}

