// Assets/Scripts/Enemies/ChargerEnemy.cs
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ChargerEnemy : MonoBehaviour
{
    public int maxHP = 3;
    public GameObject tokenPickupPrefab;      // drop on death

    [Header("Behaviour")]
    public float idleSpeed = 1.3f;
    public float triggerRadius = 6f;
    public float windUpTime = 0.6f;
    public float chargeSpeed = 7f;
    public float chargeTime = 0.8f;
    public float recoverTime = 0.4f;

    [Header("FX (optional)")]
    public SpriteRenderer sr;                 // drag your sprite-renderer
    public Color windUpColor = Color.red;
    public UnityEngine.Events.UnityEvent onWindUp;
    public UnityEngine.Events.UnityEvent onCharge;

    Rigidbody2D rb;
    Transform player;
    int hp;
    Color baseColor;

    enum State { Idle, WindUp, Charge, Recover }
    State state = State.Idle;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        hp = maxHP;
        if (sr) baseColor = sr.color;
    }

    void Update()
    {
        switch (state)
        {
            case State.Idle: DoIdle(); break;
            case State.Charge: /* handled in coroutine */ break;
        }
    }

    /*────────────────────────  STATES  ────────────────────────*/
    void DoIdle()
    {
        // 1. Basic slow follow so it feels alive
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * idleSpeed;

        // 2. If close enough, start charge routine
        if (Vector2.Distance(player.position, transform.position) < triggerRadius)
        {
            StartCoroutine(ChargeRoutine(dir));
        }
    }

    IEnumerator ChargeRoutine(Vector2 dirToPlayer)
    {
        state = State.WindUp;

        // WIND-UP  (show telegraph)
        rb.linearVelocity = Vector2.zero;
        if (sr) sr.color = windUpColor;
        onWindUp?.Invoke();
        yield return new WaitForSeconds(windUpTime);

        // CHARGE  (straight dash)
        state = State.Charge;
        if (sr) sr.color = baseColor;
        onCharge?.Invoke();
        rb.linearVelocity = dirToPlayer.normalized * chargeSpeed;

        float t = 0;
        while (t < chargeTime)
        {
            t += Time.deltaTime;
            yield return null;
        }

        // Stop moving
        rb.linearVelocity = Vector2.zero;

        // RECOVER
        state = State.Recover;
        yield return new WaitForSeconds(recoverTime);

        state = State.Idle;
    }

    /*──────────────────────  COMBAT  ─────────────────────────*/
    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0) Die();
    }

    void Die()
    {
        Instantiate(tokenPickupPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        /* 1. Crash into wall ends dash (kept as-is) */
        if (state == State.Charge && col.collider.CompareTag("Wall"))
        {
            rb.linearVelocity = Vector2.zero;
        }

        /* 2. Normal contact damage to player */
        if (col.collider.CompareTag("Player"))
        {
            var stats = col.collider.GetComponent<PlayerStats>();
            if (stats) stats.TakeDamage(1);        //  ← NEW
        }
    }
}
