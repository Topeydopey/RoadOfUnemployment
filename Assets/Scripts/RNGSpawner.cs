using UnityEngine;
using System.Collections.Generic;

public class RNGSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableObject
    {
        public GameObject prefab;
        public int spawnCount = 10;
        public float minDistanceBetween = 2f;
        public LayerMask overlapMask;
    }

    [Header("Spawn Area Settings")]
    public float spawnRadius = 30f;
    public LayerMask groundLayer; // Optional: for placing only on walkable areas

    [Header("Objects to Spawn")]
    public List<SpawnableObject> spawnableObjects = new List<SpawnableObject>();

    void Start()
    {
        foreach (var obj in spawnableObjects)
        {
            SpawnObjects(obj);
        }
    }

    void SpawnObjects(SpawnableObject obj)
    {
        int spawned = 0;
        int maxAttempts = obj.spawnCount * 10;

        int attempts = 0;
        while (spawned < obj.spawnCount && attempts < maxAttempts)
        {
            Vector2 randomPos = GetRandomPointInCircle();

            if (!Physics2D.OverlapCircle(randomPos, obj.minDistanceBetween, obj.overlapMask))
            {
                Instantiate(obj.prefab, randomPos, Quaternion.identity);
                spawned++;
            }

            attempts++;
        }

        if (spawned < obj.spawnCount)
        {
            Debug.LogWarning($"Only spawned {spawned}/{obj.spawnCount} of {obj.prefab.name}. Try increasing max radius or lowering min distance.");
        }
    }

    Vector2 GetRandomPointInCircle()
    {
        Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;
        return (Vector2)transform.position + randomPoint;
    }
}