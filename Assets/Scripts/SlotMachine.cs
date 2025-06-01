// Assets/Scripts/World/SlotMachine.cs
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class SlotMachine : MonoBehaviour
{
    [Header("Reel")]
    public SpellSO[] reelPool;
    public float spinTime = 1.5f;

    [Header("Feedback (optional)")]
    public TextMeshProUGUI label;
    public AudioSource kaChing;

    [Header("Input")]
    public InputActionReference interact;   // <- drag the “Interact” action here in Inspector

    bool isSpinning;
    bool playerInside;

    PlayerStats cachedStats;               // remember who’s inside

    void OnEnable() => interact.action.Enable();
    void OnDisable() => interact.action.Disable();

    void Update()
    {
        // Only listen for input while a player is in range
        if (playerInside && interact.action.WasPressedThisFrame())
        {
            TrySpin();
        }
    }

    /* ---------- trigger detection ---------- */
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = true;
        cachedStats = other.GetComponent<PlayerStats>();
        Flash("Press E to Spin");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;
        cachedStats = null;
        Flash("");                          // clear hint
    }

    /* ---------- core logic ---------- */
    void TrySpin()
    {
        if (isSpinning || cachedStats == null) return;

        if (cachedStats.PayOneToken())
        {
            StartCoroutine(Spin());
        }
        else
        {
            Flash("NO TOKENS!");
        }
    }

    System.Collections.IEnumerator Spin()
    {
        isSpinning = true;
        Flash("SPINNING…");
        yield return new WaitForSeconds(spinTime);

        var prize = reelPool[Random.Range(0, reelPool.Length)];
        cachedStats.GiveSpell(prize);

        Flash($"WON: {prize.spellName}");
        kaChing?.Play();
        isSpinning = false;
    }

    void Flash(string txt) { if (label) label.text = txt; }
}
