namespace UnityEngine.SocialPlatforms
{
    using System;

    public interface ILocalUser : IUserProfile
    {
        void Authenticate(Action<bool> callback);
        void LoadFriends(Action<bool> callback);

        bool authenticated { get; }

        IUserProfile[] friends { get; }

        bool underage { get; }
    }
}

