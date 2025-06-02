using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    public float life = 2f;

    void Start() => Destroy(gameObject, life);

    void Update()
        => transform.Translate(Vector2.right * speed * Time.deltaTime); // assumes prefab faces +X

    void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.GetComponent<EnemyDummy>();
        if (enemy)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
