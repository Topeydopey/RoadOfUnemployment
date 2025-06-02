// Assets/Scripts/Player/PlayerHealth.cs
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public PlayerStats stats;   // link in Inspector

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.GetComponent<EnemyDummy>())
        {
            stats.currentTokens--;
            if (stats.currentTokens <= 0)
            {
                // TODO: load Purgatory scene here
                Debug.Log("Player died → send to purgatory!");
            }
        }
    }
}
