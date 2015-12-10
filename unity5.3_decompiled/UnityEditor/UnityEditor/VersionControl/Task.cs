namespace UnityEditor.VersionControl
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class Task
    {
        private IntPtr m_thisDummy;

        internal Task()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Dispose();
        ~Task()
        {
            this.Dispose();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern Asset[] Internal_GetAssetList();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern ChangeSet[] Internal_GetChangeSets();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern Message[] Internal_GetMessages();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetCompletionAction(CompletionAction action);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Wait();

        public AssetList assetList
        {
            get
            {
                AssetList list = new AssetList();
                foreach (Asset asset in this.Internal_GetAssetList())
                {
                    list.Add(asset);
                }
                return list;
            }
        }

        public ChangeSets changeSets
        {
            get
            {
                ChangeSets sets = new ChangeSets();
                foreach (ChangeSet set in this.Internal_GetChangeSets())
                {
                    sets.Add(set);
                }
                return sets;
            }
        }

        public string description { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Message[] messages { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string progressMessage { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int progressPct { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int resultCode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int secondsSpent { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool success { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string text { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int userIdentifier { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

