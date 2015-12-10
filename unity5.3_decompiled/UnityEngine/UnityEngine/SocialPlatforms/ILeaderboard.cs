namespace UnityEngine.SocialPlatforms
{
    using System;

    public interface ILeaderboard
    {
        void LoadScores(Action<bool> callback);
        void SetUserFilter(string[] userIDs);

        string id { get; set; }

        bool loading { get; }

        IScore localUserScore { get; }

        uint maxRange { get; }

        Range range { get; set; }

        IScore[] scores { get; }

        TimeScope timeScope { get; set; }

        string title { get; }

        UserScope userScope { get; set; }
    }
}

