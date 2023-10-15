using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Codebase.Runtime.UI.LeaderBoard
{
    public class LeaderBoardEntity : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _displayName;
        [SerializeField] private Color _color;
        public ulong ClientId { get; private set; }
        public FixedString32Bytes PlayerName { get; private set; }
        public int Coins { get; set; }

        public void Initialize(ulong id, FixedString32Bytes playerName, int coins)
        {
            ClientId = id;
            PlayerName = playerName;
            Coins = coins;
            
            if(ClientId == NetworkManager.Singleton.LocalClientId)
                _displayName.color = _color;
            
            UpdateCoins(coins);
        }

        public void UpdateCoins(int coins)
        {
            Coins = coins;
            UpdateText();
        }

        public void UpdateText()
        {
            _displayName.text = $"{transform.GetSiblingIndex() + 1} {PlayerName} ({Coins})";
        }
    }
}