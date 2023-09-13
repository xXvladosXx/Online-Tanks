using System;
using UnityEngine;

namespace Codebase.Runtime.Utils
{
    public class SpawnOnDestroy : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;

        private void OnDestroy()
        {
            Instantiate(_prefab, transform.position, Quaternion.identity);
        }
    }
}