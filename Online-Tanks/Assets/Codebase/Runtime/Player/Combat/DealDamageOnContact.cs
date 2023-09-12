using System;
using Unity.Netcode;
using UnityEngine;

namespace Codebase.Runtime.Player.Combat
{
    public class DealDamageOnContact : MonoBehaviour
    {
        [SerializeField] private int _damage = 15;
        private ulong _owner;

        public void SetOwner(ulong owner) => _owner = owner;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.attachedRigidbody == null)
                return;
            
            if (other.attachedRigidbody.TryGetComponent(out NetworkObject networkObject))
            {
                if(networkObject.OwnerClientId == _owner)
                    return;
            }
            
            if (other.attachedRigidbody.TryGetComponent(out Health health))
            {
                health.TakeDamage(_damage);
            } 
        }
    }
}