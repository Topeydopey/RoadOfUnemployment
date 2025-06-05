using System.Collections.Generic;
using UnityEngine;

public class EnemyScatter : MonoBehaviour
{
    [Header("Area")]
    public BoxCollider2D area;                   // ← drag your collider here

    [Header("Enemies")]
    public List<GameObject> enemyPrefabs;
    public int totalEnemies = 30;
    public float avoidRadius = 1f;
    public LayerMask obstacleMask;

    void Start()
    {
        if (!area)
        {
            Debug.LogError("EnemyScatter: no BoxCollider2D assigned!");
            return;
        }

        Bounds b = area.bounds;                 // world-space AABB
        int spawned = 0, attempts = 0;

        while (spawned < totalEnemies && attempts < totalEnemies * 10)
        {
            attempts++;

            Vector2 pos = new(
                Random.Range(b.min.x, b.max.x),
                Random.Range(b.min.y, b.max.y));

            if (Physics2D.OverlapCircle(pos, avoidRadius, obstacleMask)) continue;

            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            Instantiate(prefab, pos, Quaternion.identity, transform);
            spawned++;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!area) return;
        Gizmos.color = new Color(1, 0, 0, 0.25f);
        Gizmos.DrawCube(area.bounds.center, area.bounds.size);
    }
#endif
}
