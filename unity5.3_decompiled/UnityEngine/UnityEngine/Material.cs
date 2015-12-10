namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    public class Material : Object
    {
        [Obsolete("Creating materials from shader source string will be removed in the future. Use Shader assets instead.")]
        public Material(string contents)
        {
            Internal_CreateWithString(this, contents);
        }

        public Material(Material source)
        {
            Internal_CreateWithMaterial(this, source);
        }

        public Material(Shader shader)
        {
            Internal_CreateWithShader(this, shader);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void CopyPropertiesFromMaterial(Material mat);
        [Obsolete("Creating materials from shader source string will be removed in the future. Use Shader assets instead.")]
        public static Material Create(string scriptContents)
        {
            return new Material(scriptContents);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void DisableKeyword(string keyword);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void EnableKeyword(string keyword);
        public Color GetColor(int nameID)
        {
            Color color;
            INTERNAL_CALL_GetColor(this, nameID, out color);
            return color;
        }

        public Color GetColor(string propertyName)
        {
            return this.GetColor(Shader.PropertyToID(propertyName));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern float GetFloat(int nameID);
        public float GetFloat(string propertyName)
        {
            return this.GetFloat(Shader.PropertyToID(propertyName));
        }

        public int GetInt(int nameID)
        {
            return (int) this.GetFloat(nameID);
        }

        public int GetInt(string propertyName)
        {
            return (int) this.GetFloat(propertyName);
        }

        public Matrix4x4 GetMatrix(int nameID)
        {
            Matrix4x4 matrixx;
            INTERNAL_CALL_GetMatrix(this, nameID, out matrixx);
            return matrixx;
        }

        public Matrix4x4 GetMatrix(string propertyName)
        {
            return this.GetMatrix(Shader.PropertyToID(propertyName));
        }

        [ExcludeFromDocs]
        public string GetTag(string tag, bool searchFallbacks)
        {
            string defaultValue = string.Empty;
            return this.GetTag(tag, searchFallbacks, defaultValue);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetTag(string tag, bool searchFallbacks, [DefaultValue("\"\"")] string defaultValue);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Texture GetTexture(int nameID);
        public Texture GetTexture(string propertyName)
        {
            return this.GetTexture(Shader.PropertyToID(propertyName));
        }

        public Vector2 GetTextureOffset(string propertyName)
        {
            Vector4 vector;
            Internal_GetTextureScaleAndOffset(this, propertyName, out vector);
            return new Vector2(vector.z, vector.w);
        }

        public Vector2 GetTextureScale(string propertyName)
        {
            Vector4 vector;
            Internal_GetTextureScaleAndOffset(this, propertyName, out vector);
            return new Vector2(vector.x, vector.y);
        }

        public Vector4 GetVector(int nameID)
        {
            Color color = this.GetColor(nameID);
            return new Vector4(color.r, color.g, color.b, color.a);
        }

        public Vector4 GetVector(string propertyName)
        {
            Color color = this.GetColor(propertyName);
            return new Vector4(color.r, color.g, color.b, color.a);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool HasProperty(int nameID);
        public bool HasProperty(string propertyName)
        {
            return this.HasProperty(Shader.PropertyToID(propertyName));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetColor(Material self, int nameID, out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetMatrix(Material self, int nameID, out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetColor(Material self, int nameID, ref Color color);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetMatrix(Material self, int nameID, ref Matrix4x4 matrix);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetTextureOffset(Material self, string propertyName, ref Vector2 offset);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetTextureScale(Material self, string propertyName, ref Vector2 scale);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_CreateWithMaterial([Writable] Material mono, Material source);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_CreateWithShader([Writable] Material mono, Shader shader);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_CreateWithString([Writable] Material mono, string contents);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_GetTextureScaleAndOffset(Material mat, string name, out Vector4 output);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool IsKeywordEnabled(string keyword);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Lerp(Material start, Material end, float t);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetBuffer(string propertyName, ComputeBuffer buffer);
        public void SetColor(int nameID, Color color)
        {
            INTERNAL_CALL_SetColor(this, nameID, ref color);
        }

        public void SetColor(string propertyName, Color color)
        {
            this.SetColor(Shader.PropertyToID(propertyName), color);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetFloat(int nameID, float value);
        public void SetFloat(string propertyName, float value)
        {
            this.SetFloat(Shader.PropertyToID(propertyName), value);
        }

        public void SetInt(int nameID, int value)
        {
            this.SetFloat(nameID, (float) value);
        }

        public void SetInt(string propertyName, int value)
        {
            this.SetFloat(propertyName, (float) value);
        }

        public void SetMatrix(int nameID, Matrix4x4 matrix)
        {
            INTERNAL_CALL_SetMatrix(this, nameID, ref matrix);
        }

        public void SetMatrix(string propertyName, Matrix4x4 matrix)
        {
            this.SetMatrix(Shader.PropertyToID(propertyName), matrix);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetOverrideTag(string tag, string val);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool SetPass(int pass);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetTexture(int nameID, Texture texture);
        public void SetTexture(string propertyName, Texture texture)
        {
            this.SetTexture(Shader.PropertyToID(propertyName), texture);
        }

        public void SetTextureOffset(string propertyName, Vector2 offset)
        {
            INTERNAL_CALL_SetTextureOffset(this, propertyName, ref offset);
        }

        public void SetTextureScale(string propertyName, Vector2 scale)
        {
            INTERNAL_CALL_SetTextureScale(this, propertyName, ref scale);
        }

        public void SetVector(int nameID, Vector4 vector)
        {
            this.SetColor(nameID, new Color(vector.x, vector.y, vector.z, vector.w));
        }

        public void SetVector(string propertyName, Vector4 vector)
        {
            this.SetColor(propertyName, new Color(vector.x, vector.y, vector.z, vector.w));
        }

        public Color color
        {
            get
            {
                return this.GetColor("_Color");
            }
            set
            {
                this.SetColor("_Color", value);
            }
        }

        public MaterialGlobalIlluminationFlags globalIlluminationFlags { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Texture mainTexture
        {
            get
            {
                return this.GetTexture("_MainTex");
            }
            set
            {
                this.SetTexture("_MainTex", value);
            }
        }

        public Vector2 mainTextureOffset
        {
            get
            {
                return this.GetTextureOffset("_MainTex");
            }
            set
            {
                this.SetTextureOffset("_MainTex", value);
            }
        }

        public Vector2 mainTextureScale
        {
            get
            {
                return this.GetTextureScale("_MainTex");
            }
            set
            {
                this.SetTextureScale("_MainTex", value);
            }
        }

        public int passCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int renderQueue { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Shader shader { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public string[] shaderKeywords { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

