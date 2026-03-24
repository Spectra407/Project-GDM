using UnityEngine;

public class InfoPanelController : MonoBehaviour
{
    [SerializeField] private GameObject infoPanel;

    // Call this from the Help Button
    public void OpenPanel()
    {
        infoPanel.SetActive(true);
        
    }

    // Call this from the X Button
    public void ClosePanel()
    {
        infoPanel.SetActive(false);
        
    }
}