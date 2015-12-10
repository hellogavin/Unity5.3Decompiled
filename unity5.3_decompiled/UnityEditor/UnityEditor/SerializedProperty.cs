namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Internal;

    [StructLayout(LayoutKind.Sequential)]
    public sealed class SerializedProperty
    {
        private IntPtr m_Property;
        internal SerializedObject m_SerializedObject;
        internal SerializedProperty()
        {
        }

        ~SerializedProperty()
        {
            this.Dispose();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Dispose();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool EqualContents(SerializedProperty x, SerializedProperty y);
        public SerializedObject serializedObject
        {
            get
            {
                return this.m_SerializedObject;
            }
        }
        public bool hasMultipleDifferentValues { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        internal int hasMultipleDifferentValuesBitwise { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void SetBitAtIndexForAllTargetsImmediate(int index, bool value);
        public string displayName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public string name { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public string type { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public string tooltip { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public int depth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public string propertyPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        internal int hashCodeForPropertyPathWithoutArrayIndex { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public bool editable { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public bool isAnimated { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public bool isExpanded { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public bool hasChildren { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public bool hasVisibleChildren { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public bool isInstantiatedPrefab { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public bool prefabOverride { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public SerializedPropertyType propertyType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public int intValue { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public long longValue { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public bool boolValue { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public float floatValue { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public double doubleValue { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public string stringValue { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public Color colorValue
        {
            get
            {
                Color color;
                this.INTERNAL_get_colorValue(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_colorValue(ref value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_colorValue(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_colorValue(ref Color value);
        public AnimationCurve animationCurveValue { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        internal Gradient gradientValue { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public Object objectReferenceValue { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public int objectReferenceInstanceIDValue { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        internal string objectReferenceStringValue { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern bool ValidateObjectReferenceValue(Object obj);
        internal string objectReferenceTypeString { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void AppendFoldoutPPtrValue(Object obj);
        internal string layerMaskStringValue { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public int enumValueIndex { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public string[] enumNames { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public string[] enumDisplayNames { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public Vector2 vector2Value
        {
            get
            {
                Vector2 vector;
                this.INTERNAL_get_vector2Value(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_vector2Value(ref value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_vector2Value(out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_vector2Value(ref Vector2 value);
        public Vector3 vector3Value
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_vector3Value(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_vector3Value(ref value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_vector3Value(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_vector3Value(ref Vector3 value);
        public Vector4 vector4Value
        {
            get
            {
                Vector4 vector;
                this.INTERNAL_get_vector4Value(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_vector4Value(ref value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_vector4Value(out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_vector4Value(ref Vector4 value);
        public Quaternion quaternionValue
        {
            get
            {
                Quaternion quaternion;
                this.INTERNAL_get_quaternionValue(out quaternion);
                return quaternion;
            }
            set
            {
                this.INTERNAL_set_quaternionValue(ref value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_quaternionValue(out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_quaternionValue(ref Quaternion value);
        public Rect rectValue
        {
            get
            {
                Rect rect;
                this.INTERNAL_get_rectValue(out rect);
                return rect;
            }
            set
            {
                this.INTERNAL_set_rectValue(ref value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_rectValue(out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_rectValue(ref Rect value);
        public Bounds boundsValue
        {
            get
            {
                Bounds bounds;
                this.INTERNAL_get_boundsValue(out bounds);
                return bounds;
            }
            set
            {
                this.INTERNAL_set_boundsValue(ref value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_boundsValue(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_boundsValue(ref Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool Next(bool enterChildren);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool NextVisible(bool enterChildren);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Reset();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int CountRemaining();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int CountInProperty();
        public SerializedProperty Copy()
        {
            SerializedProperty property = this.CopyInternal();
            property.m_SerializedObject = this.m_SerializedObject;
            return property;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern SerializedProperty CopyInternal();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool DuplicateCommand();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool DeleteCommand();
        public SerializedProperty FindPropertyRelative(string relativePropertyPath)
        {
            SerializedProperty property = this.Copy();
            if (property.FindPropertyRelativeInternal(relativePropertyPath))
            {
                return property;
            }
            return null;
        }

        [ExcludeFromDocs]
        public SerializedProperty GetEndProperty()
        {
            bool includeInvisible = false;
            return this.GetEndProperty(includeInvisible);
        }

        public SerializedProperty GetEndProperty([DefaultValue("false")] bool includeInvisible)
        {
            SerializedProperty property = this.Copy();
            if (includeInvisible)
            {
                property.Next(false);
                return property;
            }
            property.NextVisible(false);
            return property;
        }

        [DebuggerHidden]
        public IEnumerator GetEnumerator()
        {
            return new <GetEnumerator>c__Iterator1 { <>f__this = this };
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern bool FindPropertyInternal(string propertyPath);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern bool FindPropertyRelativeInternal(string propertyPath);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern int[] GetLayerMaskSelectedIndex();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern string[] GetLayerMaskNames();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void ToggleLayerMaskAtIndex(int index);
        public bool isArray { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        public int arraySize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
        public SerializedProperty GetArrayElementAtIndex(int index)
        {
            SerializedProperty property = this.Copy();
            if (property.GetArrayElementAtIndexInternal(index))
            {
                return property;
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool GetArrayElementAtIndexInternal(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InsertArrayElementAtIndex(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void DeleteArrayElementAtIndex(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ClearArray();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool MoveArrayElement(int srcIndex, int dstIndex);
        internal void SetToValueOfTarget(Object target)
        {
            SerializedProperty property = new SerializedObject(target).FindProperty(this.propertyPath);
            if (property == null)
            {
                Debug.LogError(target.name + " does not have the property " + this.propertyPath);
            }
            else
            {
                switch (this.propertyType)
                {
                    case SerializedPropertyType.Integer:
                        this.intValue = property.intValue;
                        break;

                    case SerializedPropertyType.Boolean:
                        this.boolValue = property.boolValue;
                        break;

                    case SerializedPropertyType.Float:
                        this.floatValue = property.floatValue;
                        break;

                    case SerializedPropertyType.String:
                        this.stringValue = property.stringValue;
                        break;

                    case SerializedPropertyType.Color:
                        this.colorValue = property.colorValue;
                        break;

                    case SerializedPropertyType.ObjectReference:
                        this.objectReferenceValue = property.objectReferenceValue;
                        break;

                    case SerializedPropertyType.LayerMask:
                        this.intValue = property.intValue;
                        break;

                    case SerializedPropertyType.Enum:
                        this.enumValueIndex = property.enumValueIndex;
                        break;

                    case SerializedPropertyType.Vector2:
                        this.vector2Value = property.vector2Value;
                        break;

                    case SerializedPropertyType.Vector3:
                        this.vector3Value = property.vector3Value;
                        break;

                    case SerializedPropertyType.Vector4:
                        this.vector4Value = property.vector4Value;
                        break;

                    case SerializedPropertyType.Rect:
                        this.rectValue = property.rectValue;
                        break;

                    case SerializedPropertyType.ArraySize:
                        this.intValue = property.intValue;
                        break;

                    case SerializedPropertyType.Character:
                        this.intValue = property.intValue;
                        break;

                    case SerializedPropertyType.AnimationCurve:
                        this.animationCurveValue = property.animationCurveValue;
                        break;

                    case SerializedPropertyType.Bounds:
                        this.boundsValue = property.boundsValue;
                        break;

                    case SerializedPropertyType.Gradient:
                        this.gradientValue = property.gradientValue;
                        break;
                }
            }
        }
        [CompilerGenerated]
        private sealed class <GetEnumerator>c__Iterator1 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal SerializedProperty <>f__this;
            internal SerializedProperty <end>__1;
            internal int <i>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        if (!this.<>f__this.isArray)
                        {
                            this.<end>__1 = this.<>f__this.GetEndProperty();
                            goto Label_00BB;
                        }
                        this.<i>__0 = 0;
                        break;

                    case 1:
                        this.<i>__0++;
                        break;

                    case 2:
                        goto Label_00BB;

                    default:
                        goto Label_00E9;
                }
                if (this.<i>__0 < this.<>f__this.arraySize)
                {
                    this.$current = this.<>f__this.GetArrayElementAtIndex(this.<i>__0);
                    this.$PC = 1;
                    goto Label_00EB;
                }
                goto Label_00E2;
            Label_00BB:
                while (this.<>f__this.NextVisible(true) && !SerializedProperty.EqualContents(this.<>f__this, this.<end>__1))
                {
                    this.$current = this.<>f__this;
                    this.$PC = 2;
                    goto Label_00EB;
                }
            Label_00E2:
                this.$PC = -1;
            Label_00E9:
                return false;
            Label_00EB:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

