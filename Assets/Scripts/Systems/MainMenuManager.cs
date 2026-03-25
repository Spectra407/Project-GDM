using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
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
    }
}
