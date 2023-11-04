using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Codebase.Runtime.Player.Combat
{
    public class HealingZone : NetworkBehaviour
    {
        [SerializeField] private Image _healingBar;
        [SerializeField] private int _maxHealing = 25;
        [SerializeField] private float _healCooldown = 30f;
        [SerializeField] private float _healInterval = 0.5f;
        [SerializeField] private int _cost = 10;
        [SerializeField] private int _healthPerInterval = 1;
        
        private float _remainingCooldown;
        private float _tick;
        
        private List<Player> _playersInZone = new List<Player>();
        public NetworkVariable<int> HealPower = new NetworkVariable<int>();
        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                HealPower.OnValueChanged += HandleHealPowerChanged;
                HandleHealPowerChanged(0, HealPower.Value);
            }

            if (IsServer)
            {
                HealPower.Value = _maxHealing;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsClient)
            {
                HealPower.OnValueChanged -= HandleHealPowerChanged;
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if(!IsServer)
                return;

            if (col.attachedRigidbody.TryGetComponent(out Player player))
            {
                _playersInZone.Add(player);
            }
        }
        
        private void OnTriggerExit2D(Collider2D col)
        {
            if(!IsServer)
                return;

            if (col.attachedRigidbody.TryGetComponent(out Player player))
            {
                _playersInZone.Remove(player);
            }
        }

        private void Update()
        {
            if(!IsServer)
                return;

            if (_remainingCooldown > 0f)
            {
                _remainingCooldown -= Time.deltaTime;
                if (_remainingCooldown <= 0)
                {
                    HealPower.Value = _maxHealing;
                }
                else
                {
                    return;
                }
            }
            
            _tick += Time.deltaTime;
            if (_tick >= 1 / _healInterval)
            {
                foreach (var player in _playersInZone)
                {
                    if (HealPower.Value == 0)
                        break;
                    
                    if(player.Health.CurrentHealth.Value == player.Health.MaxHealth)
                        continue;
                    
                    if(player.CoinCollector.Coins.Value < _cost)
                        continue;
                    
                    player.CoinCollector.Spend(_cost);
                    player.Health.Heal(_healthPerInterval);

                    HealPower.Value -= 1;

                    if (HealPower.Value == 0)
                    {
                        _remainingCooldown = _healCooldown;
                    }
                }
                
                _tick %= (1 / _healInterval);
            }
        }

        private void HandleHealPowerChanged(int oldPower, int newPower)
        {
            _healingBar.fillAmount = (float) newPower / _maxHealing;
        }
    }
}