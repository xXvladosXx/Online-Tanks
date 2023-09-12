using System;
using Unity.Netcode;
using UnityEngine;

namespace Codebase.Runtime.Utils
{
    public class Lifetime : MonoBehaviour
    {
        [SerializeField] private float _lifetime = 1f;

        public void Start() => Destroy(gameObject, _lifetime);
    }
}