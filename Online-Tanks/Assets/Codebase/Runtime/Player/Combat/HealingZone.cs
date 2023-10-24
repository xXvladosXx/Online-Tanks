using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
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
        [SerializeField] private int _helathPerInterval = 1;
        
        private List<Player> _playersInZone = new List<Player>();

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
    }
}