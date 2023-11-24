using System.Threading.Tasks;
using Codebase.Runtime.Networking.Host;
using Codebase.Runtime.Networking.Shared;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;

namespace Codebase.Runtime.Networking.Server
{
    public class ServerSingleton : MonoBehaviour
    {
        private static ServerSingleton _instance;
        
        public static ServerSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ServerSingleton>();
                    if (_instance == null)
                    {
                        return null;
                    }
                }

                return _instance;
            }
        }
        
        public ServerGameManager GameManager { get; private set; }

        public void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public async Task CreateServer(NetworkObject playerPrefab)
        {
            await UnityServices.InitializeAsync();

            GameManager = new ServerGameManager(
                ApplicationData.IP(),
                ApplicationData.Port(),
                ApplicationData.QPort(),
                NetworkManager.Singleton,
                playerPrefab
            );
        }

        private void OnDestroy()
        {
            GameManager?.Dispose();
        }
    }
}