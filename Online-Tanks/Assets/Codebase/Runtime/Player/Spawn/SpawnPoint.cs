using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Codebase.Runtime.Player.Spawn
{
    public class SpawnPoint : MonoBehaviour
    {
        private static readonly List<SpawnPoint> SpawnPoints = new List<SpawnPoint>();

        private void OnEnable() => 
            SpawnPoints.Add(this);

        private void OnDisable() => 
            SpawnPoints.Remove(this);

        public static Vector3 GetRandomSpawnPoint() => 
            SpawnPoints.Count == 0 ? Vector3.zero : SpawnPoints[Random.Range(0, SpawnPoints.Count)].transform.position;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 1f);
        }
    }
}