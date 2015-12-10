namespace UnityEditorInternal
{
    using System;
    using UnityEditor;

    internal class AnimationWindowHierarchyNode : TreeViewItem
    {
        public Type animatableObjectType;
        public EditorCurveBinding? binding;
        public AnimationWindowCurve[] curves;
        public int indent;
        public string path;
        public string propertyName;
        public float? topPixel;

        public AnimationWindowHierarchyNode(int instanceID, int depth, TreeViewItem parent, Type animatableObjectType, string propertyName, string path, string displayName) : base(instanceID, depth, parent, displayName)
        {
            this.displayName = displayName;
            this.animatableObjectType = animatableObjectType;
            this.propertyName = propertyName;
            this.path = path;
        }
    }
}

