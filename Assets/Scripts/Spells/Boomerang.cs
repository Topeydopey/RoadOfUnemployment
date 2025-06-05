using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Boomerang : MonoBehaviour
{
    public float speedOut = 12f;    // outward travel speed
    public float speedReturn = 14f;    // back-home speed
    public float maxRange = 6f;     // how far before returning
    public float spinSpeed = 360f;   // deg/sec
    public int damage = 2;

    Transform owner;          // player, to fly back to
    Vector2 launchDir;
    bool returning;
    float traveled;

    public void Init(Vector2 dir, Transform ownerRef)
    {
        launchDir = dir.normalized;
        owner = ownerRef;
    }

    void Update()
    {
        float step = (returning ? speedReturn : speedOut) * Time.deltaTime;

        if (!returning)
        {
            transform.Translate(launchDir * step, Space.World);
            traveled += step;
            if (traveled >= maxRange)
                returning = true;
        }
        else  // homing
        {
            Vector2 toOwner = (owner.position - transform.position);
            Vector2 move = toOwner.normalized * step;
            transform.Translate(move, Space.World);

            // close enough? destroy and (optionally) refund a charge
            if (toOwner.magnitude < 0.3f)
                Destroy(gameObject);
        }

        // spin for juice
        transform.Rotate(Vector3.forward * -spinSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // don't harm the player or hit twice on return
        if (col.CompareTag("Player")) return;

        col.GetComponent<ChargerEnemy>()?.TakeDamage(damage);
        col.GetComponent<EnemyDummy>()?.TakeDamage(damage);
    }
}
