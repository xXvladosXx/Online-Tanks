using Codebase.Runtime.Networking.Client;
using Codebase.Runtime.Networking.Host;
using TMPro;
using UnityEngine;

namespace Codebase.Runtime.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _ipInputField;
        public async void StartHost()
        {
            await HostSingleton.Instance.GameManager.StartHostAsync();
        }
        
        public async void StartClient()
        {
            await ClientSingleton.Instance.GameManager.StartClientAsync(_ipInputField.text);
        }
    }
}