namespace UnityEditor.Animations
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal struct PushUndoIfNeeded
    {
        private PushUndoIfNeededImpl m_Impl;
        public PushUndoIfNeeded(bool pushUndo)
        {
            this.m_Impl = new PushUndoIfNeededImpl(pushUndo);
        }

        public bool pushUndo
        {
            get
            {
                return this.impl.m_PushUndo;
            }
            set
            {
                this.impl.m_PushUndo = value;
            }
        }
        public void DoUndo(Object target, string undoOperation)
        {
            this.impl.DoUndo(target, undoOperation);
        }

        private PushUndoIfNeededImpl impl
        {
            get
            {
                if (this.m_Impl == null)
                {
                    this.m_Impl = new PushUndoIfNeededImpl(true);
                }
                return this.m_Impl;
            }
        }
        private class PushUndoIfNeededImpl
        {
            public bool m_PushUndo;

            public PushUndoIfNeededImpl(bool pushUndo)
            {
                this.m_PushUndo = pushUndo;
            }

            public void DoUndo(Object target, string undoOperation)
            {
                if (this.m_PushUndo)
                {
                    Undo.RegisterCompleteObjectUndo(target, undoOperation);
                }
            }
        }
    }
}

