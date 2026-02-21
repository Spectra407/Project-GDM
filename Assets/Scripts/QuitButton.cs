using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    public void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(Quit);
    }

    public void Quit()
    {
        Debug.Log("Quitting D: (will actually quit in build of the game)");
        Application.Quit();
    }
}
