namespace UnityEditor.VersionControl
{
    using System;

    public enum CompletionAction
    {
        OnAddedChangeWindow = 7,
        OnChangeContentsPendingWindow = 2,
        OnChangeSetsPendingWindow = 4,
        OnCheckoutCompleted = 8,
        OnGotLatestPendingWindow = 5,
        OnIncomingPendingWindow = 3,
        OnSubmittedChangeWindow = 6,
        UpdatePendingWindow = 1
    }
}

