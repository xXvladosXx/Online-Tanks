using System;
using Unity.Netcode;
using UnityEngine;

namespace Codebase.Runtime.Player.Resource
{
    public class CoinCollector : NetworkBehaviour
    {
        public NetworkVariable<int> Coins = new();

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
    }
}