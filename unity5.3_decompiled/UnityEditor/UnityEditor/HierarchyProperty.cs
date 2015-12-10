namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public sealed class HierarchyProperty : IHierarchyProperty
    {
        private IntPtr m_Property;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern HierarchyProperty(HierarchyType hierarchytType);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int CountRemaining(int[] expanded);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Dispose();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void FilterSingleSceneObject(int instanceID, bool otherVisibilityState);
        ~HierarchyProperty()
        {
            this.Dispose();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool Find(int instanceID, int[] expanded);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int[] FindAllAncestors(int[] instanceIDs);
        public Scene GetScene()
        {
            Scene scene;
            INTERNAL_CALL_GetScene(this, out scene);
            return scene;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetScene(HierarchyProperty self, out Scene value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool IsExpanded(int[] expanded);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool Next(int[] expanded);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool NextWithDepthCheck(int[] expanded, int minDepth);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool Parent();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool Previous(int[] expanded);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Reset();
        internal void SetSearchFilter(SearchFilter filter)
        {
            this.SetSearchFilterINTERNAL(SearchFilter.Split(filter.nameFilter), filter.classNames, filter.assetLabels, filter.assetBundleNames, filter.referencingInstanceIDs, filter.showAllHits);
        }

        public void SetSearchFilter(string searchString, int mode)
        {
            SearchFilter filter = SearchableEditorWindow.CreateFilter(searchString, (SearchableEditorWindow.SearchMode) mode);
            this.SetSearchFilter(filter);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetSearchFilterINTERNAL(string[] nameFilters, string[] classNames, string[] assetLabels, string[] assetBundleNames, int[] referencingInstanceIDs, bool showAllHits);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool Skip(int count, int[] expanded);

        public bool alphaSorted { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int[] ancestors { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int colorCode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int depth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string guid { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool hasChildren { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool hasFullPreviewImage { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Texture2D icon { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public IconDrawStyle iconDrawStyle { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int instanceID { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isFolder { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isMainRepresentation { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isValid { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public string name { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Object pptrValue { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int row { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
    }
}

