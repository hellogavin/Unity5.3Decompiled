namespace UnityEngine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal class InternalStaticBatchingUtility
    {
        [CompilerGenerated]
        private static Func<Material, bool> <>f__am$cache0;
        private const string CombinedMeshPrefix = "Combined Mesh";
        private const int MaxVerticesInBatch = 0xfa00;

        public static void Combine(GameObject staticBatchRoot, bool combineOnlyStatic, bool isEditorPostprocessScene)
        {
            GameObject[] objArray = (GameObject[]) Object.FindObjectsOfType(typeof(GameObject));
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject obj2 in objArray)
            {
                if (((staticBatchRoot == null) || obj2.transform.IsChildOf(staticBatchRoot.transform)) && (!combineOnlyStatic || obj2.isStaticBatchable))
                {
                    list.Add(obj2);
                }
            }
            CombineGameObjects(list.ToArray(), staticBatchRoot, isEditorPostprocessScene);
        }

        public static void CombineGameObjects(GameObject[] gos, GameObject staticBatchRoot, bool isEditorPostprocessScene)
        {
            Matrix4x4 identity = Matrix4x4.identity;
            Transform staticBatchRootTransform = null;
            if (staticBatchRoot != null)
            {
                identity = staticBatchRoot.transform.worldToLocalMatrix;
                staticBatchRootTransform = staticBatchRoot.transform;
            }
            int batchIndex = 0;
            int num2 = 0;
            List<MeshSubsetCombineUtility.MeshInstance> meshes = new List<MeshSubsetCombineUtility.MeshInstance>();
            List<MeshSubsetCombineUtility.SubMeshInstance> subsets = new List<MeshSubsetCombineUtility.SubMeshInstance>();
            List<GameObject> subsetGOs = new List<GameObject>();
            Array.Sort(gos, new SortGO());
            foreach (GameObject obj2 in gos)
            {
                MeshFilter component = obj2.GetComponent(typeof(MeshFilter)) as MeshFilter;
                if (component != null)
                {
                    Mesh sharedMesh = component.sharedMesh;
                    if ((sharedMesh != null) && (isEditorPostprocessScene || sharedMesh.canAccess))
                    {
                        Renderer renderer = component.GetComponent<Renderer>();
                        if (((renderer != null) && renderer.enabled) && (renderer.staticBatchIndex == 0))
                        {
                            Material[] sharedMaterials = component.GetComponent<Renderer>().sharedMaterials;
                            if (<>f__am$cache0 == null)
                            {
                                <>f__am$cache0 = m => ((m != null) && (m.shader != null)) && (m.shader.disableBatching != DisableBatchingType.False);
                            }
                            if (!sharedMaterials.Any<Material>(<>f__am$cache0))
                            {
                                if ((num2 + component.sharedMesh.vertexCount) > 0xfa00)
                                {
                                    MakeBatch(meshes, subsets, subsetGOs, staticBatchRootTransform, batchIndex++);
                                    meshes.Clear();
                                    subsets.Clear();
                                    subsetGOs.Clear();
                                    num2 = 0;
                                }
                                MeshSubsetCombineUtility.MeshInstance item = new MeshSubsetCombineUtility.MeshInstance {
                                    meshInstanceID = sharedMesh.GetInstanceID(),
                                    rendererInstanceID = renderer.GetInstanceID()
                                };
                                MeshRenderer renderer2 = renderer as MeshRenderer;
                                if ((renderer2 != null) && (renderer2.additionalVertexStreams != null))
                                {
                                    item.additionalVertexStreamsMeshInstanceID = renderer2.additionalVertexStreams.GetInstanceID();
                                }
                                item.transform = identity * component.transform.localToWorldMatrix;
                                item.lightmapScaleOffset = renderer.lightmapScaleOffset;
                                item.realtimeLightmapScaleOffset = renderer.realtimeLightmapScaleOffset;
                                meshes.Add(item);
                                if (sharedMaterials.Length > sharedMesh.subMeshCount)
                                {
                                    Debug.LogWarning(string.Concat(new object[] { "Mesh has more materials (", sharedMaterials.Length, ") than subsets (", sharedMesh.subMeshCount, ")" }), component.GetComponent<Renderer>());
                                    Material[] materialArray2 = new Material[sharedMesh.subMeshCount];
                                    for (int j = 0; j < sharedMesh.subMeshCount; j++)
                                    {
                                        materialArray2[j] = component.GetComponent<Renderer>().sharedMaterials[j];
                                    }
                                    component.GetComponent<Renderer>().sharedMaterials = materialArray2;
                                    sharedMaterials = materialArray2;
                                }
                                for (int i = 0; i < Math.Min(sharedMaterials.Length, sharedMesh.subMeshCount); i++)
                                {
                                    MeshSubsetCombineUtility.SubMeshInstance instance2 = new MeshSubsetCombineUtility.SubMeshInstance {
                                        meshInstanceID = component.sharedMesh.GetInstanceID(),
                                        vertexOffset = num2,
                                        subMeshIndex = i,
                                        gameObjectInstanceID = obj2.GetInstanceID(),
                                        transform = item.transform
                                    };
                                    subsets.Add(instance2);
                                    subsetGOs.Add(obj2);
                                }
                                num2 += sharedMesh.vertexCount;
                            }
                        }
                    }
                }
            }
            MakeBatch(meshes, subsets, subsetGOs, staticBatchRootTransform, batchIndex);
        }

        public static void CombineRoot(GameObject staticBatchRoot)
        {
            Combine(staticBatchRoot, false, false);
        }

        private static void MakeBatch(List<MeshSubsetCombineUtility.MeshInstance> meshes, List<MeshSubsetCombineUtility.SubMeshInstance> subsets, List<GameObject> subsetGOs, Transform staticBatchRootTransform, int batchIndex)
        {
            if (meshes.Count >= 2)
            {
                MeshSubsetCombineUtility.MeshInstance[] instanceArray = meshes.ToArray();
                MeshSubsetCombineUtility.SubMeshInstance[] submeshes = subsets.ToArray();
                string meshName = "Combined Mesh";
                meshName = meshName + " (root: " + ((staticBatchRootTransform == null) ? "scene" : staticBatchRootTransform.name) + ")";
                if (batchIndex > 0)
                {
                    meshName = meshName + " " + (batchIndex + 1);
                }
                Mesh combinedMesh = StaticBatchingUtility.InternalCombineVertices(instanceArray, meshName);
                StaticBatchingUtility.InternalCombineIndices(submeshes, combinedMesh);
                int subSetIndexForMaterial = 0;
                for (int i = 0; i < submeshes.Length; i++)
                {
                    MeshSubsetCombineUtility.SubMeshInstance instance = submeshes[i];
                    GameObject obj2 = subsetGOs[i];
                    Mesh mesh2 = combinedMesh;
                    MeshFilter component = (MeshFilter) obj2.GetComponent(typeof(MeshFilter));
                    component.sharedMesh = mesh2;
                    Renderer renderer = obj2.GetComponent<Renderer>();
                    renderer.SetSubsetIndex(instance.subMeshIndex, subSetIndexForMaterial);
                    renderer.staticBatchRootTransform = staticBatchRootTransform;
                    renderer.enabled = false;
                    renderer.enabled = true;
                    MeshRenderer renderer2 = renderer as MeshRenderer;
                    if (renderer2 != null)
                    {
                        renderer2.additionalVertexStreams = null;
                    }
                    subSetIndexForMaterial++;
                }
            }
        }

        internal class SortGO : IComparer
        {
            private static int GetLightmapIndex(Renderer renderer)
            {
                if (renderer == null)
                {
                    return -1;
                }
                return renderer.lightmapIndex;
            }

            private static int GetMaterialId(Renderer renderer)
            {
                if ((renderer != null) && (renderer.sharedMaterial != null))
                {
                    return renderer.sharedMaterial.GetInstanceID();
                }
                return 0;
            }

            private static Renderer GetRenderer(GameObject go)
            {
                if (go == null)
                {
                    return null;
                }
                MeshFilter component = go.GetComponent(typeof(MeshFilter)) as MeshFilter;
                if (component == null)
                {
                    return null;
                }
                return component.GetComponent<Renderer>();
            }

            int IComparer.Compare(object a, object b)
            {
                if (a == b)
                {
                    return 0;
                }
                Renderer renderer = GetRenderer(a as GameObject);
                Renderer renderer2 = GetRenderer(b as GameObject);
                int num = GetMaterialId(renderer).CompareTo(GetMaterialId(renderer2));
                if (num == 0)
                {
                    num = GetLightmapIndex(renderer).CompareTo(GetLightmapIndex(renderer2));
                }
                return num;
            }
        }
    }
}

