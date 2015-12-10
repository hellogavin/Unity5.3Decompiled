namespace UnityEditor
{
    using System;

    internal interface IFlexibleMenuItemProvider
    {
        int Add(object obj);
        int Count();
        object Create();
        object GetItem(int index);
        string GetName(int index);
        int[] GetSeperatorIndices();
        bool IsModificationAllowed(int index);
        void Move(int index, int destIndex, bool insertAfterDestIndex);
        void Remove(int index);
        void Replace(int index, object newPresetObject);
    }
}

