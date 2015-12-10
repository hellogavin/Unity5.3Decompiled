namespace UnityEditor
{
    using System;

    internal class EditorApplicationLayout
    {
        private static GameView m_GameView;
        private static View m_RootSplit;

        private static void Clear()
        {
            m_RootSplit = null;
            m_GameView = null;
        }

        internal static void FinalizePlaymodeLayout()
        {
            if (m_GameView != null)
            {
                if (m_RootSplit != null)
                {
                    WindowLayout.MaximizePresent(m_GameView, m_RootSplit);
                }
                m_GameView.m_Parent.ClearStartView();
            }
            Clear();
        }

        internal static void InitPlaymodeLayout()
        {
            m_GameView = WindowLayout.ShowAppropriateViewOnEnterExitPlaymode(true) as GameView;
            if (m_GameView != null)
            {
                if (m_GameView.maximizeOnPlay)
                {
                    DockArea parent = m_GameView.m_Parent as DockArea;
                    if ((parent != null) && !parent.actualView.m_Parent.window.maximized)
                    {
                        m_RootSplit = WindowLayout.MaximizePrepare(parent.actualView);
                    }
                }
                m_GameView.m_Parent.SetAsStartView();
                Toolbar.RepaintToolbar();
            }
        }

        internal static bool IsInitializingPlaymodeLayout()
        {
            return (m_GameView != null);
        }

        internal static void SetPausemodeLayout()
        {
            SetStopmodeLayout();
        }

        internal static void SetPlaymodeLayout()
        {
            InitPlaymodeLayout();
            FinalizePlaymodeLayout();
        }

        internal static void SetStopmodeLayout()
        {
            WindowLayout.ShowAppropriateViewOnEnterExitPlaymode(false);
            Toolbar.RepaintToolbar();
        }
    }
}

