using UnityEngine;

namespace Codebase.Runtime.Utils
{
    public class DestroyOnContact : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D col)
        {
            Destroy(gameObject);
        }
    }
}