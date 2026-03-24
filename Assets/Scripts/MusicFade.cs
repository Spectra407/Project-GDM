using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicFade : MonoBehaviour
{

    public IEnumerator MusicFading(bool fadeIn, AudioSource[] sources, float duration, float targetVolume)
    {
        float time = 0f;
        
        // Store the starting volumes of all sources involved
        float[] startVolumes = new float[sources.Length];
        for (int i = 0; i < sources.Length; i++)
        {
            if (sources[i] != null) startVolumes[i] = sources[i].volume;
        }

        while (time < duration)
        {
            time += Time.deltaTime;
            float progress = time / duration;

            for (int i = 0; i < sources.Length; i++)
            {
                if (sources[i] != null)
                {
                    sources[i].volume = Mathf.Lerp(startVolumes[i], targetVolume, progress);
                }
            }
            yield return null;
        }

        // Ensure they hit the exact target volume at the end
        foreach (var s in sources)
        {
            if (s != null) s.volume = targetVolume;
        }
    }
}