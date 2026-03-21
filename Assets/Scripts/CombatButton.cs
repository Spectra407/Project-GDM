using UnityEngine;
using UnityEngine.UI;

public class CombatButton : MonoBehaviour
{
    public CombatManager combatManager;
    public string inputID;
    
    private static CombatButton _drawButton;
    private static CombatButton _standButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(SendInput);
        
        // Register this as the draw button so we can grey it out
        if (inputID == "HitButton")
            _drawButton = this;
        else if (inputID == "StandButton")
            _standButton = this;
    }

    void SendInput()
    {
        combatManager.HandleInput(inputID);
    }
    
    public static void SetDrawInteractable(bool interactable)
    {
        if (_drawButton == null) return;
        _drawButton.GetComponent<Button>().interactable = interactable;
    }
    
    public static void SetStandInteractable(bool interactable)
    {
        if (_standButton == null) return;
        _standButton.GetComponent<Button>().interactable = interactable;
    }
}
