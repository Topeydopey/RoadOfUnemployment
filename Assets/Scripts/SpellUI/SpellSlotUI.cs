// Assets/Scripts/UI/SpellSlotUI.cs
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellSlotUI : MonoBehaviour
{
    [Header("Refs")]
    public Image           iconImage;     // <- points to the Image on this GO
    public TextMeshProUGUI chargesText;   // <- child Charges text

    public void Show(Sprite icon, int charges, bool selected)
    {
        iconImage.enabled   = true;
        iconImage.sprite    = icon;
        iconImage.color     = selected ? Color.white : new Color(1,1,1,0.35f);

        chargesText.enabled = charges > 1;   // hide “1” for cleanliness
        chargesText.text    = charges.ToString();
        chargesText.color   = selected ? Color.white : new Color(1,1,1,0.55f);
    }

    public void Hide()
    {
        iconImage.enabled   = false;
        chargesText.enabled = false;
    }
}
