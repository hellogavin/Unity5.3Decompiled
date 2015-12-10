namespace UnityEditor.Animations
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    internal class MecanimUtilities
    {
        internal static bool AreSameAsset(Object obj1, Object obj2)
        {
            return (AssetDatabase.GetAssetPath(obj1) == AssetDatabase.GetAssetPath(obj2));
        }

        internal static void DestroyBlendTreeRecursive(BlendTree blendTree)
        {
            for (int i = 0; i < blendTree.children.Length; i++)
            {
                BlendTree motion = blendTree.children[i].motion as BlendTree;
                if ((motion != null) && AreSameAsset(blendTree, motion))
                {
                    DestroyBlendTreeRecursive(motion);
                }
            }
            Undo.DestroyObjectImmediate(blendTree);
        }

        public static bool StateMachineRelativePath(AnimatorStateMachine parent, AnimatorStateMachine toFind, ref List<AnimatorStateMachine> hierarchy)
        {
            hierarchy.Add(parent);
            if (parent == toFind)
            {
                return true;
            }
            for (int i = 0; i < parent.stateMachines.Length; i++)
            {
                if (StateMachineRelativePath(parent.stateMachines[i].stateMachine, toFind, ref hierarchy))
                {
                    return true;
                }
            }
            hierarchy.Remove(parent);
            return false;
        }
    }
}

