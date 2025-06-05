using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class VortexField : MonoBehaviour
{
    public float radius = 3f;
    public float pullStrength = 20f;      // force applied per frame
    public float lockDistance = 0.25f;    // when enemies this close get frozen
    public float lifeTime = 3f;       // total duration
    public int damageOnEnter = 1;

    HashSet<GameObject> damaged = new();   // remember who we hurt already
    CircleCollider2D col;

    void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        col.radius = radius;              // visual & trigger radius
        StartCoroutine(KillAfter());
    }

    void FixedUpdate()
    {
        // pull everything with a Rigidbody and Enemy tag/layer inside radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var h in hits)
        {
            if (!h.CompareTag("Enemy")) continue;          // adjust if you use layers

            Rigidbody2D rb = h.attachedRigidbody;
            if (!rb) continue;

            Vector2 toCenter = (Vector2)transform.position - rb.position;

            // 1. Initial damage (once)
            if (!damaged.Contains(h.gameObject))
            {
                h.GetComponent<ChargerEnemy>()?.TakeDamage(damageOnEnter);
                h.GetComponent<EnemyDummy>()?.TakeDamage(damageOnEnter);
                damaged.Add(h.gameObject);
            }

            // 2. Pull force
            if (toCenter.magnitude > lockDistance)
            {
                rb.AddForce(toCenter.normalized * pullStrength * Time.fixedDeltaTime,
                            ForceMode2D.Force);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;       // hold them in place
            }
        }
    }

    IEnumerator KillAfter()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    // gizmo for scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 1, 0.25f);
        Gizmos.DrawSphere(transform.position, radius);
    }
#endif
}
