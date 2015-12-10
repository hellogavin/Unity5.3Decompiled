namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    public sealed class Rigidbody : Component
    {
        [ExcludeFromDocs]
        public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius)
        {
            ForceMode force = ForceMode.Force;
            float upwardsModifier = 0f;
            INTERNAL_CALL_AddExplosionForce(this, explosionForce, ref explosionPosition, explosionRadius, upwardsModifier, force);
        }

        [ExcludeFromDocs]
        public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier)
        {
            ForceMode force = ForceMode.Force;
            INTERNAL_CALL_AddExplosionForce(this, explosionForce, ref explosionPosition, explosionRadius, upwardsModifier, force);
        }

        public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, [DefaultValue("0.0F")] float upwardsModifier, [DefaultValue("ForceMode.Force")] ForceMode mode)
        {
            INTERNAL_CALL_AddExplosionForce(this, explosionForce, ref explosionPosition, explosionRadius, upwardsModifier, mode);
        }

        [ExcludeFromDocs]
        public void AddForce(Vector3 force)
        {
            ForceMode mode = ForceMode.Force;
            INTERNAL_CALL_AddForce(this, ref force, mode);
        }

        public void AddForce(Vector3 force, [DefaultValue("ForceMode.Force")] ForceMode mode)
        {
            INTERNAL_CALL_AddForce(this, ref force, mode);
        }

        [ExcludeFromDocs]
        public void AddForce(float x, float y, float z)
        {
            ForceMode force = ForceMode.Force;
            this.AddForce(x, y, z, force);
        }

        public void AddForce(float x, float y, float z, [DefaultValue("ForceMode.Force")] ForceMode mode)
        {
            this.AddForce(new Vector3(x, y, z), mode);
        }

        [ExcludeFromDocs]
        public void AddForceAtPosition(Vector3 force, Vector3 position)
        {
            ForceMode mode = ForceMode.Force;
            INTERNAL_CALL_AddForceAtPosition(this, ref force, ref position, mode);
        }

        public void AddForceAtPosition(Vector3 force, Vector3 position, [DefaultValue("ForceMode.Force")] ForceMode mode)
        {
            INTERNAL_CALL_AddForceAtPosition(this, ref force, ref position, mode);
        }

        [ExcludeFromDocs]
        public void AddRelativeForce(Vector3 force)
        {
            ForceMode mode = ForceMode.Force;
            INTERNAL_CALL_AddRelativeForce(this, ref force, mode);
        }

        public void AddRelativeForce(Vector3 force, [DefaultValue("ForceMode.Force")] ForceMode mode)
        {
            INTERNAL_CALL_AddRelativeForce(this, ref force, mode);
        }

        [ExcludeFromDocs]
        public void AddRelativeForce(float x, float y, float z)
        {
            ForceMode force = ForceMode.Force;
            this.AddRelativeForce(x, y, z, force);
        }

        public void AddRelativeForce(float x, float y, float z, [DefaultValue("ForceMode.Force")] ForceMode mode)
        {
            this.AddRelativeForce(new Vector3(x, y, z), mode);
        }

        [ExcludeFromDocs]
        public void AddRelativeTorque(Vector3 torque)
        {
            ForceMode force = ForceMode.Force;
            INTERNAL_CALL_AddRelativeTorque(this, ref torque, force);
        }

        public void AddRelativeTorque(Vector3 torque, [DefaultValue("ForceMode.Force")] ForceMode mode)
        {
            INTERNAL_CALL_AddRelativeTorque(this, ref torque, mode);
        }

        [ExcludeFromDocs]
        public void AddRelativeTorque(float x, float y, float z)
        {
            ForceMode force = ForceMode.Force;
            this.AddRelativeTorque(x, y, z, force);
        }

        public void AddRelativeTorque(float x, float y, float z, [DefaultValue("ForceMode.Force")] ForceMode mode)
        {
            this.AddRelativeTorque(new Vector3(x, y, z), mode);
        }

        [ExcludeFromDocs]
        public void AddTorque(Vector3 torque)
        {
            ForceMode force = ForceMode.Force;
            INTERNAL_CALL_AddTorque(this, ref torque, force);
        }

        public void AddTorque(Vector3 torque, [DefaultValue("ForceMode.Force")] ForceMode mode)
        {
            INTERNAL_CALL_AddTorque(this, ref torque, mode);
        }

        [ExcludeFromDocs]
        public void AddTorque(float x, float y, float z)
        {
            ForceMode force = ForceMode.Force;
            this.AddTorque(x, y, z, force);
        }

        public void AddTorque(float x, float y, float z, [DefaultValue("ForceMode.Force")] ForceMode mode)
        {
            this.AddTorque(new Vector3(x, y, z), mode);
        }

        public Vector3 ClosestPointOnBounds(Vector3 position)
        {
            Vector3 vector;
            INTERNAL_CALL_ClosestPointOnBounds(this, ref position, out vector);
            return vector;
        }

        public Vector3 GetPointVelocity(Vector3 worldPoint)
        {
            Vector3 vector;
            INTERNAL_CALL_GetPointVelocity(this, ref worldPoint, out vector);
            return vector;
        }

        public Vector3 GetRelativePointVelocity(Vector3 relativePoint)
        {
            Vector3 vector;
            INTERNAL_CALL_GetRelativePointVelocity(this, ref relativePoint, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_AddExplosionForce(Rigidbody self, float explosionForce, ref Vector3 explosionPosition, float explosionRadius, float upwardsModifier, ForceMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_AddForce(Rigidbody self, ref Vector3 force, ForceMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_AddForceAtPosition(Rigidbody self, ref Vector3 force, ref Vector3 position, ForceMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_AddRelativeForce(Rigidbody self, ref Vector3 force, ForceMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_AddRelativeTorque(Rigidbody self, ref Vector3 torque, ForceMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_AddTorque(Rigidbody self, ref Vector3 torque, ForceMode mode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_ClosestPointOnBounds(Rigidbody self, ref Vector3 position, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetPointVelocity(Rigidbody self, ref Vector3 worldPoint, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetRelativePointVelocity(Rigidbody self, ref Vector3 relativePoint, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_IsSleeping(Rigidbody self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_MovePosition(Rigidbody self, ref Vector3 position);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_MoveRotation(Rigidbody self, ref Quaternion rot);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_ResetCenterOfMass(Rigidbody self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_ResetInertiaTensor(Rigidbody self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetDensity(Rigidbody self, float density);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Sleep(Rigidbody self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_SweepTest(Rigidbody self, ref Vector3 direction, out RaycastHit hitInfo, float maxDistance, QueryTriggerInteraction queryTriggerInteraction);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern RaycastHit[] INTERNAL_CALL_SweepTestAll(Rigidbody self, ref Vector3 direction, float maxDistance, QueryTriggerInteraction queryTriggerInteraction);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_WakeUp(Rigidbody self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_angularVelocity(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_centerOfMass(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_inertiaTensor(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_inertiaTensorRotation(out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_position(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_rotation(out Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_velocity(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_worldCenterOfMass(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_angularVelocity(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_centerOfMass(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_inertiaTensor(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_inertiaTensorRotation(ref Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_position(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_rotation(ref Quaternion value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_velocity(ref Vector3 value);
        public bool IsSleeping()
        {
            return INTERNAL_CALL_IsSleeping(this);
        }

        public void MovePosition(Vector3 position)
        {
            INTERNAL_CALL_MovePosition(this, ref position);
        }

        public void MoveRotation(Quaternion rot)
        {
            INTERNAL_CALL_MoveRotation(this, ref rot);
        }

        public void ResetCenterOfMass()
        {
            INTERNAL_CALL_ResetCenterOfMass(this);
        }

        public void ResetInertiaTensor()
        {
            INTERNAL_CALL_ResetInertiaTensor(this);
        }

        public void SetDensity(float density)
        {
            INTERNAL_CALL_SetDensity(this, density);
        }

        [Obsolete("use Rigidbody.maxAngularVelocity instead.")]
        public void SetMaxAngularVelocity(float a)
        {
            this.maxAngularVelocity = a;
        }

        public void Sleep()
        {
            INTERNAL_CALL_Sleep(this);
        }

        [ExcludeFromDocs]
        public bool SweepTest(Vector3 direction, out RaycastHit hitInfo)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_SweepTest(this, ref direction, out hitInfo, positiveInfinity, useGlobal);
        }

        [ExcludeFromDocs]
        public bool SweepTest(Vector3 direction, out RaycastHit hitInfo, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return INTERNAL_CALL_SweepTest(this, ref direction, out hitInfo, maxDistance, useGlobal);
        }

        public bool SweepTest(Vector3 direction, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return INTERNAL_CALL_SweepTest(this, ref direction, out hitInfo, maxDistance, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public RaycastHit[] SweepTestAll(Vector3 direction)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_SweepTestAll(this, ref direction, positiveInfinity, useGlobal);
        }

        [ExcludeFromDocs]
        public RaycastHit[] SweepTestAll(Vector3 direction, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return INTERNAL_CALL_SweepTestAll(this, ref direction, maxDistance, useGlobal);
        }

        public RaycastHit[] SweepTestAll(Vector3 direction, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return INTERNAL_CALL_SweepTestAll(this, ref direction, maxDistance, queryTriggerInteraction);
        }

        public void WakeUp()
        {
            INTERNAL_CALL_WakeUp(this);
        }

        public float angularDrag { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 angularVelocity
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_angularVelocity(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_angularVelocity(ref value);
            }
        }

        public Vector3 centerOfMass
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_centerOfMass(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_centerOfMass(ref value);
            }
        }

        public CollisionDetectionMode collisionDetectionMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public RigidbodyConstraints constraints { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool detectCollisions { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float drag { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool freezeRotation { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 inertiaTensor
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_inertiaTensor(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_inertiaTensor(ref value);
            }
        }

        public Quaternion inertiaTensorRotation
        {
            get
            {
                Quaternion quaternion;
                this.INTERNAL_get_inertiaTensorRotation(out quaternion);
                return quaternion;
            }
            set
            {
                this.INTERNAL_set_inertiaTensorRotation(ref value);
            }
        }

        public RigidbodyInterpolation interpolation { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool isKinematic { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float mass { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float maxAngularVelocity { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float maxDepenetrationVelocity { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

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

        [Obsolete("The sleepAngularVelocity is no longer supported. Set Use sleepThreshold to specify energy.")]
        public float sleepAngularVelocity { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float sleepThreshold { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("The sleepVelocity is no longer supported. Use sleepThreshold. Note that sleepThreshold is energy but not velocity.")]
        public float sleepVelocity { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int solverIterationCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool useConeFriction { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool useGravity { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 velocity
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_velocity(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_velocity(ref value);
            }
        }

        public Vector3 worldCenterOfMass
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_worldCenterOfMass(out vector);
                return vector;
            }
        }
    }
}

