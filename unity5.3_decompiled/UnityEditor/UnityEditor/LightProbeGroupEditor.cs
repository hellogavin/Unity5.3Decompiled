namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Xml.Serialization;
    using UnityEngine;

    internal class LightProbeGroupEditor : IEditablePoint
    {
        [CompilerGenerated]
        private static Func<int, int> <>f__am$cacheB;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map1B;
        private static readonly Color kCloudColor = new Color(0.7843137f, 0.7843137f, 0.07843138f, 0.85f);
        private static readonly Color kSelectedCloudColor = new Color(0.3f, 0.6f, 1f, 1f);
        private bool m_Editing;
        private readonly LightProbeGroup m_Group;
        private Vector3 m_LastPosition = Vector3.zero;
        private Quaternion m_LastRotation = Quaternion.identity;
        private Vector3 m_LastScale = Vector3.one;
        private List<int> m_Selection = new List<int>();
        private readonly LightProbeGroupSelection m_SerializedSelectedProbes;
        private bool m_ShouldRecalculateTetrahedra;
        private List<Vector3> m_SourcePositions;

        public LightProbeGroupEditor(LightProbeGroup group)
        {
            this.m_Group = group;
            this.MarkTetrahedraDirty();
            this.m_SerializedSelectedProbes = ScriptableObject.CreateInstance<LightProbeGroupSelection>();
            this.m_SerializedSelectedProbes.hideFlags = HideFlags.HideAndDontSave;
        }

        public void AddProbe(Vector3 position)
        {
            Object[] objectsToUndo = new Object[] { this.m_Group, this.m_SerializedSelectedProbes };
            Undo.RegisterCompleteObjectUndo(objectsToUndo, "Add Probe");
            this.m_SourcePositions.Add(position);
            this.SelectProbe(this.m_SourcePositions.Count - 1);
            this.MarkTetrahedraDirty();
        }

        private static bool CanPasteProbes()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Vector3[]));
                StringReader textReader = new StringReader(GUIUtility.systemCopyBuffer);
                serializer.Deserialize(textReader);
                textReader.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void CopySelectedProbes()
        {
            IEnumerable<Vector3> enumerable = this.SelectedProbePositions();
            XmlSerializer serializer = new XmlSerializer(typeof(Vector3[]));
            StringWriter writer = new StringWriter();
            serializer.Serialize((TextWriter) writer, (from pos in enumerable select this.m_Group.transform.TransformPoint(pos)).ToArray<Vector3>());
            writer.Close();
            GUIUtility.systemCopyBuffer = writer.ToString();
        }

        public void DeselectProbes()
        {
            this.m_Selection.Clear();
        }

        private void DrawTetrahedra()
        {
            if ((Event.current.type == EventType.Repaint) && (SceneView.lastActiveSceneView != null))
            {
                LightmapVisualization.DrawTetrahedra(this.m_ShouldRecalculateTetrahedra, SceneView.lastActiveSceneView.camera.transform.position);
                this.m_ShouldRecalculateTetrahedra = false;
            }
        }

        public void DuplicateSelectedProbes()
        {
            if (this.m_Selection.Count != 0)
            {
                Object[] objectsToUndo = new Object[] { this.m_Group, this.m_SerializedSelectedProbes };
                Undo.RegisterCompleteObjectUndo(objectsToUndo, "Duplicate Probes");
                IEnumerator<Vector3> enumerator = this.SelectedProbePositions().GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Vector3 current = enumerator.Current;
                        this.m_SourcePositions.Add(current);
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
                this.MarkTetrahedraDirty();
            }
        }

        public Color GetDefaultColor()
        {
            return kCloudColor;
        }

        public float GetPointScale()
        {
            return (10f * AnnotationUtility.iconSize);
        }

        public Vector3 GetPosition(int idx)
        {
            return this.m_SourcePositions[idx];
        }

        public IEnumerable<Vector3> GetPositions()
        {
            return this.m_SourcePositions;
        }

        public Color GetSelectedColor()
        {
            return kSelectedCloudColor;
        }

        public Vector3[] GetSelectedPositions()
        {
            Vector3[] vectorArray = new Vector3[this.SelectedCount];
            for (int i = 0; i < this.SelectedCount; i++)
            {
                vectorArray[i] = this.m_SourcePositions[this.m_Selection[i]];
            }
            return vectorArray;
        }

        public Vector3[] GetUnselectedPositions()
        {
            return this.m_SourcePositions.Where<Vector3>((t, i) => !this.m_Selection.Contains(i)).ToArray<Vector3>();
        }

        public Vector3 GetWorldPosition(int idx)
        {
            return this.m_Group.transform.TransformPoint(this.m_SourcePositions[idx]);
        }

        public void HandleEditMenuHotKeyCommands()
        {
            if ((Event.current.type == EventType.ValidateCommand) || (Event.current.type == EventType.ExecuteCommand))
            {
                bool flag = Event.current.type == EventType.ExecuteCommand;
                string commandName = Event.current.commandName;
                if (commandName != null)
                {
                    int num;
                    if (<>f__switch$map1B == null)
                    {
                        Dictionary<string, int> dictionary = new Dictionary<string, int>(6);
                        dictionary.Add("SoftDelete", 0);
                        dictionary.Add("Delete", 0);
                        dictionary.Add("Duplicate", 1);
                        dictionary.Add("SelectAll", 2);
                        dictionary.Add("Cut", 3);
                        dictionary.Add("Copy", 4);
                        <>f__switch$map1B = dictionary;
                    }
                    if (<>f__switch$map1B.TryGetValue(commandName, out num))
                    {
                        switch (num)
                        {
                            case 0:
                                if (flag)
                                {
                                    this.RemoveSelectedProbes();
                                }
                                Event.current.Use();
                                break;

                            case 1:
                                if (flag)
                                {
                                    this.DuplicateSelectedProbes();
                                }
                                Event.current.Use();
                                break;

                            case 2:
                                if (flag)
                                {
                                    this.SelectAllProbes();
                                }
                                Event.current.Use();
                                break;

                            case 3:
                                if (flag)
                                {
                                    this.CopySelectedProbes();
                                    this.RemoveSelectedProbes();
                                }
                                Event.current.Use();
                                break;

                            case 4:
                                if (flag)
                                {
                                    this.CopySelectedProbes();
                                }
                                Event.current.Use();
                                break;
                        }
                    }
                }
            }
        }

        public void MarkTetrahedraDirty()
        {
            this.m_ShouldRecalculateTetrahedra = true;
        }

        public bool OnSceneGUI(Transform transform)
        {
            if (Event.current.type == EventType.Layout)
            {
                if (((this.m_LastPosition != this.m_Group.transform.position) || (this.m_LastRotation != this.m_Group.transform.rotation)) || (this.m_LastScale != this.m_Group.transform.localScale))
                {
                    this.MarkTetrahedraDirty();
                }
                this.m_LastPosition = this.m_Group.transform.position;
                this.m_LastRotation = this.m_Group.transform.rotation;
                this.m_LastScale = this.m_Group.transform.localScale;
            }
            bool firstSelect = false;
            if ((((Event.current.type == EventType.MouseDown) && (Event.current.button == 0)) && ((this.SelectedCount == 0) && (PointEditor.FindNearest(Event.current.mousePosition, transform, this) != -1))) && !this.m_Editing)
            {
                this.m_Editing = true;
                firstSelect = true;
            }
            bool flag3 = Event.current.type == EventType.MouseUp;
            if (this.m_Editing && PointEditor.SelectPoints(this, transform, ref this.m_Selection, firstSelect))
            {
                Object[] objectsToUndo = new Object[] { this.m_Group, this.m_SerializedSelectedProbes };
                Undo.RegisterCompleteObjectUndo(objectsToUndo, "Select Probes");
            }
            if ((this.m_Editing && flag3) && (this.SelectedCount == 0))
            {
                this.m_Editing = false;
                this.MarkTetrahedraDirty();
            }
            if (((Event.current.type == EventType.ValidateCommand) || (Event.current.type == EventType.ExecuteCommand)) && (Event.current.commandName == "Paste"))
            {
                if ((Event.current.type == EventType.ValidateCommand) && CanPasteProbes())
                {
                    Event.current.Use();
                }
                if ((Event.current.type == EventType.ExecuteCommand) && this.PasteProbes())
                {
                    Event.current.Use();
                    this.m_Editing = true;
                }
            }
            this.DrawTetrahedra();
            PointEditor.Draw(this, transform, this.m_Selection, true);
            if (this.m_Editing)
            {
                this.HandleEditMenuHotKeyCommands();
                if (this.m_Editing && PointEditor.MovePoints(this, transform, this.m_Selection))
                {
                    Object[] objArray2 = new Object[] { this.m_Group, this.m_SerializedSelectedProbes };
                    Undo.RegisterCompleteObjectUndo(objArray2, "Move Probes");
                    if (LightmapVisualization.dynamicUpdateLightProbes)
                    {
                        this.MarkTetrahedraDirty();
                    }
                }
                if ((this.m_Editing && flag3) && !LightmapVisualization.dynamicUpdateLightProbes)
                {
                    this.MarkTetrahedraDirty();
                }
            }
            return this.m_Editing;
        }

        private bool PasteProbes()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Vector3[]));
                StringReader textReader = new StringReader(GUIUtility.systemCopyBuffer);
                Vector3[] vectorArray = (Vector3[]) serializer.Deserialize(textReader);
                textReader.Close();
                if (vectorArray.Length == 0)
                {
                    return false;
                }
                Object[] objectsToUndo = new Object[] { this.m_Group, this.m_SerializedSelectedProbes };
                Undo.RegisterCompleteObjectUndo(objectsToUndo, "Paste Probes");
                int count = this.m_SourcePositions.Count;
                foreach (Vector3 vector in vectorArray)
                {
                    this.m_SourcePositions.Add(this.m_Group.transform.InverseTransformPoint(vector));
                }
                this.DeselectProbes();
                for (int i = count; i < (count + vectorArray.Length); i++)
                {
                    this.SelectProbe(i);
                }
                this.MarkTetrahedraDirty();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void PullProbePositions()
        {
            this.m_SourcePositions = new List<Vector3>(this.m_Group.probePositions);
            this.m_Selection = new List<int>(this.m_SerializedSelectedProbes.m_Selection);
        }

        public void PushProbePositions()
        {
            bool flag = false;
            if ((this.m_Group.probePositions.Length != this.m_SourcePositions.Count) || (this.m_SerializedSelectedProbes.m_Selection.Count != this.m_Selection.Count))
            {
                flag = true;
            }
            if (!flag)
            {
                if (this.m_Group.probePositions.Where<Vector3>((t, i) => (t != this.m_SourcePositions[i])).Any<Vector3>())
                {
                    flag = true;
                }
                for (int j = 0; j < this.m_SerializedSelectedProbes.m_Selection.Count; j++)
                {
                    if (this.m_SerializedSelectedProbes.m_Selection[j] != this.m_Selection[j])
                    {
                        flag = true;
                    }
                }
            }
            if (flag)
            {
                this.m_Group.probePositions = this.m_SourcePositions.ToArray();
                this.m_SerializedSelectedProbes.m_Selection = this.m_Selection;
            }
        }

        public void RemoveSelectedProbes()
        {
            if (this.m_Selection.Count != 0)
            {
                Object[] objectsToUndo = new Object[] { this.m_Group, this.m_SerializedSelectedProbes };
                Undo.RegisterCompleteObjectUndo(objectsToUndo, "Delete Probes");
                if (<>f__am$cacheB == null)
                {
                    <>f__am$cacheB = x => x;
                }
                IEnumerator<int> enumerator = this.m_Selection.OrderByDescending<int, int>(<>f__am$cacheB).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        int current = enumerator.Current;
                        this.m_SourcePositions.RemoveAt(current);
                    }
                }
                finally
                {
                    if (enumerator == null)
                    {
                    }
                    enumerator.Dispose();
                }
                this.DeselectProbes();
                this.MarkTetrahedraDirty();
            }
        }

        public void SelectAllProbes()
        {
            this.DeselectProbes();
            for (int i = 0; i < this.m_SourcePositions.Count; i++)
            {
                this.SelectProbe(i);
            }
        }

        private IEnumerable<Vector3> SelectedProbePositions()
        {
            return (from t in this.m_Selection select this.m_SourcePositions[t]).ToList<Vector3>();
        }

        private void SelectProbe(int i)
        {
            if (!this.m_Selection.Contains(i))
            {
                this.m_Selection.Add(i);
            }
        }

        public void SetEditing(bool editing)
        {
            this.m_Editing = editing;
        }

        public void SetPosition(int idx, Vector3 position)
        {
            if (this.m_SourcePositions[idx] != position)
            {
                this.m_SourcePositions[idx] = position;
            }
        }

        public static void TetrahedralizeSceneProbes(out Vector3[] positions, out int[] indices)
        {
            LightProbeGroup[] groupArray = Object.FindObjectsOfType(typeof(LightProbeGroup)) as LightProbeGroup[];
            if (groupArray == null)
            {
                positions = new Vector3[0];
                indices = new int[0];
            }
            else
            {
                List<Vector3> list = new List<Vector3>();
                foreach (LightProbeGroup group in groupArray)
                {
                    foreach (Vector3 vector in group.probePositions)
                    {
                        Vector3 item = group.transform.TransformPoint(vector);
                        list.Add(item);
                    }
                }
                if (list.Count == 0)
                {
                    positions = new Vector3[0];
                    indices = new int[0];
                }
                else
                {
                    Lightmapping.Tetrahedralize(list.ToArray(), out indices, out positions);
                }
            }
        }

        public void UpdateSelectedPosition(int idx, Vector3 position)
        {
            if (idx <= (this.SelectedCount - 1))
            {
                this.m_SourcePositions[this.m_Selection[idx]] = position;
            }
        }

        public int Count
        {
            get
            {
                return this.m_SourcePositions.Count;
            }
        }

        public int SelectedCount
        {
            get
            {
                return this.m_Selection.Count;
            }
        }

        public Bounds selectedProbeBounds
        {
            get
            {
                if (this.m_Selection.Count == 0)
                {
                    return new Bounds();
                }
                if (this.m_Selection.Count == 1)
                {
                    return new Bounds(this.GetWorldPosition(this.m_Selection[0]), new Vector3(1f, 1f, 1f));
                }
                Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                foreach (int num in this.m_Selection)
                {
                    Vector3 worldPosition = this.GetWorldPosition(num);
                    if (worldPosition.x < min.x)
                    {
                        min.x = worldPosition.x;
                    }
                    if (worldPosition.y < min.y)
                    {
                        min.y = worldPosition.y;
                    }
                    if (worldPosition.z < min.z)
                    {
                        min.z = worldPosition.z;
                    }
                    if (worldPosition.x > max.x)
                    {
                        max.x = worldPosition.x;
                    }
                    if (worldPosition.y > max.y)
                    {
                        max.y = worldPosition.y;
                    }
                    if (worldPosition.z > max.z)
                    {
                        max.z = worldPosition.z;
                    }
                }
                Bounds bounds = new Bounds();
                bounds.SetMinMax(min, max);
                return bounds;
            }
        }
    }
}

