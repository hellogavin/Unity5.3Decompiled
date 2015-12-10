namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditorInternal;
    using UnityEngine;

    internal static class SpriteUtility
    {
        [CompilerGenerated]
        private static Comparison<Object> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<Object, bool> <>f__am$cache3;
        private static DragType s_DragType;
        private static List<Object> s_SceneDragObjects;

        public static void AddAnimationToGO(GameObject go, Sprite[] frames)
        {
            if ((frames != null) && (frames.Length > 0))
            {
                SpriteRenderer component = go.GetComponent<SpriteRenderer>();
                if (component == null)
                {
                    Debug.LogWarning("There should be a SpriteRenderer in dragged object");
                    component = go.AddComponent<SpriteRenderer>();
                }
                component.sprite = frames[0];
                if (frames.Length > 1)
                {
                    Analytics.Event("Sprite Drag and Drop", "Drop multiple sprites to scene", "null", 1);
                    if (!CreateAnimation(go, frames))
                    {
                        Debug.LogError("Failed to create animation for dragged object");
                    }
                }
                else
                {
                    Analytics.Event("Sprite Drag and Drop", "Drop single sprite to scene", "null", 1);
                }
            }
        }

        private static void AddSpriteAnimationToClip(AnimationClip newClip, Object[] frames)
        {
            newClip.frameRate = 12f;
            ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[frames.Length];
            for (int i = 0; i < keyframes.Length; i++)
            {
                keyframes[i] = new ObjectReferenceKeyframe();
                keyframes[i].value = RemapObjectToSprite(frames[i]);
                keyframes[i].time = ((float) i) / newClip.frameRate;
            }
            EditorCurveBinding binding = EditorCurveBinding.PPtrCurve(string.Empty, typeof(SpriteRenderer), "m_Sprite");
            AnimationUtility.SetObjectReferenceCurve(newClip, binding, keyframes);
        }

        private static void CleanUp()
        {
            if (s_SceneDragObjects != null)
            {
                s_SceneDragObjects.Clear();
                s_SceneDragObjects = null;
            }
            HandleUtility.ignoreRaySnapObjects = null;
            s_DragType = DragType.NotInitialized;
        }

        private static bool CreateAnimation(GameObject gameObject, Object[] frames)
        {
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = (a, b) => EditorUtility.NaturalCompare(a.name, b.name);
            }
            Array.Sort<Object>(frames, <>f__am$cache2);
            if (AnimationWindowUtility.EnsureActiveAnimationPlayer(gameObject) == null)
            {
                return false;
            }
            Animator closestAnimatorInParents = AnimationWindowUtility.GetClosestAnimatorInParents(gameObject.transform);
            if (closestAnimatorInParents == null)
            {
                return false;
            }
            AnimationClip newClip = AnimationWindowUtility.CreateNewClip(gameObject.name);
            if (newClip == null)
            {
                return false;
            }
            AddSpriteAnimationToClip(newClip, frames);
            return AnimationWindowUtility.AddClipToAnimatorComponent(closestAnimatorInParents, newClip);
        }

        public static GameObject CreateDragGO(Sprite frame, Vector3 position)
        {
            string name = !string.IsNullOrEmpty(frame.name) ? frame.name : "Sprite";
            GameObject obj2 = new GameObject(GameObjectUtility.GetUniqueNameForSibling(null, name));
            obj2.AddComponent<SpriteRenderer>().sprite = frame;
            obj2.transform.position = position;
            return obj2;
        }

        public static GameObject DropSpriteToSceneToCreateGO(Sprite sprite, Vector3 position)
        {
            GameObject obj2 = new GameObject(!string.IsNullOrEmpty(sprite.name) ? sprite.name : "Sprite");
            obj2.AddComponent<SpriteRenderer>().sprite = sprite;
            obj2.transform.position = position;
            Selection.activeObject = obj2;
            return obj2;
        }

        private static void ForcedImportFor(string newPath)
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                AssetDatabase.ImportAsset(newPath);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        private static Sprite GenerateDefaultSprite(Texture2D texture)
        {
            string assetPath = AssetDatabase.GetAssetPath(texture);
            TextureImporter atPath = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if ((atPath.textureType != TextureImporterType.Sprite) && (atPath.textureType != TextureImporterType.Advanced))
            {
                return null;
            }
            if (atPath.spriteImportMode == SpriteImportMode.None)
            {
                if (atPath.textureType == TextureImporterType.Advanced)
                {
                    return null;
                }
                atPath.spriteImportMode = SpriteImportMode.Single;
                AssetDatabase.WriteImportSettingsIfDirty(assetPath);
                ForcedImportFor(assetPath);
            }
            Object obj2 = null;
            try
            {
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = t => t is Sprite;
                }
                obj2 = AssetDatabase.LoadAllAssetsAtPath(assetPath).First<Object>(<>f__am$cache3);
            }
            catch (Exception)
            {
                Debug.LogWarning("Texture being dragged has no Sprites.");
            }
            return (obj2 as Sprite);
        }

        public static Sprite[] GetSpriteFromDraggedPathsOrObjects()
        {
            List<Sprite> list = new List<Sprite>();
            foreach (Object obj2 in DragAndDrop.objectReferences)
            {
                if (AssetDatabase.Contains(obj2))
                {
                    if (obj2 is Sprite)
                    {
                        list.Add(obj2 as Sprite);
                    }
                    else if (obj2 is Texture2D)
                    {
                        list.AddRange(TextureToSprites(obj2 as Texture2D));
                    }
                }
            }
            if (list.Count > 0)
            {
                return list.ToArray();
            }
            Sprite sprite = HandleExternalDrag(Event.current.type == EventType.DragPerform);
            if (sprite != null)
            {
                return new Sprite[] { sprite };
            }
            return null;
        }

        public static Sprite[] GetSpritesFromDraggedObjects()
        {
            List<Sprite> list = new List<Sprite>();
            foreach (Object obj2 in DragAndDrop.objectReferences)
            {
                if (obj2.GetType() == typeof(Sprite))
                {
                    list.Add(obj2 as Sprite);
                }
                else if (obj2.GetType() == typeof(Texture2D))
                {
                    list.AddRange(TextureToSprites(obj2 as Texture2D));
                }
            }
            return list.ToArray();
        }

        private static Sprite HandleExternalDrag(bool perform)
        {
            if (DragAndDrop.paths.Length == 0)
            {
                return null;
            }
            string path = DragAndDrop.paths[0];
            if (!ValidPathForTextureAsset(path))
            {
                return null;
            }
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            if (!perform)
            {
                return null;
            }
            string to = AssetDatabase.GenerateUniqueAssetPath(Path.Combine("Assets", FileUtil.GetLastPathNameComponent(path)));
            if (to.Length <= 0)
            {
                return null;
            }
            FileUtil.CopyFileOrDirectory(path, to);
            ForcedImportFor(to);
            return GenerateDefaultSprite(AssetDatabase.LoadMainAssetAtPath(to) as Texture2D);
        }

        public static void OnSceneDrag(SceneView sceneView)
        {
            Event current = Event.current;
            if (((current.type == EventType.DragUpdated) || (current.type == EventType.DragPerform)) || (current.type == EventType.DragExited))
            {
                if (!sceneView.in2DMode)
                {
                    GameObject obj2 = HandleUtility.PickGameObject(Event.current.mousePosition, true);
                    if (((obj2 != null) && (DragAndDrop.objectReferences.Length == 1)) && ((DragAndDrop.objectReferences[0] is Texture) && (obj2.GetComponent<Renderer>() != null)))
                    {
                        return;
                    }
                }
                EventType type2 = current.type;
                if (type2 == EventType.DragUpdated)
                {
                    DragType type = !current.alt ? DragType.SpriteAnimation : DragType.CreateMultiple;
                    if ((s_DragType != type) || (s_SceneDragObjects == null))
                    {
                        Sprite[] spriteFromDraggedPathsOrObjects = GetSpriteFromDraggedPathsOrObjects();
                        if ((spriteFromDraggedPathsOrObjects == null) || (spriteFromDraggedPathsOrObjects.Length == 0))
                        {
                            return;
                        }
                        Sprite sprite = spriteFromDraggedPathsOrObjects[0];
                        if (sprite == null)
                        {
                            return;
                        }
                        if (s_DragType != DragType.NotInitialized)
                        {
                            CleanUp();
                        }
                        s_DragType = type;
                        s_SceneDragObjects = new List<Object>();
                        if (s_DragType == DragType.CreateMultiple)
                        {
                            foreach (Sprite sprite2 in spriteFromDraggedPathsOrObjects)
                            {
                                s_SceneDragObjects.Add(CreateDragGO(sprite2, Vector3.zero));
                            }
                        }
                        else
                        {
                            s_SceneDragObjects.Add(CreateDragGO(spriteFromDraggedPathsOrObjects[0], Vector3.zero));
                        }
                        List<Transform> list = new List<Transform>();
                        foreach (GameObject obj3 in s_SceneDragObjects)
                        {
                            list.AddRange(obj3.GetComponentsInChildren<Transform>());
                            obj3.hideFlags = HideFlags.HideInHierarchy;
                        }
                        HandleUtility.ignoreRaySnapObjects = list.ToArray();
                    }
                    Vector3 zero = Vector3.zero;
                    zero = HandleUtility.GUIPointToWorldRay(current.mousePosition).GetPoint(10f);
                    if (sceneView.in2DMode)
                    {
                        zero.z = 0f;
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        object obj4 = HandleUtility.RaySnap(HandleUtility.GUIPointToWorldRay(current.mousePosition));
                        if (obj4 != null)
                        {
                            RaycastHit hit = (RaycastHit) obj4;
                            zero = hit.point;
                        }
                    }
                    foreach (GameObject obj5 in s_SceneDragObjects)
                    {
                        obj5.transform.position = zero;
                    }
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    current.Use();
                }
                else if (type2 == EventType.DragPerform)
                {
                    Sprite[] frames = GetSpriteFromDraggedPathsOrObjects();
                    if ((frames != null) && (s_SceneDragObjects != null))
                    {
                        if (s_DragType == DragType.SpriteAnimation)
                        {
                            AddAnimationToGO((GameObject) s_SceneDragObjects[0], frames);
                        }
                        foreach (GameObject obj6 in s_SceneDragObjects)
                        {
                            Undo.RegisterCreatedObjectUndo(obj6, "Create Sprite");
                            obj6.hideFlags = HideFlags.None;
                        }
                        Selection.objects = s_SceneDragObjects.ToArray();
                        CleanUp();
                        current.Use();
                    }
                }
                else if ((type2 == EventType.DragExited) && ((s_SceneDragObjects != null) && (s_SceneDragObjects != null)))
                {
                    foreach (GameObject obj7 in s_SceneDragObjects)
                    {
                        Object.DestroyImmediate(obj7, false);
                    }
                    CleanUp();
                    current.Use();
                }
            }
        }

        public static Sprite RemapObjectToSprite(Object obj)
        {
            if (obj is Sprite)
            {
                return (Sprite) obj;
            }
            if (obj is Texture2D)
            {
                Object[] objArray = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(obj));
                for (int i = 0; i < objArray.Length; i++)
                {
                    if (objArray[i].GetType() == typeof(Sprite))
                    {
                        return (objArray[i] as Sprite);
                    }
                }
            }
            return null;
        }

        public static Sprite TextureToSprite(Texture2D tex)
        {
            Sprite[] spriteArray = TextureToSprites(tex);
            if (spriteArray.Length > 0)
            {
                return spriteArray[0];
            }
            return null;
        }

        public static Sprite[] TextureToSprites(Texture2D tex)
        {
            Object[] objArray = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(tex));
            List<Sprite> list = new List<Sprite>();
            for (int i = 0; i < objArray.Length; i++)
            {
                if (objArray[i].GetType() == typeof(Sprite))
                {
                    list.Add(objArray[i] as Sprite);
                }
            }
            if (list.Count > 0)
            {
                return list.ToArray();
            }
            return new Sprite[] { GenerateDefaultSprite(tex) };
        }

        private static bool ValidPathForTextureAsset(string path)
        {
            string str = FileUtil.GetPathExtension(path).ToLower();
            return ((((((str == "jpg") || (str == "jpeg")) || ((str == "tif") || (str == "tiff"))) || (((str == "tga") || (str == "gif")) || ((str == "png") || (str == "psd")))) || ((((str == "bmp") || (str == "iff")) || ((str == "pict") || (str == "pic"))) || ((str == "pct") || (str == "exr")))) || (str == "hdr"));
        }

        private enum DragType
        {
            NotInitialized,
            SpriteAnimation,
            CreateMultiple
        }
    }
}

