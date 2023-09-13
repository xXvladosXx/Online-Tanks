using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Codebase.Runtime.Networking.Client
{
    public class ClientSingleton : MonoBehaviour
    {
        private static ClientSingleton _instance;
        
        public static ClientSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ClientSingleton>();
                    if (_instance == null)
                    {
                        var go = new GameObject(nameof(ClientSingleton));
                        _instance = go.AddComponent<ClientSingleton>();
                    }
                }

                return _instance;
            }
        }
        
        public ClientGameManager GameManager { get; private set; }
        
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public async Task<bool> CreateClient()
        {
            GameManager = new ClientGameManager();
            return await GameManager.InitAsync();
        }
    }
}