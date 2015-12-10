namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal sealed class OptimizedGUIBlock
    {
        private int m_ActiveDragControl;
        private Color m_GUIColor;
        private int m_KeyboardControl;
        private int m_LastSearchIndex;
        [NonSerialized]
        private IntPtr m_Ptr;
        private bool m_Recording;
        private Rect m_Rect;
        private bool m_Valid;
        private bool m_WatchForUsed;

        public OptimizedGUIBlock()
        {
            this.Init();
        }

        public bool Begin(bool hasChanged, Rect position)
        {
            if (hasChanged)
            {
                this.m_Valid = false;
            }
            if (Event.current.type == EventType.Repaint)
            {
                if (GUIUtility.keyboardControl != this.m_KeyboardControl)
                {
                    this.m_Valid = false;
                    this.m_KeyboardControl = GUIUtility.keyboardControl;
                }
                if (DragAndDrop.activeControlID != this.m_ActiveDragControl)
                {
                    this.m_Valid = false;
                    this.m_ActiveDragControl = DragAndDrop.activeControlID;
                }
                if (GUI.color != this.m_GUIColor)
                {
                    this.m_Valid = false;
                    this.m_GUIColor = GUI.color;
                }
                position = GUIClip.Unclip(position);
                if (this.m_Valid && (position != this.m_Rect))
                {
                    this.m_Rect = position;
                    this.m_Valid = false;
                }
                if (!EditorGUI.isCollectingTooltips)
                {
                    if (this.m_Valid)
                    {
                        return false;
                    }
                    this.m_Recording = true;
                    this.BeginRecording();
                }
                return true;
            }
            if (Event.current.type == EventType.Used)
            {
                return false;
            }
            if (Event.current.type != EventType.Used)
            {
                this.m_WatchForUsed = true;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void BeginRecording();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Dispose();
        public void End()
        {
            bool recording = this.m_Recording;
            if (this.m_Recording)
            {
                this.EndRecording();
                this.m_Recording = false;
                this.m_Valid = true;
                this.m_LastSearchIndex = EditorGUIUtility.GetSearchIndexOfControlIDList();
            }
            if (Event.current == null)
            {
                Debug.LogError("Event.current is null");
            }
            if ((Event.current.type == EventType.Repaint) && !EditorGUI.isCollectingTooltips)
            {
                this.Execute();
                if (!recording)
                {
                    EditorGUIUtility.SetSearchIndexOfControlIDList(this.m_LastSearchIndex);
                }
            }
            if (this.m_WatchForUsed && (Event.current.type == EventType.Used))
            {
                this.m_Valid = false;
            }
            this.m_WatchForUsed = false;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void EndRecording();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Execute();
        ~OptimizedGUIBlock()
        {
            if (this.m_Ptr != IntPtr.Zero)
            {
                Debug.Log("Failed cleaning up Optimized GUI Block");
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Init();

        public bool valid
        {
            get
            {
                return this.m_Valid;
            }
            set
            {
                this.m_Valid = value;
            }
        }
    }
}

