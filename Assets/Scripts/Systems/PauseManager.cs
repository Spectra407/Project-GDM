using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private Canvas pauseCanvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseCanvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("Esc key pressed, pausing");
            TogglePaused();
        }
    }

    public void TogglePaused()
    {
        pauseCanvas.enabled = !pauseCanvas.enabled;
    }
}
