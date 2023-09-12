using System;
using Unity.Netcode;
using UnityEngine;

namespace Codebase.Runtime.Player.Combat
{
    public class Health : NetworkBehaviour
    {
        [field: SerializeField] public int MaxHealth { get; private set; } = 100;
        public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>(100);

        private bool _isDead;

        public event Action<Health> OnDied; 
        public event Action OnHealthChanged; 

        public override void OnNetworkSpawn()
        {
            if(!IsServer)
                return;
            
            CurrentHealth.Value = MaxHealth;
        }
        
        public void TakeDamage(int damage)
        {
            if(!IsServer)
                return;
            
            if(_isDead)
                return;
            
            CurrentHealth.Value -= damage;
            OnHealthChanged?.Invoke();

            if (CurrentHealth.Value <= 0)
            {
                OnDied?.Invoke(this);
                _isDead = true;
            }
        }
    }
}