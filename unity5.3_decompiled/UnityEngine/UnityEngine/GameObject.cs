namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using UnityEngine.Internal;
    using UnityEngine.SceneManagement;
    using UnityEngineInternal;

    public sealed class GameObject : Object
    {
        public GameObject()
        {
            Internal_CreateGameObject(this, null);
        }

        public GameObject(string name)
        {
            Internal_CreateGameObject(this, name);
        }

        public GameObject(string name, params Type[] components)
        {
            Internal_CreateGameObject(this, name);
            foreach (Type type in components)
            {
                this.AddComponent(type);
            }
        }

        public T AddComponent<T>() where T: Component
        {
            return (this.AddComponent(typeof(T)) as T);
        }

        [Obsolete("GameObject.AddComponent with string argument has been deprecated. Use GameObject.AddComponent<T>() instead. (UnityUpgradable).", true)]
        public Component AddComponent(string className)
        {
            throw new NotSupportedException("AddComponent(string) is deprecated");
        }

        [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public Component AddComponent(Type componentType)
        {
            return this.Internal_AddComponentWithType(componentType);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern Component AddComponentInternal(string className);
        [ExcludeFromDocs]
        public void BroadcastMessage(string methodName)
        {
            SendMessageOptions requireReceiver = SendMessageOptions.RequireReceiver;
            object parameter = null;
            this.BroadcastMessage(methodName, parameter, requireReceiver);
        }

        [ExcludeFromDocs]
        public void BroadcastMessage(string methodName, object parameter)
        {
            SendMessageOptions requireReceiver = SendMessageOptions.RequireReceiver;
            this.BroadcastMessage(methodName, parameter, requireReceiver);
        }

        public void BroadcastMessage(string methodName, SendMessageOptions options)
        {
            this.BroadcastMessage(methodName, null, options);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void BroadcastMessage(string methodName, [DefaultValue("null")] object parameter, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool CompareTag(string tag);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern GameObject CreatePrimitive(PrimitiveType type);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern GameObject Find(string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern GameObject[] FindGameObjectsWithTag(string tag);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern GameObject FindGameObjectWithTag(string tag);
        public static GameObject FindWithTag(string tag)
        {
            return FindGameObjectWithTag(tag);
        }

        [SecuritySafeCritical]
        public unsafe T GetComponent<T>()
        {
            CastHelper<T> helper = new CastHelper<T>();
            this.GetComponentFastPath(typeof(T), new IntPtr((void*) &helper.onePointerFurtherThanT));
            return helper.t;
        }

        public Component GetComponent(string type)
        {
            return this.GetComponentByName(type);
        }

        [MethodImpl(MethodImplOptions.InternalCall), TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument), WrapperlessIcall]
        public extern Component GetComponent(Type type);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern Component GetComponentByName(string type);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void GetComponentFastPath(Type type, IntPtr oneFurtherThanResultValue);
        [ExcludeFromDocs]
        public T GetComponentInChildren<T>()
        {
            bool includeInactive = false;
            return this.GetComponentInChildren<T>(includeInactive);
        }

        public T GetComponentInChildren<T>([DefaultValue("false")] bool includeInactive)
        {
            return (T) this.GetComponentInChildren(typeof(T), includeInactive);
        }

        [ExcludeFromDocs]
        public Component GetComponentInChildren(Type type)
        {
            bool includeInactive = false;
            return this.GetComponentInChildren(type, includeInactive);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public extern Component GetComponentInChildren(Type type, [DefaultValue("false")] bool includeInactive);
        public T GetComponentInParent<T>()
        {
            return (T) this.GetComponentInParent(typeof(T));
        }

        [MethodImpl(MethodImplOptions.InternalCall), TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument), WrapperlessIcall]
        public extern Component GetComponentInParent(Type type);
        public T[] GetComponents<T>()
        {
            return (T[]) this.GetComponentsInternal(typeof(T), true, false, true, false, null);
        }

        public void GetComponents<T>(List<T> results)
        {
            this.GetComponentsInternal(typeof(T), false, false, true, false, results);
        }

        public Component[] GetComponents(Type type)
        {
            return (Component[]) this.GetComponentsInternal(type, false, false, true, false, null);
        }

        public void GetComponents(Type type, List<Component> results)
        {
            this.GetComponentsInternal(type, false, false, true, false, results);
        }

        public T[] GetComponentsInChildren<T>()
        {
            return this.GetComponentsInChildren<T>(false);
        }

        public T[] GetComponentsInChildren<T>(bool includeInactive)
        {
            return (T[]) this.GetComponentsInternal(typeof(T), true, true, includeInactive, false, null);
        }

        public void GetComponentsInChildren<T>(List<T> results)
        {
            this.GetComponentsInChildren<T>(false, results);
        }

        [ExcludeFromDocs]
        public Component[] GetComponentsInChildren(Type type)
        {
            bool includeInactive = false;
            return this.GetComponentsInChildren(type, includeInactive);
        }

        public void GetComponentsInChildren<T>(bool includeInactive, List<T> results)
        {
            this.GetComponentsInternal(typeof(T), true, true, includeInactive, false, results);
        }

        public Component[] GetComponentsInChildren(Type type, [DefaultValue("false")] bool includeInactive)
        {
            return (Component[]) this.GetComponentsInternal(type, false, true, includeInactive, false, null);
        }

        public T[] GetComponentsInParent<T>()
        {
            return this.GetComponentsInParent<T>(false);
        }

        public T[] GetComponentsInParent<T>(bool includeInactive)
        {
            return (T[]) this.GetComponentsInternal(typeof(T), true, true, includeInactive, true, null);
        }

        [ExcludeFromDocs]
        public Component[] GetComponentsInParent(Type type)
        {
            bool includeInactive = false;
            return this.GetComponentsInParent(type, includeInactive);
        }

        public void GetComponentsInParent<T>(bool includeInactive, List<T> results)
        {
            this.GetComponentsInternal(typeof(T), true, true, includeInactive, true, results);
        }

        public Component[] GetComponentsInParent(Type type, [DefaultValue("false")] bool includeInactive)
        {
            return (Component[]) this.GetComponentsInternal(type, false, true, includeInactive, true, null);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern Array GetComponentsInternal(Type type, bool useSearchTypeAsArrayReturnType, bool recursive, bool includeInactive, bool reverse, object resultList);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern Component Internal_AddComponentWithType(Type componentType);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_CreateGameObject([Writable] GameObject mono, string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_scene(out Scene value);
        [Obsolete("gameObject.PlayAnimation is not supported anymore. Use animation.Play()", true)]
        public void PlayAnimation(Object animation)
        {
            throw new NotSupportedException("gameObject.PlayAnimation is not supported anymore. Use animation.Play();");
        }

        [Obsolete("GameObject.SampleAnimation(AnimationClip, float) has been deprecated. Use AnimationClip.SampleAnimation(GameObject, float) instead (UnityUpgradable).", true)]
        public void SampleAnimation(Object clip, float time)
        {
            throw new NotSupportedException("GameObject.SampleAnimation is deprecated");
        }

        [ExcludeFromDocs]
        public void SendMessage(string methodName)
        {
            SendMessageOptions requireReceiver = SendMessageOptions.RequireReceiver;
            object obj2 = null;
            this.SendMessage(methodName, obj2, requireReceiver);
        }

        [ExcludeFromDocs]
        public void SendMessage(string methodName, object value)
        {
            SendMessageOptions requireReceiver = SendMessageOptions.RequireReceiver;
            this.SendMessage(methodName, value, requireReceiver);
        }

        public void SendMessage(string methodName, SendMessageOptions options)
        {
            this.SendMessage(methodName, null, options);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SendMessage(string methodName, [DefaultValue("null")] object value, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);
        [ExcludeFromDocs]
        public void SendMessageUpwards(string methodName)
        {
            SendMessageOptions requireReceiver = SendMessageOptions.RequireReceiver;
            object obj2 = null;
            this.SendMessageUpwards(methodName, obj2, requireReceiver);
        }

        [ExcludeFromDocs]
        public void SendMessageUpwards(string methodName, object value)
        {
            SendMessageOptions requireReceiver = SendMessageOptions.RequireReceiver;
            this.SendMessageUpwards(methodName, value, requireReceiver);
        }

        public void SendMessageUpwards(string methodName, SendMessageOptions options)
        {
            this.SendMessageUpwards(methodName, null, options);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SendMessageUpwards(string methodName, [DefaultValue("null")] object value, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetActive(bool value);
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("gameObject.SetActiveRecursively() is obsolete. Use GameObject.SetActive(), which is now inherited by children."), WrapperlessIcall]
        public extern void SetActiveRecursively(bool state);
        [Obsolete("gameObject.StopAnimation is not supported anymore. Use animation.Stop()", true)]
        public void StopAnimation()
        {
            throw new NotSupportedException("gameObject.StopAnimation(); is not supported anymore. Use animation.Stop();");
        }

        [Obsolete("GameObject.active is obsolete. Use GameObject.SetActive(), GameObject.activeSelf or GameObject.activeInHierarchy.")]
        public bool active { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool activeInHierarchy { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool activeSelf { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("Property animation has been deprecated. Use GetComponent<Animation>() instead. (UnityUpgradable)", true)]
        public Component animation
        {
            get
            {
                throw new NotSupportedException("animation property has been deprecated");
            }
        }

        [Obsolete("Property audio has been deprecated. Use GetComponent<AudioSource>() instead. (UnityUpgradable)", true)]
        public Component audio
        {
            get
            {
                throw new NotSupportedException("audio property has been deprecated");
            }
        }

        [Obsolete("Property camera has been deprecated. Use GetComponent<Camera>() instead. (UnityUpgradable)", true)]
        public Component camera
        {
            get
            {
                throw new NotSupportedException("camera property has been deprecated");
            }
        }

        [Obsolete("Property collider has been deprecated. Use GetComponent<Collider>() instead. (UnityUpgradable)", true)]
        public Component collider
        {
            get
            {
                throw new NotSupportedException("collider property has been deprecated");
            }
        }

        [Obsolete("Property collider2D has been deprecated. Use GetComponent<Collider2D>() instead. (UnityUpgradable)", true)]
        public Component collider2D
        {
            get
            {
                throw new NotSupportedException("collider2D property has been deprecated");
            }
        }

        [Obsolete("Property constantForce has been deprecated. Use GetComponent<ConstantForce>() instead. (UnityUpgradable)", true)]
        public Component constantForce
        {
            get
            {
                throw new NotSupportedException("constantForce property has been deprecated");
            }
        }

        public GameObject gameObject
        {
            get
            {
                return this;
            }
        }

        [Obsolete("Property guiElement has been deprecated. Use GetComponent<GUIElement>() instead. (UnityUpgradable)", true)]
        public Component guiElement
        {
            get
            {
                throw new NotSupportedException("guiElement property has been deprecated");
            }
        }

        [Obsolete("Property guiText has been deprecated. Use GetComponent<GUIText>() instead. (UnityUpgradable)", true)]
        public Component guiText
        {
            get
            {
                throw new NotSupportedException("guiText property has been deprecated");
            }
        }

        [Obsolete("Property guiTexture has been deprecated. Use GetComponent<GUITexture>() instead. (UnityUpgradable)", true)]
        public Component guiTexture
        {
            get
            {
                throw new NotSupportedException("guiTexture property has been deprecated");
            }
        }

        [Obsolete("Property hingeJoint has been deprecated. Use GetComponent<HingeJoint>() instead. (UnityUpgradable)", true)]
        public Component hingeJoint
        {
            get
            {
                throw new NotSupportedException("hingeJoint property has been deprecated");
            }
        }

        public bool isStatic { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal bool isStaticBatchable { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int layer { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("Property light has been deprecated. Use GetComponent<Light>() instead. (UnityUpgradable)", true)]
        public Component light
        {
            get
            {
                throw new NotSupportedException("light property has been deprecated");
            }
        }

        [Obsolete("Property networkView has been deprecated. Use GetComponent<NetworkView>() instead. (UnityUpgradable)", true)]
        public Component networkView
        {
            get
            {
                throw new NotSupportedException("networkView property has been deprecated");
            }
        }

        [Obsolete("Property particleEmitter has been deprecated. Use GetComponent<ParticleEmitter>() instead. (UnityUpgradable)", true)]
        public Component particleEmitter
        {
            get
            {
                throw new NotSupportedException("particleEmitter property has been deprecated");
            }
        }

        [Obsolete("Property particleSystem has been deprecated. Use GetComponent<ParticleSystem>() instead. (UnityUpgradable)", true)]
        public Component particleSystem
        {
            get
            {
                throw new NotSupportedException("particleSystem property has been deprecated");
            }
        }

        [Obsolete("Property renderer has been deprecated. Use GetComponent<Renderer>() instead. (UnityUpgradable)", true)]
        public Component renderer
        {
            get
            {
                throw new NotSupportedException("renderer property has been deprecated");
            }
        }

        [Obsolete("Property rigidbody has been deprecated. Use GetComponent<Rigidbody>() instead. (UnityUpgradable)", true)]
        public Component rigidbody
        {
            get
            {
                throw new NotSupportedException("rigidbody property has been deprecated");
            }
        }

        [Obsolete("Property rigidbody2D has been deprecated. Use GetComponent<Rigidbody2D>() instead. (UnityUpgradable)", true)]
        public Component rigidbody2D
        {
            get
            {
                throw new NotSupportedException("rigidbody2D property has been deprecated");
            }
        }

        public Scene scene
        {
            get
            {
                Scene scene;
                this.INTERNAL_get_scene(out scene);
                return scene;
            }
        }

        public string tag { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Transform transform { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

