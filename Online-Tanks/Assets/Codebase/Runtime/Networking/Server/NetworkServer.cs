using System.Collections.Generic;
using System.Text;
using Codebase.Runtime.Networking.Shared;
using Unity.Netcode;
using UnityEngine;

namespace Codebase.Runtime.Networking.Server
{
    public class NetworkServer 
    {
        private readonly Dictionary<ulong, string> _connectedClients = new Dictionary<ulong, string>();
        private readonly Dictionary<string, UserData> _connectedUsers = new Dictionary<string, UserData>();

        private readonly NetworkManager _networkManager;

        public NetworkServer(NetworkManager networkManager)
        {
            _networkManager = networkManager;
        }

        public void Initialize()
        {
            _networkManager.ConnectionApprovalCallback += ApprovalCheck;
            _networkManager.OnServerStarted += OnServerStarted;
        }

        public void Shutdown()
        {
            _networkManager.ConnectionApprovalCallback -= ApprovalCheck;
            _networkManager.OnServerStarted -= OnServerStarted;
            OnServerStopped();
            
            if(_networkManager.IsListening)
                _networkManager.Shutdown();
        }

        private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response)
        {
            var payload = Encoding.UTF8.GetString(request.Payload);
            var userData = JsonUtility.FromJson<UserData>(payload);
            
            _connectedClients[request.ClientNetworkId] = userData.UserId;
            _connectedUsers[userData.UserId] = userData;
            
            response.Approved = true;
            response.CreatePlayerObject = true;
        }

        private void OnServerStarted() => _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
        private void OnServerStopped() => _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;

        private void OnClientDisconnect(ulong clientId)
        {
            if (_connectedClients.TryGetValue(clientId, out var userId))
            {
                _connectedUsers.Remove(userId);
                _connectedClients.Remove(clientId);
            }
        }
    }
}