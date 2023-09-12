using System;
using Codebase.Runtime.Input;
using Unity.Netcode;
using UnityEngine;

namespace Codebase.Runtime.Player
{
    public class PlayerMovement : NetworkBehaviour, IMovement
    {
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        
        [Header("Settings")]
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _rotationSpeed = 200f;
        
        private Vector2 _movementInput;

        public override void OnNetworkSpawn()
        {
            if(!IsOwner)
                return;
            
            _inputReader.OnMoved += OnMoved;
            _inputReader.OnFired += OnFired;
        }

        public override void OnNetworkDespawn()
        {
            if(!IsOwner)
                return;
            
            _inputReader.OnMoved -= OnMoved;
            _inputReader.OnFired -= OnFired;
        }

        private void Update()
        {
            if(!IsOwner)
                return;

            var zRotation = _movementInput.x * -_rotationSpeed * Time.deltaTime;
            _bodyTransform.Rotate(0f,0f, zRotation);
        }

        private void FixedUpdate()
        {
            if(!IsOwner)
                return;
            
            var movement = _movementInput.y * _speed;
            _rigidbody2D.velocity = _bodyTransform.up * movement;
        }

        private void OnMoved(Vector2 movement)
        {
            _movementInput = movement;
        }

        private void OnFired(bool isFiring)
        {
            
        }
    }
}