namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    [Serializable]
    internal class AnimationWindowState : ScriptableObject, IPlayHead
    {
        [CompilerGenerated]
        private static Comparison<AnimationWindowKeyframe> <>f__am$cache1A;
        [SerializeField]
        public AnimEditor animEditor;
        [NonSerialized]
        public AnimationWindowHierarchyDataSource hierarchyData;
        [SerializeField]
        public AnimationWindowHierarchyState hierarchyState;
        [SerializeField]
        private AnimationClip m_ActiveAnimationClip;
        private List<AnimationWindowCurve> m_ActiveCurvesCache;
        private List<CurveWrapper> m_ActiveCurveWrappersCache;
        [SerializeField]
        private GameObject m_ActiveGameObject;
        private AnimationWindowKeyframe m_ActiveKeyframeCache;
        [SerializeField]
        private int m_ActiveKeyframeHash;
        private List<AnimationWindowCurve> m_AllCurvesCache;
        [SerializeField]
        private float m_CurrentTime;
        private List<DopeLine> m_dopelinesCache;
        [NonSerialized]
        public bool m_FrameCurveEditor;
        private EditorCurveBinding? m_lastAddedCurveBinding;
        [SerializeField]
        private bool m_Locked;
        private HashSet<int> m_ModifiedCurves = new HashSet<int>();
        private GameObject m_PreviousActiveRootGameObject;
        private int m_PreviousRefreshHash;
        private RefreshType m_Refresh;
        [SerializeField]
        private HashSet<int> m_SelectedKeyHashes;
        private List<AnimationWindowKeyframe> m_SelectedKeysCache;
        [SerializeField]
        private TimeArea m_timeArea;
        [NonSerialized]
        public Action onClipSelectionChanged;
        public Action<float> onFrameRateChange;
        private static List<AnimationWindowKeyframe> s_KeyframeClipboard;
        [SerializeField]
        public bool showCurveEditor;

        public bool AnyKeyIsSelected(DopeLine dopeline)
        {
            foreach (AnimationWindowKeyframe keyframe in dopeline.keys)
            {
                if (this.KeyIsSelected(keyframe))
                {
                    return true;
                }
            }
            return false;
        }

        public void ClearHierarchySelection()
        {
            this.hierarchyState.selectedIDs.Clear();
        }

        public void ClearKeySelections()
        {
            this.selectedKeyHashes.Clear();
            this.m_SelectedKeysCache = null;
        }

        public void ClearSelections()
        {
            this.ClearKeySelections();
            this.ClearHierarchySelection();
        }

        public void CopyAllActiveCurves()
        {
            foreach (AnimationWindowCurve curve in this.activeCurves)
            {
                foreach (AnimationWindowKeyframe keyframe in curve.m_Keyframes)
                {
                    s_KeyframeClipboard.Add(new AnimationWindowKeyframe(keyframe));
                }
            }
        }

        public void CopyKeys()
        {
            if (s_KeyframeClipboard == null)
            {
                s_KeyframeClipboard = new List<AnimationWindowKeyframe>();
            }
            float maxValue = float.MaxValue;
            s_KeyframeClipboard.Clear();
            foreach (AnimationWindowKeyframe keyframe in this.selectedKeys)
            {
                s_KeyframeClipboard.Add(new AnimationWindowKeyframe(keyframe));
                if (keyframe.time < maxValue)
                {
                    maxValue = keyframe.time;
                }
            }
            if (s_KeyframeClipboard.Count > 0)
            {
                foreach (AnimationWindowKeyframe keyframe2 in s_KeyframeClipboard)
                {
                    keyframe2.time -= maxValue;
                }
            }
            else
            {
                this.CopyAllActiveCurves();
            }
        }

        private void CurveWasModified(AnimationClip clip, EditorCurveBinding binding, AnimationUtility.CurveModifiedType type)
        {
            if (clip == this.activeAnimationClip)
            {
                if (type == AnimationUtility.CurveModifiedType.CurveModified)
                {
                    bool flag = false;
                    int hashCode = binding.GetHashCode();
                    foreach (AnimationWindowCurve curve in this.allCurves)
                    {
                        int item = curve.binding.GetHashCode();
                        if (item == hashCode)
                        {
                            this.m_ModifiedCurves.Add(item);
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        this.refresh = RefreshType.CurvesOnly;
                    }
                    else
                    {
                        this.m_lastAddedCurveBinding = new EditorCurveBinding?(binding);
                        this.refresh = RefreshType.Everything;
                    }
                }
                else
                {
                    this.refresh = RefreshType.Everything;
                }
            }
        }

        public void DeleteSelectedKeys()
        {
            if (this.selectedKeys.Count != 0)
            {
                foreach (AnimationWindowKeyframe keyframe in this.selectedKeys)
                {
                    this.UnselectKey(keyframe);
                    keyframe.curve.m_Keyframes.Remove(keyframe);
                    this.SaveCurve(keyframe.curve);
                }
            }
        }

        public string FormatFrame(int frame, int frameDigits)
        {
            int num = frame / ((int) this.frameRate);
            float num2 = ((float) frame) % this.frameRate;
            return (num.ToString() + ":" + num2.ToString().PadLeft(frameDigits, '0'));
        }

        public float FrameDeltaToPixel(Rect rect)
        {
            return (rect.width / this.visibleFrameSpan);
        }

        public float FrameToPixel(float i, Rect rect)
        {
            return (((i - this.minVisibleFrame) * rect.width) / this.visibleFrameSpan);
        }

        public float FrameToTime(float frame)
        {
            return (frame / this.frameRate);
        }

        public float FrameToTimeCeiling(float frame)
        {
            return ((frame + 0.5f) / this.frameRate);
        }

        public float FrameToTimeFloor(float frame)
        {
            return ((frame - 0.5f) / this.frameRate);
        }

        public List<AnimationWindowCurve> GetAffectedCurves(List<AnimationWindowKeyframe> keyframes)
        {
            List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
            foreach (AnimationWindowKeyframe keyframe in keyframes)
            {
                if (!list.Contains(keyframe.curve))
                {
                    list.Add(keyframe.curve);
                }
            }
            return list;
        }

        public List<DopeLine> GetAffectedDopelines(List<AnimationWindowKeyframe> keyframes)
        {
            List<DopeLine> list = new List<DopeLine>();
            foreach (AnimationWindowCurve curve in this.GetAffectedCurves(keyframes))
            {
                foreach (DopeLine line in this.dopelines)
                {
                    if (!list.Contains(line) && line.m_Curves.Contains<AnimationWindowCurve>(curve))
                    {
                        list.Add(line);
                    }
                }
            }
            return list;
        }

        public List<int> GetAffectedHierarchyIDs(List<AnimationWindowKeyframe> keyframes)
        {
            List<int> list = new List<int>();
            foreach (DopeLine line in this.GetAffectedDopelines(keyframes))
            {
                if (!list.Contains(line.m_HierarchyNodeID))
                {
                    list.Add(line.m_HierarchyNodeID);
                }
            }
            return list;
        }

        public List<AnimationWindowKeyframe> GetAggregateKeys(AnimationWindowHierarchyNode hierarchyNode)
        {
            <GetAggregateKeys>c__AnonStorey40 storey = new <GetAggregateKeys>c__AnonStorey40 {
                hierarchyNode = hierarchyNode
            };
            DopeLine line = this.dopelines.FirstOrDefault<DopeLine>(new Func<DopeLine, bool>(storey.<>m__6F));
            if (line == null)
            {
                return null;
            }
            return line.keys;
        }

        public List<AnimationWindowCurve> GetCurves(AnimationWindowHierarchyNode hierarchyNode, bool entireHierarchy)
        {
            return AnimationWindowUtility.FilterCurves(this.allCurves.ToArray(), hierarchyNode.path, hierarchyNode.animatableObjectType, hierarchyNode.propertyName);
        }

        public DopeLine GetDopeline(int selectedInstanceID)
        {
            foreach (DopeLine line in this.dopelines)
            {
                if (line.m_HierarchyNodeID == selectedInstanceID)
                {
                    return line;
                }
            }
            return null;
        }

        private int GetRefreshHash()
        {
            return ((((((this.activeAnimationClip == null) ? 0 : this.activeAnimationClip.GetHashCode()) ^ ((this.activeRootGameObject == null) ? 0 : this.activeRootGameObject.GetHashCode())) ^ ((this.hierarchyState == null) ? 0 : this.hierarchyState.expandedIDs.Count)) ^ ((this.hierarchyState == null) ? 0 : this.hierarchyState.GetTallInstancesCount())) ^ (!this.showCurveEditor ? 0 : 1));
        }

        public void HandleHierarchySelectionChanged(int[] selectedInstanceIDs, bool triggerSceneSelectionSync)
        {
            this.m_ActiveCurvesCache = null;
            this.m_FrameCurveEditor = true;
            if (triggerSceneSelectionSync)
            {
                this.SyncSceneSelection(selectedInstanceIDs);
            }
        }

        public bool KeyIsSelected(AnimationWindowKeyframe keyframe)
        {
            return this.selectedKeyHashes.Contains(keyframe.GetHash());
        }

        public void MoveSelectedKeys(float deltaTime)
        {
            this.MoveSelectedKeys(deltaTime, false);
        }

        public void MoveSelectedKeys(float deltaTime, bool snapToFrame)
        {
            this.MoveSelectedKeys(deltaTime, snapToFrame, true);
        }

        public void MoveSelectedKeys(float deltaTime, bool snapToFrame, bool saveToClip)
        {
            List<AnimationWindowKeyframe> currentSelectedKeys = new List<AnimationWindowKeyframe>(this.selectedKeys);
            foreach (AnimationWindowKeyframe keyframe in currentSelectedKeys)
            {
                keyframe.time += deltaTime;
                if (snapToFrame)
                {
                    keyframe.time = this.SnapToFrame(keyframe.time, !saveToClip);
                }
            }
            if (saveToClip)
            {
                this.SaveSelectedKeys(currentSelectedKeys);
            }
            this.ClearKeySelections();
            foreach (AnimationWindowKeyframe keyframe2 in currentSelectedKeys)
            {
                this.SelectKey(keyframe2);
            }
        }

        public void OnControllerChange()
        {
            AnimationClip[] animationClips = AnimationUtility.GetAnimationClips(this.activeRootGameObject);
            bool flag = (animationClips != null) && (animationClips.Length > 0);
            this.activeAnimationClip = !flag ? null : animationClips[0];
            this.refresh = RefreshType.Everything;
        }

        public void OnDisable()
        {
            CurveBindingUtility.Cleanup();
            this.recording = false;
            this.playing = false;
            AnimationUtility.onCurveWasModified = (AnimationUtility.OnCurveWasModified) Delegate.Remove(AnimationUtility.onCurveWasModified, new AnimationUtility.OnCurveWasModified(this.CurveWasModified));
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
        }

        public void OnEnable()
        {
            base.hideFlags = HideFlags.HideAndDontSave;
            AnimationUtility.onCurveWasModified = (AnimationUtility.OnCurveWasModified) Delegate.Combine(AnimationUtility.onCurveWasModified, new AnimationUtility.OnCurveWasModified(this.CurveWasModified));
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
        }

        public void OnGUI()
        {
            this.RefreshHashCheck();
            this.Refresh();
        }

        public void OnHierarchySelectionChanged(int[] selectedInstanceIDs)
        {
            this.HandleHierarchySelectionChanged(selectedInstanceIDs, true);
            foreach (DopeLine line in this.dopelines)
            {
                if (selectedInstanceIDs.Contains<int>(line.m_HierarchyNodeID))
                {
                    this.SelectKeysFromDopeline(line);
                }
                else
                {
                    this.UnselectKeysFromDopeline(line);
                }
            }
        }

        private void OnNewCurveAdded(EditorCurveBinding newCurve)
        {
            string propertyGroupName = AnimationWindowUtility.GetPropertyGroupName(newCurve.propertyName);
            int hierarchyNodeID = AnimationWindowUtility.GetPropertyNodeID(newCurve.path, newCurve.type, propertyGroupName);
            this.SelectHierarchyItem(hierarchyNodeID, false, false);
            if (newCurve.isPPtrCurve)
            {
                this.hierarchyState.AddTallInstance(hierarchyNodeID);
            }
            this.m_lastAddedCurveBinding = null;
        }

        public void OnSelectionChange()
        {
            if (!this.m_Locked)
            {
                AnimationClip[] array = new AnimationClip[0];
                if ((this.activeRootGameObject != null) && !(Selection.activeObject is AnimationClip))
                {
                    array = AnimationUtility.GetAnimationClips(this.activeRootGameObject);
                    if ((this.activeAnimationClip == null) && (this.activeGameObject != null))
                    {
                        this.activeAnimationClip = (array.Length <= 0) ? null : array[0];
                    }
                    else if (!Array.Exists<AnimationClip>(array, x => x == this.activeAnimationClip))
                    {
                        this.activeAnimationClip = (array.Length <= 0) ? null : array[0];
                    }
                }
                else if (this.activeRootGameObject == null)
                {
                    this.m_ActiveAnimationClip = null;
                    this.onFrameRateChange(this.frameRate);
                }
                if (this.m_PreviousActiveRootGameObject != this.activeRootGameObject)
                {
                    this.recording = false;
                }
                this.m_PreviousActiveRootGameObject = this.activeRootGameObject;
            }
        }

        public void PasteKeys()
        {
            if (s_KeyframeClipboard == null)
            {
                s_KeyframeClipboard = new List<AnimationWindowKeyframe>();
            }
            HashSet<int> set = new HashSet<int>(this.m_SelectedKeyHashes);
            this.ClearSelections();
            AnimationWindowCurve curve = null;
            AnimationWindowCurve curve2 = null;
            float startTime = 0f;
            List<AnimationWindowCurve> source = new List<AnimationWindowCurve>();
            foreach (AnimationWindowKeyframe keyframe in s_KeyframeClipboard)
            {
                if (!source.Any<AnimationWindowCurve>() || (source.Last<AnimationWindowCurve>() != keyframe.curve))
                {
                    source.Add(keyframe.curve);
                }
            }
            bool flag = source.Count<AnimationWindowCurve>() == this.activeCurves.Count<AnimationWindowCurve>();
            int num2 = 0;
            foreach (AnimationWindowKeyframe keyframe2 in s_KeyframeClipboard)
            {
                if ((curve2 != null) && (keyframe2.curve != curve2))
                {
                    num2++;
                }
                AnimationWindowKeyframe item = new AnimationWindowKeyframe(keyframe2);
                if (flag)
                {
                    item.curve = this.activeCurves[num2];
                }
                else
                {
                    item.curve = AnimationWindowUtility.BestMatchForPaste(item.curve.binding, this.activeCurves);
                }
                if (item.curve == null)
                {
                    item.curve = new AnimationWindowCurve(this.activeAnimationClip, keyframe2.curve.binding, keyframe2.curve.type);
                    item.time = keyframe2.time;
                }
                item.time += this.time.time;
                if (item.curve != null)
                {
                    if (item.curve.HasKeyframe(AnimationKeyTime.Time(item.time, this.frameRate)))
                    {
                        item.curve.RemoveKeyframe(AnimationKeyTime.Time(item.time, this.frameRate));
                    }
                    if (curve == item.curve)
                    {
                        item.curve.RemoveKeysAtRange(startTime, item.time);
                    }
                    item.curve.m_Keyframes.Add(item);
                    this.SelectKey(item);
                    this.SaveCurve(item.curve);
                    curve = item.curve;
                    startTime = item.time;
                }
                curve2 = keyframe2.curve;
            }
            if (this.m_SelectedKeyHashes.Count == 0)
            {
                this.m_SelectedKeyHashes = set;
            }
            else
            {
                this.ResampleAnimation();
            }
        }

        public float PixelDeltaToTime(Rect rect)
        {
            return (this.visibleTimeSpan / rect.width);
        }

        public float PixelToTime(float pixel)
        {
            return this.PixelToTime(pixel, false);
        }

        public float PixelToTime(float pixel, bool snapToFrame)
        {
            float num = pixel - this.zeroTimePixel;
            if (snapToFrame)
            {
                return this.SnapToFrame(num / this.pixelPerSecond);
            }
            return (num / this.pixelPerSecond);
        }

        public float PixelToTime(float pixelX, Rect rect)
        {
            return (((pixelX * this.visibleTimeSpan) / rect.width) + this.minVisibleTime);
        }

        private UndoPropertyModification[] PostprocessAnimationRecordingModifications(UndoPropertyModification[] modifications)
        {
            return AnimationRecording.Process(this, modifications);
        }

        private void Refresh()
        {
            if (this.refresh == RefreshType.Everything)
            {
                CurveRendererCache.ClearCurveRendererCache();
                this.m_ActiveKeyframeCache = null;
                this.m_AllCurvesCache = null;
                this.m_ActiveCurvesCache = null;
                this.m_dopelinesCache = null;
                this.m_SelectedKeysCache = null;
                this.m_ActiveCurveWrappersCache = null;
                if (this.hierarchyData != null)
                {
                    this.hierarchyData.UpdateData();
                }
                if (this.m_lastAddedCurveBinding.HasValue)
                {
                    this.OnNewCurveAdded(this.m_lastAddedCurveBinding.Value);
                }
                if ((this.activeCurves.Count == 0) && (this.dopelines.Count > 0))
                {
                    this.SelectHierarchyItem(this.dopelines[0], false, false);
                }
                this.m_Refresh = RefreshType.None;
            }
            else if (this.refresh == RefreshType.CurvesOnly)
            {
                CurveRendererCache.ClearCurveRendererCache();
                this.m_ActiveKeyframeCache = null;
                this.m_ActiveCurvesCache = null;
                this.m_ActiveCurveWrappersCache = null;
                this.m_SelectedKeysCache = null;
                this.ReloadModifiedAnimationCurveCache();
                this.ReloadModifiedDopelineCache();
                this.m_Refresh = RefreshType.None;
                this.m_ModifiedCurves.Clear();
            }
            if (this.disabled && this.recording)
            {
                this.recording = false;
            }
        }

        private void RefreshHashCheck()
        {
            int refreshHash = this.GetRefreshHash();
            if (this.m_PreviousRefreshHash != refreshHash)
            {
                this.refresh = RefreshType.Everything;
                this.m_PreviousRefreshHash = refreshHash;
            }
        }

        private void ReloadModifiedAnimationCurveCache()
        {
            if (this.m_AllCurvesCache != null)
            {
                foreach (AnimationWindowCurve curve in this.m_AllCurvesCache)
                {
                    if (this.m_ModifiedCurves.Contains(curve.binding.GetHashCode()))
                    {
                        curve.LoadKeyframes(this.activeAnimationClip);
                    }
                }
            }
        }

        private void ReloadModifiedDopelineCache()
        {
            if (this.m_dopelinesCache != null)
            {
                foreach (DopeLine line in this.m_dopelinesCache)
                {
                    foreach (AnimationWindowCurve curve in line.m_Curves)
                    {
                        if (this.m_ModifiedCurves.Contains(curve.binding.GetHashCode()))
                        {
                            line.LoadKeyframes();
                        }
                    }
                }
            }
        }

        public void RemoveCurve(AnimationWindowCurve curve)
        {
            Undo.RegisterCompleteObjectUndo(this.activeAnimationClip, "Remove Curve");
            if (curve.isPPtrCurve)
            {
                AnimationUtility.SetObjectReferenceCurve(this.activeAnimationClip, curve.binding, null);
            }
            else
            {
                AnimationUtility.SetEditorCurve(this.activeAnimationClip, curve.binding, null);
            }
        }

        public void Repaint()
        {
            if (this.animEditor != null)
            {
                this.animEditor.Repaint();
            }
        }

        public void ResampleAnimation()
        {
            if (this.activeAnimationClip != null)
            {
                if (!this.recording)
                {
                    this.recording = true;
                }
                Undo.FlushUndoRecordObjects();
                AnimationMode.BeginSampling();
                CurveBindingUtility.SampleAnimationClip(this.activeRootGameObject, this.activeAnimationClip, this.currentTime);
                AnimationMode.EndSampling();
                SceneView.RepaintAll();
                ParticleSystemWindow instance = ParticleSystemWindow.GetInstance();
                if (instance != null)
                {
                    instance.Repaint();
                }
            }
        }

        public void SaveCurve(AnimationWindowCurve curve)
        {
            if (<>f__am$cache1A == null)
            {
                <>f__am$cache1A = (a, b) => a.time.CompareTo(b.time);
            }
            curve.m_Keyframes.Sort(<>f__am$cache1A);
            Undo.RegisterCompleteObjectUndo(this.activeAnimationClip, "Edit Curve");
            if (curve.isPPtrCurve)
            {
                ObjectReferenceKeyframe[] keyframes = curve.ToObjectCurve();
                if (keyframes.Length == 0)
                {
                    keyframes = null;
                }
                AnimationUtility.SetObjectReferenceCurve(this.activeAnimationClip, curve.binding, keyframes);
            }
            else
            {
                AnimationCurve curve2 = curve.ToAnimationCurve();
                if (curve2.keys.Length == 0)
                {
                    curve2 = null;
                }
                else
                {
                    QuaternionCurveTangentCalculation.UpdateTangentsFromMode(curve2, this.activeAnimationClip, curve.binding);
                }
                AnimationUtility.SetEditorCurve(this.activeAnimationClip, curve.binding, curve2);
            }
            this.Repaint();
        }

        public void SaveSelectedKeys(List<AnimationWindowKeyframe> currentSelectedKeys)
        {
            List<AnimationWindowCurve> list = new List<AnimationWindowCurve>();
            foreach (AnimationWindowKeyframe keyframe in currentSelectedKeys)
            {
                if (!list.Contains(keyframe.curve))
                {
                    list.Add(keyframe.curve);
                }
                List<AnimationWindowKeyframe> list2 = new List<AnimationWindowKeyframe>();
                foreach (AnimationWindowKeyframe keyframe2 in keyframe.curve.m_Keyframes)
                {
                    if (!currentSelectedKeys.Contains(keyframe2) && (AnimationKeyTime.Time(keyframe.time, this.frameRate).frame == AnimationKeyTime.Time(keyframe2.time, this.frameRate).frame))
                    {
                        list2.Add(keyframe2);
                    }
                }
                foreach (AnimationWindowKeyframe keyframe3 in list2)
                {
                    keyframe.curve.m_Keyframes.Remove(keyframe3);
                }
            }
            foreach (AnimationWindowCurve curve in list)
            {
                this.SaveCurve(curve);
            }
        }

        public void SelectHierarchyItem(DopeLine dopeline, bool additive)
        {
            this.SelectHierarchyItem(dopeline.m_HierarchyNodeID, additive, true);
        }

        public void SelectHierarchyItem(int hierarchyNodeID, bool additive, bool triggerSceneSelectionSync)
        {
            if (!additive)
            {
                this.ClearHierarchySelection();
            }
            this.hierarchyState.selectedIDs.Add(hierarchyNodeID);
            int[] selectedInstanceIDs = this.hierarchyState.selectedIDs.ToArray();
            this.HandleHierarchySelectionChanged(selectedInstanceIDs, triggerSceneSelectionSync);
        }

        public void SelectHierarchyItem(DopeLine dopeline, bool additive, bool triggerSceneSelectionSync)
        {
            this.SelectHierarchyItem(dopeline.m_HierarchyNodeID, additive, triggerSceneSelectionSync);
        }

        public void SelectKey(AnimationWindowKeyframe keyframe)
        {
            int hash = keyframe.GetHash();
            if (!this.selectedKeyHashes.Contains(hash))
            {
                this.selectedKeyHashes.Add(hash);
            }
            this.m_SelectedKeysCache = null;
        }

        public void SelectKeysFromDopeline(DopeLine dopeline)
        {
            if (dopeline != null)
            {
                foreach (AnimationWindowKeyframe keyframe in dopeline.keys)
                {
                    this.SelectKey(keyframe);
                }
            }
        }

        public float SnapTimeToWholeFPS(float time)
        {
            return (Mathf.Round(time * this.frameRate) / this.frameRate);
        }

        public float SnapToFrame(float time)
        {
            return (Mathf.Round(time * this.frameRate) / this.frameRate);
        }

        public float SnapToFrame(float time, bool preventHashCollision)
        {
            if (preventHashCollision)
            {
                return ((Mathf.Round(time * this.frameRate) / this.frameRate) + (0.01f / this.frameRate));
            }
            return this.SnapToFrame(time);
        }

        private void SyncSceneSelection(int[] selectedNodeIDs)
        {
            List<int> list = new List<int>();
            foreach (int num in selectedNodeIDs)
            {
                AnimationWindowHierarchyNode node = this.hierarchyData.FindItem(num) as AnimationWindowHierarchyNode;
                if (((this.activeRootGameObject != null) && (node != null)) && !(node is AnimationWindowHierarchyMasterNode))
                {
                    Transform tr = this.activeRootGameObject.transform.Find(node.path);
                    if (((tr != null) && (this.activeRootGameObject != null)) && (this.activeAnimationPlayer == AnimationWindowUtility.GetClosestAnimationPlayerComponentInParents(tr)))
                    {
                        list.Add(tr.gameObject.GetInstanceID());
                    }
                }
            }
            Selection.instanceIDs = list.ToArray();
        }

        public float TimeToFrame(float time)
        {
            return (time * this.frameRate);
        }

        public int TimeToFrameFloor(float time)
        {
            return Mathf.FloorToInt(this.TimeToFrame(time));
        }

        public int TimeToFrameRound(float time)
        {
            return Mathf.RoundToInt(this.TimeToFrame(time));
        }

        public float TimeToPixel(float time)
        {
            return this.TimeToPixel(time, false);
        }

        public float TimeToPixel(float time, bool snapToFrame)
        {
            return (((!snapToFrame ? time : this.SnapToFrame(time)) * this.pixelPerSecond) + this.zeroTimePixel);
        }

        public float TimeToPixel(float time, Rect rect)
        {
            return this.FrameToPixel(time * this.frameRate, rect);
        }

        public void UndoRedoPerformed()
        {
            this.refresh = RefreshType.Everything;
        }

        public void UnSelectHierarchyItem(int hierarchyNodeID)
        {
            this.hierarchyState.selectedIDs.Remove(hierarchyNodeID);
        }

        public void UnSelectHierarchyItem(DopeLine dopeline)
        {
            this.UnSelectHierarchyItem(dopeline.m_HierarchyNodeID);
        }

        public void UnselectKey(AnimationWindowKeyframe keyframe)
        {
            int hash = keyframe.GetHash();
            if (this.selectedKeyHashes.Contains(hash))
            {
                this.selectedKeyHashes.Remove(hash);
            }
            this.m_SelectedKeysCache = null;
        }

        public void UnselectKeysFromDopeline(DopeLine dopeline)
        {
            if (dopeline != null)
            {
                foreach (AnimationWindowKeyframe keyframe in dopeline.keys)
                {
                    this.UnselectKey(keyframe);
                }
            }
        }

        public AnimationClip activeAnimationClip
        {
            get
            {
                return this.m_ActiveAnimationClip;
            }
            set
            {
                if ((this.m_ActiveAnimationClip != value) && !this.m_Locked)
                {
                    this.m_ActiveAnimationClip = value;
                    if (this.onFrameRateChange != null)
                    {
                        this.onFrameRateChange(this.frameRate);
                    }
                    CurveBindingUtility.Cleanup();
                    if (this.onClipSelectionChanged != null)
                    {
                        this.onClipSelectionChanged();
                    }
                }
            }
        }

        public Component activeAnimationPlayer
        {
            get
            {
                if (this.activeGameObject != null)
                {
                    return AnimationWindowUtility.GetClosestAnimationPlayerComponentInParents(this.activeGameObject.transform);
                }
                return null;
            }
        }

        public List<AnimationWindowCurve> activeCurves
        {
            get
            {
                if (this.m_ActiveCurvesCache == null)
                {
                    this.m_ActiveCurvesCache = new List<AnimationWindowCurve>();
                    if ((this.hierarchyState != null) && (this.hierarchyData != null))
                    {
                        foreach (int num in this.hierarchyState.selectedIDs)
                        {
                            AnimationWindowHierarchyNode hierarchyNode = this.hierarchyData.FindItem(num) as AnimationWindowHierarchyNode;
                            if (hierarchyNode != null)
                            {
                                foreach (AnimationWindowCurve curve in this.GetCurves(hierarchyNode, true))
                                {
                                    if (!this.m_ActiveCurvesCache.Contains(curve))
                                    {
                                        this.m_ActiveCurvesCache.Add(curve);
                                    }
                                }
                            }
                        }
                    }
                }
                return this.m_ActiveCurvesCache;
            }
        }

        public List<CurveWrapper> activeCurveWrappers
        {
            get
            {
                if ((this.m_ActiveCurveWrappersCache == null) || (this.m_ActiveCurvesCache == null))
                {
                    this.m_ActiveCurveWrappersCache = new List<CurveWrapper>();
                    foreach (AnimationWindowCurve curve in this.activeCurves)
                    {
                        if (!curve.isPPtrCurve)
                        {
                            this.m_ActiveCurveWrappersCache.Add(AnimationWindowUtility.GetCurveWrapper(curve, this.activeAnimationClip));
                        }
                    }
                    if (!this.m_ActiveCurveWrappersCache.Any<CurveWrapper>())
                    {
                        foreach (AnimationWindowCurve curve2 in this.allCurves)
                        {
                            if (!curve2.isPPtrCurve)
                            {
                                this.m_ActiveCurveWrappersCache.Add(AnimationWindowUtility.GetCurveWrapper(curve2, this.activeAnimationClip));
                            }
                        }
                    }
                }
                return this.m_ActiveCurveWrappersCache;
            }
        }

        public GameObject activeGameObject
        {
            get
            {
                if ((!this.m_Locked && (Selection.activeGameObject != null)) && !EditorUtility.IsPersistent(Selection.activeGameObject))
                {
                    this.m_ActiveGameObject = Selection.activeGameObject;
                }
                return this.m_ActiveGameObject;
            }
        }

        public AnimationWindowKeyframe activeKeyframe
        {
            get
            {
                if (this.m_ActiveKeyframeCache == null)
                {
                    foreach (AnimationWindowCurve curve in this.allCurves)
                    {
                        foreach (AnimationWindowKeyframe keyframe in curve.m_Keyframes)
                        {
                            if (keyframe.GetHash() == this.m_ActiveKeyframeHash)
                            {
                                this.m_ActiveKeyframeCache = keyframe;
                            }
                        }
                    }
                }
                return this.m_ActiveKeyframeCache;
            }
            set
            {
                this.m_ActiveKeyframeCache = null;
                this.m_ActiveKeyframeHash = (value == null) ? 0 : value.GetHash();
            }
        }

        public bool activeObjectIsPrefab
        {
            get
            {
                if (this.activeGameObject == null)
                {
                    return false;
                }
                return (EditorUtility.IsPersistent(this.activeGameObject) || ((this.activeGameObject.hideFlags & HideFlags.NotEditable) != HideFlags.None));
            }
        }

        public GameObject activeRootGameObject
        {
            get
            {
                if (this.activeGameObject != null)
                {
                    if (this.activeObjectIsPrefab)
                    {
                        return null;
                    }
                    Component closestAnimationPlayerComponentInParents = AnimationWindowUtility.GetClosestAnimationPlayerComponentInParents(this.activeGameObject.transform);
                    if (closestAnimationPlayerComponentInParents != null)
                    {
                        return closestAnimationPlayerComponentInParents.gameObject;
                    }
                }
                return null;
            }
        }

        public List<AnimationWindowCurve> allCurves
        {
            get
            {
                if (this.m_AllCurvesCache == null)
                {
                    this.m_AllCurvesCache = new List<AnimationWindowCurve>();
                    if (this.activeAnimationClip != null)
                    {
                        EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(this.activeAnimationClip);
                        EditorCurveBinding[] objectReferenceCurveBindings = AnimationUtility.GetObjectReferenceCurveBindings(this.activeAnimationClip);
                        foreach (EditorCurveBinding binding in curveBindings)
                        {
                            if (AnimationWindowUtility.ShouldShowAnimationWindowCurve(binding))
                            {
                                this.m_AllCurvesCache.Add(new AnimationWindowCurve(this.activeAnimationClip, binding, CurveBindingUtility.GetEditorCurveValueType(this.activeRootGameObject, binding)));
                            }
                        }
                        foreach (EditorCurveBinding binding2 in objectReferenceCurveBindings)
                        {
                            this.m_AllCurvesCache.Add(new AnimationWindowCurve(this.activeAnimationClip, binding2, CurveBindingUtility.GetEditorCurveValueType(this.activeRootGameObject, binding2)));
                        }
                        this.m_AllCurvesCache.Sort();
                    }
                }
                return this.m_AllCurvesCache;
            }
        }

        public bool animationIsEditable
        {
            get
            {
                if (this.activeGameObject == null)
                {
                    return false;
                }
                if ((this.activeAnimationClip != null) && ((this.activeAnimationClip.hideFlags & HideFlags.NotEditable) != HideFlags.None))
                {
                    return false;
                }
                if (this.activeObjectIsPrefab)
                {
                    return false;
                }
                return true;
            }
        }

        public bool animationIsReadOnly
        {
            get
            {
                return ((this.activeAnimationClip == null) || (((this.m_ActiveAnimationClip.hideFlags & HideFlags.NotEditable) != HideFlags.None) || !this.animationIsEditable));
            }
        }

        public bool animatorIsOptimized
        {
            get
            {
                if (this.activeRootGameObject == null)
                {
                    return false;
                }
                Animator component = this.activeRootGameObject.GetComponent<Animator>();
                return ((component != null) && (component.isOptimizable && !component.hasTransformHierarchy));
            }
        }

        public bool clipIsEditable
        {
            get
            {
                if (this.activeAnimationClip == null)
                {
                    return false;
                }
                if ((this.activeAnimationClip.hideFlags & HideFlags.NotEditable) != HideFlags.None)
                {
                    return false;
                }
                if (!AssetDatabase.IsOpenForEdit(this.activeAnimationClip))
                {
                    return false;
                }
                return true;
            }
        }

        public bool clipOnlyMode
        {
            get
            {
                return ((this.activeRootGameObject == null) && (this.activeAnimationClip != null));
            }
        }

        public float currentTime
        {
            get
            {
                return this.m_CurrentTime;
            }
            set
            {
                if (!Mathf.Approximately(this.m_CurrentTime, value))
                {
                    this.m_CurrentTime = value;
                    this.ResampleAnimation();
                }
            }
        }

        public bool disabled
        {
            get
            {
                return ((this.activeRootGameObject == null) || (this.activeAnimationClip == null));
            }
        }

        public List<DopeLine> dopelines
        {
            get
            {
                if (this.m_dopelinesCache == null)
                {
                    this.m_dopelinesCache = new List<DopeLine>();
                    if (this.hierarchyData != null)
                    {
                        foreach (TreeViewItem item in this.hierarchyData.GetRows())
                        {
                            AnimationWindowHierarchyNode hierarchyNode = item as AnimationWindowHierarchyNode;
                            if ((hierarchyNode != null) && !(hierarchyNode is AnimationWindowHierarchyAddButtonNode))
                            {
                                List<AnimationWindowCurve> allCurves;
                                if (item is AnimationWindowHierarchyMasterNode)
                                {
                                    allCurves = this.allCurves;
                                }
                                else
                                {
                                    allCurves = this.GetCurves(hierarchyNode, true);
                                }
                                DopeLine line = new DopeLine(item.id, allCurves.ToArray()) {
                                    tallMode = this.hierarchyState.GetTallMode(hierarchyNode),
                                    objectType = hierarchyNode.animatableObjectType,
                                    hasChildren = !(hierarchyNode is AnimationWindowHierarchyPropertyNode),
                                    isMasterDopeline = item is AnimationWindowHierarchyMasterNode
                                };
                                this.m_dopelinesCache.Add(line);
                            }
                        }
                    }
                }
                return this.m_dopelinesCache;
            }
        }

        public int frame
        {
            get
            {
                return this.TimeToFrameFloor(this.currentTime);
            }
            set
            {
                this.currentTime = this.FrameToTime((float) value);
            }
        }

        public float frameRate
        {
            get
            {
                if (this.activeAnimationClip == null)
                {
                    return 60f;
                }
                return this.activeAnimationClip.frameRate;
            }
            set
            {
                if (((this.activeAnimationClip != null) && (value > 0f)) && (value <= 10000f))
                {
                    foreach (AnimationWindowCurve curve in this.allCurves)
                    {
                        foreach (AnimationWindowKeyframe keyframe in curve.m_Keyframes)
                        {
                            int frame = AnimationKeyTime.Time(keyframe.time, this.frameRate).frame;
                            keyframe.time = AnimationKeyTime.Frame(frame, value).time;
                        }
                        this.SaveCurve(curve);
                    }
                    AnimationEvent[] animationEvents = AnimationUtility.GetAnimationEvents(this.m_ActiveAnimationClip);
                    foreach (AnimationEvent event2 in animationEvents)
                    {
                        int num3 = AnimationKeyTime.Time(event2.time, this.frameRate).frame;
                        event2.time = AnimationKeyTime.Frame(num3, value).time;
                    }
                    AnimationUtility.SetAnimationEvents(this.m_ActiveAnimationClip, animationEvents);
                    this.m_ActiveAnimationClip.frameRate = value;
                    if (this.onFrameRateChange != null)
                    {
                        this.onFrameRateChange(this.frameRate);
                    }
                }
            }
        }

        public bool locked
        {
            get
            {
                return this.m_Locked;
            }
            set
            {
                if (!this.disabled)
                {
                    this.m_Locked = value;
                    if (this.m_ActiveGameObject != Selection.activeGameObject)
                    {
                        this.OnSelectionChange();
                    }
                }
            }
        }

        public int maxFrame
        {
            get
            {
                return this.TimeToFrameRound(this.maxTime);
            }
        }

        public float maxTime
        {
            get
            {
                return (((this.m_ActiveAnimationClip == null) || (this.m_ActiveAnimationClip.stopTime <= 0f)) ? 1f : this.m_ActiveAnimationClip.stopTime);
            }
        }

        public float maxVisibleFrame
        {
            get
            {
                return (this.maxVisibleTime * this.frameRate);
            }
        }

        public float maxVisibleTime
        {
            get
            {
                return this.m_timeArea.shownArea.xMax;
            }
        }

        public int minFrame
        {
            get
            {
                return this.TimeToFrameRound(this.minTime);
            }
        }

        public float minTime
        {
            get
            {
                return ((this.m_ActiveAnimationClip == null) ? 0f : this.m_ActiveAnimationClip.startTime);
            }
        }

        public float minVisibleFrame
        {
            get
            {
                return (this.minVisibleTime * this.frameRate);
            }
        }

        public float minVisibleTime
        {
            get
            {
                return this.m_timeArea.shownArea.xMin;
            }
        }

        public float pixelPerSecond
        {
            get
            {
                return this.timeArea.m_Scale.x;
            }
        }

        public bool playing
        {
            get
            {
                return AnimationMode.InAnimationPlaybackMode();
            }
            set
            {
                if (value && !AnimationMode.InAnimationPlaybackMode())
                {
                    AnimationMode.StartAnimationPlaybackMode();
                    this.recording = true;
                }
                if (!value && AnimationMode.InAnimationPlaybackMode())
                {
                    AnimationMode.StopAnimationPlaybackMode();
                    this.currentTime = this.FrameToTime((float) this.frame);
                }
            }
        }

        public bool recording
        {
            get
            {
                return AnimationMode.InAnimationMode();
            }
            set
            {
                if (value && !AnimationMode.InAnimationMode())
                {
                    AnimationMode.StartAnimationMode();
                    Undo.postprocessModifications = (Undo.PostprocessModifications) Delegate.Combine(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
                }
                else if (!value)
                {
                    AnimationMode.StopAnimationMode();
                    Undo.postprocessModifications = (Undo.PostprocessModifications) Delegate.Remove(Undo.postprocessModifications, new Undo.PostprocessModifications(this.PostprocessAnimationRecordingModifications));
                }
            }
        }

        public RefreshType refresh
        {
            get
            {
                return this.m_Refresh;
            }
            set
            {
                if (this.m_Refresh < value)
                {
                    this.m_Refresh = value;
                }
            }
        }

        public List<AnimationWindowHierarchyNode> selectedHierarchyNodes
        {
            get
            {
                List<AnimationWindowHierarchyNode> list = new List<AnimationWindowHierarchyNode>();
                if (this.hierarchyData != null)
                {
                    foreach (int num in this.hierarchyState.selectedIDs)
                    {
                        AnimationWindowHierarchyNode item = (AnimationWindowHierarchyNode) this.hierarchyData.FindItem(num);
                        if ((item != null) && !(item is AnimationWindowHierarchyAddButtonNode))
                        {
                            list.Add(item);
                        }
                    }
                }
                return list;
            }
        }

        private HashSet<int> selectedKeyHashes
        {
            get
            {
                if (this.m_SelectedKeyHashes == null)
                {
                }
                return (this.m_SelectedKeyHashes = new HashSet<int>());
            }
            set
            {
                this.m_SelectedKeyHashes = value;
            }
        }

        public List<AnimationWindowKeyframe> selectedKeys
        {
            get
            {
                if (this.m_SelectedKeysCache == null)
                {
                    this.m_SelectedKeysCache = new List<AnimationWindowKeyframe>();
                    foreach (AnimationWindowCurve curve in this.allCurves)
                    {
                        foreach (AnimationWindowKeyframe keyframe in curve.m_Keyframes)
                        {
                            if (this.KeyIsSelected(keyframe))
                            {
                                this.m_SelectedKeysCache.Add(keyframe);
                            }
                        }
                    }
                }
                return this.m_SelectedKeysCache;
            }
        }

        public AnimationKeyTime time
        {
            get
            {
                return AnimationKeyTime.Frame(this.frame, this.frameRate);
            }
        }

        public TimeArea timeArea
        {
            get
            {
                return this.m_timeArea;
            }
            set
            {
                this.m_timeArea = value;
            }
        }

        public float visibleFrameSpan
        {
            get
            {
                return (this.visibleTimeSpan * this.frameRate);
            }
        }

        public float visibleTimeSpan
        {
            get
            {
                return (this.maxVisibleTime - this.minVisibleTime);
            }
        }

        public float zeroTimePixel
        {
            get
            {
                return ((this.timeArea.shownArea.xMin * this.timeArea.m_Scale.x) * -1f);
            }
        }

        [CompilerGenerated]
        private sealed class <GetAggregateKeys>c__AnonStorey40
        {
            internal AnimationWindowHierarchyNode hierarchyNode;

            internal bool <>m__6F(DopeLine e)
            {
                return (e.m_HierarchyNodeID == this.hierarchyNode.id);
            }
        }

        public enum RefreshType
        {
            None,
            CurvesOnly,
            Everything
        }
    }
}

