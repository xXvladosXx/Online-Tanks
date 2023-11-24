using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codebase.Runtime.Networking.Client;
using Codebase.Runtime.Networking.Host;
using Codebase.Runtime.Networking.Server;
using Codebase.Runtime.Networking.Shared;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Codebase.Runtime.Networking
{
    public class ApplicationController : MonoBehaviour
    {
        [SerializeField] private ClientSingleton _clientPrefab;
        [SerializeField] private HostSingleton _hostPrefab;
        [SerializeField] private ServerSingleton _serverPrefab;
        [SerializeField] private NetworkObject _playerPrefab;

        private ApplicationData _applicationData;
        private const string GameSceneName = "Gameplay";

        private async void Start()
        {
            DontDestroyOnLoad(gameObject);
            await LaunchInMode(SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null);
        }

        private async Task LaunchInMode(bool isServer)
        {
            if (isServer)
            {
                Application.targetFrameRate = 60;
                _applicationData = new ApplicationData();
                var serverSingleton = Instantiate(_serverPrefab);
                StartCoroutine(LoadGameSceneAsync(serverSingleton));
            }
            else
            {
                var hostSingleton = Instantiate(_hostPrefab);
                hostSingleton.CreateHost(_playerPrefab);
                
                var clientSingleton = Instantiate(_clientPrefab);
                bool auth = await clientSingleton.CreateClient();

                if (auth)
                {
                    clientSingleton.GameManager.GoToMenu();
                }
            }
        }
        
        private IEnumerator LoadGameSceneAsync(ServerSingleton serverSingleton)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(GameSceneName);

            while (!asyncOperation.isDone)
            {
                yield return null;
            }

            Task createServerTask = serverSingleton.CreateServer(_playerPrefab);
            yield return new WaitUntil(() => createServerTask.IsCompleted);

            Task startServerTask = serverSingleton.GameManager.StartGameServerAsync();
            yield return new WaitUntil(() => startServerTask.IsCompleted);
        }
    }
}