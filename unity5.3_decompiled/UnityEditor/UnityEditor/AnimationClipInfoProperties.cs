namespace UnityEditor
{
    using System;
    using UnityEditor.Animations;
    using UnityEngine;

    internal class AnimationClipInfoProperties
    {
        private SerializedProperty m_Property;

        public AnimationClipInfoProperties(SerializedProperty prop)
        {
            this.m_Property = prop;
        }

        public void AddCurve()
        {
            SerializedProperty property = this.Get("curves");
            if ((property != null) && property.isArray)
            {
                property.InsertArrayElementAtIndex(property.arraySize);
                property.GetArrayElementAtIndex(property.arraySize - 1).FindPropertyRelative("name").stringValue = "Curve";
                Keyframe[] keys = new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 0f) };
                AnimationCurve curve = new AnimationCurve(keys) {
                    preWrapMode = WrapMode.Default,
                    postWrapMode = WrapMode.Default
                };
                property.GetArrayElementAtIndex(property.arraySize - 1).FindPropertyRelative("curve").animationCurveValue = curve;
            }
        }

        public void AddEvent(float time)
        {
            SerializedProperty property = this.Get("events");
            if ((property != null) && property.isArray)
            {
                property.InsertArrayElementAtIndex(property.arraySize);
                property.GetArrayElementAtIndex(property.arraySize - 1).FindPropertyRelative("functionName").stringValue = "NewEvent";
                property.GetArrayElementAtIndex(property.arraySize - 1).FindPropertyRelative("time").floatValue = time;
            }
        }

        public void AssignToPreviewClip(AnimationClip clip)
        {
            AnimationClipSettings srcClipInfo = new AnimationClipSettings {
                startTime = this.firstFrame / clip.frameRate,
                stopTime = this.lastFrame / clip.frameRate,
                orientationOffsetY = this.orientationOffsetY,
                level = this.level,
                cycleOffset = this.cycleOffset,
                loopTime = this.loopTime,
                loopBlend = this.loopBlend,
                loopBlendOrientation = this.loopBlendOrientation,
                loopBlendPositionY = this.loopBlendPositionY,
                loopBlendPositionXZ = this.loopBlendPositionXZ,
                keepOriginalOrientation = this.keepOriginalOrientation,
                keepOriginalPositionY = this.keepOriginalPositionY,
                keepOriginalPositionXZ = this.keepOriginalPositionXZ,
                heightFromFeet = this.heightFromFeet,
                mirror = this.mirror,
                hasAdditiveReferencePose = this.hasAdditiveReferencePose,
                additiveReferencePoseTime = this.additiveReferencePoseFrame / clip.frameRate
            };
            AnimationUtility.SetAnimationClipSettingsNoDirty(clip, srcClipInfo);
        }

        public void ClearCurves()
        {
            SerializedProperty property = this.Get("curves");
            if ((property != null) && property.isArray)
            {
                property.ClearArray();
            }
        }

        public void ClearEvents()
        {
            SerializedProperty property = this.Get("events");
            if ((property != null) && property.isArray)
            {
                property.ClearArray();
            }
        }

        public void ExtractFromPreviewClip(AnimationClip clip)
        {
            AnimationClipSettings animationClipSettings = AnimationUtility.GetAnimationClipSettings(clip);
            if ((this.firstFrame / clip.frameRate) != animationClipSettings.startTime)
            {
                this.firstFrame = this.FixPrecisionErrors(animationClipSettings.startTime * clip.frameRate);
            }
            if ((this.lastFrame / clip.frameRate) != animationClipSettings.stopTime)
            {
                this.lastFrame = this.FixPrecisionErrors(animationClipSettings.stopTime * clip.frameRate);
            }
            this.orientationOffsetY = animationClipSettings.orientationOffsetY;
            this.level = animationClipSettings.level;
            this.cycleOffset = animationClipSettings.cycleOffset;
            this.loopTime = animationClipSettings.loopTime;
            this.loopBlend = animationClipSettings.loopBlend;
            this.loopBlendOrientation = animationClipSettings.loopBlendOrientation;
            this.loopBlendPositionY = animationClipSettings.loopBlendPositionY;
            this.loopBlendPositionXZ = animationClipSettings.loopBlendPositionXZ;
            this.keepOriginalOrientation = animationClipSettings.keepOriginalOrientation;
            this.keepOriginalPositionY = animationClipSettings.keepOriginalPositionY;
            this.keepOriginalPositionXZ = animationClipSettings.keepOriginalPositionXZ;
            this.heightFromFeet = animationClipSettings.heightFromFeet;
            this.mirror = animationClipSettings.mirror;
            this.hasAdditiveReferencePose = animationClipSettings.hasAdditiveReferencePose;
            if ((this.additiveReferencePoseFrame / clip.frameRate) != animationClipSettings.additiveReferencePoseTime)
            {
                this.additiveReferencePoseFrame = this.FixPrecisionErrors(animationClipSettings.additiveReferencePoseTime * clip.frameRate);
            }
        }

        private float FixPrecisionErrors(float f)
        {
            float num = Mathf.Round(f);
            if (Mathf.Abs((float) (f - num)) < 0.0001f)
            {
                return num;
            }
            return f;
        }

        private SerializedProperty Get(string property)
        {
            return this.m_Property.FindPropertyRelative(property);
        }

        public AnimationCurve GetCurve(int index)
        {
            AnimationCurve animationCurveValue = null;
            SerializedProperty curveProperty = this.GetCurveProperty(index);
            if (curveProperty != null)
            {
                animationCurveValue = curveProperty.animationCurveValue;
            }
            return animationCurveValue;
        }

        public int GetCurveCount()
        {
            int arraySize = 0;
            SerializedProperty property = this.Get("curves");
            if ((property != null) && property.isArray)
            {
                arraySize = property.arraySize;
            }
            return arraySize;
        }

        public string GetCurveName(int index)
        {
            string stringValue = string.Empty;
            SerializedProperty property = this.Get("curves");
            if ((property != null) && property.isArray)
            {
                stringValue = property.GetArrayElementAtIndex(index).FindPropertyRelative("name").stringValue;
            }
            return stringValue;
        }

        public SerializedProperty GetCurveProperty(int index)
        {
            SerializedProperty property = null;
            SerializedProperty property2 = this.Get("curves");
            if ((property2 != null) && property2.isArray)
            {
                property = property2.GetArrayElementAtIndex(index).FindPropertyRelative("curve");
            }
            return property;
        }

        public AnimationEvent GetEvent(int index)
        {
            AnimationEvent event2 = new AnimationEvent();
            SerializedProperty property = this.Get("events");
            if ((property != null) && property.isArray)
            {
                if (index < property.arraySize)
                {
                    event2.floatParameter = property.GetArrayElementAtIndex(index).FindPropertyRelative("floatParameter").floatValue;
                    event2.functionName = property.GetArrayElementAtIndex(index).FindPropertyRelative("functionName").stringValue;
                    event2.intParameter = property.GetArrayElementAtIndex(index).FindPropertyRelative("intParameter").intValue;
                    event2.objectReferenceParameter = property.GetArrayElementAtIndex(index).FindPropertyRelative("objectReferenceParameter").objectReferenceValue;
                    event2.stringParameter = property.GetArrayElementAtIndex(index).FindPropertyRelative("data").stringValue;
                    event2.time = property.GetArrayElementAtIndex(index).FindPropertyRelative("time").floatValue;
                    return event2;
                }
                Debug.LogWarning("Invalid Event Index");
            }
            return event2;
        }

        public int GetEventCount()
        {
            int arraySize = 0;
            SerializedProperty property = this.Get("events");
            if ((property != null) && property.isArray)
            {
                arraySize = property.arraySize;
            }
            return arraySize;
        }

        public AnimationEvent[] GetEvents()
        {
            AnimationEvent[] eventArray = new AnimationEvent[this.GetEventCount()];
            SerializedProperty property = this.Get("events");
            if ((property != null) && property.isArray)
            {
                for (int i = 0; i < this.GetEventCount(); i++)
                {
                    eventArray[i] = this.GetEvent(i);
                }
            }
            return eventArray;
        }

        public void MaskFromClip(AvatarMask mask)
        {
            SerializedProperty property = this.Get("bodyMask");
            if ((property != null) && property.isArray)
            {
                for (AvatarMaskBodyPart part = AvatarMaskBodyPart.Root; part < AvatarMaskBodyPart.LastBodyPart; part += 1)
                {
                    mask.SetHumanoidBodyPartActive(part, property.GetArrayElementAtIndex((int) part).intValue != 0);
                }
            }
            SerializedProperty property2 = this.Get("transformMask");
            if ((property2 != null) && property2.isArray)
            {
                if ((property2.arraySize > 0) && (mask.transformCount != property2.arraySize))
                {
                    mask.transformCount = property2.arraySize;
                }
                int arraySize = property2.arraySize;
                for (int i = 0; i < arraySize; i++)
                {
                    SerializedProperty property3 = property2.GetArrayElementAtIndex(i).FindPropertyRelative("m_Path");
                    SerializedProperty property4 = property2.GetArrayElementAtIndex(i).FindPropertyRelative("m_Weight");
                    mask.SetTransformPath(i, property3.stringValue);
                    mask.SetTransformActive(i, property4.floatValue > 0.5);
                }
            }
        }

        public bool MaskNeedsUpdating()
        {
            AvatarMask maskSource = this.maskSource;
            if (maskSource != null)
            {
                SerializedProperty property = this.Get("bodyMask");
                if ((property == null) || !property.isArray)
                {
                    return true;
                }
                for (AvatarMaskBodyPart part = AvatarMaskBodyPart.Root; part < AvatarMaskBodyPart.LastBodyPart; part += 1)
                {
                    if (maskSource.GetHumanoidBodyPartActive(part) != (property.GetArrayElementAtIndex((int) part).intValue != 0))
                    {
                        return true;
                    }
                }
                SerializedProperty property2 = this.Get("transformMask");
                if ((property2 == null) || !property2.isArray)
                {
                    return true;
                }
                if ((property2.arraySize > 0) && (maskSource.transformCount != property2.arraySize))
                {
                    return true;
                }
                int arraySize = property2.arraySize;
                for (int i = 0; i < arraySize; i++)
                {
                    SerializedProperty property3 = property2.GetArrayElementAtIndex(i).FindPropertyRelative("m_Path");
                    SerializedProperty property4 = property2.GetArrayElementAtIndex(i).FindPropertyRelative("m_Weight");
                    if ((maskSource.GetTransformPath(i) != property3.stringValue) || (maskSource.GetTransformActive(i) != (property4.floatValue > 0.5f)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void MaskToClip(AvatarMask mask)
        {
            SerializedProperty property = this.Get("bodyMask");
            if ((property != null) && property.isArray)
            {
                for (AvatarMaskBodyPart part = AvatarMaskBodyPart.Root; part < AvatarMaskBodyPart.LastBodyPart; part += 1)
                {
                    if (part >= property.arraySize)
                    {
                        property.InsertArrayElementAtIndex((int) part);
                    }
                    property.GetArrayElementAtIndex((int) part).intValue = !mask.GetHumanoidBodyPartActive(part) ? 0 : 1;
                }
            }
            SerializedProperty serializedProperty = this.Get("transformMask");
            ModelImporter.UpdateTransformMask(mask, serializedProperty);
        }

        public void RemoveCurve(int index)
        {
            SerializedProperty property = this.Get("curves");
            if ((property != null) && property.isArray)
            {
                property.DeleteArrayElementAtIndex(index);
            }
        }

        public void RemoveEvent(int index)
        {
            SerializedProperty property = this.Get("events");
            if ((property != null) && property.isArray)
            {
                property.DeleteArrayElementAtIndex(index);
            }
        }

        public void SetCurve(int index, AnimationCurve curveValue)
        {
            SerializedProperty curveProperty = this.GetCurveProperty(index);
            if (curveProperty != null)
            {
                curveProperty.animationCurveValue = curveValue;
            }
        }

        public void SetCurveName(int index, string name)
        {
            SerializedProperty property = this.Get("curves");
            if ((property != null) && property.isArray)
            {
                property.GetArrayElementAtIndex(index).FindPropertyRelative("name").stringValue = name;
            }
        }

        public void SetEvent(int index, AnimationEvent animationEvent)
        {
            SerializedProperty property = this.Get("events");
            if ((property != null) && property.isArray)
            {
                if (index < property.arraySize)
                {
                    property.GetArrayElementAtIndex(index).FindPropertyRelative("floatParameter").floatValue = animationEvent.floatParameter;
                    property.GetArrayElementAtIndex(index).FindPropertyRelative("functionName").stringValue = animationEvent.functionName;
                    property.GetArrayElementAtIndex(index).FindPropertyRelative("intParameter").intValue = animationEvent.intParameter;
                    property.GetArrayElementAtIndex(index).FindPropertyRelative("objectReferenceParameter").objectReferenceValue = animationEvent.objectReferenceParameter;
                    property.GetArrayElementAtIndex(index).FindPropertyRelative("data").stringValue = animationEvent.stringParameter;
                    property.GetArrayElementAtIndex(index).FindPropertyRelative("time").floatValue = animationEvent.time;
                }
                else
                {
                    Debug.LogWarning("Invalid Event Index");
                }
            }
        }

        public void SetEvents(AnimationEvent[] newEvents)
        {
            SerializedProperty property = this.Get("events");
            if ((property != null) && property.isArray)
            {
                property.ClearArray();
                foreach (AnimationEvent event2 in newEvents)
                {
                    property.InsertArrayElementAtIndex(property.arraySize);
                    this.SetEvent(property.arraySize - 1, event2);
                }
            }
        }

        public float additiveReferencePoseFrame
        {
            get
            {
                return this.Get("additiveReferencePoseFrame").floatValue;
            }
            set
            {
                this.Get("additiveReferencePoseFrame").floatValue = value;
            }
        }

        public SerializedProperty bodyMaskProperty
        {
            get
            {
                return this.Get("bodyMask");
            }
        }

        public float cycleOffset
        {
            get
            {
                return this.Get("cycleOffset").floatValue;
            }
            set
            {
                this.Get("cycleOffset").floatValue = value;
            }
        }

        public float firstFrame
        {
            get
            {
                return this.Get("firstFrame").floatValue;
            }
            set
            {
                this.Get("firstFrame").floatValue = value;
            }
        }

        public bool hasAdditiveReferencePose
        {
            get
            {
                return this.Get("hasAdditiveReferencePose").boolValue;
            }
            set
            {
                this.Get("hasAdditiveReferencePose").boolValue = value;
            }
        }

        public bool heightFromFeet
        {
            get
            {
                return this.Get("heightFromFeet").boolValue;
            }
            set
            {
                this.Get("heightFromFeet").boolValue = value;
            }
        }

        public bool keepOriginalOrientation
        {
            get
            {
                return this.Get("keepOriginalOrientation").boolValue;
            }
            set
            {
                this.Get("keepOriginalOrientation").boolValue = value;
            }
        }

        public bool keepOriginalPositionXZ
        {
            get
            {
                return this.Get("keepOriginalPositionXZ").boolValue;
            }
            set
            {
                this.Get("keepOriginalPositionXZ").boolValue = value;
            }
        }

        public bool keepOriginalPositionY
        {
            get
            {
                return this.Get("keepOriginalPositionY").boolValue;
            }
            set
            {
                this.Get("keepOriginalPositionY").boolValue = value;
            }
        }

        public float lastFrame
        {
            get
            {
                return this.Get("lastFrame").floatValue;
            }
            set
            {
                this.Get("lastFrame").floatValue = value;
            }
        }

        public float level
        {
            get
            {
                return this.Get("level").floatValue;
            }
            set
            {
                this.Get("level").floatValue = value;
            }
        }

        public bool loop
        {
            get
            {
                return this.Get("loop").boolValue;
            }
            set
            {
                this.Get("loop").boolValue = value;
            }
        }

        public bool loopBlend
        {
            get
            {
                return this.Get("loopBlend").boolValue;
            }
            set
            {
                this.Get("loopBlend").boolValue = value;
            }
        }

        public bool loopBlendOrientation
        {
            get
            {
                return this.Get("loopBlendOrientation").boolValue;
            }
            set
            {
                this.Get("loopBlendOrientation").boolValue = value;
            }
        }

        public bool loopBlendPositionXZ
        {
            get
            {
                return this.Get("loopBlendPositionXZ").boolValue;
            }
            set
            {
                this.Get("loopBlendPositionXZ").boolValue = value;
            }
        }

        public bool loopBlendPositionY
        {
            get
            {
                return this.Get("loopBlendPositionY").boolValue;
            }
            set
            {
                this.Get("loopBlendPositionY").boolValue = value;
            }
        }

        public bool loopTime
        {
            get
            {
                return this.Get("loopTime").boolValue;
            }
            set
            {
                this.Get("loopTime").boolValue = value;
            }
        }

        public AvatarMask maskSource
        {
            get
            {
                return (this.Get("maskSource").objectReferenceValue as AvatarMask);
            }
            set
            {
                this.Get("maskSource").objectReferenceValue = value;
            }
        }

        public SerializedProperty maskSourceProperty
        {
            get
            {
                return this.Get("maskSource");
            }
        }

        public ClipAnimationMaskType maskType
        {
            get
            {
                return (ClipAnimationMaskType) this.Get("maskType").intValue;
            }
            set
            {
                this.Get("maskType").intValue = (int) value;
            }
        }

        public SerializedProperty maskTypeProperty
        {
            get
            {
                return this.Get("maskType");
            }
        }

        public bool mirror
        {
            get
            {
                return this.Get("mirror").boolValue;
            }
            set
            {
                this.Get("mirror").boolValue = value;
            }
        }

        public string name
        {
            get
            {
                return this.Get("name").stringValue;
            }
            set
            {
                this.Get("name").stringValue = value;
            }
        }

        public float orientationOffsetY
        {
            get
            {
                return this.Get("orientationOffsetY").floatValue;
            }
            set
            {
                this.Get("orientationOffsetY").floatValue = value;
            }
        }

        public string takeName
        {
            get
            {
                return this.Get("takeName").stringValue;
            }
            set
            {
                this.Get("takeName").stringValue = value;
            }
        }

        public SerializedProperty transformMaskProperty
        {
            get
            {
                return this.Get("transformMask");
            }
        }

        public int wrapMode
        {
            get
            {
                return this.Get("wrapMode").intValue;
            }
            set
            {
                this.Get("wrapMode").intValue = value;
            }
        }
    }
}

