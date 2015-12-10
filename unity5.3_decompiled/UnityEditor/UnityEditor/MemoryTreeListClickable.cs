namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class MemoryTreeListClickable : MemoryTreeList
    {
        public MemoryTreeListClickable(EditorWindow editorWindow, MemoryTreeList detailview) : base(editorWindow, detailview)
        {
        }

        protected override void DrawData(Rect rect, MemoryElement memoryElement, int indent, int row, bool selected)
        {
            if (Event.current.type == EventType.Repaint)
            {
                string name = memoryElement.name;
                if ((memoryElement.ChildCount() > 0) && (indent < 3))
                {
                    name = name + " (" + memoryElement.AccumulatedChildCount().ToString() + ")";
                }
                int index = 0;
                rect.xMax = base.m_Splitter.realSizes[index];
                MemoryTreeList.styles.numberLabel.Draw(rect, name, false, false, false, selected);
                rect.x = rect.xMax;
                rect.width = base.m_Splitter.realSizes[++index] - 4f;
                MemoryTreeList.styles.numberLabel.Draw(rect, EditorUtility.FormatBytes(memoryElement.totalMemory), false, false, false, selected);
                rect.x += base.m_Splitter.realSizes[index++];
                rect.width = base.m_Splitter.realSizes[index] - 4f;
                if (memoryElement.ReferenceCount() > 0)
                {
                    MemoryTreeList.styles.numberLabel.Draw(rect, memoryElement.ReferenceCount().ToString(), false, false, false, selected);
                }
                else if (selected)
                {
                    MemoryTreeList.styles.numberLabel.Draw(rect, string.Empty, false, false, false, selected);
                }
            }
        }

        protected override void DrawHeader()
        {
            GUILayout.Label("Name", MemoryTreeList.styles.header, new GUILayoutOption[0]);
            GUILayout.Label("Memory", MemoryTreeList.styles.header, new GUILayoutOption[0]);
            GUILayout.Label("Ref count", MemoryTreeList.styles.header, new GUILayoutOption[0]);
        }

        protected override void SetupSplitter()
        {
            float[] relativeSizes = new float[3];
            int[] minSizes = new int[3];
            relativeSizes[0] = 300f;
            minSizes[0] = 100;
            relativeSizes[1] = 70f;
            minSizes[1] = 50;
            relativeSizes[2] = 70f;
            minSizes[2] = 50;
            base.m_Splitter = new SplitterState(relativeSizes, minSizes, null);
        }
    }
}

