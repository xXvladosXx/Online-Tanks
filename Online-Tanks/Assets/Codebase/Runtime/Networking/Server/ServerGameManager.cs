using System;
using System.Threading.Tasks;
using Codebase.Runtime.Networking.Server.Services;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Codebase.Runtime.Networking.Server
{
    public class ServerGameManager : IDisposable
    {
        private readonly string _serverIP;
        private readonly int _serverPort;
        private readonly int _serverQPort;
        private readonly MultiplayAllocationService _multiplayAllocationService;
        private readonly NetworkServer _networkServer;

        private const string SCENE_NAME = "Gameplay";

        public ServerGameManager(string serverIP, int serverPort,
            int serverQPort, NetworkManager networkManager)
        {
            _serverIP = serverIP;
            _serverPort = serverPort;
            _serverQPort = serverQPort;
            _networkServer = new NetworkServer(networkManager);
            _multiplayAllocationService = new MultiplayAllocationService();
        }
        
        public async Task StartGameServerAsync()
        {
            await _multiplayAllocationService.BeginServerCheck();

            if (!_networkServer.OpenConnection(_serverIP, _serverPort))
            {
                Debug.LogError("Failed to open connection sever");
                return;
            }
            
            NetworkManager.Singleton.SceneManager.LoadScene(SCENE_NAME, LoadSceneMode.Single);
        }

        public void Dispose()
        {
            _multiplayAllocationService?.Dispose();
            _networkServer?.Shutdown();
        }
    }
}