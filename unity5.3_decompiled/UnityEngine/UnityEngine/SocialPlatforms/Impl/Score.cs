namespace UnityEngine.SocialPlatforms.Impl
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.SocialPlatforms;

    public class Score : IScore
    {
        private DateTime m_Date;
        private string m_FormattedValue;
        private int m_Rank;
        private string m_UserID;

        public Score() : this("unkown", -1L)
        {
        }

        public Score(string leaderboardID, long value) : this(leaderboardID, value, "0", DateTime.Now, string.Empty, -1)
        {
        }

        public Score(string leaderboardID, long value, string userID, DateTime date, string formattedValue, int rank)
        {
            this.leaderboardID = leaderboardID;
            this.value = value;
            this.m_UserID = userID;
            this.m_Date = date;
            this.m_FormattedValue = formattedValue;
            this.m_Rank = rank;
        }

        public void ReportScore(Action<bool> callback)
        {
            ActivePlatform.Instance.ReportScore(this.value, this.leaderboardID, callback);
        }

        public void SetDate(DateTime date)
        {
            this.m_Date = date;
        }

        public void SetFormattedValue(string value)
        {
            this.m_FormattedValue = value;
        }

        public void SetRank(int rank)
        {
            this.m_Rank = rank;
        }

        public void SetUserID(string userID)
        {
            this.m_UserID = userID;
        }

        public override string ToString()
        {
            object[] objArray1 = new object[] { "Rank: '", this.m_Rank, "' Value: '", this.value, "' Category: '", this.leaderboardID, "' PlayerID: '", this.m_UserID, "' Date: '", this.m_Date };
            return string.Concat(objArray1);
        }

        public DateTime date
        {
            get
            {
                return this.m_Date;
            }
        }

        public string formattedValue
        {
            get
            {
                return this.m_FormattedValue;
            }
        }

        public string leaderboardID { get; set; }

        public int rank
        {
            get
            {
                return this.m_Rank;
            }
        }

        public string userID
        {
            get
            {
                return this.m_UserID;
            }
        }

        public long value { get; set; }
    }
}

