// Assets/Scripts/UI/SpellBarDisplay.cs
using UnityEngine;

public class SpellBarDisplay : MonoBehaviour
{
    public PlayerStats stats;        // drag your Player GameObject
    public SpellSlotUI[] slots;        // size 3 → drag Slot0/1/2 here

    void Update()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < stats.spellBar.Count)
            {
                var spell = stats.spellBar[i];
                slots[i].Show(spell.icon, spell.charges, i == stats.activeIndex);
            }
            else
            {
                slots[i].Hide();
            }
        }
    }
}
