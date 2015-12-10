namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal interface IEditablePoint
    {
        Color GetDefaultColor();
        float GetPointScale();
        Vector3 GetPosition(int idx);
        IEnumerable<Vector3> GetPositions();
        Color GetSelectedColor();
        Vector3[] GetSelectedPositions();
        Vector3[] GetUnselectedPositions();
        void SetPosition(int idx, Vector3 position);

        int Count { get; }
    }
}

