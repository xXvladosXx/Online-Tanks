using System;
using Codebase.Runtime.Networking.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Codebase.Runtime.UI
{
    public class NameSelector : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private Button _connectButton;
        [SerializeField] private int _minNameLength = 3;
        [SerializeField] private int _maxNameLength = 12;
        
        private void Start()
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                return;
            }
            
            _nameInput.text = PlayerPrefs.GetString(Constants.PLAYER_NAME_PREF_KEY, string.Empty);
            HandleNameChanged();
        }

        public void HandleNameChanged()
        {
            _connectButton.interactable = _nameInput.text.Length >= _minNameLength && _nameInput.text.Length <= _maxNameLength;
        }

        public void Connect()
        {
            PlayerPrefs.SetString(Constants.PLAYER_NAME_PREF_KEY, _nameInput.text);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}