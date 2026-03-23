using System.Collections;
using UnityEngine;

public class FadeScript : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private float fadeDuration = 5.0f;

    [SerializeField] private bool fadeIn = false;

    [SerializeField] private AudioSource source;
    [SerializeField] private MusicFade fading;

    private void Start()
    {
        if (fadeIn)
        {
            FadeIn();
        }
        else
        {
            FadeOut();
        }
    }


    public void FadeIn()
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0, fadeDuration));
        StartCoroutine(fading.MusicFading(true, source, fadeDuration, 0.5f));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1, fadeDuration));
        StartCoroutine(fading.MusicFading(false, source, fadeDuration, 0f));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            yield return null;
        }
        cg.alpha = end;
    }
}
