using System;
using System.Threading.Tasks;
using Codebase.Runtime.Networking.Client;
using Codebase.Runtime.Networking.Host;
using Codebase.Runtime.Networking.Server;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Codebase.Runtime.Networking
{
    public class ApplicationController : MonoBehaviour
    {
        [SerializeField] private ClientSingleton _clientPrefab;
        [SerializeField] private HostSingleton _hostPrefab;
        [SerializeField] private ServerSingleton _serverPrefab;
        private async void Start()
        {
            DontDestroyOnLoad(gameObject);
            await LaunchInMode(SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null);
        }

        private async Task LaunchInMode(bool isServer)
        {
            if (isServer)
            {
                var serverSingleton = Instantiate(_serverPrefab);
                await serverSingleton.CreateServer();
                await serverSingleton.GameManager.StartGameServerAsync();
            }
            else
            {
                var hostSingleton = Instantiate(_hostPrefab);
                hostSingleton.MakePersistent();
                hostSingleton.CreateHost();
                
                var clientSingleton = Instantiate(_clientPrefab);
                bool auth = await clientSingleton.CreateClient();

                if (auth)
                {
                    clientSingleton.GameManager.GoToMenu();
                }
            }
        }
    }
}