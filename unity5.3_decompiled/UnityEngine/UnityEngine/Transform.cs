namespace UnityEngine
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    public class Transform : Component, IEnumerable
    {
        protected Transform()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void DetachChildren();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Transform Find(string name);
        public Transform FindChild(string name)
        {
            return this.Find(name);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Transform GetChild(int index);
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("use Transform.childCount instead."), WrapperlessIcall]
        public extern int GetChildCount();
        public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        internal Vector3 GetLocalEulerAngles(RotationOrder order)
        {
            Vector3 vector;
            INTERNAL_CALL_GetLocalEulerAngles(this, order, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int GetSiblingIndex();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetLocalEulerAngles(Transform self, RotationOrder order, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_InverseTransformDirection(Transform self, ref Vector3 direction, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_InverseTransformPoint(Transform self, ref Vector3 position, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_InverseTransformVector(Transform self, ref Vector3 vector, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_LookAt(Transform self, ref Vector3 worldPosition, ref Vector3 worldUp);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_RotateAround(Transform self, ref Vector3 axis, float angle);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_RotateAroundInternal(Transform self, ref Vector3 axis, float angle);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_RotateAroundLocal(Transform self, ref Vector3 axis, float angle);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetLocalEulerAngles(Transform self, ref Vector3 euler, RotationOrder order);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_TransformDirection(Transform self, ref Vector3 direction, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_TransformPoint(Transform self, ref Vector3 position, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_TransformVector(Transform self, ref Vector3 vector, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_localEulerAngles(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_localPosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_localRotation(out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_localScale(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_localToWorldMatrix(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_lossyScale(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_position(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_rotation(out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_worldToLocalMatrix(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_localEulerAngles(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_localPosition(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_localRotation(ref Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_localScale(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_position(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_rotation(ref Quaternion value);
        public Vector3 InverseTransformDirection(Vector3 direction)
        {
            Vector3 vector;
            INTERNAL_CALL_InverseTransformDirection(this, ref direction, out vector);
            return vector;
        }

        public Vector3 InverseTransformDirection(float x, float y, float z)
        {
            return this.InverseTransformDirection(new Vector3(x, y, z));
        }

        public Vector3 InverseTransformPoint(Vector3 position)
        {
            Vector3 vector;
            INTERNAL_CALL_InverseTransformPoint(this, ref position, out vector);
            return vector;
        }

        public Vector3 InverseTransformPoint(float x, float y, float z)
        {
            return this.InverseTransformPoint(new Vector3(x, y, z));
        }

        public Vector3 InverseTransformVector(Vector3 vector)
        {
            Vector3 vector2;
            INTERNAL_CALL_InverseTransformVector(this, ref vector, out vector2);
            return vector2;
        }

        public Vector3 InverseTransformVector(float x, float y, float z)
        {
            return this.InverseTransformVector(new Vector3(x, y, z));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool IsChildOf(Transform parent);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern bool IsNonUniformScaleTransform();
        [ExcludeFromDocs]
        public void LookAt(Transform target)
        {
            Vector3 up = Vector3.up;
            this.LookAt(target, up);
        }

        [ExcludeFromDocs]
        public void LookAt(Vector3 worldPosition)
        {
            Vector3 up = Vector3.up;
            INTERNAL_CALL_LookAt(this, ref worldPosition, ref up);
        }

        public void LookAt(Transform target, [DefaultValue("Vector3.up")] Vector3 worldUp)
        {
            if (target != null)
            {
                this.LookAt(target.position, worldUp);
            }
        }

        public void LookAt(Vector3 worldPosition, [DefaultValue("Vector3.up")] Vector3 worldUp)
        {
            INTERNAL_CALL_LookAt(this, ref worldPosition, ref worldUp);
        }

        [ExcludeFromDocs]
        public void Rotate(Vector3 eulerAngles)
        {
            Space self = Space.Self;
            this.Rotate(eulerAngles, self);
        }

        [ExcludeFromDocs]
        public void Rotate(Vector3 axis, float angle)
        {
            Space self = Space.Self;
            this.Rotate(axis, angle, self);
        }

        public void Rotate(Vector3 eulerAngles, [DefaultValue("Space.Self")] Space relativeTo)
        {
            Quaternion quaternion = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
            if (relativeTo == Space.Self)
            {
                this.localRotation *= quaternion;
            }
            else
            {
                this.rotation *= (Quaternion.Inverse(this.rotation) * quaternion) * this.rotation;
            }
        }

        [ExcludeFromDocs]
        public void Rotate(float xAngle, float yAngle, float zAngle)
        {
            Space self = Space.Self;
            this.Rotate(xAngle, yAngle, zAngle, self);
        }

        public void Rotate(Vector3 axis, float angle, [DefaultValue("Space.Self")] Space relativeTo)
        {
            if (relativeTo == Space.Self)
            {
                this.RotateAroundInternal(base.transform.TransformDirection(axis), angle * 0.01745329f);
            }
            else
            {
                this.RotateAroundInternal(axis, angle * 0.01745329f);
            }
        }

        public void Rotate(float xAngle, float yAngle, float zAngle, [DefaultValue("Space.Self")] Space relativeTo)
        {
            this.Rotate(new Vector3(xAngle, yAngle, zAngle), relativeTo);
        }

        [Obsolete("use Transform.Rotate instead.")]
        public void RotateAround(Vector3 axis, float angle)
        {
            INTERNAL_CALL_RotateAround(this, ref axis, angle);
        }

        public void RotateAround(Vector3 point, Vector3 axis, float angle)
        {
            Vector3 position = this.position;
            Quaternion quaternion = Quaternion.AngleAxis(angle, axis);
            Vector3 vector2 = position - point;
            vector2 = (Vector3) (quaternion * vector2);
            position = point + vector2;
            this.position = position;
            this.RotateAroundInternal(axis, angle * 0.01745329f);
        }

        internal void RotateAroundInternal(Vector3 axis, float angle)
        {
            INTERNAL_CALL_RotateAroundInternal(this, ref axis, angle);
        }

        [Obsolete("use Transform.Rotate instead.")]
        public void RotateAroundLocal(Vector3 axis, float angle)
        {
            INTERNAL_CALL_RotateAroundLocal(this, ref axis, angle);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void SendTransformChangedScale();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetAsFirstSibling();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetAsLastSibling();
        internal void SetLocalEulerAngles(Vector3 euler, RotationOrder order)
        {
            INTERNAL_CALL_SetLocalEulerAngles(this, ref euler, order);
        }

        public void SetParent(Transform parent)
        {
            this.SetParent(parent, true);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetParent(Transform parent, bool worldPositionStays);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetSiblingIndex(int index);
        public Vector3 TransformDirection(Vector3 direction)
        {
            Vector3 vector;
            INTERNAL_CALL_TransformDirection(this, ref direction, out vector);
            return vector;
        }

        public Vector3 TransformDirection(float x, float y, float z)
        {
            return this.TransformDirection(new Vector3(x, y, z));
        }

        public Vector3 TransformPoint(Vector3 position)
        {
            Vector3 vector;
            INTERNAL_CALL_TransformPoint(this, ref position, out vector);
            return vector;
        }

        public Vector3 TransformPoint(float x, float y, float z)
        {
            return this.TransformPoint(new Vector3(x, y, z));
        }

        public Vector3 TransformVector(Vector3 vector)
        {
            Vector3 vector2;
            INTERNAL_CALL_TransformVector(this, ref vector, out vector2);
            return vector2;
        }

        public Vector3 TransformVector(float x, float y, float z)
        {
            return this.TransformVector(new Vector3(x, y, z));
        }

        [ExcludeFromDocs]
        public void Translate(Vector3 translation)
        {
            Space self = Space.Self;
            this.Translate(translation, self);
        }

        public void Translate(Vector3 translation, [DefaultValue("Space.Self")] Space relativeTo)
        {
            if (relativeTo == Space.World)
            {
                this.position += translation;
            }
            else
            {
                this.position += this.TransformDirection(translation);
            }
        }

        public void Translate(Vector3 translation, Transform relativeTo)
        {
            if (relativeTo != null)
            {
                this.position += relativeTo.TransformDirection(translation);
            }
            else
            {
                this.position += translation;
            }
        }

        [ExcludeFromDocs]
        public void Translate(float x, float y, float z)
        {
            Space self = Space.Self;
            this.Translate(x, y, z, self);
        }

        public void Translate(float x, float y, float z, [DefaultValue("Space.Self")] Space relativeTo)
        {
            this.Translate(new Vector3(x, y, z), relativeTo);
        }

        public void Translate(float x, float y, float z, Transform relativeTo)
        {
            this.Translate(new Vector3(x, y, z), relativeTo);
        }

        public int childCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Vector3 eulerAngles
        {
            get
            {
                return this.rotation.eulerAngles;
            }
            set
            {
                this.rotation = Quaternion.Euler(value);
            }
        }

        public Vector3 forward
        {
            get
            {
                return (Vector3) (this.rotation * Vector3.forward);
            }
            set
            {
                this.rotation = Quaternion.LookRotation(value);
            }
        }

        public bool hasChanged { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 localEulerAngles
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_localEulerAngles(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_localEulerAngles(ref value);
            }
        }

        public Vector3 localPosition
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_localPosition(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_localPosition(ref value);
            }
        }

        public Quaternion localRotation
        {
            get
            {
                Quaternion quaternion;
                this.INTERNAL_get_localRotation(out quaternion);
                return quaternion;
            }
            set
            {
                this.INTERNAL_set_localRotation(ref value);
            }
        }

        public Vector3 localScale
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_localScale(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_localScale(ref value);
            }
        }

        public Matrix4x4 localToWorldMatrix
        {
            get
            {
                Matrix4x4 matrixx;
                this.INTERNAL_get_localToWorldMatrix(out matrixx);
                return matrixx;
            }
        }

        public Vector3 lossyScale
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_lossyScale(out vector);
                return vector;
            }
        }

        public Transform parent
        {
            get
            {
                return this.parentInternal;
            }
            set
            {
                if (this is RectTransform)
                {
                    Debug.LogWarning("Parent of RectTransform is being set with parent property. Consider using the SetParent method instead, with the worldPositionStays argument set to false. This will retain local orientation and scale rather than world orientation and scale, which can prevent common UI scaling issues.", this);
                }
                this.parentInternal = value;
            }
        }

        internal Transform parentInternal { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 position
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_position(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_position(ref value);
            }
        }

        public Vector3 right
        {
            get
            {
                return (Vector3) (this.rotation * Vector3.right);
            }
            set
            {
                this.rotation = Quaternion.FromToRotation(Vector3.right, value);
            }
        }

        public Transform root { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Quaternion rotation
        {
            get
            {
                Quaternion quaternion;
                this.INTERNAL_get_rotation(out quaternion);
                return quaternion;
            }
            set
            {
                this.INTERNAL_set_rotation(ref value);
            }
        }

        internal RotationOrder rotationOrder { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 up
        {
            get
            {
                return (Vector3) (this.rotation * Vector3.up);
            }
            set
            {
                this.rotation = Quaternion.FromToRotation(Vector3.up, value);
            }
        }

        public Matrix4x4 worldToLocalMatrix
        {
            get
            {
                Matrix4x4 matrixx;
                this.INTERNAL_get_worldToLocalMatrix(out matrixx);
                return matrixx;
            }
        }

        private sealed class Enumerator : IEnumerator
        {
            private int currentIndex = -1;
            private Transform outer;

            internal Enumerator(Transform outer)
            {
                this.outer = outer;
            }

            public bool MoveNext()
            {
                int childCount = this.outer.childCount;
                return (++this.currentIndex < childCount);
            }

            public void Reset()
            {
                this.currentIndex = -1;
            }

            public object Current
            {
                get
                {
                    return this.outer.GetChild(this.currentIndex);
                }
            }
        }
    }
}

