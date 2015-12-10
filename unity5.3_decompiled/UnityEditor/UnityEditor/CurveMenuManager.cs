namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditorInternal;
    using UnityEngine;

    internal class CurveMenuManager
    {
        private CurveUpdater updater;

        public CurveMenuManager(CurveUpdater updater)
        {
            this.updater = updater;
        }

        public void AddTangentMenuItems(GenericMenu menu, List<KeyIdentifier> keyList)
        {
            bool flag = keyList.Count > 0;
            bool on = flag;
            bool flag3 = flag;
            bool flag4 = flag;
            bool flag5 = flag;
            bool flag6 = flag;
            bool flag7 = flag;
            bool flag8 = flag;
            bool flag9 = flag;
            bool flag10 = flag;
            bool flag11 = flag;
            foreach (KeyIdentifier identifier in keyList)
            {
                Keyframe key = identifier.keyframe;
                TangentMode keyTangentMode = CurveUtility.GetKeyTangentMode(key, 0);
                TangentMode mode2 = CurveUtility.GetKeyTangentMode(key, 1);
                bool keyBroken = CurveUtility.GetKeyBroken(key);
                if ((keyTangentMode != TangentMode.Smooth) || (mode2 != TangentMode.Smooth))
                {
                    on = false;
                }
                if ((keyBroken || (keyTangentMode != TangentMode.Editable)) || (mode2 != TangentMode.Editable))
                {
                    flag3 = false;
                }
                if ((keyBroken || (keyTangentMode != TangentMode.Editable)) || (((key.inTangent != 0f) || (mode2 != TangentMode.Editable)) || (key.outTangent != 0f)))
                {
                    flag4 = false;
                }
                if (!keyBroken)
                {
                    flag5 = false;
                }
                if (!keyBroken || (keyTangentMode != TangentMode.Editable))
                {
                    flag6 = false;
                }
                if (!keyBroken || (keyTangentMode != TangentMode.Linear))
                {
                    flag7 = false;
                }
                if (!keyBroken || (keyTangentMode != TangentMode.Stepped))
                {
                    flag8 = false;
                }
                if (!keyBroken || (mode2 != TangentMode.Editable))
                {
                    flag9 = false;
                }
                if (!keyBroken || (mode2 != TangentMode.Linear))
                {
                    flag10 = false;
                }
                if (!keyBroken || (mode2 != TangentMode.Stepped))
                {
                    flag11 = false;
                }
            }
            if (flag)
            {
                menu.AddItem(EditorGUIUtility.TextContent("Auto"), on, new GenericMenu.MenuFunction2(this.SetSmooth), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Free Smooth"), flag3, new GenericMenu.MenuFunction2(this.SetEditable), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Flat"), flag4, new GenericMenu.MenuFunction2(this.SetFlat), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Broken"), flag5, new GenericMenu.MenuFunction2(this.SetBroken), keyList);
                menu.AddSeparator(string.Empty);
                menu.AddItem(EditorGUIUtility.TextContent("Left Tangent/Free"), flag6, new GenericMenu.MenuFunction2(this.SetLeftEditable), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Left Tangent/Linear"), flag7, new GenericMenu.MenuFunction2(this.SetLeftLinear), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Left Tangent/Constant"), flag8, new GenericMenu.MenuFunction2(this.SetLeftConstant), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Right Tangent/Free"), flag9, new GenericMenu.MenuFunction2(this.SetRightEditable), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Right Tangent/Linear"), flag10, new GenericMenu.MenuFunction2(this.SetRightLinear), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Right Tangent/Constant"), flag11, new GenericMenu.MenuFunction2(this.SetRightConstant), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Both Tangents/Free"), flag9 && flag6, new GenericMenu.MenuFunction2(this.SetBothEditable), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Both Tangents/Linear"), flag10 && flag7, new GenericMenu.MenuFunction2(this.SetBothLinear), keyList);
                menu.AddItem(EditorGUIUtility.TextContent("Both Tangents/Constant"), flag11 && flag8, new GenericMenu.MenuFunction2(this.SetBothConstant), keyList);
            }
            else
            {
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Auto"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Free Smooth"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Flat"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Broken"));
                menu.AddSeparator(string.Empty);
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Left Tangent/Free"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Left Tangent/Linear"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Left Tangent/Constant"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Right Tangent/Free"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Right Tangent/Linear"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Right Tangent/Constant"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Both Tangents/Free"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Both Tangents/Linear"));
                menu.AddDisabledItem(EditorGUIUtility.TextContent("Both Tangents/Constant"));
            }
        }

        public void Flatten(List<KeyIdentifier> keysToSet)
        {
            List<ChangedCurve> list = new List<ChangedCurve>();
            List<int> curveIds = new List<int>();
            foreach (KeyIdentifier identifier in keysToSet)
            {
                AnimationCurve curve = identifier.curve;
                Keyframe key = identifier.keyframe;
                key.inTangent = 0f;
                key.outTangent = 0f;
                curve.MoveKey(identifier.key, key);
                CurveUtility.UpdateTangentsFromModeSurrounding(curve, identifier.key);
                ChangedCurve item = new ChangedCurve(curve, identifier.binding);
                if (!list.Contains(item))
                {
                    list.Add(item);
                }
                curveIds.Add(identifier.curveId);
            }
            if (this.updater is DopeSheetEditor)
            {
                this.updater.UpdateCurves(list, "Set Tangents");
            }
            else
            {
                this.updater.UpdateCurves(curveIds, "Set Tangents");
            }
        }

        public void SetBoth(TangentMode mode, List<KeyIdentifier> keysToSet)
        {
            List<ChangedCurve> list = new List<ChangedCurve>();
            List<int> curveIds = new List<int>();
            foreach (KeyIdentifier identifier in keysToSet)
            {
                AnimationCurve curve = identifier.curve;
                Keyframe key = identifier.keyframe;
                CurveUtility.SetKeyBroken(ref key, false);
                CurveUtility.SetKeyTangentMode(ref key, 1, mode);
                CurveUtility.SetKeyTangentMode(ref key, 0, mode);
                if (mode == TangentMode.Editable)
                {
                    float num = CurveUtility.CalculateSmoothTangent(key);
                    key.inTangent = num;
                    key.outTangent = num;
                }
                curve.MoveKey(identifier.key, key);
                CurveUtility.UpdateTangentsFromModeSurrounding(curve, identifier.key);
                ChangedCurve item = new ChangedCurve(curve, identifier.binding);
                if (!list.Contains(item))
                {
                    list.Add(item);
                }
                curveIds.Add(identifier.curveId);
            }
            if (this.updater is DopeSheetEditor)
            {
                this.updater.UpdateCurves(list, "Set Tangents");
            }
            else
            {
                this.updater.UpdateCurves(curveIds, "Set Tangents");
            }
        }

        public void SetBothConstant(object keysToSet)
        {
            this.SetTangent(2, TangentMode.Stepped, (List<KeyIdentifier>) keysToSet);
        }

        public void SetBothEditable(object keysToSet)
        {
            this.SetTangent(2, TangentMode.Editable, (List<KeyIdentifier>) keysToSet);
        }

        public void SetBothLinear(object keysToSet)
        {
            this.SetTangent(2, TangentMode.Linear, (List<KeyIdentifier>) keysToSet);
        }

        public void SetBroken(object _keysToSet)
        {
            List<ChangedCurve> list = new List<ChangedCurve>();
            List<KeyIdentifier> list2 = (List<KeyIdentifier>) _keysToSet;
            List<int> curveIds = new List<int>();
            foreach (KeyIdentifier identifier in list2)
            {
                AnimationCurve curve = identifier.curve;
                Keyframe key = identifier.keyframe;
                CurveUtility.SetKeyBroken(ref key, true);
                if (CurveUtility.GetKeyTangentMode(key, 1) == TangentMode.Smooth)
                {
                    CurveUtility.SetKeyTangentMode(ref key, 1, TangentMode.Editable);
                }
                if (CurveUtility.GetKeyTangentMode(key, 0) == TangentMode.Smooth)
                {
                    CurveUtility.SetKeyTangentMode(ref key, 0, TangentMode.Editable);
                }
                curve.MoveKey(identifier.key, key);
                CurveUtility.UpdateTangentsFromModeSurrounding(curve, identifier.key);
                ChangedCurve item = new ChangedCurve(curve, identifier.binding);
                if (!list.Contains(item))
                {
                    list.Add(item);
                }
                curveIds.Add(identifier.curveId);
            }
            if (this.updater is DopeSheetEditor)
            {
                this.updater.UpdateCurves(list, "Set Tangents");
            }
            else
            {
                this.updater.UpdateCurves(curveIds, "Set Tangents");
            }
        }

        public void SetEditable(object keysToSet)
        {
            this.SetBoth(TangentMode.Editable, (List<KeyIdentifier>) keysToSet);
        }

        public void SetFlat(object keysToSet)
        {
            this.SetBoth(TangentMode.Editable, (List<KeyIdentifier>) keysToSet);
            this.Flatten((List<KeyIdentifier>) keysToSet);
        }

        public void SetLeftConstant(object keysToSet)
        {
            this.SetTangent(0, TangentMode.Stepped, (List<KeyIdentifier>) keysToSet);
        }

        public void SetLeftEditable(object keysToSet)
        {
            this.SetTangent(0, TangentMode.Editable, (List<KeyIdentifier>) keysToSet);
        }

        public void SetLeftLinear(object keysToSet)
        {
            this.SetTangent(0, TangentMode.Linear, (List<KeyIdentifier>) keysToSet);
        }

        public void SetRightConstant(object keysToSet)
        {
            this.SetTangent(1, TangentMode.Stepped, (List<KeyIdentifier>) keysToSet);
        }

        public void SetRightEditable(object keysToSet)
        {
            this.SetTangent(1, TangentMode.Editable, (List<KeyIdentifier>) keysToSet);
        }

        public void SetRightLinear(object keysToSet)
        {
            this.SetTangent(1, TangentMode.Linear, (List<KeyIdentifier>) keysToSet);
        }

        public void SetSmooth(object keysToSet)
        {
            this.SetBoth(TangentMode.Smooth, (List<KeyIdentifier>) keysToSet);
        }

        public void SetTangent(int leftRight, TangentMode mode, List<KeyIdentifier> keysToSet)
        {
            List<int> curveIds = new List<int>();
            List<ChangedCurve> list2 = new List<ChangedCurve>();
            foreach (KeyIdentifier identifier in keysToSet)
            {
                AnimationCurve curve = identifier.curve;
                Keyframe key = identifier.keyframe;
                CurveUtility.SetKeyBroken(ref key, true);
                if (leftRight == 2)
                {
                    CurveUtility.SetKeyTangentMode(ref key, 0, mode);
                    CurveUtility.SetKeyTangentMode(ref key, 1, mode);
                }
                else
                {
                    CurveUtility.SetKeyTangentMode(ref key, leftRight, mode);
                    if (CurveUtility.GetKeyTangentMode(key, 1 - leftRight) == TangentMode.Smooth)
                    {
                        CurveUtility.SetKeyTangentMode(ref key, 1 - leftRight, TangentMode.Editable);
                    }
                }
                if ((mode == TangentMode.Stepped) && ((leftRight == 0) || (leftRight == 2)))
                {
                    key.inTangent = float.PositiveInfinity;
                }
                if ((mode == TangentMode.Stepped) && ((leftRight == 1) || (leftRight == 2)))
                {
                    key.outTangent = float.PositiveInfinity;
                }
                curve.MoveKey(identifier.key, key);
                CurveUtility.UpdateTangentsFromModeSurrounding(curve, identifier.key);
                ChangedCurve item = new ChangedCurve(curve, identifier.binding);
                if (!list2.Contains(item))
                {
                    list2.Add(item);
                }
                curveIds.Add(identifier.curveId);
            }
            if (this.updater is DopeSheetEditor)
            {
                this.updater.UpdateCurves(list2, "Set Tangents");
            }
            else
            {
                this.updater.UpdateCurves(curveIds, "Set Tangents");
            }
        }
    }
}

