using Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace Codebase.Runtime.Player.Camera
{
    public class TankCamera : NetworkBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _tankCamera;
        [SerializeField] private int _priority = 15;
        
        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                _tankCamera.Priority = _priority;
            }
        }
    }
}