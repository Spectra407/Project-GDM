using TMPro;
using UnityEngine;

public class BlockAliceWidget : MonoBehaviour
{
    [SerializeField] private CombatManager cm;
    [SerializeField] private TMP_Text blockAliceText;

    void Update()
    {
        blockAliceText.text = $"{cm.tempDefense}";
    }
}
