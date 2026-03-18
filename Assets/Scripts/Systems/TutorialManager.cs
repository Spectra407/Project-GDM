using UnityEngine;
using UnityEngine.UI;
using System;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("UI")]
    public GameObject tutorialPanel;
    public Image slideImage;
    public Sprite[] slides; // Assign your all slide sprites

    private int _currentSlide = 0;
    private Action _onComplete;

    private void Awake() => Instance = this;

    public void ShowTutorial(Action onComplete)
    {
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
            tutorialPanel.SetActive(false);
            _onComplete?.Invoke(); // Signal combat to begin
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
}