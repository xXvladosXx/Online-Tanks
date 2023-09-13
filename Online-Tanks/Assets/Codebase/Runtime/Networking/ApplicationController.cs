using System;
using System.Threading.Tasks;
using Codebase.Runtime.Networking.Client;
using Codebase.Runtime.Networking.Host;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Codebase.Runtime.Networking
{
    public class ApplicationController : MonoBehaviour
    {
        [SerializeField] private ClientSingleton _clientPrefab;
        [SerializeField] private HostSingleton _hostPrefab;
        private async void Start()
        {
            DontDestroyOnLoad(gameObject);
            await LaunchInMode(SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null);
        }

        private async Task LaunchInMode(bool isServer)
        {
            if (isServer)
            {
                
            }
            else
            {
                var clientSingleton = Instantiate(_clientPrefab);
                bool auth = await clientSingleton.CreateClient();
                
                var hostSingleton = Instantiate(_hostPrefab);
                hostSingleton.CreateHost();

                if (auth)
                {
                    clientSingleton.GameManager.GoToMenu();
                }
            }
        }
    }
}