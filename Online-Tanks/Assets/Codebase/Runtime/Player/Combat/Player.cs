using System;
using Cinemachine;
using Codebase.Runtime.Networking.Host;
using Codebase.Runtime.Player.Resource;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Codebase.Runtime.Player.Combat
{
    public class Player : NetworkBehaviour
    {
        [field: SerializeField] public Health Health { get; private set; }
        [field: SerializeField] public CoinCollector CoinCollector { get; private set; }
        [SerializeField] private SpriteRenderer _miniMapIcon;
        [SerializeField] private CinemachineVirtualCamera _tankCamera;
        [SerializeField] private int _priority = 15;
        
        public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

        public static event Action<Player> OnPlayerSpawned;
        public static event Action<Player> OnPlayerDespawned;
        
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                var data = HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
                PlayerName.Value = data.Username;
                OnPlayerSpawned?.Invoke(this);
            }

            if (IsOwner)
            {
                _tankCamera.Priority = _priority;
                _miniMapIcon.color = Color.blue;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                OnPlayerDespawned?.Invoke(this);
            }
        }
    }
}