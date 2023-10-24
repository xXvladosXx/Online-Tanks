using Codebase.Runtime.Player.Combat;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Codebase.Runtime.UI.Player
{
    public class HealthDisplay : NetworkBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private Image _healthBar;
        private bool _wasAssigned;

        public override void OnNetworkSpawn()
        {
            if (!IsClient || _wasAssigned) 
                return;
            
            _wasAssigned = true;
            _health.OnHealthChanged += OnHealthChanged;
            OnHealthChanged();
        }

        public override void OnNetworkDespawn()
        {
            if (!IsClient) 
                return;
            
            _health.OnHealthChanged -= OnHealthChanged;
        }

        private void OnHealthChanged() => 
            _healthBar.fillAmount = (float) _health.CurrentHealth.Value / _health.MaxHealth;
    }
}