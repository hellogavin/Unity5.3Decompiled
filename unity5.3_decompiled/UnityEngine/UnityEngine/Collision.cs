namespace UnityEngine
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), RequiredByNativeCode]
    public class Collision
    {
        internal Vector3 m_Impulse;
        internal Vector3 m_RelativeVelocity;
        internal Rigidbody m_Rigidbody;
        internal Collider m_Collider;
        internal ContactPoint[] m_Contacts;
        public Vector3 relativeVelocity
        {
            get
            {
                return this.m_RelativeVelocity;
            }
        }
        public Rigidbody rigidbody
        {
            get
            {
                return this.m_Rigidbody;
            }
        }
        public Collider collider
        {
            get
            {
                return this.m_Collider;
            }
        }
        public Transform transform
        {
            get
            {
                return ((this.rigidbody == null) ? this.collider.transform : this.rigidbody.transform);
            }
        }
        public GameObject gameObject
        {
            get
            {
                return ((this.m_Rigidbody == null) ? this.m_Collider.gameObject : this.m_Rigidbody.gameObject);
            }
        }
        public ContactPoint[] contacts
        {
            get
            {
                return this.m_Contacts;
            }
        }
        public virtual IEnumerator GetEnumerator()
        {
            return this.contacts.GetEnumerator();
        }

        public Vector3 impulse
        {
            get
            {
                return this.m_Impulse;
            }
        }
        [Obsolete("Use Collision.relativeVelocity instead.", false)]
        public Vector3 impactForceSum
        {
            get
            {
                return this.relativeVelocity;
            }
        }
        [Obsolete("Will always return zero.", false)]
        public Vector3 frictionForceSum
        {
            get
            {
                return Vector3.zero;
            }
        }
        [Obsolete("Please use Collision.rigidbody, Collision.transform or Collision.collider instead", false)]
        public Component other
        {
            get
            {
                return ((this.m_Rigidbody == null) ? ((Component) this.m_Collider) : ((Component) this.m_Rigidbody));
            }
        }
    }
}

