namespace UnityEditor
{
    using System;
    using UnityEngine;

    [Obsolete("Use Undo.RecordObject before modifying the object instead")]
    public sealed class UndoSnapshot
    {
        public UndoSnapshot(Object[] objectsToUndo)
        {
        }

        public void Dispose()
        {
        }

        public void Restore()
        {
        }
    }
}

