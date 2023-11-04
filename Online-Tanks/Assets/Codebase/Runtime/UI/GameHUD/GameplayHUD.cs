using Codebase.Runtime.Networking.Client;
using Codebase.Runtime.Networking.Host;
using Unity.Netcode;
using UnityEngine;

namespace Codebase.Runtime.UI.GameHUD
{
    public class GameplayHUD : MonoBehaviour
    {
        public void LeaveGame()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                HostSingleton.Instance.GameManager.Shutdown();
            }
            
            ClientSingleton.Instance.GameManager.Disconnect();
        }
    }
}