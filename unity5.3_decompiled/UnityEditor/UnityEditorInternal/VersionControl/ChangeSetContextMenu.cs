namespace UnityEditorInternal.VersionControl
{
    using System;
    using UnityEditor.VersionControl;

    public class ChangeSetContextMenu
    {
        private static void DeleteChangeSet(int userData)
        {
            Provider.DeleteChangeSets(ListControl.FromID(userData).SelectedChangeSets).SetCompletionAction(CompletionAction.UpdatePendingWindow);
        }

        private static bool DeleteChangeSetTest(int userData)
        {
            ListControl control = ListControl.FromID(userData);
            ChangeSets selectedChangeSets = control.SelectedChangeSets;
            if (selectedChangeSets.Count == 0)
            {
                return false;
            }
            ChangeSet changeSet = GetChangeSet(selectedChangeSets);
            if (changeSet.id == "-1")
            {
                return false;
            }
            ListItem changeSetItem = control.GetChangeSetItem(changeSet);
            bool flag = (((changeSetItem != null) && changeSetItem.HasChildren) && (changeSetItem.FirstChild.Asset != null)) && (changeSetItem.FirstChild.Name != "Empty change list");
            if (!flag)
            {
                Task task = Provider.ChangeSetStatus(changeSet);
                task.Wait();
                flag = task.assetList.Count != 0;
            }
            return (!flag && Provider.DeleteChangeSetsIsValid(selectedChangeSets));
        }

        private static void EditChangeSet(int userData)
        {
            ChangeSet changeSet = GetChangeSet(ListControl.FromID(userData).SelectedChangeSets);
            if (changeSet != null)
            {
                WindowChange.Open(changeSet, new AssetList(), false);
            }
        }

        private static bool EditChangeSetTest(int userData)
        {
            ChangeSets selectedChangeSets = ListControl.FromID(userData).SelectedChangeSets;
            if (selectedChangeSets.Count == 0)
            {
                return false;
            }
            return ((GetChangeSet(selectedChangeSets).id != "-1") && Provider.SubmitIsValid(selectedChangeSets[0], null));
        }

        private static ChangeSet GetChangeSet(ChangeSets changes)
        {
            if (changes.Count == 0)
            {
                return null;
            }
            return changes[0];
        }

        private static void NewChangeSet(int userData)
        {
            WindowChange.Open(new AssetList(), false);
        }

        private static bool NewChangeSetTest(int userDatad)
        {
            return Provider.isActive;
        }

        private static void Resolve(int userData)
        {
            ChangeSet changeSet = GetChangeSet(ListControl.FromID(userData).SelectedChangeSets);
            if (changeSet != null)
            {
                WindowResolve.Open(changeSet);
            }
        }

        private static bool ResolveTest(int userData)
        {
            return (ListControl.FromID(userData).SelectedChangeSets.Count > 0);
        }

        private static void Revert(int userData)
        {
            ChangeSet changeSet = GetChangeSet(ListControl.FromID(userData).SelectedChangeSets);
            if (changeSet != null)
            {
                WindowRevert.Open(changeSet);
            }
        }

        private static bool RevertTest(int userData)
        {
            return (ListControl.FromID(userData).SelectedChangeSets.Count > 0);
        }

        private static void RevertUnchanged(int userData)
        {
            Provider.RevertChangeSets(ListControl.FromID(userData).SelectedChangeSets, RevertMode.Unchanged).SetCompletionAction(CompletionAction.UpdatePendingWindow);
            Provider.InvalidateCache();
        }

        private static bool RevertUnchangedTest(int userData)
        {
            return (ListControl.FromID(userData).SelectedChangeSets.Count > 0);
        }

        private static void Submit(int userData)
        {
            ChangeSet changeSet = GetChangeSet(ListControl.FromID(userData).SelectedChangeSets);
            if (changeSet != null)
            {
                WindowChange.Open(changeSet, new AssetList(), true);
            }
        }

        private static bool SubmitTest(int userData)
        {
            ChangeSets selectedChangeSets = ListControl.FromID(userData).SelectedChangeSets;
            return ((selectedChangeSets.Count > 0) && Provider.SubmitIsValid(selectedChangeSets[0], null));
        }
    }
}

