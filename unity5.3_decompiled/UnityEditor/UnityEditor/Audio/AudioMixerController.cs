namespace UnityEditor.Audio
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Audio;

    internal sealed class AudioMixerController : AudioMixer
    {
        [CompilerGenerated]
        private static Func<GUID, AudioMixerGroupController, <>__AnonType0<GUID, AudioMixerGroupController>> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<<>__AnonType0<GUID, AudioMixerGroupController>, bool> <>f__am$cache9;
        [CompilerGenerated]
        private static Func<<>__AnonType0<GUID, AudioMixerGroupController>, GUID> <>f__am$cacheA;
        [CompilerGenerated]
        private static Func<Object, AudioMixerGroupController> <>f__am$cacheB;
        public static float kMaxEffect;
        public static float kMinVolume = -80f;
        public static float kVolumeWarp = 1.7f;
        [NonSerialized]
        private List<AudioMixerGroupController> m_CachedSelection;
        [NonSerialized]
        private Dictionary<GUID, AudioParameterPath> m_ExposedParamPathCache;
        [NonSerialized]
        public int m_HighlightEffectIndex = -1;
        public static string s_GroupEffectDisplaySeperator = @"\";

        public event ChangedExposedParameterHandler ChangedExposedParameter;

        public AudioMixerController()
        {
            Internal_CreateAudioMixerController(this);
        }

        public void AddChildToParent(AudioMixerGroupController child, AudioMixerGroupController parent)
        {
            AudioMixerGroupController[] groups = new AudioMixerGroupController[] { child };
            this.RemoveGroupsFromParent(groups, false);
            parent.children = new List<AudioMixerGroupController>(parent.children) { child }.ToArray();
        }

        public void AddExposedParameter(AudioParameterPath path)
        {
            if (!this.ContainsExposedParameter(path.parameter))
            {
                List<ExposedAudioParameter> list = new List<ExposedAudioParameter>(this.exposedParameters);
                ExposedAudioParameter item = new ExposedAudioParameter {
                    name = this.FindUniqueParameterName("MyExposedParam", this.exposedParameters),
                    guid = path.parameter
                };
                list.Add(item);
                list.Sort(new Comparison<ExposedAudioParameter>(this.SortFuncForExposedParameters));
                this.exposedParameters = list.ToArray();
                this.OnChangedExposedParameter();
                this.exposedParamCache[path.parameter] = path;
                AudioMixerUtility.RepaintAudioMixerAndInspectors();
            }
            else
            {
                Debug.LogError("Cannot expose the same parameter more than once!");
            }
        }

        public void AddGroupToCurrentView(AudioMixerGroupController group)
        {
            MixerGroupView[] views = this.views;
            List<GUID> list = views[this.currentViewIndex].guids.ToList<GUID>();
            list.Add(group.groupID);
            views[this.currentViewIndex].guids = list.ToArray();
            this.views = views.ToArray<MixerGroupView>();
        }

        private void AddNewSubAsset(Object obj, bool storeUndoState)
        {
            AssetDatabase.AddObjectToAsset(obj, this);
            if (storeUndoState)
            {
                Undo.RegisterCreatedObjectUndo(obj, string.Empty);
            }
        }

        public bool AreAnyOfTheGroupsInTheListAncestors(List<AudioMixerGroupController> groups)
        {
            <AreAnyOfTheGroupsInTheListAncestors>c__AnonStoreyD yd = new <AreAnyOfTheGroupsInTheListAncestors>c__AnonStoreyD {
                groups = groups,
                <>f__this = this
            };
            return yd.groups.Any<AudioMixerGroupController>(new Func<AudioMixerGroupController, bool>(yd.<>m__4));
        }

        private static Dictionary<object, ConnectionNode> BuildTemporaryGraph(List<AudioMixerGroupController> allGroups, AudioMixerGroupController groupWhoseEffectIsChanged, AudioMixerEffectController effectWhoseTargetIsChanged, AudioMixerEffectController targetToTest, AudioMixerGroupController modifiedGroup1, List<AudioMixerEffectController> modifiedGroupEffects1, AudioMixerGroupController modifiedGroup2, List<AudioMixerEffectController> modifiedGroupEffects2)
        {
            Dictionary<object, ConnectionNode> dictionary = new Dictionary<object, ConnectionNode>();
            foreach (AudioMixerGroupController controller in allGroups)
            {
                ConnectionNode node = new ConnectionNode {
                    group = controller,
                    effect = null
                };
                dictionary[controller] = node;
                object obj2 = controller;
                List<AudioMixerEffectController> list = (controller != modifiedGroup1) ? ((controller != modifiedGroup2) ? controller.effects.ToList<AudioMixerEffectController>() : modifiedGroupEffects2) : modifiedGroupEffects1;
                foreach (AudioMixerEffectController controller2 in list)
                {
                    if (!dictionary.ContainsKey(controller2))
                    {
                        dictionary[controller2] = new ConnectionNode();
                    }
                    dictionary[controller2].group = controller;
                    dictionary[controller2].effect = controller2;
                    if (!dictionary[obj2].targets.Contains(controller2))
                    {
                        dictionary[obj2].targets.Add(controller2);
                    }
                    AudioMixerEffectController key = ((controller != groupWhoseEffectIsChanged) || (effectWhoseTargetIsChanged != controller2)) ? controller2.sendTarget : targetToTest;
                    if (key != null)
                    {
                        if (!dictionary.ContainsKey(key))
                        {
                            dictionary[key] = new ConnectionNode();
                            dictionary[key].group = controller;
                            dictionary[key].effect = key;
                        }
                        if (!dictionary[controller2].targets.Contains(key))
                        {
                            dictionary[controller2].targets.Add(key);
                        }
                    }
                    obj2 = controller2;
                }
                dictionary[controller].groupTail = obj2;
            }
            return dictionary;
        }

        public void BuildTestSetup(int minSpan, int maxSpan, int maxGroups)
        {
            int numGroups = 0;
            this.DeleteGroups(this.masterGroup.children);
            this.BuildTestSetup(new Random(), this.masterGroup, minSpan, maxSpan, maxGroups, "G", ref numGroups);
        }

        private void BuildTestSetup(Random r, AudioMixerGroupController parent, int minSpan, int maxSpan, int maxGroups, string prefix, ref int numGroups)
        {
            int num = (numGroups != 0) ? r.Next(minSpan, maxSpan + 1) : maxSpan;
            for (int i = 0; i < num; i++)
            {
                string name = prefix + i;
                AudioMixerGroupController child = this.CreateNewGroup(name, false);
                this.AddChildToParent(child, parent);
                if (++numGroups >= maxGroups)
                {
                    return;
                }
                this.BuildTestSetup(r, child, minSpan, (maxSpan <= minSpan) ? minSpan : (maxSpan - 1), maxGroups, name, ref numGroups);
            }
        }

        private static bool CheckForCycle(object curr, Dictionary<object, ConnectionNode> graph, List<ConnectionNode> identifiedLoop)
        {
            ConnectionNode item = graph[curr];
            if (item.visited)
            {
                if (identifiedLoop != null)
                {
                    identifiedLoop.Clear();
                    identifiedLoop.Add(item);
                }
                return true;
            }
            item.visited = true;
            foreach (object obj2 in item.targets)
            {
                if (CheckForCycle(obj2, graph, identifiedLoop))
                {
                    item.visited = false;
                    if (identifiedLoop != null)
                    {
                        identifiedLoop.Add(item);
                    }
                    return true;
                }
            }
            item.visited = false;
            return false;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool CheckForCyclicReferences(AudioMixer mixer, AudioMixerGroup group);
        public void ClearEventHandlers()
        {
            if (this.ChangedExposedParameter != null)
            {
                foreach (Delegate delegate2 in this.ChangedExposedParameter.GetInvocationList())
                {
                    this.ChangedExposedParameter = (ChangedExposedParameterHandler) Delegate.Remove(this.ChangedExposedParameter, (ChangedExposedParameterHandler) delegate2);
                }
            }
        }

        public void ClearSendConnectionsTo(AudioMixerEffectController sendTarget)
        {
            foreach (AudioMixerGroupController controller in this.GetAllAudioGroupsSlow())
            {
                foreach (AudioMixerEffectController controller2 in controller.effects)
                {
                    if (controller2.IsSend() && (controller2.sendTarget == sendTarget))
                    {
                        Undo.RecordObject(controller2, "Clear Send target");
                        controller2.sendTarget = null;
                    }
                }
            }
        }

        public void CloneNewSnapshotFromTarget(bool storeUndoState)
        {
            List<AudioMixerSnapshotController> list = new List<AudioMixerSnapshotController>(this.snapshots);
            AudioMixerSnapshotController item = Object.Instantiate<AudioMixerSnapshotController>(this.TargetSnapshot);
            item.name = this.TargetSnapshot.name + " - Copy";
            list.Add(item);
            this.snapshots = list.ToArray();
            this.TargetSnapshot = list[list.Count - 1];
            AssetDatabase.AddObjectToAsset(item, this);
            if (storeUndoState)
            {
                Undo.RegisterCreatedObjectUndo(item, string.Empty);
            }
            this.OnSubAssetChanged();
        }

        public void CloneViewFromCurrent()
        {
            Undo.RecordObject(this, "Create view");
            List<MixerGroupView> list = new List<MixerGroupView>(this.views);
            MixerGroupView item = new MixerGroupView {
                name = this.views[this.currentViewIndex].name + " - Copy",
                guids = this.views[this.currentViewIndex].guids
            };
            list.Add(item);
            this.views = list.ToArray();
            this.currentViewIndex = list.Count - 1;
        }

        public bool ContainsExposedParameter(GUID parameter)
        {
            <ContainsExposedParameter>c__AnonStoreyB yb = new <ContainsExposedParameter>c__AnonStoreyB {
                parameter = parameter
            };
            return (this.exposedParameters.Where<ExposedAudioParameter>(new Func<ExposedAudioParameter, bool>(yb.<>m__2)).ToArray<ExposedAudioParameter>().Length > 0);
        }

        public void CopyAllSettingsToAllSnapshots(AudioMixerGroupController group, AudioMixerSnapshotController snapshot)
        {
            for (int i = 0; i < group.effects.Length; i++)
            {
                this.CopyEffectSettingsToAllSnapshots(group, i, snapshot, true);
            }
            AudioMixerSnapshotController[] snapshots = this.snapshots;
            for (int j = 0; j < snapshots.Length; j++)
            {
                if (snapshots[j] != snapshot)
                {
                    AudioMixerSnapshotController controller = snapshots[j];
                    group.SetValueForVolume(this, controller, group.GetValueForVolume(this, snapshot));
                    group.SetValueForPitch(this, controller, group.GetValueForPitch(this, snapshot));
                }
            }
        }

        public void CopyAttenuationToAllSnapshots(AudioMixerGroupController group, AudioMixerSnapshotController snapshot)
        {
            AudioMixerSnapshotController[] snapshots = this.snapshots;
            for (int i = 0; i < snapshots.Length; i++)
            {
                if (snapshots[i] != snapshot)
                {
                    AudioMixerSnapshotController controller = snapshots[i];
                    group.SetValueForVolume(this, controller, group.GetValueForVolume(this, snapshot));
                }
            }
        }

        public AudioMixerEffectController CopyEffect(AudioMixerEffectController sourceEffect)
        {
            AudioMixerEffectController objectToAdd = new AudioMixerEffectController(sourceEffect.effectName) {
                name = sourceEffect.name
            };
            objectToAdd.PreallocateGUIDs();
            MixerParameterDefinition[] effectParameters = MixerEffectDefinitions.GetEffectParameters(sourceEffect.effectName);
            foreach (AudioMixerSnapshotController controller2 in this.snapshots)
            {
                float num;
                if (controller2.GetValue(sourceEffect.GetGUIDForMixLevel(), out num))
                {
                    controller2.SetValue(objectToAdd.GetGUIDForMixLevel(), num);
                }
                foreach (MixerParameterDefinition definition in effectParameters)
                {
                    if (controller2.GetValue(sourceEffect.GetGUIDForParameter(definition.name), out num))
                    {
                        controller2.SetValue(objectToAdd.GetGUIDForParameter(definition.name), num);
                    }
                }
            }
            AssetDatabase.AddObjectToAsset(objectToAdd, this);
            return objectToAdd;
        }

        public void CopyEffectSettingsToAllSnapshots(AudioMixerGroupController group, int effectIndex, AudioMixerSnapshotController snapshot, bool includeWetParam)
        {
            AudioMixerSnapshotController[] snapshots = this.snapshots;
            for (int i = 0; i < snapshots.Length; i++)
            {
                if (snapshots[i] != snapshot)
                {
                    float num2;
                    AudioMixerEffectController controller = group.effects[effectIndex];
                    MixerParameterDefinition[] effectParameters = MixerEffectDefinitions.GetEffectParameters(controller.effectName);
                    if (includeWetParam)
                    {
                        GUID gUIDForMixLevel = controller.GetGUIDForMixLevel();
                        if (snapshot.GetValue(gUIDForMixLevel, out num2))
                        {
                            snapshots[i].SetValue(gUIDForMixLevel, num2);
                        }
                    }
                    foreach (MixerParameterDefinition definition in effectParameters)
                    {
                        GUID gUIDForParameter = controller.GetGUIDForParameter(definition.name);
                        if (snapshot.GetValue(gUIDForParameter, out num2))
                        {
                            snapshots[i].SetValue(gUIDForParameter, num2);
                        }
                    }
                }
            }
        }

        public void CreateDefaultAsset(string path)
        {
            this.masterGroup = new AudioMixerGroupController(this);
            this.masterGroup.name = "Master";
            this.masterGroup.PreallocateGUIDs();
            AudioMixerEffectController effect = new AudioMixerEffectController("Attenuation");
            effect.PreallocateGUIDs();
            this.masterGroup.InsertEffect(effect, 0);
            AudioMixerSnapshotController controller2 = new AudioMixerSnapshotController(this) {
                name = "Snapshot"
            };
            this.snapshots = new AudioMixerSnapshotController[] { controller2 };
            this.startSnapshot = controller2;
            Object[] assets = new Object[] { this, this.masterGroup, effect, controller2 };
            AssetDatabase.CreateAssetFromObjects(assets, path);
        }

        public static AudioMixerController CreateMixerControllerAtPath(string path)
        {
            AudioMixerController controller = new AudioMixerController();
            controller.CreateDefaultAsset(path);
            return controller;
        }

        public AudioMixerGroupController CreateNewGroup(string name, bool storeUndoState)
        {
            AudioMixerGroupController controller = new AudioMixerGroupController(this) {
                name = name
            };
            controller.PreallocateGUIDs();
            AudioMixerEffectController controller2 = new AudioMixerEffectController("Attenuation");
            this.AddNewSubAsset(controller2, storeUndoState);
            controller2.PreallocateGUIDs();
            controller.InsertEffect(controller2, 0);
            this.AddNewSubAsset(controller, storeUndoState);
            return controller;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool CurrentViewContainsGroup(GUID group);
        public static float DbToLin(float x)
        {
            if (x < kMinVolume)
            {
                return 0f;
            }
            return Mathf.Pow(10f, x * 0.05f);
        }

        public void DeleteGroups(AudioMixerGroupController[] groups)
        {
            List<AudioMixerGroupController> list = groups.ToList<AudioMixerGroupController>();
            this.RemoveAncestorGroups(list);
            this.DeleteGroupsInternal(list, this.GetAllAudioGroupsSlow());
            this.OnUnitySelectionChanged();
        }

        private void DeleteGroupsInternal(List<AudioMixerGroupController> groupsToDelete, List<AudioMixerGroupController> allGroups)
        {
            foreach (AudioMixerGroupController controller in allGroups)
            {
                IEnumerable<AudioMixerGroupController> source = groupsToDelete.Intersect<AudioMixerGroupController>(controller.children);
                if (source.Count<AudioMixerGroupController>() > 0)
                {
                    Undo.RegisterCompleteObjectUndo(controller, "Delete Group(s)");
                    controller.children = controller.children.Except<AudioMixerGroupController>(source).ToArray<AudioMixerGroupController>();
                }
            }
            foreach (AudioMixerGroupController controller2 in groupsToDelete)
            {
                this.DeleteSubGroupRecursive(controller2);
            }
        }

        private void DeleteSubGroupRecursive(AudioMixerGroupController group)
        {
            foreach (AudioMixerGroupController controller in group.children)
            {
                this.DeleteSubGroupRecursive(controller);
            }
            foreach (AudioMixerEffectController controller2 in group.effects)
            {
                this.DestroyExposedParametersContainedInEffect(controller2);
                Undo.DestroyObjectImmediate(controller2);
            }
            this.DestroyExposedParametersContainedInGroup(group);
            Undo.DestroyObjectImmediate(group);
        }

        public void DeleteView(int index)
        {
            Undo.RecordObject(this, "Delete view");
            List<MixerGroupView> list = new List<MixerGroupView>(this.views);
            list.RemoveAt(index);
            this.views = list.ToArray();
            int num = Mathf.Clamp(this.currentViewIndex, 0, list.Count - 1);
            this.ForceSetView(num);
        }

        private void DestroyExposedParametersContainedInEffect(AudioMixerEffectController effect)
        {
            Undo.RecordObject(this, "Changed Exposed Parameters");
            foreach (ExposedAudioParameter parameter in this.exposedParameters)
            {
                if (effect.ContainsParameterGUID(parameter.guid))
                {
                    this.RemoveExposedParameter(parameter.guid);
                }
            }
        }

        private void DestroyExposedParametersContainedInGroup(AudioMixerGroupController group)
        {
            Undo.RecordObject(this, "Remove Exposed Parameter");
            foreach (ExposedAudioParameter parameter in this.exposedParameters)
            {
                if ((group.GetGUIDForVolume() == parameter.guid) || (group.GetGUIDForPitch() == parameter.guid))
                {
                    this.RemoveExposedParameter(parameter.guid);
                }
            }
        }

        public static bool DoesTheTemporaryGraphHaveAnyCycles(List<AudioMixerGroupController> allGroups, List<ConnectionNode> identifiedLoop, Dictionary<object, ConnectionNode> graph)
        {
            foreach (AudioMixerGroupController controller in allGroups)
            {
                if (!CheckForCycle(controller, graph, identifiedLoop))
                {
                    continue;
                }
                if (identifiedLoop != null)
                {
                    ConnectionNode node = identifiedLoop[0];
                    int index = 1;
                    while (index < identifiedLoop.Count)
                    {
                        if (identifiedLoop[index++] == node)
                        {
                            break;
                        }
                    }
                    identifiedLoop.RemoveRange(index, identifiedLoop.Count - index);
                    identifiedLoop.Reverse();
                }
                return true;
            }
            return false;
        }

        private AudioMixerGroupController DuplicateGroupRecurse(AudioMixerGroupController sourceGroup)
        {
            AudioMixerGroupController objectToAdd = new AudioMixerGroupController(this);
            List<AudioMixerEffectController> list = new List<AudioMixerEffectController>();
            foreach (AudioMixerEffectController controller2 in sourceGroup.effects)
            {
                list.Add(this.CopyEffect(controller2));
            }
            List<AudioMixerGroupController> list2 = new List<AudioMixerGroupController>();
            foreach (AudioMixerGroupController controller3 in sourceGroup.children)
            {
                list2.Add(this.DuplicateGroupRecurse(controller3));
            }
            objectToAdd.name = sourceGroup.name + " - Copy";
            objectToAdd.PreallocateGUIDs();
            objectToAdd.effects = list.ToArray();
            objectToAdd.children = list2.ToArray();
            objectToAdd.solo = sourceGroup.solo;
            objectToAdd.mute = sourceGroup.mute;
            objectToAdd.bypassEffects = sourceGroup.bypassEffects;
            foreach (AudioMixerSnapshotController controller4 in this.snapshots)
            {
                float num3;
                if (controller4.GetValue(sourceGroup.GetGUIDForVolume(), out num3))
                {
                    controller4.SetValue(objectToAdd.GetGUIDForVolume(), num3);
                }
                if (controller4.GetValue(sourceGroup.GetGUIDForPitch(), out num3))
                {
                    controller4.SetValue(objectToAdd.GetGUIDForPitch(), num3);
                }
            }
            AssetDatabase.AddObjectToAsset(objectToAdd, this);
            if (this.CurrentViewContainsGroup(sourceGroup.groupID))
            {
                objectToAdd.controller.AddGroupToCurrentView(objectToAdd);
            }
            return objectToAdd;
        }

        public List<AudioMixerGroupController> DuplicateGroups(AudioMixerGroupController[] sourceGroups)
        {
            List<AudioMixerGroupController> groups = sourceGroups.ToList<AudioMixerGroupController>();
            this.RemoveAncestorGroups(groups);
            List<AudioMixerGroupController> list2 = new List<AudioMixerGroupController>();
            foreach (AudioMixerGroupController controller in groups)
            {
                AudioMixerGroupController controller2 = this.FindParentGroup(this.masterGroup, controller);
                if ((controller2 != null) && (controller != null))
                {
                    AudioMixerGroupController item = this.DuplicateGroupRecurse(controller);
                    controller2.children = new List<AudioMixerGroupController>(controller2.children) { item }.ToArray();
                    list2.Add(item);
                }
            }
            return list2;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool EditingTargetSnapshot();
        public AudioMixerGroupController FindParentGroup(AudioMixerGroupController node, AudioMixerGroupController group)
        {
            for (int i = 0; i < node.children.Length; i++)
            {
                if (node.children[i] == group)
                {
                    return node;
                }
                AudioMixerGroupController controller = this.FindParentGroup(node.children[i], group);
                if (controller != null)
                {
                    return controller;
                }
            }
            return null;
        }

        private string FindUniqueParameterName(string template, ExposedAudioParameter[] parameters)
        {
            string str = template;
            int num = 1;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (str == parameters[i].name)
                {
                    str = template + " " + num++;
                    i = -1;
                }
            }
            return str;
        }

        public static string FixNameForPopupMenu(string s)
        {
            return s;
        }

        public void ForceSetView(int index)
        {
            this.currentViewIndex = index;
            this.SanitizeGroupViews();
        }

        public List<AudioMixerGroupController> GetAllAudioGroupsSlow()
        {
            List<AudioMixerGroupController> groups = new List<AudioMixerGroupController>();
            if (this.masterGroup != null)
            {
                this.GetAllAudioGroupsSlowRecurse(this.masterGroup, ref groups);
            }
            return groups;
        }

        private void GetAllAudioGroupsSlowRecurse(AudioMixerGroupController g, ref List<AudioMixerGroupController> groups)
        {
            groups.Add(g);
            foreach (AudioMixerGroupController controller in g.children)
            {
                this.GetAllAudioGroupsSlowRecurse(controller, ref groups);
            }
        }

        public AudioMixerGroupController[] GetCurrentViewGroupList()
        {
            <GetCurrentViewGroupList>c__AnonStorey10 storey = new <GetCurrentViewGroupList>c__AnonStorey10();
            List<AudioMixerGroupController> allAudioGroupsSlow = this.GetAllAudioGroupsSlow();
            storey.view = this.views[this.currentViewIndex];
            return allAudioGroupsSlow.Where<AudioMixerGroupController>(new Func<AudioMixerGroupController, bool>(storey.<>m__A)).ToArray<AudioMixerGroupController>();
        }

        private static void GetGroupsRecurse(AudioMixerGroupController group, List<AudioMixerGroupController> groups)
        {
            groups.Add(group);
            AudioMixerGroupController[] children = group.children;
            for (int i = 0; i < children.Length; i++)
            {
                GetGroupsRecurse(children[i], groups);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int GetGroupVUInfo(GUID group, bool fader, ref float[] vuLevel, ref float[] vuPeak);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern float GetMaxVolume();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern float GetVolumeSplitPoint();
        public bool HasMoreThanOneGroup()
        {
            return (this.masterGroup.children.Length > 0);
        }

        public static bool InsertEffect(AudioMixerEffectController effect, ref List<AudioMixerEffectController> targetEffects, int targetIndex)
        {
            if ((targetIndex < 0) || (targetIndex > targetEffects.Count))
            {
                Debug.LogError(string.Concat(new object[] { "Inserting effect failed! size: ", targetEffects.Count, " at index: ", targetIndex }));
                return false;
            }
            targetEffects.Insert(targetIndex, effect);
            return true;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_CreateAudioMixerController(AudioMixerController mono);
        private bool IsChildOf(AudioMixerGroupController child, List<AudioMixerGroupController> groups)
        {
            while (child != null)
            {
                child = this.FindParentGroup(this.masterGroup, child);
                if (groups.Contains(child))
                {
                    return true;
                }
            }
            return false;
        }

        private static void ListTemporaryGraph(Dictionary<object, ConnectionNode> graph)
        {
            Debug.Log("Listing temporary graph:");
            int num = 0;
            foreach (KeyValuePair<object, ConnectionNode> pair in graph)
            {
                Debug.Log(string.Format("Node {0}: {1}", num++, pair.Value.GetDisplayString()));
                int num2 = 0;
                foreach (object obj2 in pair.Value.targets)
                {
                    Debug.Log(string.Format("  Target {0}: {1}", num2++, graph[obj2].GetDisplayString()));
                }
            }
        }

        public static bool MoveEffect(ref List<AudioMixerEffectController> sourceEffects, int sourceIndex, ref List<AudioMixerEffectController> targetEffects, int targetIndex)
        {
            if (sourceEffects == targetEffects)
            {
                if (targetIndex > sourceIndex)
                {
                    targetIndex--;
                }
                if (sourceIndex == targetIndex)
                {
                    return false;
                }
            }
            if ((sourceIndex < 0) || (sourceIndex >= sourceEffects.Count))
            {
                return false;
            }
            if ((targetIndex < 0) || (targetIndex > targetEffects.Count))
            {
                return false;
            }
            AudioMixerEffectController item = sourceEffects[sourceIndex];
            sourceEffects.RemoveAt(sourceIndex);
            targetEffects.Insert(targetIndex, item);
            return true;
        }

        public void OnChangedExposedParameter()
        {
            if (this.ChangedExposedParameter != null)
            {
                this.ChangedExposedParameter();
            }
        }

        public void OnSubAssetChanged()
        {
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(this));
        }

        public void OnUnitySelectionChanged()
        {
            List<AudioMixerGroupController> allAudioGroupsSlow = this.GetAllAudioGroupsSlow();
            Object[] filtered = Selection.GetFiltered(typeof(AudioMixerGroupController), SelectionMode.Deep);
            if (<>f__am$cacheB == null)
            {
                <>f__am$cacheB = g => (AudioMixerGroupController) g;
            }
            this.m_CachedSelection = allAudioGroupsSlow.Intersect<AudioMixerGroupController>(filtered.Select<Object, AudioMixerGroupController>(<>f__am$cacheB)).ToList<AudioMixerGroupController>();
        }

        private void RemoveAncestorGroups(List<AudioMixerGroupController> groups)
        {
            <RemoveAncestorGroups>c__AnonStoreyE ye = new <RemoveAncestorGroups>c__AnonStoreyE {
                groups = groups,
                <>f__this = this
            };
            ye.groups.RemoveAll(new Predicate<AudioMixerGroupController>(ye.<>m__5));
            object.Equals(this.AreAnyOfTheGroupsInTheListAncestors(ye.groups), false);
        }

        public void RemoveEffect(AudioMixerEffectController effect, AudioMixerGroupController group)
        {
            Undo.RecordObject(group, "Delete Effect");
            List<AudioMixerEffectController> list = new List<AudioMixerEffectController>(group.effects);
            list.Remove(effect);
            group.effects = list.ToArray();
            this.DestroyExposedParametersContainedInEffect(effect);
            Undo.DestroyObjectImmediate(effect);
        }

        public void RemoveExposedParameter(GUID parameter)
        {
            <RemoveExposedParameter>c__AnonStoreyC yc = new <RemoveExposedParameter>c__AnonStoreyC {
                parameter = parameter
            };
            this.exposedParameters = this.exposedParameters.Where<ExposedAudioParameter>(new Func<ExposedAudioParameter, bool>(yc.<>m__3)).ToArray<ExposedAudioParameter>();
            this.OnChangedExposedParameter();
            if (this.exposedParamCache.ContainsKey(yc.parameter))
            {
                this.exposedParamCache.Remove(yc.parameter);
            }
            AudioMixerUtility.RepaintAudioMixerAndInspectors();
        }

        public void RemoveGroupsFromParent(AudioMixerGroupController[] groups, bool storeUndoState)
        {
            List<AudioMixerGroupController> list = groups.ToList<AudioMixerGroupController>();
            this.RemoveAncestorGroups(list);
            if (storeUndoState)
            {
                Undo.RecordObject(this, "Remove group");
            }
            foreach (AudioMixerGroupController controller in list)
            {
                foreach (AudioMixerGroupController controller2 in this.GetAllAudioGroupsSlow())
                {
                    List<AudioMixerGroupController> list3 = new List<AudioMixerGroupController>(controller2.children);
                    if (list3.Contains(controller))
                    {
                        list3.Remove(controller);
                    }
                    if (controller2.children.Length != list3.Count)
                    {
                        controller2.children = list3.ToArray();
                    }
                }
            }
        }

        public void RemoveSnapshot(AudioMixerSnapshotController snapshot)
        {
            if (this.snapshots.Length >= 2)
            {
                AudioMixerSnapshotController item = snapshot;
                Undo.RecordObject(this, "Remove Snapshot");
                List<AudioMixerSnapshotController> list = new List<AudioMixerSnapshotController>(this.snapshots);
                list.Remove(item);
                this.snapshots = list.ToArray();
                Undo.DestroyObjectImmediate(item);
                this.OnSubAssetChanged();
            }
        }

        public void RemoveTargetSnapshot()
        {
            if (this.snapshots.Length >= 2)
            {
                AudioMixerSnapshotController targetSnapshot = this.TargetSnapshot;
                Undo.RecordObject(this, "Remove Snapshot");
                List<AudioMixerSnapshotController> list = new List<AudioMixerSnapshotController>(this.snapshots);
                list.Remove(targetSnapshot);
                this.snapshots = list.ToArray();
                Undo.DestroyObjectImmediate(targetSnapshot);
                this.OnSubAssetChanged();
            }
        }

        public void ReparentSelection(AudioMixerGroupController newParent, AudioMixerGroupController insertAfterThisNode, List<AudioMixerGroupController> selection)
        {
            Undo.RecordObject(newParent, "Change Audio Mixer Group Parent");
            foreach (AudioMixerGroupController controller in this.GetAllAudioGroupsSlow())
            {
                if (controller.children.Intersect<AudioMixerGroupController>(selection).Any<AudioMixerGroupController>())
                {
                    Undo.RecordObject(controller, string.Empty);
                    List<AudioMixerGroupController> list2 = new List<AudioMixerGroupController>(controller.children);
                    foreach (AudioMixerGroupController controller2 in selection)
                    {
                        list2.Remove(controller2);
                    }
                    controller.children = list2.ToArray();
                }
            }
            List<AudioMixerGroupController> list3 = new List<AudioMixerGroupController>(newParent.children);
            int index = list3.IndexOf(insertAfterThisNode) + 1;
            list3.InsertRange(index, selection);
            newParent.children = list3.ToArray();
        }

        public string ResolveExposedParameterPath(GUID parameter, bool getOnlyBasePath)
        {
            if (this.exposedParamCache.ContainsKey(parameter))
            {
                AudioParameterPath path = this.exposedParamCache[parameter];
                return path.ResolveStringPath(getOnlyBasePath);
            }
            foreach (AudioMixerGroupController controller in this.GetAllAudioGroupsSlow())
            {
                if ((controller.GetGUIDForVolume() == parameter) || (controller.GetGUIDForPitch() == parameter))
                {
                    AudioGroupParameterPath path2 = new AudioGroupParameterPath(controller, parameter);
                    this.exposedParamCache[parameter] = path2;
                    return path2.ResolveStringPath(getOnlyBasePath);
                }
                for (int i = 0; i < controller.effects.Length; i++)
                {
                    AudioMixerEffectController effect = controller.effects[i];
                    MixerParameterDefinition[] effectParameters = MixerEffectDefinitions.GetEffectParameters(effect.effectName);
                    for (int j = 0; j < effectParameters.Length; j++)
                    {
                        if (effect.GetGUIDForParameter(effectParameters[j].name) == parameter)
                        {
                            AudioEffectParameterPath path3 = new AudioEffectParameterPath(controller, effect, parameter);
                            this.exposedParamCache[parameter] = path3;
                            return path3.ResolveStringPath(getOnlyBasePath);
                        }
                    }
                }
            }
            return "Error finding Parameter path";
        }

        public void SanitizeGroupViews()
        {
            <SanitizeGroupViews>c__AnonStoreyF yf = new <SanitizeGroupViews>c__AnonStoreyF {
                allGroups = this.GetAllAudioGroupsSlow()
            };
            MixerGroupView[] views = this.views;
            for (int i = 0; i < views.Length; i++)
            {
                if (<>f__am$cache8 == null)
                {
                    <>f__am$cache8 = (x, y) => new <>__AnonType0<GUID, AudioMixerGroupController>(x, y);
                }
                if (<>f__am$cache9 == null)
                {
                    <>f__am$cache9 = <>__TranspIdent0 => <>__TranspIdent0.y.groupID == <>__TranspIdent0.x;
                }
                if (<>f__am$cacheA == null)
                {
                    <>f__am$cacheA = <>__TranspIdent0 => <>__TranspIdent0.x;
                }
                views[i].guids = views[i].guids.SelectMany<GUID, AudioMixerGroupController, <>__AnonType0<GUID, AudioMixerGroupController>>(new Func<GUID, IEnumerable<AudioMixerGroupController>>(yf.<>m__6), <>f__am$cache8).Where<<>__AnonType0<GUID, AudioMixerGroupController>>(<>f__am$cache9).Select<<>__AnonType0<GUID, AudioMixerGroupController>, GUID>(<>f__am$cacheA).ToArray<GUID>();
            }
            this.views = views.ToArray<MixerGroupView>();
        }

        public void SetCurrentViewVisibility(GUID[] guids)
        {
            MixerGroupView[] views = this.views;
            views[this.currentViewIndex].guids = guids;
            this.views = views.ToArray<MixerGroupView>();
            this.SanitizeGroupViews();
        }

        public void SetView(int index)
        {
            if (this.currentViewIndex != index)
            {
                this.ForceSetView(index);
            }
        }

        private int SortFuncForExposedParameters(ExposedAudioParameter p1, ExposedAudioParameter p2)
        {
            return string.CompareOrdinal(this.ResolveExposedParameterPath(p1.guid, true), this.ResolveExposedParameterPath(p2.guid, true));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void UpdateBypass();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void UpdateMuteSolo();
        public static float VolumeToScreenMapping(float value, float screenRange, bool forward)
        {
            float num = GetVolumeSplitPoint() * screenRange;
            float num2 = screenRange - num;
            if (forward)
            {
                return ((value <= 0f) ? ((Mathf.Pow(value / kMinVolume, 1f / kVolumeWarp) * num2) + num) : (num - (Mathf.Pow(value / GetMaxVolume(), 1f / kVolumeWarp) * num)));
            }
            return ((value >= num) ? (Mathf.Pow((value - num) / num2, kVolumeWarp) * kMinVolume) : (Mathf.Pow(1f - (value / num), kVolumeWarp) * GetMaxVolume()));
        }

        public static bool WillChangeOfEffectTargetCauseFeedback(List<AudioMixerGroupController> allGroups, AudioMixerGroupController groupWhoseEffectIsChanged, int effectWhoseTargetIsChanged, AudioMixerEffectController targetToTest, List<ConnectionNode> identifiedLoop)
        {
            Dictionary<object, ConnectionNode> graph = BuildTemporaryGraph(allGroups, groupWhoseEffectIsChanged, groupWhoseEffectIsChanged.effects[effectWhoseTargetIsChanged], targetToTest, null, null, null, null);
            foreach (AudioMixerGroupController controller in allGroups)
            {
                foreach (AudioMixerGroupController controller2 in controller.children)
                {
                    object groupTail = graph[controller2].groupTail;
                    if (!graph[groupTail].targets.Contains(controller))
                    {
                        graph[groupTail].targets.Add(controller);
                    }
                }
            }
            return DoesTheTemporaryGraphHaveAnyCycles(allGroups, identifiedLoop, graph);
        }

        public static bool WillModificationOfTopologyCauseFeedback(List<AudioMixerGroupController> allGroups, List<AudioMixerGroupController> groupsToBeMoved, AudioMixerGroupController newParentForMovedGroups, List<ConnectionNode> identifiedLoop)
        {
            Dictionary<object, ConnectionNode> graph = BuildTemporaryGraph(allGroups, null, null, null, null, null, null, null);
            foreach (AudioMixerGroupController controller in allGroups)
            {
                foreach (AudioMixerGroupController controller2 in controller.children)
                {
                    AudioMixerGroupController item = !groupsToBeMoved.Contains(controller2) ? controller : newParentForMovedGroups;
                    object groupTail = graph[controller2].groupTail;
                    if (!graph[groupTail].targets.Contains(item))
                    {
                        graph[groupTail].targets.Add(item);
                    }
                }
            }
            return DoesTheTemporaryGraphHaveAnyCycles(allGroups, identifiedLoop, graph);
        }

        public static bool WillMovingEffectCauseFeedback(List<AudioMixerGroupController> allGroups, AudioMixerGroupController sourceGroup, int sourceIndex, AudioMixerGroupController targetGroup, int targetIndex, List<ConnectionNode> identifiedLoop)
        {
            Dictionary<object, ConnectionNode> dictionary;
            if (sourceGroup == targetGroup)
            {
                List<AudioMixerEffectController> sourceEffects = sourceGroup.effects.ToList<AudioMixerEffectController>();
                if (!MoveEffect(ref sourceEffects, sourceIndex, ref sourceEffects, targetIndex))
                {
                    return false;
                }
                dictionary = BuildTemporaryGraph(allGroups, null, null, null, sourceGroup, sourceEffects, null, null);
            }
            else
            {
                List<AudioMixerEffectController> list2 = sourceGroup.effects.ToList<AudioMixerEffectController>();
                List<AudioMixerEffectController> targetEffects = targetGroup.effects.ToList<AudioMixerEffectController>();
                if (!MoveEffect(ref list2, sourceIndex, ref targetEffects, targetIndex))
                {
                    return false;
                }
                dictionary = BuildTemporaryGraph(allGroups, null, null, null, sourceGroup, list2, targetGroup, targetEffects);
            }
            foreach (AudioMixerGroupController controller in allGroups)
            {
                foreach (AudioMixerGroupController controller2 in controller.children)
                {
                    object groupTail = dictionary[controller2].groupTail;
                    if (!dictionary[groupTail].targets.Contains(controller))
                    {
                        dictionary[groupTail].targets.Add(controller);
                    }
                }
            }
            return DoesTheTemporaryGraphHaveAnyCycles(allGroups, identifiedLoop, dictionary);
        }

        public AudioMixerGroupController[] allGroups
        {
            get
            {
                List<AudioMixerGroupController> groups = new List<AudioMixerGroupController>();
                GetGroupsRecurse(this.masterGroup, groups);
                return groups.ToArray();
            }
        }

        public List<AudioMixerGroupController> CachedSelection
        {
            get
            {
                if (this.m_CachedSelection == null)
                {
                    this.m_CachedSelection = new List<AudioMixerGroupController>();
                }
                return this.m_CachedSelection;
            }
        }

        public int currentViewIndex { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        private Dictionary<GUID, AudioParameterPath> exposedParamCache
        {
            get
            {
                if (this.m_ExposedParamPathCache == null)
                {
                    this.m_ExposedParamPathCache = new Dictionary<GUID, AudioParameterPath>();
                }
                return this.m_ExposedParamPathCache;
            }
        }

        public ExposedAudioParameter[] exposedParameters { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool isSuspended { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public AudioMixerGroupController masterGroup { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int numExposedParameters { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public AudioMixerSnapshotController[] snapshots { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public AudioMixerSnapshot startSnapshot { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public AudioMixerSnapshotController TargetSnapshot { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public MixerGroupView[] views { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [CompilerGenerated]
        private sealed class <AreAnyOfTheGroupsInTheListAncestors>c__AnonStoreyD
        {
            internal AudioMixerController <>f__this;
            internal List<AudioMixerGroupController> groups;

            internal bool <>m__4(AudioMixerGroupController g)
            {
                return this.<>f__this.IsChildOf(g, this.groups);
            }
        }

        [CompilerGenerated]
        private sealed class <ContainsExposedParameter>c__AnonStoreyB
        {
            internal GUID parameter;

            internal bool <>m__2(ExposedAudioParameter val)
            {
                return (val.guid == this.parameter);
            }
        }

        [CompilerGenerated]
        private sealed class <GetCurrentViewGroupList>c__AnonStorey10
        {
            internal MixerGroupView view;

            internal bool <>m__A(AudioMixerGroupController g)
            {
                return this.view.guids.Contains<GUID>(g.groupID);
            }
        }

        [CompilerGenerated]
        private sealed class <RemoveAncestorGroups>c__AnonStoreyE
        {
            internal AudioMixerController <>f__this;
            internal List<AudioMixerGroupController> groups;

            internal bool <>m__5(AudioMixerGroupController g)
            {
                return this.<>f__this.IsChildOf(g, this.groups);
            }
        }

        [CompilerGenerated]
        private sealed class <RemoveExposedParameter>c__AnonStoreyC
        {
            internal GUID parameter;

            internal bool <>m__3(ExposedAudioParameter val)
            {
                return (val.guid != this.parameter);
            }
        }

        [CompilerGenerated]
        private sealed class <SanitizeGroupViews>c__AnonStoreyF
        {
            internal List<AudioMixerGroupController> allGroups;

            internal IEnumerable<AudioMixerGroupController> <>m__6(GUID x)
            {
                return this.allGroups;
            }
        }

        public class ConnectionNode
        {
            public AudioMixerEffectController effect;
            public AudioMixerGroupController group;
            public object groupTail;
            public List<object> targets = new List<object>();
            public bool visited;

            public string GetDisplayString()
            {
                string displayString = this.group.GetDisplayString();
                if (this.effect != null)
                {
                    displayString = displayString + AudioMixerController.s_GroupEffectDisplaySeperator + AudioMixerController.FixNameForPopupMenu(this.effect.effectName);
                }
                return displayString;
            }
        }
    }
}

