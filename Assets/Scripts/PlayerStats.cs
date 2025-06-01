using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour
{
    [Header("♥ / Tokens")]
    public int maxTokens = 5;
    public int currentTokens = 3;

    [Header("Spell bar (max 3)")]
    public List<SpellSO> spellBar = new();
    public int activeIndex;

    [Header("Input")]
    public InputActionReference cast;
    public InputActionReference cycle;   // expects Vector2 (scroll)

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
        // 1. Cast
        if (cast.action.WasPressedThisFrame() && spellBar.Count > 0)
        {
            Cast(spellBar[activeIndex]);
            spellBar[activeIndex].charges--;
            if (spellBar[activeIndex].charges <= 0)
                spellBar.RemoveAt(activeIndex);
        }

        // 2. Cycle bar
        float scrollY = cycle.action.ReadValue<Vector2>().y;
        if (Mathf.Abs(scrollY) > 0.01f && spellBar.Count > 0)
        {
            int dir = scrollY > 0 ? -1 : 1;           // invert for natural feel
            activeIndex = (activeIndex + dir + spellBar.Count) % spellBar.Count;
        }
    }

    public bool PayOneToken()          // called by slot machine
    {
        if (currentTokens <= 0) return false;
        currentTokens--;
        return true;
    }

    public void GiveSpell(SpellSO s)   // called by slot machine
    {
        if (spellBar.Count >= 3) spellBar.RemoveAt(0);
        spellBar.Add(Instantiate(s));  // own local copy for charges
        activeIndex = spellBar.Count - 1;
    }

    void Cast(SpellSO s)
    {
        if (!s.castPrefab) return;
        var go = Instantiate(s.castPrefab, transform.position, Quaternion.identity);
        Destroy(go, s.prefabLifetime);
    }
}
