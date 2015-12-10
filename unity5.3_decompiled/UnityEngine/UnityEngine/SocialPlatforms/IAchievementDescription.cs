namespace UnityEngine.SocialPlatforms
{
    using System;
    using UnityEngine;

    public interface IAchievementDescription
    {
        string achievedDescription { get; }

        bool hidden { get; }

        string id { get; set; }

        Texture2D image { get; }

        int points { get; }

        string title { get; }

        string unachievedDescription { get; }
    }
}

