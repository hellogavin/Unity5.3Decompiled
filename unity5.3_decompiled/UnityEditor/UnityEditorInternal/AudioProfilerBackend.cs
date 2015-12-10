namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal class AudioProfilerBackend
    {
        public AudioProfilerTreeViewState m_TreeViewState;
        public DataUpdateDelegate OnUpdate;

        public AudioProfilerBackend(AudioProfilerTreeViewState state)
        {
            this.m_TreeViewState = state;
            this.items = new List<AudioProfilerInfoWrapper>();
        }

        public void SetData(List<AudioProfilerInfoWrapper> data)
        {
            this.items = data;
            this.UpdateSorting();
        }

        public void UpdateSorting()
        {
            this.items.Sort(new AudioProfilerInfoHelper.AudioProfilerInfoComparer((AudioProfilerInfoHelper.ColumnIndices) this.m_TreeViewState.selectedColumn, (AudioProfilerInfoHelper.ColumnIndices) this.m_TreeViewState.prevSelectedColumn, this.m_TreeViewState.sortByDescendingOrder));
            if (this.OnUpdate != null)
            {
                this.OnUpdate();
            }
        }

        public List<AudioProfilerInfoWrapper> items { get; private set; }

        public delegate void DataUpdateDelegate();
    }
}

