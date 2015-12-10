namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public sealed class MaterialProperty
    {
        private Object[] m_Targets;
        private ApplyPropertyCallback m_ApplyPropertyCallback;
        private string m_Name;
        private string m_DisplayName;
        private object m_Value;
        private Vector4 m_TextureScaleAndOffset;
        private Vector2 m_RangeLimits;
        private PropType m_Type;
        private PropFlags m_Flags;
        private TexDim m_TextureDimension;
        private int m_MixedValueMask;
        public Object[] targets
        {
            get
            {
                return this.m_Targets;
            }
        }
        public PropType type
        {
            get
            {
                return this.m_Type;
            }
        }
        public string name
        {
            get
            {
                return this.m_Name;
            }
        }
        public string displayName
        {
            get
            {
                return this.m_DisplayName;
            }
        }
        public PropFlags flags
        {
            get
            {
                return this.m_Flags;
            }
        }
        public TexDim textureDimension
        {
            get
            {
                return this.m_TextureDimension;
            }
        }
        public Vector2 rangeLimits
        {
            get
            {
                return this.m_RangeLimits;
            }
        }
        public bool hasMixedValue
        {
            get
            {
                return ((this.m_MixedValueMask & 1) != 0);
            }
        }
        public ApplyPropertyCallback applyPropertyCallback
        {
            get
            {
                return this.m_ApplyPropertyCallback;
            }
            set
            {
                this.m_ApplyPropertyCallback = value;
            }
        }
        internal int mixedValueMask
        {
            get
            {
                return this.m_MixedValueMask;
            }
        }
        public void ReadFromMaterialPropertyBlock(MaterialPropertyBlock block)
        {
            ShaderUtil.ApplyMaterialPropertyBlockToMaterialProperty(block, this);
        }

        public void WriteToMaterialPropertyBlock(MaterialPropertyBlock materialblock, int changedPropertyMask)
        {
            ShaderUtil.ApplyMaterialPropertyToMaterialPropertyBlock(this, changedPropertyMask, materialblock);
        }

        public Color colorValue
        {
            get
            {
                if (this.m_Type == PropType.Color)
                {
                    return (Color) this.m_Value;
                }
                return Color.black;
            }
            set
            {
                if ((this.m_Type == PropType.Color) && (this.hasMixedValue || (value != ((Color) this.m_Value))))
                {
                    this.ApplyProperty(value);
                }
            }
        }
        public Vector4 vectorValue
        {
            get
            {
                if (this.m_Type == PropType.Vector)
                {
                    return (Vector4) this.m_Value;
                }
                return Vector4.zero;
            }
            set
            {
                if ((this.m_Type == PropType.Vector) && (this.hasMixedValue || (value != ((Vector4) this.m_Value))))
                {
                    this.ApplyProperty(value);
                }
            }
        }
        internal static bool IsTextureOffsetAndScaleChangedMask(int changedMask)
        {
            changedMask = changedMask >> 1;
            return (changedMask != 0);
        }

        public float floatValue
        {
            get
            {
                if ((this.m_Type != PropType.Float) && (this.m_Type != PropType.Range))
                {
                    return 0f;
                }
                return (float) this.m_Value;
            }
            set
            {
                if (((this.m_Type == PropType.Float) || (this.m_Type == PropType.Range)) && (this.hasMixedValue || (value != ((float) this.m_Value))))
                {
                    this.ApplyProperty(value);
                }
            }
        }
        public Texture textureValue
        {
            get
            {
                if (this.m_Type == PropType.Texture)
                {
                    return (Texture) this.m_Value;
                }
                return null;
            }
            set
            {
                if ((this.m_Type == PropType.Texture) && (this.hasMixedValue || (value != ((Texture) this.m_Value))))
                {
                    this.m_MixedValueMask &= -2;
                    object previousValue = this.m_Value;
                    this.m_Value = value;
                    this.ApplyProperty(previousValue, 1);
                }
            }
        }
        public Vector4 textureScaleAndOffset
        {
            get
            {
                if (this.m_Type == PropType.Texture)
                {
                    return this.m_TextureScaleAndOffset;
                }
                return Vector4.zero;
            }
            set
            {
                if ((this.m_Type == PropType.Texture) && (this.hasMixedValue || (value != this.m_TextureScaleAndOffset)))
                {
                    this.m_MixedValueMask &= 1;
                    int changedPropertyMask = 0;
                    for (int i = 1; i < 5; i++)
                    {
                        changedPropertyMask |= ((int) 1) << i;
                    }
                    object textureScaleAndOffset = this.m_TextureScaleAndOffset;
                    this.m_TextureScaleAndOffset = value;
                    this.ApplyProperty(textureScaleAndOffset, changedPropertyMask);
                }
            }
        }
        private void ApplyProperty(object newValue)
        {
            this.m_MixedValueMask = 0;
            object previousValue = this.m_Value;
            this.m_Value = newValue;
            this.ApplyProperty(previousValue, 1);
        }

        private void ApplyProperty(object previousValue, int changedPropertyMask)
        {
            string name;
            if ((this.targets == null) || (this.targets.Length == 0))
            {
                throw new ArgumentException("No material targets provided");
            }
            Object[] targets = this.targets;
            if (targets.Length == 1)
            {
                name = targets[0].name;
            }
            else
            {
                object[] objArray1 = new object[] { targets.Length, " ", ObjectNames.NicifyVariableName(ObjectNames.GetClassName(targets[0])), "s" };
                name = string.Concat(objArray1);
            }
            bool flag = false;
            if (this.m_ApplyPropertyCallback != null)
            {
                flag = this.m_ApplyPropertyCallback(this, changedPropertyMask, previousValue);
            }
            if (!flag)
            {
                ShaderUtil.ApplyProperty(this, changedPropertyMask, "Modify " + this.displayName + " of " + name);
            }
        }
        public delegate bool ApplyPropertyCallback(MaterialProperty prop, int changeMask, object previousValue);

        [Flags]
        public enum PropFlags
        {
            HDR = 0x10,
            HideInInspector = 1,
            None = 0,
            Normal = 8,
            NoScaleOffset = 4,
            PerRendererData = 2
        }

        public enum PropType
        {
            Color,
            Vector,
            Float,
            Range,
            Texture
        }

        public enum TexDim
        {
            Any = 6,
            Cube = 4,
            None = 0,
            Tex2D = 2,
            Tex3D = 3,
            Unknown = -1
        }
    }
}

