using System;
using System.Text;
using System.Threading.Tasks;
using Codebase.Runtime.Networking.Shared;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Codebase.Runtime.Networking.Client
{
    public class ClientGameManager : IDisposable
    { 
        private JoinAllocation _joinAllocation;
        private NetworkClient _networkClient;

        public async Task<bool> InitAsync()
        {
            await UnityServices.InitializeAsync();
            var state = await AuthenticationWrapper.TryAuth();
            
            _networkClient = new NetworkClient(NetworkManager.Singleton);
            _networkClient.Initialize();
            
            if (state == AuthState.Authenticated)
            {
                return true;
            }

            return false;   
        }

        public void Dispose()
        {
            _networkClient?.Shutdown();
        }

        public void GoToMenu()
        {
            SceneManager.LoadScene(Constants.MAIN_MENU_SCENE_NAME);
        }

        public async Task StartClientAsync(string id)
        {
            try
            {
                _joinAllocation = await Relay.Instance.JoinAllocationAsync(id);
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.StackTrace);
                Debug.LogError(exception);
                return;
            } 
            
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            var relayServerData = new RelayServerData(_joinAllocation, "dtls");
            transport.SetRelayServerData(relayServerData);
            
            var userData = new UserData
            {
                Username = PlayerPrefs.GetString(Constants.PLAYER_NAME_PREF_KEY, "Player"),
                UserId = AuthenticationService.Instance.PlayerId
            };

            var payload = JsonUtility.ToJson(userData);
            var payloadBytes = Encoding.UTF8.GetBytes(payload);
            
            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
            
            NetworkManager.Singleton.StartClient();
        }
    }
}