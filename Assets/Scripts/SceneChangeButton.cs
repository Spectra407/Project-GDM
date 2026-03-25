using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SceneChangeButton : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene(nextSceneName));
    }
}
