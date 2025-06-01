// Assets/Scripts/Spells/SpellSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpell", menuName = "Slots/Spell")]
public class SpellSO : ScriptableObject
{
    public string spellName;
    public Sprite icon;
    public int charges = 3;

    // quick prototype effect
    public GameObject castPrefab;   // e.g. small explosion
    public float prefabLifetime = 2f;
}
