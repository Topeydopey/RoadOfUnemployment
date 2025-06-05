// Assets/Scripts/Enemies/ChargerEnemy.cs
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ChargerEnemy : MonoBehaviour
{
    /*──────────────────────── CONFIG ────────────────────────*/
    [Header("Stats")]
    public int maxHP = 3;
    public GameObject tokenPickupPrefab;

    [Header("Movement")]
    public float idleSpeed = 1.3f;   // speed while following / wandering
    public float agroRadius = 10f;    // start tracking player inside this
    public float triggerRadius = 6f;   // start charge inside this

    [Header("Charge")]
    public float windUpTime = 0.6f;
    public float chargeSpeed = 7f;
    public float chargeTime = 0.8f;
    public float recoverTime = 0.4f;

    [Header("Wander")]
    public float wanderRadius = 1.5f;  // roam around spawn point
    public float wanderDelay = 2f;    // seconds between wander direction picks

    [Header("FX (optional)")]
    public SpriteRenderer sr;
    public Color windUpColor = Color.red;
    public UnityEngine.Events.UnityEvent onWindUp;
    public UnityEngine.Events.UnityEvent onCharge;

    /*──────────────────────── PRIVATES ───────────────────────*/
    Rigidbody2D rb;
    Transform player;
    Vector2 spawnPos;
    float nextWanderTime;
    int hp;
    Color baseColor;

    enum State { Idle, WindUp, Charge, Recover }
    State state = State.Idle;

    /*──────────────────────── MONO ───────────────────────*/
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spawnPos = transform.position;
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

    /*──────────────────────── IDLE LOGIC ────────────────────*/
    void DoIdle()
    {
        float dist = Vector2.Distance(player.position, transform.position);

        /* 1. Charge if very close */
        if (dist < triggerRadius)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            StartCoroutine(ChargeRoutine(dir));
            return;
        }

        /* 2. Follow player if within agro radius */
        if (dist < agroRadius)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.linearVelocity = dir * idleSpeed;
            return;
        }

        /* 3. Wander around spawn */
        if (Time.time >= nextWanderTime)
        {
            Vector2 wanderTarget = spawnPos + Random.insideUnitCircle * wanderRadius;
            Vector2 dir = (wanderTarget - (Vector2)transform.position).normalized;
            rb.linearVelocity = dir * idleSpeed * 0.6f;  // slower wander
            nextWanderTime = Time.time + wanderDelay;
        }
    }

    /*──────────────────────── CHARGE LOGIC ──────────────────*/
    IEnumerator ChargeRoutine(Vector2 dirToPlayer)
    {
        state = State.WindUp;

        rb.linearVelocity = Vector2.zero;
        if (sr) sr.color = windUpColor;
        onWindUp?.Invoke();
        yield return new WaitForSeconds(windUpTime);

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

        rb.linearVelocity = Vector2.zero;

        state = State.Recover;
        yield return new WaitForSeconds(recoverTime);

        state = State.Idle;
    }

    /*──────────────────────── COMBAT ───────────────────────*/
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

    /*──────────────────────── COLLISIONS ───────────────────*/
    void OnCollisionEnter2D(Collision2D col)
    {
        /* Crash into wall ends dash */
        if (state == State.Charge && col.collider.CompareTag("Wall"))
        {
            rb.linearVelocity = Vector2.zero;
        }

        /* Damage player */
        if (col.collider.CompareTag("Player"))
        {
            col.collider.GetComponent<PlayerStats>()?.TakeDamage(1);
        }
    }
}
