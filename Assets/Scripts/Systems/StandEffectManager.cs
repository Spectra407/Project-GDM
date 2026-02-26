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
            UpdateStrength(cm.dem.PendingStats[CardData.CardType.Strength]);
            UpdatePoison(cm.dem.PendingStats[CardData.CardType.Poison]);
            UpdateDefense(cm.dem.PendingStats[CardData.CardType.Defense]);
            UpdateDamage(cm.dem.PendingStats[CardData.CardType.Damage] + cm.dem.AttackBonus + cm.poison);
            DecayPoison();
            Debug.Log("Gained " + cm.dem.PendingStats[CardData.CardType.Defense] + " defense, " 
            + cm.dem.PendingStats[CardData.CardType.Strength] + " strength, and " 
            + cm.dem.PendingStats[CardData.CardType.Poison] + " poison. Dealt " 
            + (cm.dem.PendingStats[CardData.CardType.Damage] + cm.dem.AttackBonus + cm.poison) + " damage. Poison decayed to " 
            + cm.poison + ".");
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
