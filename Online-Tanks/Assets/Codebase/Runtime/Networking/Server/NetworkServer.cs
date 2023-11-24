using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Codebase.Runtime.Networking.Shared;
using Codebase.Runtime.Player.Spawn;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Codebase.Runtime.Networking.Server
{
    public class NetworkServer : IDisposable
    {
        private readonly Dictionary<ulong, string> _connectedClients = new Dictionary<ulong, string>();
        private readonly Dictionary<string, UserData> _connectedUsers = new Dictionary<string, UserData>();

        private readonly NetworkManager _networkManager;
        private readonly NetworkObject _playerPrefab;

        public event Action<string> OnClientLeft;
        public event Action<UserData> OnUserConnected;
        public event Action<UserData> OnUserDisconnected; 

        public NetworkServer(NetworkManager networkManager, NetworkObject networkObject)
        {
            _networkManager = networkManager;
            _playerPrefab = networkObject;

            _networkManager.ConnectionApprovalCallback += ApprovalCheck;
            _networkManager.OnServerStarted += OnServerStarted;
        }

        public void Dispose()
        {
            if (_networkManager == null) 
                return;

            _networkManager.ConnectionApprovalCallback -= ApprovalCheck;
            _networkManager.OnServerStarted -= OnServerStarted;
            _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
            
            if(_networkManager.IsListening)
                _networkManager.Shutdown();
        }

        public bool OpenConnection(string ip, int port)
        {
            var transport =_networkManager.gameObject.GetComponent<UnityTransport>();
            transport.SetConnectionData(ip, (ushort)port);
            return _networkManager.StartServer();
        }
        
        public UserData GetUserDataByClientId(ulong clientId)
        {
            if (_connectedClients.TryGetValue(clientId, out var userId))
            {
                return _connectedUsers.TryGetValue(userId, out var userData) ? userData : null;
            }

            return null;
        }

        private void ApprovalCheck(
            NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response)
        {
            string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
            UserData userData = JsonUtility.FromJson<UserData>(payload);

            _connectedClients[request.ClientNetworkId] = userData.userAuthId;
            _connectedUsers[userData.userAuthId] = userData;
            OnUserConnected?.Invoke(userData);

            _ = SpawnPlayerDelayed(request.ClientNetworkId);

            response.Approved = true;
            response.CreatePlayerObject = false;
        }

        private async Task SpawnPlayerDelayed(ulong clientId)
        {
            await Task.Delay(1000);

            NetworkObject playerInstance =
                GameObject.Instantiate(_playerPrefab, SpawnPoint.GetRandomSpawnPoint(), Quaternion.identity);

            playerInstance.SpawnAsPlayerObject(clientId);
        }

        private void OnServerStarted() => _networkManager.OnClientDisconnectCallback += OnClientDisconnect;

        private void OnClientDisconnect(ulong clientId)
        {
            if (_connectedClients.TryGetValue(clientId, out var userId))
            {
                OnUserDisconnected?.Invoke(_connectedUsers[userId]);
                _connectedUsers.Remove(userId);
                _connectedClients.Remove(clientId);
                OnClientLeft?.Invoke(userId);
            }
        }
    }
}