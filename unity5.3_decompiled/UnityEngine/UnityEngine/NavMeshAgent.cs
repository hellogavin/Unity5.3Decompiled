namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class NavMeshAgent : Behaviour
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ActivateCurrentOffMeshLink(bool activated);
        public bool CalculatePath(Vector3 targetPosition, NavMeshPath path)
        {
            path.ClearCorners();
            return this.CalculatePathInternal(targetPosition, path);
        }

        private bool CalculatePathInternal(Vector3 targetPosition, NavMeshPath path)
        {
            return INTERNAL_CALL_CalculatePathInternal(this, ref targetPosition, path);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void CompleteOffMeshLink();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void CopyPathTo(NavMeshPath path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool FindClosestEdge(out NavMeshHit hit);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern float GetAreaCost(int areaIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern OffMeshLinkData GetCurrentOffMeshLinkDataInternal();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("Use GetAreaCost instead.")]
        public extern float GetLayerCost(int layer);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern OffMeshLinkData GetNextOffMeshLinkDataInternal();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_CalculatePathInternal(NavMeshAgent self, ref Vector3 targetPosition, NavMeshPath path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Move(NavMeshAgent self, ref Vector3 offset);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_Raycast(NavMeshAgent self, ref Vector3 targetPosition, out NavMeshHit hit);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_SetDestination(NavMeshAgent self, ref Vector3 target);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool INTERNAL_CALL_Warp(NavMeshAgent self, ref Vector3 newPosition);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_desiredVelocity(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_destination(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_nextPosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_pathEndPosition(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_steeringTarget(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_velocity(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_destination(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_nextPosition(ref Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_velocity(ref Vector3 value);
        public void Move(Vector3 offset)
        {
            INTERNAL_CALL_Move(this, ref offset);
        }

        public bool Raycast(Vector3 targetPosition, out NavMeshHit hit)
        {
            return INTERNAL_CALL_Raycast(this, ref targetPosition, out hit);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ResetPath();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Resume();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool SamplePathPosition(int areaMask, float maxDistance, out NavMeshHit hit);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetAreaCost(int areaIndex, float areaCost);
        public bool SetDestination(Vector3 target)
        {
            return INTERNAL_CALL_SetDestination(this, ref target);
        }

        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Use SetAreaCost instead."), WrapperlessIcall]
        public extern void SetLayerCost(int layer, float cost);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool SetPath(NavMeshPath path);
        public void Stop()
        {
            this.StopInternal();
        }

        [Obsolete("Use Stop() instead")]
        public void Stop(bool stopUpdates)
        {
            this.StopInternal();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void StopInternal();
        public bool Warp(Vector3 newPosition)
        {
            return INTERNAL_CALL_Warp(this, ref newPosition);
        }

        public float acceleration { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float angularSpeed { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int areaMask { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool autoBraking { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool autoRepath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool autoTraverseOffMeshLink { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int avoidancePriority { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float baseOffset { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public OffMeshLinkData currentOffMeshLinkData
        {
            get
            {
                return this.GetCurrentOffMeshLinkDataInternal();
            }
        }

        public Vector3 desiredVelocity
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_desiredVelocity(out vector);
                return vector;
            }
        }

        public Vector3 destination
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_destination(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_destination(ref value);
            }
        }

        public bool hasPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float height { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool isOnNavMesh { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isOnOffMeshLink { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isPathStale { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public OffMeshLinkData nextOffMeshLinkData
        {
            get
            {
                return this.GetNextOffMeshLinkDataInternal();
            }
        }

        public Vector3 nextPosition
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_nextPosition(out vector);
                return vector;
            }
            set
            {
                this.INTERNAL_set_nextPosition(ref value);
            }
        }

        public ObstacleAvoidanceType obstacleAvoidanceType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public NavMeshPath path
        {
            get
            {
                NavMeshPath path = new NavMeshPath();
                this.CopyPathTo(path);
                return path;
            }
            set
            {
                if (value == null)
                {
                    throw new NullReferenceException();
                }
                this.SetPath(value);
            }
        }

        public Vector3 pathEndPosition
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_pathEndPosition(out vector);
                return vector;
            }
        }

        public bool pathPending { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public NavMeshPathStatus pathStatus { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float radius { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float remainingDistance { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float speed { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 steeringTarget
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_steeringTarget(out vector);
                return vector;
            }
        }

        public float stoppingDistance { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool updatePosition { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool updateRotation { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

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

        [Obsolete("Use areaMask instead.")]
        public int walkableMask { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

