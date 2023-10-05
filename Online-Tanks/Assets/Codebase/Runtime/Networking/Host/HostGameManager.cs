using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Codebase.Runtime.Networking.Host
{
    public class HostGameManager 
    {
        private Allocation _allocation;
        private JoinAllocation _joinAllocation;
        private string _joinCode;
        private string _lobbyId;
        
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

                var lobby = await Lobbies.Instance.CreateLobbyAsync("My Lobby", MAX_CONNECTIONS, lobbyOptions);
                _lobbyId = lobby.Id;
                HostSingleton.Instance.StartCoroutine(HeartbeatLobby(15));
            }
            
            catch (Exception exception)
            {
                Debug.LogError(exception.StackTrace);
                Debug.LogError(exception);
                return;
            }
            
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene(SCENE_NAME, LoadSceneMode.Single);
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