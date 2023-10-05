using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Codebase.Runtime.UI
{
    public class LobbyItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _playerCount;
        
        private LobbiesList _lobbiesList;
        private Lobby _lobby;

        public void Init(LobbiesList lobbiesList, Lobby lobby)
        {
            _lobbiesList = lobbiesList;
            _lobby = lobby;
            
            _name.text = lobby.Name;
            _playerCount.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
        }

        public void Join()
        {
            _lobbiesList.JoinAsync(_lobby);
        }
    }
}