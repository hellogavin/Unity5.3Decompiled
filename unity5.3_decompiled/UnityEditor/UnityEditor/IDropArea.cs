namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal interface IDropArea
    {
        DropInfo DragOver(EditorWindow w, Vector2 screenPos);
        bool PerformDrop(EditorWindow w, DropInfo dropInfo, Vector2 screenPos);
    }
}

