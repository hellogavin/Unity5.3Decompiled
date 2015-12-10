namespace UnityEditor.MemoryProfiler
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;

    public static class MemorySnapshot
    {
        public static  event Action<PackedMemorySnapshot> OnSnapshotReceived;

        private static void DispatchSnapshot(PackedMemorySnapshot snapshot)
        {
            Action<PackedMemorySnapshot> onSnapshotReceived = OnSnapshotReceived;
            if (onSnapshotReceived != null)
            {
                onSnapshotReceived(snapshot);
            }
        }

        public static void RequestNewSnapshot()
        {
            ProfilerDriver.RequestMemorySnapshot();
        }
    }
}

