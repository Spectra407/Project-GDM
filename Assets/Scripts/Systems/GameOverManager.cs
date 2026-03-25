using UnityEngine;

public class GameOverManager : MonoBehaviour
{

    [SerializeField] private string mainMenuScene;
    public FadeScript fade;

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
