using UnityEngine;

namespace Systems
{
    public class StandEffectManager : MonoBehaviour
    {
        public CombatManager cm;
        public DrawEffectManager dem;
        public void ResolveOnStand(CardData cardData)
        {
            //deal with any special effects (i dont think there are any really)
            UpdateStrength(dem.PendingStrength);
            UpdatePoison(dem.PendingPoison);
            UpdateDefense(dem.PendingDefense);
            UpdateDamage(dem.PendingDamage + dem.AttackBonus + dem.Poison);
            DecayPoison();
            dem.Reset();
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
            if (cm.strength < 0)
            {
                cm.strength = 0;
            }
        }
        private void UpdateDefense()
        {
            cm.tempDefense += dem.PendingDefense;
            if (cm.tempDefense < 0)
            {
                cm.tempDefense = 0;
            }
        }
        private void UpdateDamage(int damage) //deal [damage] damage to the enemy, checks if it is dead
        {
            cm.enemyHealth = Math.max(0, cm.enemyHealth - damage);
            if (cm.enemyHealth == 0)
            {
                Debug.Log("deafeated enemy");
                //move onto new phase or something
            }
            Debug.Log("dealt " + damage + "damage.");
        }
        private void DecayPoison() //halves poison, rounded down
        {
            cm.poison = (int) Math.Floor(cm.poison/2);
        }
    }
}
