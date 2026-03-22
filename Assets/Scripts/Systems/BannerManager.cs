using UnityEngine;
using System.Collections;
using TMPro;

public class BannerManager : MonoBehaviour
{
    public static BannerManager Instance;

    [SerializeField] private CanvasGroup bannerCanvasGroup;
    [SerializeField] private TMP_Text bannerText;
    
    private Coroutine _currentBanner;

    private void Awake() => Instance = this;

    public void ShowBanner(string message, float displayTime = 1.2f)
    {
        if (_currentBanner != null) StopCoroutine(_currentBanner);
        _currentBanner = StartCoroutine(AnimateBanner(message, displayTime));
    }

    public void ShowBannerPersistent(string message)
    {
        if (_currentBanner != null) StopCoroutine(_currentBanner);
        _currentBanner = StartCoroutine(FadeInOnly(message));
    }

    public void HideBanner()
    {
        if (_currentBanner != null) StopCoroutine(_currentBanner);
        _currentBanner = StartCoroutine(FadeOutOnly());
    }

    private IEnumerator FadeInOnly(string message)
    {
        bannerText.text = message;
        bannerCanvasGroup.alpha = 0f;

        float t = 0f;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            bannerCanvasGroup.alpha = t / 0.3f;
            yield return null;
        }
        bannerCanvasGroup.alpha = 1f;
        // stays visible indefinitely until HideBanner() is called
    }

    private IEnumerator FadeOutOnly()
    {
        float startAlpha = bannerCanvasGroup.alpha;
        float t = 0f;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            bannerCanvasGroup.alpha = startAlpha * (1f - (t / 0.3f));
            yield return null;
        }
        bannerCanvasGroup.alpha = 0f;
    }

    private IEnumerator AnimateBanner(string message, float displayTime)
    {
        bannerText.text = message;
        bannerCanvasGroup.alpha = 0f;

        float t = 0f;
        while (t < 0.3f) {
            t += Time.deltaTime;
            bannerCanvasGroup.alpha = t / 0.3f;
            yield return null;
        }

        bannerCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(displayTime);

        t = 0f;
        while (t < 0.3f) {
            t += Time.deltaTime;
            bannerCanvasGroup.alpha = 1f - (t / 0.3f);
            yield return null;
        }

        bannerCanvasGroup.alpha = 0f;
    }
}