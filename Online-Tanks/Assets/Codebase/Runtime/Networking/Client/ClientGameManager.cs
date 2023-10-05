using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Codebase.Runtime.Networking.Client
{
    public class ClientGameManager 
    {
        private JoinAllocation _joinAllocation;

        public async Task<bool> InitAsync()
        {
            await UnityServices.InitializeAsync();
            var state = await AuthenticationWrapper.TryAuth();
            if (state == AuthState.Authenticated)
            {
                return true;
            }

            return false;   
        }

        public void GoToMenu()
        {
            SceneManager.LoadScene("Menu");
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
            
            NetworkManager.Singleton.StartClient();
        }
    }
}