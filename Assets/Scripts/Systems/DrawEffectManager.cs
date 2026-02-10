using UnityEngine;

// This script will contain all the On Draw effects for every card, ex: peek and what not.
namespace Systems
{
    public class DrawEffectManager : MonoBehaviour
    {   
        public CombatManager cm;
        public int PendingDamage, PendingDefense, PendingStrength, PendingPoison;
        public int AttackNum, AttackBonus; //for strength calculations
        //drawn acrds to add too??
        public void ResolveOnDraw()
        {
            ProcessMadness();
            ProcessSpecial();
            ProcessPendingStats();
            ProcessPeeking();
            // Check if Alice.freeze is 0, if yes increment madness before resolving any other on draw card effects, else lower FreezeMadness by 1.            
        }

        public void Reset() //reset all pending stats, etc. should be called at resolvestand?
        {
            PendingDamage = 0;
            PendingDefense = 0;
            PendingStrength = 0;
            PendingPoison = 0;
            AttackBonus = 0;
            AttackNum = 0;
        }

        private void ProcessMadness() //adds madness of card to player's madness
        {
            cm.madness += cm.lastDrawnCard.madness;
            if (cm.madness > cm.alice.maxMadness)
            {
                Debug.Log("Shatter!"); //return state. once is shatters need to also reset this and recalculate with just the card to keep.
            }
            else
            {
                Debug.Log("Current madness: " + cm.madness);
            }
        }
        private void ProcessSpecial() //trigger special effects based on card ID
        {
            Debug.Log("Special effect processed: " + cm.lastDrawnCard.specialID);
        }
        private void ProcessPendingStats() //calculates what the new attack, defense, etc. will be now
        {
            ProcessPendingStrength();
            ProcessPendingPoison();
            ProcessPendingDamage();
            ProcessPendingDefense();
        }
        private void ProcessPeeking() //if card allows you to peek, trigger that
        {
            Debug.Log("Peek");
        }
        private void ProcessPendingDamage() //deals with damage bonus too
        {
            PendingDamage += cm.lastDrawnCard.damage;
            AttackNum++;
            AttackBonus = AttackNum * (cm.alice.strength + PendingStrength);
            /*
            update the attack bonus, which is extra damage you can do with your strength (adds per attack)
            this updates frequently w changes in strength, including previous attacks (need to look back)
            so what is the best way ui wise to communicate this bonus??
            */
        }
        private void ProcessPendingDefense ()
        {
            PendingDefense += cm.lastDrawnCard.defense;   
        }
        private void ProcessPendingStrength()
        {
            PendingStrength += cm.lastDrawnCard.strength;
        }
        private void ProcessPendingPoison()
        {
            PendingPoison += cm.lastDrawnCard.poison;
        }
    }
}

