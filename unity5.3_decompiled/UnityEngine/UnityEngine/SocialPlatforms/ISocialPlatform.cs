namespace UnityEngine.SocialPlatforms
{
    using System;

    public interface ISocialPlatform
    {
        void Authenticate(ILocalUser user, Action<bool> callback);
        IAchievement CreateAchievement();
        ILeaderboard CreateLeaderboard();
        bool GetLoading(ILeaderboard board);
        void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback);
        void LoadAchievements(Action<IAchievement[]> callback);
        void LoadFriends(ILocalUser user, Action<bool> callback);
        void LoadScores(string leaderboardID, Action<IScore[]> callback);
        void LoadScores(ILeaderboard board, Action<bool> callback);
        void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback);
        void ReportProgress(string achievementID, double progress, Action<bool> callback);
        void ReportScore(long score, string board, Action<bool> callback);
        void ShowAchievementsUI();
        void ShowLeaderboardUI();

        ILocalUser localUser { get; }
    }
}

