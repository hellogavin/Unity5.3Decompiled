namespace UnityEditor.TreeViewTests
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor;

    internal class FooTreeViewItem : TreeViewItem
    {
        public FooTreeViewItem(int id, int depth, TreeViewItem parent, string displayName, BackendData.Foo foo) : base(id, depth, parent, displayName)
        {
            this.foo = foo;
        }

        public BackendData.Foo foo { get; private set; }
    }
}

