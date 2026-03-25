using UnityEngine;

public class GameOverManager : MonoBehaviour
{

    [SerializeField] private string mainMenuScene;
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
    
    private void Start()
    {
        AudioManager.instance.PlayFightMusic(backgroundMusic, loopMusic);
        fade.FadeIn();
    }
    public void ReturnMainMenu()
    {
        
        fade.FadeOut();
        Invoke("DelayedReturnMainMenu", 3.0f);
    }

    void DelayedReturnMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuScene);
    }
}
