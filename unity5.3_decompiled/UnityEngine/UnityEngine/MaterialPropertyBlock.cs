namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class MaterialPropertyBlock
    {
        internal IntPtr m_Ptr;

        public MaterialPropertyBlock()
        {
            this.InitBlock();
        }

        [Obsolete("AddColor has been deprecated. Please use SetColor instead.")]
        public void AddColor(int nameID, Color value)
        {
            this.SetColor(nameID, value);
        }

        [Obsolete("AddColor has been deprecated. Please use SetColor instead.")]
        public void AddColor(string name, Color value)
        {
            this.AddColor(Shader.PropertyToID(name), value);
        }

        [Obsolete("AddFloat has been deprecated. Please use SetFloat instead.")]
        public void AddFloat(int nameID, float value)
        {
            this.SetFloat(nameID, value);
        }

        [Obsolete("AddFloat has been deprecated. Please use SetFloat instead.")]
        public void AddFloat(string name, float value)
        {
            this.AddFloat(Shader.PropertyToID(name), value);
        }

        [Obsolete("AddMatrix has been deprecated. Please use SetMatrix instead.")]
        public void AddMatrix(int nameID, Matrix4x4 value)
        {
            this.SetMatrix(nameID, value);
        }

        [Obsolete("AddMatrix has been deprecated. Please use SetMatrix instead.")]
        public void AddMatrix(string name, Matrix4x4 value)
        {
            this.AddMatrix(Shader.PropertyToID(name), value);
        }

        [Obsolete("AddTexture has been deprecated. Please use SetTexture instead.")]
        public void AddTexture(int nameID, Texture value)
        {
            this.SetTexture(nameID, value);
        }

        [Obsolete("AddTexture has been deprecated. Please use SetTexture instead.")]
        public void AddTexture(string name, Texture value)
        {
            this.AddTexture(Shader.PropertyToID(name), value);
        }

        [Obsolete("AddVector has been deprecated. Please use SetVector instead.")]
        public void AddVector(int nameID, Vector4 value)
        {
            this.SetVector(nameID, value);
        }

        [Obsolete("AddVector has been deprecated. Please use SetVector instead.")]
        public void AddVector(string name, Vector4 value)
        {
            this.AddVector(Shader.PropertyToID(name), value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Clear();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void DestroyBlock();
        ~MaterialPropertyBlock()
        {
            this.DestroyBlock();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern float GetFloat(int nameID);
        public float GetFloat(string name)
        {
            return this.GetFloat(Shader.PropertyToID(name));
        }

        public Matrix4x4 GetMatrix(int nameID)
        {
            Matrix4x4 matrixx;
            INTERNAL_CALL_GetMatrix(this, nameID, out matrixx);
            return matrixx;
        }

        public Matrix4x4 GetMatrix(string name)
        {
            return this.GetMatrix(Shader.PropertyToID(name));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Texture GetTexture(int nameID);
        public Texture GetTexture(string name)
        {
            return this.GetTexture(Shader.PropertyToID(name));
        }

        public Vector4 GetVector(int nameID)
        {
            Vector4 vector;
            INTERNAL_CALL_GetVector(this, nameID, out vector);
            return vector;
        }

        public Vector4 GetVector(string name)
        {
            return this.GetVector(Shader.PropertyToID(name));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void InitBlock();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetMatrix(MaterialPropertyBlock self, int nameID, out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetVector(MaterialPropertyBlock self, int nameID, out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetColor(MaterialPropertyBlock self, int nameID, ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetMatrix(MaterialPropertyBlock self, int nameID, ref Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetVector(MaterialPropertyBlock self, int nameID, ref Vector4 value);
        public void SetColor(int nameID, Color value)
        {
            INTERNAL_CALL_SetColor(this, nameID, ref value);
        }

        public void SetColor(string name, Color value)
        {
            this.SetColor(Shader.PropertyToID(name), value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetFloat(int nameID, float value);
        public void SetFloat(string name, float value)
        {
            this.SetFloat(Shader.PropertyToID(name), value);
        }

        public void SetMatrix(int nameID, Matrix4x4 value)
        {
            INTERNAL_CALL_SetMatrix(this, nameID, ref value);
        }

        public void SetMatrix(string name, Matrix4x4 value)
        {
            this.SetMatrix(Shader.PropertyToID(name), value);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetTexture(int nameID, Texture value);
        public void SetTexture(string name, Texture value)
        {
            this.SetTexture(Shader.PropertyToID(name), value);
        }

        public void SetVector(int nameID, Vector4 value)
        {
            INTERNAL_CALL_SetVector(this, nameID, ref value);
        }

        public void SetVector(string name, Vector4 value)
        {
            this.SetVector(Shader.PropertyToID(name), value);
        }

        public bool isEmpty { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

