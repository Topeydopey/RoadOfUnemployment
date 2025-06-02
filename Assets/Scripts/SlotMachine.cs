// Assets/Scripts/World/SlotMachine.cs
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[DefaultExecutionOrder(50)]   // run after Player spawns
public class SlotMachine : MonoBehaviour
{
    [Header("Reel")]
    public SpellSO[] reelPool;        // at least 1 element, none null
    [Min(0.1f)] public float spinTime = 1.5f;

    [Header("FX (optional)")]
    public TextMeshProUGUI label;
    public AudioSource kaChing;

    [Header("Input")]
    public InputActionReference interact;   // drag “Interact” (E) here

    bool isSpinning;
    bool playerInside;
    PlayerStats stats;                      // cached player ref

    /* ───────────────────────── SETUP ───────────────────────── */

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

    /* ───────────────────────── INPUT ───────────────────────── */

    void OnInteract(InputAction.CallbackContext _)
    {
        if (playerInside) TrySpin();
    }

    /* ─────────────────── TRIGGER HANDLERS ─────────────────── */

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        playerInside = true;
        stats = col.GetComponent<PlayerStats>();
        Flash("Press E to Spin");
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        playerInside = false;
        stats = null;
        Flash(string.Empty);
    }

    /* ────────────────────── CORE LOGIC ────────────────────── */

    void TrySpin()
    {
        if (isSpinning || stats == null) return;

        if (reelPool == null || reelPool.Length == 0)
        {
            Debug.LogWarning($"{name}: Reel pool is empty!");
            Flash("OUT OF ORDER");
            return;
        }

        // Only pay after we know we can roll
        if (!stats.PayOneToken())
        {
            Flash("NO TOKENS!");
            return;
        }

        StartCoroutine(SpinRoutine());
    }

    IEnumerator SpinRoutine()
    {
        isSpinning = true;
        Flash("SPINNING…");

        yield return new WaitForSeconds(spinTime);

        // Draw a prize until we hit a non-null entry (safety loop)
        SpellSO prize;
        int safety = 20;
        do
        {
            prize = reelPool[Random.Range(0, reelPool.Length)];
        } while (prize == null && --safety > 0);

        if (prize)
        {
            stats.GiveSpell(prize);
            Flash($"WON: {prize.spellName}");
            kaChing?.Play();
        }
        else
        {
            Debug.LogWarning($"{name}: All reel entries were null!");
            Flash("JACKPOT ERROR");
            // optionally refund the token here
        }

        isSpinning = false;
    }

    /* ────────────────────── HELPERS ───────────────────────── */

    void Flash(string msg)
    {
        if (label) label.text = msg;
    }
}
