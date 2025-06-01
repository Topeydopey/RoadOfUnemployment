// Assets/Scripts/UI/TokenDisplay.cs
using UnityEngine;
using TMPro;

public class TokenDisplay : MonoBehaviour
{
    public PlayerStats stats;
    public TextMeshProUGUI tokenText;

    void Update() => tokenText.text = $"HP/Tokens: {stats.currentTokens}";
}
