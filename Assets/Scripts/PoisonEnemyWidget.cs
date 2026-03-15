using UnityEngine;
using TMPro;

public class PoisonEnemyWidget : MonoBehaviour
{
    [SerializeField] private CombatManager cm;
    [SerializeField] private TMP_Text poisonEnemyText;

    void Update()
    {
        poisonEnemyText.text = $"{cm.poison}";
    }
}