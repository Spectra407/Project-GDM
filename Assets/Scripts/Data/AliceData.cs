using UnityEngine;

[CreateAssetMenu(fileName = "NewAliceData", menuName = "Combat/Alice Data")]
public class AliceData : ScriptableObject
{
    [Header("Health Stats")]
    public int maxHealth = 50;
    public int currentHealth;   // This is what the EnemyTurnState updates

    [Header("Madness")]
    // The "Bust" limit (We can adjust this in the Inspector for balance)
    public int maxMadness = 7; 
    public int startingMadness = 0;

    [Header("Cash money (buttons)")] public int buttons;
    
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    // Ended up moving this into the CombatManager itself in the EnemyTurnState!
    // public void AliceTakeDamage(int damage)
    // {
    //     if (defense <= damage)
    //     {
    //         defense -= damage;
    //     }
    //     else
    //     {
    //         damage -= defense;
    //         defense = 0;
    //         currentHealth -= damage;
    //     }
    // }
}