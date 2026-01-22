using UnityEngine;

public class AliceData : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public int defense;
    public int buttons;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHealth = 100;
        currentHealth = maxHealth;
        Debug.Log("Current Health " + currentHealth);
        TakeDamage(30);
        Debug.Log("Current Health " + currentHealth);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
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
