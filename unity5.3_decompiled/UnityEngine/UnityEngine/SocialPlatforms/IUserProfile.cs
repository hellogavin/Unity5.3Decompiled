namespace UnityEngine.SocialPlatforms
{
    using System;
    using UnityEngine;

    public interface IUserProfile
    {
        string id { get; }

        Texture2D image { get; }

        bool isFriend { get; }

        UserState state { get; }

        string userName { get; }
    }
}

