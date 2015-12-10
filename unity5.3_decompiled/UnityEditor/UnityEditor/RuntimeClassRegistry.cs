namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Advertisements;
    using UnityEngine.Audio;

    internal class RuntimeClassRegistry
    {
        protected Dictionary<int, string> allNativeClasses = new Dictionary<int, string>();
        protected BuildTarget buildTarget;
        protected Dictionary<string, string> functionalityGroups = new Dictionary<string, string>();
        protected Dictionary<string, HashSet<string>> groupManagedDependencies = new Dictionary<string, HashSet<string>>();
        protected Dictionary<string, HashSet<string>> groupNativeDependencies = new Dictionary<string, HashSet<string>>();
        internal List<MethodDescription> m_MethodsToPreserve = new List<MethodDescription>();
        protected Dictionary<string, string[]> m_UsedTypesPerUserAssembly = new Dictionary<string, string[]>();
        internal List<string> m_UserAssemblies = new List<string>();
        protected HashSet<string> monoBaseClasses = new HashSet<string>();
        protected HashSet<string> monoClasses = new HashSet<string>();
        protected Dictionary<int, string> nativeClasses = new Dictionary<int, string>();
        protected Dictionary<string, string> retentionLevel = new Dictionary<string, string>();

        protected void AddFunctionalityGroup(string groupName, string managerClassName)
        {
            this.functionalityGroups.Add(groupName, managerClassName);
            this.groupManagedDependencies[groupName] = new HashSet<string>();
            this.groupNativeDependencies[groupName] = new HashSet<string>();
        }

        protected void AddManagedBaseClass(string className)
        {
            this.monoBaseClasses.Add(className);
        }

        protected void AddManagedDependenciesForFunctionalityGroup(string groupName, string depClassName)
        {
            this.AddManagedDependenciesForFunctionalityGroup(groupName, depClassName, null);
        }

        protected void AddManagedDependenciesForFunctionalityGroup(string groupName, Type depClass)
        {
            this.AddManagedDependenciesForFunctionalityGroup(groupName, this.ResolveTypeName(depClass));
        }

        protected void AddManagedDependenciesForFunctionalityGroup(string groupName, string depClassName, string retain)
        {
            this.groupManagedDependencies[groupName].Add(depClassName);
            if (retain != null)
            {
                this.SetRetentionLevel(depClassName, retain);
            }
        }

        protected void AddManagedDependenciesForFunctionalityGroup(string groupName, Type depClass, string retain)
        {
            this.AddManagedDependenciesForFunctionalityGroup(groupName, this.ResolveTypeName(depClass), retain);
        }

        internal void AddMethodToPreserve(string assembly, string @namespace, string klassName, string methodName)
        {
            MethodDescription item = new MethodDescription {
                assembly = assembly,
                fullTypeName = @namespace + ((@namespace.Length <= 0) ? string.Empty : ".") + klassName,
                methodName = methodName
            };
            this.m_MethodsToPreserve.Add(item);
        }

        public void AddMonoClass(string className)
        {
            this.monoClasses.Add(className);
        }

        public void AddMonoClasses(List<string> classes)
        {
            foreach (string str in classes)
            {
                this.AddMonoClass(str);
            }
        }

        protected void AddNativeClassFromName(string className)
        {
            int iD = BaseObjectTools.StringToClassID(className);
            if ((iD != -1) && !BaseObjectTools.IsBaseObject(iD))
            {
                this.nativeClasses[iD] = className;
            }
        }

        public void AddNativeClassID(int ID)
        {
            string str = BaseObjectTools.ClassIDToString(ID);
            if (str.Length > 0)
            {
                this.allNativeClasses[ID] = str;
                if (!this.functionalityGroups.ContainsValue(str))
                {
                    this.nativeClasses[ID] = str;
                }
            }
        }

        protected void AddNativeDependenciesForFunctionalityGroup(string groupName, string depClassName)
        {
            this.groupNativeDependencies[groupName].Add(depClassName);
        }

        internal void AddUserAssembly(string assembly)
        {
            if (!this.m_UserAssemblies.Contains(assembly))
            {
                this.m_UserAssemblies.Add(assembly);
            }
        }

        public static RuntimeClassRegistry Create()
        {
            return new RuntimeClassRegistry();
        }

        public List<string> GetAllManagedBaseClassesAsString()
        {
            return new List<string>(this.monoBaseClasses);
        }

        public List<string> GetAllManagedClassesAsString()
        {
            return new List<string>(this.monoClasses);
        }

        public List<string> GetAllNativeClassesAsString()
        {
            return new List<string>(this.nativeClasses.Values);
        }

        public List<string> GetAllNativeClassesIncludingManagersAsString()
        {
            return new List<string>(this.allNativeClasses.Values);
        }

        internal List<MethodDescription> GetMethodsToPreserve()
        {
            return this.m_MethodsToPreserve;
        }

        public string GetRetentionLevel(string className)
        {
            if (this.retentionLevel.ContainsKey(className))
            {
                return this.retentionLevel[className];
            }
            return "fields";
        }

        internal string[] GetUserAssemblies()
        {
            return this.m_UserAssemblies.ToArray();
        }

        public void Initialize(int[] nativeClassIDs, BuildTarget buildTarget)
        {
            this.buildTarget = buildTarget;
            this.InitRuntimeClassRegistry();
            foreach (int num in nativeClassIDs)
            {
                this.AddNativeClassID(num);
            }
        }

        protected void InitRuntimeClassRegistry()
        {
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(this.buildTarget);
            this.AddFunctionalityGroup("Runtime", "[no manager]");
            this.AddNativeDependenciesForFunctionalityGroup("Runtime", "GameObject");
            this.AddNativeDependenciesForFunctionalityGroup("Runtime", "Material");
            this.AddNativeDependenciesForFunctionalityGroup("Runtime", "PreloadData");
            this.AddNativeDependenciesForFunctionalityGroup("Runtime", "PlayerSettings");
            this.AddNativeDependenciesForFunctionalityGroup("Runtime", "InputManager");
            this.AddNativeDependenciesForFunctionalityGroup("Runtime", "BuildSettings");
            this.AddNativeDependenciesForFunctionalityGroup("Runtime", "GraphicsSettings");
            this.AddNativeDependenciesForFunctionalityGroup("Runtime", "QualitySettings");
            this.AddNativeDependenciesForFunctionalityGroup("Runtime", "MonoManager");
            this.AddNativeDependenciesForFunctionalityGroup("Runtime", "AudioManager");
            this.AddNativeDependenciesForFunctionalityGroup("Runtime", "ScriptMapper");
            this.AddNativeDependenciesForFunctionalityGroup("Runtime", "DelayedCallManager");
            this.AddNativeDependenciesForFunctionalityGroup("Runtime", "TimeManager");
            this.AddNativeDependenciesForFunctionalityGroup("Runtime", "Cubemap");
            this.AddNativeDependenciesForFunctionalityGroup("Runtime", "Texture3D");
            this.AddNativeDependenciesForFunctionalityGroup("Runtime", "LODGroup");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(GameObject), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Transform), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Mesh), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(SkinnedMeshRenderer), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(MeshRenderer), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(UnityException), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Resolution));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(LayerMask));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(SerializeField));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(WaitForSeconds));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(WaitForFixedUpdate));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(WaitForEndOfFrame));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AssetBundle), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AssetBundleRequest));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Event), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(HideInInspector));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(SerializePrivateVariables));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(SerializeField));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Font), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(GUIStyle));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(GUISkin), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(GUITargetAttribute), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(GUI), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(TextGenerator), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(SendMouseEvents), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(SetupCoroutine), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Coroutine));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AttributeHelperEngine), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(StackTraceUtility), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(GUIUtility), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(GUI), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Application), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Animation), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AnimationClip), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AnimationEvent));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AsyncOperation));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Resources), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(CacheIndex));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Keyframe));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(RenderTexture));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AnimationCurve), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(BoneWeight));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Particle));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(SliderState), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(GUI.ScrollViewState), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(GUIScrollGroup), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(TextEditor), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(ClassLibraryInitializer), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AssetBundleCreateRequest), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", "ImageEffectTransformsToLDR");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", "ImageEffectOpaque");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Gradient), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(GradientColorKey));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(GradientAlphaKey));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Canvas), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(RectTransform), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AssemblyIsEditorAssembly), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Camera), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(CullingGroup), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(StateMachineBehaviour), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", "Experimental.Networking.DownloadHandler", "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", "Experimental.Director.Playable", "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(SharedBetweenAnimatorsAttribute), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AnimatorStateInfo), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AnimatorTransitionInfo), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AnimatorClipInfo), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(SkeletonBone), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(HumanBone), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(UIVertex), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(UICharInfo), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(UILineInfo), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AudioClip), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AudioMixer), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AudioSettings), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", "iPhone", "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", "AndroidJNI", "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", "AndroidJNIHelper", "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", "_AndroidJNIHelper", "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", "AndroidJavaObject", "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", "AndroidJavaClass", "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", "AndroidJavaRunnableProxy", "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", "SamsungTV", "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(ISerializationCallbackReceiver), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", "UnhandledExceptionHandler", "all");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", "Display", "all");
            switch (buildTargetGroup)
            {
                case BuildTargetGroup.Android:
                    this.AddManagedDependenciesForFunctionalityGroup("Runtime", "AndroidJNI", "all");
                    this.AddManagedDependenciesForFunctionalityGroup("Runtime", "AndroidJNIHelper", "all");
                    this.AddManagedDependenciesForFunctionalityGroup("Runtime", "_AndroidJNIHelper", "all");
                    this.AddManagedDependenciesForFunctionalityGroup("Runtime", "AndroidJavaObject", "all");
                    this.AddManagedDependenciesForFunctionalityGroup("Runtime", "AndroidJavaClass", "all");
                    this.AddManagedDependenciesForFunctionalityGroup("Runtime", "AndroidJavaRunnableProxy", "all");
                    break;

                case BuildTargetGroup.SamsungTV:
                    this.AddManagedDependenciesForFunctionalityGroup("Runtime", "SamsungTV", "all");
                    break;

                case BuildTargetGroup.iPhone:
                    this.AddManagedDependenciesForFunctionalityGroup("Runtime", "iPhoneKeyboard");
                    break;
            }
            if ((buildTargetGroup == BuildTargetGroup.iPhone) || ((buildTargetGroup == BuildTargetGroup.Standalone) && (((this.buildTarget == BuildTarget.StandaloneOSXIntel) || (this.buildTarget == BuildTarget.StandaloneOSXIntel64)) || (this.buildTarget == BuildTarget.StandaloneOSXUniversal))))
            {
                this.AddManagedDependenciesForFunctionalityGroup("Runtime", "SocialPlatforms.GameCenter.GameCenterPlatform", "all");
                this.AddManagedDependenciesForFunctionalityGroup("Runtime", "SocialPlatforms.GameCenter.GcLeaderboard", "all");
            }
            if ((((buildTargetGroup == BuildTargetGroup.iPhone) || (buildTargetGroup == BuildTargetGroup.Android)) || ((buildTargetGroup == BuildTargetGroup.BlackBerry) || (buildTargetGroup == BuildTargetGroup.WP8))) || ((buildTargetGroup == BuildTargetGroup.Metro) || (buildTargetGroup == BuildTargetGroup.Tizen)))
            {
                this.AddManagedDependenciesForFunctionalityGroup("Runtime", "TouchScreenKeyboard");
            }
            this.AddNativeDependenciesForFunctionalityGroup("Runtime", "NetworkManager");
            this.AddNativeDependenciesForFunctionalityGroup("Runtime", "NetworkView");
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Network));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(NetworkMessageInfo));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(RPC));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(HostData));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(BitStream));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(NetworkPlayer));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(NetworkViewID));
            this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Ping), "all");
            this.AddFunctionalityGroup("Physics", "PhysicsManager");
            this.AddNativeDependenciesForFunctionalityGroup("Physics", "PhysicsManager");
            this.AddNativeDependenciesForFunctionalityGroup("Physics", "Rigidbody");
            this.AddNativeDependenciesForFunctionalityGroup("Physics", "Collider");
            this.AddManagedDependenciesForFunctionalityGroup("Physics", typeof(ControllerColliderHit));
            this.AddManagedDependenciesForFunctionalityGroup("Physics", typeof(RaycastHit));
            this.AddManagedDependenciesForFunctionalityGroup("Physics", typeof(Collision));
            this.AddManagedDependenciesForFunctionalityGroup("Physics", typeof(MeshCollider));
            this.AddFunctionalityGroup("Physics2D", "Physics2DSettings");
            this.AddNativeDependenciesForFunctionalityGroup("Physics2D", "Physics2DSettings");
            this.AddNativeDependenciesForFunctionalityGroup("Physics2D", "Rigidbody2D");
            this.AddNativeDependenciesForFunctionalityGroup("Physics2D", "Collider2D");
            this.AddNativeDependenciesForFunctionalityGroup("Physics2D", "Joint2D");
            this.AddNativeDependenciesForFunctionalityGroup("Physics2D", "PhysicsMaterial2D");
            this.AddManagedDependenciesForFunctionalityGroup("Physics2D", typeof(RaycastHit2D));
            this.AddManagedDependenciesForFunctionalityGroup("Physics2D", typeof(Collision2D));
            this.AddManagedDependenciesForFunctionalityGroup("Physics2D", typeof(JointMotor2D));
            this.AddManagedDependenciesForFunctionalityGroup("Physics2D", typeof(JointAngleLimits2D));
            this.AddManagedDependenciesForFunctionalityGroup("Physics2D", typeof(JointTranslationLimits2D));
            this.AddManagedDependenciesForFunctionalityGroup("Physics2D", typeof(JointSuspension2D));
            this.AddFunctionalityGroup("UnityAds", "UnityAdsSettings");
            this.AddNativeDependenciesForFunctionalityGroup("UnityAds", "UnityAdsSettings");
            this.AddManagedDependenciesForFunctionalityGroup("UnityAds", typeof(UnityAdsManager), "all");
            this.AddFunctionalityGroup("Terrain", "Terrain");
            this.AddManagedDependenciesForFunctionalityGroup("Terrain", typeof(Terrain), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Terrain", typeof(TerrainData), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Terrain", typeof(TerrainCollider), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Terrain", typeof(DetailPrototype), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Terrain", typeof(TreePrototype), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Terrain", typeof(TreeInstance), "all");
            this.AddManagedDependenciesForFunctionalityGroup("Terrain", typeof(SplatPrototype), "all");
            this.AddFunctionalityGroup("Shuriken", "ParticleSystem");
            this.AddManagedDependenciesForFunctionalityGroup("Shuriken", typeof(ParticleSystem));
            this.AddManagedDependenciesForFunctionalityGroup("Shuriken", typeof(ParticleSystemRenderer));
            this.AddManagedBaseClass("UnityEngine.MonoBehaviour");
            this.AddManagedBaseClass("UnityEngine.ScriptableObject");
            if (buildTargetGroup == BuildTargetGroup.Android)
            {
                this.AddManagedBaseClass("UnityEngine.AndroidJavaProxy");
            }
            foreach (string str in RuntimeInitializeOnLoadManager.dontStripClassNames)
            {
                this.AddManagedBaseClass(str);
            }
        }

        protected void InjectFunctionalityGroupDependencies()
        {
            HashSet<string> set = new HashSet<string>();
            foreach (string str in this.functionalityGroups.Keys)
            {
                foreach (string str2 in this.monoClasses)
                {
                    if (this.groupManagedDependencies[str].Contains(str2) || this.groupNativeDependencies[str].Contains(str2))
                    {
                        set.Add(str);
                    }
                }
            }
            foreach (string str3 in set)
            {
                foreach (string str4 in this.groupManagedDependencies[str3])
                {
                    this.AddMonoClass(str4);
                }
                foreach (string str5 in this.groupNativeDependencies[str3])
                {
                    this.AddNativeClassFromName(str5);
                }
            }
        }

        public bool IsDLLUsed(string dll)
        {
            return ((this.m_UsedTypesPerUserAssembly == null) || ((Array.IndexOf<string>(CodeStrippingUtils.UserAssemblies, dll) != -1) || this.m_UsedTypesPerUserAssembly.ContainsKey(dll)));
        }

        protected string ResolveTypeName(Type type)
        {
            string fullName = type.FullName;
            return fullName.Substring(fullName.LastIndexOf(".") + 1).Replace("+", "/");
        }

        protected void SetRetentionLevel(string className, string level)
        {
            this.retentionLevel[className] = level;
        }

        public void SetUsedTypesInUserAssembly(string[] typeNames, string assemblyName)
        {
            this.m_UsedTypesPerUserAssembly[assemblyName] = typeNames;
        }

        public void SynchronizeClasses()
        {
            this.SynchronizeMonoToNativeClasses();
            this.SynchronizeNativeToMonoClasses();
            this.InjectFunctionalityGroupDependencies();
            this.SynchronizeMonoToNativeClasses();
        }

        protected void SynchronizeMonoToNativeClasses()
        {
            foreach (string str in this.monoClasses)
            {
                this.AddNativeClassFromName(str);
            }
        }

        protected void SynchronizeNativeToMonoClasses()
        {
            foreach (string str in this.nativeClasses.Values)
            {
                this.AddMonoClass(str);
            }
        }

        public Dictionary<string, string[]> UsedTypePerUserAssembly
        {
            get
            {
                return this.m_UsedTypesPerUserAssembly;
            }
        }

        internal class MethodDescription
        {
            public string assembly;
            public string fullTypeName;
            public string methodName;
        }
    }
}

