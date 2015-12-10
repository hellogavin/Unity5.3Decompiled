namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class TreeViewItemExpansionAnimator
    {
        private Rect m_CurrentClipRect;
        private bool m_InsideGUIClip;
        private TreeViewAnimationInput m_Setup;
        private static bool s_Debug;

        public void BeginAnimating(TreeViewAnimationInput setup)
        {
            if (this.m_Setup != null)
            {
                if (this.m_Setup.item.id == setup.item.id)
                {
                    if (this.m_Setup.elapsedTime >= 0.0)
                    {
                        setup.elapsedTime = this.m_Setup.animationDuration - this.m_Setup.elapsedTime;
                    }
                    else
                    {
                        Debug.LogError("Invaid duration " + this.m_Setup.elapsedTime);
                    }
                    this.m_Setup = setup;
                }
                else
                {
                    this.m_Setup.FireAnimationEndedEvent();
                    this.m_Setup = setup;
                }
                this.m_Setup.expanding = setup.expanding;
            }
            this.m_Setup = setup;
            if (this.m_Setup == null)
            {
                Debug.LogError("Setup is null");
            }
            if (this.printDebug)
            {
                Console.WriteLine("Begin animating: " + this.m_Setup);
            }
            this.m_CurrentClipRect = this.GetCurrentClippingRect();
        }

        private void BeginClip()
        {
            GUI.BeginClip(this.m_CurrentClipRect);
            this.m_InsideGUIClip = true;
            if (this.printDebug)
            {
                Console.WriteLine("BeginClip startRow: " + this.m_Setup.startRow);
            }
        }

        public bool CullRow(int row, ITreeViewGUI gui)
        {
            if (this.isAnimating)
            {
                if (this.printDebug && (row == 0))
                {
                    Console.WriteLine("--------");
                }
                if ((row > this.m_Setup.startRow) && (row <= this.m_Setup.endRow))
                {
                    float num = gui.GetRowRect(row, 1f).y - this.m_Setup.startRowRect.y;
                    if (num > this.m_CurrentClipRect.height)
                    {
                        if (this.m_InsideGUIClip)
                        {
                            this.EndClip();
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private string DebugItemName(int row)
        {
            return this.m_Setup.treeView.data.GetRows()[row].displayName;
        }

        private void EndClip()
        {
            GUI.EndClip();
            this.m_InsideGUIClip = false;
            if (this.printDebug)
            {
                Console.WriteLine("EndClip endRow: " + this.m_Setup.endRow);
            }
        }

        private Rect GetCurrentClippingRect()
        {
            Rect rowsRect = this.m_Setup.rowsRect;
            rowsRect.height *= this.expandedValueNormalized;
            return rowsRect;
        }

        public bool IsAnimating(int itemID)
        {
            if (!this.isAnimating)
            {
                return false;
            }
            return (this.m_Setup.item.id == itemID);
        }

        public void OnAfterAllRowsGUI()
        {
            if (this.m_InsideGUIClip)
            {
                this.EndClip();
            }
            if (this.isAnimating)
            {
                HandleUtility.Repaint();
            }
        }

        public void OnBeforeAllRowsGUI()
        {
            if (this.isAnimating)
            {
                this.m_CurrentClipRect = this.GetCurrentClippingRect();
                if (this.m_Setup.elapsedTime > this.m_Setup.animationDuration)
                {
                    this.m_Setup.FireAnimationEndedEvent();
                    this.m_Setup = null;
                    if (this.printDebug)
                    {
                        Debug.Log("Animation ended");
                    }
                }
            }
        }

        public Rect OnBeginRowGUI(int row, Rect rowRect)
        {
            if (this.isAnimating)
            {
                if (row == this.m_Setup.startRow)
                {
                    this.BeginClip();
                }
                if ((row >= this.m_Setup.startRow) && (row <= this.m_Setup.endRow))
                {
                    rowRect.y -= this.m_Setup.startRowRect.y;
                    return rowRect;
                }
                if (row > this.m_Setup.endRow)
                {
                    rowRect.y -= this.m_Setup.rowsRect.height - this.m_CurrentClipRect.height;
                }
            }
            return rowRect;
        }

        public void OnEndRowGUI(int row)
        {
            if (this.isAnimating && (this.m_InsideGUIClip && (row == this.m_Setup.endRow)))
            {
                this.EndClip();
            }
        }

        public void OnRowGUI(int row)
        {
            if (this.printDebug)
            {
                Console.WriteLine(row + " Do item " + this.DebugItemName(row));
            }
        }

        public float deltaHeight
        {
            get
            {
                return (this.m_Setup.rowsRect.height - (this.m_Setup.rowsRect.height * this.expandedValueNormalized));
            }
        }

        public int endRow
        {
            get
            {
                return this.m_Setup.endRow;
            }
        }

        public float expandedValueNormalized
        {
            get
            {
                float elapsedTimeNormalized = this.m_Setup.elapsedTimeNormalized;
                return (!this.m_Setup.expanding ? (1f - elapsedTimeNormalized) : elapsedTimeNormalized);
            }
        }

        public bool isAnimating
        {
            get
            {
                return (this.m_Setup != null);
            }
        }

        public bool isExpanding
        {
            get
            {
                return this.m_Setup.expanding;
            }
        }

        private bool printDebug
        {
            get
            {
                return (((s_Debug && (this.m_Setup != null)) && (this.m_Setup.treeView != null)) && (Event.current.type == EventType.Repaint));
            }
        }

        public int startRow
        {
            get
            {
                return this.m_Setup.startRow;
            }
        }
    }
}

