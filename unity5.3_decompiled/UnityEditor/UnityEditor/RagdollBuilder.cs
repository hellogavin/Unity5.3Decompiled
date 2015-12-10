namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class RagdollBuilder : ScriptableWizard
    {
        private ArrayList bones;
        public bool flipForward;
        private Vector3 forward = Vector3.forward;
        public Transform head;
        public Transform leftArm;
        public Transform leftElbow;
        public Transform leftFoot;
        public Transform leftHips;
        public Transform leftKnee;
        public Transform middleSpine;
        public Transform pelvis;
        private Vector3 right = Vector3.right;
        public Transform rightArm;
        public Transform rightElbow;
        public Transform rightFoot;
        public Transform rightHips;
        public Transform rightKnee;
        private BoneInfo rootBone;
        public float strength;
        public float totalMass = 20f;
        private Vector3 up = Vector3.up;
        private Vector3 worldForward = Vector3.forward;
        private Vector3 worldRight = Vector3.right;
        private Vector3 worldUp = Vector3.up;

        private void AddBreastColliders()
        {
            if ((this.middleSpine != null) && (this.pelvis != null))
            {
                Bounds bounds = this.Clip(this.GetBreastBounds(this.pelvis), this.pelvis, this.middleSpine, false);
                BoxCollider collider = this.pelvis.gameObject.AddComponent<BoxCollider>();
                collider.center = bounds.center;
                collider.size = bounds.size;
                bounds = this.Clip(this.GetBreastBounds(this.middleSpine), this.middleSpine, this.middleSpine, true);
                collider = this.middleSpine.gameObject.AddComponent<BoxCollider>();
                collider.center = bounds.center;
                collider.size = bounds.size;
            }
            else
            {
                Bounds bounds2 = new Bounds();
                bounds2.Encapsulate(this.pelvis.InverseTransformPoint(this.leftHips.position));
                bounds2.Encapsulate(this.pelvis.InverseTransformPoint(this.rightHips.position));
                bounds2.Encapsulate(this.pelvis.InverseTransformPoint(this.leftArm.position));
                bounds2.Encapsulate(this.pelvis.InverseTransformPoint(this.rightArm.position));
                Vector3 size = bounds2.size;
                size[SmallestComponent(bounds2.size)] = size[LargestComponent(bounds2.size)] / 2f;
                BoxCollider collider2 = this.pelvis.gameObject.AddComponent<BoxCollider>();
                collider2.center = bounds2.center;
                collider2.size = size;
            }
        }

        private void AddHeadCollider()
        {
            int num2;
            float num3;
            if (this.head.GetComponent<Collider>() != null)
            {
                Object.Destroy(this.head.GetComponent<Collider>());
            }
            float num = Vector3.Distance(this.leftArm.transform.position, this.rightArm.transform.position) / 4f;
            SphereCollider collider = this.head.gameObject.AddComponent<SphereCollider>();
            collider.radius = num;
            Vector3 zero = Vector3.zero;
            CalculateDirection(this.head.InverseTransformPoint(this.pelvis.position), out num2, out num3);
            if (num3 > 0f)
            {
                zero[num2] = -num;
            }
            else
            {
                zero[num2] = num;
            }
            collider.center = zero;
        }

        private void AddJoint(string name, Transform anchor, string parent, Vector3 worldTwistAxis, Vector3 worldSwingAxis, float minLimit, float maxLimit, float swingLimit, Type colliderType, float radiusScale, float density)
        {
            BoneInfo info = new BoneInfo {
                name = name,
                anchor = anchor,
                axis = worldTwistAxis,
                normalAxis = worldSwingAxis,
                minLimit = minLimit,
                maxLimit = maxLimit,
                swingLimit = swingLimit,
                density = density,
                colliderType = colliderType,
                radiusScale = radiusScale
            };
            if (this.FindBone(parent) != null)
            {
                info.parent = this.FindBone(parent);
            }
            else if (name.StartsWith("Left"))
            {
                info.parent = this.FindBone("Left " + parent);
            }
            else if (name.StartsWith("Right"))
            {
                info.parent = this.FindBone("Right " + parent);
            }
            info.parent.children.Add(info);
            this.bones.Add(info);
        }

        private void AddMirroredJoint(string name, Transform leftAnchor, Transform rightAnchor, string parent, Vector3 worldTwistAxis, Vector3 worldSwingAxis, float minLimit, float maxLimit, float swingLimit, Type colliderType, float radiusScale, float density)
        {
            this.AddJoint("Left " + name, leftAnchor, parent, worldTwistAxis, worldSwingAxis, minLimit, maxLimit, swingLimit, colliderType, radiusScale, density);
            this.AddJoint("Right " + name, rightAnchor, parent, worldTwistAxis, worldSwingAxis, minLimit, maxLimit, swingLimit, colliderType, radiusScale, density);
        }

        private void BuildBodies()
        {
            IEnumerator enumerator = this.bones.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    BoneInfo current = (BoneInfo) enumerator.Current;
                    current.anchor.gameObject.AddComponent<Rigidbody>();
                    current.anchor.GetComponent<Rigidbody>().mass = current.density;
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        }

        private void BuildCapsules()
        {
            IEnumerator enumerator = this.bones.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    BoneInfo current = (BoneInfo) enumerator.Current;
                    if (current.colliderType == typeof(CapsuleCollider))
                    {
                        int num;
                        float num2;
                        if (current.children.Count == 1)
                        {
                            BoneInfo info2 = (BoneInfo) current.children[0];
                            Vector3 position = info2.anchor.position;
                            CalculateDirection(current.anchor.InverseTransformPoint(position), out num, out num2);
                        }
                        else
                        {
                            Vector3 vector2 = (current.anchor.position - current.parent.anchor.position) + current.anchor.position;
                            CalculateDirection(current.anchor.InverseTransformPoint(vector2), out num, out num2);
                            if (current.anchor.GetComponentsInChildren(typeof(Transform)).Length > 1)
                            {
                                Bounds bounds = new Bounds();
                                foreach (Transform transform in current.anchor.GetComponentsInChildren(typeof(Transform)))
                                {
                                    bounds.Encapsulate(current.anchor.InverseTransformPoint(transform.position));
                                }
                                if (num2 > 0f)
                                {
                                    num2 = bounds.max[num];
                                }
                                else
                                {
                                    num2 = bounds.min[num];
                                }
                            }
                        }
                        CapsuleCollider collider = current.anchor.gameObject.AddComponent<CapsuleCollider>();
                        collider.direction = num;
                        Vector3 zero = Vector3.zero;
                        zero[num] = num2 * 0.5f;
                        collider.center = zero;
                        collider.height = Mathf.Abs(num2);
                        collider.radius = Mathf.Abs((float) (num2 * current.radiusScale));
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        }

        private void BuildJoints()
        {
            IEnumerator enumerator = this.bones.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    BoneInfo current = (BoneInfo) enumerator.Current;
                    if (current.parent != null)
                    {
                        CharacterJoint joint = current.anchor.gameObject.AddComponent<CharacterJoint>();
                        current.joint = joint;
                        joint.axis = CalculateDirectionAxis(current.anchor.InverseTransformDirection(current.axis));
                        joint.swingAxis = CalculateDirectionAxis(current.anchor.InverseTransformDirection(current.normalAxis));
                        joint.anchor = Vector3.zero;
                        joint.connectedBody = current.parent.anchor.GetComponent<Rigidbody>();
                        joint.enablePreprocessing = false;
                        SoftJointLimit limit = new SoftJointLimit {
                            contactDistance = 0f,
                            limit = current.minLimit
                        };
                        joint.lowTwistLimit = limit;
                        limit.limit = current.maxLimit;
                        joint.highTwistLimit = limit;
                        limit.limit = current.swingLimit;
                        joint.swing1Limit = limit;
                        limit.limit = 0f;
                        joint.swing2Limit = limit;
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        }

        private void CalculateAxes()
        {
            if ((this.head != null) && (this.pelvis != null))
            {
                this.up = CalculateDirectionAxis(this.pelvis.InverseTransformPoint(this.head.position));
            }
            if ((this.rightElbow != null) && (this.pelvis != null))
            {
                Vector3 vector;
                Vector3 vector2;
                this.DecomposeVector(out vector2, out vector, this.pelvis.InverseTransformPoint(this.rightElbow.position), this.up);
                this.right = CalculateDirectionAxis(vector);
            }
            this.forward = Vector3.Cross(this.right, this.up);
            if (this.flipForward)
            {
                this.forward = -this.forward;
            }
        }

        private static void CalculateDirection(Vector3 point, out int direction, out float distance)
        {
            direction = 0;
            if (Mathf.Abs(point[1]) > Mathf.Abs(point[0]))
            {
                direction = 1;
            }
            if (Mathf.Abs(point[2]) > Mathf.Abs(point[direction]))
            {
                direction = 2;
            }
            distance = point[direction];
        }

        private static Vector3 CalculateDirectionAxis(Vector3 point)
        {
            float num2;
            int direction = 0;
            CalculateDirection(point, out direction, out num2);
            Vector3 zero = Vector3.zero;
            if (num2 > 0f)
            {
                zero[direction] = 1f;
                return zero;
            }
            zero[direction] = -1f;
            return zero;
        }

        private void CalculateMass()
        {
            this.CalculateMassRecurse(this.rootBone);
            float num = this.totalMass / this.rootBone.summedMass;
            IEnumerator enumerator = this.bones.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    BoneInfo current = (BoneInfo) enumerator.Current;
                    Rigidbody component = current.anchor.GetComponent<Rigidbody>();
                    component.mass *= num;
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            this.CalculateMassRecurse(this.rootBone);
        }

        private void CalculateMassRecurse(BoneInfo bone)
        {
            float mass = bone.anchor.GetComponent<Rigidbody>().mass;
            IEnumerator enumerator = bone.children.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    BoneInfo current = (BoneInfo) enumerator.Current;
                    this.CalculateMassRecurse(current);
                    mass += current.summedMass;
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            bone.summedMass = mass;
        }

        private string CheckConsistency()
        {
            this.PrepareBones();
            Hashtable hashtable = new Hashtable();
            IEnumerator enumerator = this.bones.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    BoneInfo current = (BoneInfo) enumerator.Current;
                    if (current.anchor != null)
                    {
                        if (hashtable[current.anchor] != null)
                        {
                            BoneInfo info2 = (BoneInfo) hashtable[current.anchor];
                            return string.Format("{0} and {1} may not be assigned to the same bone.", current.name, info2.name);
                        }
                        hashtable[current.anchor] = current;
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            IEnumerator enumerator2 = this.bones.GetEnumerator();
            try
            {
                while (enumerator2.MoveNext())
                {
                    BoneInfo info3 = (BoneInfo) enumerator2.Current;
                    if (info3.anchor == null)
                    {
                        return string.Format("{0} has not been assigned yet.\n", info3.name);
                    }
                }
            }
            finally
            {
                IDisposable disposable2 = enumerator2 as IDisposable;
                if (disposable2 == null)
                {
                }
                disposable2.Dispose();
            }
            return string.Empty;
        }

        private void Cleanup()
        {
            IEnumerator enumerator = this.bones.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    BoneInfo current = (BoneInfo) enumerator.Current;
                    if (current.anchor != null)
                    {
                        foreach (Joint joint in current.anchor.GetComponentsInChildren(typeof(Joint)))
                        {
                            Object.DestroyImmediate(joint);
                        }
                        foreach (Rigidbody rigidbody in current.anchor.GetComponentsInChildren(typeof(Rigidbody)))
                        {
                            Object.DestroyImmediate(rigidbody);
                        }
                        foreach (Collider collider in current.anchor.GetComponentsInChildren(typeof(Collider)))
                        {
                            Object.DestroyImmediate(collider);
                        }
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        }

        private Bounds Clip(Bounds bounds, Transform relativeTo, Transform clipTransform, bool below)
        {
            int num = LargestComponent(bounds.size);
            if ((Vector3.Dot(this.worldUp, relativeTo.TransformPoint(bounds.max)) > Vector3.Dot(this.worldUp, relativeTo.TransformPoint(bounds.min))) == below)
            {
                Vector3 min = bounds.min;
                min[num] = relativeTo.InverseTransformPoint(clipTransform.position)[num];
                bounds.min = min;
                return bounds;
            }
            Vector3 max = bounds.max;
            max[num] = relativeTo.InverseTransformPoint(clipTransform.position)[num];
            bounds.max = max;
            return bounds;
        }

        [MenuItem("GameObject/3D Object/Ragdoll...", false, 0x7d0)]
        private static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard<RagdollBuilder>("Create Ragdoll");
        }

        private void DecomposeVector(out Vector3 normalCompo, out Vector3 tangentCompo, Vector3 outwardDir, Vector3 outwardNormal)
        {
            outwardNormal = outwardNormal.normalized;
            normalCompo = (Vector3) (outwardNormal * Vector3.Dot(outwardDir, outwardNormal));
            tangentCompo = outwardDir - normalCompo;
        }

        private BoneInfo FindBone(string name)
        {
            IEnumerator enumerator = this.bones.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    BoneInfo current = (BoneInfo) enumerator.Current;
                    if (current.name == name)
                    {
                        return current;
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            return null;
        }

        private Bounds GetBreastBounds(Transform relativeTo)
        {
            Bounds bounds = new Bounds();
            bounds.Encapsulate(relativeTo.InverseTransformPoint(this.leftHips.position));
            bounds.Encapsulate(relativeTo.InverseTransformPoint(this.rightHips.position));
            bounds.Encapsulate(relativeTo.InverseTransformPoint(this.leftArm.position));
            bounds.Encapsulate(relativeTo.InverseTransformPoint(this.rightArm.position));
            Vector3 size = bounds.size;
            size[SmallestComponent(bounds.size)] = size[LargestComponent(bounds.size)] / 2f;
            bounds.size = size;
            return bounds;
        }

        private static int LargestComponent(Vector3 point)
        {
            int num = 0;
            if (Mathf.Abs(point[1]) > Mathf.Abs(point[0]))
            {
                num = 1;
            }
            if (Mathf.Abs(point[2]) > Mathf.Abs(point[num]))
            {
                num = 2;
            }
            return num;
        }

        private void OnDrawGizmos()
        {
            if (this.pelvis != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(this.pelvis.position, this.pelvis.TransformDirection(this.right));
                Gizmos.color = Color.green;
                Gizmos.DrawRay(this.pelvis.position, this.pelvis.TransformDirection(this.up));
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(this.pelvis.position, this.pelvis.TransformDirection(this.forward));
            }
        }

        private void OnWizardCreate()
        {
            this.Cleanup();
            this.BuildCapsules();
            this.AddBreastColliders();
            this.AddHeadCollider();
            this.BuildBodies();
            this.BuildJoints();
            this.CalculateMass();
        }

        private void OnWizardUpdate()
        {
            base.errorString = this.CheckConsistency();
            this.CalculateAxes();
            if (base.errorString.Length != 0)
            {
                base.helpString = "Drag all bones from the hierarchy into their slots.\nMake sure your character is in T-Stand.\n";
            }
            else
            {
                base.helpString = "Make sure your character is in T-Stand.\nMake sure the blue axis faces in the same direction the chracter is looking.\nUse flipForward to flip the direction";
            }
            base.isValid = base.errorString.Length == 0;
        }

        private void PrepareBones()
        {
            if (this.pelvis != null)
            {
                this.worldRight = this.pelvis.TransformDirection(this.right);
                this.worldUp = this.pelvis.TransformDirection(this.up);
                this.worldForward = this.pelvis.TransformDirection(this.forward);
            }
            this.bones = new ArrayList();
            this.rootBone = new BoneInfo();
            this.rootBone.name = "Pelvis";
            this.rootBone.anchor = this.pelvis;
            this.rootBone.parent = null;
            this.rootBone.density = 2.5f;
            this.bones.Add(this.rootBone);
            this.AddMirroredJoint("Hips", this.leftHips, this.rightHips, "Pelvis", this.worldRight, this.worldForward, -20f, 70f, 30f, typeof(CapsuleCollider), 0.3f, 1.5f);
            this.AddMirroredJoint("Knee", this.leftKnee, this.rightKnee, "Hips", this.worldRight, this.worldForward, -80f, 0f, 0f, typeof(CapsuleCollider), 0.25f, 1.5f);
            this.AddJoint("Middle Spine", this.middleSpine, "Pelvis", this.worldRight, this.worldForward, -20f, 20f, 10f, null, 1f, 2.5f);
            this.AddMirroredJoint("Arm", this.leftArm, this.rightArm, "Middle Spine", this.worldUp, this.worldForward, -70f, 10f, 50f, typeof(CapsuleCollider), 0.25f, 1f);
            this.AddMirroredJoint("Elbow", this.leftElbow, this.rightElbow, "Arm", this.worldForward, this.worldUp, -90f, 0f, 0f, typeof(CapsuleCollider), 0.2f, 1f);
            this.AddJoint("Head", this.head, "Middle Spine", this.worldRight, this.worldForward, -40f, 25f, 25f, null, 1f, 1f);
        }

        private static int SecondLargestComponent(Vector3 point)
        {
            int num = SmallestComponent(point);
            int num2 = LargestComponent(point);
            if (num < num2)
            {
                int num3 = num2;
                num2 = num;
                num = num3;
            }
            if ((num == 0) && (num2 == 1))
            {
                return 2;
            }
            if ((num == 0) && (num2 == 2))
            {
                return 1;
            }
            return 0;
        }

        private static int SmallestComponent(Vector3 point)
        {
            int num = 0;
            if (Mathf.Abs(point[1]) < Mathf.Abs(point[0]))
            {
                num = 1;
            }
            if (Mathf.Abs(point[2]) < Mathf.Abs(point[num]))
            {
                num = 2;
            }
            return num;
        }

        private class BoneInfo
        {
            public Transform anchor;
            public Vector3 axis;
            public ArrayList children = new ArrayList();
            public Type colliderType;
            public float density;
            public CharacterJoint joint;
            public float maxLimit;
            public float minLimit;
            public string name;
            public Vector3 normalAxis;
            public RagdollBuilder.BoneInfo parent;
            public float radiusScale;
            public float summedMass;
            public float swingLimit;
        }
    }
}

