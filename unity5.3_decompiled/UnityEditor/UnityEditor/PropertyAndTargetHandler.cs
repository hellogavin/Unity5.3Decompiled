namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class PropertyAndTargetHandler
    {
        public TargetChoiceHandler.TargetChoiceMenuFunction function;
        public SerializedProperty property;
        public Object target;

        public PropertyAndTargetHandler(SerializedProperty property, Object target, TargetChoiceHandler.TargetChoiceMenuFunction function)
        {
            this.property = property;
            this.target = target;
            this.function = function;
        }
    }
}

