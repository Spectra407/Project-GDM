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

    public FadeScript fade;

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

    public void NextSlide() // Triggered by the Next button
    {
        _currentSlide++;
        if (_currentSlide >= slides.Length)
        {
            fade.FadeOut();
            Invoke("DelayedExitTutorial",3.0f); // Signal combat to begin
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

    void DelayedExitTutorial()
    {
        _onComplete?.Invoke();
        tutorialPanel.SetActive(false);
    }
}