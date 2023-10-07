using System;
using System.Collections.Generic;
using System.Text;
using Codebase.Runtime.Networking.Shared;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Codebase.Runtime.Networking.Client
{
    public class NetworkClient
    {
        private readonly NetworkManager _networkManager;
        
        public NetworkClient(NetworkManager networkManager)
        {
            _networkManager = networkManager;
        }

        public void Initialize() => _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
        public void Shutdown() => _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;

        private void OnClientDisconnect(ulong clientId)
        {
            if(clientId != 0 && clientId != _networkManager.LocalClientId)
                return;

            if (SceneManager.GetActiveScene().name != Constants.MAIN_MENU_SCENE_NAME)
            {
                SceneManager.LoadScene(Constants.MAIN_MENU_SCENE_NAME);
            }
            
            if(_networkManager.IsConnectedClient)
                _networkManager.Shutdown();
        }
        
    }
}