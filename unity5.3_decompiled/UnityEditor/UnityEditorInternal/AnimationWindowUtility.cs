namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEngine;

    internal static class AnimationWindowUtility
    {
        internal static string s_LastPathUsedForNewClip;

        public static bool AddClipToAnimationComponent(Animation animation, AnimationClip newClip)
        {
            SetClipAsLegacy(newClip);
            animation.AddClip(newClip, newClip.name);
            return true;
        }

        public static bool AddClipToAnimationPlayerComponent(Component animationPlayer, AnimationClip newClip)
        {
            if (animationPlayer is Animator)
            {
                return AddClipToAnimatorComponent(animationPlayer as Animator, newClip);
            }
            return ((animationPlayer is Animation) && AddClipToAnimationComponent(animationPlayer as Animation, newClip));
        }

        public static bool AddClipToAnimatorComponent(Animator animator, AnimationClip newClip)
        {
            AnimatorController effectiveAnimatorController = AnimatorController.GetEffectiveAnimatorController(animator);
            if (effectiveAnimatorController == null)
            {
                effectiveAnimatorController = AnimatorController.CreateAnimatorControllerForClip(newClip, animator.gameObject);
                AnimatorController.SetAnimatorController(animator, effectiveAnimatorController);
                return (effectiveAnimatorController != null);
            }
            ChildAnimatorState state = effectiveAnimatorController.layers[0].stateMachine.FindState(newClip.name);
            if (state.Equals(new ChildAnimatorState()))
            {
                effectiveAnimatorController.AddMotion(newClip);
            }
            else if ((state.state != null) && (state.state.motion == null))
            {
                state.state.motion = newClip;
            }
            else if ((state.state != null) && (state.state.motion != newClip))
            {
                effectiveAnimatorController.AddMotion(newClip);
            }
            return true;
        }

        public static AnimationWindowKeyframe AddKeyframeToCurve(AnimationWindowState state, AnimationWindowCurve curve, AnimationKeyTime time)
        {
            object currentValue = CurveBindingUtility.GetCurrentValue(state.activeRootGameObject, curve.binding);
            Type editorCurveValueType = CurveBindingUtility.GetEditorCurveValueType(state.activeRootGameObject, curve.binding);
            AnimationWindowKeyframe keyframe = AddKeyframeToCurve(curve, currentValue, editorCurveValueType, time);
            state.SaveCurve(curve);
            return keyframe;
        }

        public static AnimationWindowKeyframe AddKeyframeToCurve(AnimationWindowCurve curve, object value, Type type, AnimationKeyTime time)
        {
            AnimationWindowKeyframe keyframe = curve.FindKeyAtTime(time);
            if (keyframe != null)
            {
                keyframe.value = value;
                return keyframe;
            }
            AnimationWindowKeyframe key = new AnimationWindowKeyframe {
                time = time.time
            };
            if (curve.isPPtrCurve)
            {
                key.value = value;
                key.curve = curve;
                curve.AddKeyframe(key, time);
                return key;
            }
            if ((type == typeof(bool)) || (type == typeof(float)))
            {
                AnimationCurve curve2 = curve.ToAnimationCurve();
                Keyframe keyframe3 = new Keyframe(time.time, (float) value);
                if (type == typeof(bool))
                {
                    CurveUtility.SetKeyTangentMode(ref keyframe3, 0, TangentMode.Stepped);
                    CurveUtility.SetKeyTangentMode(ref keyframe3, 1, TangentMode.Stepped);
                    CurveUtility.SetKeyBroken(ref keyframe3, true);
                    key.m_TangentMode = keyframe3.tangentMode;
                    key.m_InTangent = float.PositiveInfinity;
                    key.m_OutTangent = float.PositiveInfinity;
                }
                else
                {
                    int keyIndex = curve2.AddKey(keyframe3);
                    if (keyIndex != -1)
                    {
                        CurveUtility.SetKeyModeFromContext(curve2, keyIndex);
                        Keyframe keyframe4 = curve2[keyIndex];
                        key.m_TangentMode = keyframe4.tangentMode;
                    }
                }
                key.value = value;
                key.curve = curve;
                curve.AddKeyframe(key, time);
            }
            return key;
        }

        public static void AddSelectedKeyframes(AnimationWindowState state, AnimationKeyTime time)
        {
            if (state.activeCurves.Count > 0)
            {
                foreach (AnimationWindowCurve curve in state.activeCurves)
                {
                    AddKeyframeToCurve(state, curve, time);
                }
            }
            else
            {
                foreach (AnimationWindowCurve curve2 in state.allCurves)
                {
                    AddKeyframeToCurve(state, curve2, time);
                }
            }
        }

        internal static AnimationClip AllocateAndSetupClip(bool useAnimator)
        {
            AnimationClip clip = new AnimationClip();
            if (useAnimator)
            {
                AnimationClipSettings animationClipSettings = AnimationUtility.GetAnimationClipSettings(clip);
                animationClipSettings.loopTime = true;
                AnimationUtility.SetAnimationClipSettingsNoDirty(clip, animationClipSettings);
            }
            return clip;
        }

        public static CurveSelection AnimationWindowKeyframeToCurveSelection(AnimationWindowKeyframe keyframe, CurveEditor curveEditor)
        {
            foreach (CurveWrapper wrapper in curveEditor.animationCurves)
            {
                if ((wrapper.binding == keyframe.curve.binding) && (keyframe.GetIndex() >= 0))
                {
                    return new CurveSelection(wrapper.id, curveEditor, keyframe.GetIndex());
                }
            }
            return null;
        }

        public static AnimationWindowCurve BestMatchForPaste(EditorCurveBinding binding, List<AnimationWindowCurve> curves)
        {
            foreach (AnimationWindowCurve curve in curves)
            {
                if (curve.binding == binding)
                {
                    return curve;
                }
            }
            foreach (AnimationWindowCurve curve2 in curves)
            {
                if (curve2.binding.propertyName == binding.propertyName)
                {
                    return curve2;
                }
            }
            if (curves.Count == 1)
            {
                return curves[0];
            }
            return null;
        }

        public static bool ContainsFloatKeyframes(List<AnimationWindowKeyframe> keyframes)
        {
            if ((keyframes != null) && (keyframes.Count != 0))
            {
                foreach (AnimationWindowKeyframe keyframe in keyframes)
                {
                    if (!keyframe.isPPtrCurve)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void ControllerChanged()
        {
            foreach (AnimEditor editor in AnimEditor.GetAllAnimationWindows())
            {
                editor.OnControllerChange();
            }
        }

        public static AnimationWindowCurve CreateDefaultCurve(AnimationClip clip, GameObject rootGameObject, EditorCurveBinding binding)
        {
            Type editorCurveValueType = CurveBindingUtility.GetEditorCurveValueType(rootGameObject, binding);
            AnimationWindowCurve curve = new AnimationWindowCurve(clip, binding, editorCurveValueType);
            object currentValue = CurveBindingUtility.GetCurrentValue(rootGameObject, binding);
            if (clip.length == 0f)
            {
                AddKeyframeToCurve(curve, currentValue, editorCurveValueType, AnimationKeyTime.Time(0f, clip.frameRate));
                return curve;
            }
            AddKeyframeToCurve(curve, currentValue, editorCurveValueType, AnimationKeyTime.Time(0f, clip.frameRate));
            AddKeyframeToCurve(curve, currentValue, editorCurveValueType, AnimationKeyTime.Time(clip.length, clip.frameRate));
            return curve;
        }

        public static void CreateDefaultCurves(AnimationWindowState state, EditorCurveBinding[] properties)
        {
            AnimationClip activeAnimationClip = state.activeAnimationClip;
            GameObject activeRootGameObject = state.activeRootGameObject;
            properties = RotationCurveInterpolation.ConvertRotationPropertiesToDefaultInterpolation(state.activeAnimationClip, properties);
            foreach (EditorCurveBinding binding in properties)
            {
                state.SaveCurve(CreateDefaultCurve(activeAnimationClip, activeRootGameObject, binding));
            }
        }

        internal static AnimationClip CreateNewClip(string gameObjectName)
        {
            string message = string.Format("Create a new animation for the game object '{0}':", gameObjectName);
            string activeFolderPath = ProjectWindowUtil.GetActiveFolderPath();
            if (s_LastPathUsedForNewClip != null)
            {
                string directoryName = Path.GetDirectoryName(s_LastPathUsedForNewClip);
                if ((directoryName != null) && Directory.Exists(directoryName))
                {
                    activeFolderPath = directoryName;
                }
            }
            string clipPath = EditorUtility.SaveFilePanelInProject("Create New Animation", "New Animation", "anim", message, activeFolderPath);
            if (clipPath == string.Empty)
            {
                return null;
            }
            return CreateNewClipAtPath(clipPath);
        }

        internal static AnimationClip CreateNewClipAtPath(string clipPath)
        {
            s_LastPathUsedForNewClip = clipPath;
            AnimationClip clip = new AnimationClip();
            AnimationClipSettings animationClipSettings = AnimationUtility.GetAnimationClipSettings(clip);
            animationClipSettings.loopTime = true;
            AnimationUtility.SetAnimationClipSettingsNoDirty(clip, animationClipSettings);
            AnimationClip dest = AssetDatabase.LoadMainAssetAtPath(clipPath) as AnimationClip;
            if (dest != null)
            {
                EditorUtility.CopySerialized(clip, dest);
                AssetDatabase.SaveAssets();
                Object.DestroyImmediate(clip);
                return dest;
            }
            AssetDatabase.CreateAsset(clip, clipPath);
            return clip;
        }

        public static bool CurveExists(EditorCurveBinding binding, AnimationWindowCurve[] curves)
        {
            foreach (AnimationWindowCurve curve in curves)
            {
                if (((binding.propertyName == curve.binding.propertyName) && (binding.type == curve.binding.type)) && (binding.path == curve.binding.path))
                {
                    return true;
                }
            }
            return false;
        }

        public static AnimationWindowKeyframe CurveSelectionToAnimationWindowKeyframe(CurveSelection curveSelection, List<AnimationWindowCurve> allCurves)
        {
            foreach (AnimationWindowCurve curve in allCurves)
            {
                if ((curve.binding == curveSelection.curveWrapper.binding) && (curve.m_Keyframes.Count > curveSelection.key))
                {
                    return curve.m_Keyframes[curveSelection.key];
                }
            }
            return null;
        }

        public static void DrawEndOfClip(Rect rect, float endOfClipPixel)
        {
            Rect rect2 = new Rect(Mathf.Max(endOfClipPixel, rect.xMin), rect.yMin, rect.width, rect.height);
            Vector3[] corners = new Vector3[] { new Vector3(rect2.xMin, rect2.yMin), new Vector3(rect2.xMax, rect2.yMin), new Vector3(rect2.xMax, rect2.yMax), new Vector3(rect2.xMin, rect2.yMax) };
            Color color = !EditorGUIUtility.isProSkin ? Color.gray.AlphaMultiplied(0.32f) : Color.gray.RGBMultiplied((float) 0.3f).AlphaMultiplied(0.5f);
            Color color2 = !EditorGUIUtility.isProSkin ? Color.white.RGBMultiplied((float) 0.4f) : Color.white.RGBMultiplied((float) 0.4f);
            DrawRect(corners, color);
            TimeArea.DrawVerticalLine(corners[0].x, corners[0].y, corners[3].y, color2);
            DrawLine(corners[0], corners[3] + new Vector3(0f, -1f, 0f), color2);
        }

        private static void DrawLine(Vector2 p1, Vector2 p2, Color color)
        {
            HandleUtility.ApplyWireMaterial();
            GL.PushMatrix();
            GL.MultMatrix(Handles.matrix);
            GL.Begin(1);
            GL.Color(color);
            GL.Vertex((Vector3) p1);
            GL.Vertex((Vector3) p2);
            GL.End();
            GL.PopMatrix();
        }

        public static void DrawPlayHead(float positionX, float minY, float maxY, float alpha)
        {
            TimeArea.DrawVerticalLine(positionX, minY, maxY, Color.red.AlphaMultiplied(alpha));
        }

        private static void DrawRect(Vector3[] corners, Color color)
        {
            HandleUtility.ApplyWireMaterial();
            GL.PushMatrix();
            GL.MultMatrix(Handles.matrix);
            GL.Begin(7);
            GL.Color(color);
            GL.Vertex(corners[0]);
            GL.Vertex(corners[1]);
            GL.Vertex(corners[2]);
            GL.Vertex(corners[3]);
            GL.End();
            GL.PopMatrix();
        }

        public static void DrawVerticalSplitLine(Vector2 start, Vector2 end)
        {
            TimeArea.DrawVerticalLine(start.x, start.y, end.y, !EditorGUIUtility.isProSkin ? Color.white.RGBMultiplied((float) 0.6f) : Color.white.RGBMultiplied((float) 0.15f));
        }

        public static Component EnsureActiveAnimationPlayer(GameObject animatedObject)
        {
            Component closestAnimationPlayerComponentInParents = GetClosestAnimationPlayerComponentInParents(animatedObject.transform);
            if (closestAnimationPlayerComponentInParents == null)
            {
                return animatedObject.AddComponent<Animator>();
            }
            return closestAnimationPlayerComponentInParents;
        }

        private static bool EnsureAnimationPlayerHasClip(Component animationPlayer)
        {
            if (animationPlayer == null)
            {
                return false;
            }
            if (AnimationUtility.GetAnimationClips(animationPlayer.gameObject).Length > 0)
            {
                return true;
            }
            AnimationClip newClip = CreateNewClip(animationPlayer.gameObject.name);
            if (newClip == null)
            {
                return false;
            }
            AnimationMode.StopAnimationMode();
            return AddClipToAnimationPlayerComponent(animationPlayer, newClip);
        }

        public static List<AnimationWindowCurve> FilterCurves(AnimationWindowCurve[] curves, string path, bool entireHierarchy)
        {
            List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
            foreach (AnimationWindowCurve curve in curves)
            {
                if (curve.path.Equals(path) || (entireHierarchy && curve.path.Contains(path)))
                {
                    list.Add(curve);
                }
            }
            return list;
        }

        public static List<AnimationWindowCurve> FilterCurves(AnimationWindowCurve[] curves, string path, Type animatableObjectType)
        {
            List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
            foreach (AnimationWindowCurve curve in curves)
            {
                if (curve.path.Equals(path) && (curve.type == animatableObjectType))
                {
                    list.Add(curve);
                }
            }
            return list;
        }

        public static List<AnimationWindowCurve> FilterCurves(AnimationWindowCurve[] curves, string path, Type animatableObjectType, string propertyName)
        {
            List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
            string propertyGroupName = GetPropertyGroupName(propertyName);
            bool flag = propertyGroupName == propertyName;
            foreach (AnimationWindowCurve curve in curves)
            {
                bool flag2 = !flag ? curve.propertyName.Equals(propertyName) : GetPropertyGroupName(curve.propertyName).Equals(propertyGroupName);
                if ((curve.path.Equals(path) && (curve.type == animatableObjectType)) && flag2)
                {
                    list.Add(curve);
                }
            }
            return list;
        }

        public static bool ForceGrouping(EditorCurveBinding binding)
        {
            if (binding.type == typeof(Transform))
            {
                return true;
            }
            if (binding.type == typeof(RectTransform))
            {
                string propertyGroupName = GetPropertyGroupName(binding.propertyName);
                return (((((propertyGroupName == "m_LocalPosition") || (propertyGroupName == "m_LocalScale")) || ((propertyGroupName == "m_LocalRotation") || (propertyGroupName == "localEulerAnglesBaked"))) || (propertyGroupName == "localEulerAngles")) || (propertyGroupName == "localEulerAnglesRaw"));
            }
            return (typeof(Renderer).IsAssignableFrom(binding.type) && (GetPropertyGroupName(binding.propertyName) == "material._Color"));
        }

        internal static Rect FromToRect(Vector2 start, Vector2 end)
        {
            Rect rect = new Rect(start.x, start.y, end.x - start.x, end.y - start.y);
            if (rect.width < 0f)
            {
                rect.x += rect.width;
                rect.width = -rect.width;
            }
            if (rect.height < 0f)
            {
                rect.y += rect.height;
                rect.height = -rect.height;
            }
            return rect;
        }

        public static bool GameObjectIsAnimatable(GameObject gameObject, AnimationClip animationClip)
        {
            if (gameObject == null)
            {
                return false;
            }
            if ((gameObject.hideFlags & HideFlags.NotEditable) != HideFlags.None)
            {
                return false;
            }
            if (EditorUtility.IsPersistent(gameObject))
            {
                return false;
            }
            return ((animationClip == null) || (((animationClip.hideFlags & HideFlags.NotEditable) == HideFlags.None) && AssetDatabase.IsOpenForEdit(animationClip)));
        }

        public static List<EditorCurveBinding> GetAnimatableProperties(GameObject root, GameObject gameObject, Type valueType)
        {
            EditorCurveBinding[] animatableBindings = AnimationUtility.GetAnimatableBindings(root, gameObject);
            List<EditorCurveBinding> list = new List<EditorCurveBinding>();
            foreach (EditorCurveBinding binding in animatableBindings)
            {
                if (AnimationUtility.GetEditorCurveValueType(root, binding) == valueType)
                {
                    list.Add(binding);
                }
            }
            return list;
        }

        public static List<EditorCurveBinding> GetAnimatableProperties(GameObject root, GameObject gameObject, Type objectType, Type valueType)
        {
            EditorCurveBinding[] animatableBindings = AnimationUtility.GetAnimatableBindings(root, gameObject);
            List<EditorCurveBinding> list = new List<EditorCurveBinding>();
            foreach (EditorCurveBinding binding in animatableBindings)
            {
                if ((binding.type == objectType) && (AnimationUtility.GetEditorCurveValueType(root, binding) == valueType))
                {
                    list.Add(binding);
                }
            }
            return list;
        }

        public static Animation GetClosestAnimationInParents(Transform tr)
        {
            while (tr.GetComponent<Animation>() == null)
            {
                if (tr == tr.root)
                {
                    return null;
                }
                tr = tr.parent;
            }
            return tr.GetComponent<Animation>();
        }

        public static Component GetClosestAnimationPlayerComponentInParents(Transform tr)
        {
            Animator closestAnimatorInParents = GetClosestAnimatorInParents(tr);
            if (closestAnimatorInParents != null)
            {
                return closestAnimatorInParents;
            }
            Animation closestAnimationInParents = GetClosestAnimationInParents(tr);
            if (closestAnimationInParents != null)
            {
                return closestAnimationInParents;
            }
            return null;
        }

        public static Animator GetClosestAnimatorInParents(Transform tr)
        {
            while (tr.GetComponent<Animator>() == null)
            {
                if (tr == tr.root)
                {
                    return null;
                }
                tr = tr.parent;
            }
            return tr.GetComponent<Animator>();
        }

        public static int GetComponentIndex(string name)
        {
            if ((name.Length >= 3) && (name[name.Length - 2] == '.'))
            {
                switch (name[name.Length - 1])
                {
                    case 'r':
                        return 0;

                    case 'w':
                        return 3;

                    case 'x':
                        return 0;

                    case 'y':
                        return 1;

                    case 'z':
                        return 2;

                    case 'a':
                        return 3;

                    case 'b':
                        return 2;

                    case 'g':
                        return 1;
                }
            }
            return -1;
        }

        public static object GetCurrentValue(GameObject rootGameObject, EditorCurveBinding curveBinding)
        {
            float num;
            if (curveBinding.isPPtrCurve)
            {
                Object obj2;
                AnimationUtility.GetObjectReferenceValue(rootGameObject, curveBinding, out obj2);
                return obj2;
            }
            AnimationUtility.GetFloatValue(rootGameObject, curveBinding, out num);
            return num;
        }

        public static CurveWrapper GetCurveWrapper(AnimationWindowCurve curve, AnimationClip clip)
        {
            CurveWrapper wrapper = new CurveWrapper {
                renderer = new NormalCurveRenderer(curve.ToAnimationCurve())
            };
            wrapper.renderer.SetWrap(WrapMode.Once, !clip.isLooping ? WrapMode.Once : WrapMode.Loop);
            wrapper.renderer.SetCustomRange(clip.startTime, clip.stopTime);
            wrapper.binding = curve.binding;
            wrapper.id = CurveUtility.GetCurveID(clip, curve.binding);
            wrapper.color = CurveUtility.GetPropertyColor(curve.propertyName);
            wrapper.hidden = false;
            return wrapper;
        }

        public static float GetNextKeyframeTime(AnimationWindowCurve[] curves, float currentTime, float frameRate)
        {
            float maxValue = float.MaxValue;
            float num2 = currentTime + (1f / frameRate);
            bool flag = false;
            foreach (AnimationWindowCurve curve in curves)
            {
                foreach (AnimationWindowKeyframe keyframe in curve.m_Keyframes)
                {
                    if ((keyframe.time < maxValue) && (keyframe.time > currentTime))
                    {
                        maxValue = Math.Max(keyframe.time, num2);
                        flag = true;
                    }
                }
            }
            return (!flag ? currentTime : maxValue);
        }

        public static string GetNicePropertyDisplayName(Type animatableObjectType, string propertyName)
        {
            if (ShouldPrefixWithTypeName(animatableObjectType, propertyName))
            {
                return (ObjectNames.NicifyVariableName(animatableObjectType.Name) + "." + GetPropertyDisplayName(propertyName));
            }
            return GetPropertyDisplayName(propertyName);
        }

        public static string GetNicePropertyGroupDisplayName(Type animatableObjectType, string propertyGroupName)
        {
            if (ShouldPrefixWithTypeName(animatableObjectType, propertyGroupName))
            {
                return (ObjectNames.NicifyVariableName(animatableObjectType.Name) + "." + NicifyPropertyGroupName(animatableObjectType, propertyGroupName));
            }
            return NicifyPropertyGroupName(animatableObjectType, propertyGroupName);
        }

        public static float GetPreviousKeyframeTime(AnimationWindowCurve[] curves, float currentTime, float frameRate)
        {
            float minValue = float.MinValue;
            float b = Mathf.Max((float) 0f, (float) (currentTime - (1f / frameRate)));
            bool flag = false;
            foreach (AnimationWindowCurve curve in curves)
            {
                foreach (AnimationWindowKeyframe keyframe in curve.m_Keyframes)
                {
                    if ((keyframe.time > minValue) && (keyframe.time < currentTime))
                    {
                        minValue = Mathf.Min(keyframe.time, b);
                        flag = true;
                    }
                }
            }
            return (!flag ? currentTime : minValue);
        }

        public static string GetPropertyDisplayName(string propertyName)
        {
            propertyName = propertyName.Replace("m_LocalPosition", "Position");
            propertyName = propertyName.Replace("m_LocalScale", "Scale");
            propertyName = propertyName.Replace("m_LocalRotation", "Rotation");
            propertyName = propertyName.Replace("localEulerAnglesBaked", "Rotation");
            propertyName = propertyName.Replace("localEulerAnglesRaw", "Rotation");
            propertyName = propertyName.Replace("localEulerAngles", "Rotation");
            propertyName = propertyName.Replace("m_Materials.Array.data", "Material Reference");
            propertyName = ObjectNames.NicifyVariableName(propertyName);
            propertyName = propertyName.Replace("m_", string.Empty);
            return propertyName;
        }

        public static string GetPropertyGroupName(string propertyName)
        {
            if (GetComponentIndex(propertyName) != -1)
            {
                return propertyName.Substring(0, propertyName.Length - 2);
            }
            return propertyName;
        }

        public static int GetPropertyNodeID(string path, Type type, string propertyName)
        {
            return (path + type.Name + propertyName).GetHashCode();
        }

        public static EditorCurveBinding GetRenamedBinding(EditorCurveBinding binding, string newPath)
        {
            return new EditorCurveBinding { path = newPath, propertyName = binding.propertyName, type = binding.type };
        }

        public static bool InitializeGameobjectForAnimation(GameObject animatedObject)
        {
            Component closestAnimationPlayerComponentInParents = GetClosestAnimationPlayerComponentInParents(animatedObject.transform);
            if (closestAnimationPlayerComponentInParents != null)
            {
                return EnsureAnimationPlayerHasClip(closestAnimationPlayerComponentInParents);
            }
            AnimationClip newClip = CreateNewClip(animatedObject.name);
            if (newClip == null)
            {
                return false;
            }
            closestAnimationPlayerComponentInParents = EnsureActiveAnimationPlayer(animatedObject);
            bool flag = AddClipToAnimationPlayerComponent(closestAnimationPlayerComponentInParents, newClip);
            if (!flag)
            {
                Object.DestroyImmediate(closestAnimationPlayerComponentInParents);
            }
            return flag;
        }

        public static bool IsCurveCreated(AnimationClip clip, EditorCurveBinding binding)
        {
            if (binding.isPPtrCurve)
            {
                return (AnimationUtility.GetObjectReferenceCurve(clip, binding) != null);
            }
            if (IsRectTransformPosition(binding))
            {
                binding.propertyName = binding.propertyName.Replace(".x", ".z").Replace(".y", ".z");
            }
            return (AnimationUtility.GetEditorCurve(clip, binding) != null);
        }

        public static bool IsNodeAmbiguous(AnimationWindowHierarchyNode node, GameObject rootGameObject)
        {
            if (rootGameObject != null)
            {
                if (node.binding.HasValue)
                {
                    return AnimationUtility.AmbiguousBinding(node.binding.Value.path, node.binding.Value.m_ClassID, rootGameObject.transform);
                }
                if (node.hasChildren)
                {
                    foreach (TreeViewItem item in node.children)
                    {
                        return IsNodeAmbiguous(item as AnimationWindowHierarchyNode, rootGameObject);
                    }
                }
            }
            return false;
        }

        public static bool IsNodeLeftOverCurve(AnimationWindowHierarchyNode node, GameObject rootGameObject)
        {
            if (rootGameObject != null)
            {
                if (node.binding.HasValue)
                {
                    return (AnimationUtility.GetEditorCurveValueType(rootGameObject, node.binding.Value) == null);
                }
                if (node.hasChildren)
                {
                    foreach (TreeViewItem item in node.children)
                    {
                        return IsNodeLeftOverCurve(item as AnimationWindowHierarchyNode, rootGameObject);
                    }
                }
            }
            return false;
        }

        public static bool IsRectTransformPosition(EditorCurveBinding curveBinding)
        {
            return ((curveBinding.type == typeof(RectTransform)) && (GetPropertyGroupName(curveBinding.propertyName) == "m_LocalPosition"));
        }

        public static bool IsTransformType(Type type)
        {
            return ((type == typeof(Transform)) || (type == typeof(RectTransform)));
        }

        public static string NicifyPropertyGroupName(Type animatableObjectType, string propertyGroupName)
        {
            string str = GetPropertyGroupName(GetPropertyDisplayName(propertyGroupName));
            if ((animatableObjectType == typeof(RectTransform)) & str.Equals("Position"))
            {
                str = "Position (Z)";
            }
            return str;
        }

        public static void RenameCurvePath(AnimationWindowCurve curve, EditorCurveBinding newBinding, AnimationClip clip)
        {
            AnimationUtility.SetEditorCurve(clip, curve.binding, null);
            AnimationUtility.SetEditorCurve(clip, newBinding, curve.ToAnimationCurve());
        }

        private static void SetClipAsLegacy(AnimationClip clip)
        {
            SerializedObject obj2 = new SerializedObject(clip);
            obj2.FindProperty("m_Legacy").boolValue = true;
            obj2.ApplyModifiedProperties();
        }

        public static bool ShouldPrefixWithTypeName(Type animatableObjectType, string propertyName)
        {
            if ((animatableObjectType == typeof(Transform)) || (animatableObjectType == typeof(RectTransform)))
            {
                return false;
            }
            if ((animatableObjectType == typeof(SpriteRenderer)) && (propertyName == "m_Sprite"))
            {
                return false;
            }
            return true;
        }

        public static bool ShouldShowAnimationWindowCurve(EditorCurveBinding curveBinding)
        {
            if (IsTransformType(curveBinding.type))
            {
                return !curveBinding.propertyName.EndsWith(".w");
            }
            return true;
        }

        public static void SyncTimeArea(TimeArea from, TimeArea to)
        {
            to.SetDrawRectHack(from.drawRect);
            to.m_Scale = new Vector2(from.m_Scale.x, to.m_Scale.y);
            to.m_Translation = new Vector2(from.m_Translation.x, to.m_Translation.y);
            to.EnforceScaleAndRange();
        }
    }
}

