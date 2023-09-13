using Unity.Netcode;
using UnityEngine;

namespace Codebase.Runtime.Player.Resource
{
    public abstract class Coin : NetworkBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] protected int CoinValue = 10;
        
        protected bool IsCollected;

        public void SetValue(int value) => CoinValue = value;
        protected void Show(bool show) => _spriteRenderer.enabled = show;
        
        public abstract int Collect();
    }
}