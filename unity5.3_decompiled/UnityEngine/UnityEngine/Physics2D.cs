namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    public class Physics2D
    {
        public const int AllLayers = -1;
        public const int DefaultRaycastLayers = -5;
        public const int IgnoreRaycastLayer = 4;
        private static List<Rigidbody2D> m_LastDisabledRigidbody2D = new List<Rigidbody2D>();

        [ExcludeFromDocs]
        public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            float distance = float.PositiveInfinity;
            return BoxCast(origin, size, angle, direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return BoxCast(origin, size, angle, direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return BoxCast(origin, size, angle, direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return BoxCast(origin, size, angle, direction, distance, layerMask, minDepth, positiveInfinity);
        }

        public static RaycastHit2D BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            RaycastHit2D hitd;
            Internal_BoxCast(origin, size, angle, direction, distance, layerMask, minDepth, maxDepth, out hitd);
            return hitd;
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            float distance = float.PositiveInfinity;
            return INTERNAL_CALL_BoxCastAll(ref origin, ref size, angle, ref direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_BoxCastAll(ref origin, ref size, angle, ref direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_BoxCastAll(ref origin, ref size, angle, ref direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_BoxCastAll(ref origin, ref size, angle, ref direction, distance, layerMask, minDepth, positiveInfinity);
        }

        public static RaycastHit2D[] BoxCastAll(Vector2 origin, Vector2 size, float angle, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_BoxCastAll(ref origin, ref size, angle, ref direction, distance, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            float distance = float.PositiveInfinity;
            return INTERNAL_CALL_BoxCastNonAlloc(ref origin, ref size, angle, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_BoxCastNonAlloc(ref origin, ref size, angle, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_BoxCastNonAlloc(ref origin, ref size, angle, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_BoxCastNonAlloc(ref origin, ref size, angle, ref direction, results, distance, layerMask, minDepth, positiveInfinity);
        }

        public static int BoxCastNonAlloc(Vector2 origin, Vector2 size, float angle, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_BoxCastNonAlloc(ref origin, ref size, angle, ref direction, results, distance, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            float distance = float.PositiveInfinity;
            return CircleCast(origin, radius, direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return CircleCast(origin, radius, direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return CircleCast(origin, radius, direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return CircleCast(origin, radius, direction, distance, layerMask, minDepth, positiveInfinity);
        }

        public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            RaycastHit2D hitd;
            Internal_CircleCast(origin, radius, direction, distance, layerMask, minDepth, maxDepth, out hitd);
            return hitd;
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            float distance = float.PositiveInfinity;
            return INTERNAL_CALL_CircleCastAll(ref origin, radius, ref direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_CircleCastAll(ref origin, radius, ref direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_CircleCastAll(ref origin, radius, ref direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_CircleCastAll(ref origin, radius, ref direction, distance, layerMask, minDepth, positiveInfinity);
        }

        public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_CircleCastAll(ref origin, radius, ref direction, distance, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            float distance = float.PositiveInfinity;
            return INTERNAL_CALL_CircleCastNonAlloc(ref origin, radius, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_CircleCastNonAlloc(ref origin, radius, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_CircleCastNonAlloc(ref origin, radius, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_CircleCastNonAlloc(ref origin, radius, ref direction, results, distance, layerMask, minDepth, positiveInfinity);
        }

        public static int CircleCastNonAlloc(Vector2 origin, float radius, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_CircleCastNonAlloc(ref origin, radius, ref direction, results, distance, layerMask, minDepth, maxDepth);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetIgnoreCollision(Collider2D collider1, Collider2D collider2);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetIgnoreLayerCollision(int layer1, int layer2);
        [ExcludeFromDocs]
        public static RaycastHit2D GetRayIntersection(Ray ray)
        {
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return GetRayIntersection(ray, positiveInfinity, layerMask);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D GetRayIntersection(Ray ray, float distance)
        {
            int layerMask = -5;
            return GetRayIntersection(ray, distance, layerMask);
        }

        public static RaycastHit2D GetRayIntersection(Ray ray, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
        {
            RaycastHit2D hitd;
            Internal_GetRayIntersection(ray, distance, layerMask, out hitd);
            return hitd;
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] GetRayIntersectionAll(Ray ray)
        {
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_GetRayIntersectionAll(ref ray, positiveInfinity, layerMask);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] GetRayIntersectionAll(Ray ray, float distance)
        {
            int layerMask = -5;
            return INTERNAL_CALL_GetRayIntersectionAll(ref ray, distance, layerMask);
        }

        public static RaycastHit2D[] GetRayIntersectionAll(Ray ray, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
        {
            return INTERNAL_CALL_GetRayIntersectionAll(ref ray, distance, layerMask);
        }

        [ExcludeFromDocs]
        public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results)
        {
            int layerMask = -5;
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_GetRayIntersectionNonAlloc(ref ray, results, positiveInfinity, layerMask);
        }

        [ExcludeFromDocs]
        public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results, float distance)
        {
            int layerMask = -5;
            return INTERNAL_CALL_GetRayIntersectionNonAlloc(ref ray, results, distance, layerMask);
        }

        public static int GetRayIntersectionNonAlloc(Ray ray, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask)
        {
            return INTERNAL_CALL_GetRayIntersectionNonAlloc(ref ray, results, distance, layerMask);
        }

        [ExcludeFromDocs]
        public static void IgnoreCollision(Collider2D collider1, Collider2D collider2)
        {
            bool ignore = true;
            IgnoreCollision(collider1, collider2, ignore);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void IgnoreCollision(Collider2D collider1, Collider2D collider2, [DefaultValue("true")] bool ignore);
        [ExcludeFromDocs]
        public static void IgnoreLayerCollision(int layer1, int layer2)
        {
            bool ignore = true;
            IgnoreLayerCollision(layer1, layer2, ignore);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void IgnoreLayerCollision(int layer1, int layer2, [DefaultValue("true")] bool ignore);
        private static void Internal_BoxCast(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit)
        {
            INTERNAL_CALL_Internal_BoxCast(ref origin, ref size, angle, ref direction, distance, layerMask, minDepth, maxDepth, out raycastHit);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern RaycastHit2D[] INTERNAL_CALL_BoxCastAll(ref Vector2 origin, ref Vector2 size, float angle, ref Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int INTERNAL_CALL_BoxCastNonAlloc(ref Vector2 origin, ref Vector2 size, float angle, ref Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern RaycastHit2D[] INTERNAL_CALL_CircleCastAll(ref Vector2 origin, float radius, ref Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int INTERNAL_CALL_CircleCastNonAlloc(ref Vector2 origin, float radius, ref Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern RaycastHit2D[] INTERNAL_CALL_GetRayIntersectionAll(ref Ray ray, float distance, int layerMask);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int INTERNAL_CALL_GetRayIntersectionNonAlloc(ref Ray ray, RaycastHit2D[] results, float distance, int layerMask);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_BoxCast(ref Vector2 origin, ref Vector2 size, float angle, ref Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_CircleCast(ref Vector2 origin, float radius, ref Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_GetRayIntersection(ref Ray ray, float distance, int layerMask, out RaycastHit2D raycastHit);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_Linecast(ref Vector2 start, ref Vector2 end, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_Raycast(ref Vector2 origin, ref Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern RaycastHit2D[] INTERNAL_CALL_LinecastAll(ref Vector2 start, ref Vector2 end, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int INTERNAL_CALL_LinecastNonAlloc(ref Vector2 start, ref Vector2 end, RaycastHit2D[] results, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Collider2D INTERNAL_CALL_OverlapArea(ref Vector2 pointA, ref Vector2 pointB, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Collider2D[] INTERNAL_CALL_OverlapAreaAll(ref Vector2 pointA, ref Vector2 pointB, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int INTERNAL_CALL_OverlapAreaNonAlloc(ref Vector2 pointA, ref Vector2 pointB, Collider2D[] results, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Collider2D INTERNAL_CALL_OverlapCircle(ref Vector2 point, float radius, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Collider2D[] INTERNAL_CALL_OverlapCircleAll(ref Vector2 point, float radius, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int INTERNAL_CALL_OverlapCircleNonAlloc(ref Vector2 point, float radius, Collider2D[] results, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Collider2D INTERNAL_CALL_OverlapPoint(ref Vector2 point, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Collider2D[] INTERNAL_CALL_OverlapPointAll(ref Vector2 point, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int INTERNAL_CALL_OverlapPointNonAlloc(ref Vector2 point, Collider2D[] results, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern RaycastHit2D[] INTERNAL_CALL_RaycastAll(ref Vector2 origin, ref Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern int INTERNAL_CALL_RaycastNonAlloc(ref Vector2 origin, ref Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth, float maxDepth);
        private static void Internal_CircleCast(Vector2 origin, float radius, Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit)
        {
            INTERNAL_CALL_Internal_CircleCast(ref origin, radius, ref direction, distance, layerMask, minDepth, maxDepth, out raycastHit);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_get_gravity(out Vector2 value);
        private static void Internal_GetRayIntersection(Ray ray, float distance, int layerMask, out RaycastHit2D raycastHit)
        {
            INTERNAL_CALL_Internal_GetRayIntersection(ref ray, distance, layerMask, out raycastHit);
        }

        private static void Internal_Linecast(Vector2 start, Vector2 end, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit)
        {
            INTERNAL_CALL_Internal_Linecast(ref start, ref end, layerMask, minDepth, maxDepth, out raycastHit);
        }

        private static void Internal_Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth, out RaycastHit2D raycastHit)
        {
            INTERNAL_CALL_Internal_Raycast(ref origin, ref direction, distance, layerMask, minDepth, maxDepth, out raycastHit);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_set_gravity(ref Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsTouching(Collider2D collider1, Collider2D collider2);
        [ExcludeFromDocs]
        public static bool IsTouchingLayers(Collider2D collider)
        {
            int layerMask = -1;
            return IsTouchingLayers(collider, layerMask);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsTouchingLayers(Collider2D collider, [DefaultValue("AllLayers")] int layerMask);
        [ExcludeFromDocs]
        public static RaycastHit2D Linecast(Vector2 start, Vector2 end)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return Linecast(start, end, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D Linecast(Vector2 start, Vector2 end, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return Linecast(start, end, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D Linecast(Vector2 start, Vector2 end, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return Linecast(start, end, layerMask, minDepth, positiveInfinity);
        }

        public static RaycastHit2D Linecast(Vector2 start, Vector2 end, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            RaycastHit2D hitd;
            Internal_Linecast(start, end, layerMask, minDepth, maxDepth, out hitd);
            return hitd;
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_LinecastAll(ref start, ref end, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_LinecastAll(ref start, ref end, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_LinecastAll(ref start, ref end, layerMask, minDepth, positiveInfinity);
        }

        public static RaycastHit2D[] LinecastAll(Vector2 start, Vector2 end, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_LinecastAll(ref start, ref end, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_LinecastNonAlloc(ref start, ref end, results, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_LinecastNonAlloc(ref start, ref end, results, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_LinecastNonAlloc(ref start, ref end, results, layerMask, minDepth, positiveInfinity);
        }

        public static int LinecastNonAlloc(Vector2 start, Vector2 end, RaycastHit2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_LinecastNonAlloc(ref start, ref end, results, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapArea(ref pointA, ref pointB, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapArea(ref pointA, ref pointB, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapArea(ref pointA, ref pointB, layerMask, minDepth, positiveInfinity);
        }

        public static Collider2D OverlapArea(Vector2 pointA, Vector2 pointB, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapArea(ref pointA, ref pointB, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapAreaAll(ref pointA, ref pointB, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapAreaAll(ref pointA, ref pointB, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapAreaAll(ref pointA, ref pointB, layerMask, minDepth, positiveInfinity);
        }

        public static Collider2D[] OverlapAreaAll(Vector2 pointA, Vector2 pointB, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapAreaAll(ref pointA, ref pointB, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapAreaNonAlloc(ref pointA, ref pointB, results, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapAreaNonAlloc(ref pointA, ref pointB, results, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapAreaNonAlloc(ref pointA, ref pointB, results, layerMask, minDepth, positiveInfinity);
        }

        public static int OverlapAreaNonAlloc(Vector2 pointA, Vector2 pointB, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapAreaNonAlloc(ref pointA, ref pointB, results, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapCircle(Vector2 point, float radius)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapCircle(ref point, radius, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapCircle(Vector2 point, float radius, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapCircle(ref point, radius, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapCircle(Vector2 point, float radius, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapCircle(ref point, radius, layerMask, minDepth, positiveInfinity);
        }

        public static Collider2D OverlapCircle(Vector2 point, float radius, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapCircle(ref point, radius, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapCircleAll(Vector2 point, float radius)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapCircleAll(ref point, radius, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapCircleAll(Vector2 point, float radius, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapCircleAll(ref point, radius, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapCircleAll(Vector2 point, float radius, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapCircleAll(ref point, radius, layerMask, minDepth, positiveInfinity);
        }

        public static Collider2D[] OverlapCircleAll(Vector2 point, float radius, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapCircleAll(ref point, radius, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapCircleNonAlloc(ref point, radius, results, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapCircleNonAlloc(ref point, radius, results, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapCircleNonAlloc(ref point, radius, results, layerMask, minDepth, positiveInfinity);
        }

        public static int OverlapCircleNonAlloc(Vector2 point, float radius, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapCircleNonAlloc(ref point, radius, results, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapPoint(Vector2 point)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapPoint(ref point, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapPoint(Vector2 point, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapPoint(ref point, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D OverlapPoint(Vector2 point, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapPoint(ref point, layerMask, minDepth, positiveInfinity);
        }

        public static Collider2D OverlapPoint(Vector2 point, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapPoint(ref point, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapPointAll(Vector2 point)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapPointAll(ref point, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapPointAll(Vector2 point, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapPointAll(ref point, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static Collider2D[] OverlapPointAll(Vector2 point, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapPointAll(ref point, layerMask, minDepth, positiveInfinity);
        }

        public static Collider2D[] OverlapPointAll(Vector2 point, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapPointAll(ref point, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_OverlapPointNonAlloc(ref point, results, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_OverlapPointNonAlloc(ref point, results, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_OverlapPointNonAlloc(ref point, results, layerMask, minDepth, positiveInfinity);
        }

        public static int OverlapPointNonAlloc(Vector2 point, Collider2D[] results, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_OverlapPointNonAlloc(ref point, results, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            float distance = float.PositiveInfinity;
            return Raycast(origin, direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return Raycast(origin, direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return Raycast(origin, direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return Raycast(origin, direction, distance, layerMask, minDepth, positiveInfinity);
        }

        public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            RaycastHit2D hitd;
            Internal_Raycast(origin, direction, distance, layerMask, minDepth, maxDepth, out hitd);
            return hitd;
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            float distance = float.PositiveInfinity;
            return INTERNAL_CALL_RaycastAll(ref origin, ref direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_RaycastAll(ref origin, ref direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_RaycastAll(ref origin, ref direction, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_RaycastAll(ref origin, ref direction, distance, layerMask, minDepth, positiveInfinity);
        }

        public static RaycastHit2D[] RaycastAll(Vector2 origin, Vector2 direction, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_RaycastAll(ref origin, ref direction, distance, layerMask, minDepth, maxDepth);
        }

        [ExcludeFromDocs]
        public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            float distance = float.PositiveInfinity;
            return INTERNAL_CALL_RaycastNonAlloc(ref origin, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            int layerMask = -5;
            return INTERNAL_CALL_RaycastNonAlloc(ref origin, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
        {
            float positiveInfinity = float.PositiveInfinity;
            float negativeInfinity = float.NegativeInfinity;
            return INTERNAL_CALL_RaycastNonAlloc(ref origin, ref direction, results, distance, layerMask, negativeInfinity, positiveInfinity);
        }

        [ExcludeFromDocs]
        public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
        {
            float positiveInfinity = float.PositiveInfinity;
            return INTERNAL_CALL_RaycastNonAlloc(ref origin, ref direction, results, distance, layerMask, minDepth, positiveInfinity);
        }

        public static int RaycastNonAlloc(Vector2 origin, Vector2 direction, RaycastHit2D[] results, [DefaultValue("Mathf.Infinity")] float distance, [DefaultValue("DefaultRaycastLayers")] int layerMask, [DefaultValue("-Mathf.Infinity")] float minDepth, [DefaultValue("Mathf.Infinity")] float maxDepth)
        {
            return INTERNAL_CALL_RaycastNonAlloc(ref origin, ref direction, results, distance, layerMask, minDepth, maxDepth);
        }

        internal static void SetEditorDragMovement(bool dragging, GameObject[] objs)
        {
            foreach (Rigidbody2D rigidbodyd in m_LastDisabledRigidbody2D)
            {
                if (rigidbodyd != null)
                {
                    rigidbodyd.isKinematic = false;
                }
            }
            m_LastDisabledRigidbody2D.Clear();
            if (dragging)
            {
                foreach (GameObject obj2 in objs)
                {
                    foreach (Rigidbody2D rigidbodyd2 in obj2.GetComponentsInChildren<Rigidbody2D>(false))
                    {
                        if (!rigidbodyd2.isKinematic)
                        {
                            rigidbodyd2.isKinematic = true;
                            m_LastDisabledRigidbody2D.Add(rigidbodyd2);
                        }
                    }
                }
            }
        }

        public static float angularSleepTolerance { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float baumgarteScale { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float baumgarteTOIScale { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool changeStopsCallbacks { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("Physics2D.deleteStopsCallbacks is deprecated. Use Physics2D.changeStopsCallbacks instead. (UnityUpgradable) -> changeStopsCallbacks", true)]
        public static bool deleteStopsCallbacks
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public static Vector2 gravity
        {
            get
            {
                Vector2 vector;
                INTERNAL_get_gravity(out vector);
                return vector;
            }
            set
            {
                INTERNAL_set_gravity(ref value);
            }
        }

        public static float linearSleepTolerance { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float maxAngularCorrection { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float maxLinearCorrection { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float maxRotationSpeed { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float maxTranslationSpeed { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float minPenetrationForPenalty { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int positionIterations { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool queriesHitTriggers { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool queriesStartInColliders { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("Physics2D.raycastsHitTriggers is deprecated. Use Physics2D.queriesHitTriggers instead. (UnityUpgradable) -> queriesHitTriggers", true)]
        public static bool raycastsHitTriggers
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        [Obsolete("Physics2D.raycastsStartInColliders is deprecated. Use Physics2D.queriesStartInColliders instead. (UnityUpgradable) -> queriesStartInColliders", true)]
        public static bool raycastsStartInColliders
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public static float timeToSleep { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static int velocityIterations { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static float velocityThreshold { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

