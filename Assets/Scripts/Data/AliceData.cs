using UnityEngine;

[CreateAssetMenu(fileName = "NewAliceData", menuName = "Combat/Alice Data")]
public class AliceData : ScriptableObject
{
    [Header("Health Stats")]
    public int maxHealth = 50;
    public int currentHealth;   // This is what the EnemyTurnState updates

    [Header("Madness Mechanics")]
    // The "Bust" limit (e.g., set this to 7 in the Inspector)
    public int maxMadness = 7; 

    // Optional: Add a starting madness value if she begins battles corrupted
    public int startingMadness = 0; 
}