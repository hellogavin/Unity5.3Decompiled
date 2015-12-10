namespace UnityEngine.SocialPlatforms
{
    using System;

    public interface IAchievement
    {
        void ReportProgress(Action<bool> callback);

        bool completed { get; }

        bool hidden { get; }

        string id { get; set; }

        DateTime lastReportedDate { get; }

        double percentCompleted { get; set; }
    }
}

