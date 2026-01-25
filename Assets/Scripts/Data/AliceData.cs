using UnityEngine;

public class AliceData : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public int defense;
    public int maxMadness;
    public int buttons;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHealth = 50;
        maxMadness = 7;    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AliceTakeDamage(int damage)
    {
        if (defense <= damage)
        {
            defense -= damage;
        }
        else
        {
            damage -= defense;
            defense = 0;
            currentHealth -= damage;
        }
    }
}
