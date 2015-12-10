namespace UnityEditor.TreeViewTests
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class TreeViewTestWindow : EditorWindow, IHasCustomMenu
    {
        private BackendData m_BackendData;
        private BackendData m_BackendData2;
        private TestType m_TestType = TestType.TreeWithCustomItemHeight;
        private TreeViewTest m_TreeViewTest;
        private TreeViewTest m_TreeViewTest2;
        private TreeViewTestWithCustomHeight m_TreeViewWithCustomHeight;

        public TreeViewTestWindow()
        {
            base.titleContent = new GUIContent("TreeView Test");
        }

        public virtual void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Large TreeView"), this.m_TestType == TestType.LargeTreesWithStandardGUI, (GenericMenu.MenuFunction) (() => (this.m_TestType = TestType.LargeTreesWithStandardGUI)));
            menu.AddItem(new GUIContent("Custom Item Height TreeView"), this.m_TestType == TestType.TreeWithCustomItemHeight, (GenericMenu.MenuFunction) (() => (this.m_TestType = TestType.TreeWithCustomItemHeight)));
        }

        private void OnEnable()
        {
            base.position = new Rect(100f, 100f, 600f, 600f);
        }

        private void OnGUI()
        {
            switch (this.m_TestType)
            {
                case TestType.LargeTreesWithStandardGUI:
                    this.TestLargeTreesWithFixedItemHeightAndPingingAndFraming();
                    break;

                case TestType.TreeWithCustomItemHeight:
                    this.TestTreeWithCustomItemHeights();
                    break;
            }
        }

        private void TestLargeTreesWithFixedItemHeightAndPingingAndFraming()
        {
            Rect rect = new Rect(0f, 0f, base.position.width / 2f, base.position.height);
            Rect rect2 = new Rect(base.position.width / 2f, 0f, base.position.width / 2f, base.position.height);
            if (this.m_TreeViewTest == null)
            {
                this.m_BackendData = new BackendData();
                this.m_BackendData.GenerateData(0xf4240);
                bool lazy = false;
                this.m_TreeViewTest = new TreeViewTest(this, lazy);
                this.m_TreeViewTest.Init(rect, this.m_BackendData);
                lazy = true;
                this.m_TreeViewTest2 = new TreeViewTest(this, lazy);
                this.m_TreeViewTest2.Init(rect2, this.m_BackendData);
            }
            this.m_TreeViewTest.OnGUI(rect);
            this.m_TreeViewTest2.OnGUI(rect2);
            EditorGUI.DrawRect(new Rect(rect.xMax - 1f, 0f, 2f, base.position.height), new Color(0.4f, 0.4f, 0.4f, 0.8f));
        }

        private void TestTreeWithCustomItemHeights()
        {
            Rect rect = new Rect(0f, 0f, base.position.width, base.position.height);
            if (this.m_TreeViewWithCustomHeight == null)
            {
                this.m_BackendData2 = new BackendData();
                this.m_BackendData2.GenerateData(300);
                this.m_TreeViewWithCustomHeight = new TreeViewTestWithCustomHeight(this, this.m_BackendData2, rect);
            }
            this.m_TreeViewWithCustomHeight.OnGUI(rect);
        }

        private enum TestType
        {
            LargeTreesWithStandardGUI,
            TreeWithCustomItemHeight
        }
    }
}

