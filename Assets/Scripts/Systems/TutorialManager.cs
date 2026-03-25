using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("UI")]
    public GameObject tutorialPanel;
    public Image slideImage;
    public Sprite[] slides; // Assign your all slide sprites
    
    [Header("Scene Transition")]
    [SerializeField] private string nextSceneName = "TitleScreenScene";
    public FadeScript fade;
    
    [Header("Music")]
    public AudioClip backgroundMusic;   // Intro part of soundtrack
    public AudioClip loopMusic; // Loop part of the track
    
    private int _currentSlide = 0;
    private Action _onComplete;

    private void Awake()
    {
        Instance = this;
        if (fade == null)
        {
            fade = FindAnyObjectByType<FadeScript>();
        }
    }

    private void Start()
    {
        AudioManager.instance.PlayFightMusic(backgroundMusic, loopMusic);
        fade.FadeIn();
        
        _currentSlide = 0;
        tutorialPanel.SetActive(true);
        ShowSlide(_currentSlide);
    }

    public void ShowTutorial(Action onComplete)
    {
        fade.FadeIn();
        _onComplete = onComplete;
        _currentSlide = 0;
        tutorialPanel.SetActive(true);
        ShowSlide(_currentSlide);
    }

    private void ShowSlide(int index)
    {
        slideImage.sprite = slides[index];
    }

    public void NextSlide() 
    {
        _currentSlide++;
        if (_currentSlide >= slides.Length)
        {
            if(fade != null) fade.FadeOut();
            StartCoroutine(LoadNextScene());
        }
        else
        {
            ShowSlide(_currentSlide);
        }
    }
    
    public void PreviousSlide()
    {
        _currentSlide = Mathf.Max(0, _currentSlide - 1);
        ShowSlide(_currentSlide);
    }

    private IEnumerator LoadNextScene()
    {
        fade.FadeOut();
        // Wait for the FadeOut duration
        yield return new WaitForSeconds(3.0f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }
}