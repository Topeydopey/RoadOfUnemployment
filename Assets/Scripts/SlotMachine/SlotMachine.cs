// Assets/Scripts/World/SlotMachine.cs
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Animator))]
public class SlotMachine : MonoBehaviour
{
    [Header("Reel Settings")]
    public SpellSO[] reelPool;
    [Min(0.1f)] public float spinTime = 1.5f;      // if you don’t use an Animation Event

    [Header("Audio")]
    public AudioSource spinLoop;                    // looping “reel blur”
    public AudioSource kaChing;                    // one-shot “ding!”

    [Header("UI")]
    public TextMeshProUGUI promptText;              // “Press E to Spin”
    public TextMeshProUGUI resultText;              // “SPINNING… / WON: ___”

    [Header("Input")]
    public InputActionReference interact;           // bind to your “E” action

    /* ───────────────────────── locals ───────────────────────── */
    Animator anim;
    bool isSpinning;
    bool playerInside;
    PlayerStats currentStats;

    void Awake()
    {
        anim = GetComponent<Animator>();
        if (promptText) promptText.enabled = false;
        if (resultText) resultText.enabled = false;
    }

    /* ───────────── INPUT HOOKUP ───────────── */
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

    /* ───────────── TRIGGERS ───────────── */
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        playerInside = true;
        currentStats = col.GetComponent<PlayerStats>();
        SetPrompt("Press E to Spin");
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        playerInside = false;
        currentStats = null;
        SetPrompt(string.Empty);
    }

    /* ───────────── CORE ───────────── */
    void TrySpin()
    {
        if (isSpinning || currentStats == null) return;

        if (!currentStats.PayOneToken())
        {
            ShowResult("NO TOKENS!", 1.5f);
            return;
        }

        StartCoroutine(SpinRoutine(currentStats));
    }

    IEnumerator SpinRoutine(PlayerStats target)
    {
        isSpinning = true;
        SetPrompt(string.Empty);                 // hide hint while spinning
        ShowResult("SPINNING…", spinTime);       // stays visible during spin

        anim.SetTrigger("Spin");
        spinLoop?.Play();

        yield return new WaitForSeconds(spinTime);   // or wait for Animation Event

        spinLoop?.Stop();

        if (target)                                  // player might have died
        {
            var prize = reelPool[Random.Range(0, reelPool.Length)];
            target.GiveSpell(prize);
            ShowResult($"WON: {prize.spellName}", 2f);
        }

        isSpinning = false;
        if (playerInside) SetPrompt("Press E to Spin");
    }

    /* ───────────── UI HELPERS ───────────── */
    void SetPrompt(string msg)
    {
        if (!promptText) return;
        promptText.text = msg;
        promptText.enabled = !string.IsNullOrEmpty(msg);
    }

    void ShowResult(string msg, float life)
    {
        if (!resultText) return;
        resultText.text = msg;
        resultText.enabled = true;
        resultText.color = new Color(resultText.color.r, resultText.color.g,
                                       resultText.color.b, 1);
        StopCoroutine(nameof(FadeOut));
        StartCoroutine(FadeOut(life));
    }

    IEnumerator FadeOut(float life)
    {
        yield return new WaitForSeconds(life);

        float t = 2, dur = 0.3f;
        Color c = resultText.color;
        while (t < dur)
        {
            t += Time.deltaTime;
            resultText.color = new Color(c.r, c.g, c.b, 1 - t / dur);
            yield return null;
        }
        resultText.enabled = false;
    }
}
