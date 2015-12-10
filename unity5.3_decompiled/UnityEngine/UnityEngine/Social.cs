namespace UnityEngine
{
    using System;
    using UnityEngine.SocialPlatforms;

    public static class Social
    {
        public static IAchievement CreateAchievement()
        {
            return Active.CreateAchievement();
        }

        public static ILeaderboard CreateLeaderboard()
        {
            return Active.CreateLeaderboard();
        }

        public static void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
        {
            Active.LoadAchievementDescriptions(callback);
        }

        public static void LoadAchievements(Action<IAchievement[]> callback)
        {
            Active.LoadAchievements(callback);
        }

        public static void LoadScores(string leaderboardID, Action<IScore[]> callback)
        {
            Active.LoadScores(leaderboardID, callback);
        }

        public static void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback)
        {
            Active.LoadUsers(userIDs, callback);
        }

        public static void ReportProgress(string achievementID, double progress, Action<bool> callback)
        {
            Active.ReportProgress(achievementID, progress, callback);
        }

        public static void ReportScore(long score, string board, Action<bool> callback)
        {
            Active.ReportScore(score, board, callback);
        }

        public static void ShowAchievementsUI()
        {
            Active.ShowAchievementsUI();
        }

        public static void ShowLeaderboardUI()
        {
            Active.ShowLeaderboardUI();
        }

        public static ISocialPlatform Active
        {
            get
            {
                return ActivePlatform.Instance;
            }
            set
            {
                ActivePlatform.Instance = value;
            }
        }

        public static ILocalUser localUser
        {
            get
            {
                return Active.localUser;
            }
        }
    }
}

