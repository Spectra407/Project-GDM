using UnityEngine;
using System;

namespace Systems
{
    public class StandEffectManager : MonoBehaviour
    {
        public CombatManager cm;
        public void ResolveOnStand()
        {
            int handCount = cm.Hand.handCardViews.Count;
            
            // Calculate bomb strength buff to give to enemy
            int bombStrength = 0;
            foreach (var cardView in cm.Hand.handCardViews)
            {
                if (cardView?.data != null && cardView.data.cardType.Contains(CardData.CardType.Bomb))
                    bombStrength += cardView.data.strength;
            }
            if (bombStrength > 0)
            {
                cm.enemyStrength += bombStrength;
                Debug.Log($"Bomb cards gave {bombStrength} strength to the enemy!");
            }
            
            int equalDamage = cm.dem.GetEqualBonus(CardData.CardType.Damage);
            int equalDefense = cm.dem.GetEqualBonus(CardData.CardType.Defense);
            int equalStrength = cm.dem.GetEqualBonus(CardData.CardType.Strength);
            int equalPoison = cm.dem.GetEqualBonus(CardData.CardType.Poison);

            UpdateStrength((cm.dem.PendingStats[CardData.CardType.Strength] + equalStrength
                            + cm.dem.PerStats[CardData.CardType.Strength] * handCount)
                           * cm.dem.MultStats[CardData.CardType.Strength]);
            UpdatePoison((cm.dem.PendingStats[CardData.CardType.Poison] + equalPoison
                          + cm.dem.PerStats[CardData.CardType.Poison] * handCount)
                         * cm.dem.MultStats[CardData.CardType.Poison]);
            UpdateDefense((cm.dem.PendingStats[CardData.CardType.Defense] + equalDefense
                           + cm.dem.PerStats[CardData.CardType.Defense] * handCount)
                          * cm.dem.MultStats[CardData.CardType.Defense]);

            int totalDamage = ((cm.dem.PendingStats[CardData.CardType.Damage]  + equalDamage
                                + cm.dem.PerStats[CardData.CardType.Damage] * handCount)
                               * cm.dem.MultStats[CardData.CardType.Damage]
                               + cm.dem.AttackBonus)
                              + cm.poison;

            UpdateDamage(totalDamage);
            
            // Play animations of portraits attacking
            PortraitAnimator.Instance.PlayAliceAttack();
            PortraitAnimator.Instance.PlayEnemyHit();

            Debug.Log("Gained " + cm.tempDefense + " defense, " 
                      + cm.strength + " strength, and " 
                      + cm.poison + " poison. Dealt " 
                      + totalDamage + " damage.");

            DecayPoison();
            Debug.Log("Poison decayed to " + cm.poison + ".");
            cm.dem.ResetForStand();
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
            if (damage > 0) cm.OnTakeDamage.Invoke();   // Invoke SFX deal damage
        }
        private void DecayPoison() //halves poison, rounded down
        {
            cm.poison = (int) Math.Floor(cm.poison/2.0);
        }
    }
}
