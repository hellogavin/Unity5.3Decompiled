namespace UnityEditor.Utils
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class PerformanceChecks
    {
        private static readonly string[] kShadersWithMobileVariants = new string[] { "VertexLit", "Diffuse", "Bumped Diffuse", "Bumped Specular", "Particles/Additive", "Particles/VertexLit Blended", "Particles/Alpha Blended", "Particles/Multiply", "RenderFX/Skybox" };

        public static string CheckMaterial(Material mat, BuildTarget buildTarget)
        {
            <CheckMaterial>c__AnonStoreyB4 yb = new <CheckMaterial>c__AnonStoreyB4();
            if ((mat != null) && (mat.shader != null))
            {
                yb.shaderName = mat.shader.name;
                int lOD = ShaderUtil.GetLOD(mat.shader);
                bool flag = Array.Exists<string>(kShadersWithMobileVariants, new Predicate<string>(yb.<>m__21F));
                bool flag2 = IsMobileBuildTarget(buildTarget);
                if (mat.GetTag("PerformanceChecks", true).ToLower() != "false")
                {
                    if (flag)
                    {
                        if ((flag2 && mat.HasProperty("_Color")) && (mat.GetColor("_Color") == new Color(1f, 1f, 1f, 1f)))
                        {
                            object[] args = new object[] { "Mobile/" + yb.shaderName };
                            return FormattedTextContent("Shader is using white color which does nothing; Consider using {0} shader for performance.", args);
                        }
                        if (flag2 && yb.shaderName.StartsWith("Particles/"))
                        {
                            object[] objArray2 = new object[] { "Mobile/" + yb.shaderName };
                            return FormattedTextContent("Consider using {0} shader on this platform for performance.", objArray2);
                        }
                        if (((yb.shaderName == "RenderFX/Skybox") && mat.HasProperty("_Tint")) && (mat.GetColor("_Tint") == new Color(0.5f, 0.5f, 0.5f, 0.5f)))
                        {
                            object[] objArray3 = new object[] { "Mobile/Skybox" };
                            return FormattedTextContent("Skybox shader is using gray color which does nothing; Consider using {0} shader for performance.", objArray3);
                        }
                    }
                    if (((lOD >= 300) && flag2) && !yb.shaderName.StartsWith("Mobile/"))
                    {
                        return FormattedTextContent("Shader might be expensive on this platform. Consider switching to a simpler shader; look under Mobile shaders.", new object[0]);
                    }
                    if (yb.shaderName.Contains("VertexLit") && mat.HasProperty("_Emission"))
                    {
                        Color color = mat.GetColor("_Emission");
                        if (((color.r >= 0.5f) && (color.g >= 0.5f)) && (color.b >= 0.5f))
                        {
                            return FormattedTextContent("Looks like you're using VertexLit shader to simulate an unlit object (white emissive). Use one of Unlit shaders instead for performance.", new object[0]);
                        }
                    }
                    if (mat.HasProperty("_BumpMap") && (mat.GetTexture("_BumpMap") == null))
                    {
                        return FormattedTextContent("Normal mapped shader without a normal map. Consider using a non-normal mapped shader for performance.", new object[0]);
                    }
                }
            }
            return null;
        }

        private static string FormattedTextContent(string localeString, params object[] args)
        {
            return string.Format(EditorGUIUtility.TextContent(localeString).text, args);
        }

        private static bool IsMobileBuildTarget(BuildTarget target)
        {
            return (((target == BuildTarget.iOS) || (target == BuildTarget.Android)) || (target == BuildTarget.Tizen));
        }

        [CompilerGenerated]
        private sealed class <CheckMaterial>c__AnonStoreyB4
        {
            internal string shaderName;

            internal bool <>m__21F(string s)
            {
                return (s == this.shaderName);
            }
        }
    }
}

