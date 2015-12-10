namespace UnityEditorInternal
{
    using System;
    using UnityEngine;

    [Obsolete("State is obsolete. Use UnityEditor.Animations.AnimatorState instead (UnityUpgradable) -> UnityEditor.Animations.AnimatorState", true)]
    public class State : Object
    {
        public BlendTree CreateBlendTree()
        {
            return null;
        }

        public BlendTree CreateBlendTree(AnimatorControllerLayer layer)
        {
            return null;
        }

        public Motion GetMotion()
        {
            return null;
        }

        public Motion GetMotion(AnimatorControllerLayer layer)
        {
            return null;
        }

        public bool iKOnFeet
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public bool mirror
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public float speed
        {
            get
            {
                return -1f;
            }
            set
            {
            }
        }

        public string tag
        {
            get
            {
                return string.Empty;
            }
            set
            {
            }
        }

        public string uniqueName
        {
            get
            {
                return string.Empty;
            }
        }

        public int uniqueNameHash
        {
            get
            {
                return -1;
            }
        }
    }
}

