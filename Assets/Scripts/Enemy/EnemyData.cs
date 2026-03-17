using System.Collections.Generic;
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

    public string[] moveText;
    
    [Header("Attack Count")]
    public int attackCount; // Set to 2 for cheshire cat
    
    [Header("Gold Reward")]
    public int goldReward;
    
    [Header("Next Scene After Victory")]
    public string nextSceneName;
    
    [Header("Enemy Bombs according to CardID")]
    public List<int> bombCardIDs; // e.g. [3,3,3,4,4,5] for fight 1
}
