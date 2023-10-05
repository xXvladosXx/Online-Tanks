using System;
using System.Collections.Generic;
using Codebase.Runtime.Networking.Client;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Codebase.Runtime.UI
{
    public class LobbiesList : MonoBehaviour
    {
        [SerializeField] private LobbyItem _lobbyItemPrefab;
        [SerializeField] private Transform _content;

        private bool _isJoining;
        private bool _isRefreshing;

        private void OnEnable()
        {
            RefreshList();
        }

        public async void RefreshList()
        {
            if(_isRefreshing)
                return;
            
            _isRefreshing = true;
            
            var options = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter>
                {
                    new(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                    new(QueryFilter.FieldOptions.IsLocked, "0", QueryFilter.OpOptions.EQ),
                }
            };

            var lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

            foreach (Transform child in _content)
            {
                Destroy(child.gameObject);
            }

            foreach (var lobby in lobbies.Results)
            {
                var lobbyItem = Instantiate(_lobbyItemPrefab, _content);
                lobbyItem.Init(this, lobby);
            }
            
            _isRefreshing = false;
        }

        public async void JoinAsync(Lobby lobby)
        {
            if(_isJoining)
                return;
            
            _isJoining = true;
            
            try
            {
                var joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
                var joinCode = joiningLobby.Data["JoinCode"].Value;

                await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
            }
            catch (Exception exception)
            {
                Debug.Log(exception.StackTrace);
                Debug.Log(exception);
            }
            
            _isJoining = false;
        }
    }
}