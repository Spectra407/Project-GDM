using TMPro;
using UnityEngine;

public class StrengthEnemyWidget : MonoBehaviour
{
    [SerializeField] private CombatManager cm;
    [SerializeField] private TMP_Text strengthEnemyText;

    void Update()
    {
        strengthEnemyText.text = $"{cm.enemyStrength}";
    }
}
