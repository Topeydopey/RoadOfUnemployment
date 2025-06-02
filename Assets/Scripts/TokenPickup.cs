// Assets/Scripts/World/TokenPickup.cs
using UnityEngine;

public class TokenPickup : MonoBehaviour
{
    public int value = 1;
    public float spinSpeed = 180f;

    void Update() => transform.Rotate(Vector3.forward * spinSpeed * Time.deltaTime);

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var stats = other.GetComponent<PlayerStats>();
        if (stats == null) return;

        stats.currentTokens = Mathf.Min(stats.currentTokens + value, stats.maxTokens);
        Destroy(gameObject);
    }
}
