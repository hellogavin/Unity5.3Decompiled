namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(SpaceAttribute))]
    internal sealed class SpaceDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            return (base.attribute as SpaceAttribute).height;
        }
    }
}

