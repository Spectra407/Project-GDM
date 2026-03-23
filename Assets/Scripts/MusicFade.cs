using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicFade : MonoBehaviour
{
    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        source.volume = 0f;
    }

    public IEnumerator MusicFading(bool fadeIn, AudioSource source, float duration, float targetVolume)
    {
        // if (!fadeIn)
        // {
        //     double lengthofSource = (double)source.clip.samples / source.clip.frequency;
        //     yield return new WaitForSecondsRealtime((float)(lengthofSource - duration));
        // }

        float time =0f;
        float startVol = source.volume;
        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVol, targetVolume, time / duration);
            yield return null;
        }
    }
}
