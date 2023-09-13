using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Codebase.Runtime.Player.Resource
{
    public class CoinSpawner : NetworkBehaviour
    {
        [SerializeField] private RespawningCoin _coinPrefab;
        [SerializeField] private float _spawnInterval = 5f;
        [SerializeField] private int _maxCoins = 5;
        [SerializeField] private int _coinsValue = 3;
        [SerializeField] private Vector2 _xSpawnRange = new Vector2(-10f, 10f);
        [SerializeField] private Vector2 _ySpawnRange = new Vector2(-10f, 10f);
        [SerializeField] private LayerMask _spawnLayerMask;
        
        private readonly Collider2D[] _spawnOverlapResults = new Collider2D[1];
        private float _coinRadius;

        public override void OnNetworkSpawn()
        {
            if(!IsServer)
                return;
            
            _coinRadius = _coinPrefab.GetComponent<CircleCollider2D>().radius;

            for (int i = 0; i < _maxCoins; i++)
            {
                SpawnCoin();
            }
        }

        private void SpawnCoin()
        {
            var coin = Instantiate(_coinPrefab, GetRandomSpawnPosition(), Quaternion.identity);
            coin.SetValue(_coinsValue);
            coin.GetComponent<NetworkObject>().Spawn();
            
            coin.OnCollected += OnCollected;
        }

        private void OnCollected(RespawningCoin coin)
        {
            coin.transform.position = GetRandomSpawnPosition();
            coin.Reset();
        }

        private Vector2 GetRandomSpawnPosition()
        {
            while (true)
            {
                var x = Random.Range(_xSpawnRange.x, _xSpawnRange.y);
                var y = Random.Range(_ySpawnRange.x, _ySpawnRange.y);
                var spawnPosition = new Vector2(x, y);
                var result = Physics2D.OverlapCircleNonAlloc(spawnPosition, _coinRadius, _spawnOverlapResults, _spawnLayerMask);
                if (result == 0)
                    return spawnPosition;
            }
        }
    }
}