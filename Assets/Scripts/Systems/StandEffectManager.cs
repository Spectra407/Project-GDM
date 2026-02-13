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
            (cm.dem.PendingDamage + cm.dem.AttackBonus + cm.poison) + "damage. Poison decayed to " + cm.poison + ".");
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
        private void UpdateDamage(int damage) //deal [damage] damage to the enemy, checks if it is dead
        {
            cm.enemyHealth = Math.Max(0, cm.enemyHealth - damage);
            if (cm.enemyHealth == 0)
            {
                Debug.Log("defeated enemy");
                //move onto new phase or something
            }
        }
        private void DecayPoison() //halves poison, rounded down
        {
            cm.poison = (int) Math.Floor(cm.poison/2.0);
        }
    }
}
