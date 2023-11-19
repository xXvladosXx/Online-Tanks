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

        public override void OnNetworkSpawn()
        {
            if (!IsClient) 
                return;
            
            _health.CurrentHealth.OnValueChanged += OnHealthChanged;
            OnHealthChanged(0, _health.CurrentHealth.Value);
        }

        private void OnHealthChanged(int oldHealth, int newHealth)
        {
            _healthBar.fillAmount = (float)newHealth / _health.MaxHealth;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsClient) 
                return;
            
            _health.CurrentHealth.OnValueChanged -= OnHealthChanged;
        }
    }
}