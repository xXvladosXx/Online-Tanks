using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Codebase.Runtime.UI.LeaderBoard
{
    public class LeaderBoardHolder : NetworkBehaviour
    {
        [SerializeField] private Transform _container;
        [SerializeField] private LeaderBoardEntity _entityPrefab;
        [SerializeField] private int _entitiesToDisplayCount = 5;
        
        private readonly NetworkList<LeaderBoardEntityState> _leaderBoardEntities = new();
        private readonly List<LeaderBoardEntity> _entitiesToDisplay = new();

        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                _leaderBoardEntities.OnListChanged += HandleListChanged;
                foreach (var entity in _leaderBoardEntities)
                {
                    HandleListChanged(new NetworkListEvent<LeaderBoardEntityState>()
                    {
                        Type = NetworkListEvent<LeaderBoardEntityState>.EventType.Add,
                        Value = entity
                    });
                }
            }
            
            if(!IsServer)
                return;
            
            var players = FindObjectsByType<Runtime.Player.Combat.Player>(FindObjectsSortMode.None);
            foreach (var player in players)
            {
                HandlePlayerSpawned(player);
            }
            
            Runtime.Player.Combat.Player.OnPlayerSpawned += HandlePlayerSpawned;
            Runtime.Player.Combat.Player.OnPlayerDespawned += HandlePlayerDespawned;
        }

        public override void OnNetworkDespawn()
        {
            if(IsClient)
                _leaderBoardEntities.OnListChanged -= HandleListChanged;
            
            if(!IsServer)
                return;
            
            Runtime.Player.Combat.Player.OnPlayerSpawned -= HandlePlayerSpawned;
            Runtime.Player.Combat.Player.OnPlayerDespawned -= HandlePlayerDespawned;
        }

        private void HandleListChanged(NetworkListEvent<LeaderBoardEntityState> changed)
        {
            var searchedEntity = _entitiesToDisplay.FirstOrDefault(x => x.ClientId == changed.Value.ClientID);

            switch (changed.Type)
            {
                case NetworkListEvent<LeaderBoardEntityState>.EventType.Add:
                    if (_entitiesToDisplay.All(x => x.ClientId != changed.Value.ClientID))
                    {
                        var entity = Instantiate(_entityPrefab, _container);
                        entity.Initialize(changed.Value.ClientID, changed.Value.Username, changed.Value.Coins);
                        _entitiesToDisplay.Add(entity);
                    }
                    break;
                case NetworkListEvent<LeaderBoardEntityState>.EventType.Remove:
                    if (searchedEntity != null)
                    {
                        searchedEntity.transform.SetParent(null);
                        Destroy(searchedEntity.gameObject);
                        _entitiesToDisplay.Remove(searchedEntity);
                    }
                    break;
                case NetworkListEvent<LeaderBoardEntityState>.EventType.Value:
                    if(searchedEntity != null)
                        searchedEntity.UpdateCoins(changed.Value.Coins);
                    break;
                default:
                    Debug.Log("Unhandled event type: " + changed.Type);
                    break;
            }
            
            _entitiesToDisplay.Sort((a, b) => b.Coins.CompareTo(a.Coins));
            for (int i = 0; i < _entitiesToDisplay.Count; i++)
            {
                _entitiesToDisplay[i].transform.SetSiblingIndex(i);
                _entitiesToDisplay[i].UpdateText();
                
                var shouldShow = i <= _entitiesToDisplayCount - 1;
                _entitiesToDisplay[i].gameObject.SetActive(shouldShow);
            }

            var display = _entitiesToDisplay.FirstOrDefault(x => x.ClientId == NetworkManager.Singleton.LocalClientId);

            if (display != null)
            {
                if (display.transform.GetSiblingIndex() >= _entitiesToDisplayCount)
                {
                    _container.GetChild(_entitiesToDisplayCount - 1).gameObject.SetActive(false);
                    display.gameObject.SetActive(true);
                }
            }
        }

        private void HandlePlayerSpawned(Runtime.Player.Combat.Player player)
        {
            _leaderBoardEntities.Add(new LeaderBoardEntityState()
            {
                ClientID = player.OwnerClientId,
                Username = player.PlayerName.Value,
                Coins = 0
            });  
            
            player.CoinCollector.Coins.OnValueChanged += (oldCoins, newCoins) => HandleCoinsChanged(player.OwnerClientId, newCoins);
            HandleCoinsChanged(player.OwnerClientId, player.CoinCollector.Coins.Value);
        }

        private void HandleCoinsChanged(ulong clientID, int newValue)
        {
            for (int i = 0; i < _leaderBoardEntities.Count; i++)
            {
                if (_leaderBoardEntities[i].ClientID != clientID)
                    continue;

                _leaderBoardEntities[i] = new LeaderBoardEntityState()
                {
                    ClientID = _leaderBoardEntities[i].ClientID,
                    Username = _leaderBoardEntities[i].Username,
                    Coins = newValue
                };
                
                return;
            }
        }

        private void HandlePlayerDespawned(Runtime.Player.Combat.Player player)
        {
            if(player == null)
                return;
            
            foreach (var boardEntity in _leaderBoardEntities)
            {
                if(boardEntity.ClientID != player.OwnerClientId)
                    continue;
                
                _leaderBoardEntities.Remove(boardEntity);
                break;
            }
            
            player.CoinCollector.Coins.OnValueChanged -= (oldCoins, newCoins) => HandleCoinsChanged(player.OwnerClientId, newCoins);
        }
    }
}