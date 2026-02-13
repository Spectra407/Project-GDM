using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemies/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Stats")]
    public string enemyName;
    public int maxHealth;

    [Header("Sprite")]
    public Sprite enemySprite;

    [Header("Moves")]
    public EnemyMove[] moves;
}
