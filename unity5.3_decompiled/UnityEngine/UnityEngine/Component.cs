namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Security;
    using UnityEngine.Internal;
    using UnityEngine.Scripting;
    using UnityEngineInternal;

    [RequiredByNativeCode]
    public class Component : Object
    {
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
        [SecuritySafeCritical]
        public unsafe T GetComponent<T>()
        {
            CastHelper<T> helper = new CastHelper<T>();
            this.GetComponentFastPath(typeof(T), new IntPtr((void*) &helper.onePointerFurtherThanT));
            return helper.t;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern Component GetComponent(string type);
        [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public Component GetComponent(Type type)
        {
            return this.gameObject.GetComponent(type);
        }

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

        [ExcludeFromDocs, TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public Component GetComponentInChildren(Type t)
        {
            bool includeInactive = false;
            return this.GetComponentInChildren(t, includeInactive);
        }

        public Component GetComponentInChildren(Type t, [DefaultValue("false")] bool includeInactive)
        {
            return this.gameObject.GetComponentInChildren(t, includeInactive);
        }

        public T GetComponentInParent<T>()
        {
            return (T) this.GetComponentInParent(typeof(T));
        }

        [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
        public Component GetComponentInParent(Type t)
        {
            return this.gameObject.GetComponentInParent(t);
        }

        public T[] GetComponents<T>()
        {
            return this.gameObject.GetComponents<T>();
        }

        public void GetComponents<T>(List<T> results)
        {
            this.GetComponentsForListInternal(typeof(T), results);
        }

        public Component[] GetComponents(Type type)
        {
            return this.gameObject.GetComponents(type);
        }

        public void GetComponents(Type type, List<Component> results)
        {
            this.GetComponentsForListInternal(type, results);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void GetComponentsForListInternal(Type searchType, object resultList);
        public T[] GetComponentsInChildren<T>()
        {
            return this.GetComponentsInChildren<T>(false);
        }

        public T[] GetComponentsInChildren<T>(bool includeInactive)
        {
            return this.gameObject.GetComponentsInChildren<T>(includeInactive);
        }

        public void GetComponentsInChildren<T>(List<T> results)
        {
            this.GetComponentsInChildren<T>(false, results);
        }

        [ExcludeFromDocs]
        public Component[] GetComponentsInChildren(Type t)
        {
            bool includeInactive = false;
            return this.GetComponentsInChildren(t, includeInactive);
        }

        public void GetComponentsInChildren<T>(bool includeInactive, List<T> result)
        {
            this.gameObject.GetComponentsInChildren<T>(includeInactive, result);
        }

        public Component[] GetComponentsInChildren(Type t, [DefaultValue("false")] bool includeInactive)
        {
            return this.gameObject.GetComponentsInChildren(t, includeInactive);
        }

        public T[] GetComponentsInParent<T>()
        {
            return this.GetComponentsInParent<T>(false);
        }

        public T[] GetComponentsInParent<T>(bool includeInactive)
        {
            return this.gameObject.GetComponentsInParent<T>(includeInactive);
        }

        [ExcludeFromDocs]
        public Component[] GetComponentsInParent(Type t)
        {
            bool includeInactive = false;
            return this.GetComponentsInParent(t, includeInactive);
        }

        public void GetComponentsInParent<T>(bool includeInactive, List<T> results)
        {
            this.gameObject.GetComponentsInParent<T>(includeInactive, results);
        }

        public Component[] GetComponentsInParent(Type t, [DefaultValue("false")] bool includeInactive)
        {
            return this.gameObject.GetComponentsInParent(t, includeInactive);
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

        public GameObject gameObject { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

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

        public string tag
        {
            get
            {
                return this.gameObject.tag;
            }
            set
            {
                this.gameObject.tag = value;
            }
        }

        public Transform transform { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

