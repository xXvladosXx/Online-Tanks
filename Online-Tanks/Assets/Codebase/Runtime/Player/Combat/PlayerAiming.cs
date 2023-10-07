using System;
using Codebase.Runtime.Input;
using Unity.Netcode;
using UnityEngine;

namespace Codebase.Runtime.Player.Combat
{
    public class PlayerAiming : NetworkBehaviour
    {
        [SerializeField] private Transform _aimingTransform;
        [SerializeField] private InputReader _inputReader;

        private void LateUpdate()
        {
            if (!IsOwner)
                return;

            var mousePosition = _inputReader.MouseAim;
            var mouseWorldPosition = UnityEngine.Camera.main.ScreenToWorldPoint(mousePosition);
            Vector2 direction = (mouseWorldPosition - transform.position).normalized;
            _aimingTransform.up = direction;
        }
    }
}