using System.Collections;
using Codebase.Runtime.Player.Spawn;
using Unity.Netcode;
using UnityEngine;

namespace Codebase.Runtime.Player.Combat
{
    public class RespawnHandler : NetworkBehaviour
    {
        [SerializeField] private Player _playerPrefab;
        [SerializeField] private float coinPercentage;

        public override void OnNetworkSpawn()
        {
            if(!IsServer)
                return;

            var players = FindObjectsByType<Player>(FindObjectsSortMode.None);
            foreach (var player in players)
            {
                OnPlayerSpawned(player);
            }
            
            Player.OnPlayerSpawned += OnPlayerSpawned;
            Player.OnPlayerDespawned += OnPlayerDespawned;
        }

        public override void OnNetworkDespawn()
        {
            if(!IsServer)
                return;
            
            Player.OnPlayerSpawned -= OnPlayerSpawned;
            Player.OnPlayerDespawned -= OnPlayerDespawned;
        }

        private void OnPlayerSpawned(Player player)
        {
            player.Health.OnDied += (x) => OnPlayerDied(player);
        }

        private void OnPlayerDespawned(Player player)
        {
        }

        private void OnPlayerDied(Player player)
        {
            var keptCoins = (int) (player.CoinCollector.Coins.Value * (coinPercentage / 100));
            Destroy(player.gameObject);
            StartCoroutine(RespawnPlayer(player.OwnerClientId, keptCoins));
        }

        private IEnumerator RespawnPlayer(ulong ownerClientId, int keptCoins)
        {
            yield return null;
            var player = Instantiate(_playerPrefab, SpawnPoint.GetRandomSpawnPoint(), Quaternion.identity);
            player.NetworkObject.SpawnAsPlayerObject(ownerClientId);
            player.CoinCollector.Coins.Value = keptCoins;
        }
    }
}