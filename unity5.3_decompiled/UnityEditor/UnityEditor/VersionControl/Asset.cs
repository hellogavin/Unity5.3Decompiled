namespace UnityEditor.VersionControl
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEditor;
    using UnityEngine;

    public sealed class Asset
    {
        private GUID m_guid;

        public Asset(string clientPath)
        {
            this.InternalCreateFromString(clientPath);
        }

        internal string AllStateToString()
        {
            return AllStateToString(this.state);
        }

        internal static string AllStateToString(States state)
        {
            StringBuilder builder = new StringBuilder();
            if (IsState(state, States.AddedLocal))
            {
                builder.AppendLine("Added Local");
            }
            if (IsState(state, States.AddedRemote))
            {
                builder.AppendLine("Added Remote");
            }
            if (IsState(state, States.CheckedOutLocal))
            {
                builder.AppendLine("Checked Out Local");
            }
            if (IsState(state, States.CheckedOutRemote))
            {
                builder.AppendLine("Checked Out Remote");
            }
            if (IsState(state, States.Conflicted))
            {
                builder.AppendLine("Conflicted");
            }
            if (IsState(state, States.DeletedLocal))
            {
                builder.AppendLine("Deleted Local");
            }
            if (IsState(state, States.DeletedRemote))
            {
                builder.AppendLine("Deleted Remote");
            }
            if (IsState(state, States.Local))
            {
                builder.AppendLine("Local");
            }
            if (IsState(state, States.LockedLocal))
            {
                builder.AppendLine("Locked Local");
            }
            if (IsState(state, States.LockedRemote))
            {
                builder.AppendLine("Locked Remote");
            }
            if (IsState(state, States.OutOfSync))
            {
                builder.AppendLine("Out Of Sync");
            }
            if (IsState(state, States.Synced))
            {
                builder.AppendLine("Synced");
            }
            if (IsState(state, States.Missing))
            {
                builder.AppendLine("Missing");
            }
            if (IsState(state, States.ReadOnly))
            {
                builder.AppendLine("ReadOnly");
            }
            return builder.ToString();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Dispose();
        public void Edit()
        {
            Object target = this.Load();
            if (target != null)
            {
                AssetDatabase.OpenAsset(target);
            }
        }

        ~Asset()
        {
            this.Dispose();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void InternalCreateFromString(string clientPath);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool IsChildOf(Asset other);
        public bool IsOneOfStates(States[] states)
        {
            foreach (States states2 in states)
            {
                if ((this.state & states2) != States.None)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsState(States state)
        {
            return IsState(this.state, state);
        }

        internal static bool IsState(States isThisState, States partOfThisState)
        {
            return ((isThisState & partOfThisState) != States.None);
        }

        public Object Load()
        {
            if ((this.state != States.DeletedLocal) && !this.isMeta)
            {
                return AssetDatabase.LoadAssetAtPath(this.path, typeof(Object));
            }
            return null;
        }

        internal string StateToString()
        {
            return StateToString(this.state);
        }

        internal static string StateToString(States state)
        {
            if (IsState(state, States.AddedLocal))
            {
                return "Added Local";
            }
            if (IsState(state, States.AddedRemote))
            {
                return "Added Remote";
            }
            if (IsState(state, States.CheckedOutLocal) && !IsState(state, States.LockedLocal))
            {
                return "Checked Out Local";
            }
            if (IsState(state, States.CheckedOutRemote) && !IsState(state, States.LockedRemote))
            {
                return "Checked Out Remote";
            }
            if (IsState(state, States.Conflicted))
            {
                return "Conflicted";
            }
            if (IsState(state, States.DeletedLocal))
            {
                return "Deleted Local";
            }
            if (IsState(state, States.DeletedRemote))
            {
                return "Deleted Remote";
            }
            if (IsState(state, States.Local))
            {
                return "Local";
            }
            if (IsState(state, States.LockedLocal))
            {
                return "Locked Local";
            }
            if (IsState(state, States.LockedRemote))
            {
                return "Locked Remote";
            }
            if (IsState(state, States.OutOfSync))
            {
                return "Out Of Sync";
            }
            return string.Empty;
        }

        public string fullName { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isFolder { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isInCurrentProject { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isMeta { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal bool IsUnderVersionControl
        {
            get
            {
                return ((this.IsState(States.Synced) || this.IsState(States.OutOfSync)) || this.IsState(States.AddedLocal));
            }
        }

        public bool locked { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string name { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string path { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string prettyPath
        {
            get
            {
                return this.path;
            }
        }

        public bool readOnly { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public States state { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Flags]
        public enum States
        {
            AddedLocal = 0x100,
            AddedRemote = 0x200,
            CheckedOutLocal = 0x10,
            CheckedOutRemote = 0x20,
            Conflicted = 0x400,
            DeletedLocal = 0x40,
            DeletedRemote = 0x80,
            Local = 1,
            LockedLocal = 0x800,
            LockedRemote = 0x1000,
            MetaFile = 0x8000,
            Missing = 8,
            None = 0,
            OutOfSync = 4,
            ReadOnly = 0x4000,
            Synced = 2,
            Updating = 0x2000
        }
    }
}

