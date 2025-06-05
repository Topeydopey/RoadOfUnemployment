using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class BloodBolt : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 10f;
    public float maxLife = 3f;          // failsafe cleanup

    [Header("Damage")]
    public int damage = 1;

    Animator anim;
    Vector2 dir;
    bool hasHit;

    /* Called right after Instantiate() */
    public void Init(Vector2 direction) => dir = direction.normalized;

    void Awake()
    {
        anim = GetComponent<Animator>();
        Destroy(gameObject, maxLife);   // in case nothing is hit
    }

    void Update()
    {
        if (!hasHit)                    // stop moving once we splash
            transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (hasHit) return;            // already splashed

        // optional: ignore player & other projectiles
        if (col.CompareTag("Player") || col.CompareTag("Projectile")) return;

        // deal damage if target has a TakeDamage(int) method
        col.GetComponent<ChargerEnemy>()?.TakeDamage(damage);
        col.GetComponent<EnemyDummy>()?.TakeDamage(damage);

        // play splash animation
        hasHit = true;
        anim.SetTrigger("Hit");         // Animator handles Fly→Splash
        Destroy(gameObject, 0.25f);     // length of Splash clip
    }
}
