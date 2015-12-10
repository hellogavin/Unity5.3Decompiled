namespace UnityEditorInternal
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal static class MaterialAnimationUtility
    {
        private const string kMaterialPrefix = "material.";

        private static void ApplyMaterialModificationToAnimationRecording(string name, Object target, Vector4 vec)
        {
            UndoPropertyModification[] modifications = CreateUndoPropertyModifications(4, target);
            SetupPropertyModification(name + ".x", vec.x, modifications[0]);
            SetupPropertyModification(name + ".y", vec.y, modifications[1]);
            SetupPropertyModification(name + ".z", vec.z, modifications[2]);
            SetupPropertyModification(name + ".w", vec.w, modifications[3]);
            Undo.postprocessModifications(modifications);
        }

        private static void ApplyMaterialModificationToAnimationRecording(MaterialProperty materialProp, Object target, float value)
        {
            UndoPropertyModification[] modifications = CreateUndoPropertyModifications(1, target);
            SetupPropertyModification(materialProp.name, value, modifications[0]);
            Undo.postprocessModifications(modifications);
        }

        private static void ApplyMaterialModificationToAnimationRecording(MaterialProperty materialProp, Object target, Color color)
        {
            UndoPropertyModification[] modifications = CreateUndoPropertyModifications(4, target);
            SetupPropertyModification(materialProp.name + ".r", color.r, modifications[0]);
            SetupPropertyModification(materialProp.name + ".g", color.g, modifications[1]);
            SetupPropertyModification(materialProp.name + ".b", color.b, modifications[2]);
            SetupPropertyModification(materialProp.name + ".a", color.a, modifications[3]);
            Undo.postprocessModifications(modifications);
        }

        public static bool ApplyMaterialModificationToAnimationRecording(MaterialProperty materialProp, int changedMask, Renderer target, object oldValue)
        {
            switch (materialProp.type)
            {
                case MaterialProperty.PropType.Color:
                    SetupMaterialPropertyBlock(materialProp, changedMask, target);
                    ApplyMaterialModificationToAnimationRecording(materialProp, target, (Color) oldValue);
                    return true;

                case MaterialProperty.PropType.Vector:
                    SetupMaterialPropertyBlock(materialProp, changedMask, target);
                    ApplyMaterialModificationToAnimationRecording(materialProp, target, (Color) ((Vector4) oldValue));
                    return true;

                case MaterialProperty.PropType.Float:
                case MaterialProperty.PropType.Range:
                    SetupMaterialPropertyBlock(materialProp, changedMask, target);
                    ApplyMaterialModificationToAnimationRecording(materialProp, target, (float) oldValue);
                    return true;

                case MaterialProperty.PropType.Texture:
                {
                    if (!MaterialProperty.IsTextureOffsetAndScaleChangedMask(changedMask))
                    {
                        return false;
                    }
                    string name = materialProp.name + "_ST";
                    SetupMaterialPropertyBlock(materialProp, changedMask, target);
                    ApplyMaterialModificationToAnimationRecording(name, target, (Vector4) oldValue);
                    return true;
                }
            }
            return false;
        }

        private static UndoPropertyModification[] CreateUndoPropertyModifications(int count, Object target)
        {
            UndoPropertyModification[] modificationArray = new UndoPropertyModification[count];
            for (int i = 0; i < modificationArray.Length; i++)
            {
                modificationArray[i].previousValue = new PropertyModification();
                modificationArray[i].previousValue.target = target;
            }
            return modificationArray;
        }

        public static bool IsAnimated(MaterialProperty materialProp, Renderer target)
        {
            if (materialProp.type == MaterialProperty.PropType.Texture)
            {
                return AnimationMode.IsPropertyAnimated(target, "material." + materialProp.name + "_ST");
            }
            return AnimationMode.IsPropertyAnimated(target, "material." + materialProp.name);
        }

        public static void SetupMaterialPropertyBlock(MaterialProperty materialProp, int changedMask, Renderer target)
        {
            MaterialPropertyBlock dest = new MaterialPropertyBlock();
            target.GetPropertyBlock(dest);
            materialProp.WriteToMaterialPropertyBlock(dest, changedMask);
            target.SetPropertyBlock(dest);
        }

        private static void SetupPropertyModification(string name, float value, UndoPropertyModification prop)
        {
            prop.previousValue.propertyPath = "material." + name;
            prop.previousValue.value = value.ToString();
        }
    }
}

