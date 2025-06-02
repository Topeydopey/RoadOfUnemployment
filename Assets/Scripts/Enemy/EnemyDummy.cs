// Assets/Scripts/World/EnemyDummy.cs
using UnityEngine;

public class EnemyDummy : MonoBehaviour
{
    public int maxHP = 3;
    public GameObject tokenPickupPrefab;   // assign in Inspector

    int hp;

    void Start() => hp = maxHP;

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
}
