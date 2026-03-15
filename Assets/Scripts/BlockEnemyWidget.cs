using TMPro;
using UnityEngine;

public class BlockEnemyWidget : MonoBehaviour
{
    [SerializeField] private CombatManager cm;
    [SerializeField] private TMP_Text blockEnemyText;

    void Update()
    {
        blockEnemyText.text = $"{cm.enemyDefense}";
    }
}
