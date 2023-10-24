using UnityEngine;

namespace Codebase.Runtime.Player.Resource
{
    public class BountyCoin : Coin
    {
        [field: SerializeField] public CircleCollider2D Collider { get; private set; }
        public override int Collect()
        {
            if (!IsServer)
            {
                Show(false);
                return 0;
            }
            
            if (IsCollected)
                return 0;
            
            IsCollected = true;
            Destroy(gameObject);
            return CoinValue;
        }
    }
}