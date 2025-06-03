// Assets/Scripts/UI/SlotToast.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SlotToast : MonoBehaviour
{
    public CanvasGroup group;
    public TextMeshProUGUI label;
    public Image iconImage;

    void Awake() => gameObject.SetActive(false);

    public void Show(string msg, Sprite icon = null, float life = 2f)
    {
        label.text = msg;
        iconImage.enabled = icon;
        if (icon) iconImage.sprite = icon;

        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(Fade(life));
    }

    System.Collections.IEnumerator Fade(float life)
    {
        group.alpha = 1;
        yield return new WaitForSeconds(life);

        float t = 0;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            group.alpha = 1 - t / 0.3f;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
