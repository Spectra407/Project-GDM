using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsSceneController : MonoBehaviour
{
    public GameObject[] slides;
    public float secsPerSlide;
    public float fadeTime;
    public float pauseTime;

    private float period;
    private float time;
    
    public FadeScript fade;
    [Header("Music")]
    public AudioClip backgroundMusic;   // Intro part of soundtrack
    public AudioClip loopMusic; // Loop part of the track
    
    private void Awake()
    {
        if (fade == null)
        {
            fade = FindAnyObjectByType<FadeScript>();
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.instance.PlayFightMusic(backgroundMusic, loopMusic);
        fade.FadeIn();
        period = secsPerSlide + fadeTime + pauseTime;
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        int slide = (int) (time / period);
        float phase = time % period;

        if (slide >= slides.Length)
        {
            fade.FadeOut();
            Invoke("DelayedLeaveScene",1.5f);
        }
        else
        {
            for (int i = 0; i < slides.Length; i++)
            {
                slides[i].SetActive(false);
            }

            float alpha = Math.Min((phase - pauseTime / 2) / (fadeTime / 2),
                (period - phase - pauseTime / 2) / (fadeTime / 2));
            if (alpha < 0) alpha = 0;
            if (alpha > 1) alpha = 1;

            GameObject current = slides[slide];
            current.GetComponent<CanvasGroup>().alpha = alpha;
            current.SetActive(true);
        }

        
    }
    
    void DelayedLeaveScene()
    {
        SceneManager.LoadScene("TitleScreenScene");
    }
}
