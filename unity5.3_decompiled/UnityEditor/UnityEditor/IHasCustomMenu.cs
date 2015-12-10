namespace UnityEditor
{
    using System;

    public interface IHasCustomMenu
    {
        void AddItemsToMenu(GenericMenu menu);
    }
}

