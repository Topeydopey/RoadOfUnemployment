// Assets/Scripts/World/SlotMachine.cs
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class SlotMachine : MonoBehaviour
{
    [Header("Reel")]
    public SpellSO[] reelPool;
    [Min(0.1f)] public float spinTime = 1.5f;

    [Header("FX")]
    public TextMeshProUGUI label;
    public AudioSource kaChing;

    [Header("Input")]
    public InputActionReference interact;      // “Interact” action (E)

    bool   isSpinning;
    bool   playerInside;
    PlayerStats currentStats;                  // who’s standing inside

    /* ───────────────────────── INPUT ───────────────────────── */
    void OnEnable()
    {
        interact.action.Enable();
        interact.action.performed += OnInteract;
    }
    void OnDisable()
    {
        interact.action.performed -= OnInteract;
        interact.action.Disable();
    }
    void OnInteract(InputAction.CallbackContext _) { if (playerInside) TrySpin(); }

    /* ─────────────────── TRIGGER HANDLERS ─────────────────── */
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        playerInside  = true;
        currentStats  = col.GetComponent<PlayerStats>();
        Flash("Press E to Spin");
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        playerInside = false;
        currentStats = null;
        Flash(string.Empty);
    }

    /* ────────────────────── CORE LOGIC ────────────────────── */
    void TrySpin()
    {
        if (isSpinning || currentStats == null) return;

        if (!currentStats.PayOneToken()) { Flash("NO TOKENS!"); return; }

        // hand off a snapshot of the player to the coroutine
        StartCoroutine(SpinRoutine(currentStats));
    }

    IEnumerator SpinRoutine(PlayerStats target)
    {
        isSpinning = true;
        Flash("SPINNING…");

        yield return new WaitForSeconds(spinTime);

        SpellSO prize = reelPool[Random.Range(0, reelPool.Length)];
        if (prize && target)                       // player could have died
        {
            target.GiveSpell(prize);
            Flash($"WON: {prize.spellName}");
            kaChing?.Play();
        }
        else
        {
            // Refund if something went wrong
            if (target) target.currentTokens++;
            Flash("MALFUNCTION");
        }

        isSpinning = false;
    }

    /* ────────────────────── HELPERS ───────────────────────── */
    void Flash(string msg) { if (label) label.text = msg; }
}
