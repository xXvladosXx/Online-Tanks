using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Codebase
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
