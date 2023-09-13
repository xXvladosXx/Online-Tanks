using System.Net;
using System.Threading.Tasks;
using Unity.Services.Authentication;

namespace Codebase.Runtime.Networking.Client
{
    public static class AuthenticationWrapper
    {
        public static AuthState State { get; private set; } = AuthState.None;
        
        public static async Task<AuthState> TryAuth(int tries = 5)
        {
            if(State == AuthState.Authenticated)
                return State;

            State = AuthState.Authenticating;
            
            int currentTries = 0;
            while (State == AuthState.Authenticating && currentTries < tries)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    State = AuthState.Authenticated;
                    break;
                }

                tries++;
                await Task.Delay(1000);
            }

            return State;
        }
    }

    public enum AuthState
    {
        None,
        Authenticating,
        Authenticated,
        Failed,
        Timeout
    }
}