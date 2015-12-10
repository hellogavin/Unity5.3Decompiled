namespace UnityEditorInternal
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class AddCurvesPopup : EditorWindow
    {
        private const float k_WindowPadding = 3f;
        private static AddCurvesPopup s_AddCurvesPopup;
        private static AddCurvesPopupHierarchy s_Hierarchy;
        private static long s_LastClosedTime;
        internal static AnimationWindowState s_State;
        private static Vector2 windowSize = new Vector2(240f, 250f);

        internal static void AddNewCurve(AddCurvesPopupPropertyNode node)
        {
            AnimationWindowUtility.CreateDefaultCurves(s_State, node.curveBindings);
            TreeViewItem item = !(node.parent.displayName == "GameObject") ? node.parent.parent : node.parent;
            s_State.hierarchyState.selectedIDs.Clear();
            s_State.hierarchyState.selectedIDs.Add(item.id);
            s_State.hierarchyData.SetExpanded(item, true);
            s_State.hierarchyData.SetExpanded(node.parent.id, true);
        }

        private void Init(Rect buttonRect)
        {
            buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
            PopupLocationHelper.PopupLocation[] locationPriorityOrder = new PopupLocationHelper.PopupLocation[] { PopupLocationHelper.PopupLocation.Right };
            base.ShowAsDropDown(buttonRect, windowSize, locationPriorityOrder);
        }

        private void OnDisable()
        {
            s_LastClosedTime = DateTime.Now.Ticks / 0x2710L;
            s_AddCurvesPopup = null;
            s_Hierarchy = null;
        }

        internal void OnGUI()
        {
            if (Event.current.type != EventType.Layout)
            {
                if (s_Hierarchy == null)
                {
                    s_Hierarchy = new AddCurvesPopupHierarchy(s_State);
                }
                Rect position = new Rect(1f, 1f, windowSize.x - 3f, windowSize.y - 3f);
                GUI.Box(new Rect(0f, 0f, windowSize.x, windowSize.y), GUIContent.none, new GUIStyle("grey_border"));
                s_Hierarchy.OnGUI(position, this);
            }
        }

        internal static bool ShowAtPosition(Rect buttonRect, AnimationWindowState state)
        {
            long num = DateTime.Now.Ticks / 0x2710L;
            if (num < (s_LastClosedTime + 50L))
            {
                return false;
            }
            Event.current.Use();
            if (s_AddCurvesPopup == null)
            {
                s_AddCurvesPopup = ScriptableObject.CreateInstance<AddCurvesPopup>();
            }
            s_State = state;
            s_AddCurvesPopup.Init(buttonRect);
            return true;
        }

        internal static Object animatableObject
        {
            [CompilerGenerated]
            get
            {
                return <animatableObject>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                <animatableObject>k__BackingField = value;
            }
        }

        internal static GameObject gameObject
        {
            [CompilerGenerated]
            get
            {
                return <gameObject>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                <gameObject>k__BackingField = value;
            }
        }

        internal static string path
        {
            get
            {
                return AnimationUtility.CalculateTransformPath(gameObject.transform, s_State.activeRootGameObject.transform);
            }
        }
    }
}

