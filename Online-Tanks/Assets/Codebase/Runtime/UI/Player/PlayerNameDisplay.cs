using System;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Codebase.Runtime.UI.Player
{
    public class PlayerNameDisplay : MonoBehaviour
    {
        [FormerlySerializedAs("_tankCamera")] [SerializeField] private Runtime.Player.Combat.Player _player;
        [SerializeField] private TextMeshProUGUI _nameText;

        private void Start()
        {
            OnPlayerNameChanged(string.Empty, _player.PlayerName.Value);
            _player.PlayerName.OnValueChanged += OnPlayerNameChanged;
        }

        private void OnDestroy()
        {
            _player.PlayerName.OnValueChanged -= OnPlayerNameChanged;
        }

        private void OnPlayerNameChanged(FixedString32Bytes previousName, FixedString32Bytes newName)
        {
            _nameText.text = newName.ToString();
        }
    }
}