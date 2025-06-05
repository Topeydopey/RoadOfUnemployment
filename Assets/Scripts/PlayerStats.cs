// Assets/Scripts/Player/PlayerStats.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerStats : MonoBehaviour
{
    /*────────────────────────  CONFIG  ────────────────────────*/
    [Header("♥  /  Tokens")]
    [Min(1)] public int maxTokens = 5;
    [Min(0)] public int currentTokens = 3;

    [Header("Spells (max 3)")]
    public List<SpellSO> spellBar = new();   // runtime copies with charges
    [HideInInspector] public int activeIndex;

    [Header("Input Actions")]
    public InputActionReference cast;        // Button  (Space / South)
    public InputActionReference cycle;       // Value2 (MouseScroll)
    [Header("Visuals")]
    public Animator anim;

    /*────────────────────────  LIFECYCLE  ─────────────────────*/
    void OnEnable()
    {
        cast.action.Enable();
        cycle.action.Enable();
    }
    void OnDisable()
    {
        cast.action.Disable();
        cycle.action.Disable();
    }

    void Update()
    {
        HandleCast();
        HandleCycle();
    }

    /*────────────────────────  PUBLIC API  ────────────────────*/
    public bool PayOneToken()                  // called by SlotMachine
    {
        if (currentTokens <= 0) return false;
        currentTokens--;
        return true;
    }

    public void GiveSpell(SpellSO s)           // called by SlotMachine
    {
        if (spellBar.Count >= 3) spellBar.RemoveAt(0);   // FIFO discard
        spellBar.Add(Instantiate(s));                    // local copy
        activeIndex = spellBar.Count - 1;                // auto-select
    }

    /*────────────────────────  PRIVATE  ───────────────────────*/
    void HandleCast()
    {
        if (!cast.action.WasPressedThisFrame() || spellBar.Count == 0) return;

        SpellSO spell = spellBar[activeIndex];
        if (!spell.castPrefab) return;

        /* ---------- figure out mouse direction ---------- */
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 dir = (mouseWorld - (Vector2)transform.position).normalized;
        Quaternion rot = Quaternion.FromToRotation(Vector3.right, dir);

        /* ---------- decide where to spawn ---------- */
        Vector3 spawnPos;
        if (spell.castPrefab.GetComponent<VortexField>())   // <-- only VortexField spawns at cursor
        {
            spawnPos = mouseWorld;
            rot = Quaternion.identity;                 // no rotation needed for vortex
        }
        else
        {
            spawnPos = transform.position;                  // projectiles spawn on player
        }

        /* ---------- instantiate & pass data ---------- */
        GameObject go = Instantiate(spell.castPrefab, spawnPos, rot);

        go.GetComponent<Boomerang>()?.Init(dir, transform);
        go.GetComponent<BloodBolt>()?.Init(dir);
        // (VortexField ignores these Init calls)

        /* NEW → kick off the cast animation */
        anim.SetTrigger("Cast");

        Destroy(go, spell.prefabLifetime);

        /* ---------- consume a charge ---------- */
        spell.charges--;
        if (spell.charges <= 0)
        {
            spellBar.RemoveAt(activeIndex);
            activeIndex = Mathf.Clamp(activeIndex, 0, spellBar.Count - 1);
        }
    }

    void HandleCycle()
    {
        if (spellBar.Count == 0) return;

        float scrollY = cycle.action.ReadValue<Vector2>().y;
        if (Mathf.Abs(scrollY) < 0.01f) return;

        int dir = scrollY > 0 ? -1 : 1;                 // invert for natural feel
        activeIndex = (activeIndex + dir + spellBar.Count) % spellBar.Count;
    }
    public void TakeDamage(int amount = 1)
    {
        Debug.Log($"TakeDamage() called for {amount}");   //  ← TEMP

        currentTokens = Mathf.Max(0, currentTokens - amount);
        anim.SetTrigger("Hurt");

        if (currentTokens <= 0)
            Debug.Log("Player died — send to Purgatory!");
    }
}
