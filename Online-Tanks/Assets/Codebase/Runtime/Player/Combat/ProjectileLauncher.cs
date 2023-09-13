using System;
using Codebase.Runtime.Input;
using Codebase.Runtime.Player.Resource;
using Unity.Netcode;
using UnityEngine;

namespace Codebase.Runtime.Player.Combat
{
    public class ProjectileLauncher : NetworkBehaviour
    {
        [SerializeField] private GameObject _serverProjectilePrefab;
        [SerializeField] private GameObject _clientProjectilePrefab;
        [SerializeField] private GameObject _muzzleFlashPrefab;
        [SerializeField] private Collider2D _playerCollider;
        [SerializeField] private Transform _projectileSpawnPoint;
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private CoinCollector _coinCollector;
        
        [Header("Settings")]
        [SerializeField] private float _projectileSpeed = 10f;
        [SerializeField] private float _fireRate = 0.5f;
        [SerializeField] private float _muzzleFlashDuration = 0.1f;
        [SerializeField] private int _costToFire;
        
        private bool _shouldFire;
        private float _muzzleFlashTimer;
        private float _timer;

        public override void OnNetworkSpawn()
        {
            if(!IsOwner)
                return;
            
            _inputReader.OnFired += OnFired;
        }

        public override void OnNetworkDespawn()
        {
            if(!IsOwner)
                return;
            
            _inputReader.OnFired -= OnFired;
        }

        private void Update()
        {
            if (_muzzleFlashTimer > 0f)
            {
                _muzzleFlashTimer -= Time.deltaTime;

                if (_muzzleFlashTimer <= 0f)
                {
                    _muzzleFlashPrefab.SetActive(false);
                }
            }
            
            if(!IsOwner)
                return;

            if(_timer > 0)
                _timer -= Time.deltaTime;
            
            if(!_shouldFire)
                return;
            
            if(_timer > 0)
                return;

            if(_coinCollector.Coins.Value < _costToFire)
                return;
            
            PrimaryFireServerRpc(_projectileSpawnPoint.position, _projectileSpawnPoint.up );
            SpawnProjectile(_projectileSpawnPoint.position, _projectileSpawnPoint.up);
            
            _timer = 1 / _fireRate;
        }

        [ServerRpc]
        private void PrimaryFireServerRpc(Vector3 position, Vector3 direction)
        {
            _coinCollector.Spend(_costToFire);

            var projectile = Instantiate(_serverProjectilePrefab, position, Quaternion.identity);
            projectile.transform.up = direction;
            Physics2D.IgnoreCollision(_playerCollider, projectile.GetComponent<Collider2D>());

            if(projectile.TryGetComponent(out DealDamageOnContact dealDamageOnContact))
                dealDamageOnContact.SetOwner(OwnerClientId);
            
            if (projectile.TryGetComponent(out Rigidbody2D rigidbody2D))
                rigidbody2D.velocity = rigidbody2D.transform.up * _projectileSpeed;

            SpawnProjectileClientRpc(position, direction);
        }

        [ClientRpc]
        private void SpawnProjectileClientRpc(Vector3 position, Vector3 direction)
        {
            if(IsOwner)
                return;
            
            SpawnProjectile(position, direction);
        }
        
        private void SpawnProjectile(Vector3 position, Vector3 direction)
        {
            _muzzleFlashPrefab.SetActive(true);
            _muzzleFlashTimer = _muzzleFlashDuration;
            
            var projectile = Instantiate(_clientProjectilePrefab, position, Quaternion.identity);
            projectile.transform.up = direction;
            
            Physics2D.IgnoreCollision(_playerCollider, projectile.GetComponent<Collider2D>());

            if (projectile.TryGetComponent(out Rigidbody2D rigidbody2D))
                rigidbody2D.velocity = rigidbody2D.transform.up * _projectileSpeed;
        }

        private void OnFired(bool shouldFire) => _shouldFire = shouldFire;
    }
}