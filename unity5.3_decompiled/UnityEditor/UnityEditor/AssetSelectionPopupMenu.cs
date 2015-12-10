namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class AssetSelectionPopupMenu
    {
        [CompilerGenerated]
        private static Comparison<Object> <>f__am$cache0;

        private static List<Object> FindAssetsOfType(string[] classNames)
        {
            HierarchyProperty property = new HierarchyProperty(HierarchyType.Assets);
            SearchFilter filter = new SearchFilter {
                classNames = classNames
            };
            property.SetSearchFilter(filter);
            List<Object> list = new List<Object>();
            while (property.Next(null))
            {
                list.Add(property.pptrValue);
            }
            return list;
        }

        private static void SelectCallback(object userData)
        {
            Object obj2 = userData as Object;
            if (obj2 != null)
            {
                Selection.activeInstanceID = obj2.GetInstanceID();
            }
        }

        public static void Show(Rect buttonRect, string[] classNames, int initialSelectedInstanceID)
        {
            GenericMenu menu = new GenericMenu();
            List<Object> source = FindAssetsOfType(classNames);
            if (source.Any<Object>())
            {
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = (result1, result2) => EditorUtility.NaturalCompare(result1.name, result2.name);
                }
                source.Sort(<>f__am$cache0);
                foreach (Object obj2 in source)
                {
                    GUIContent content = new GUIContent(obj2.name);
                    bool on = obj2.GetInstanceID() == initialSelectedInstanceID;
                    menu.AddItem(content, on, new GenericMenu.MenuFunction2(AssetSelectionPopupMenu.SelectCallback), obj2);
                }
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("No Audio Mixers found in this project"));
            }
            menu.DropDown(buttonRect);
        }
    }
}

