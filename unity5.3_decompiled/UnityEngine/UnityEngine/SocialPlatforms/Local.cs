namespace UnityEngine.SocialPlatforms
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.SocialPlatforms.Impl;

    public class Local : ISocialPlatform
    {
        [CompilerGenerated]
        private static Comparison<Score> <>f__am$cache7;
        private List<AchievementDescription> m_AchievementDescriptions = new List<AchievementDescription>();
        private List<Achievement> m_Achievements = new List<Achievement>();
        private Texture2D m_DefaultTexture;
        private List<UserProfile> m_Friends = new List<UserProfile>();
        private List<Leaderboard> m_Leaderboards = new List<Leaderboard>();
        private static LocalUser m_LocalUser;
        private List<UserProfile> m_Users = new List<UserProfile>();

        public IAchievement CreateAchievement()
        {
            return new Achievement();
        }

        private Texture2D CreateDummyTexture(int width, int height)
        {
            Texture2D textured = new Texture2D(width, height);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Color color = ((j & i) <= 0) ? Color.gray : Color.white;
                    textured.SetPixel(j, i, color);
                }
            }
            textured.Apply();
            return textured;
        }

        public ILeaderboard CreateLeaderboard()
        {
            return new Leaderboard();
        }

        public void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
        {
            if (this.VerifyUser() && (callback != null))
            {
                callback(this.m_AchievementDescriptions.ToArray());
            }
        }

        public void LoadAchievements(Action<IAchievement[]> callback)
        {
            if (this.VerifyUser() && (callback != null))
            {
                callback(this.m_Achievements.ToArray());
            }
        }

        public void LoadScores(string leaderboardID, Action<IScore[]> callback)
        {
            if (this.VerifyUser())
            {
                foreach (Leaderboard leaderboard in this.m_Leaderboards)
                {
                    if (leaderboard.id == leaderboardID)
                    {
                        this.SortScores(leaderboard);
                        if (callback != null)
                        {
                            callback(leaderboard.scores);
                        }
                        return;
                    }
                }
                Debug.LogError("Leaderboard not found");
                if (callback != null)
                {
                    callback(new Score[0]);
                }
            }
        }

        public void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback)
        {
            List<UserProfile> list = new List<UserProfile>();
            if (this.VerifyUser())
            {
                foreach (string str in userIDs)
                {
                    foreach (UserProfile profile in this.m_Users)
                    {
                        if (profile.id == str)
                        {
                            list.Add(profile);
                        }
                    }
                    foreach (UserProfile profile2 in this.m_Friends)
                    {
                        if (profile2.id == str)
                        {
                            list.Add(profile2);
                        }
                    }
                }
                callback(list.ToArray());
            }
        }

        private void PopulateStaticData()
        {
            this.m_Friends.Add(new UserProfile("Fred", "1001", true, UserState.Online, this.m_DefaultTexture));
            this.m_Friends.Add(new UserProfile("Julia", "1002", true, UserState.Online, this.m_DefaultTexture));
            this.m_Friends.Add(new UserProfile("Jeff", "1003", true, UserState.Online, this.m_DefaultTexture));
            this.m_Users.Add(new UserProfile("Sam", "1004", false, UserState.Offline, this.m_DefaultTexture));
            this.m_Users.Add(new UserProfile("Max", "1005", false, UserState.Offline, this.m_DefaultTexture));
            this.m_AchievementDescriptions.Add(new AchievementDescription("Achievement01", "First achievement", this.m_DefaultTexture, "Get first achievement", "Received first achievement", false, 10));
            this.m_AchievementDescriptions.Add(new AchievementDescription("Achievement02", "Second achievement", this.m_DefaultTexture, "Get second achievement", "Received second achievement", false, 20));
            this.m_AchievementDescriptions.Add(new AchievementDescription("Achievement03", "Third achievement", this.m_DefaultTexture, "Get third achievement", "Received third achievement", false, 15));
            Leaderboard item = new Leaderboard();
            item.SetTitle("High Scores");
            item.id = "Leaderboard01";
            item.SetScores(new List<Score> { new Score("Leaderboard01", 300L, "1001", DateTime.Now.AddDays(-1.0), "300 points", 1), new Score("Leaderboard01", 0xffL, "1002", DateTime.Now.AddDays(-1.0), "255 points", 2), new Score("Leaderboard01", 0x37L, "1003", DateTime.Now.AddDays(-1.0), "55 points", 3), new Score("Leaderboard01", 10L, "1004", DateTime.Now.AddDays(-1.0), "10 points", 4) }.ToArray());
            this.m_Leaderboards.Add(item);
        }

        public void ReportProgress(string id, double progress, Action<bool> callback)
        {
            if (this.VerifyUser())
            {
                foreach (Achievement achievement in this.m_Achievements)
                {
                    if ((achievement.id == id) && (achievement.percentCompleted <= progress))
                    {
                        if (progress >= 100.0)
                        {
                            achievement.SetCompleted(true);
                        }
                        achievement.SetHidden(false);
                        achievement.SetLastReportedDate(DateTime.Now);
                        achievement.percentCompleted = progress;
                        if (callback != null)
                        {
                            callback(true);
                        }
                        return;
                    }
                }
                foreach (AchievementDescription description in this.m_AchievementDescriptions)
                {
                    if (description.id == id)
                    {
                        bool completed = progress >= 100.0;
                        Achievement item = new Achievement(id, progress, completed, false, DateTime.Now);
                        this.m_Achievements.Add(item);
                        if (callback != null)
                        {
                            callback(true);
                        }
                        return;
                    }
                }
                Debug.LogError("Achievement ID not found");
                if (callback != null)
                {
                    callback(false);
                }
            }
        }

        public void ReportScore(long score, string board, Action<bool> callback)
        {
            if (this.VerifyUser())
            {
                foreach (Leaderboard leaderboard in this.m_Leaderboards)
                {
                    if (leaderboard.id == board)
                    {
                        leaderboard.SetScores(new List<Score>((Score[]) leaderboard.scores) { new Score(board, score, this.localUser.id, DateTime.Now, score + " points", 0) }.ToArray());
                        if (callback != null)
                        {
                            callback(true);
                        }
                        return;
                    }
                }
                Debug.LogError("Leaderboard not found");
                if (callback != null)
                {
                    callback(false);
                }
            }
        }

        private void SetLocalPlayerScore(Leaderboard board)
        {
            foreach (Score score in board.scores)
            {
                if (score.userID == this.localUser.id)
                {
                    board.SetLocalUserScore(score);
                    break;
                }
            }
        }

        public void ShowAchievementsUI()
        {
            Debug.Log("ShowAchievementsUI not implemented");
        }

        public void ShowLeaderboardUI()
        {
            Debug.Log("ShowLeaderboardUI not implemented");
        }

        private void SortScores(Leaderboard board)
        {
            List<Score> list = new List<Score>((Score[]) board.scores);
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = (s1, s2) => s2.value.CompareTo(s1.value);
            }
            list.Sort(<>f__am$cache7);
            for (int i = 0; i < list.Count; i++)
            {
                list[i].SetRank(i + 1);
            }
        }

        void ISocialPlatform.Authenticate(ILocalUser user, Action<bool> callback)
        {
            LocalUser user2 = (LocalUser) user;
            this.m_DefaultTexture = this.CreateDummyTexture(0x20, 0x20);
            this.PopulateStaticData();
            user2.SetAuthenticated(true);
            user2.SetUnderage(false);
            user2.SetUserID("1000");
            user2.SetUserName("Lerpz");
            user2.SetImage(this.m_DefaultTexture);
            if (callback != null)
            {
                callback(true);
            }
        }

        bool ISocialPlatform.GetLoading(ILeaderboard board)
        {
            if (!this.VerifyUser())
            {
                return false;
            }
            return ((Leaderboard) board).loading;
        }

        void ISocialPlatform.LoadFriends(ILocalUser user, Action<bool> callback)
        {
            if (this.VerifyUser())
            {
                ((LocalUser) user).SetFriends(this.m_Friends.ToArray());
                if (callback != null)
                {
                    callback(true);
                }
            }
        }

        void ISocialPlatform.LoadScores(ILeaderboard board, Action<bool> callback)
        {
            if (this.VerifyUser())
            {
                Leaderboard leaderboard = (Leaderboard) board;
                foreach (Leaderboard leaderboard2 in this.m_Leaderboards)
                {
                    if (leaderboard2.id == leaderboard.id)
                    {
                        leaderboard.SetTitle(leaderboard2.title);
                        leaderboard.SetScores(leaderboard2.scores);
                        leaderboard.SetMaxRange((uint) leaderboard2.scores.Length);
                    }
                }
                this.SortScores(leaderboard);
                this.SetLocalPlayerScore(leaderboard);
                if (callback != null)
                {
                    callback(true);
                }
            }
        }

        private bool VerifyUser()
        {
            if (!this.localUser.authenticated)
            {
                Debug.LogError("Must authenticate first");
                return false;
            }
            return true;
        }

        public ILocalUser localUser
        {
            get
            {
                if (m_LocalUser == null)
                {
                    m_LocalUser = new LocalUser();
                }
                return m_LocalUser;
            }
        }
    }
}

