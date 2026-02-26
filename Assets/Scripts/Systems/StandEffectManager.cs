using UnityEngine;
using System;

namespace Systems
{
    public class StandEffectManager : MonoBehaviour
    {
        public CombatManager cm;
        public void ResolveOnStand()
        {
            //deal with any special effects (i dont think there are any really)
            UpdateStrength(cm.dem.PendingStrength);
            UpdatePoison(cm.dem.PendingPoison);
            UpdateDefense(cm.dem.PendingDefense);
            UpdateDamage(cm.dem.PendingDamage + cm.dem.AttackBonus + cm.poison);
            DecayPoison();
            Debug.Log("Gained " + cm.dem.PendingDefense + " defense, " + cm.dem.PendingStrength + " strength, and " + cm.dem.PendingPoison + " poison. Dealt " + 
            (cm.dem.PendingDamage + cm.dem.AttackBonus + cm.poison) + " damage. Poison decayed to " + cm.poison + ".");
            cm.dem.Reset();
        }
        private void UpdateStrength(int strength)
        {
            cm.strength += strength;
            if (cm.strength < 0)
            {
                cm.strength = 0;
            }
        }
        private void UpdatePoison(int poison)
        {
            cm.poison += poison;
            if (cm.poison < 0)
            {
                cm.poison = 0;
            }
        }
        private void UpdateDefense(int defense)
        {
            cm.tempDefense += defense;
            if (cm.tempDefense < 0)
            {
                cm.tempDefense = 0;
            }
        }
        private void UpdateDamage(int damage) //deal [damage] damage to the enemy
        {
            if (cm.enemyDefense >= damage)
            {
                // Defense big enough to tank full hit
                cm.enemyDefense -= damage;
            }
            else
            {
                // Defense not big enough to tank full hit
                damage -= cm.enemyDefense;
                cm.enemyDefense = 0;
                cm.enemyCurrentHealth -= damage;
            }
            // Stop negative health values
            cm.enemyCurrentHealth = Mathf.Max(0, cm.enemyCurrentHealth);
            Debug.Log("Enemy health: " + cm.enemyCurrentHealth);
        }
        private void DecayPoison() //halves poison, rounded down
        {
            cm.poison = (int) Math.Floor(cm.poison/2.0);
        }
    }
}
