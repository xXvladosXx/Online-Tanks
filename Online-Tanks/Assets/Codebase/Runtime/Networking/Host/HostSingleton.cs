using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Codebase.Runtime.Networking.Host
{
    public class HostSingleton : MonoBehaviour
    {
        private static HostSingleton _instance;
        
        public static HostSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<HostSingleton>();
                    if (_instance == null)
                    {
                        return null;
                    }
                }

                return _instance;
            }
        }
        
        public HostGameManager GameManager { get; private set; }

        private void OnDestroy()
        {
            GameManager?.Dispose();
        }

        public void MakePersistent()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void CreateHost()
        {
            GameManager = new HostGameManager();
        }
    }
}