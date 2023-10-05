using System;
using System.Net;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Codebase.Runtime.Networking.Client
{
    public static class AuthenticationWrapper
    {
        public static AuthState State { get; private set; } = AuthState.NotAuthenticated;
        
        public static async Task<AuthState> TryAuth(int tries = 5)
        {
            if(State == AuthState.Authenticated)
                return State;

            if (State == AuthState.Authenticating)
            {
                await Authenticating();
                return State;
            }   
            
            await SignInAnonymouslyAsync(tries);

            return State;
        }

        private static async Task Authenticating()
        {
            while (State is AuthState.Authenticating or AuthState.NotAuthenticated)
            {
                await Task.Delay(200);
            }
        }
        
        private static async Task SignInAnonymouslyAsync(int tries)
        {
            State = AuthState.Authenticating;
            
            int currentTries = 0;
            while (State == AuthState.Authenticating && currentTries < tries)
            {
                try
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    
                    if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                    {
                        State = AuthState.Authenticated;
                        break;
                    }
                }
                catch (AuthenticationException exception)
                {
                    Debug.LogError(exception);
                    Debug.LogError(exception.StackTrace);
                    
                    State = AuthState.Failed;
                }
                catch (RequestFailedException exception)
                {
                    Debug.LogError(exception);
                    Debug.LogError(exception.StackTrace);
                    
                    State = AuthState.Failed;
                }

                currentTries++;
                await Task.Delay(1000);
            }

            if (State != AuthState.Authenticated)
            {
                Debug.LogWarning("Failed to authenticate after " + tries + " tries.");
                State = AuthState.Timeout;
            }
        }
    }
}