using System.Collections;
using UnityEngine;

public class FadeScript : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 5.0f;
    
    [Header("Settings")]
    [SerializeField] private bool fadeInOnStart = true;

    private void Start()
    {
        // Setup initial state based on toggle
        if (fadeInOnStart)
        {
            canvasGroup.alpha = 1f;
            AudioManager.instance.SetFadeMultiplier(0f);
            FadeIn();
        }
    }

    public void FadeIn()
    {
        Debug.Log("Fading In");
        StopAllCoroutines();
        // Fade UI from 1 to 0 
        StartCoroutine(FadeCanvasGroup(0f, fadeDuration));
        // Fade Music from 0 to 1
        StartCoroutine(MusicSystemFade(0f, 1f));
    }

    public void FadeOut()
    {
        Debug.Log("Fading Out");
        StopAllCoroutines();
        // Fade UI from 0 to 1
        StartCoroutine(FadeCanvasGroup(1f, fadeDuration));
        // Fade Music from 1 to 0
        StartCoroutine(MusicSystemFade(1f, 0f));
    }

    private IEnumerator MusicSystemFade(float start, float end)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentMultiplier = Mathf.Lerp(start, end, elapsedTime / fadeDuration);
            
            AudioManager.instance.SetFadeMultiplier(currentMultiplier);
            yield return null;
        }
        AudioManager.instance.SetFadeMultiplier(end);
    }

    private IEnumerator FadeCanvasGroup(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }
}