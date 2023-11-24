using System;
using Cinemachine;
using Codebase.Runtime.Networking.Client;
using Codebase.Runtime.Networking.Host;
using Codebase.Runtime.Networking.Server;
using Codebase.Runtime.Networking.Shared;
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
        
        [SerializeField] private Texture2D _crosshair;
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
                var userData = IsHost ? HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId) 
                    : ServerSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
                
                PlayerName.Value = userData.userName;
                OnPlayerSpawned?.Invoke(this);
            }

            if (IsOwner)
            {
                _tankCamera.Priority = _priority;
                _miniMapIcon.color = Color.blue;
                Cursor.SetCursor(_crosshair, new Vector2(_crosshair.width / 2, _crosshair.height / 2), CursorMode.Auto);
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