namespace UnityEditorInternal
{
    using System;

    internal interface IProfilerWindowController
    {
        void ClearSelectedPropertyPath();
        ProfilerProperty CreateProperty(bool details);
        int GetActiveVisibleFrameIndex();
        string GetSearch();
        bool IsSearching();
        void Repaint();
        void SetSearch(string searchString);
        void SetSelectedPropertyPath(string path);
    }
}

