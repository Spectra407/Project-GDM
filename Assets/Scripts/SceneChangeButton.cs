using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SceneChangeButton : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    private MainMenuManager mainMenuManager;
    
    private void Awake()
    {
        if (mainMenuManager == null)
        {
            mainMenuManager = FindAnyObjectByType<MainMenuManager>();
        }
    }
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            StartCoroutine(LoadSceneWithFade());
        });
    }
    
    private IEnumerator LoadSceneWithFade()
    {
        mainMenuManager.fade.FadeOut();
        
        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(nextSceneName);
    }
}
