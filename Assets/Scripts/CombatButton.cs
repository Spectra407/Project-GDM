using UnityEngine;
using UnityEngine.UI;

public class CombatButton : MonoBehaviour
{
    public CombatManager combatManager;
    public string inputID;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(SendInput);
    }

    void SendInput()
    {
        combatManager.HandleInput(inputID);
    }
}
