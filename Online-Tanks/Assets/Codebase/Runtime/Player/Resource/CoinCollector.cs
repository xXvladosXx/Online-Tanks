using System;
using Codebase.Runtime.Player.Combat;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Codebase.Runtime.Player.Resource
{
    public class CoinCollector : NetworkBehaviour
    {
        [SerializeField] private BountyCoin _coinPrefab;
        [SerializeField] private int _bountyCoinCount = 10;
        [SerializeField] private int _minBountyCoinValue = 1;
        [SerializeField] private float _spawnRadius = 3f;
        [SerializeField] private float _bountyPercentage = 0.5f;
        [SerializeField] private LayerMask _spawnLayerMask; 
        [SerializeField] private Health _health;
        
        private Collider2D[] _spawnBuffer = new Collider2D[1];
        
        public NetworkVariable<int> Coins = new();

        public override void OnNetworkSpawn()
        {
            if(!IsServer)
                return;
            
            _health.OnDied += OnDied;
        }
        
        public override void OnNetworkDespawn()
        {
            if(!IsServer)
                return;
            
            _health.OnDied -= OnDied;
        }

        private void OnDied(Health health)
        {
            var bountyValue = (int)(Coins.Value * _bountyPercentage);
            var bountyCoinValue = bountyValue / _bountyCoinCount;
            
            if(bountyCoinValue < _minBountyCoinValue)
                return;

            for (int i = 0; i < _bountyCoinCount; i++)
            {
                var coin = Instantiate(_coinPrefab, GetRandomSpawnPosition(), Quaternion.identity);
                coin.SetValue(bountyCoinValue);
                coin.NetworkObject.Spawn();
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.TryGetComponent(out Coin coin)) 
                return;
            
            int collected = coin.Collect();
            if(!IsServer)
                return;
            
            Coins.Value += collected;
        }
        
        public void Spend(int amount)
        {
            if(!IsServer)
                return;
            
            Coins.Value -= amount;
        }
        
        private Vector2 GetRandomSpawnPosition()
        {
            while (true)
            {
                var spawnPosition = (Vector2) transform.position + Random.insideUnitCircle * _spawnRadius;
                var result = Physics2D.OverlapCircleNonAlloc(spawnPosition, _coinPrefab.Collider.radius, _spawnBuffer, _spawnLayerMask);
                if (result == 0)
                    return spawnPosition;
            }
        }
    }
}