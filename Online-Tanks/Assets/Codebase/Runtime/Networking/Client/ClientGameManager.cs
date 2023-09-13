using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine.SceneManagement;

namespace Codebase.Runtime.Networking.Client
{
    public class ClientGameManager 
    {
        public async Task<bool> InitAsync()
        {
            await UnityServices.InitializeAsync();
            var state = await AuthenticationWrapper.TryAuth();
            if (state == AuthState.Authenticated)
            {
                return true;
            }

            return false;   
        }

        public void GoToMenu()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}