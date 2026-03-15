using TMPro;
using UnityEngine;

public class StrengthAliceWidget : MonoBehaviour
{
    [SerializeField] private CombatManager cm;
    [SerializeField] private TMP_Text strengthAliceText;

    void Update()
    {
        strengthAliceText.text = $"{cm.strength}";
    }
}
