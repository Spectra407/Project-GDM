using UnityEngine;
using System.Collections;
using TMPro;

public class BannerManager : MonoBehaviour
{
    public static BannerManager Instance;

    [SerializeField] private CanvasGroup bannerCanvasGroup;
    [SerializeField] private TMP_Text bannerText;

    private void Awake() => Instance = this;

    public void ShowBanner(string message, float displayTime = 1.2f)
    {
        StartCoroutine(AnimateBanner(message, displayTime));
    }

    private IEnumerator AnimateBanner(string message, float displayTime)
    {
        bannerText.text = message;
        bannerCanvasGroup.alpha = 0f;

        // Fade in
        float t = 0f;
        while (t < 0.3f) {
            t += Time.deltaTime;
            bannerCanvasGroup.alpha = t / 0.3f;
            yield return null;
        }

        bannerCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(displayTime);

        // Fade out
        t = 0f;
        while (t < 0.3f) {
            t += Time.deltaTime;
            bannerCanvasGroup.alpha = 1f - (t / 0.3f);
            yield return null;
        }

        bannerCanvasGroup.alpha = 0f;
    }
}