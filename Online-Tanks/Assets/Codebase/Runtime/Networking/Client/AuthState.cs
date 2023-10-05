namespace Codebase.Runtime.Networking.Client
{
    public enum AuthState
    {
        NotAuthenticated,
        Authenticating,
        Authenticated,
        Failed,
        Timeout
    }
}