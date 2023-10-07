using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Codebase.Runtime.Networking.Server;
using Codebase.Runtime.Networking.Shared;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Codebase.Runtime.Networking.Host
{
    public class HostGameManager : IDisposable
    {
        private Allocation _allocation;
        private NetworkServer _networkServer;
        
        private string _joinCode;
        private string _lobbyId;
        private Coroutine _heartbeat;

        private const int MAX_CONNECTIONS = 10;
        private const string SCENE_NAME = "Gameplay";

        public async Task StartHostAsync()
        {
            try
            {
               _allocation = await Relay.Instance.CreateAllocationAsync(MAX_CONNECTIONS);
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.StackTrace);
                Debug.LogError(exception);
                return;
            } 
            
            try
            {
                _joinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
                Debug.Log(_joinCode);
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.StackTrace);
                Debug.LogError(exception);
                return;
            }

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            var relayServerData = new RelayServerData(_allocation, "dtls");
            transport.SetRelayServerData(relayServerData);

            try
            {
                var lobbyOptions = new CreateLobbyOptions
                {
                    IsPrivate = false,
                    Data = new Dictionary<string, DataObject>
                    {
                        {"JoinCode", new DataObject(DataObject.VisibilityOptions.Member, _joinCode)}   
                    }
                };

                var playerName = PlayerPrefs.GetString(Constants.PLAYER_NAME_PREF_KEY, "Player");
                var lobby = await Lobbies.Instance.CreateLobbyAsync($"{playerName}'s Lobby", MAX_CONNECTIONS, lobbyOptions);
                _lobbyId = lobby.Id;
                _heartbeat = HostSingleton.Instance.StartCoroutine(HeartbeatLobby(15));
            }
            
            catch (Exception exception)
            {
                Debug.LogError(exception.StackTrace);
                Debug.LogError(exception);
                return;
            }
            
            _networkServer = new NetworkServer(NetworkManager.Singleton);
            _networkServer.Initialize();
            
            var userData = new UserData
            {
                Username = PlayerPrefs.GetString(Constants.PLAYER_NAME_PREF_KEY, "Player"),
                UserId = AuthenticationService.Instance.PlayerId
            };

            var payload = JsonUtility.ToJson(userData);
            var payloadBytes = Encoding.UTF8.GetBytes(payload);
            
            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
            
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene(SCENE_NAME, LoadSceneMode.Single);
        }

        public async void Dispose()
        {
            HostSingleton.Instance.StopCoroutine(_heartbeat);

            if (!string.IsNullOrEmpty(_lobbyId))
            {
                try
                {
                    await Lobbies.Instance.DeleteLobbyAsync(_lobbyId);
                }
                catch (Exception exception)
                {
                    Debug.Log(exception);
                }

                _lobbyId = string.Empty;
            }

            _networkServer?.Shutdown();
        }

        private IEnumerator HeartbeatLobby(float waitTimeSeconds)
        {
            var delay = new WaitForSecondsRealtime(waitTimeSeconds);
            
            while (true)
            {
                Lobbies.Instance.SendHeartbeatPingAsync(_lobbyId);
                yield return delay;
            }
        }
    }
}