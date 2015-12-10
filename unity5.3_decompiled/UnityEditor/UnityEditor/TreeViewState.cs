namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    internal class TreeViewState
    {
        [SerializeField]
        private float[] m_ColumnWidths;
        [SerializeField]
        private CreateAssetUtility m_CreateAssetUtility = new CreateAssetUtility();
        [SerializeField]
        private List<int> m_ExpandedIDs = new List<int>();
        [SerializeField]
        private int m_LastClickedID;
        [SerializeField]
        private RenameOverlay m_RenameOverlay = new RenameOverlay();
        [SerializeField]
        private string m_SearchString;
        [SerializeField]
        private List<int> m_SelectedIDs = new List<int>();
        public Vector2 scrollPos;

        public void OnAwake()
        {
            this.m_RenameOverlay.Clear();
            this.m_CreateAssetUtility = new CreateAssetUtility();
        }

        public float[] columnWidths
        {
            get
            {
                return this.m_ColumnWidths;
            }
            set
            {
                this.m_ColumnWidths = value;
            }
        }

        public CreateAssetUtility createAssetUtility
        {
            get
            {
                return this.m_CreateAssetUtility;
            }
            set
            {
                this.m_CreateAssetUtility = value;
            }
        }

        public List<int> expandedIDs
        {
            get
            {
                return this.m_ExpandedIDs;
            }
            set
            {
                this.m_ExpandedIDs = value;
            }
        }

        public int lastClickedID
        {
            get
            {
                return this.m_LastClickedID;
            }
            set
            {
                this.m_LastClickedID = value;
            }
        }

        public RenameOverlay renameOverlay
        {
            get
            {
                return this.m_RenameOverlay;
            }
            set
            {
                this.m_RenameOverlay = value;
            }
        }

        public string searchString
        {
            get
            {
                return this.m_SearchString;
            }
            set
            {
                this.m_SearchString = value;
            }
        }

        public List<int> selectedIDs
        {
            get
            {
                return this.m_SelectedIDs;
            }
            set
            {
                this.m_SelectedIDs = value;
            }
        }
    }
}

