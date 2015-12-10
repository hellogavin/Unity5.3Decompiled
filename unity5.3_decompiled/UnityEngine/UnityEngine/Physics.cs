namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    public class Physics
    {
        public const int AllLayers = -1;
        public const int DefaultRaycastLayers = -5;
        public const int IgnoreRaycastLayer = 4;
        [Obsolete("Please use Physics.AllLayers instead. (UnityUpgradable) -> AllLayers", true)]
        public const int kAllLayers = -1;
        [Obsolete("Please use Physics.DefaultRaycastLayers instead. (UnityUpgradable) -> DefaultRaycastLayers", true)]
        public const int kDefaultRaycastLayers = -5;
        [Obsolete("Please use Physics.IgnoreRaycastLayer instead. (UnityUpgradable) -> IgnoreRaycastLayer", true)]
        public const int kIgnoreRaycastLayer = 4;

        [ExcludeFromDocs]
        public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            Quaternion identity = Quaternion.identity;
            return BoxCast(center, halfExtents, direction, identity, positiveInfinity, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return BoxCast(center, halfExtents, direction, orientation, positiveInfinity, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, out RaycastHit hitInfo)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            Quaternion identity = Quaternion.identity;
            return BoxCast(center, halfExtents, direction, out hitInfo, identity, positiveInfinity, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            return BoxCast(center, halfExtents, direction, orientation, maxDistance, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, out RaycastHit hitInfo, Quaternion orientation)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return BoxCast(center, halfExtents, direction, out hitInfo, orientation, positiveInfinity, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return BoxCast(center, halfExtents, direction, orientation, maxDistance, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, out RaycastHit hitInfo, Quaternion orientation, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            return BoxCast(center, halfExtents, direction, out hitInfo, orientation, maxDistance, layerMask, useGlobal);
        }

        public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, [DefaultValue("Quaternion.identity")] Quaternion orientation, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            RaycastHit hit;
            return Internal_BoxCast(center, halfExtents, orientation, direction, out hit, maxDistance, layerMask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, out RaycastHit hitInfo, Quaternion orientation, float maxDistance, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return BoxCast(center, halfExtents, direction, out hitInfo, orientation, maxDistance, layerMask, useGlobal);
        }

        public static bool BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, out RaycastHit hitInfo, [DefaultValue("Quaternion.identity")] Quaternion orientation, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return Internal_BoxCast(center, halfExtents, orientation, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static RaycastHit[] BoxCastAll(Vector3 center, Vector3 halfExtents, Vector3 direction)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layermask = -5;
            float positiveInfinity = float.PositiveInfinity;
            Quaternion identity = Quaternion.identity;
            return INTERNAL_CALL_BoxCastAll(ref center, ref halfExtents, ref direction, ref identity, positiveInfinity, layermask, useGlobal);
        }

        [ExcludeFromDocs]
        public static RaycastHit[] BoxCastAll(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layermask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_BoxCastAll(ref center, ref halfExtents, ref direction, ref orientation, positiveInfinity, layermask, useGlobal);
        }

        [ExcludeFromDocs]
        public static RaycastHit[] BoxCastAll(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layermask = -5;
            return INTERNAL_CALL_BoxCastAll(ref center, ref halfExtents, ref direction, ref orientation, maxDistance, layermask, useGlobal);
        }

        [ExcludeFromDocs]
        public static RaycastHit[] BoxCastAll(Vector3 center, Vector3 halfExtents, Vector3 direction, Quaternion orientation, float maxDistance, int layermask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return INTERNAL_CALL_BoxCastAll(ref center, ref halfExtents, ref direction, ref orientation, maxDistance, layermask, useGlobal);
        }

        public static RaycastHit[] BoxCastAll(Vector3 center, Vector3 halfExtents, Vector3 direction, [DefaultValue("Quaternion.identity")] Quaternion orientation, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layermask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return INTERNAL_CALL_BoxCastAll(ref center, ref halfExtents, ref direction, ref orientation, maxDistance, layermask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static int BoxCastNonAlloc(Vector3 center, Vector3 halfExtents, Vector3 direction, RaycastHit[] results)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layermask = -5;
            float positiveInfinity = float.PositiveInfinity;
            Quaternion identity = Quaternion.identity;
            return INTERNAL_CALL_BoxCastNonAlloc(ref center, ref halfExtents, ref direction, results, ref identity, positiveInfinity, layermask, useGlobal);
        }

        [ExcludeFromDocs]
        public static int BoxCastNonAlloc(Vector3 center, Vector3 halfExtents, Vector3 direction, RaycastHit[] results, Quaternion orientation)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layermask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_BoxCastNonAlloc(ref center, ref halfExtents, ref direction, results, ref orientation, positiveInfinity, layermask, useGlobal);
        }

        [ExcludeFromDocs]
        public static int BoxCastNonAlloc(Vector3 center, Vector3 halfExtents, Vector3 direction, RaycastHit[] results, Quaternion orientation, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layermask = -5;
            return INTERNAL_CALL_BoxCastNonAlloc(ref center, ref halfExtents, ref direction, results, ref orientation, maxDistance, layermask, useGlobal);
        }

        [ExcludeFromDocs]
        public static int BoxCastNonAlloc(Vector3 center, Vector3 halfExtents, Vector3 direction, RaycastHit[] results, Quaternion orientation, float maxDistance, int layermask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return INTERNAL_CALL_BoxCastNonAlloc(ref center, ref halfExtents, ref direction, results, ref orientation, maxDistance, layermask, useGlobal);
        }

        public static int BoxCastNonAlloc(Vector3 center, Vector3 halfExtents, Vector3 direction, RaycastHit[] results, [DefaultValue("Quaternion.identity")] Quaternion orientation, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layermask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return INTERNAL_CALL_BoxCastNonAlloc(ref center, ref halfExtents, ref direction, results, ref orientation, maxDistance, layermask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return CapsuleCast(point1, point2, radius, direction, positiveInfinity, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            return CapsuleCast(point1, point2, radius, direction, maxDistance, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return CapsuleCast(point1, point2, radius, direction, out hitInfo, positiveInfinity, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return CapsuleCast(point1, point2, radius, direction, maxDistance, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            return CapsuleCast(point1, point2, radius, direction, out hitInfo, maxDistance, layerMask, useGlobal);
        }

        public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            RaycastHit hit;
            return Internal_CapsuleCast(point1, point2, radius, direction, out hit, maxDistance, layerMask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return CapsuleCast(point1, point2, radius, direction, out hitInfo, maxDistance, layerMask, useGlobal);
        }

        public static bool CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return Internal_CapsuleCast(point1, point2, radius, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layermask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_CapsuleCastAll(ref point1, ref point2, radius, ref direction, positiveInfinity, layermask, useGlobal);
        }

        [ExcludeFromDocs]
        public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layermask = -5;
            return INTERNAL_CALL_CapsuleCastAll(ref point1, ref point2, radius, ref direction, maxDistance, layermask, useGlobal);
        }

        [ExcludeFromDocs]
        public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, float maxDistance, int layermask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return INTERNAL_CALL_CapsuleCastAll(ref point1, ref point2, radius, ref direction, maxDistance, layermask, useGlobal);
        }

        public static RaycastHit[] CapsuleCastAll(Vector3 point1, Vector3 point2, float radius, Vector3 direction, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layermask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return INTERNAL_CALL_CapsuleCastAll(ref point1, ref point2, radius, ref direction, maxDistance, layermask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static int CapsuleCastNonAlloc(Vector3 point1, Vector3 point2, float radius, Vector3 direction, RaycastHit[] results)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layermask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_CapsuleCastNonAlloc(ref point1, ref point2, radius, ref direction, results, positiveInfinity, layermask, useGlobal);
        }

        [ExcludeFromDocs]
        public static int CapsuleCastNonAlloc(Vector3 point1, Vector3 point2, float radius, Vector3 direction, RaycastHit[] results, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layermask = -5;
            return INTERNAL_CALL_CapsuleCastNonAlloc(ref point1, ref point2, radius, ref direction, results, maxDistance, layermask, useGlobal);
        }

        [ExcludeFromDocs]
        public static int CapsuleCastNonAlloc(Vector3 point1, Vector3 point2, float radius, Vector3 direction, RaycastHit[] results, float maxDistance, int layermask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return INTERNAL_CALL_CapsuleCastNonAlloc(ref point1, ref point2, radius, ref direction, results, maxDistance, layermask, useGlobal);
        }

        public static int CapsuleCastNonAlloc(Vector3 point1, Vector3 point2, float radius, Vector3 direction, RaycastHit[] results, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layermask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return INTERNAL_CALL_CapsuleCastNonAlloc(ref point1, ref point2, radius, ref direction, results, maxDistance, layermask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static bool CheckBox(Vector3 center, Vector3 halfExtents)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layermask = -5;
            Quaternion identity = Quaternion.identity;
            return INTERNAL_CALL_CheckBox(ref center, ref halfExtents, ref identity, layermask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool CheckBox(Vector3 center, Vector3 halfExtents, Quaternion orientation)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layermask = -5;
            return INTERNAL_CALL_CheckBox(ref center, ref halfExtents, ref orientation, layermask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool CheckBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, int layermask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return INTERNAL_CALL_CheckBox(ref center, ref halfExtents, ref orientation, layermask, useGlobal);
        }

        public static bool CheckBox(Vector3 center, Vector3 halfExtents, [DefaultValue("Quaternion.identity")] Quaternion orientation, [DefaultValue("DefaultRaycastLayers")] int layermask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return INTERNAL_CALL_CheckBox(ref center, ref halfExtents, ref orientation, layermask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static bool CheckCapsule(Vector3 start, Vector3 end, float radius)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layermask = -5;
            return INTERNAL_CALL_CheckCapsule(ref start, ref end, radius, layermask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layermask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return INTERNAL_CALL_CheckCapsule(ref start, ref end, radius, layermask, useGlobal);
        }

        public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, [DefaultValue("DefaultRaycastLayers")] int layermask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return INTERNAL_CALL_CheckCapsule(ref start, ref end, radius, layermask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static bool CheckSphere(Vector3 position, float radius)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            return INTERNAL_CALL_CheckSphere(ref position, radius, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool CheckSphere(Vector3 position, float radius, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return INTERNAL_CALL_CheckSphere(ref position, radius, layerMask, useGlobal);
        }

        public static bool CheckSphere(Vector3 position, float radius, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return INTERNAL_CALL_CheckSphere(ref position, radius, layerMask, queryTriggerInteraction);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetIgnoreLayerCollision(int layer1, int layer2);
        [ExcludeFromDocs]
        public static void IgnoreCollision(Collider collider1, Collider collider2)
        {
            bool ignore = true;
            IgnoreCollision(collider1, collider2, ignore);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void IgnoreCollision(Collider collider1, Collider collider2, [DefaultValue("true")] bool ignore);
        [ExcludeFromDocs]
        public static void IgnoreLayerCollision(int layer1, int layer2)
        {
            bool ignore = true;
            IgnoreLayerCollision(layer1, layer2, ignore);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void IgnoreLayerCollision(int layer1, int layer2, [DefaultValue("true")] bool ignore);
        private static bool Internal_BoxCast(Vector3 center, Vector3 halfExtents, Quaternion orientation, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction)
        {
            return INTERNAL_CALL_Internal_BoxCast(ref center, ref halfExtents, ref orientation, ref direction, out hitInfo, maxDistance, layermask, queryTriggerInteraction);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern RaycastHit[] INTERNAL_CALL_BoxCastAll(ref Vector3 center, ref Vector3 halfExtents, ref Vector3 direction, ref Quaternion orientation, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int INTERNAL_CALL_BoxCastNonAlloc(ref Vector3 center, ref Vector3 halfExtents, ref Vector3 direction, RaycastHit[] results, ref Quaternion orientation, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern RaycastHit[] INTERNAL_CALL_CapsuleCastAll(ref Vector3 point1, ref Vector3 point2, float radius, ref Vector3 direction, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int INTERNAL_CALL_CapsuleCastNonAlloc(ref Vector3 point1, ref Vector3 point2, float radius, ref Vector3 direction, RaycastHit[] results, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_CheckBox(ref Vector3 center, ref Vector3 halfExtents, ref Quaternion orientation, int layermask, QueryTriggerInteraction queryTriggerInteraction);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_CheckCapsule(ref Vector3 start, ref Vector3 end, float radius, int layermask, QueryTriggerInteraction queryTriggerInteraction);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_CheckSphere(ref Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_Internal_BoxCast(ref Vector3 center, ref Vector3 halfExtents, ref Quaternion orientation, ref Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_Internal_CapsuleCast(ref Vector3 point1, ref Vector3 point2, float radius, ref Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_Internal_Raycast(ref Vector3 origin, ref Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_Internal_RaycastTest(ref Vector3 origin, ref Vector3 direction, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Collider[] INTERNAL_CALL_OverlapBox(ref Vector3 center, ref Vector3 halfExtents, ref Quaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int INTERNAL_CALL_OverlapBoxNonAlloc(ref Vector3 center, ref Vector3 halfExtents, Collider[] results, ref Quaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Collider[] INTERNAL_CALL_OverlapSphere(ref Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int INTERNAL_CALL_OverlapSphereNonAlloc(ref Vector3 position, float radius, Collider[] results, int layerMask, QueryTriggerInteraction queryTriggerInteraction);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern RaycastHit[] INTERNAL_CALL_RaycastAll(ref Vector3 origin, ref Vector3 direction, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int INTERNAL_CALL_RaycastNonAlloc(ref Vector3 origin, ref Vector3 direction, RaycastHit[] results, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction);
        private static bool Internal_CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction)
        {
            return INTERNAL_CALL_Internal_CapsuleCast(ref point1, ref point2, radius, ref direction, out hitInfo, maxDistance, layermask, queryTriggerInteraction);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_get_gravity(out Vector3 value);
        private static bool Internal_Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction)
        {
            return INTERNAL_CALL_Internal_Raycast(ref origin, ref direction, out hitInfo, maxDistance, layermask, queryTriggerInteraction);
        }

        private static bool Internal_RaycastTest(Vector3 origin, Vector3 direction, float maxDistance, int layermask, QueryTriggerInteraction queryTriggerInteraction)
        {
            return INTERNAL_CALL_Internal_RaycastTest(ref origin, ref direction, maxDistance, layermask, queryTriggerInteraction);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_set_gravity(ref Vector3 value);
        [ExcludeFromDocs]
        public static bool Linecast(Vector3 start, Vector3 end)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            return Linecast(start, end, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool Linecast(Vector3 start, Vector3 end, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return Linecast(start, end, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            return Linecast(start, end, out hitInfo, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return Linecast(start, end, out hitInfo, layerMask, useGlobal);
        }

        public static bool Linecast(Vector3 start, Vector3 end, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            Vector3 direction = end - start;
            return Raycast(start, direction, direction.magnitude, layerMask, queryTriggerInteraction);
        }

        public static bool Linecast(Vector3 start, Vector3 end, out RaycastHit hitInfo, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            Vector3 direction = end - start;
            return Raycast(start, direction, out hitInfo, direction.magnitude, layerMask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static Collider[] OverlapBox(Vector3 center, Vector3 halfExtents)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -1;
            Quaternion identity = Quaternion.identity;
            return INTERNAL_CALL_OverlapBox(ref center, ref halfExtents, ref identity, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static Collider[] OverlapBox(Vector3 center, Vector3 halfExtents, Quaternion orientation)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -1;
            return INTERNAL_CALL_OverlapBox(ref center, ref halfExtents, ref orientation, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static Collider[] OverlapBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return INTERNAL_CALL_OverlapBox(ref center, ref halfExtents, ref orientation, layerMask, useGlobal);
        }

        public static Collider[] OverlapBox(Vector3 center, Vector3 halfExtents, [DefaultValue("Quaternion.identity")] Quaternion orientation, [DefaultValue("AllLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return INTERNAL_CALL_OverlapBox(ref center, ref halfExtents, ref orientation, layerMask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static int OverlapBoxNonAlloc(Vector3 center, Vector3 halfExtents, Collider[] results)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -1;
            Quaternion identity = Quaternion.identity;
            return INTERNAL_CALL_OverlapBoxNonAlloc(ref center, ref halfExtents, results, ref identity, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static int OverlapBoxNonAlloc(Vector3 center, Vector3 halfExtents, Collider[] results, Quaternion orientation)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -1;
            return INTERNAL_CALL_OverlapBoxNonAlloc(ref center, ref halfExtents, results, ref orientation, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static int OverlapBoxNonAlloc(Vector3 center, Vector3 halfExtents, Collider[] results, Quaternion orientation, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return INTERNAL_CALL_OverlapBoxNonAlloc(ref center, ref halfExtents, results, ref orientation, layerMask, useGlobal);
        }

        public static int OverlapBoxNonAlloc(Vector3 center, Vector3 halfExtents, Collider[] results, [DefaultValue("Quaternion.identity")] Quaternion orientation, [DefaultValue("AllLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return INTERNAL_CALL_OverlapBoxNonAlloc(ref center, ref halfExtents, results, ref orientation, layerMask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static Collider[] OverlapSphere(Vector3 position, float radius)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -1;
            return INTERNAL_CALL_OverlapSphere(ref position, radius, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static Collider[] OverlapSphere(Vector3 position, float radius, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return INTERNAL_CALL_OverlapSphere(ref position, radius, layerMask, useGlobal);
        }

        public static Collider[] OverlapSphere(Vector3 position, float radius, [DefaultValue("AllLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return INTERNAL_CALL_OverlapSphere(ref position, radius, layerMask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static int OverlapSphereNonAlloc(Vector3 position, float radius, Collider[] results)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -1;
            return INTERNAL_CALL_OverlapSphereNonAlloc(ref position, radius, results, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static int OverlapSphereNonAlloc(Vector3 position, float radius, Collider[] results, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return INTERNAL_CALL_OverlapSphereNonAlloc(ref position, radius, results, layerMask, useGlobal);
        }

        public static int OverlapSphereNonAlloc(Vector3 position, float radius, Collider[] results, [DefaultValue("AllLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return INTERNAL_CALL_OverlapSphereNonAlloc(ref position, radius, results, layerMask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static bool Raycast(Ray ray)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return Raycast(ray, positiveInfinity, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool Raycast(Ray ray, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            return Raycast(ray, maxDistance, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool Raycast(Ray ray, out RaycastHit hitInfo)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return Raycast(ray, out hitInfo, positiveInfinity, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool Raycast(Vector3 origin, Vector3 direction)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return Raycast(origin, direction, positiveInfinity, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool Raycast(Ray ray, float maxDistance, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return Raycast(ray, maxDistance, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            return Raycast(ray, out hitInfo, maxDistance, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool Raycast(Vector3 origin, Vector3 direction, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            return Raycast(origin, direction, maxDistance, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return Raycast(origin, direction, out hitInfo, positiveInfinity, layerMask, useGlobal);
        }

        public static bool Raycast(Ray ray, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return Raycast(ray.origin, ray.direction, maxDistance, layerMask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return Raycast(ray, out hitInfo, maxDistance, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool Raycast(Vector3 origin, Vector3 direction, float maxDistance, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return Raycast(origin, direction, maxDistance, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            return Raycast(origin, direction, out hitInfo, maxDistance, layerMask, useGlobal);
        }

        public static bool Raycast(Ray ray, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return Raycast(ray.origin, ray.direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return Internal_RaycastTest(origin, direction, maxDistance, layerMask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return Raycast(origin, direction, out hitInfo, maxDistance, layerMask, useGlobal);
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return Internal_Raycast(origin, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static RaycastHit[] RaycastAll(Ray ray)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return RaycastAll(ray, positiveInfinity, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static RaycastHit[] RaycastAll(Ray ray, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            return RaycastAll(ray, maxDistance, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layermask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_RaycastAll(ref origin, ref direction, positiveInfinity, layermask, useGlobal);
        }

        [ExcludeFromDocs]
        public static RaycastHit[] RaycastAll(Ray ray, float maxDistance, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return RaycastAll(ray, maxDistance, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layermask = -5;
            return INTERNAL_CALL_RaycastAll(ref origin, ref direction, maxDistance, layermask, useGlobal);
        }

        public static RaycastHit[] RaycastAll(Ray ray, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return RaycastAll(ray.origin, ray.direction, maxDistance, layerMask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, float maxDistance, int layermask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return INTERNAL_CALL_RaycastAll(ref origin, ref direction, maxDistance, layermask, useGlobal);
        }

        public static RaycastHit[] RaycastAll(Vector3 origin, Vector3 direction, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layermask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return INTERNAL_CALL_RaycastAll(ref origin, ref direction, maxDistance, layermask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static int RaycastNonAlloc(Ray ray, RaycastHit[] results)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return RaycastNonAlloc(ray, results, positiveInfinity, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static int RaycastNonAlloc(Ray ray, RaycastHit[] results, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            return RaycastNonAlloc(ray, results, maxDistance, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static int RaycastNonAlloc(Vector3 origin, Vector3 direction, RaycastHit[] results)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layermask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_RaycastNonAlloc(ref origin, ref direction, results, positiveInfinity, layermask, useGlobal);
        }

        [ExcludeFromDocs]
        public static int RaycastNonAlloc(Ray ray, RaycastHit[] results, float maxDistance, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return RaycastNonAlloc(ray, results, maxDistance, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static int RaycastNonAlloc(Vector3 origin, Vector3 direction, RaycastHit[] results, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layermask = -5;
            return INTERNAL_CALL_RaycastNonAlloc(ref origin, ref direction, results, maxDistance, layermask, useGlobal);
        }

        public static int RaycastNonAlloc(Ray ray, RaycastHit[] results, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return RaycastNonAlloc(ray.origin, ray.direction, results, maxDistance, layerMask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static int RaycastNonAlloc(Vector3 origin, Vector3 direction, RaycastHit[] results, float maxDistance, int layermask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return INTERNAL_CALL_RaycastNonAlloc(ref origin, ref direction, results, maxDistance, layermask, useGlobal);
        }

        public static int RaycastNonAlloc(Vector3 origin, Vector3 direction, RaycastHit[] results, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layermask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return INTERNAL_CALL_RaycastNonAlloc(ref origin, ref direction, results, maxDistance, layermask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static bool SphereCast(Ray ray, float radius)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return SphereCast(ray, radius, positiveInfinity, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool SphereCast(Ray ray, float radius, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            return SphereCast(ray, radius, maxDistance, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return SphereCast(ray, radius, out hitInfo, positiveInfinity, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool SphereCast(Ray ray, float radius, float maxDistance, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return SphereCast(ray, radius, maxDistance, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            return SphereCast(ray, radius, out hitInfo, maxDistance, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return SphereCast(origin, radius, direction, out hitInfo, positiveInfinity, layerMask, useGlobal);
        }

        public static bool SphereCast(Ray ray, float radius, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            RaycastHit hit;
            return Internal_CapsuleCast(ray.origin, ray.origin, radius, ray.direction, out hit, maxDistance, layerMask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, float maxDistance, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return SphereCast(ray, radius, out hitInfo, maxDistance, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            return SphereCast(origin, radius, direction, out hitInfo, maxDistance, layerMask, useGlobal);
        }

        public static bool SphereCast(Ray ray, float radius, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return Internal_CapsuleCast(ray.origin, ray.origin, radius, ray.direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return SphereCast(origin, radius, direction, out hitInfo, maxDistance, layerMask, useGlobal);
        }

        public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return Internal_CapsuleCast(origin, origin, radius, direction, out hitInfo, maxDistance, layerMask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static RaycastHit[] SphereCastAll(Ray ray, float radius)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return SphereCastAll(ray, radius, positiveInfinity, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static RaycastHit[] SphereCastAll(Ray ray, float radius, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            return SphereCastAll(ray, radius, maxDistance, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return SphereCastAll(origin, radius, direction, positiveInfinity, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static RaycastHit[] SphereCastAll(Ray ray, float radius, float maxDistance, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return SphereCastAll(ray, radius, maxDistance, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            return SphereCastAll(origin, radius, direction, maxDistance, layerMask, useGlobal);
        }

        public static RaycastHit[] SphereCastAll(Ray ray, float radius, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return CapsuleCastAll(ray.origin, ray.origin, radius, ray.direction, maxDistance, layerMask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, float maxDistance, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return SphereCastAll(origin, radius, direction, maxDistance, layerMask, useGlobal);
        }

        public static RaycastHit[] SphereCastAll(Vector3 origin, float radius, Vector3 direction, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return CapsuleCastAll(origin, origin, radius, direction, maxDistance, layerMask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static int SphereCastNonAlloc(Ray ray, float radius, RaycastHit[] results)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return SphereCastNonAlloc(ray, radius, results, positiveInfinity, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static int SphereCastNonAlloc(Ray ray, float radius, RaycastHit[] results, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            return SphereCastNonAlloc(ray, radius, results, maxDistance, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static int SphereCastNonAlloc(Vector3 origin, float radius, Vector3 direction, RaycastHit[] results)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return SphereCastNonAlloc(origin, radius, direction, results, positiveInfinity, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static int SphereCastNonAlloc(Ray ray, float radius, RaycastHit[] results, float maxDistance, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return SphereCastNonAlloc(ray, radius, results, maxDistance, layerMask, useGlobal);
        }

        [ExcludeFromDocs]
        public static int SphereCastNonAlloc(Vector3 origin, float radius, Vector3 direction, RaycastHit[] results, float maxDistance)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            int layerMask = -5;
            return SphereCastNonAlloc(origin, radius, direction, results, maxDistance, layerMask, useGlobal);
        }

        public static int SphereCastNonAlloc(Ray ray, float radius, RaycastHit[] results, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return CapsuleCastNonAlloc(ray.origin, ray.origin, radius, ray.direction, results, maxDistance, layerMask, queryTriggerInteraction);
        }

        [ExcludeFromDocs]
        public static int SphereCastNonAlloc(Vector3 origin, float radius, Vector3 direction, RaycastHit[] results, float maxDistance, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return SphereCastNonAlloc(origin, radius, direction, results, maxDistance, layerMask, useGlobal);
        }

        public static int SphereCastNonAlloc(Vector3 origin, float radius, Vector3 direction, RaycastHit[] results, [DefaultValue("Mathf.Infinity")] float maxDistance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return CapsuleCastNonAlloc(origin, origin, radius, direction, results, maxDistance, layerMask, queryTriggerInteraction);
        }

        public static float bounceThreshold { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("Please use bounceThreshold instead.")]
        public static float bounceTreshold
        {
            get
            {
                return bounceThreshold;
            }
            set
            {
                bounceThreshold = value;
            }
        }

        public static float defaultContactOffset { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static Vector3 gravity
        {
            get
            {
                Vector3 vector;
                INTERNAL_get_gravity(out vector);
                return vector;
            }
            set
            {
                INTERNAL_set_gravity(ref value);
            }
        }

        [Obsolete("use Rigidbody.maxAngularVelocity instead.", true)]
        public static float maxAngularVelocity { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("use Physics.defaultContactOffset or Collider.contactOffset instead.", true)]
        public static float minPenetrationForPenalty { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("penetrationPenaltyForce has no effect.")]
        public static float penetrationPenaltyForce { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool queriesHitTriggers { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("The sleepAngularVelocity is no longer supported. Use sleepThreshold. Note that sleepThreshold is energy but not velocity.")]
        public static float sleepAngularVelocity { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float sleepThreshold { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("The sleepVelocity is no longer supported. Use sleepThreshold. Note that sleepThreshold is energy but not velocity.")]
        public static float sleepVelocity { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int solverIterationCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

