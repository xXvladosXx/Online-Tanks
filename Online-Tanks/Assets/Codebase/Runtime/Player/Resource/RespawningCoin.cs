using System;
using UnityEngine;

namespace Codebase.Runtime.Player.Resource
{
    public class RespawningCoin : Coin
    {
        public event Action<RespawningCoin> OnCollected; 
        private Vector3 _previousSpawnPosition;

        private void Update()
        {
            if (_previousSpawnPosition != transform.position)
                Show(true);
            
            _previousSpawnPosition = transform.position;
        }
        
        public override int Collect()
        {
            if (!IsServer)
            {
                Show(false);
                return 0;
            }
            
            if (IsCollected)
                return 0;
            
            IsCollected = true;
            OnCollected?.Invoke(this);
            return CoinValue;
        }

        public void Reset() => IsCollected = false;
    }
}