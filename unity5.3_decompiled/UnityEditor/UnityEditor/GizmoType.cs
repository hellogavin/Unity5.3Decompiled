namespace UnityEditor
{
    using System;

    public enum GizmoType
    {
        Active = 8,
        InSelectionHierarchy = 0x10,
        NonSelected = 0x20,
        NotInSelectionHierarchy = 2,
        [Obsolete("Use NotInSelectionHierarchy instead (UnityUpgradable) -> NotInSelectionHierarchy")]
        NotSelected = -127,
        Pickable = 1,
        Selected = 4,
        [Obsolete("Use InSelectionHierarchy instead (UnityUpgradable) -> InSelectionHierarchy")]
        SelectedOrChild = -127
    }
}

