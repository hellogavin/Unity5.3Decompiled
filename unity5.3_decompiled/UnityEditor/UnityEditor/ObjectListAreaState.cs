namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    internal class ObjectListAreaState
    {
        public CreateAssetUtility m_CreateAssetUtility = new CreateAssetUtility();
        public List<int> m_ExpandedInstanceIDs = new List<int>();
        public int m_GridSize = 0x40;
        public bool m_HadKeyboardFocusLastEvent;
        public int m_LastClickedInstanceID;
        public int m_NewAssetIndexInList = -1;
        public RenameOverlay m_RenameOverlay = new RenameOverlay();
        public Vector2 m_ScrollPosition;
        public List<int> m_SelectedInstanceIDs = new List<int>();

        public void OnAwake()
        {
            this.m_NewAssetIndexInList = -1;
            this.m_RenameOverlay.Clear();
            this.m_CreateAssetUtility = new CreateAssetUtility();
        }
    }
}

