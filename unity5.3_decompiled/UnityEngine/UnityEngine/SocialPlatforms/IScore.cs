namespace UnityEngine.SocialPlatforms
{
    using System;

    public interface IScore
    {
        void ReportScore(Action<bool> callback);

        DateTime date { get; }

        string formattedValue { get; }

        string leaderboardID { get; set; }

        int rank { get; }

        string userID { get; }

        long value { get; set; }
    }
}

