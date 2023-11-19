using System;

namespace Codebase.Runtime.Networking.Shared
{
    public enum Map
    {
        Default
    }

    public enum GameMode
    {
        Default
    }

    public enum GameQueue
    {
        Solo,
        Team
    }

    [Serializable]
    public class GameInfo
    {
        public Map Map;
        public GameMode GameMode;
        public GameQueue GameQueue;
    }
    
    [Serializable]
    public class UserData
    {
        public string Username;
        public string UserId;
        public GameInfo GamePreferences;

        public string ToMultiplayQueue() => "";
    }
}