using Unity.Netcode;
using UnityEngine;

namespace Codebase.Runtime.Utils
{
    public class JoinServer : MonoBehaviour
    {
        public void StartHost()
        {
            NetworkManager.Singleton.StartHost();
        }
        
        public void Join()
        {
            NetworkManager.Singleton.StartClient();
        }
    }
}
